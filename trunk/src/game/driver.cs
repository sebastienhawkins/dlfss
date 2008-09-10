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

using System.Data;
using System;

namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Script_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Config_;
    using Drive_LFSS.Session_;

    public sealed class Driver : Car, IDriver
    {
        public Driver(ISession _session)
            : base()
        {
            iSession = _session;
        }
        ~Driver()
        {
            if (true == false) { }
        }
        public static void ConfigApply()
        {
            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "DriverSave");
        }
        new public void Init(PacketNCN _packet)
        {
            adminFlag = _packet.adminStatus > 0 ? true : false;
            driverName = _packet.driverName;
            driverTypeMask = _packet.driverTypeMask;
            //_packet.total;// What is Total????

            base.Init(_packet);

            if (IsBot())
                return;

            SendBanner();
            SendTrackPrefix();
            //To make the MODT look on a very Black BG
            for (byte itr = 0; ++itr < 5; )
                SendButton((ushort)Button_Entry.MOTD_BACKGROUND);

            SendGui((ushort)Gui_Entry.MOTD);

            LoadFromDB();
            if (guid == 0)
            {
                if (!SetNewGuid())
                    Log.error("Error When Creating a New GUID for licenceName: " + licenceName + ", driverName: " + driverName + "\r\n");
            }
        }
        new public void Init(PacketNPL _packet)
        {
            if (driverName != _packet.driverName)    //I think should be a check != null && != then Error... like custom cheater packet
                driverName = _packet.driverName;

            driverModel = _packet.driverModel;

            if (driverTypeMask != _packet.driverTypeMask)
                driverTypeMask = _packet.driverTypeMask;

            driverMask = _packet.driverMask;

            //_packet.SName; //What is that???
            //_packet.numberInRaceCar_NSURE // What is That???

            base.Init(_packet);

            currentLap.Dispose();
            currentLap = new Lap(iSession.GetRaceGuid(), guid, CarName, iSession.GetRaceTrackPrefix(), driverMask);

            if (IsBot())
                return;

            //Removing This Site banner
            //if (iSession.IsRaceInProgress())
            {
                RemoveTrackPrefix(); //TODO: readd it when Pit
                RemoveBanner();
            }
            if (guid == 0)
            {
                LoadFromDB();
                if (guid == 0)
                {
                    if (!SetNewGuid())
                        Log.error("Error When Creating a New GUID for licenceName: " + licenceName + ", driverName: " + driverName + "\r\n");
                }
            }

        }
        public void ProcessLapInformation(PacketLAP _packet)
        {
            currentLap.ProcessPacketLap(_packet);

            // Check for fastest
            // do other thing we need
            currentLap.Dispose();
            currentLap = new Lap(iSession.GetRaceGuid(), guid, CarName, iSession.GetRaceTrackPrefix(), driverMask);
        }
        public void ProcessSplitInformation(PacketSPX _packet)
        {
            currentLap.ProcessPacketSplit(_packet);
        }
        public void ProcessRaceStart()
        {
            currentLap.Dispose();
            currentLap = new Lap(iSession.GetRaceGuid(), guid, CarName, iSession.GetRaceTrackPrefix(), driverMask);
        }
        public void ProcessRaceEnd()
        {
            currentLap.Dispose();
            currentLap = new Lap(iSession.GetRaceGuid(), guid, CarName, iSession.GetRaceTrackPrefix(), driverMask);
        }
        
        //Loaded From packet
        private bool adminFlag = false;
        private string driverName = "";
        private byte driverModel = 0;
        public Driver_Flag driverMask = Driver_Flag.DRIVER_FLAG_NONE;
        private Driver_Type_Flag driverTypeMask = Driver_Type_Flag.DRIVER_TYPE_NONE;
        private ISession iSession;

        //
        private uint guid = 0;
        private uint configMask = 0;
        private Lap currentLap = new Lap();
        private Lap fastestLap = new Lap(); //will be to be loaded... from RaceStart(RST)
        private class Lap
        {
            public Lap()
            { }
            public Lap(uint _raceGuid, uint _driverGuid, string _carAbreviation, string _trackAbreaviation, Driver_Flag _driverMask)
            {
                raceGuid = _raceGuid;
                driverGuid = _driverGuid;
                carPrefix = _carAbreviation;
                trackPrefix = _trackAbreaviation;
                driverMask = _driverMask;
            }
            ~Lap()
            {
            }
            public void Dispose()
            {
                if (raceGuid != 0)
                {
                    if (!SetNewGuid())
                        Log.error("Database Error, unable to Create new GUID for Lap.\r\n");
                }
            }
            public void ProcessPacketLap(PacketLAP _packet)
            {
                lapTime = _packet.lapTime;
                totalTime = _packet.totalTime;
                lapCompleted = _packet.lapCompleted;
                driverMask = _packet.driverMask;
                currentPenality = _packet.currentPenality;
                pitStopTotal = _packet.pitStopTotal;
                SetPitStopCount();
            }
            public void ProcessPacketSplit(PacketSPX _packet)
            {
                splitTime[_packet.splitNode] = _packet.splitTime;
                totalTime = _packet.totalTime;
                pitStopTotal = _packet.pitStopTotal;
                SetPitStopCount();
                currentPenality = _packet.currentPenalty;
            }

            private uint guid = 0;
            private uint driverGuid = 0;
            private uint raceGuid = 0;
            private string carPrefix = "";
            private string trackPrefix = "";
            private uint lapTime = 0;
            private uint totalTime = 0;
            private ushort lapCompleted = 0;
            private Driver_Flag driverMask = Driver_Flag.DRIVER_FLAG_NONE;
            private Penalty_Type currentPenality = Penalty_Type.PENALTY_TYPE_NONE;
            private uint[] splitTime = new uint[4] { 0, 0, 0, 0 };
            private ushort yellowFlagCount = 0;
            private ushort blueFlagCount = 0;
            private byte pitStopTotal = 0;      //current race total pitstop.
            private byte pitStopTotalCount = 0; //To help make PitStop by lap and not by race.
            private byte pitStopCount = 0;      //this is the Current Lap Pitstop, cen be more then 2, since on a cruise server is possible i think so.

            //It important this is called eachTime we change: private byte pitStopTotal, since we count pisStop that happen during this lap.
            private void SetPitStopCount()
            {
                if (pitStopTotal > pitStopTotalCount)
                {
                    pitStopCount = (byte)(pitStopTotal - pitStopTotalCount);
                    pitStopTotalCount = pitStopTotal;
                }
            }
            private void SaveToDB()
            {
                Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `driver_lap` WHERE `guid`=" + guid);
                string query = "INSERT INTO `driver_lap` (`guid`,`guid_race`,`guid_driver`,`car_prefix`,`track_prefix`,`driver_mask`,`split_time_1`,`split_time_2`,`split_time_3`,`lap_time`,`total_time`,`lap_completed`,`current_penalty`,`pit_stop_count`,`yellow_flag_count`,`blue_flag_count`)";
                query += "VALUES (" + guid + "," + raceGuid + "," + driverGuid + ",'" + carPrefix + "','" + trackPrefix + "'," + (byte)driverMask + "," + splitTime[1] + "," + splitTime[2] + "," + splitTime[3] + "," + lapTime + "," + totalTime + "," + lapCompleted + "," + (byte)currentPenality + "," + pitStopCount + "," + yellowFlagCount + "," + blueFlagCount + ")";
                Program.dlfssDatabase.ExecuteNonQuery(query);
                Log.database("LapGuid: " + guid + ", DriverGuid: " + driverGuid + ", car_prefix:" + carPrefix + ", track_prefix: " + trackPrefix + ", saved To Database.\r\n");
            }
            private bool SetNewGuid()
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `driver_lap`");
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
        }

        private static uint SAVE_INTERVAL = 60000;
        private uint driverSaveInterval = 0;

        private void LoadFromDB()
        {
            Program.dlfssDatabase.NewTransaction();
            IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `guid`,`config_mask` FROM `driver` WHERE `licence_name`='" + licenceName + "' AND `driver_name`='" + driverName + "'");
            if (reader.Read())
            {
                guid = (uint)reader.GetInt32(0);
                //...
            }

            reader.Dispose();
            Program.dlfssDatabase.EndTransaction();
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `driver` WHERE `guid`=" + guid);
            Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `driver` (`guid`,`licence_name`,`driver_name`,`config_mask`,`last_connection_time`) VALUES (" + guid + ", '" + licenceName + "','" + driverName + "', " + configMask + ", " + (System.DateTime.Now.Ticks / 10000000) + ")");
            driverSaveInterval = 0;
            Log.database(iSession.GetSessionNameForLog() + " DriverGuid: " + guid + ", DriverName: " + driverName + ", licenceName:" + licenceName + ", saved To Database.\r\n");
        }
        private bool SetNewGuid()
        {
            IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `driver`");
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
        new public void update(uint diff) 
        {
            if (!IsBot())
            {
                if ((driverSaveInterval += diff) >= SAVE_INTERVAL) //Into Server.update() i use different approch for Timer Solution, so just see both and take the one you love more.
                    SaveToDB();
                //...
            }

            base.update(diff);
        }
        public ISession ISession
        {
            get { return iSession; }
        }
        public void SendMessage(string message)
        {
            //Serve no Purpose sending a Message to a Bot.
            if (IsBot()) 
                return;

            ((Session)ISession).AddToTcpSendingQueud
            (
                new Packet(Packet_Size.PACKET_SIZE_MTC, Packet_Type.PACKET_MTC_CHAT_TO_LICENCE,
                    new PacketMTC(CarId, message)));
            Log.progress("Sending MTC packet to:"+CarId + ", with Licence: "+LicenceId+"\r\n");
        }
        public bool AdminFlag
        {
            get { return adminFlag; }
            //set { adminFlag = value; }
        }
        public string DriverName
        {
            get { return driverName; }
           // set { driverName = value; }
        }
        public uint GetGuid()
        {
            return guid;
        }
        public bool IsBot()
        {
            return ((Driver_Type_Flag.DRIVER_TYPE_AI & driverTypeMask) == Driver_Type_Flag.DRIVER_TYPE_AI || LicenceId == 0);
        }
        public void SendMTCMessage(string _message)
        {
            if (IsBot())
                return;
            PacketMTC _packet;
            if (CarId == 0)
                _packet = new PacketMTC(LicenceId, _message, 0);
            else
                _packet = new PacketMTC(CarId, _message);

            ((Session)iSession).AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MTC,Packet_Type.PACKET_MTC_CHAT_TO_LICENCE,_packet));
        }
    }
}
