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
    using Drive_LFSS.Session_;
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
            session = _session;
        }
        ~Race()
        {
            if (true == false) { }
        }
        public static void ConfigApply()
        {
            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "RaceSave");
        }

        public void Init(PacketREO _packet)
        {
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
        } //Is the Main RaceSTART Procedure
        public void Init(PacketRST _packet)
        {
            timeStart = (uint)(System.DateTime.Now.Ticks / 10000);
            requestedFinalResult = false;
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

            grid = new Grid(nodeCount);

            if (!SetNewGuid())
                Log.error("Error When Creating a New GUID for Race.\r\n");
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
                SaveToDB();
                hasToBeSavedIntoPPSTA = false;
            }
            if (finishedCount >= carCount && !requestedFinalResult) //This is not working good in all condition during qualify
            {
                RequestFinalResult();
            }
        }
        public void ProcessCarInformation(Car car)
        {
            if (car.GetRacePosition() == 0 || car.GetRacePosition() == 255) //Position is Unknow for that Car.
                return;
            SetCarPosition(car.CarId, (byte)(car.GetRacePosition() - 1));
            grid.ProcessCarInformation((CarMotion)car);
        }
        public void ProcessResult(PacketRES _packet)
        {
            SetCarPosition(_packet.carId, _packet.positionFinal);

            if ((_packet.requestId == 1 && raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY ) 
                || _packet.confirmMask == Confirm_Flag.CONFIRM_CONFIRMED)
            {
                ++finalResultCount;
                if(finalResultCount == (finishedCount))
                    FinishRace();
            }
        }
        public void ProcessResult(PacketFIN _packet)
        {
            //In fact this packet is more needed into Lap/Driver
        }
        public void ProcessRaceEnd()
        {
            guid = 0;
            qualifyRaceGuid = 0;
        }
        
        //LFS Insim Defined var
        private Race_Feature_Flag raceFeatureMask = (Race_Feature_Flag)0;
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
        private Grid grid = new Grid(0);
        private bool requestedFinalResult = false;
        private byte finalResultCount = 0;
        private bool stateHasChange = false;
        private bool hasToBeSavedIntoPPSTA = false;
        private uint guid = 0;
        private uint qualifyRaceGuid = 0;
        private Session session;
        private uint timeStart = 0;
        private byte[] carPosition = new byte[(int)PositionIndex.POSITION_LAST+1]; //index 0, mean nothing and index 193 mean Nothing too.

        private static uint SAVE_INTERVAL = 5000;
        private uint raceSaveInterval = 0;
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
            Log.debug("Race: " + guid + ", was Finished SucessFull.\r\n");

            for (byte itr = (byte)PositionIndex.POSITION_FIRST; itr < finishedCount; itr++)
                finishOrder += carPosition[itr]+" ";

            if (finishOrder.EndsWith(" "))
                finishOrder = finishOrder.TrimEnd(' ');

            if (guid != 0)
                SaveToDB();

            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                qualifyRaceGuid = guid;
            guid = 0;
        }            //Is The Finish Race Procudure(A Sucess CompletedRace)

        private void SetCarPosition(byte _carId, byte _position)
        {
            if (carPosition[_position] == _carId)   //Same Position
                return;
            else if (carPosition[_position] == 0)   //Not same position, but don't take positon of anyone... a weird case
            {
                carPosition[_position] = _carId;
                Log.error("Race.SetCarPosition(), Weird Case happen. CarId: " + _carId + ", PositionAsked:" + _position + ", the old Position carId was 0.\r\n");
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
            query += "VALUES(" + guid + "," + qualifyRaceGuid + ", '" + trackPrefix + "'," + (System.DateTime.Now.Ticks / 10000000) + ", " + 0 + ", '" + gridOrder + "', '" + finishOrder + "'," + raceLaps + ", " + (byte)raceInProgressStatus + "," + (byte)raceFeatureMask + "," + qualificationMinute + "," + (byte)weatherStatus + "," + (byte)windStatus + ")";
            //Since this is GUID creation, important 1 at time is created.
            Program.dlfssDatabase.ExecuteNonQuery(query);

            raceSaveInterval = 0;
            stateHasChange = false;
            Log.database(session.GetSessionNameForLog() + " RaceGuid: " + guid + ", TrackPrefix: " + trackPrefix + ", raceLaps:" + raceLaps + ", saved To Database.\r\n");
        }
        private void RequestFinalResult()
        {
            requestedFinalResult = true;
            session.AddToTcpSendingQueud
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_TINY,
                    Packet_Type.PACKET_TINY_MULTI_PURPOSE,
                    new PacketTiny(1, Tiny_Type.TINY_RES)
                )
            );
        }
    }
}
