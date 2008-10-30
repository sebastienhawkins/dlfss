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

    class Race : Track, IRace
	{
        private const int MIN_FORCED_FINISH_TIME = 40000;
        private const int MAX_FORCED_FINISH_TIME = 120000;
        private const int PCT_FORCED_FINISH_TIME = 8;
        private const int GTH_TIME_DIFF_FROM_GREEN_LIGHT = 16000;

        internal Race(Session session) : base()
        {
            iSession = session;
        }
        ~Race()
        {
            if (true == false) { }
        }
        new internal void ConfigApply()
        {
            base.ConfigApply();

            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "RaceSave");
            RESTART_RACE_INTERVAL = (uint)Config.GetIntValue("Race",iSession.GetSessionName() ,"AutoRestart");
            grid.ConfigApply();
        }

        internal void Init(PacketREO packet)
        {
            EndAutoRestart(); //feature auto restart
            driverToPosition.Clear();
            lock(carWeWaitForFinish){carWeWaitForFinish.Clear();}
            gridOrder = "";
            carCount = packet.carCount;
            uint driverGuid;
            IDriver driver;
            for (byte itr = 0; itr < carCount; itr++)
            {
                driver = iSession.GetCarId(packet.carIds[itr]);
                if(driver == null)
                    driverGuid = 0;
                else
                {
                    lock (carWeWaitForFinish) { carWeWaitForFinish.Add(packet.carIds[itr]); }
                    driverGuid = driver.GetGuid();
                }

                gridOrder += (driverGuid >= (uint)Bot_GUI.FIRST ? 0 : driverGuid) + " ";
            }

            //This is commented, since it important each value got a space after, for other function.
            //if (gridOrder.EndsWith(" "))
                //gridOrder = gridOrder.TrimEnd(' ');

            stateHasChange = true;
        } //Is the first Race/qual START Procedure
        internal void Init(PacketRST packet)
        {
            timeStart = (uint)(System.DateTime.Now.Ticks / Program.tickPerMs);
            requestedFinalResultDone = false;
            finishOrder = "";

            if (trackPrefix != packet.trackPrefix)
                finishCount = 0;
            trackPrefix = packet.trackPrefix;
            carCount = packet.carCount;
            nodeCount = packet.nodeCount;
            nodeFinishIndex = packet.nodeFinishIndex;
            raceFeatureMask = packet.raceFeatureMask;
            qualificationMinute = packet.qualificationMinute;
            lapCount = packet.raceLaps;
            weatherStatus = packet.weatherStatus;
            windStatus = packet.windStatus;
            nodeSplit1Index = packet.nodeSplit1Index;
            nodeSplit2Index = packet.nodeSplit2Index;
            nodeSplit3Index = packet.nodeSplit3Index;
            hasToBeSavedIntoPPSTA = true;

            grid.Init(nodeCount);

            if (!SetNewGuid())
                Log.error("Error when creating a new GUID for race.\r\n");
        }
        internal void Init(PacketSTA packet)
        {
            if(connectionCount != packet.connectionCount || 
                finishedCount != packet.finishedCount ||
                carCount != packet.carCount ||
                qualificationMinute != packet.qualificationMinute ||
                raceInProgressStatus != (Race_In_Progress_Status)packet.raceInProgressStatus ||
                lapCount != packet.lapCount ||
                trackPrefix != packet.trackPrefix ||
                weatherStatus != (Weather_Status)packet.weatherStatus ||
                windStatus != (Wind_Status)packet.windStatus)
                stateHasChange = true;

            connectionCount = packet.connectionCount;
            finishedCount = packet.finishedCount;
            carCount = packet.carCount;
            qualificationMinute = packet.qualificationMinute;
            raceInProgressStatus = (Race_In_Progress_Status)packet.raceInProgressStatus;
            lapCount = packet.lapCount;
            trackPrefix = packet.trackPrefix;
            weatherStatus = (Weather_Status)packet.weatherStatus;
            windStatus = (Wind_Status)packet.windStatus;

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
        internal void ProcessCarInformation(CarMotion car)
        {
            grid.ProcessCarInformation(car);
        }
        internal void ProcessResult(PacketRES packet)
        {
            if(guid == 0) //No race, no result process
                return;

            if ( (packet.requestId == 2 && raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                || (packet.confirmMask != Confirm_Flag.CONFIRM_NONE && packet.lapCount >= lapCount))
            {
                lock (carWeWaitForFinish)
                {
                    if(carWeWaitForFinish.Contains(packet.carId))
                        carWeWaitForFinish.Remove(packet.carId);
                }
                    
                IDriver driver = iSession.GetCarId(packet.carId);
                if(driver == null) //Probaly return from a network failure and driver got disconnected during the failure, we will cancel this result.
                    return;

                ((Button)driver).SendFlagRace((ushort)Gui_Entry.FLAG_RACE_END, 5000);
                
                if (driverToPosition.ContainsKey(driver.GetGuid()))
                    driverToPosition[driver.GetGuid()] = packet.positionFinal;
                else
                    driverToPosition.Add(driver.GetGuid(),packet.positionFinal);

                if (timeTotalFromFirstRES == 0 && guid != 0)
                {
                    timeTotalFromFirstRES = packet.totalTime+1;

                    uint pcDiff = GetFirstRESTimePc(PCT_FORCED_FINISH_TIME);
                    iSession.SendUpdateButtonToAll((ushort)Button_Entry.INFO_1,"^1Finish in ^7" + (pcDiff / 1000));
                    //iSession.SendMSTMessage("/rstend " + (pcDiff / 1000));
                }
                if (HasAllResult() && raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                    FinishRace();
            }
        }
        internal void ProcessCarLeaveRace(CarMotion car)
        {
            lock(carWeWaitForFinish)
            {
                if( carWeWaitForFinish.Contains(car.CarId) )
                    carWeWaitForFinish.Remove(car.CarId);
            }
            RemoveFromGrid(car);
        }
        internal void ProcessCarJoinRace(CarMotion car)
        {
            AddToGrid(car);
        }
        internal void ProcessVoteAction(Vote_Action voteAction)
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
                            if(raceInfo.maximunFinishCount > 0 && finishCount >= raceInfo.maximunFinishCount)
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
        new internal void ProcessRaceEnd()
        {
            base.ProcessRaceEnd(); 

            guid = 0;
            qualifyRaceGuid = 0;
            EndAutoRestart();
        }
        internal void ProcessGTH(uint time)
        {
            timeTotal = time;

            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY && (timeTotal / 6000.0d) >= qualificationMinute && !requestedFinalResultDone)
                RequestFinalQualResult();
            else if (requestedFinalResultDone && timeTotalFromFirstRES == 0 && guid != 0)
            {    
                timeTotalFromFirstRES = 1;
                uint pcDiff = GetFirstRESTimePc(PCT_FORCED_FINISH_TIME);
                iSession.SendUpdateButtonToAll((ushort)Button_Entry.INFO_1, "^1Finish in ^7" + (pcDiff / 1000));
            }
            //This will bypass FinalQualifyResult.... This is Finishing a Race What Ever.
            if( timeTotalFromFirstRES > 0 )
            {
                uint pcDiff = GetFirstRESTimePc(PCT_FORCED_FINISH_TIME);
                pcDiff += GTH_TIME_DIFF_FROM_GREEN_LIGHT; //+12000 is GTH time start before Green light.
                if((timeTotalFromFirstRES+pcDiff) <= timeTotal*10)
                {
                    iSession.RemoveButtonToAll((ushort)Button_Entry.INFO_1);
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
        Dictionary<string, int> scoringResultTextDisplay = new Dictionary<string,int>();
        private uint timeTotal = 0;
        private Grid grid = new Grid();
        private bool requestedFinalResultDone = false;
        private bool stateHasChange = false;
        private bool hasToBeSavedIntoPPSTA = false;
        private uint guid = 0;
        private uint qualifyRaceGuid = 0;
        private ISession iSession;
        private uint timeStart = 0;
        private Dictionary<uint, byte> driverToPosition = new Dictionary<uint, byte>(); //index 0, mean nothing and index 193 mean Nothing too.
        private List<byte> carWeWaitForFinish = new List<byte>();
        
        private uint RESTART_RACE_INTERVAL = 0;
        private uint timerRaceRestart = 0;
        private static uint SAVE_INTERVAL = 5000;
        private uint raceSaveInterval = 0;
        private uint requestGTHInterval = 0;
        private uint advertRaceRestart = 0;
        private uint timeTotalFromFirstRES = 0;
        new public void update(uint diff)
        {
            base.update(diff);

            if (raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_NONE)
            {
                if (guid != 0 )
                {
                    if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_RACING && HasAllResult())
                        FinishRace();
 
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
                    requestGTHInterval = 1000;
                }
                else
                    requestGTHInterval -= diff;
            

                //Feature Auto restart
                if(timerRaceRestart > 0)
                {
                    if(diff >= timerRaceRestart)
                    {
                        EndAutoRestart();
                        ExecAutoRestart();
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
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `race` WHERE `guid`='" + guid+"'");
            string query = "INSERT INTO `race` (`guid`,`qualify_race_guid`,`track_prefix`,`start_timestamp`,`end_timestamp`,`grid_order`,`finish_order`,`race_laps`,`race_status`,`race_feature`,`qualification_minute`,`weather_status`,`wind_status`)";
            query += "VALUES('" + guid + "','" + qualifyRaceGuid + "', '" + trackPrefix + "','" + ((UInt64)new TimeSpan(System.DateTime.Now.Ticks).TotalMilliseconds) + "','" + 0 + "','" + gridOrder + "','" + finishOrder + "','" + lapCount + "','" + (byte)raceInProgressStatus + "','" + (byte)raceFeatureMask + "','" + qualificationMinute + "','" + (byte)weatherStatus + "','" + (byte)windStatus + "')";
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
        public byte GetLapCount()
        {
            return lapCount;
        }
        private void AddToGrid(CarMotion car)
        {
            grid.Add(car);
        }
        private void RemoveFromGrid(CarMotion car)
        {
            grid.Remove(car);
        }
        private void StartAutoRestart()
        {
            #if DEBUG
            Log.debug(iSession.GetSessionNameForLog() + " StartRestart(), has been launched with  '" + RESTART_RACE_INTERVAL + "' to go.\r\n");
            #endif
            timerRaceRestart = RESTART_RACE_INTERVAL;
            //iSession.SendFlagRaceToAll((ushort)Gui_Entry.FLAG_GREEN, RESTART_RACE_INTERVAL + 12000);
            //iSession.AddMessageTopToAll("^2Race will restart in ^7" + RESTART_RACE_INTERVAL / 1000 + " ^2sec", (3000 > RESTART_RACE_INTERVAL ? RESTART_RACE_INTERVAL : 3000));
        }
        private void ExecAutoRestart()
        {
            #if DEBUG
            Log.debug(iSession.GetSessionNameForLog() + " ExecRestart(), Exec /restart.\r\n");
            #endif
            if (raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_QUALIFY && (raceInfo.QualifyMinute > 0 || qualificationMinute > 0))
                iSession.SendMSTMessage("/qualify");
            else
                iSession.SendMSTMessage("/restart");

            //iSession.RemoveFlagRaceToAll((ushort)Gui_Entry.FLAG_BLACK_NO_SCORE);
            //iSession.SendFlagRaceToAll((ushort)Gui_Entry.FLAG_PIT_CLOSE, 12000);
        }
        private void EndAutoRestart()
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
        internal uint GetGuid()
        {
            return guid;
        }
        public byte GetCarCount()
        {
            return carCount;
        }
        public string GetTrackPrefix()
        {
            return trackPrefix;
        }
        public bool IsRaceInProgress()
        {
            return (raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_NONE);
        }
        public bool StartGridHasDriverGuid(uint driverGuid)
        {
            if(gridOrder.IndexOf(driverGuid+" ") != -1)
                return true;
            return false;
        }
        private void FinishRace()
        {
            if (timeTotalFromFirstRES == 0) //This can be multithread call, so we ensure the other thread exit.
                return;

            //iSession.SendFlagRaceToAll((ushort)Gui_Entry.FLAG_BLACK_NO_SCORE, 5000);
            timeTotalFromFirstRES = 0;
            iSession.RemoveButtonToAll((ushort)Button_Entry.INFO_1);
            Log.debug("Race: " + guid + ", was finished successfully.\r\n");

            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_RACING)
            {//Win Scoring
                Dictionary<uint,byte>.Enumerator enu = driverToPosition.GetEnumerator();
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
                            if (driver != null)
                            {
                                driver.FinishRace();
                                if(driver.GetCurrentWRTime() != 0 && driver.LapCountTotalLastRace > 0)
                                {
                                    //int posScore = (driverCount / 2) - itr + 1;//+1 is position 0 become 1 ...
                                    double averageLapTime = driver.TimeTotalLastRace / driver.LapCountTotalLastRace;
                                    double bestEver = driver.GetCurrentWRTime() + 3000.0d;
                                    double lapRatio = (((driver.LapCountTotalLastRace+20.0d)/2.0d)*20.0d/100.0d)-2.0d;
                                    
				                    winScore = (driverCount + ((bestEver-averageLapTime)/bestEver*100.0d)-5.0d)+lapRatio; // -5 , is the min factor
					                winScore += driverCount-itr+1;
                                    if (winScore < 0)
                                        winScore = 0;
					                //if(driverCount > 1 && itr==0)
						            //   winScore = winScore+1;
                                    goodResult = true;
                                    scoringResultTextDisplay.Add("^7"+(itr+1).ToString() + "^2-^7 "+driver.DriverName,(int)Math.Round(winScore,0));
                                }
                                else
                                    scoringResultTextDisplay.Add("^7" + (itr + 1).ToString() + "^2-^7 " + driver.DriverName, 0);   
                            }
                            else
                                scoringResultTextDisplay.Add((itr + 1).ToString() + "- ^1Driver Leaved", 0);
                            break;
                        }
                    }
                    enu = driverToPosition.GetEnumerator();
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
                StartAutoRestart();
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
            return (carWeWaitForFinish.Count == 0 && carCount > 0);
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
        internal Dictionary<string,int> GetLastResultString()
        {
            return new Dictionary<string,int>(scoringResultTextDisplay);
        }
        public bool IsStillRacing(byte carId)
        {
            return carWeWaitForFinish.Contains(carId);
        }
        public bool HasRacerOn()
        {
            return carWeWaitForFinish.Count > 0 ? true : false;
        }
        new internal void EndRace()
        {
            base.EndRace();
        }
    }
}
