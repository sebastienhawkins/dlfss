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
        }
        ~Session()
        {
            if (true == false) { }
        }
        internal void ConfigApply(InSimSetting _inSimSetting)
        {
            base.SetInSimSetting(_inSimSetting);
            ConfigApply();
        }
        new internal void ConfigApply(bool onlyVoteSystem)
        {
            if (onlyVoteSystem)
            {
                race.ConfigApply();
                return;
            }

            base.ConfigApply();
            race.ConfigApply();
            Driver.ConfigApply();
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

        //Object
        private Script script;
        private CommandInGame command;
        private Race race;
        private List<Driver> driverList;
        private ChatModo chatModo;
        
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
        public void SendUpdateButtonToAll(ushort buttonEntry, string text)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].SendUpdateButton(buttonEntry, text);
        }
        public void SendResultGuiToAll(Dictionary<string, int> scoringResultTextDisplay)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
                driverList[itr].SendResultGui(scoringResultTextDisplay);
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
            driverList[GetLicenceIndexNotBot(connectionId)].RemoveButton(buttonEntry);
        }
        public bool IsRaceInProgress()
        {
            return race.IsRaceInProgress();
        }
        public bool CanVote()
        {
            return race.CanVote();
        }
        public Script Script
        {
            get { return script; }
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
        private byte GetLicenceIndexWithName(byte connectionId, string _driverName)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((IDriver)driverList[itr]).ConnectionId == connectionId && ((IDriver)driverList[itr]).DriverName == _driverName)
                    return itr;
            }
            return 255;
        }
        private byte GetLicenceIndexNotBot(byte connectionId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (driverList[itr].ConnectionId == connectionId && !driverList[itr].IsBot())
                    return itr;
            }
            return 255;
        }
        private byte GetFirstLicenceIndex(byte connectionId)
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
        public IDriver GetDriverWith(byte carId)
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
        internal void update(uint diff)
        {
            // For moment will test processPacket from the network thread! gave better reaction time.
            //ProcessReceivedPacket();

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

        protected sealed override void processPacket(PacketNCN _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            
            //Since we create the driver index 0 in this.construstor()
            //will conflit with the Host NCN receive packet, so here is a overide!
            //will have to rethink this later, that is looking like a HackFix, suck CPU for nothing.
            if (IsExistconnectionId(_packet.connectionId))
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
            Driver _driver = new Driver(this);
            _driver.Init(_packet);

            //Prevent the Main thread from Doing the driverList.update()
            lock (this){driverList.Add(_driver);}
        }      // new Connection
        protected sealed override void processPacket(PacketCNL _packet)
        {
            //TODO: use _packet.Total as a Debug check to be sure we have same racer count into our memory as the server do. 
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif

            if (!IsExistconnectionId(_packet.connectionId))
            {
                Log.error(GetSessionNameForLog() + " Licence disconnection, but no connectionId associated with it, what to do?");
                return;
            }

            //Prevent the Main thread from Doing the driverList.update()
            //Im not sure i love this design, since Mutex refresh time.
            byte index;
            while ((index = GetFirstLicenceIndex(_packet.connectionId)) != 255)
            {
                driverList[index].ProcessCNLPacket(_packet);
                lock (this) { driverList.RemoveAt((int)index); }
            }
   
        }      // delete Connection
        protected sealed override void processPacket(PacketCPR _packet)
        {
            driverList[GetLicenceIndexNotBot(_packet.connectionId)].ProcessCPR(_packet);
        }       //Driver Rename it self.
        protected sealed override void processPacket(PacketNPL _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif

            if (!IsExistconnectionId(_packet.connectionId))
            {
                Log.error(GetSessionNameForLog() + " New car joined race, but no ConnectionId associated with it, what to do?");
                return;
            }


            byte index;
            //this is first version, i expect some probleme into the case, we don't know Driver as Leave the Race or Disconnected... Let See.
            Driver driver;
            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) == Driver_Type_Flag.DRIVER_TYPE_AI)      //AI
            {
                if ((index = GetLicenceIndexWithName(_packet.connectionId, _packet.driverName)) != 255)
                {
                    driverList[index].Init(_packet);
                    driver = driverList[index];
                }
                else
                {
                    driver = new Driver(this);
                    driver.Init(_packet);
                    driverList.Add(driver);
                }
            }
            else    //Human
            {
                index = GetLicenceIndexWithName(_packet.connectionId, _packet.driverName);
                driverList[index].Init(_packet);
                driver = driverList[index];
            }
            race.ProcessCarJoinRace(driver);
        }      // Car Join Race
        protected sealed override void processPacket(PacketPLL _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif

            byte itr;
            if ((itr = GetCarIndex(_packet.carId)) == 255)
            {
                Log.error(GetSessionNameForLog() + " Car left race, but no car association found, what to do?");
                return;
            }

            //Do we delete the entire Driver on a Bot Leave Race???
            ((Driver)driverList[itr]).ProcessLeaveRace(_packet);
            race.ProcessCarLeaveRace(((CarMotion)driverList[itr]));
        }      // Delete Car leave (spectate - loses slot)
        protected sealed override void processPacket(PacketMCI _packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif

            CarInformation[] carInformation = _packet.carInformation;
            byte carIndex;
            for (byte itr = 0; itr < carInformation.Length; itr++)
            {
                
                if (carInformation[itr].carId == 0)
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
        protected sealed override void processPacket(PacketMSO _packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif
            //Chat_User_Type chatUserType = allo;

            //_packet.chatUserType;
            if (!IsExistconnectionId(_packet.connectionId))
                return;

            Driver driver = driverList[GetFirstLicenceIndex(_packet.connectionId)];

            if (_packet.message[_packet.textStart] == commandPrefix) //Ingame Command
            {
                #if DEBUG
                Log.debug(GetSessionNameForLog() + " Received Command: " + _packet.message.Substring(_packet.textStart) + ", From LicenceUser: " + driver.LicenceName + "\r\n");
                #endif
                CommandExec(driver, _packet.message.Substring(_packet.textStart));
            }
            else if (_packet.chatUserType == Chat_Type.SYSTEM || driver.IsBot()) //Host chat
            {
                Program.ircClient.SendToChannel(GetSessionNameForLog() + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03"));
                Log.chat(GetSessionNameForLog() + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03") + "\r\n");
            
            }
            else if (_packet.chatUserType == Chat_Type.USER && !driver.IsBot())  //Player Chat
            {
                if(driver.GetGuid() != 0)
                    chatModo.AddNewLine(driver.DriverName,_packet.message );
                Program.ircClient.SendToChannel(GetSessionNameForLog() + " " + driver.DriverName.Replace("^", "\x03") + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03"));
                Log.chat(GetSessionNameForLog() + " " + driver.DriverName + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03") + "\r\n");
            }
        }      // message out
        protected sealed override void processPacket(PacketREO _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            race.Init(_packet);
        }      // Race Grid Order
        protected sealed override void processPacket(PacketRST _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            race.Init(_packet);

            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                driverList[itr].ProcessRaceStart();
            }
        }      // Race Start
        protected sealed override void processPacket(PacketSTA _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif

            string oldTrackPrefix =  race.GetTrackPrefix();
            if (_packet.currentCarId == 0)
                race.Init(_packet);

            if (_packet.trackPrefix != oldTrackPrefix)
            {//THIS IS NOT NEEDED IF WE FOLLOW TINY_CLR WHO MEAN ALL PLAYE CLEARED FROM TRACK.

                //SendMSTMessage(Msg.TRACK_PREFIX_NEW + GetRaceTrackPrefix());
            }

            clientConnectionCount = _packet.connectionCount;
            //else
            //    ;//driver Case

        }      // State Change race/car
        protected sealed override void processPacket(PacketTiny _packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif
            switch (_packet.subTinyType)
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
                        driverList[itr].LeaveTrack();
                        itr++;
                    }
                } break;
                case Tiny_Type.TINY_NONE: break;
                default: Log.missingDefinition(GetSessionNameForLog() + " Missing case for TinyPacket: " + _packet.subTinyType + "\r\n"); break;
            }
        }     // Multipurpose 
        protected sealed override void processPacket(PacketSmall _packet)
        {
            #if DEBUG
            //base.processPacket(_packet); //Keep the Log
            #endif
            switch (_packet.subType)
            {
                case Small_Type.SMALL_VTA_VOTE_ACTION:
                {
                    race.ProcessVoteAction((Vote_Action)_packet.uintValue);
                } break;
                case Small_Type.SMALL_RTP_RACE_TIME:
                {
                    race.ProcessGTH(_packet.uintValue);
                }break;
                default: Log.missingDefinition(GetSessionNameForLog() + " Missing case for SmallPacket: " + (Small_Type)_packet.subType + "\r\n"); break;
            }
        }    // Multipurpose 
        protected sealed override void processPacket(PacketLAP _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            byte index = GetCarIndex(_packet.carId);
            if(index == 255)
            {
                Log.error("processPacket(PacketLAP), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessLapInformation(_packet);
        }      // Lap Completed
        protected sealed override void processPacket(PacketSPX _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            byte index = GetCarIndex(_packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketSPX), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessSplitInformation(_packet);
        }      // Split Time Receive
        protected sealed override void processPacket(PacketRES _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            byte index = GetCarIndex(_packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketRES), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessRESPacket(_packet);

            race.ProcessResult(_packet);
        }      // Result, "Confimation" is only working for a race not qualify
        protected sealed override void processPacket(PacketFIN _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
        }      // Final Result, Only into a Race
        protected sealed override void processPacket(PacketBTC _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif

            Driver driver = driverList[GetLicenceIndexNotBot(_packet.connectionId)];

            switch ((Button_Entry)driver.GetButtonEntry(_packet.buttonId))
            {
                case Button_Entry.MOTD_BUTTON_DRIVE:
                {
                    driver.RemoveGui(Gui_Entry.MOTD);
                } break;
                case Button_Entry.MOTD_BUTTON_HELP:
                {
                    driver.RemoveGui(Gui_Entry.MOTD);
                    driver.SendHelpGui();
                } break;
                case Button_Entry.MOTD_BUTTON_CONFIG:
                {
                    driver.RemoveGui(Gui_Entry.MOTD);
                    driver.SendConfigGui();
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
                case Button_Entry.CONFIG_USER_CLOSE:
                {
                    driver.RemoveGui(Gui_Entry.CONFIG_USER);
                } break;
                
                case Button_Entry.HELP_BUTTON_CONFIG:
                {
                    driver.RemoveGui(Gui_Entry.HELP);
                    driver.SendConfigGui();
                } break;
                case Button_Entry.HELP_BUTTON_DRIVE:
                {
                    driver.RemoveGui(Gui_Entry.HELP);
                } break;
                case Button_Entry.TEXT_BUTTON_DRIVE:
                {
                    driver.RemoveGui(Gui_Entry.TEXT);
                } break;
                case Button_Entry.RANK_BUTTON_CLOSE:
                {
                    driver.RemoveRankGui();
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
                case Button_Entry.RESULT_CLOSE_BUTTON:
                {
                    driver.RemoveResultGui();
                } break;
                
                case Button_Entry.VOTE_OPTION_1: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_1,_packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_2: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_2, _packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_3: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_3, _packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_4: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_4, _packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_5: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_5, _packet.connectionId); break;
                case Button_Entry.VOTE_OPTION_6: race.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_6, _packet.connectionId); break;
                default:
                {
                    Log.error("We received a button ClickId from an unknown source, licenceName: " + driver.LicenceName + "\r\n");
                } break;
            }
        }      // Button Click Receive
        protected sealed override void processPacket(PacketBTT _packet)         // Button Text Receive
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif

            byte driverIndex = GetLicenceIndexNotBot(_packet.connectionId);
            Driver car = driverList[driverIndex];
            switch((Button_Entry)car.GetButtonEntry(_packet.buttonId))
            {
                case Button_Entry.CONFIG_USER_ACC_START:
                {
                    ushort startKmh;
                    try { startKmh = Convert.ToUInt16(_packet.typedText); }
                    catch (Exception)
                    {
                        car.AddMessageMiddle("^1Bad value (^7" + _packet.typedText + "^1) entered for 'Acceleration Start speed (in Kmh)'.", 7000);
                        return;
                    }
                    if (startKmh > car.GetAccelerationEndSpeed())
                    {
                        car.AddMessageMiddle("^1Start speed (^7" + startKmh + "^1) cannot be higher then End speed (^7" + car.GetAccelerationEndSpeed() + "^1)", 7000);
                        return;
                    }
                    car.SetAccelerationStartSpeed(startKmh);
                    car.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_ACC_CURRENT, "^7" + car.GetAccelerationStartSpeed() + "^2-^7" + car.GetAccelerationEndSpeed() + " ^2Kmh");
                } break;
                case Button_Entry.CONFIG_USER_ACC_END:
                {
                    ushort endKmh;
                    try { endKmh = Convert.ToUInt16(_packet.typedText); }
                    catch (Exception)
                    {
                        car.AddMessageMiddle("^1Bad value (^7" + _packet.typedText + "^1) entered for 'Acceleration End speed (in Kmh)'.", 7000);
                        return;
                    }
                    if (endKmh < car.GetAccelerationStartSpeed())
                    {
                        car.AddMessageMiddle("^1End speed (^7" + endKmh + "^1) cannot be lower than the Start speed (^7"+car.GetAccelerationStartSpeed()+"^1)", 7000);
                        return;
                    }
                    if (endKmh < 10)
                    {
                        car.AddMessageMiddle("^1End speed (^7" + endKmh + "^1) cannot be lower than ^710.", 7000);
                        return;
                    }
                    car.SetAccelerationEndSpeed(endKmh);
                    car.SendUpdateButton((ushort)Button_Entry.CONFIG_USER_ACC_CURRENT, "^7" + car.GetAccelerationStartSpeed() + "^2-^7" + car.GetAccelerationEndSpeed() + " ^2Kmh");
                } break;
                case Button_Entry.RANK_SEARCH_BUTTON_TRACK:
                {
                    car.RankSearchTrack(_packet.typedText.ToUpperInvariant());
                } break;
                case Button_Entry.RANK_SEARCH_BUTTON_CAR:
                {
                    car.RankSearchCar(_packet.typedText.ToUpperInvariant());
                } break;
                case Button_Entry.RANK_SEARCH_BUTTON_LICENCE:
                {
                    car.RankSearchAdd(_packet.typedText);
                }break;
            }
        }
        protected sealed override void processPacket(PacketBFN _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            int index = GetLicenceIndexNotBot(_packet.connectionId);
            if(index == 255)
            {
                Log.error("processPacket(PacketBFN), received a Not Found Driver ID.\r\n");
                return;
            }
            switch (_packet.buttonFunction)
            {
                case Button_Function.BUTTON_FUNCTION_USER_CLEAR:
                {
                    Driver driver = driverList[index];
                    driver.ProcessBFNClearAll(driver.HasGuiDisplay() ? false : true);
                } break;
                case Button_Function.BUTTON_FUNCTION_REQUEST:
                    driverList[index].ProcessBFNRequest(); break;
            }
        }      //Delete All Button or request Button.
        protected sealed override void processPacket(PacketVTN _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            race.ProcessVoteNotification(_packet.voteAction, _packet.connectionId);
        }      //Vote Notification
        protected sealed override void processPacket(PacketPLP _packet)         //Car enter garage
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif

            byte index = GetCarIndex(_packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketSPX), we can find any driver with this car.\r\n");
                return;
            }

            driverList[index].ProcessEnterGarage();
            race.ProcessCarLeaveRace(((CarMotion)driverList[index]));
        }
        protected sealed override void processPacket(PacketFLG _packet)
        {
            #if DEBUG
            base.processPacket(_packet); //Keep the Log
            #endif
            byte index = GetCarIndex(_packet.carId);
            if (index == 255)
            {
                Log.error("processPacket(PacketFLG), we can find any driver with this car.\r\n");
                return;
            }
            driverList[index].ProcessFLGPacket(_packet);
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
