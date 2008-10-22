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
using System.Globalization;
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
    
    partial class Driver : Button, IDriver, ICar, CarMotion, IButton
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
        internal void Init(PacketNCN _packet)
        {
            isAdmin = _packet.adminStatus > 0 ? true : false;
            driverName = _packet.driverName;
            driverTypeMask = _packet.driverTypeMask;
            //_packet.total;// What is Total????

            licenceName = _packet.licenceName;
            if (licenceName == "") //LFS Server Driver
                licenceName = "AI";
            connectionId = _packet.connectionId;
           
            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";

            //Only a Host will trigger a NCN as a Bot, real AI Bot trigger only a NPL
            if (IsBot()) 
                return;
            ProcessBFNClearAll(false);
            SendBanner();
            SendTrackPrefix();

            //To make the MOTD look on a very Black BG
            if (!ISession.IsFreezeMotdSend())
            {
                for (byte itr = 0; ++itr < 5; )
                    SendButton(Button_Entry.MOTD_BACKGROUND);

                SendGui(Gui_Entry.MOTD);
            }

            SetConfigData("");
            
            LoadFromDB();
            if (guid == 0)
            {
                if (!SetNewGuid())
                    Log.error("Error when creating a new GUID for licenceName: " + LicenceName + ", driverName: " + driverName + "\r\n");
            }
            pb = Program.pubStats.GetPB(LicenceName, "");
            rank = Ranking.GetDriverRanks(LicenceName);
        }
        internal void ProcessCPR(PacketCPR _packet)
        {
            if (carPlate != _packet.carPlate)
                carPlate = _packet.carPlate;

            if(_packet.driverName != driverName)
            {
                Program.dlfssDatabase.Lock();
                {
                    SaveToDB();
                }
                Program.dlfssDatabase.Unlock();
                guid = 0;
                driverName = _packet.driverName;
                SetConfigData("");
            }
        }
        internal void Init(PacketNPL _packet)
        {

            if (driverName != _packet.driverName)    //I think should be a check != null && != then Error... like custom cheater packet
                driverName = _packet.driverName;

            driverModel = _packet.driverModel;

            if (driverTypeMask != _packet.driverTypeMask)
                driverTypeMask = _packet.driverTypeMask;
            if (LicenceName == "" && !IsBot()) //What the ???
            {
                Log.error("Driver \"" + driverName + "\", has no licence name and was Kicked, something weird happen.\r\n");
                iSession.SendMSTMessage("/msg ^1driver ^7" + driverName + " ^1will be kicked for wrong licence.");
                iSession.SendMSTMessage("/kick " + driverName);

                return;
            }

            driverMask = _packet.driverMask;

            //_packet.SName; //What is that???
            //_packet.numberInRaceCar_NSURE // What is That???
            if (connectionId != _packet.connectionId)
            {
                if(!IsBot())
                    Log.error("Licence.Init(PacketNPL _packet), current connectionId("+connectionId+") was not same as packet connectionId("+_packet.connectionId+"), LicenceName: "+licenceName+".\r\n");
                connectionId = _packet.connectionId;
            }

            if ((driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";

            
            bool firstTime = false;
            if (carPrefix != _packet.carPrefix || trackPrefix != iSession.GetRaceTrackPrefix())
                firstTime = true;

            trackPrefix = iSession.GetRaceTrackPrefix();
            carPrefix = _packet.carPrefix;
            carId = _packet.carId;
            carPlate = _packet.carPlate;
            carSkin = _packet.skinName;
            addedIntakeRestriction = _packet.addedIntakeRestriction;
            addedMass = _packet.addedMass;
            passenger = _packet.passenger;
            tyreFrontLeft = _packet.tyreFrontLeft;
            tyreFrontRight = _packet.tyreFrontRight;
            tyreRearLeft = _packet.tyreRearLeft;
            tyreRearRight = _packet.tyreRearRight;

            EnterTrack(firstTime);

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
            totalLapCount++;
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
            
            if(isMaxSpeedDisplay)
                AddMessageMiddle("^2MS ^7" + Math.Round((decimal)ConvertX.MSToKhm(maxSpeedMs), 2) + " ^7Kmh", 6000);
            maxSpeedMs = 0.0d; 

            laps.Enqueue(lap);
            lap = new Lap();
        }
        internal void ProcessSplitInformation(PacketSPX _packet)
        {
            //Internal Lap
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
        internal void ProcessRESPacket(PacketRES _packet)
        {
            timeTotalLastRace = _packet.totalTime;
            timeFastestLapLastRace = _packet.fastestLapTime;
            lapCountTotalLastRace = _packet.lapCount;
        }
        internal void ProcessRaceStart()
        {
            wr = Program.pubStats.GetWR("");
            maxSpeedMs = 0.0d;
            lap = new Lap();
            
            if(IsOnTrack() && currentGui == Gui_Entry.RESULT)
                RemoveGui(Gui_Entry.RESULT);
        }
        internal void ProcessRaceEnd()
        {
        }
        internal void ProcessLeaveRace(PacketPLL _packet)  //to be called when a car is removed from a race
        {
            carId = 0;
            LeaveTrack();
            
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

            Program.dlfssDatabase.Lock();
            {
                SaveToDB();
            }
            Program.dlfssDatabase.Unlock();
        }
        internal void ProcessFLGPacket(PacketFLG _packet)
        {
            if(_packet.OffOn > 0)
            {
                if(_packet.blueOrYellow == Racing_Flag.RACE_BLUE_FLAG)
                {
                    blueFlagActive = true;
                    lap.BlueFlagCount++;
                }
                else if(_packet.blueOrYellow == Racing_Flag.RACE_YELLOW_FLAG)
                {
                    yellowFlagActive = true;
                    lap.YellowFlagCount++;
                    if(warningDrivingCancelTimer == 0 && warningDrivingType == Warning_Driving_Type.VICTIM)
                    {
                        SendCancelWarningDriving();
                    }
                }
            }
            else
            {
                if (_packet.blueOrYellow == Racing_Flag.RACE_BLUE_FLAG)
                {
                    blueFlagActive = false;
                }
                else if (_packet.blueOrYellow == Racing_Flag.RACE_YELLOW_FLAG)
                {
                    yellowFlagActive = false;
                }
            }
            
        }
        internal void ProcessEnterGarage()                //When a car enter garage.
        {
            LeaveTrack();
        }

        private bool isTimeDiffLap = true;
        private bool isTimeDiffSplit = true;
        private bool isMaxSpeedDisplay = true;
        private bool isAdmin = false;
        private string licenceName = "";
        private byte connectionId = 0;
        private string driverName = "";
        private byte driverModel = 0;
        private uint timeTotalLastRace = 0;
        private uint timeFastestLapLastRace = 0;
        private uint lapCountTotalLastRace = 0;
        private bool yellowFlagActive = false;
        private bool blueFlagActive = false;
        private uint warningDrivingCancelTimer = 0;
        private uint warningDrivingTypeTimer = 0;
        private byte warningDrivingReferenceCarId = 0;
        private uint badDrivingCount = 0;
        private int totalLapCount = 0;
        private int totalRaceCount = 0;
        private int totalRaceFinishCount = 0;
        private uint driftScoreByTime = 0;
        private uint driftScoreTimer = 0;
        private const uint DRIFT_SCORE_TIMER = 40000;
        private Warning_Driving_Type warningDrivingType = Warning_Driving_Type.NONE;
        internal Driver_Flag driverMask = Driver_Flag.NONE;
        private Driver_Type_Flag driverTypeMask = Driver_Type_Flag.DRIVER_TYPE_NONE;
        private Leave_Reason quitReason = Leave_Reason.LEAVE_REASON_DISCONNECTED;
        private ISession iSession;
        private uint guid = 0;
        private string[] configData;
        private class Lap
        {
            internal protected Lap()
            {
            }
            ~Lap()
            {
                if (true == false) { }
            }
            internal protected void ProcessPacketLap(PacketLAP _packet, uint _raceGuid, string _carPrefix, string _trackPrefix, double _maxSpeedKmh)
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
            internal protected void ProcessPacketSplit(PacketSPX _packet)
            {
                splitTime[_packet.splitNode] = _packet.splitTime;
                totalTime = _packet.totalTime;
                SetPitStopCount(_packet.pitStopTotal);
            }

            private uint raceGuid = 0;
            private string trackPrefix = "";
            private string carPrefix = "";
            private Driver_Flag driverMask = Driver_Flag.NONE;
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

            internal Penalty_Type CurrentPenality
            {
                get { return currentPenality; }
            }
            internal Driver_Flag DriverMask
            {
                get { return driverMask; }
            }
            internal string CarPrefix
            {
                get { return carPrefix; }
            }
            internal string TrackPrefix
            {
                get { return trackPrefix; }
             }
            internal uint RaceGuid
            {
                get { return raceGuid; }
            }
            internal uint LapTime
            {
                get {return lapTime;}
            }
            internal uint[] SplitTime
            {
                get { return splitTime; }
            }
            internal ushort BlueFlagCount
            {
                set { blueFlagCount = value; }
                get { return blueFlagCount; }
            }
            internal ushort YellowFlagCount
            {
                set { yellowFlagCount = value; }
                get { return yellowFlagCount; }
            }
            internal uint TotalTime
            {
                get { return totalTime; }
            }
            internal ushort LapCompleted
            {
                get { return lapCompleted; }
            }
            internal double MaxSpeedMs
            {
                get { return maxSpeedKhm; }
            }
            internal byte PitStopCount
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
        private Lap lap = new Lap();
        private Queue<Lap> laps = new Queue<Lap>();
        internal PB pb = null;
        internal WR wr = null;
        private Dictionary<string,Dictionary<string,Rank>> rank = null;

        private static uint TIMER_WARNING_DRIVING_CHECK = 3000;
        
        private static uint TIMER_MOVING_CHECK = 300;
        private uint timerMovingCheck = 0;
        
        private static uint SAVE_INTERVAL = 60000;
        private uint driverSaveInterval = 0;
        new internal void update(uint diff) 
        {
            driverSaveInterval += diff;
            if (SAVE_INTERVAL < driverSaveInterval  )
            {
                if (!IsBot())
                {
                    Log.database(iSession.GetSessionNameForLog() + "Driver DriverGuid: " + guid + ", DriverName: " + driverName + ", licenceName:" + LicenceName + ", saved to database.\r\n");
                    Program.dlfssDatabase.Lock();
                    {
                        SaveToDB();
                    }
                    Program.dlfssDatabase.Unlock();
                    lock (laps)
                    {
                        while (laps.Count > 0)
                        {
                            Lap lapToSave = laps.Dequeue();
                            if (lapToSave.LapTime < 1 )
                                continue;
                            Program.dlfssDatabase.Lock();
                            {
                                SaveLapsToDB(lapToSave);
                            }
                            Program.dlfssDatabase.Unlock();
                            Log.database(iSession.GetSessionNameForLog() + "Lap for DriverGuid: " + guid + ", car_prefix:" + lapToSave.CarPrefix + ", track_prefix: " + lapToSave.TrackPrefix + ", saved to database.\r\n");
                        }
                        laps.Clear();
                    }
                }
                else //this is Bot Only
                    laps.Clear();
            }

            if (driftScoreByTime > 0)
            {
                if ((driftScoreTimer += diff) > DRIFT_SCORE_TIMER)
                {
                    driftScoreTimer = 0;
                    AddMessageMiddle("^2Your driftScore for the last ^7" + (DRIFT_SCORE_TIMER / 1000) + "^2sec is ^7" + driftScoreByTime.ToString(), 7000);
                    driftScoreByTime = 0;
                }
            }
            
            if (IsOnTrack())
            {
                if(timerMovingCheck < diff)
                { 
                    timerMovingCheck = TIMER_MOVING_CHECK;
                    if(x == xOld && y == yOld)
                    {
                        if(isMoving)
                            isMoving = false;
                    }
                    else
                    {
                        isMoving = true;
                        timeIldeOnTrack = 0;
                    }
                    
                    xOld = x;
                    yOld = y;
                }
                else
                    timerMovingCheck -= diff;

                if (!isMoving)
                    timeIldeOnTrack += diff;
            }


            if (warningDrivingCancelTimer > 0)
            {
                if (warningDrivingCancelTimer > diff)
                    warningDrivingCancelTimer -= diff;
                else
                {
                    warningDrivingCancelTimer = 0;
                    Driver _driver = (Driver)ISession.GetDriverWith(warningDrivingReferenceCarId);
                    if(_driver != null)
                    {
                        _driver.BadDrivingCount++;
                        _driver.AddMessageMiddle("^1Undesirable driving detected and recorded.",7000);
                    }
                    RemoveCancelWarningDriving(false);
                }
            }

            if (warningDrivingType != Warning_Driving_Type.NONE)
            {
                if (warningDrivingTypeTimer > diff)
                    warningDrivingTypeTimer -= diff;
                else
                {
                    warningDrivingType = Warning_Driving_Type.NONE;
                    warningDrivingTypeTimer = 0;
                }
            }

            base.update(diff);
        }

        private void LoadFromDB()
        {
            Program.dlfssDatabase.Lock();
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `guid`,`config_data`,`warning_driving_count` FROM `driver` WHERE `licence_name`LIKE'" + ConvertX.SQLString(LicenceName) + "' AND `driver_name`LIKE'" + ConvertX.SQLString(driverName) + "'");
                if (reader.Read())
                {
                    guid = (uint)reader.GetInt32(0);
                    SetConfigData(reader.GetString(1));
                    badDrivingCount = (uint)reader.GetInt32(2);
                    reader.Close();reader.Dispose();
                    
                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT COUNT(`guid_driver`) FROM `driver_lap` WHERE `guid_driver`='"+guid+"'");
                    if(reader.Read())
                        totalLapCount = reader.GetInt32(0);
                    reader.Close(); reader.Dispose();

                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT COUNT(`guid`) FROM `race` WHERE LOCATE(' " + guid + "',`grid_order`) > 0 OR LOCATE('" + guid + " ',`grid_order`) > 0");
                    if (reader.Read())
                        totalRaceCount = reader.GetInt32(0);
                    reader.Close(); reader.Dispose();

                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT COUNT(`guid`) FROM `race` WHERE LOCATE(' " + guid + "',`finish_order`) > 0 OR LOCATE('" + guid + " ',`finish_order`) > 0");
                    if (reader.Read())
                        totalRaceFinishCount = reader.GetInt32(0);
                    reader.Close(); reader.Dispose();

                }
            }
            Program.dlfssDatabase.Unlock();
        }
        private void SaveLapsToDB(Lap lap)
        {
            string query = "INSERT INTO `driver_lap` (`guid_race`,`guid_driver`,`car_prefix`,`track_prefix`,`driver_mask`,`split_time_1`,`split_time_2`,`split_time_3`,`lap_time`,`total_time`,`lap_completed`,`max_speed_ms`,`current_penalty`,`pit_stop_count`,`yellow_flag_count`,`blue_flag_count`)";
            query += "VALUES ('" + lap.RaceGuid + "','" + guid + "','" + lap.CarPrefix + "','" + iSession.GetRaceTrackPrefix() + "','" + (byte)lap.DriverMask + "','" + lap.SplitTime[1] + "','" + lap.SplitTime[2] + "','" + lap.SplitTime[3] + "','" + lap.LapTime + "','" + lap.TotalTime + "','" + lap.LapCompleted + "','" + ConvertX.DecimalInvariant<double>(lap.MaxSpeedMs) + "','" + (byte)lap.CurrentPenality + "','" + lap.PitStopCount + "','" + lap.YellowFlagCount + "','" + lap.BlueFlagCount + "')";
            Program.dlfssDatabase.ExecuteNonQuery(query);
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `driver` WHERE `guid`=" + guid);
            Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `driver` (`guid`,`licence_name`,`driver_name`,`config_data`,`warning_driving_count`,`last_connection_time`) VALUES ('" + guid + "', '" + ConvertX.SQLString(LicenceName) + "','" + ConvertX.SQLString(driverName) + "', '" + String.Join(" ", configData) + "','"+badDrivingCount+"', '" + (System.DateTime.Now.Ticks / 10000000) + "')");
            driverSaveInterval = 0;
        }
        private bool SetNewGuid()
        {
            bool returnValue = false;
            Program.dlfssDatabase.Lock();
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `driver`");
                if (reader.Read())
                {
                    guid = reader.IsDBNull(0) ? 1 : (uint)reader.GetInt32(0) + 1;
                    reader.Dispose();
                    SaveToDB();
                    returnValue = true;
                }
            }
            Program.dlfssDatabase.Unlock();
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
               IsTimeDiffLapDisplay = true;
           else
               IsTimeDiffLapDisplay = false;

           if (GetConfigUint16(Config_User.TIMEDIFF_SPLIT) > 0)
               IsTimeDiffSplitDisplay = true;
           else
               IsTimeDiffSplitDisplay = false;

           if (GetConfigUint16(Config_User.MAX_SPEED_ON) > 0)
               IsMaxSpeedDisplay = true;
           else
               IsMaxSpeedDisplay = false;   
        }
        private void SetConfigData(string configString)
        {
            string[] configStrings = configString.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
            if(configStrings.Length != (int)Config_User.END)
            {
                configData = new string[(int)Config_User.END];
                for(byte itr = 0 ; itr < (byte)Config_User.END;itr++)
                    configData[itr] = "0";

                configData[(int)Config_User.ACCELERATION_ON] = "1";
                configData[(int)Config_User.ACCELERATION_START] = "0";
                configData[(int)Config_User.ACCELERATION_STOP] = "100";
                configData[(int)Config_User.DRIFT_SCORE_ON] = "0";
                configData[(int)Config_User.TIMEDIFF_LAP] = "1";
                configData[(int)Config_User.TIMEDIFF_SPLIT] = "1";
                configData[(int)Config_User.MAX_SPEED_ON] = "1";
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

        internal bool IsTimeDiffLapDisplay
        {
            get { return isTimeDiffLap; }
            set
            {
                isTimeDiffLap = value;
                SetConfigValue(Config_User.TIMEDIFF_LAP, (isTimeDiffLap ? "1" : "0"));
            }
        }
        internal bool IsTimeDiffSplitDisplay
        {
            get { return isTimeDiffSplit; }
            set
            {
                isTimeDiffSplit = value;
                SetConfigValue(Config_User.TIMEDIFF_SPLIT, (isTimeDiffSplit ? "1" : "0"));
            }
        }
        internal bool IsMaxSpeedDisplay
        {
            get { return isMaxSpeedDisplay; }
            set
            {
                isMaxSpeedDisplay = value;
                SetConfigValue(Config_User.MAX_SPEED_ON, (isMaxSpeedDisplay ? "1" : "0"));
            }
        }
        
        public bool HasWarningDrivingCheck()
        {
            return warningDrivingType != Warning_Driving_Type.NONE;
        }
        public void SetWarningDrivingCheck(Warning_Driving_Type _warningDrivingType, byte referenceCarId)
        {
            warningDrivingType = _warningDrivingType;
            warningDrivingTypeTimer = TIMER_WARNING_DRIVING_CHECK;
            warningDrivingReferenceCarId = referenceCarId; //TODO: licenceName hihi :) this can become not funny!
        }
        public void SendCancelWarningDriving()
        {
            Driver _driver = (Driver)ISession.GetDriverWith(warningDrivingReferenceCarId);
            if (_driver == null)
                return;

            warningDrivingTypeTimer = warningDrivingCancelTimer = 8000;
            SendUpdateButton(Button_Entry.CANCEL_WARNING_DRIVING_1);
            SendUpdateButton(Button_Entry.CANCEL_WARNING_DRIVING_2);
            SendUpdateButton((ushort)Button_Entry.CANCEL_WARNING_DRIVING_3, _driver.driverName);
 
        }
        public void RemoveCancelWarningDriving(bool isCancelClick)
        {
            warningDrivingCancelTimer = warningDrivingTypeTimer = 0;
            RemoveButton((ushort)Button_Entry.CANCEL_WARNING_DRIVING_1);
            RemoveButton((ushort)Button_Entry.CANCEL_WARNING_DRIVING_2);
            RemoveButton((ushort)Button_Entry.CANCEL_WARNING_DRIVING_3);

            Driver _driver = (Driver)ISession.GetDriverWith(warningDrivingReferenceCarId);
            if (_driver == null)
                return;

            if(isCancelClick)
                AddMessageMiddle("^2Removed Warning Driving for "+_driver.driverName,4500);
        }
 
        public bool IsYellowFlagActive()
        {
            return yellowFlagActive;
        }
        public bool IsBlueFlagActive()
        {
            return blueFlagActive;
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
        public uint GetGuid()
        {
            return guid;
        }
        public bool IsBot()
        {
            return (licenceName == "AI" || (Driver_Type_Flag.DRIVER_TYPE_AI & driverTypeMask) == Driver_Type_Flag.DRIVER_TYPE_AI);
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
        public uint GetCurrentWRTime()
        {
            if(wr == null || wr.LapTime == 0)
                return 0;
            return wr.LapTime;
        }
        public uint TimeTotalLastRace
        {
            get { return timeTotalLastRace; }
        }
        public uint TimeFastestLapLastRace
        {
            get { return timeFastestLapLastRace; }
        }
        public uint LapCountTotalLastRace
        {
            get { return lapCountTotalLastRace; }
        }
        internal uint BadDrivingCount
        {
            get{return badDrivingCount;}
            set{badDrivingCount = value;}
        }
    }
}
