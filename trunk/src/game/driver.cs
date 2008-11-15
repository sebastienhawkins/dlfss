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
        internal Driver(Session _session): base(_session)
        {
            session = _session;
        }
        ~Driver()
        {
            if (true == false) { }
        }
        internal static void ConfigApply()
        {
            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "DriverSave");
        }
        internal void Init(PacketNCN packet)
        {
            isAdmin = packet.adminStatus > 0 ? true : false;
            driverName = packet.driverName;
            driverTypeMask = packet.driverTypeMask;
            //_packet.total;// What is Total????

            licenceName = packet.licenceName;
            if (licenceName == "") //LFS Server Driver
                licenceName = "AI";
            connectionId = packet.connectionId;
           
            if ((packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";

            //Only a Host will trigger a NCN as a Bot, real AI Bot trigger only a NPL
            if (IsBot()) 
                return;

            if (isAdmin)
            {
                if(!session.IsFreezeMotdSend())
                    session.SendMSXMessage("^7Admin ^8"+driverName+"^7 has come ^2online.");
                ((Session)session).AddAdminOnline(connectionId);
            }
            SetTimeYellowMax();
            ProcessBFNClearAll(false);
            SetConfigData("");
            
            LoadFromDB();
            if (guid == 0)
            {
                if (!SetNewGuid())
                    Log.error("Error when creating a new GUID for licenceName: " + LicenceName + ", driverName: " + driverName + "\r\n");
            }
            pb = Program.pubStats.GetPB(LicenceName, "");
            rank = Ranking.GetDriverRanks(LicenceName);

            if (!session.IsFreezeMotdSend())
                SendMotd();
            SendAllStaticButton();
        }
        internal void ProcessCPR(PacketCPR packet)
        {
            if (carPlate != packet.carPlate)
                carPlate = packet.carPlate;

            if(packet.driverName != driverName)
                driverName = packet.driverName;
        }
        internal void Init(PacketNPL packet)
        {

            if (driverName != packet.driverName)    //I think should be a check != null && != then Error... like custom cheater packet
                driverName = packet.driverName;

            driverModel = packet.driverModel;

            if (driverTypeMask != packet.driverTypeMask)
                driverTypeMask = packet.driverTypeMask;
            if (LicenceName == "" && !IsBot()) //What the ???
            {
                Log.error("Driver \"" + driverName + "\", has no licence name and was Kicked, something weird happen.\r\n");
                session.SendMSTMessage("/msg ^1driver ^7" + driverName + " ^1will be kicked for wrong licence.");
                session.SendMSTMessage("/kick " + driverName);

                return;
            }
            timeIldeOnTrack = 0;
            driverMask = packet.driverMask;

            //_packet.SName; //What is that???
            //_packet.numberInRaceCar_NSURE // What is That???
            if (connectionId != packet.connectionId)
            {
                if(!IsBot())
                    Log.error("Licence.Init(PacketNPL _packet), current connectionId("+connectionId+") was not same as packet connectionId("+packet.connectionId+"), LicenceName: "+licenceName+".\r\n");
                connectionId = packet.connectionId;
            }

            if ((driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";

            
            bool firstTime = false;
            if (carPrefix != packet.carPrefix || trackPrefix != session.GetRaceTrackPrefix())
                firstTime = true;

            trackPrefix = session.GetRaceTrackPrefix();
            carPrefix = packet.carPrefix;
            carId = packet.carId;
            carPlate = packet.carPlate;
            carSkin = packet.skinName;
            addedIntakeRestriction = packet.addedIntakeRestriction;
            addedMass = packet.addedMass;
            passenger = packet.passenger;
            tyreFrontLeft = packet.tyreFrontLeft;
            tyreFrontRight = packet.tyreFrontRight;
            tyreRearLeft = packet.tyreRearLeft;
            tyreRearRight = packet.tyreRearRight;
            
            //if(session.GetRaceInProgressStatus() != Race_In_Progress_Status.RACE_PROGRESS_NONE)
            EnterRace(firstTime);

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
        internal void ProcessLapInformation(PacketLAP packet)
        {
            totalLapCount++;
            lap.ProcessPacketLap(packet, session.GetRaceGuid(), CarPrefix, session.GetRaceTrackPrefix(), maxSpeedMs);
            
           // if(packet.lapCompleted+1 == session.GetRaceLapCount())
            //    SendFlagRace(Gui_Entry.FLAG_WHITE_FINAL_LAP,120000);
            //else if (HasFlagRace(Gui_Entry.FLAG_WHITE_FINAL_LAP))
            //    RemoveFlagRace(Gui_Entry.FLAG_WHITE_FINAL_LAP);
                

            pb = Program.pubStats.GetPB(LicenceName, CarPrefix + session.GetRaceTrackPrefix());
            wr = Program.pubStats.GetWR(CarPrefix + session.GetRaceTrackPrefix());
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
                    //wr.Splits[1] = lap.SplitTime[1];
                    //wr.Splits[2] = lap.SplitTime[2];
                    //wr.Splits[3] = lap.SplitTime[3];
                    //wr.LapTime = lap.LapTime;
                    //wr.LicenceName = LicenceName;
                    ISession.AddMessageMiddleToAll("^2New WR " + ConvertX.MSTimeToHMSC(wr.LapTime, Msg.COLOR_DIFF_TOP, Msg.COLOR_DIFF_TOP) + " ^2by " + LicenceName + ", Bravo!", 8000);
                }
            }
            if(isTimeDiffDisplay)
            {
                if (pb != null && wr != null)
                {
                    AddMessageTop("^2PB " + ConvertX.MSTimeToHMSC(lapDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER) + " ^2WR " + ConvertX.MSTimeToHMSC(lapWRDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
                }
                else if (pb != null)
                {
                    AddMessageTop("^2PB " + ConvertX.MSTimeToHMSC(lapDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
                }
            }
            
            if(isMaxSpeedDisplay)
                AddMessageMiddle("^2MS ^7" + Math.Round((decimal)ConvertX.MSToKhm(maxSpeedMs), 2) + " ^7Kmh", 6000);
            maxSpeedMs = 0.0d; 

            laps.Enqueue(lap);
            lap = new Lap(packet);
        }
        internal void ProcessSplitInformation(PacketSPX packet)
        {
            //Internal Lap
            lap.ProcessPacketSplit(packet);
            
            //PubStats
            pb = Program.pubStats.GetPB(LicenceName, CarPrefix+session.GetRaceTrackPrefix());
            wr = Program.pubStats.GetWR(CarPrefix + session.GetRaceTrackPrefix());
            int splitDiff, splitWRDiff;
            splitDiff = splitWRDiff = 0;
            if (pb != null)
            {
                if (pb.Splits[packet.splitNode] > 0 && lap.SplitTime[packet.splitNode] > 0)
                    splitDiff = (int)lap.SplitTime[packet.splitNode] - (int)pb.Splits[packet.splitNode];
            }

            if(isTimeDiffSplit)
            {
                if (wr != null && pb != null)
                {
                    splitWRDiff = (int)lap.SplitTime[packet.splitNode] - (int)wr.Splits[packet.splitNode];
                    AddMessageMiddle("^2Split " + ConvertX.MSTimeToHMSC(splitDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER) + " ^2WR " + ConvertX.MSTimeToHMSC(splitWRDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
                }
                else if(pb != null)
                    AddMessageMiddle("^2Split " + ConvertX.MSTimeToHMSC(splitDiff, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 4500);
            }
        }
        internal void ProcessRESPacket(PacketRES packet)
        {
            timeTotalLastRace = packet.totalTime;
            timeFastestLapLastRace = packet.fastestLapTime;
            lapCountTotalLastRace = packet.lapCount;
        }
        internal void ProcessRaceStart()
        {
            if(IsRacing())
                StartRacing();
        }
        internal void ProcessTrackChange()
        {
            trackPrefix = session.GetRaceTrackPrefix();
            ClearAllStaticButton();
            SendAllStaticButton();
        }
        internal void ProcessRaceEnd()
        {
            LeaveRacing();
        }
        internal void ProcessLeaveRace(PacketPLL packet)  //to be called when a car is removed from a race
        {
            carId = 0;
            LeaveRace();
            
            if(IsBot())
            {
                botGuid[guid-(uint)Bot_GUI.FIRST] = false;
                guid = 0;
            }
        }
        internal void ProcessCNLPacket(PacketCNL packet) //Disconnect
        {
            quitReason = packet.quitReason;
            
            //Only a host will trigger this a real bot trigger only a Leave race.
            if(IsBot())
                return;

            if (isAdmin)
                session.RemoveAdminOnline(connectionId);

            Program.dlfssDatabase.Lock();
            {
                SaveToDB();
            }
            Program.dlfssDatabase.Unlock();
        }
        internal void ProcessFLGPacket(PacketFLG packet)
        {
            if(packet.OffOn > 0)
            {
                if(packet.blueOrYellow == LFS_Race_Flag.BLUE)
                {
                    SendFlagRace(Gui_Entry.FLAG_BLUE_SLOW_CAR,999000);
                    lfsRaceFlag |= LFS_Race_Flag.YELLOW;
                    blueFlagActive = true;
                    lap.BlueFlagCount++;
                }
                else if(packet.blueOrYellow == LFS_Race_Flag.YELLOW)
                {
                    yellowFlagActive = true;
                    lfsRaceFlag |= LFS_Race_Flag.BLUE;
                    SendFlagRace(Gui_Entry.FLAG_YELLOW_LOCAL, 999000);
                    lap.YellowFlagCount++;
                    TrySendCancelWarning();
                }
            }
            else
            {
                if (packet.blueOrYellow == LFS_Race_Flag.BLUE)
                {
                    blueFlagActive = false;
                    lfsRaceFlag ^= LFS_Race_Flag.YELLOW;
                    RemoveFlagRace(Gui_Entry.FLAG_BLUE_SLOW_CAR);
                }
                else if (packet.blueOrYellow == LFS_Race_Flag.YELLOW)
                {
                    RemoveFlagRace(Gui_Entry.FLAG_YELLOW_LOCAL);
                    lfsRaceFlag ^= LFS_Race_Flag.BLUE;
                    yellowFlagActive = false;
                }
            }
            
        }
        internal void ProcessPLAPacket(PacketPLA packet)
        {
            if(packet.action == Pit_Lane_Action.EXIT)
                isInPit = false;
        }
        internal void ProcessPITPacket(PacketPIT packet)
        {
            isInPit = true;
            driverMask = packet.driverMask;
        }
        internal void ProcessPFLPacket(PacketPFL packet)
        {
            driverMask = packet.driverMask;
        }
        internal void ProcessPenality(PacketPEN packet)
        {
            penalityCurrent = packet.penalityCurrent;
            penalityOld = packet.penalityOld;
            penalityReason = packet.penalityReason;
        }
        internal void ProcessEnterGarage()                //When a car enter garage.
        {
            LeaveRace();
        }


        private bool isTimeDiffDisplay = true;
        private bool isTimeDiffSplit = true;
        private bool isMaxSpeedDisplay = true;
        private bool isAdmin = false;
        private string licenceName = "";
        private byte connectionId = 0;
        private string driverName = "";
        private byte driverModel = 0;
        private byte racePosition = 0;
        private uint timeTotalLastRace = 0;
        private uint timeFastestLapLastRace = 0;
        private uint lapCountTotalLastRace = 0;
        private bool yellowFlagActive = false;
        private bool blueFlagActive = false;
        private uint warningDrivingCancelTimer = 0;
        private uint warningDrivingTypeTimer = 0;
        private byte warningDrivingReferenceCarId = 0;
        private int badDrivingCount = 0;
        private int warningChatCount = 0;
        private int floodChatCount = 0;
        private int totalLapCount = 0;
        private int totalRaceCount = 0;
        private int totalRaceFinishCount = 0;
        private uint driftScoreByTime = 0;
        private uint driftScoreTimer = 0;
        private int safePct = 0;
        private int oldSafePct = 0;
        private bool isStupidDriving = false;
        private Penalty_Type  penalityOld = Penalty_Type.NONE;
        private Penalty_Type penalityCurrent = Penalty_Type.NONE;
        private Penalty_Reason penalityReason = Penalty_Reason.NONE;
        private const uint DRIFT_SCORE_TIMER = 40000;
        private Warning_Driving_Type warningDrivingType = Warning_Driving_Type.NONE;
        internal Driver_Flag driverMask = Driver_Flag.NONE;
        private Driver_Type_Flag driverTypeMask = Driver_Type_Flag.DRIVER_TYPE_NONE;
        private Leave_Reason quitReason = Leave_Reason.LEAVE_REASON_DISCONNECTED;
        private uint guid = 0;
        private string[] configData;
        private class Lap
        {
            internal protected Lap()
            {
            }
            internal protected Lap(PacketLAP packet)
            {
                totalTime = packet.totalTime;
                lapCompleted = packet.lapCompleted;
            }
            ~Lap()
            {
                if (true == false) { }
            }
            internal protected void ProcessPacketLap(PacketLAP packet, uint _raceGuid, string _carPrefix, string _trackPrefix, double _maxSpeedKmh)
            {
                lapTime = packet.lapTime;
                totalTime = packet.totalTime;
                SetPitStopCount(packet.pitStopTotal);
                carPrefix = _carPrefix;
                trackPrefix = _trackPrefix;
                raceGuid = _raceGuid;
                driverMask = packet.driverMask;
                currentPenality = packet.currentPenality;
                lapCompleted = packet.lapCompleted;
                maxSpeedKhm = _maxSpeedKmh;
            }
            internal protected void ProcessPacketSplit(PacketSPX packet)
            {
                splitTime[packet.splitNode] = packet.splitTime;
                totalTime = packet.totalTime;
                SetPitStopCount(packet.pitStopTotal);
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
            private Penalty_Type currentPenality = Penalty_Type.NONE;
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
        internal PST pst = null;
        private Dictionary<string,Dictionary<string,Rank>> rank = null;


        private static uint TIMER_WARNING_DRIVING_CHECK = 1700;
        private static uint WARNING_DRIVING_CANCEL_TIMER = 8000;
        
        private static uint TIMER_300_CHECK = 300;
        private uint timer300Check = 0;

        private uint timeIldeOnTrack = 0;

        private bool timeYellowSend = false;
        private uint timeYellowFlag = 0;
        private uint TIME_YELLOW_MAX = 30000;
        private uint TIME_YELLOW_1_10 = 3000;
        private uint TIME_YELLOW_1_3 = 10000;
        private uint TIME_YELLOW_2_3 = 20000;
        public void SetTimeYellowMax()
        {
            timeYellowFlag = 0;
            
            TIME_YELLOW_MAX = session.GetTimeYellowMax();
            TIME_YELLOW_1_10 = TIME_YELLOW_MAX / 10;
            TIME_YELLOW_1_3 = TIME_YELLOW_MAX / 3;
            TIME_YELLOW_2_3 = (uint)(TIME_YELLOW_MAX / 1.5d);
        }
        
        private static uint SAVE_INTERVAL = 60000;
        private uint driverSaveInterval = 0;
        new internal void update(uint diff) 
        {
            driverSaveInterval += diff;
            if (SAVE_INTERVAL < driverSaveInterval  )
            {
                if (!IsBot())
                {
                    Log.database(session.GetSessionNameForLog() + "Driver DriverGuid: " + guid + ", DriverName: " + driverName + ", licenceName:" + LicenceName + ", saved to database.\r\n");
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
                            Log.database(session.GetSessionNameForLog() + "Lap for DriverGuid: " + guid + ", car_prefix:" + lapToSave.CarPrefix + ", track_prefix: " + lapToSave.TrackPrefix + ", saved to database.\r\n");
                        }
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
            if(!IsMoving())
                timeIldeOnTrack += diff;
            else
                timeIldeOnTrack = 0;    
            
            if (HasLFSRaceFlag(LFS_Race_Flag.YELLOW))
                timeYellowFlag += diff;
            else if (timeYellowFlag > 0)
                timeYellowFlag = 0;
 

            if(timer300Check < diff)
            {
                timer300Check = TIMER_300_CHECK;
                if (IsRacing())
                {
                    //Idle Check
                    if (x == xOld && y == yOld && isMoving)
                       isMoving = false;
                    else if(!isMoving)
                        isMoving = true;
                    xOld = x;
                    yOld = y;
                }
                
                //Yellow Time Check
                if (timeYellowFlag > TIME_YELLOW_1_10)
                {
                    timeYellowSend = true;

                    if (timeYellowFlag > TIME_YELLOW_MAX)
                    {
                        timeYellowSend = false;
                        RemoveButton(Button_Entry.INFO_2);
                        session.SendMSTMessage("/spec " + driverName);
                    }
                    else if (timeYellowFlag > TIME_YELLOW_2_3)
                        SendUpdateButton(Button_Entry.INFO_2, "^1Yellow Time ^7" + Math.Round(((double)timeYellowFlag / 1000.0d), 1));
                    else if (timeYellowFlag > TIME_YELLOW_1_3)
                        SendUpdateButton(Button_Entry.INFO_2, "^3Yellow Time ^7" + Math.Round(((double)timeYellowFlag / 1000.0d), 1));
                    else
                        SendUpdateButton(Button_Entry.INFO_2, "^2Yellow Time ^7" + Math.Round(((double)timeYellowFlag / 1000.0d), 1));
                }
                else if (timeYellowSend && !HasLFSRaceFlag(LFS_Race_Flag.YELLOW))
                {
                    timeYellowSend = false;
                    RemoveButton(Button_Entry.INFO_2);
                }
                //FinalLap Check
                if (IsRacing() && session.GetRaceInProgressStatus() != Race_In_Progress_Status.RACE_PROGRESS_QUALIFY && lap.LapCompleted + 1 == session.GetRaceLapCount())
                {
                    if(!HasFlagRace(Gui_Entry.FLAG_WHITE_FINAL_LAP))
                        SendFlagRace(Gui_Entry.FLAG_WHITE_FINAL_LAP,999999);
                }
                else if(HasFlagRace(Gui_Entry.FLAG_WHITE_FINAL_LAP))
                    RemoveFlagRace(Gui_Entry.FLAG_WHITE_FINAL_LAP);   
                
                //PenalityFlag Check
                if (IsRacing() && (penalityCurrent == Penalty_Type.PIT_STOP
                    || penalityCurrent == Penalty_Type.DRIVE_THROUGH
                    || penalityCurrent == Penalty_Type.ADD_30_SEC
                    || penalityCurrent == Penalty_Type.ADD_45_SEC))
                {
                    if (!HasFlagRace(Gui_Entry.FLAG_BLACK_PENALITY))
                        SendFlagRace(Gui_Entry.FLAG_BLACK_PENALITY, 999999);
                }
                else if(HasFlagRace(Gui_Entry.FLAG_BLACK_PENALITY))
                    RemoveFlagRace(Gui_Entry.FLAG_BLACK_PENALITY);  
            }
            else
                timer300Check -= diff;
            
            if (warningDrivingCancelTimer > 0)
            {
                if (warningDrivingCancelTimer > diff)
                    warningDrivingCancelTimer -= diff;
                else
                {
                    warningDrivingCancelTimer = 0;
                    Driver driver = (Driver)session.GetCarId(warningDrivingReferenceCarId);
                    if(driver != null)
                    {
                        driver.AddBadDriving();
                        driver.SetSafePct();
                        session.SendMTCMessageToAllAdmin("^1Bad ^7driving ^8"+driver.DriverName +"^7 safe(^4"+ driver.GetSafePct()+"^7)%.");
                        if (driver.GetSafePct() < -350)
                        {
                            driver.SendMTCMessage("^4" + driver.GetSafePct() + "^7% is too low.");
                            session.SendMSXMessage("^1Banned " + driver.DriverName + "^7 1 days for Bad driving.");
                            if(!driver.IsAdmin)
                                session.SendMSTMessage("/ban " + driver.LicenceName+" 1");
                        }
                        else if (driver.GetSafePct() < -100)
                        {
                            driver.SendMTCMessage("^4" + driver.GetSafePct() + "^7% is too low.");
                            session.SendMSXMessage("^1Kicked " + driver.DriverName + "^7 for Bad driving.");
                            if (!driver.IsAdmin)
                                session.SendMSTMessage("/kick " + driver.LicenceName);
                        }
                        else if (driver.GetSafePct() < 0)
                        {
                            driver.SendMTCMessage("^4" + driver.GetSafePct() + "^7% is very low.");
                            driver.SendMTCMessage("^7You ^1MUST ^7stay ^1CLEAN ^7at ^1ALL COST^7.");
                            session.SendMSXMessage("^1Spec " + driver.DriverName + "^7 for Bad driving.");
                            if (!driver.IsAdmin)
                                session.SendMSTMessage("/spec " + driver.LicenceName);
                        }
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
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `guid`,`config_data`,`warning_driving_count`,`warning_chat_count`,`flood_chat_count` FROM `driver` WHERE `licence_name`LIKE'" + ConvertX.SQLString(licenceName) + "'");
                if (reader.Read())
                {
                    guid = (uint)reader.GetInt32(0);
                    SetConfigData(reader.GetString(1));
                    badDrivingCount = reader.GetInt32(2);
                    warningChatCount = reader.GetInt32(3);
                    floodChatCount = reader.GetInt32(4);
                    reader.Close();reader.Dispose();
                    
                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT COUNT(`guid_driver`) FROM `driver_lap` WHERE `guid_driver`='"+guid+"'");
                    if(reader.Read())
                        totalLapCount = reader.GetInt32(0);
                    reader.Close(); reader.Dispose();

                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT COUNT(`guid`) FROM `race` WHERE (LOCATE(' " + guid + "',`grid_order`) > 0 OR LOCATE('" + guid + " ',`grid_order`) > 0) AND `race_laps`>1");
                    if (reader.Read())
                        totalRaceCount = reader.GetInt32(0);
                    reader.Close(); reader.Dispose();

                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT COUNT(`guid`) FROM `race` WHERE (LOCATE(' " + guid + "',`finish_order`) > 0 OR LOCATE('" + guid + " ',`finish_order`) > 0) AND `race_laps`>1");
                    if (reader.Read())
                        totalRaceFinishCount = reader.GetInt32(0);
                    reader.Close(); reader.Dispose();

                }
            }
            Program.dlfssDatabase.Unlock();
            SetSafePct();
        }
        private void SaveLapsToDB(Lap lap)
        {
            string query = "INSERT INTO `driver_lap` (`guid_race`,`guid_driver`,`car_prefix`,`track_prefix`,`driver_mask`,`split_time_1`,`split_time_2`,`split_time_3`,`lap_time`,`total_time`,`lap_completed`,`max_speed_ms`,`current_penalty`,`pit_stop_count`,`yellow_flag_count`,`blue_flag_count`)";
            query += "VALUES ('" + lap.RaceGuid + "','" + guid + "','" + lap.CarPrefix + "','" + session.GetRaceTrackPrefix() + "','" + (byte)lap.DriverMask + "','" + lap.SplitTime[1] + "','" + lap.SplitTime[2] + "','" + lap.SplitTime[3] + "','" + lap.LapTime + "','" + lap.TotalTime + "','" + lap.LapCompleted + "','" + ConvertX.DecimalInvariant<double>(lap.MaxSpeedMs) + "','" + (byte)lap.CurrentPenality + "','" + lap.PitStopCount + "','" + lap.YellowFlagCount + "','" + lap.BlueFlagCount + "')";
            Program.dlfssDatabase.ExecuteNonQuery(query);
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `driver` WHERE `guid`=" + guid);
            Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `driver` (`guid`,`licence_name`,`driver_name`,`config_data`,`warning_driving_count`,`warning_chat_count`,`flood_chat_count`,`last_connection_time`) VALUES ('" + guid + "', '" + ConvertX.SQLString(LicenceName) + "','" + ConvertX.SQLString(driverName) + "', '" + String.Join(" ", configData) + "','"+badDrivingCount+"', '" +warningChatCount + "', '" +floodChatCount + "', '" + (System.DateTime.Now.Ticks / 10000000) + "')");
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
            get { return session; }
        }
        internal Rank GetRank(string trackPrefix, string carPrefix)
        {
            if( rank != null && rank.ContainsKey(trackPrefix) && rank[trackPrefix].ContainsKey(carPrefix))
                return rank[trackPrefix][carPrefix];
            else
                return null;
        }
        public void SetSafePct()
        {
            oldSafePct = safePct;
            safePct = SetSafePct(badDrivingCount, totalRaceFinishCount, totalLapCount);
        }
        internal int SetSafePct(int badDriving, int raceFinish, int lapCount)
        {
            int _safePct = (badDriving / (raceFinish > 0 ? (raceFinish*2) : 1)) * 25;
            _safePct += (int)((badDriving / (lapCount > 0 ? ((double)lapCount / 10.0d) : 1)) * 75);
            _safePct = 102 - _safePct;
            if (_safePct > 100)
                _safePct = 100;
            return _safePct;
        }
        public int GetSafePct()
        {
            return safePct;
        }
        public void SaySafe()
        {
            if (oldSafePct > safePct)
                session.SendMSTMessage("/msg " + driverName + " ^2is now '^4" + safePct + "^7%^2' safe.");
            else if (oldSafePct < safePct)
                session.SendMSTMessage("/msg " + driverName + " ^2is now '^3" + safePct + "^7%^2' safe.");
           
        }     
        internal bool IsTimeDiffLapDisplay
        {
            get { return isTimeDiffDisplay; }
            set
            {
                isTimeDiffDisplay = value;
                SetConfigValue(Config_User.TIMEDIFF_LAP, (isTimeDiffDisplay ? "1" : "0"));
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
        private void ClearPenalty()
        {
            penalityCurrent = Penalty_Type.NONE;
            penalityOld = Penalty_Type.NONE;
            penalityReason = Penalty_Reason.NONE;
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
        public void TrySendCancelWarning()
        {       
            if(warningDrivingCancelTimer == 0 && warningDrivingType == Warning_Driving_Type.VICTIM)
                SendCancelWarningDriving();
        }             
        private void SendCancelWarningDriving()
        {
            Driver _driver = (Driver)ISession.GetCarId(warningDrivingReferenceCarId);
            if (_driver == null)
                return;

            warningDrivingTypeTimer = warningDrivingCancelTimer = WARNING_DRIVING_CANCEL_TIMER;
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

            Driver _driver = (Driver)ISession.GetCarId(warningDrivingReferenceCarId);
            if (_driver == null)
                return;

            if(isCancelClick)
                AddMessageMiddle("^2Removed Warning Driving for "+_driver.driverName,4500);
        }
        private void EnterPit()
        {

        }
        public bool IsRacing()
        {
            return (carId > 0 && isInRace && session.GetRaceInProgressStatus() != Race_In_Progress_Status.RACE_PROGRESS_NONE);
        }
        public bool IsInRace()
        {
            return (carId > 0 && isInRace);
        }
        internal void LeaveRace()
        {
            LeaveRacing();
            isInRace = false;
        }
        internal void LeaveRacing()
        {
            isLostControl = false;
            ClearPenalty();
            SimulateLastMCI();
            SendAllStaticButton();
            RemoveFlagRaceGuiAll();
            RemoveNodeBar();
        }
        private void EnterRace(bool firstTime)
        {
            isInRace = true;

            if(IsRacing())
                ClearAllStaticButton();

            if (firstTime)
                EnterRaceFirstTime();
               
            if(session.GetRaceInProgressStatus() == Race_In_Progress_Status.RACE_PROGRESS_RACING && !session.IsInRaceStartGrid(guid))
                SendFlagRace(Gui_Entry.FLAG_BLACK_NO_SCORE,10000);
        }
        private void EnterRaceFirstTime()
        {
            wr = Program.pubStats.GetWR(carPrefix + ISession.GetRaceTrackPrefix());
            if (wr != null)
            {
                //lapTime = lapTime.Insert();
                AddMessageMiddle("^2WR " + ConvertX.MSTimeToHMSC(wr.LapTime, Msg.COLOR_DIFF_TOP, Msg.COLOR_DIFF_TOP) + " ^2by^ " + wr.LicenceName, 6000);
            }

            pb = Program.pubStats.GetPB(licenceName, carPrefix + ISession.GetRaceTrackPrefix());
            if (pb != null && wr != null)
            {
                AddMessageMiddle("^2PB " + ConvertX.MSTimeToHMSC(pb.LapTime, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + " ^2WR " + ConvertX.MSTimeToHMSC(pb.LapTime - wr.LapTime, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 7000);
            }
            else if (pb != null)
            {
                AddMessageMiddle("^2PB " + ConvertX.MSTimeToHMSC(pb.LapTime, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT), 7000);
            }

            Rank _rank = GetRank(ISession.GetRaceTrackPrefix(), CarPrefix);
            if (_rank != null)
            {
                if (!session.IsFreezeMotdSend())
                    session.SendMSTMessage("/msg " + driverName + " ^2" + _rank.GetStabilityComment() /*+ "^2 with ^7" + carPrefix*/);
                //AddMessageTop("^2Rank Detail, ^2BL^7"+_rank.BestLap+" ^2AV^7"+_rank.AverageLap+" ^2ST^7"+_rank.Stability+" ^2WI^7"+_rank.RaceWin,5000);
            }
            else
            {
                if (!session.IsFreezeMotdSend())
                    session.SendMSTMessage("/msg " + driverName + " ^2is " + (IsBot() ? "a ^7BOT" : "^7new")/*+"^2 with ^7" + carPrefix*/);
                //AddMessageTop("^2Rank Detail, you have no rank for ^7"+((Driver)this).ISession.GetRaceTrackPrefix()+" ^2with car ^7"+carPrefix,3000);
            }
            SetSafePct();
            if (!session.IsFreezeMotdSend() && safePct < 100)
                session.SendMSTMessage("/msg ^2    and '^7" + safePct + "%^2' safe.");
        }
        private void StartRacing()
        {
            //We reset last call , just be sure we don't keep last result
            wr = Program.pubStats.GetWR("");
            maxSpeedMs = 0.0d;
            lap = new Lap();

            EnterRace(false);
            ClearPenalty();
            ClearAllStaticButton();
            if(currentGui == Gui_Entry.RESULT)
                RemoveResultGui();
        }
        public void FinishRace()
        {
            totalRaceFinishCount++;
            SetSafePct();
            SaySafe();
           // if (((Session)((Driver)this).ISession).script.CarFinishRace((ICar)this))
           //    return;
        }
        public bool HasLFSRaceFlag(LFS_Race_Flag _lfsRaceFlag)
        {
            return (lfsRaceFlag&_lfsRaceFlag) == _lfsRaceFlag;
        }
        public bool HasYellowFlagActive()
        {
            return HasLFSRaceFlag(LFS_Race_Flag.YELLOW);
        }
        public bool IsBlueFlagActive()
        {
            return HasLFSRaceFlag(LFS_Race_Flag.BLUE);
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

            ((Session)session).AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MTC, Packet_Type.PACKET_MTC_CHAT_TO_LICENCE, _packet));
       
            //Log.progress("Sending MTC packet to: " + CarId + ", with ConnectionId: " + ConnectionId + "\r\n");
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
        public byte RacePosition
        {
            set{racePosition = value;}
            get{return racePosition;}
        }
        public byte GetRacePosition()
        {
            return RacePosition;
        }
        public int GetBadDrivingCount()
        {
            return badDrivingCount;
        }
        public void AddBadDriving()
        {
            badDrivingCount++;
        }
        public void AddWarningChat()
        {
            warningChatCount++;
        }
        public int GetWarningChatCount()
        {
            return warningChatCount;
        }
        public void AddFloodChat()
        {
            floodChatCount++;
        }
        public int GetFloodChatCount()
        {
            return floodChatCount;
        }
        public bool IsStupidDriving()
        {
            return isStupidDriving;
        }
        public void SetStupidDriving(bool value)
        {
            if (isStupidDriving != value)
            {
                isStupidDriving = value;
                if (isStupidDriving)
                    SendUpdateButton(Button_Entry.INFO_4, "^1StupidDriving");
                else
                    SendUpdateButton(Button_Entry.INFO_4, "^2SmartDriving");
            }
        }

        private string tracjectoryDisplay = "";
        public void SendTrajDisplay(string trajDisplay)
        {
            if (tracjectoryDisplay == trajDisplay)
                return;
            
            tracjectoryDisplay = trajDisplay;
            //SendUpdateButton((ushort)Button_Entry.NODE_TRAJ_TO_TRACK, trajDisplay);
        }
        private string orientationDisplay = "";
        public void SendOriDisplay(string oriDisplay)
        {
            if (orientationDisplay == oriDisplay)
                return;
            orientationDisplay = oriDisplay;
            //SendUpdateButton((ushort)Button_Entry.NODE_ORIE_TO_TRACK, oriDisplay);
        }
        private string pathDisplay = "";
        public void SendPathDisplay(string _pathDisplay)
        {
            if (pathDisplay == _pathDisplay)
                return;
            pathDisplay = _pathDisplay;
            //SendUpdateButton((ushort)Button_Entry.NODE_POS_TO_PATH, pathDisplay);
        }
    }
}
