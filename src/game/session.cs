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
using System;
using System.Collections.Generic;
using System.Text;

namespace Drive_LFSS.Game_
{
    using Server_;
    using InSim_;
    using Packet_;
    using Definition_;
    using Script_;
    using Log_;
    using PubStats_;
    using ChatModo_;

    class Session : InSimClient, ISession
    {
        internal Session(string _serverName, InSimSetting _inSimSetting): base( _inSimSetting)
        {
            sessionName = _serverName;
            commandPrefix = _inSimSetting.commandPrefix;
            race = new Race(this);
            driverList = new List<Driver>();
            driverList.Capacity = 255;
            //driverList.Add(new Driver((ISession)this)); //put Default Driver 0, will save some If.
            //driverList.Capacity = 256;
            script = new Script(this);
            ping = new Ping();
            command = new CommandInGame(this);
            chatModo = new ChatModo(this);
            connectionRequest = true;
            if (!LoadFromDB())
            {
                SetConfigData("");
                SaveToDB();
            }
        }
        ~Session()
        {
            if (true == false) { }
        }
        internal void ConfigApply(InSimSetting _inSimSetting)
        {
            base.SetInSimSetting(_inSimSetting);
            ConfigApply(false);
        }
        private void ConfigApply(bool onlyVoteSystem)
        {
            if (onlyVoteSystem)
            {
                race.ConfigApply();
                return;
            }

            base.ConfigApply();
            race.ConfigApply();
            Driver.ConfigApply();
            ReloadMessage();
        }
        internal void ConfigApplyToVote()
        {
            ConfigApply(true);        
        }
        
        private class Ping
        {
            public Ping()
            {
            }
            private DateTime pingTime;
            private TimeSpan sessionLatency;
            uint diff = 0;
            public void Sending(uint diff)
            {
                pingTime = DateTime.Now;
            }
            public double Received()
            {
                return (sessionLatency = (DateTime.Now - pingTime)).TotalMilliseconds;
            }
            public int GetLatency()
            {
                return (int)sessionLatency.TotalMilliseconds;
            }
            public int GetReactionTime()
            {
                return (int)(sessionLatency.TotalMilliseconds - diff);
            }
        }

        private Ping ping;
        private string sessionName;
        private char commandPrefix;
        internal bool connectionRequest;
        private byte clientConnectionCount = 0;
        private uint freezeMotdSend = 7000;
        private List<byte> adminOnlineList = new List<byte>();
        private Script script;
        private CommandInGame command;
        private Race race;
        private List<Driver> driverList;
        private ChatModo chatModo;
        private string motdMessage = "";
        private string rulesMessage = "";
        private string[] configData = new string[(int)Config_Session.END];
        private uint TIME_YELLOW_MAX = 30000;

        public string GetMotdMessage()
        {
            return motdMessage;
        }
        public string GetRulesMessage()
        {
            return rulesMessage;
        }
        private void ApplyConfigData()
        {
            TIME_YELLOW_MAX = GetConfigUint32(Config_Session.YELLOW_TIME);

            uint entry = GetConfigUint32(Config_Session.RESTRICTION_JOIN_ENTRY);
            if (entry != 0)
            {
                SendMSXMessage("^7Join Restriction has been ^2Reloaded^7.");
                race.LoadRestrictionJoin(entry);
            }
            entry = GetConfigUint32(Config_Session.RESTRICTION_RACE_ENTRY);
            if (entry != 0)
            {
                SendMSXMessage("^7Race Restriction has been ^2Reloaded^7.");
                race.LoadRestrictionRace(entry);
            }
            
        }
        private void SetConfigData(string configString)
        {
            string[] configStrings = configString.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (configStrings.Length != (int)Config_Session.END)
            {
                configData = new string[(int)Config_Session.END];
                for (byte itr = 0; itr < (byte)Config_Session.END; itr++)
                    configData[itr] = "0";

                configData[(int)Config_Session.YELLOW_TIME] = "30000";
            }
            configStrings.CopyTo(configData, 0);
        }
        internal ushort GetConfigUint16(Config_Session index)
        {
            return Convert.ToUInt16(configData[(int)index]);
        }
        internal uint GetConfigUint32(Config_Session index)
        {
            return Convert.ToUInt32(configData[(int)index]);
        }
        internal string GetConfigString(Config_Session index)
        {
            return configData[(int)index];
        }
        internal void SetConfigValue(Config_Session index, string value)
        {
            configData[(int)index] = value;
        }
        private bool LoadFromDB()
        {
            bool _return = false;
            Program.dlfssDatabase.Lock();
            {
                System.Data.IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `config_data`,`motd_message`,`rules_message` FROM `session` WHERE `session_name`LIKE'" + sessionName + "'");
                if (reader.Read())
                {
                    SetConfigData(reader.GetString(0));
                    motdMessage = reader.GetString(1);
                    rulesMessage = reader.GetString(2);
                    _return = true;
                }
                reader.Close(); reader.Dispose();
            }
            Program.dlfssDatabase.Unlock();
            
            return _return;
        }
        public void ReloadMessage()
        {
            Program.dlfssDatabase.Lock();
            {
                System.Data.IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `motd_message`,`rules_message` FROM `session` WHERE `session_name`LIKE'" + sessionName + "'");
                if (reader.Read())
                {
                    motdMessage = reader.GetString(0);
                    rulesMessage = reader.GetString(1);
                }
                reader.Close(); reader.Dispose();
            }
            Program.dlfssDatabase.Unlock();
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `session` WHERE `session_name`LIKE'" + sessionName +"'");
            Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `session` (`session_name`,`config_data`,`motd_message`,`rules_message`) VALUES ('" + sessionName + "', '" + String.Join(" ", configData) + "','" + ConvertX.SQLString(motdMessage) + "', '" + ConvertX.SQLString(rulesMessage) + "')");
            sessionSaveInterval = 0;
        }
        public void LoadRace(ushort entry)
        {
            race.LoadNextTrack(entry);
        }
        public bool IsFreezeMotdSend()
        {
            return freezeMotdSend > 0;
        }
        private void CommandExec(Driver driver, string _commandText)
        {
            command.Exec(driver, _commandText);
        }
        private void PingReceived()
        {
            double msTime = ping.Received();
            #if DEBUG
            Log.network(GetSessionNameForLog() + " Pong! " +  msTime + "ms\r\n");
            #endif
        }
        public int GetLatency()
        {
            return ping.GetLatency();
        }
        public int GetReactionTime()
        {
            return ping.GetReactionTime();
        }
        public string GetSessionNameForLog()
        {
            return "["+sessionName+">";
        }
        public string GetSessionName()
        {
            return sessionName;
        }
        internal Dictionary<string,int> GetRaceLastResult()
        {
            return race.GetLastResultString();
        }
        public uint GetRaceGuid()
        {
            return race.GetGuid();
        }
        public string GetRaceTrackPrefix()
        {
            return race.GetTrackPrefix();
        }
        public byte GetRaceLapCount()
        {
            return race.GetLapCount();
        }
        public Race_In_Progress_Status GetRaceInProgressStatus()
        {
            return race.GetInProgressStatus();
        }
        public bool IsInRaceStartGrid(uint driverGuid)
        {
            return race.StartGridHasDriverGuid(driverGuid);
        }
        internal List<Driver> GetDriverList()
        {
            return driverList;
        }
        public void SendMSTMessage(string message)
        {
            AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(message)));
        }
        public void SendMSXMessage(string message)
        {
            AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MSX, Packet_Type.PACKET_MSX_SEND_BIG_CHAT, new PacketMSX(message)));
        }
        public void SendMTCMessageToAllAdmin(string message)
        {
            if(adminOnlineList.Count < 1)
                return;
                
           foreach(byte connectionId in adminOnlineList)
           {
               byte index = GetConnectionIdNotBot(connectionId);
               if(index != 255)
               {
                    driverList[index].SendMTCMessage(message);
               }
           }
        }
        public void SendUpdateButtonToAll(ushort buttonEntry, string text)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].SendUpdateButton(buttonEntry, text);
        }
        public void SendUpdateGuiToAll(Gui_Entry guiEntry, string text)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].SendUpdateGui(guiEntry, text);
        }
        public void SetTimeYellowMax(uint value)
        {
            TIME_YELLOW_MAX = value;
            SetConfigValue(Config_Session.YELLOW_TIME,value.ToString());
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].SetTimeYellowMax();
        }
        public uint GetTimeYellowMax()
        {
            return TIME_YELLOW_MAX;
        }
        public void SendResultGuiToAll(Dictionary<string, int> scoringResultTextDisplay)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
            {
                if (!driverList[itr].HasAGuiDisplay())
                    driverList[itr].SendResultGui(scoringResultTextDisplay);
            }
        }
        public void SendFlagRaceToAll(ushort guiEntry, uint time)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].SendFlagRace(guiEntry,time);
        }
        public void RemoveFlagRaceToAll(ushort guiEntry)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].RemoveRaceFlag(guiEntry);
        }
        public void AddMessageTopToAll(string text, uint duration)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].AddMessageTop(text, duration);
        }
        public void AddMessageMiddleToAll(string text, uint duration)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].AddMessageMiddle(text,duration);
        }
        public void RemoveButtonToAll(ushort buttonEntry)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].RemoveButton(buttonEntry);
        }
        public void RemoveButton(ushort buttonEntry, byte connectionId)
        {
            driverList[GetConnectionIdNotBot(connectionId)].RemoveButton(buttonEntry);
        }
        public bool IsRaceInProgress()
        {
            return race.IsRaceInProgress();
        }
        public bool CanVote()
        {
            return race.CanVote();
        }
        public void EndRace()
        {
            race.EndRace();
        }
        public Script Script
        {
            get { return script; }
        }
        internal void AddAdminOnline(byte connectionId)
        {
            if(!adminOnlineList.Contains(connectionId))
                adminOnlineList.Add(connectionId);
        }
        internal void RemoveAdminOnline(byte connectionId)
        {
            if (!adminOnlineList.Contains(connectionId))
                adminOnlineList.Remove(connectionId);
        }
        public bool HasAdminOnline()
        {
            return adminOnlineList.Count > 0;
        }
        private byte GetCarIndex(byte _carId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((ICar)driverList[itr]).CarId == _carId)
                    return itr;
            }
            return 255;
        }
        private List<byte> GetLicenceIndexs(byte connectionId)
        {
            List<byte> _return = new List<byte>();

            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((IDriver)driverList[itr]).ConnectionId == connectionId)
                    _return.Add(itr);
            }
            return _return;
        }
        private byte GetDriverIndexWith(byte connectionId, string _driverName)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((IDriver)driverList[itr]).ConnectionId == connectionId && ((IDriver)driverList[itr]).DriverName == _driverName)
                    return itr;
            }
            return 255;
        }
        private byte GetConnectionIdNotBot(byte connectionId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (driverList[itr].ConnectionId == connectionId && !driverList[itr].IsBot())
                    return itr;
            }
            return 255;
        }
        private byte GetFirstConnectionIdIndex(byte connectionId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((IDriver)driverList[itr]).ConnectionId == connectionId)
                    return itr;
            }
            return 255;
        }
        private bool IsExistconnectionId(byte connectionId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((IDriver)driverList[itr]).ConnectionId == connectionId)
                    return true;
            }
            return false;
        }
        public IDriver GetCarId(byte carId)
        {
            byte carIndex = GetCarIndex(carId);
            if (carIndex != 255)
                return driverList[carIndex];
            return null;
        }
        public IDriver GetDriverWithGuid(uint guid)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (driverList[itr].GetGuid() == guid)
                    return driverList[itr];
            }
            return null;
        }
        public IDriver GetDriverWithConnectionId(byte connectionId)
        {
            int index;
            if( (index = GetConnectionIdNotBot(connectionId)) != 255 )
                return driverList[index];
            return null;
        }
        public IDriver GetDriverWithLicenceName(string licenceName)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (driverList[itr].LicenceName == licenceName)
                    return driverList[itr];
            }
            return null;
        }
        public byte GetNbrOfDrivers()
        {
            return (byte)(driverList.Count - 1); // -1 remove the Host but... maybe not good idea removing it from here.
        }
        public byte GetNbrOfConnection()
        {
            return (byte)(clientConnectionCount - 1); //Remove The Host
        }

        //Incomplete System RCM, this is not usefull, and i hate the way it work.
        //TODO very later, complete it, juste because there is shadow under the text with this function ;)
        /*private static Queue<string[]> rcMessageList = new Queue<string[]>();
        public void SetRCMessage(string message,string toLicenceName)
        {
            rcMessageList.Enqueue(new string[2]{message,toLicenceName});
        }
        public string[] GetRCMessage()
        {
            return rcMessageList.Dequeue();
        }
        public void ClearRCMessage()
        {
            lock(rcMessageList){rcMessageList.Clear();}
        }*/

        private const uint TIMER_PING_PONG = 8000;
        private uint TimerPingPong = 7000;
        private const uint TIMER_SAVE_INTERVAL = 600000;
        private uint sessionSaveInterval = 0;

        internal void update(uint diff)
        {
            // For moment will test processPacket from the network thread! gave better reaction time.
            //ProcessReceivedPacket();
            if (sessionSaveInterval > TIMER_SAVE_INTERVAL)
                SaveToDB();
            else
                sessionSaveInterval += diff;
            if (TIMER_PING_PONG < (TimerPingPong += diff))
            {
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, new PacketTiny(1, Tiny_Type.TINY_PING)));
                ping.Sending(diff);
                TimerPingPong = 0;
            }


            for (byte itr = 1; itr < driverList.Count; ++itr)
                driverList[itr].update(diff);
            race.update(diff);
            script.update(diff);
            
            if(freezeMotdSend < diff)
                freezeMotdSend = 0;
            else
                freezeMotdSend -= diff;
            //Delete Handle
            //Since im multithreading into processPacket Delete of: Race/Driver Should take place Bellow here.
            // Use Lock on the Object, since during Delete operation we can receive packet, this will pause other thread

        }

        new internal void AddToTcpSendingQueud(Packet _serverPacket)
        {
            base.AddToTcpSendingQueud(_serverPacket);
        }
        new internal void AddToUdpSendingQueud(Packet _serverPacket)
        {
            base.AddToUdpSendingQueud(_serverPacket);
        }
        private void ProcessReceivedPacket()
        {
            object[] _nextTcpPacket = NextTcpReceiveQueud(true);
            if (_nextTcpPacket != null)
                ProcessPacket((Packet_Type)_nextTcpPacket[0], _nextTcpPacket[1]);

            object[] _nextUdpPacket = NextUdpReceiveQueud(true);
            if (_nextUdpPacket != null)
                ProcessPacket((Packet_Type)_nextUdpPacket[0], _nextUdpPacket[1]);
        }

        protected sealed override void processPacket(PacketNCN packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            
            //Since we create the driver index 0 in this.construstor()
            //will conflit with the Host NCN receive packet, so here is a overide!
            //will have to rethink this later, that is looking like a HackFix, suck CPU for nothing.
            if (IsExistconnectionId(packet.connectionId))
            {
                /*if (_packet.connectionId == 0)
                {
                    driverList[0].Init(_packet);
                    return;
                }
                else*/
                {
                    Log.error(GetSessionNameForLog() + " New licence connection, but overrides an already existing connectionId, what to do if that happens?");
                    return;
                }
            }
            Driver driver = new Driver(this);
            driver.Init(packet);



            lock (this) { driverList.Add(driver); }
            //Prevent the Main thread from Doing the driverList.update()
            
        }      // new Connection
        protected sealed override void processPacket(PacketCNL packet)
        {
            //TODO: use _packet.Total as a Debug check to be sure we have same racer count into our memory as the server do. 
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif

            if (!IsExistconnectionId(packet.connectionId))
            {
                Log.error(GetSessionNameForLog() + " Licence disconnection, but no connectionId associated with it, what to do?");
                return;
            }

            //Prevent the Main thread from Doing the driverList.update()
            //Im not sure i love this design, since Mutex refresh time.
            byte index;
            while ((index = GetFirstConnectionIdIndex(packet.connectionId)) != 255)
            {
                driverList[index].ProcessCNLPacket(packet);
                
                lock (this) { driverList.RemoveAt((int)index); }
            }
   
        }      // delete Connection
        protected sealed override void processPacket(PacketCPR packet)
        {
            driverList[GetConnectionIdNotBot(packet.connectionId)].ProcessCPR(packet);
        }      // Driver Rename it self.
        protected sealed override void processPacket(PacketNPL packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif

            if (!IsExistconnectionId(packet.connectionId))
            {
                Log.error(GetSessionNameForLog() + " New car joined race, but no ConnectionId associated with it, what to do?");
                return;
            }


            byte index;
            //this is first version, i expect some probleme into the case, we don't know Driver as Leave the Race or Disconnected... Let See.
            Driver driver;
            if ((packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) == Driver_Type_Flag.DRIVER_TYPE_AI)      //AI
            {
                if ((index = GetDriverIndexWith(packet.connectionId, packet.driverName)) != 255)
                {
                    driverList[index].Init(packet);
                    driver = driverList[index];
                }
                else
                {
                    driver = new Driver(this);
                    driver.Init(packet);
                    driverList.Add(driver);
                }
            }
            else    //Human
            {
                index = GetConnectionIdNotBot(packet.connectionId);
                driverList[index].Init(packet);
                driver = driverList[index];
            }
            race.ProcessCarJoinRace(driver);
        }      // Car Join Race
        protected sealed override void processPacket(PacketPLL packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif

            byte itr;
            if ((itr = GetCarIndex(packet.carId)) == 255)
            {
                Log.error(GetSessionNameForLog() + " Car left race, but no car association found, what to do?");
                return;
            }

            //Do we delete the entire Driver on a Bot Leave Race???
            race.ProcessCarLeaveRace(((CarMotion)driverList[itr]));
            ((Driver)driverList[itr]).ProcessLeaveRace(packet);
        }      // Delete Car leave (spectate - loses slot)
        protected sealed override void processPacket(PacketMCI packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif

            CarInformation[] carInformation = packet.carInformation;
            byte carIndex;
            for (byte itr = 0; itr < carInformation.Length; itr++)
            {
                 // posX == 0 , help bypass the LFS bug on Alternate MCI Good/Null packet.
                if (carInformation[itr].carId == 0 || carInformation[itr].posX == 0)
                    continue;

                carIndex = GetCarIndex(carInformation[itr].carId);
                if (carIndex == 255)
                {
                    //Log.error("processPacket(PacketMCI), we can find any driver with this car.\r\n");
                    continue;
                }

                if (driverList[carIndex].ConnectionId == 0) // This Bypass the Host CAR.
                    continue;

                driverList[carIndex].ProcessCarInformation(carInformation[itr]);
                race.ProcessCarInformation(((CarMotion)driverList[carIndex]));
            }
        }      // Multiple Car Information
        protected sealed override void processPacket(PacketMSO packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif
            //Chat_User_Type chatUserType = allo;

            //_packet.chatUserType;
            if (!IsExistconnectionId(packet.connectionId))
                return;

            Driver driver = driverList[GetConnectionIdNotBot(packet.connectionId)];

            if (packet.message[packet.textStart] == commandPrefix) //Ingame Command
            {
                #if DEBUG
                Log.debug(GetSessionNameForLog() + " Received Command: " + packet.message.Substring(packet.textStart) + ", From LicenceUser: " + driver.LicenceName + "\r\n");
                #endif
                CommandExec(driver, packet.message.Substring(packet.textStart));
            }
            else if (packet.chatUserType == Chat_Type.SYSTEM || driver.IsBot()) //Host chat
            {
                Program.ircClient.SendToChannel(GetSessionNameForLog() + " Say: " + packet.message.Substring(packet.textStart).Replace("^", "\x03"));
                Log.chat(GetSessionNameForLog() + " Say: " + packet.message.Substring(packet.textStart).Replace("^", "\x03") + "\r\n");
            
            }
            else if (packet.chatUserType == Chat_Type.USER && !driver.IsBot())  //Player Chat
            {
                if(driver.GetGuid() != 0)
                    chatModo.AddNewLine(driver.LicenceName,packet.message );
                Program.ircClient.SendToChannel(GetSessionNameForLog() + " " + driver.DriverName.Replace("^", "\x03") + " Say: " + packet.message.Substring(packet.textStart).Replace("^", "\x03"));
                Log.chat(GetSessionNameForLog() + " " + driver.DriverName + " Say: " + packet.message.Substring(packet.textStart).Replace("^", "\x03") + "\r\n");
            }
        }      // message out
        protected sealed override void processPacket(PacketREO packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            race.Init(packet);
        }      // Race Grid Order
        protected sealed override void processPacket(PacketRST packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            race.Init(packet);

            if (GetRaceInProgressStatus() != Race_In_Progress_Status.RACE_PROGRESS_NONE)
            {
                int count = driverList.Count;
                for (byte itr = 0; itr < count; itr++)
                    driverList[itr].ProcessRaceStart();
            }
        }      // Race Start
        protected sealed override void processPacket(PacketSTA packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif

            string oldTrackPrefix =  race.GetTrackPrefix();
            Race_In_Progress_Status oldRaceStatus = GetRaceInProgressStatus();

            if (packet.currentCarId == 0)
                race.Init(packet);
            clientConnectionCount = packet.connectionCount;
            
            if (packet.trackPrefix != oldTrackPrefix || packet.raceInProgressStatus != oldRaceStatus)
            {
                for (byte itr = 0; itr < driverList.Count; itr++)
                {
                    if (packet.trackPrefix != oldTrackPrefix) 
                        driverList[itr].ProcessTrackChange();

                    if (oldRaceStatus == Race_In_Progress_Status.RACE_PROGRESS_NONE && packet.raceInProgressStatus != Race_In_Progress_Status.RACE_PROGRESS_NONE)
                        driverList[itr].ProcessRaceStart();
                }
            }
        }      // State Change race/car
        protected sealed override void processPacket(PacketTiny packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif
            switch (packet.subTinyType)
            {
                case Tiny_Type.TINY_REPLY: PingReceived(); break;
                case Tiny_Type.TINY_REN: 
                {
                    #if DEBUG
                    Log.debug(GetSessionNameForLog() + " RACE END RACE END RACE END.\r\n"); 
                    #endif

                    race.ProcessRaceEnd(); 
                }break; //Return Setup Screen(RaceEND)
                case Tiny_Type.TINY_VTC: race.ProcessVoteCancel(); break;
                case Tiny_Type.TINY_CLR:
                {
                    byte itr = 0;
                    byte itrEnd = (byte)driverList.Count;
                    while (itr < itrEnd)
                    {
                        race.ProcessCarLeaveRace(driverList[itr]);
                        driverList[itr].LeaveRace();
                        itr++;
                    }
                } break;
                case Tiny_Type.TINY_NONE: break;
                default: Log.missingDefinition(GetSessionNameForLog() + " Missing case for TinyPacket: " + packet.subTinyType + "\r\n"); break;
            }
        }     // Multipurpose 
        protected sealed override void processPacket(PacketSmall packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif
            switch (packet.subType)
            {
                case Small_Type.SMALL_VTA_VOTE_ACTION:
                {
                    race.ProcessVoteAction((Vote_Action)packet.uintValue);
                } break;
                case Small_Type.SMALL_RTP_RACE_TIME:
                {
                    race.ProcessGTH(packet.uintValue);
                }break;
                default: Log.missingDefinition(GetSessionNameForLog() + " Missing case for SmallPacket: " + (Small_Type)packet.subType + "\r\n"); break;
            }
        }    // Multipurpose 
        protected sealed override void processPacket(PacketLAP packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if(index == 255)
            {
                Log.error("processPacket(PacketLAP), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessLapInformation(packet);
        }      // Lap Completed
        protected sealed override void processPacket(PacketSPX packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketSPX), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessSplitInformation(packet);
        }      // Split Time Receive
        protected sealed override void processPacket(PacketRES packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketRES), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessRESPacket(packet);

            race.ProcessResult(packet);
        }      // Result, "Confimation" is only working for a race not qualify
        protected sealed override void processPacket(PacketFIN packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
        }      // Final Result, Only into a Race
        protected sealed override void processPacket(PacketBTC packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif

            Driver driver = driverList[GetConnectionIdNotBot(packet.connectionId)];

            switch ((Button_Entry)driver.GetButtonEntry(packet.buttonId))
            {
                case Button_Entry.TASKBAR_BUTTON_MENU:
                {
                    if(!driver.HasGuiDisplay(Gui_Entry.MENU))
                        driver.SendMenuGui();
                } break;
                case Button_Entry.MOTD_BUTTON_CLOSE:
                {
                    driver.RemoveGui(Gui_Entry.MOTD);
                } break;
                case Button_Entry.MENU_BUTTON_HELP:
                case Button_Entry.MOTD_BUTTON_HELP:
                {
                    driver.SendHelpGui();
                } break;
                case Button_Entry.MENU_BUTTON_RULES:
                case Button_Entry.MOTD_BUTTON_RULES:
                {
                    driver.SendRules();
                } break;
                case Button_Entry.MOTD_BUTTON_MOTD:
                case Button_Entry.MENU_BUTTON_MOTD:
                {
                    driver.SendMotd();
                } break;
                case Button_Entry.MOTD_BUTTON_MENU:
                {
                    driver.SendMenuGui();
                } break;
                case Button_Entry.CONFIG_USER_ACC_ON:
                {
                    if (driver.IsAccelerationOn())
                        driver.SetAccelerationOn(false);
                    else
                        driver.SetAccelerationOn(true);

                    driver.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_ACC_ON, (driver.IsAccelerationOn() ? "^7" : "^8") + " Acceleration");
                }break;
                case Button_Entry.CONFIG_USER_DRIFT_ON:
                {
                    if (driver.IsDriftScoreOn())
                        driver.SetDriftScoreOn(false);
                    else
                        driver.SetDriftScoreOn(true);

                    driver.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_DRIFT_ON, (driver.IsDriftScoreOn() ? "^7" : "^8") + " Drift Score");
                } break;
                case Button_Entry.CONFIG_USER_TIMEDIFF_ALL:
                {
                    if (driver.IsTimeDiffLapDisplay)
                    {
                        driver.IsTimeDiffLapDisplay = false;
                        driver.IsTimeDiffSplitDisplay = false;
                    }
                    else
                    {
                        driver.IsTimeDiffLapDisplay = true;
                        driver.IsTimeDiffSplitDisplay = true;
                    }
                    driver.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_TIMEDIFF_ALL, ((driver.IsTimeDiffSplitDisplay && driver.IsTimeDiffLapDisplay) ? "^7" : "^8") + " Time Diff");
                    driver.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_TIMEDIFF_LAP, (driver.IsTimeDiffLapDisplay ? "^7" : "^8") + " PB vs lap");
                    driver.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_TIMEDIFF_SPLIT, (driver.IsTimeDiffSplitDisplay ? "^7" : "^8") + " PB vs Split");
                } break;
                case Button_Entry.CONFIG_USER_TIMEDIFF_LAP:
                {
                    if (driver.IsTimeDiffLapDisplay)
                        driver.IsTimeDiffLapDisplay = false;
                    else
                        driver.IsTimeDiffLapDisplay = true;

                    driver.SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_ALL, ((driver.IsTimeDiffSplitDisplay && driver.IsTimeDiffLapDisplay) ? "^7" : "^8") + " Time Diff");
                    driver.SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_LAP, (driver.IsTimeDiffLapDisplay ? "^7" : "^8") + " PB vs lap");
                } break;
                case Button_Entry.CONFIG_USER_TIMEDIFF_SPLIT:
                {
                    if (driver.IsTimeDiffSplitDisplay)
                        driver.IsTimeDiffSplitDisplay = false;
                    else
                        driver.IsTimeDiffSplitDisplay = true;

                    driver.SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_ALL, ((driver.IsTimeDiffSplitDisplay && driver.IsTimeDiffLapDisplay) ? "^7" : "^8") + " Time Diff");
                    driver.SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_SPLIT, (driver.IsTimeDiffSplitDisplay ? "^7" : "^8") + " PB vs Split");
                } break;
                case Button_Entry.CONFIG_USER_MAX_SPEED_ON:
                {
                    if(driver.IsMaxSpeedDisplay)
                        driver.IsMaxSpeedDisplay = false;
                    else
                        driver.IsMaxSpeedDisplay = true;
                    driver.SendUpdateButton(Button_Entry.CONFIG_USER_MAX_SPEED_ON, (driver.IsMaxSpeedDisplay ? "^7" : "^8") + " Max Speed");
                } break;
                case Button_Entry.CONFIG_NODE_SIDE:
                {
                    if (driver.IsNodeSideDisplay)
                    {
                        driver.IsNodeSideDisplay = false;
                        driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^8Track Info");
                        driver.RemoveButton(Button_Entry.NODE_SIDE);
                    }
                    else
                    {
                        driver.IsNodeSideDisplay = true;
                        if(driver.IsNodeTrajectory && driver.IsNodeOrientation)
                            driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^7Track Info");
                    }
                    driver.SendUpdateButton(Button_Entry.CONFIG_NODE_SIDE, (driver.IsNodeSideDisplay ? "^7" : "^8") + "Side Distance");
                } break;
                case Button_Entry.CONFIG_NODE_ORIENTATION:
                {
                    if (driver.IsNodeOrientation)
                    {
                        driver.IsNodeOrientation = false;
                        driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^8Track Info");
                        driver.RemoveButton(Button_Entry.NODE_ORIE_TO_TRACK);
                    }
                    else
                    {
                        driver.IsNodeOrientation = true;
                        if (driver.IsNodeTrajectory && driver.IsNodeSideDisplay)
                            driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^7Track Info");
                    }
                    driver.SendUpdateButton(Button_Entry.CONFIG_NODE_ORIENTATION, (driver.IsNodeOrientation ? "^7" : "^8") + "Orientation");
                } break;
                case Button_Entry.CONFIG_NODE_TRAJECTORY:
                {
                    if (driver.IsNodeTrajectory)
                    {
                        driver.IsNodeTrajectory = false;
                        driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^8Track Info");
                        driver.RemoveButton(Button_Entry.NODE_TRAJ_TO_TRACK);
                    }
                    else
                    {
                        driver.IsNodeTrajectory = true;
                        if (driver.IsNodeOrientation && driver.IsNodeSideDisplay)
                            driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^7Track Info");
                    }
                    driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TRAJECTORY, (driver.IsNodeTrajectory ? "^7" : "^8") + "Trajectory");
                } break;
                case Button_Entry.CONFIG_NODE_TITLE:
                {
                    if (driver.IsNodeTrajectory || driver.IsNodeSideDisplay || driver.IsNodeOrientation)
                    {
                        driver.IsNodeSideDisplay = false;
                        driver.IsNodeOrientation = false;
                        driver.IsNodeTrajectory = false;
                        driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^8Track Info");
                        driver.RemoveButton(Button_Entry.NODE_ORIE_TO_TRACK);
                        driver.RemoveButton(Button_Entry.NODE_SIDE);
                        driver.RemoveButton(Button_Entry.NODE_TRAJ_TO_TRACK);
                    }
                    else
                    {
                        driver.IsNodeSideDisplay = true;
                        driver.IsNodeOrientation = true;
                        driver.IsNodeTrajectory = true;
                        driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^7Track Info");
                    }
                    driver.SendUpdateButton(Button_Entry.CONFIG_NODE_TRAJECTORY, (driver.IsNodeTrajectory ? "^7" : "^8") + "Trajectory");
                    driver.SendUpdateButton(Button_Entry.CONFIG_NODE_SIDE, (driver.IsNodeSideDisplay ? "^7" : "^8") + "Side Distance");
                    driver.SendUpdateButton(Button_Entry.CONFIG_NODE_ORIENTATION, (driver.IsNodeOrientation ? "^7" : "^8") + "Orientation");
                
                }break;
                case Button_Entry.TASKBAR_BUTTON_EXIT:
                {
                    driver.RemoveCurrentGui();
                }break;
                case Button_Entry.HELP_BUTTON_MENU:
                {
                    driver.RemoveGui(Gui_Entry.HELP);
                    driver.SendMenuGui();
                } break;
                case Button_Entry.HELP_BUTTON_DRIVE:
                {
                    driver.RemoveGui(Gui_Entry.HELP);
                } break;
                case Button_Entry.TEXT_BUTTON_DRIVE:
                {
                    driver.RemoveGui(Gui_Entry.TEXT);
                } break;
                case Button_Entry.RANK_HELP_TEXT:
                {
                    driver.RemoveButton(Button_Entry.RANK_HELP_TEXT);
                    driver.RemoveButton(Button_Entry.RANK_HELP_BG);
                    driver.RemoveButton(Button_Entry.RANK_HELP_BG_SHADOW);

                } break;
                case Button_Entry.RANK_BUTTON_TOP20:
                {
                    driver.SendRankTop20();
                } break;
                case Button_Entry.RANK_BUTTON_SEARCH:
                {
                    driver.SendRankSearch();
                } break;
                case Button_Entry.RANK_BUTTON_CURRENT:
                {
                    driver.SendRankCurrent(0);
                } break;
                case Button_Entry.CANCEL_WARNING_DRIVING_1:
                case Button_Entry.CANCEL_WARNING_DRIVING_2:
                case Button_Entry.CANCEL_WARNING_DRIVING_3:
                {
                    driver.RemoveCancelWarningDriving(true);
                } break;
                case Button_Entry.MENU_BUTTON_CONFIG:
                {
                    driver.RemoveMenuGui();
                    driver.SendConfigGui();
                } break;
                case Button_Entry.MENU_BUTTON_RANK:
                {
                    driver.RemoveMenuGui();
                    driver.SendRankGui(Button_Entry.RANK_BUTTON_CURRENT);
                } break;
                case Button_Entry.MENU_BUTTON_SCOREBOARD:
                {
                    //driver.RemoveMenuGui();
                } break;
                case Button_Entry.MENU_BUTTON_RESULT:
                {
                    driver.RemoveMenuGui();
                    driver.SendResultGui(GetRaceLastResult());
                } break;
                case Button_Entry.MENU_BUTTON_MYSTATS:
                {
                    driver.SendMyStatus();
                } break;
                case Button_Entry.MENU_BUTTON_MANAGER:
                {
                    //driver.RemoveMenuGui();
                } break;
                case Button_Entry.MENU_BUTTON_SAY:
                {
                    //driver.RemoveMenuGui();
                } break;
                case Button_Entry.MENU_BUTTON_STATUS:
                {
                    driver.RemoveMenuGui();
                    command.Exec(driver, "!status all");
                } break;
                case Button_Entry.MENU_BUTTON_RELOAD:
                {
                    driver.RemoveMenuGui();
                    command.Exec(driver,"!reload all");
                } break;
                case Button_Entry.MENU_BUTTON_EXIT:
                {
                    driver.RemoveMenuGui();
                    command.Exec(driver, "!exit");
                } break;
                case Button_Entry.VOTE_OPTION_1: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_1,packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_2: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_2, packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_3: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_3, packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_4: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_4, packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_5: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_5, packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_6: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_6, packet.connectionId); break;
                default:
                {
                    Log.error("We received a button ClickId from an unknown source, licenceName: " + driver.LicenceName + "\r\n");
                } break;
            }
        }      // Button Click Receive
        protected sealed override void processPacket(PacketBTT packet)         // Button Text Receive
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif

            byte driverIndex = GetConnectionIdNotBot(packet.connectionId);
            Driver driver = driverList[driverIndex];
            switch((Button_Entry)driver.GetButtonEntry(packet.buttonId))
            {
                case Button_Entry.CONFIG_USER_ACC_START:
                {
                    ushort startKmh;
                    try { startKmh = Convert.ToUInt16(packet.typedText); }
                    catch (Exception)
                    {
                        driver.AddMessageMiddle("^1Bad value (^7" + packet.typedText + "^1) entered for 'Acceleration Start speed (in Kmh)'.", 7000);
                        return;
                    }
                    if (startKmh > driver.GetAccelerationEndSpeed())
                    {
                        driver.AddMessageMiddle("^1Start speed (^7" + startKmh + "^1) cannot be higher then End speed (^7" + driver.GetAccelerationEndSpeed() + "^1)", 7000);
                        return;
                    }
                    driver.SetAccelerationStartSpeed(startKmh);
                    driver.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_ACC_CURRENT, "^7" + driver.GetAccelerationStartSpeed() + "^2-^7" + driver.GetAccelerationEndSpeed() + " ^2Kmh");
                } break;
                case Button_Entry.CONFIG_USER_ACC_END:
                {
                    ushort endKmh;
                    try { endKmh = Convert.ToUInt16(packet.typedText); }
                    catch (Exception)
                    {
                        driver.AddMessageMiddle("^1Bad value (^7" + packet.typedText + "^1) entered for 'Acceleration End speed (in Kmh)'.", 7000);
                        return;
                    }
                    if (endKmh < driver.GetAccelerationStartSpeed())
                    {
                        driver.AddMessageMiddle("^1End speed (^7" + endKmh + "^1) cannot be lower than the Start speed (^7"+driver.GetAccelerationStartSpeed()+"^1)", 7000);
                        return;
                    }
                    if (endKmh < 10)
                    {
                        driver.AddMessageMiddle("^1End speed (^7" + endKmh + "^1) cannot be lower than ^710.", 7000);
                        return;
                    }
                    driver.SetAccelerationEndSpeed(endKmh);
                    driver.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_ACC_CURRENT, "^7" + driver.GetAccelerationStartSpeed() + "^2-^7" + driver.GetAccelerationEndSpeed() + " ^2Kmh");
                } break;
                case Button_Entry.RANK_SEARCH_BUTTON_TRACK:
                {
                    driver.RankSearchTrack(packet.typedText.ToUpperInvariant());
                } break;
                case Button_Entry.RANK_SEARCH_BUTTON_CAR:
                {
                    driver.RankSearchCar(packet.typedText.ToUpperInvariant());
                } break;
                case Button_Entry.RANK_SEARCH_BUTTON_LICENCE:
                {
                    driver.RankSearchAdd(packet.typedText);
                }break;

                case Button_Entry.MYSTATUS_BUTTON_SEARCH:
                {
                    if (packet.typedText.Trim() != "")
                        driver.SearchMyStatus(packet.typedText);
                } break;
                case Button_Entry.MENU_BUTTON_SEARCH_RACE:
                {
                    command.Exec(driver, "!searchrace " + packet.typedText);
                } break;
                case Button_Entry.MENU_BUTTON_LOAD_RACE:
                {
                    command.Exec(driver, "!loadrace " + packet.typedText);
                } break;
                case Button_Entry.MENU_BUTTON_RELOAD:
                {
                    command.Exec(driver, "!reload " + packet.typedText);
                } break;
                case Button_Entry.MENU_BUTTON_SAY:
                {
                    command.Exec(driver, "!say " + packet.typedText);
                } break;
                case Button_Entry.MENU_BUTTON_YELLOW_TIME:
                {
                    command.Exec(driver, "!yellowtime " + packet.typedText);
                } break;
                case Button_Entry.MENU_BUTTON_BAD_WORD:
                {
                    command.Exec(driver, "!badword " + packet.typedText);
                } break;
            }
        }
        protected sealed override void processPacket(PacketBFN packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            int index = GetConnectionIdNotBot(packet.connectionId);
            if(index == 255)
            {
                Log.error("processPacket(PacketBFN), received a Not Found Driver ID.\r\n");
                return;
            }
            switch (packet.buttonFunction)
            {
                case Button_Function.BUTTON_FUNCTION_USER_CLEAR:
                {
                    Driver driver = driverList[index];
                    driver.ProcessBFNClearAll(driver.HasAGuiDisplay() ? false : true);
                } break;
                case Button_Function.BUTTON_FUNCTION_REQUEST:
                    driverList[index].ProcessBFNRequest(); break;
            }
        }      // Delete All Button or request Button.
        protected sealed override void processPacket(PacketVTN packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            race.ProcessVoteNotification(packet.voteAction, packet.connectionId);
        }      // Vote Notification
        protected sealed override void processPacket(PacketPLP packet)         // Car enter garage
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif

            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketSPX), we can find any driver with this car.\r\n");
                return;
            }
            race.ProcessCarLeaveRace(((CarMotion)driverList[index]));
            driverList[index].ProcessEnterGarage();
            
        }
        protected sealed override void processPacket(PacketFLG packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketFLG), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessFLGPacket(packet);
        }
        protected sealed override void processPacket(PacketPLA packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketPLA), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessPLAPacket(packet);
        }
        protected sealed override void processPacket(PacketPIT packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketPIT), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessPITPacket(packet);
        }
        protected sealed override void processPacket(PacketPFL packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketPFL), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessPFLPacket(packet);
        }
        protected sealed override void processPacket(PacketPEN packet)
        {
            #if DEBUG
            base.processPacket(packet); //Keep the Log
            #endif
            byte index = GetCarIndex(packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketPEN), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessPenality(packet);
        }
        protected sealed override void processPacket(PacketVER packet)
        {
            Log.normal(GetSessionNameForLog() + " InSim v" + packet.inSimVersion + ", Licence: " + packet.productVersion + "." + packet.serverVersion + "\r\n");
            SendMSXMessage("^7D^3rive_LFSS ^7as come ^2Online.");
            ApplyConfigData();
            #if DEBUG
            //race.ProcessVoteAction(Vote_Action.VOTE_END);
            #endif
        }
    }
}

/* Text Color
 * Red = 1
 * Green = 2
 * Yellow = 3
 * Blue = 4
 * Purple = 5
 * Cyan = 6
 * White = 7
 * Gray = 8
 * Gray = 9
 * Black = 0
 */
/* IRC Text Color
0-   White
1-   Black
2-   Blue
3-   Green
4-   Light Red
5-   Brown
6-   Purple
7-   Orange
8-   Yellow
9-   Light Green
10-   Cyan
11-   Light Cyan
12-   Light Blue
13-   Pink
14-   Grey
15-   Light Grey 
*/
