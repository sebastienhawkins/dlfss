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
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Database_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Script_;
    using Drive_LFSS.Game_;
    using Drive_LFSS.Config_;

    public enum PositionIndex : byte
    {
        POSITION_FIRST = 0,
        POSITION_LAST = 255,
    }
    public sealed class Race : IRace
	{
        public Race(Session _session): base()
        {
            iSession = _session;
        }
        ~Race()
        {
            if (true == false) { }
        }
        public void ConfigApply()
        {
            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "RaceSave");
            RACE_RESTART_INTERVAL = (uint)Config.GetIntValue("Race",iSession.GetSessionName() ,"AutoRestart");
            grid.ConfigApply();
        }

        public void Init(PacketREO _packet)
        {
            EndRestart();
            for (byte itr = (byte)PositionIndex.POSITION_FIRST; itr < (byte)PositionIndex.POSITION_LAST; )
                carPosition[itr++] = 0;

            gridOrder = "";
            carCount = _packet.carCount;
            for (byte itr = 0; itr < carCount; itr++)
            {
                gridOrder += _packet.carIds[itr] + " ";
                carPosition[itr] = _packet.carIds[itr];
            }
            if (gridOrder.EndsWith(" "))
                gridOrder = gridOrder.TrimEnd(' ');

            stateHasChange = true;
        } //Is the first RaceSTART Procedure
        public void Init(PacketRST _packet)
        {
            EndRestart();
            timeStart = (uint)(System.DateTime.Now.Ticks / Program.tickPerMs);
            requestedFinalResultDone = false;
            finalResultCount = 0;
            finishOrder = "";

            trackPrefix = _packet.trackName;
            carCount = _packet.carCount;
            nodeCount = _packet.nodeCount;
            nodeFinishIndex = _packet.nodeFinishIndex;
            raceFeatureMask = _packet.raceFeatureMask;
            qualificationMinute = _packet.qualificationMinute;
            raceLaps = _packet.raceLaps;
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
        public void Init(PacketSTA _packet)
        {
            if(connectionCount != _packet.connectionCount || 
                finishedCount != _packet.finishedCount ||
                carCount != _packet.carCount ||
                qualificationMinute != _packet.qualificationMinute ||
                raceInProgressStatus != (Race_In_Progress_Status)_packet.raceInProgressStatus ||
                raceLaps != _packet.raceLaps ||
                trackPrefix != _packet.trackPrefix ||
                weatherStatus != (Weather_Status)_packet.weatherStatus ||
                windStatus != (Wind_Status)_packet.windStatus)
                stateHasChange = true;

            connectionCount = _packet.connectionCount;
            finishedCount = _packet.finishedCount;
            carCount = _packet.carCount;
            qualificationMinute = _packet.qualificationMinute;
            raceInProgressStatus = (Race_In_Progress_Status)_packet.raceInProgressStatus;
            raceLaps = _packet.raceLaps;
            trackPrefix = _packet.trackPrefix;
            weatherStatus = (Weather_Status)_packet.weatherStatus;
            windStatus = (Wind_Status)_packet.windStatus;

            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_NONE)
                return;

            if (hasToBeSavedIntoPPSTA)
            {
                lock (Program.dlfssDatabase)
                {
                    SaveToDB();
                }
                hasToBeSavedIntoPPSTA = false;
            }
            if (finishedCount >= carCount && !requestedFinalResultDone) //This is not working good in all condition during qualify
            {
                RequestFinalResult();
            }
        }
        public void ProcessCarInformation(CarMotion car)
        {
            if (car.GetRacePosition() == 0 || car.GetRacePosition() == 255) //Position is Unknow for that Car.
                return;
            SetCarPosition(car.CarId, (byte)(car.GetRacePosition() - 1));
            grid.ProcessCarInformation(car);
        }
        public void ProcessResult(PacketRES _packet)
        {
            if(guid == 0)
                return;

            SetCarPosition(_packet.carId, _packet.positionFinal);

            if ( (_packet.requestId == 2 && raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                || _packet.confirmMask == Confirm_Flag.CONFIRM_CONFIRMED)
            {
                ++finalResultCount;
                if (finalResultCount >= carCount) //This is bad
                    FinishRace();
            }
        }
        public void ProcessResult(PacketFIN _packet)
        {
            //In fact this packet is more needed into Lap/Driver
        } //This confirm a race Finished
        public void ProcessRaceEnd()
        {
            guid = 0;
            qualifyRaceGuid = 0;
            EndRestart();
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
        private byte raceLaps = 0;
        private ushort nodeSplit1Index = 0;
        private ushort nodeSplit2Index = 0;
        private ushort nodeSplit3Index = 0;
        private string trackPrefix = "";
        private Weather_Status weatherStatus = Weather_Status.WEATHER_CLEAR_DAY;
        private Wind_Status windStatus = Wind_Status.WIND_NONE;
        private string gridOrder = "";
        private string finishOrder = "";
        //
        private Grid grid = new Grid();
        private bool requestedFinalResultDone = false;
        private byte finalResultCount = 0;
        private bool stateHasChange = false;
        private bool hasToBeSavedIntoPPSTA = false;
        private uint guid = 0;
        private uint qualifyRaceGuid = 0;
        private ISession iSession;
        private uint timeStart = 0;
        private byte[] carPosition = new byte[(int)PositionIndex.POSITION_LAST+1]; //index 0, mean nothing and index 193 mean Nothing too.
        private bool triggerRaceRestart = false; 
        private uint RACE_RESTART_INTERVAL = 0;
        private uint timerRaceRestart = 0;

        private static uint SAVE_INTERVAL = 5000;
        private uint raceSaveInterval = 0;
        uint advertRaceRestart = 0;
        public void update(uint diff)
        {

            if (raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_NONE)
            {
                if (guid != 0 )
                {
                    if(stateHasChange)
                    {
                        if ((raceSaveInterval += diff) >= SAVE_INTERVAL)
                            SaveToDB();
                    }
                }
            }
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
        
        public void AddToGrid(CarMotion car)
        {
            grid.Add(car);
        }
        public void RemoveFromGrid(CarMotion car)
        {
            grid.Remove(car);
        }
        public uint GetGuid()
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
            Log.debug("Race: " + guid + ", was finished successfully.\r\n");

            for (byte itr = (byte)PositionIndex.POSITION_FIRST; itr < finishedCount; itr++)
                finishOrder += carPosition[itr]+" ";

            if (finishOrder.EndsWith(" "))
                finishOrder = finishOrder.TrimEnd(' ');

            if (guid != 0)
            {
                lock (Program.dlfssDatabase)
                {
                    SaveToDB();
                }
            }

            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                qualifyRaceGuid = guid;
            guid = 0;

            if (iSession.Script.RaceCompleted(gridOrder,finishOrder))
                return;
            
            //Qualify need better support on finish before i can add this.
            //timerRaceRestart , should not be added into the if, since this should not be called twice... let see after qual fix.
            if (RACE_RESTART_INTERVAL > 0 && timerRaceRestart == 0 && raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_RACING)
            {
                StartRestart();
            }
        }            //Is The Finish Race Procudure(A Sucess CompletedRace)

        private void SetCarPosition(byte _carId, byte _position)
        {
            if (carPosition[_position] == _carId)   //Same Position
                return;
            else if (carPosition[_position] == 0)   //Not same position, but don't take positon of anyone... a weird case
            {
                carPosition[_position] = _carId;
                Log.error("Race.SetCarPosition(), Weird case happened: CarId: " + _carId + ", PositionAsked:" + _position + ", the old position carId was 0.\r\n");
                return;
            }

            byte oldCarId = carPosition[_position];
            byte oldPosition = FindCarPosition(_carId);
            if (oldPosition != (byte)PositionIndex.POSITION_LAST)
                carPosition[oldPosition] = oldCarId;
            carPosition[_position] = _carId;

            //Log.progress("Race.SetCarPosition(),  CarId: " + _carId + ", PositionAsked:" + _position + ", the old CarID/Position:" + oldCarId + "/" + oldPosition + ".\r\n");
        }
        private byte FindCarPosition(byte _carId)
        {
            byte itr;
            for (itr = (byte)PositionIndex.POSITION_FIRST; carPosition[itr] != _carId && itr < (byte)PositionIndex.POSITION_LAST; itr++) ;
            return itr;
        }
        private bool SetNewGuid()
        {
            bool returnValue = false;
            lock (Program.dlfssDatabase)
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `race`");
                if (reader.Read())
                {
                    guid = reader.IsDBNull(0) ? 1 : (uint)reader.GetInt32(0) + 1;
                    returnValue = true;
                    reader.Close();
                    SaveToDB();
                }
                reader.Dispose();
            }
            return returnValue;
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `race` WHERE `guid`=" + guid);
            string query = "INSERT INTO `race` (`guid`,`qualify_race_guid`,`track_prefix`,`start_timestamp`,`end_timestamp`,`grid_order`,`finish_order`,`race_laps`,`race_status`,`race_feature`,`qualification_minute`,`weather_status`,`wind_status`)";
            query += "VALUES(" + guid + "," + qualifyRaceGuid + ", '" + trackPrefix + "'," + (System.DateTime.Now.Ticks / (Program.tickPerMs*1000)) + ", " + 0 + ", '" + gridOrder + "', '" + finishOrder + "'," + raceLaps + ", " + (byte)raceInProgressStatus + "," + (byte)raceFeatureMask + "," + qualificationMinute + "," + (byte)weatherStatus + "," + (byte)windStatus + ")";
            //Since this is GUID creation, important 1 at time is created.
            Program.dlfssDatabase.ExecuteNonQuery(query);

            raceSaveInterval = 0;
            stateHasChange = false;
            Log.database(iSession.GetSessionNameForLog() + " RaceGuid: " + guid + ", TrackPrefix: " + trackPrefix + ", raceLaps: " + raceLaps + ", saved to database.\r\n");
        }
        private void RequestFinalResult()
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
        
        private void StartRestart()
        {
            Log.debug(iSession.GetSessionNameForLog() + " StartRestart(), has been launched with  '" + RACE_RESTART_INTERVAL + "' to go.\r\n");
            timerRaceRestart = RACE_RESTART_INTERVAL;
            iSession.AddMessageMiddleToAll("^2Race will restart in ^7" + RACE_RESTART_INTERVAL / 1000 + " ^2sec , if no other action.", (4500 > RACE_RESTART_INTERVAL ? RACE_RESTART_INTERVAL : 4500));
        }
        private void ExecRestart()
        {
            Log.debug(iSession.GetSessionNameForLog() + " ExecRestart(), Exec /restart.\r\n");
            iSession.SendMSTMessage("/restart");
        }
        private void EndRestart()
        {
            if (timerRaceRestart > 0)
            {
                Log.debug(iSession.GetSessionNameForLog()+" EndRestart(), was ended sucess.\r\n");
                iSession.RemoveButtonToAll((ushort)Button_Entry.INFO_1);
                timerRaceRestart = 0;
            }
        }
    }
}
