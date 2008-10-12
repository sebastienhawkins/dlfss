/* 
 * Copyright (C) 2008 DLFSS <http://www.lfsforum.net/when the post is created change ME>
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System.Collections.Generic;
using System.Data;
using System;
namespace Drive_LFSS.Game_
{
    using Packet_;
    using Definition_;
    using Database_;
    using Log_;
    using Script_;
    using Game_;
    using Config_;
    using PubStats_;

    internal class Race : Vote, IRace
	{
        private const int MIN_FORCED_FINISH_TIME = 40000;
        private const int MAX_FORCED_FINISH_TIME = 120000;
        private const int GTH_TIME_DIFF_FROM_GREEN_LIGHT = 16000;

        internal protected Race(Session _session)
        {
            
            iSession = _session;
        }
        ~Race()
        {
            if (true == false) { }
        }
        internal protected void ConfigApply()
        {
            base.ConfigApply();

            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "RaceSave");
            RESTART_RACE_INTERVAL = (uint)Config.GetIntValue("Race",iSession.GetSessionName() ,"AutoRestart");
            grid.ConfigApply();
        }

        internal protected void Init(PacketREO _packet)
        {
            EndRestart(); //feature auto restart
            driverGuidRESPos.Clear();
            carFinishAndLeaveTrackCount = 0;
            gridOrder = "";
            carCount = _packet.carCount;
            uint driverGuid;
            IDriver driver;
            for (byte itr = 0; itr < carCount; itr++)
            {
                driver = iSession.GetDriverWith(_packet.carIds[itr]);
                if(driver == null)
                    driverGuid = 0;
                else
                    driverGuid = driver.GetGuid();

                gridOrder += (driverGuid >= (uint)Bot_GUI.FIRST ? 0 : driverGuid) + " ";
            }

            //This is commented, since it important each value got a space after, for other function.
            //if (gridOrder.EndsWith(" "))
                //gridOrder = gridOrder.TrimEnd(' ');

            stateHasChange = true;
        } //Is the first Race/qual START Procedure
        internal protected void Init(PacketRST _packet)
        {
            timeStart = (uint)(System.DateTime.Now.Ticks / Program.tickPerMs);
            requestedFinalResultDone = false;
            finalResultCount = 0;
            finishOrder = "";

            if (trackPrefix != _packet.trackPrefix)
                finishCount = 0;
            trackPrefix = _packet.trackPrefix;
            carCount = _packet.carCount;
            nodeCount = _packet.nodeCount;
            nodeFinishIndex = _packet.nodeFinishIndex;
            raceFeatureMask = _packet.raceFeatureMask;
            qualificationMinute = _packet.qualificationMinute;
            lapCount = _packet.raceLaps;
            weatherStatus = _packet.weatherStatus;
            windStatus = _packet.windStatus;
            nodeSplit1Index = _packet.nodeSplit1Index;
            nodeSplit2Index = _packet.nodeSplit2Index;
            nodeSplit3Index = _packet.nodeSplit3Index;
            hasToBeSavedIntoPPSTA = true;

            grid.Init(nodeCount);

            if (!SetNewGuid())
                Log.error("Error when creating a new GUID for race.\r\n");
        }
        internal protected void Init(PacketSTA _packet)
        {
            if(connectionCount != _packet.connectionCount || 
                finishedCount != _packet.finishedCount ||
                carCount != _packet.carCount ||
                qualificationMinute != _packet.qualificationMinute ||
                raceInProgressStatus != (Race_In_Progress_Status)_packet.raceInProgressStatus ||
                lapCount != _packet.raceLaps ||
                trackPrefix != _packet.trackPrefix ||
                weatherStatus != (Weather_Status)_packet.weatherStatus ||
                windStatus != (Wind_Status)_packet.windStatus)
                stateHasChange = true;

            connectionCount = _packet.connectionCount;
            finishedCount = _packet.finishedCount;
            carCount = _packet.carCount;
            qualificationMinute = _packet.qualificationMinute;
            raceInProgressStatus = (Race_In_Progress_Status)_packet.raceInProgressStatus;
            lapCount = _packet.raceLaps;
            trackPrefix = _packet.trackPrefix;
            weatherStatus = (Weather_Status)_packet.weatherStatus;
            windStatus = (Wind_Status)_packet.windStatus;

            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_NONE)
                return;

            if (hasToBeSavedIntoPPSTA)
            {
                Program.dlfssDatabase.Lock();
                {
                    SaveToDB();
                }
                Program.dlfssDatabase.Unlock();
                hasToBeSavedIntoPPSTA = false;
            }
            if (guid != 0 && raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_RACING && HasAllResult())
                FinishRace();
        }
        internal protected void ProcessCarInformation(CarMotion car)
        {
            grid.ProcessCarInformation(car);
        }
        internal protected void ProcessResult(PacketRES _packet)
        {
            if(guid == 0) //No race, no result process
                return;

            if ( (_packet.requestId == 2 && raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                || _packet.confirmMask != Confirm_Flag.CONFIRM_NONE)
            {
                IDriver driver = iSession.GetDriverWith(_packet.carId);
                if(driver == null) //Probaly return from a network failure and driver got disconnected during the failure, we will cancel this result.
                    return;
                if (driverGuidRESPos.ContainsKey(driver.GetGuid()))
                    driverGuidRESPos[driver.GetGuid()] = _packet.positionFinal;
                else
                    driverGuidRESPos.Add(driver.GetGuid(),_packet.positionFinal);

                if (timeTotalFromFirstRES == 0 && guid != 0)
                {
                    timeTotalFromFirstRES = _packet.totalTime;
                    
                    uint pcDiff = GetFirstRESTimePc(5);
                    iSession.SendUpdateButtonToAll((ushort)Button_Entry.INFO_1,"^1Finish in ^7" + (pcDiff / 1000));
                    //iSession.SendMSTMessage("/rstend " + (pcDiff / 1000));
                }
                if (HasAllResult())
                    FinishRace();
            }
        }
        internal protected void ProcessCarLeaveRace(CarMotion car)
        {
            if( driverGuidRESPos.ContainsKey( ((IDriver)car).GetGuid() ) )
                carFinishAndLeaveTrackCount++;
            
            RemoveFromGrid(car);
        }
        internal protected void ProcessCarJoinRace(CarMotion car)
        {
            AddToGrid(car);
        }
        internal protected void ProcessVoteAction(Vote_Action voteAction)
        {
            #if DEBUG
            Log.debug(iSession.GetSessionNameForLog() + " Vote Action was:" + voteAction + "\r\n");
            #endif

            if (!CanVote())
            {
                SendVoteCancel();
                return;
            }
            switch (voteAction)
            {
                case Vote_Action.VOTE_RESTART:
                {
                    switch (trackChangeBeviator)
                    {
                        case Vote_Track_Change.USER:
                        case Vote_Track_Change.NO_CHANGE:
                        case Vote_Track_Change.VOTE:
                        case Vote_Track_Change.AUTO:
                        {
                            if (raceInfo.maximunFinishCount > 0 && finishCount >= raceInfo.maximunFinishCount)
                            {
                                SendVoteCancel();
                                iSession.SendMSTMessage("/end");
                            }
                        }break;
                    }
                } break;
                case Vote_Action.VOTE_QUALIFY:
                {
                    switch (trackChangeBeviator)
                    {
                        case Vote_Track_Change.USER:
                        case Vote_Track_Change.NO_CHANGE:
                        case Vote_Track_Change.VOTE:
                        case Vote_Track_Change.AUTO: break;
                    }
                } break;
                case Vote_Action.VOTE_END:
                {
                    finishCount = 0;

                    if (doEnd == true)
                        return;

                    switch (trackChangeBeviator)
                    {
                        case Vote_Track_Change.USER: break;
                        case Vote_Track_Change.NO_CHANGE:
                        {
                            SendVoteCancel();
                            iSession.SendMSTMessage("/clear");
                        } break;
                        case Vote_Track_Change.VOTE:
                        {
                            SendVoteCancel();
                            if (raceMap.Count == 0)
                            {
                                Log.error(iSession.GetSessionNameForLog() + " Vote system Error, race_map.entry=" + raceMapEntry + " is Empty, we can't initiate a vote\r\n");
                                break;
                            }
                            StartNextTrackVote();
                        } break;
                        case Vote_Track_Change.AUTO:
                        {
                            SendVoteCancel();
                            if (raceMap.Count == 0)
                            {
                                Log.error(iSession.GetSessionNameForLog() + " Vote system Error, race_map.entry=" + raceMapEntry + " is Empty, we can't auto select a track\r\n");
                                break;
                            }
                            Random randomItr = new Random();
                            voteInProgress = true;

                            List<ushort> _raceMap = GetSmartRaceMap();
                            PrepareNextTrack(_raceMap[randomItr.Next(_raceMap.Count)]);

                        } break;
                        case Vote_Track_Change.SEQUENCE:
                        {
                            SendVoteCancel();
                            if (raceMap.Count == 0)
                            {
                                Log.error(iSession.GetSessionNameForLog() + " Vote system Error, race_map.entry=" + raceMapEntry + " is Empty, we can't auto select a track\r\n");
                                break;
                            }
                            voteInProgress = true;

                            if(raceMapStaticItr < raceMap.Count)
                                PrepareNextTrack(raceMap[raceMapStaticItr++]);
                            else
                                PrepareNextTrack(raceMap[(raceMapStaticItr=0)]); 
                        } break;
                    }
                } break;
                default:
                {

                } break;
            }
        }
        internal protected void ProcessRaceEnd()
        {
            base.ProcessRaceEnd(); 

            guid = 0;
            qualifyRaceGuid = 0;
            EndRestart();
        }
        internal protected void ProcessGTH(uint time)
        {
            timeTotal = time;

            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY && (timeTotal / 6000.0d) >= qualificationMinute && HasAllResult() && !requestedFinalResultDone)
                RequestFinalQualResult();
            
            //This will bypass FinalQualifyResult.... This is Finishing a Race What Ever.
            if( timeTotalFromFirstRES > 0 )
            {
                uint pcDiff = GetFirstRESTimePc(5);
                pcDiff += GTH_TIME_DIFF_FROM_GREEN_LIGHT; //+12000 is GTH time start before Green light.
                if((timeTotalFromFirstRES+pcDiff) <= timeTotal*10)
                {
                    iSession.RemoveButtonToAll((ushort)Button_Entry.INFO_1);
                    timeTotalFromFirstRES = 0;
                    if(guid != 0)//In case we got all resulty before apply this.
                        FinishRace();
                }
                else
                    iSession.SendUpdateButtonToAll((ushort)Button_Entry.INFO_1, "^1Finish in ^7" + ((timeTotalFromFirstRES + pcDiff) - (timeTotal * 10))/1000);
            }
        }

        //LFS Insim Defined var
        private Race_Feature_Flag raceFeatureMask = Race_Feature_Flag.RACE_FLAG_NONE;
        private ushort nodeFinishIndex = 0;
        private byte connectionCount = 0;
        private byte finishedCount = 0;
        private ushort nodeCount = 0;
        private byte carCount = 0;
        private byte qualificationMinute = 0;
        private Race_In_Progress_Status raceInProgressStatus = Race_In_Progress_Status.RACE_PROGRESS_NONE;
        private byte lapCount = 0;
        private ushort nodeSplit1Index = 0;
        private ushort nodeSplit2Index = 0;
        private ushort nodeSplit3Index = 0;
        private string trackPrefix = "";
        private Weather_Status weatherStatus = Weather_Status.WEATHER_CLEAR_DAY;
        private Wind_Status windStatus = Wind_Status.WIND_NONE;
        private string gridOrder = "";
        private string finishOrder = "";
        private ushort finishCount = 0;
        private byte carFinishAndLeaveTrackCount = 0;
        Dictionary<string, int> scoringResultTextDisplay = new Dictionary<string,int>();
        //
        private uint timeTotal = 0;
        private Grid grid = new Grid();
        private bool requestedFinalResultDone = false;
        private byte finalResultCount = 0;
        private bool stateHasChange = false;
        private bool hasToBeSavedIntoPPSTA = false;
        private uint guid = 0;
        private uint qualifyRaceGuid = 0;
        private ISession iSession;
        private uint timeStart = 0;
        //This is too keep pos of live value and not accurate
        //private byte[] positionToCarId = new byte[(int)PositionIndex.POSITION_LAST+1]; //index 0, mean nothing and index 193 mean Nothing too.
        private Dictionary<uint, byte> driverGuidRESPos = new Dictionary<uint, byte>(); //index 0, mean nothing and index 193 mean Nothing too.
        private bool triggerRaceRestart = false; 
        private uint RESTART_RACE_INTERVAL = 0;
        private uint timerRaceRestart = 0;

        private static uint SAVE_INTERVAL = 5000;
        private uint raceSaveInterval = 0;
        private uint requestGTHInterval = 0;
        private uint advertRaceRestart = 0;
        private uint timeTotalFromFirstRES = 0;

        public void update(uint diff)
        {
            base.update(diff);

            if (raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_NONE)
            {
                if (guid != 0 )
                {
                    if(stateHasChange)
                    {
                        if ((raceSaveInterval += diff) >= SAVE_INTERVAL)
                        {
                            Program.dlfssDatabase.Lock();
                            {
                                SaveToDB();
                            }
                            Program.dlfssDatabase.Unlock();
                        }
                    }
                }
                
 
                if (requestGTHInterval < diff)
                {
                    PacketTiny packetTiny = new PacketTiny(1, Tiny_Type.TINY_GTH);
                    Packet packet = new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, packetTiny);
                    ((Session)iSession).AddToTcpSendingQueud(packet);
                    requestGTHInterval = 5000;
                }
                else
                    requestGTHInterval -= diff;
            

                //Feature Auto restart
                if(timerRaceRestart > 0)
                {
                    if(diff >= timerRaceRestart)
                    {
                        EndRestart();
                        ExecRestart();
                    }
                    else
                    {
                        timerRaceRestart -= diff;
                        if (diff > advertRaceRestart)
                        {
                            advertRaceRestart = 300;
                            iSession.SendUpdateButtonToAll((ushort)Button_Entry.INFO_1, "^2Restart in ^7" + Math.Round((decimal)(timerRaceRestart / 1000.00d), 1).ToString());
                        }
                        else
                            advertRaceRestart -=diff;
                    }
                }
            }
        }
        private bool SetNewGuid()
        {
            bool returnValue = false;
            Program.dlfssDatabase.Lock();
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `race`");
                if (reader.Read())
                {
                    guid = reader.IsDBNull(0) ? 1 : (uint)reader.GetInt32(0) + 1;
                    returnValue = true;
                    reader.Dispose();
                    SaveToDB();
                }
            }
            Program.dlfssDatabase.Unlock();
            return returnValue;
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `race` WHERE `guid`=" + guid);
            string query = "INSERT INTO `race` (`guid`,`qualify_race_guid`,`track_prefix`,`start_timestamp`,`end_timestamp`,`grid_order`,`finish_order`,`race_laps`,`race_status`,`race_feature`,`qualification_minute`,`weather_status`,`wind_status`)";
            query += "VALUES(" + guid + "," + qualifyRaceGuid + ", '" + trackPrefix + "'," + (System.DateTime.Now.Ticks / (Program.tickPerMs * 1000)) + ", " + 0 + ", '" + gridOrder + "', '" + finishOrder + "'," + lapCount + ", " + (byte)raceInProgressStatus + "," + (byte)raceFeatureMask + "," + qualificationMinute + "," + (byte)weatherStatus + "," + (byte)windStatus + ")";
            //Since this is GUID creation, important 1 at time is created.
            Program.dlfssDatabase.ExecuteNonQuery(query);
            raceSaveInterval = 0;
            stateHasChange = false;
            Log.database(iSession.GetSessionNameForLog() + " RaceGuid: " + guid + ", TrackPrefix: " + trackPrefix + ", raceLaps: " + lapCount + ", saved to database.\r\n");
        }
        
        public ISession ISession
        {
            get {return iSession;}
        }
        public bool CanVote()
        {
            return (timeTotalFromFirstRES == 0 && !IsVoteInProgress());
        }
        private void AddToGrid(CarMotion car)
        {
            grid.Add(car);
        }
        private void RemoveFromGrid(CarMotion car)
        {
            grid.Remove(car);
        }
        internal protected uint GetGuid()
        {
            return guid;
        }
        public string GetTrackPrefix()
        {
            return trackPrefix;
        }
        public bool IsRaceInProgress()
        {
            return (raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_NONE);
        }
        private void FinishRace()
        {
            timeTotalFromFirstRES = 0;

            Log.debug("Race: " + guid + ", was finished successfully.\r\n");

             {//Win Scoring
                Dictionary<uint,byte>.Enumerator enu = driverGuidRESPos.GetEnumerator();
                scoringResultTextDisplay.Clear();
                int driverCount = gridOrder.Split(new char[]{' '}).Length-1;
                bool goodResult = false;
                for(byte itr = 0; itr < 255; itr++)
                {
                    while(enu.MoveNext())
                    {
                        if(enu.Current.Value == itr)
                        {
                            finishOrder += (enu.Current.Key >= (uint)Bot_GUI.FIRST ? 0 : enu.Current.Key) + " ";
                            double winScore = 0.0d;
                            IDriver driver = iSession.GetDriverWithGuid(enu.Current.Key);
                            if (driver != null && driver.GetCurrentWRTime() != 0)
                            {
                                //int posScore = (driverCount / 2) - itr + 1;//+1 is position 0 become 1 ...
                                int averageLapTime = (int)(driver.TimeTotalLastRace / driver.LapCountTotalLastRace);
                                int bestEver = (int)driver.GetCurrentWRTime();

				                winScore = driverCount + ((bestEver-averageLapTime)/bestEver*100.0d)-5; // -5 , is the min factor
					            winScore += driverCount-itr+1;
                                if (winScore < 0)
                                    winScore = 0;
					            //if(driverCount > 1 && itr==0)
						        //   winScore = winScore+1;
                                goodResult = true;
                                scoringResultTextDisplay.Add("^7"+(itr+1).ToString() + "^2-^7 "+driver.DriverName,(int)Math.Round(winScore,0));
                            }
                            else
                                scoringResultTextDisplay.Add((itr + 1).ToString() + "- ^1Driver Leaved", 0);
                            break;
                        }
                    }
                    enu = driverGuidRESPos.GetEnumerator();
                }
                if(goodResult)
                    iSession.SendResultGuiToAll(scoringResultTextDisplay);
            }
            Program.dlfssDatabase.Lock();
            {
                SaveToDB();
            }
            Program.dlfssDatabase.Unlock();
            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                qualifyRaceGuid = guid;
            else
                finishCount++;
            guid = 0;
            
            if (iSession.Script.RaceCompleted(gridOrder,finishOrder))
                return;
            
            //Qualify need better support on finish before i can add this.
            //timerRaceRestart , should not be added into the if, since this should not be called twice... let see after qual fix.
            if (RESTART_RACE_INTERVAL > 0)
                StartRestart();
        }            //Is The Finish Race Procudure(A Sucess CompletedRace)
        private void RequestFinalQualResult()
        {
            requestedFinalResultDone = true;
            ((Session)iSession).AddToTcpSendingQueud
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_TINY,
                    Packet_Type.PACKET_TINY_MULTI_PURPOSE,
                    new PacketTiny(2, Tiny_Type.TINY_RES)
                )
            );
        }
        private bool HasAllResult()
        {

            return (driverGuidRESPos.Count - carFinishAndLeaveTrackCount >= carCount && carCount > 0);
        }
        private uint GetFirstRESTimePc(uint percentage)
        {
            uint pcDiff = timeTotalFromFirstRES / 100 * percentage + (MIN_FORCED_FINISH_TIME / 2);
            if (pcDiff < MIN_FORCED_FINISH_TIME) //Minimal Race Force Finish
                pcDiff = MIN_FORCED_FINISH_TIME;
            else if (pcDiff > MAX_FORCED_FINISH_TIME) //Maximal Race Force Finish
                pcDiff = MAX_FORCED_FINISH_TIME;

            return pcDiff;
        }
        internal protected Dictionary<string,int> GetLastResultString()
        {
            return scoringResultTextDisplay;
        }
        //Feature auto restart
        private void StartRestart()
        {
            #if DEBUG
            Log.debug(iSession.GetSessionNameForLog() + " StartRestart(), has been launched with  '" + RESTART_RACE_INTERVAL + "' to go.\r\n");
            #endif
            timerRaceRestart = RESTART_RACE_INTERVAL;
            //iSession.AddMessageTopToAll("^2Race will restart in ^7" + RESTART_RACE_INTERVAL / 1000 + " ^2sec", (3000 > RESTART_RACE_INTERVAL ? RESTART_RACE_INTERVAL : 3000));
        }
        private void ExecRestart()
        {
            #if DEBUG
            Log.debug(iSession.GetSessionNameForLog() + " ExecRestart(), Exec /restart.\r\n");
            #endif
            iSession.SendMSTMessage("/restart");
        }
        private void EndRestart()
        {
            if (timerRaceRestart > 0)
            {
                #if DEBUG
                Log.debug(iSession.GetSessionNameForLog()+" EndRestart(), was ended sucess.\r\n");
                #endif
                iSession.RemoveButtonToAll((ushort)Button_Entry.INFO_1);
                timerRaceRestart = 0;
            }
        }
    }
}
