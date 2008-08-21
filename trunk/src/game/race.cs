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
        POSITION_START = 1,
        POSITION_FIRST = 1,
        POSITION_LAST = 192,
        POSITION_END = 194,
    }
    public sealed class Race : IRace
	{
        public Race(Session _session)
        {
            session = _session;
        }
        ~Race()
        {
        }
        public static void ConfigApply()
        {
            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "DriverSave");
        }

        // Thoses REO/RST Init should be called into this order REO Then RST
        public void Init(PacketREO _packet)
        {
            timeStart = (uint)(System.DateTime.Now.Ticks / 10000);
            for (byte itr = (byte)PositionIndex.POSITION_START; itr < (byte)PositionIndex.POSITION_END; )
                carPosition[itr++] = 0;

            carCount = _packet.carCount;
            gridOrder = "";
            for (byte itr = 0; itr < carCount; )
            {
                gridOrder += _packet.carIds[itr] + " ";
                carPosition[itr + 1] = _packet.carIds[itr++];
            }
            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY)
                qualifyRaceGuid = guid;
            else if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_NONE)
                qualifyRaceGuid = 0;

            if (!SetNewGuid())
                Log.error("Error When Creating a New GUID for Race.\r\n");
        } //Is the Main RaceSTART Procedure
        public void Init(PacketRST _packet)
        {
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
            relatedRaceGuid = 0;
            hasToBeSavedIntoPPSTA = true;
        }
        public void Init(PacketSTA _packet)
        {
            connectionCount = _packet.connectionCount;
            finishedCount = _packet.finishedCount;
            carCount = _packet.carCount;
            qualificationMinute = _packet.qualificationMinute;

            /*if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_QUALIFY
                && (Race_In_Progress_Status)_packet.raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_RACING)
            {
                relatedRaceGuid = qualifyRaceGuid;
                qualifyRaceGuid = 0;
            }*/

            raceInProgressStatus = (Race_In_Progress_Status)_packet.raceInProgressStatus;
            
            raceLaps = _packet.raceLaps;
            trackPrefix = _packet.trackName;
            weatherStatus = (Weather_Status)_packet.weatherStatus;
            windStatus = (Wind_Status)_packet.windStatus;

            if (hasToBeSavedIntoPPSTA)
            {
                SaveToDB();
                hasToBeSavedIntoPPSTA = false;
            }
            //replaySpeed = _packet.replaySpeed;        //This is about the Driver/Car/Licence
            //viewOptionMask = _packet.viewOptionMask;  //This is about the Driver/Car/Licence
            //cameraMode = _packet.cameraMode;          //This is about the Driver/Car/Licence
            //currentCarId = _packet.currentCarId;      //This is about the Driver/Car/Licence
        }
        public void ProcessCarInformation(CarInformation _carInformation)
        {
            if (_carInformation.position == 0) //Position is Unknow for that Car.
                return;
            ProcessPosition(_carInformation.carId,_carInformation.position);
        }
        public void ProcessRaceEnd()
        {
            //RaceEnd();
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
        private bool hasToBeSavedIntoPPSTA = false;
        private uint guid = 0;
        private uint qualifyRaceGuid = 0;
        private uint relatedRaceGuid = 0;
        private Session session;
        private uint timeStart = 0;
        private byte[] carPosition = new byte[(int)PositionIndex.POSITION_END]; //index 0, mean nothing and index 193 mean Nothing too.
        
        private static uint SAVE_INTERVAL = 60000;
        private uint raceSaveInterval = 0;
        
        public uint GetGuid()
        {
            return guid;
        }
        public string GetTrackPrefix()
        {
            return trackPrefix;
        }
        
        public void update(uint diff)
        {
           /* if (guid != 0)
            {
                if ((raceSaveInterval += diff) >= SAVE_INTERVAL) //Into Server.update() i use different approch for Timer Solution, so just see both and take the one you love more.
                    SaveToDB();
            }*/
            
            if (raceInProgressStatus == Race_In_Progress_Status.RACE_PROGRESS_RACING)
            {

            }
        }

        private void RaceEnd() //Is the Main RaceEND Procedure
        {
            qualifyRaceGuid = 0;
            SaveToDB();
        }

        private void ProcessPosition(byte _carId, byte _position)
        {
            if (carPosition[_position] == _carId)   //Same Position
                return;
            else if (carPosition[_position] == 0)   //Not same position, but don't take positon of anyone... a weird case
            {
                carPosition[_position] = _carId;
                Log.error("Race.ProcessPosition(), Weird Case happen. CarId: " + _carId + ", PositionAsked:" + _position + ", the old Position carId was 0.\r\n");
                return;
            }

            byte oldCarId = carPosition[_position];
            byte oldPosition = FindCarPosition(_carId);
            if (oldPosition == (byte)PositionIndex.POSITION_END)
                return;
            carPosition[_position] = _carId;
            carPosition[oldPosition] = oldCarId;

        }
        private byte FindCarPosition(byte _carId)
        {
            byte itr = (byte)PositionIndex.POSITION_START;
            for (; itr < (byte)PositionIndex.POSITION_END && carPosition[itr++] != _carId;);
            return itr;
        }
        private bool SetNewGuid()
        {
            IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `race`");
            if (reader.Read())
            {
                guid = reader.IsDBNull(0) ? 1 : (uint)reader.GetInt32(0) + 1;
                reader.Dispose();
                SaveToDB();
                return true;
            }
            reader.Dispose();
            return false;
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `race` WHERE `guid`=" + guid);
            string query = "INSERT INTO `race` (`guid`,`qualify_race_guid`,`track_prefix`,`start_timestamp`,`end_timestamp`,`grid_order`,`finish_order`,`race_laps`,`race_status`,`race_feature`,`qualification_minute`,`weather_status`,`wind_status`)";
            query += "VALUES(" + guid + "," + qualifyRaceGuid + ", '" + trackPrefix + "'," + (System.DateTime.Now.Ticks / 10000000) + ", " + 0 + ", '" + gridOrder + "', '" + finishOrder + "'," + raceLaps + ", " + (byte)raceInProgressStatus + "," + (byte)raceFeatureMask + "," + qualificationMinute + "," + (byte)weatherStatus + "," + (byte)windStatus + ")";
            Program.dlfssDatabase.ExecuteNonQuery(query);
            
            Log.database(session.GetSessionNameForLog() + " RaceGuid: " + guid + ", TrackPrefix: " + trackPrefix + ", raceLaps:" + raceLaps + ", saved To Database.\r\n");
        }
    }
}
