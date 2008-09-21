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
    using Definition_;
    using Packet_;
    using Script_;
    using Log_;
    using Config_;
    using Session_;
    using PubStats_;

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
            pb = Program.pubStats.GetPB(LicenceName, "");
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

            laps.Enqueue(new Lap());

            if (IsBot())
                return;

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
            laps.Peek().ProcessPacketLap(_packet,iSession.GetRaceGuid(),CarPrefix,iSession.GetRaceTrackPrefix());

            if (pb != null && wr != null)
            {
                if (pb.LapTime > 0 && laps.Peek().LapTime > 0)
                {
                    int lapDiff = (int)laps.Peek().LapTime - (int)pb.LapTime;
                    int lapWRDiff = (int)laps.Peek().LapTime - (int)wr.LapTime;

                    AddMessageTop("^2diff " + PubStats.MSToString(lapDiff, "^7", "^8") + ", ^1wr ^2diff " + PubStats.MSToString(lapWRDiff, "^7", "^8"), 7000);
                }
            }
            laps.Enqueue(new Lap());
        }
        public void ProcessSplitInformation(PacketSPX _packet)
        {
            //Internal Lap
            laps.Peek().ProcessPacketSplit(_packet);
            
            //PubStats
            pb = Program.pubStats.GetPB(LicenceName, CarPrefix+iSession.GetRaceTrackPrefix());
            wr = Program.pubStats.GetWR(CarPrefix + iSession.GetRaceTrackPrefix());
            if (pb != null && wr != null)
            {
                if (pb.Splits[_packet.splitNode - 1] > 0 && laps.Peek().SplitTime[_packet.splitNode] > 0)
                {
                    int splitDiff = (int)laps.Peek().SplitTime[_packet.splitNode] - (int)pb.Splits[_packet.splitNode - 1];
                    int splitWRDiff = (int)laps.Peek().SplitTime[_packet.splitNode] - (int)wr.Splits[_packet.splitNode - 1];

                    AddMessageTop("^2diff " + PubStats.MSToString(splitDiff, "^7", "^8") + ", ^3wr ^2diff " + PubStats.MSToString(splitWRDiff, "^7", "^8"), 7000);
                }
            }
        }
        public void ProcessRaceStart()
        {
            wr = Program.pubStats.GetWR("");
            laps.Enqueue(new Lap());
        }
        public void ProcessRaceEnd()
        {
        }
        
        private bool adminFlag = false;
        private string driverName = "";
        private byte driverModel = 0;
        public Driver_Flag driverMask = Driver_Flag.DRIVER_FLAG_NONE;
        private Driver_Type_Flag driverTypeMask = Driver_Type_Flag.DRIVER_TYPE_NONE;
        private ISession iSession;
        private uint guid = 0;
        private uint configMask = 0;
        private Queue<Lap> laps = new Queue<Lap>();
        public PB pb = null;
        public WR wr = null;

        private class Lap
        {
            public Lap()
            {
            }
            ~Lap()
            {
                if (true == false) { }
            }
            public void ProcessPacketLap(PacketLAP _packet,uint _raceGuid, string _carPrefix, string _trackPrefix)
            {
                lapTime = _packet.lapTime;
                totalTime = _packet.totalTime;
                SetPitStopCount(_packet.pitStopTotal);
                carPrefix = _carPrefix;
                trackPrefix = _trackPrefix;
                raceGuid = _raceGuid;
                driverMask = _packet.driverMask;
                currentPenality = _packet.currentPenality;
                lapCompleted = _packet.lapCompleted;
            }
            public void ProcessPacketSplit(PacketSPX _packet)
            {
                splitTime[_packet.splitNode] = _packet.splitTime;
                totalTime = _packet.totalTime;
                SetPitStopCount(_packet.pitStopTotal);
            }

            private uint raceGuid = 0;
            private string trackPrefix = "";
            private string carPrefix = "";
            private Driver_Flag driverMask = Driver_Flag.DRIVER_FLAG_NONE;
            private uint lapTime = 0;
            private uint totalTime = 0;
            private ushort lapCompleted = 0;
            private uint[] splitTime = new uint[4] { 0, 0, 0, 0 };
            private ushort yellowFlagCount = 0;
            private ushort blueFlagCount = 0;
            private Penalty_Type currentPenality = Penalty_Type.PENALTY_TYPE_NONE;
            private byte pitStopTotal = 0;      //current race total pitstop.
            private byte pitStopTotalCount = 0; //To help make PitStop by lap and not by race.
            private byte pitStopCount = 0;      //this is the Current Lap Pitstop, cen be more then 2, since on a cruise server is possible i think so.

            public Penalty_Type CurrentPenality
            {
                get { return currentPenality; }
            }
            public Driver_Flag DriverMask
            {
                get { return driverMask; }
            }
            public string CarPrefix
            {
                get { return carPrefix; }
            }
            public string TrackPrefix
            {
                get { return trackPrefix; }
             }
            public uint RaceGuid
            {
                get { return raceGuid; }
            }
            public uint LapTime
            {
                get {return lapTime;}
            }
            public uint[] SplitTime
            {
                get { return splitTime; }
            }
            public ushort BlueFlagCount
            {
                get { return blueFlagCount; }
            }
            public ushort YellowFlagCount
            {
                get { return yellowFlagCount; }
            }
            public uint TotalTime
            {
                get { return totalTime; }
            }
            public ushort LapCompleted
            {
                get { return lapCompleted; }
            }
            public byte PitStopCount
            {
                get { return pitStopCount; }
            }

            private void SetPitStopCount(byte _pitStopTotal)
            {
                pitStopTotal = _pitStopTotal;
                if (pitStopTotal > pitStopTotalCount)
                {
                    pitStopCount = (byte)(pitStopTotal - pitStopTotalCount);
                    pitStopTotalCount = pitStopTotal;
                }
            }
        }
        private static uint SAVE_INTERVAL = 60000;
        private uint driverSaveInterval = 0;
        private void LoadFromDB()
        {
            lock (Program.dlfssDatabase)
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `guid`,`config_mask` FROM `driver` WHERE `licence_name`='" + licenceName + "' AND `driver_name`='" + driverName + "'");
                if (reader.Read())
                {
                    guid = (uint)reader.GetInt32(0);
                    //...
                }
                reader.Dispose();
            }
        }
        private void SaveLapsToDB(Lap lap)
        {
            string query = "INSERT INTO `driver_lap` (`guid_race`,`guid_driver`,`car_prefix`,`track_prefix`,`driver_mask`,`split_time_1`,`split_time_2`,`split_time_3`,`lap_time`,`total_time`,`lap_completed`,`current_penalty`,`pit_stop_count`,`yellow_flag_count`,`blue_flag_count`)";
            query += "VALUES (" + lap.RaceGuid + "," + guid + ",'" + lap.CarPrefix + "','" + iSession.GetRaceTrackPrefix() + "'," + (byte)lap.DriverMask + "," + lap.SplitTime[1] + "," + lap.SplitTime[2] + "," + lap.SplitTime[3] + "," + lap.LapTime + "," + lap.TotalTime + "," + lap.LapCompleted + "," + (byte)lap.CurrentPenality + "," + lap.PitStopCount + "," + lap.YellowFlagCount + "," + lap.BlueFlagCount + ")";
            Program.dlfssDatabase.ExecuteNonQuery(query);
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `driver` WHERE `guid`=" + guid);
            Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `driver` (`guid`,`licence_name`,`driver_name`,`config_mask`,`last_connection_time`) VALUES (" + guid + ", '" + licenceName + "','" + driverName + "', " + configMask + ", " + (System.DateTime.Now.Ticks / 10000000) + ")");
            driverSaveInterval = 0;
        }
        private bool SetNewGuid()
        {
            bool returnValue = false;
            lock (Program.dlfssDatabase)
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `driver`");
                if (reader.Read())
                {
                    guid = reader.IsDBNull(0) ? 1 : (uint)reader.GetInt32(0) + 1;
                    reader.Close();
                    SaveToDB();
                    returnValue = true;
                }
                reader.Dispose();
            }
            return returnValue;
        }

        new public void update(uint diff) 
        {
            if (!IsBot())
            {
                if ((driverSaveInterval += diff) >= SAVE_INTERVAL) //Into Server.update() i use different approch for Timer Solution, so just see both and take the one you love more.
                {
                    Log.database(iSession.GetSessionNameForLog() + "Driver DriverGuid: " + guid + ", DriverName: " + driverName + ", licenceName:" + licenceName + ", saved To Database.\r\n");
                    SaveToDB();
                    if (laps.Count > 0){lock (laps)
                    {
                        Lap currentLap = laps.Peek();
                        Lap lap;
                        while (laps.Count > 0)
                        {
                            lap = laps.Dequeue();
                            if (lap.LapTime < 1)
                                continue;
                            SaveLapsToDB(lap);
                            Log.database(iSession.GetSessionNameForLog() + "Lap For DriverGuid: " + guid + ", car_prefix:" + lap.CarPrefix + ", track_prefix: " + lap.TrackPrefix + ", saved To Database.\r\n");
                        }
                        if (currentLap.LapTime < 1)
                            laps.Enqueue(currentLap);
                        else
                            laps.Enqueue(new Lap());
                    }}
                }
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
