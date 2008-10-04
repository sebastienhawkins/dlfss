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
    using Game_;
    using PubStats_;
    using Ranking_;

    public sealed class Driver : Car, IDriver
    {
        private static bool[] botGuid = new bool[128];
        internal Driver(ISession _session): base()
        {
            iSession = _session;
        }
        ~Driver()
        {
            if (true == false) { }
        }
        internal static void ConfigApply()
        {
            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "DriverSave");
        }
        new internal void Init(PacketNCN _packet)
        {
            isAdmin = _packet.adminStatus > 0 ? true : false;
            driverName = _packet.driverName;
            driverTypeMask = _packet.driverTypeMask;
            //_packet.total;// What is Total????

            licenceName = _packet.licenceName;
            connectionId = _packet.connectionId;
           
            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";

            //Only a Host will trigger a NCN as a Bot, real AI Bot trigger only a NPL
            if (IsBot()) 
                return;

            SendBanner();
            SendTrackPrefix();

            //To make the MOTD look on a very Black BG
            for (byte itr = 0; ++itr < 5; )
                SendButton((ushort)Button_Entry.MOTD_BACKGROUND);

            SendGui((ushort)Gui_Entry.MOTD);

            configData = new string[(int)Config_User.END];
            for (byte itr = 0; itr < (byte)Config_User.END; itr++)
                configData[itr] = "0";

            LoadFromDB();
            if (guid == 0)
            {
                if (!SetNewGuid())
                    Log.error("Error when creating a new GUID for licenceName: " + LicenceName + ", driverName: " + driverName + "\r\n");
            }
            pb = Program.pubStats.GetPB(LicenceName, "");
            lock(Program.dlfssDatabase){rank = Ranking.GetDriverRanks(LicenceName);}
        }
        new internal void ProcessCPR(PacketCPR _packet)
        {
            base.ProcessCPR(_packet);
            if(_packet.driverName != driverName)
            {
                SaveToDB();
                guid = 0;
                driverName = _packet.driverName;
                SetConfigData("");
            }
        }
        new internal void Init(PacketNPL _packet)
        {
            if(LicenceName == "" && !IsBot()) //What the ???
            {
                Log.error("Driver \""+driverName+"\", has no licence name and was Kicked, something weird happen.\r\n");
                iSession.SendMSTMessage("/msg ^1driver ^7" + driverName+" ^1will be kicked for wrong licence.");
                iSession.SendMSTMessage("/kick "+driverName);
                
                return;
            }
            if (driverName != _packet.driverName)    //I think should be a check != null && != then Error... like custom cheater packet
                driverName = _packet.driverName;

            driverModel = _packet.driverModel;

            if (driverTypeMask != _packet.driverTypeMask)
                driverTypeMask = _packet.driverTypeMask;

            driverMask = _packet.driverMask;

            //_packet.SName; //What is that???
            //_packet.numberInRaceCar_NSURE // What is That???
            if (connectionId != _packet.connectionId)
            {
                if(connectionId != 0)
                    Log.error("Licence.Init(PacketNPL _packet), current connectionId("+connectionId+") was not same as packet connectionId("+_packet.connectionId+"), LicenceName: "+licenceName+".\r\n");
                connectionId = _packet.connectionId;
            }

            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";

            base.Init(_packet);

            laps.Add(new Lap());
            
            if (IsBot())
            {
                for (byte botAddGuid = 0; botAddGuid < 129; botAddGuid++)
                {
                    if (!botGuid[botAddGuid])
                    {
                        botGuid[botAddGuid] = true;
                        guid = (uint)Bot_GUI.FIRST + botAddGuid;
                        break;
                    }
                }
            }
            if (guid == 0)
            {
                LoadFromDB();
                if (guid == 0)
                {
                    if (!SetNewGuid())
                        Log.error("Error when creating a new GUID for licenceName: " + LicenceName + ", driverName: " + driverName + "\r\n");
                }
            }
        }
        internal void ProcessLapInformation(PacketLAP _packet)
        {
            Lap lap = laps[laps.Count - 1];
            lap.ProcessPacketLap(_packet, iSession.GetRaceGuid(), CarPrefix, iSession.GetRaceTrackPrefix(), maxSpeedMs);

            pb = Program.pubStats.GetPB(LicenceName, CarPrefix + iSession.GetRaceTrackPrefix());
            wr = Program.pubStats.GetWR(CarPrefix + iSession.GetRaceTrackPrefix());
            int lapDiff, lapWRDiff;
            lapDiff = lapWRDiff = 0;
            if (pb != null)
            {
                if (pb.LapTime > 0 && lap.LapTime > 0)
                {
                    lapDiff = (int)lap.LapTime - (int)pb.LapTime;
                    if (lapDiff < 0) //New PB
                    {
                        pb.Splits[1] = lap.SplitTime[1];
                        pb.Splits[2] = lap.SplitTime[2];
                        pb.Splits[3] = lap.SplitTime[3];
                        pb.LapTime = lap.LapTime;
                    }
                }
            }
            if (wr != null)
            {
                lapWRDiff = (int)lap.LapTime - (int)wr.LapTime;
                if (lapWRDiff < 0)//A New World Record
                {
                    wr.Splits[1] = lap.SplitTime[1];
                    wr.Splits[2] = lap.SplitTime[2];
                    wr.Splits[3] = lap.SplitTime[3];
                    wr.LapTime = lap.LapTime;
                    wr.LicenceName = LicenceName;
                    ISession.AddMessageMiddleToAll("^2New WR " + ConvertX.MSToString(wr.LapTime, Msg.COLOR_DIFF_TOP, Msg.COLOR_DIFF_TOP) + " ^2by " + LicenceName + ", Bravo!", 8000);
                }
            }
            if(isTimeDiffLap)
            {
                if (pb != null && wr != null)
                {
                    AddMessageTop("^2PB " + ConvertX.MSToString(lapDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER) + " ^2WR " + ConvertX.MSToString(lapWRDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
                }
                else if (pb != null)
                {
                    AddMessageTop("^2PB " + ConvertX.MSToString(lapDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
                }
            }
            
            AddMessageMiddle("^2MS ^7" + Math.Round((decimal)ConvertX.MSToKhm(maxSpeedMs), 2) + " ^7Kmh", 6000);
            maxSpeedMs = 0.0d; 

            laps.Add(new Lap());
        }
        internal void ProcessSplitInformation(PacketSPX _packet)
        {
            //Internal Lap
            Lap lap = laps[laps.Count -1];
            lap.ProcessPacketSplit(_packet);
            
            //PubStats
            pb = Program.pubStats.GetPB(LicenceName, CarPrefix+iSession.GetRaceTrackPrefix());
            wr = Program.pubStats.GetWR(CarPrefix + iSession.GetRaceTrackPrefix());
            int splitDiff, splitWRDiff;
            splitDiff = splitWRDiff = 0;
            if (pb != null)
            {
                if (pb.Splits[_packet.splitNode] > 0 && lap.SplitTime[_packet.splitNode] > 0)
                    splitDiff = (int)lap.SplitTime[_packet.splitNode] - (int)pb.Splits[_packet.splitNode];
            }

            if(isTimeDiffSplit)
            {
                if (wr != null && pb != null)
                {
                    splitWRDiff = (int)lap.SplitTime[_packet.splitNode] - (int)wr.Splits[_packet.splitNode];
                    AddMessageMiddle("^2Split " + ConvertX.MSToString(splitDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER) + " ^2WR " + ConvertX.MSToString(splitWRDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
                }
                else if(pb != null)
                    AddMessageMiddle("^2Split " + ConvertX.MSToString(splitDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
            }
        }
        internal void ProcessRaceStart()
        {
            wr = Program.pubStats.GetWR("");
            laps.Add(new Lap());
            maxSpeedMs = 0.0d;
        }
        internal void ProcessRaceEnd()
        {
        }
        new internal void ProcessLeaveRace(PacketPLL _packet)  //to be called when a car is removed from a race
        {
            base.ProcessLeaveRace(_packet);
            
            if(IsBot())
            {
                botGuid[guid-(uint)Bot_GUI.FIRST] = false;
                guid = 0;
            }
        }
        internal void ProcessCNLPacket(PacketCNL _packet) //Disconnect
        {
            quitReason = _packet.quitReason;
            
            //Only a host will trigger this a real bot trigger only a Leave race.
            if(IsBot())
                return;

            lock( Program.dlfssDatabase)
            {
                SaveToDB();
            }
        }

        private bool isTimeDiffLap = true;
        private bool isTimeDiffSplit = true;
        private bool isAdmin = false;
        private string licenceName = "";
        private byte connectionId = 0;
        private byte unkFlag = 0;
        private string driverName = "";
        private byte driverModel = 0;
        internal Driver_Flag driverMask = Driver_Flag.DRIVER_FLAG_NONE;
        private Driver_Type_Flag driverTypeMask = Driver_Type_Flag.DRIVER_TYPE_NONE;
        private Leave_Reason quitReason = Leave_Reason.LEAVE_REASON_DISCONNECTED;
        private ISession iSession;
        private uint guid = 0;
        private string[] configData;
        private class Lap
        {
            public Lap()
            {
            }
            ~Lap()
            {
                if (true == false) { }
            }
            public void ProcessPacketLap(PacketLAP _packet,uint _raceGuid, string _carPrefix, string _trackPrefix,double _maxSpeedKmh)
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
                maxSpeedKhm = _maxSpeedKmh;
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
            private double maxSpeedKhm = 0.0d;
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
            public double MaxSpeedMs
            {
                get { return maxSpeedKhm; }
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
        private List<Lap> laps = new List<Lap>();
        internal PB pb = null;
        internal WR wr = null;
        private Dictionary<string,Dictionary<string,Rank>> rank = null;
        
        private static uint SAVE_INTERVAL = 60000;
        private uint driverSaveInterval = 0;
        new internal void update(uint diff) 
        {
            driverSaveInterval += diff;
            if (SAVE_INTERVAL < driverSaveInterval  ) //Into Server.update() i use different approch for Timer Solution, so just see both and take the one you love more.
            {
                if (!IsBot())
                {
                    Log.database(iSession.GetSessionNameForLog() + "Driver DriverGuid: " + guid + ", DriverName: " + driverName + ", licenceName:" + LicenceName + ", saved to database.\r\n");
                    SaveToDB();

                    if (laps.Count > 0)
                    {
                        lock (laps)
                        {
                            Lap currentLap = laps[laps.Count - 1];
                            List<Lap>.Enumerator itr = laps.GetEnumerator();
                            while (itr.MoveNext())
                            {
                                Lap lap = itr.Current;
                                if (lap.LapTime < 1)
                                    continue;
                                SaveLapsToDB(lap);
                                Log.database(iSession.GetSessionNameForLog() + "Lap for DriverGuid: " + guid + ", car_prefix:" + lap.CarPrefix + ", track_prefix: " + lap.TrackPrefix + ", saved to database.\r\n");
                            }
                            laps.Clear();
                            if (currentLap.LapTime < 1)
                                laps.Add(currentLap);
                            else
                                laps.Add(new Lap());
                        }
                    }
                }
                else //this is Bot Only
                {
                    laps.Clear();
                    laps.Add(new Lap());
                }
            }
            base.update(diff);
        }

        private void LoadFromDB()
        {
            lock (Program.dlfssDatabase)
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `guid`,`config_data` FROM `driver` WHERE `licence_name`='" + LicenceName.Replace(@"\", @"\\").Replace(@"'", @"\'") + "' AND `driver_name`='" + driverName.Replace(@"\", @"\\").Replace(@"'", @"\'") + "'");
                if (reader.Read())
                {
                    guid = (uint)reader.GetInt32(0);
                    SetConfigData(reader.GetString(1));
                }
                reader.Dispose();
            }
        }
        private void SaveLapsToDB(Lap lap)
        {
            string query = "INSERT INTO `driver_lap` (`guid_race`,`guid_driver`,`car_prefix`,`track_prefix`,`driver_mask`,`split_time_1`,`split_time_2`,`split_time_3`,`lap_time`,`total_time`,`lap_completed`,`max_speed_ms`,`current_penalty`,`pit_stop_count`,`yellow_flag_count`,`blue_flag_count`)";
            query += "VALUES (" + lap.RaceGuid + "," + guid + ",'" + lap.CarPrefix + "','" + iSession.GetRaceTrackPrefix() + "'," + (byte)lap.DriverMask + "," + lap.SplitTime[1] + "," + lap.SplitTime[2] + "," + lap.SplitTime[3] + "," + lap.LapTime + "," + lap.TotalTime + "," + lap.LapCompleted + ","+ lap.MaxSpeedMs +","+ (byte)lap.CurrentPenality + "," + lap.PitStopCount + "," + lap.YellowFlagCount + "," + lap.BlueFlagCount + ")";
            Program.dlfssDatabase.ExecuteNonQuery(query);
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `driver` WHERE `guid`=" + guid);
            Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `driver` (`guid`,`licence_name`,`driver_name`,`config_data`,`last_connection_time`) VALUES (" + guid + ", '" + LicenceName.Replace(@"\", @"\\").Replace(@"'", @"\'") + "','" + driverName.Replace(@"\", @"\\").Replace(@"'", @"\'") + "', '" + String.Join(" ", configData) + "', " + (System.DateTime.Now.Ticks / 10000000) + ")");
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
        private void ApplyConfigData()
        {
           ushort accStart = GetConfigUint16(Config_User.ACCELERATION_START);
           ushort accEnd = GetConfigUint16(Config_User.ACCELERATION_STOP);
           if(accEnd > 10 && accEnd > accStart)
           {
                SetAccelerationStartSpeed(accStart);
                SetAccelerationEndSpeed(accEnd);
           }
           if(GetConfigUint16(Config_User.ACCELERATION_ON)>0)
                SetAccelerationOn(true);
           else
                SetAccelerationOn(false);
          
           if (GetConfigUint16(Config_User.DRIFT_SCORE_ON) > 0)
                SetDriftScoreOn(true);
           else
                SetDriftScoreOn(false);

           if (GetConfigUint16(Config_User.TIMEDIFF_LAP) > 0)
               IsTimeDiffLap = true;
           else
               IsTimeDiffLap = false;

           if (GetConfigUint16(Config_User.TIMEDIFF_SPLIT) > 0)
               IsTimeDiffSplit = true;
           else
               IsTimeDiffSplit = false;
        }
        private void SetConfigData(string configString)
        {
            string[] configStrings = configString.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
            if(configStrings.Length != (int)Config_User.END)
            {
                for(byte itr = 0 ; itr < (byte)Config_User.END;itr++)
                    configData[itr] = "0";

                configData[(int)Config_User.ACCELERATION_ON] = "1";
                configData[(int)Config_User.ACCELERATION_START] = "0";
                configData[(int)Config_User.ACCELERATION_STOP] = "100";
                configData[(int)Config_User.DRIFT_SCORE_ON] = "1";
                configData[(int)Config_User.TIMEDIFF_LAP] = "1";
                configData[(int)Config_User.TIMEDIFF_SPLIT] = "1";
                return;
            }
            configStrings.CopyTo(configData,0);
            ApplyConfigData();
        }
        internal ushort GetConfigUint16(Config_User index)
        {
            return  Convert.ToUInt16(configData[(int)index]);
        }
        internal uint GetConfigUint32(Config_User index)
        {
            return Convert.ToUInt32(configData[(int)index]);
        }
        internal string GetConfigString(Config_User index)
        {
            return configData[(int)index];
        }
        internal void SetConfigValue(Config_User index, string value)
        {
            configData[(int)index] = value;
        }
        internal ISession ISession
        {
            get { return iSession; }
        }
        internal Rank GetRank(string trackPrefix, string carPrefix)
        {
            if( rank != null && rank.ContainsKey(trackPrefix) && rank[trackPrefix].ContainsKey(carPrefix))
                return rank[trackPrefix][carPrefix];
            else
                return null;
        }
        public bool IsAdmin
        {
            get { return isAdmin; }
            //set { adminFlag = value; }
        }
        public string DriverName
        {
            get { return driverName; }
           // set { driverName = value; }
        }
        public string LicenceName 
        { 
            get { return licenceName; } 
        }
        public byte ConnectionId 
        { 
            get { return connectionId; } 
        }
        internal bool IsTimeDiffLap
        {
            get { return isTimeDiffLap; }
            set 
            { 
                isTimeDiffLap = value;
                SetConfigValue(Config_User.TIMEDIFF_LAP, (isTimeDiffLap ? "1" : "0"));
            }
        }
        internal bool IsTimeDiffSplit
        {
            get { return isTimeDiffSplit; }
            set 
            { 
                isTimeDiffSplit = value;
                SetConfigValue(Config_User.TIMEDIFF_SPLIT, (isTimeDiffSplit ? "1" : "0"));
            }
        }
        public uint GetGuid()
        {
            return guid;
        }
        public bool IsBot()
        {
            return ((Driver_Type_Flag.DRIVER_TYPE_AI & driverTypeMask) == Driver_Type_Flag.DRIVER_TYPE_AI || ConnectionId == 0);
        }
        public void SendMTCMessage(string message)
        {
            //Serve no Purpose sending a Message to a Bot.
            if (IsBot())
                return;
            PacketMTC _packet;
            if (CarId == 0)
                _packet = new PacketMTC(ConnectionId, message, 0);
            else
                _packet = new PacketMTC(CarId, message);

            ((Session)iSession).AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MTC, Packet_Type.PACKET_MTC_CHAT_TO_LICENCE, _packet));
       
            Log.progress("Sending MTC packet to: " + CarId + ", with ConnectionId: " + ConnectionId + "\r\n");
        }
    }
}
