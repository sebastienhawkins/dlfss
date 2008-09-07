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

namespace Drive_LFSS.Session_
{
    using Drive_LFSS.Server_;
    using Drive_LFSS.InSim_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Script_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Game_;

    public sealed class Session : InSim, ISession
    {
        public Session(string _serverName, InSimSetting _inSimSetting): base( _inSimSetting)
        {
            sessionName = _serverName;
            commandPrefix = _inSimSetting.commandPrefix;
            race = new Race(this);
            vote = new Vote(this);
            driverList = new List<Driver>();
            driverList.Add(new Driver(this)); //put Default Driver 0, will save some If.
            driverList.Capacity = 256;
            script = new Script();
            ping = new Ping();
            command = new CommandInGame(sessionName);
            connectionRequest = true;
        }
        ~Session()
        {
            if (true == false) { }
        }
        new public void ConfigApply()
        {
            base.ConfigApply();

            vote.ConfigApply();
            Race.ConfigApply();
            Driver.ConfigApply();
        }
        private class Ping
        {
            public Ping()
            {
                pingTime = DateTime.Now.Ticks;
                sessionLatency = 0;
                diff = 0;
            }
            private long pingTime;
            private long sessionLatency;
            private uint diff;
            public void Sending(uint _diff)
            {
                pingTime = DateTime.Now.Ticks;
                diff = _diff;

            }
            public long Received()
            {
                return (sessionLatency = (DateTime.Now.Ticks - pingTime) / 10000);
            }
            public long SessionLatency
            {
                get{return sessionLatency;}
            }
            public long GetReactionTime()
            {
                return sessionLatency - diff;
            }
        }
        private Ping ping;
        private string sessionName;
        private char commandPrefix;
        public bool connectionRequest;
        private byte clientConnectionCount = 0;

        //Object
        public Script script;
        private CommandInGame command;
        private Race race;
        private Vote vote;
        private List<Driver> driverList;

        private void CommandExec(bool _adminStatus, string _licenceName, string _commandText)
        {
            command.Exec(_adminStatus, _licenceName, _commandText);
        }
        private void PingReceived()
        {
            long msTime = ping.Received();
            Log.network(GetSessionNameForLog() + " Pong! " +  msTime + "ms\r\n");
        }
        public long GetLatency()
        {
            return ping.SessionLatency;
        }
        public long GetReactionTime()
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
        public uint GetRaceGuid()
        {
            return race.GetGuid();
        }
        public string GetRaceTrackPrefix()
        {
            return race.GetTrackPrefix();
        }
        public void SendMSTMessage(string message)
        {
            AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(message)));
        }
        public void SendUpdateButtonToAll(ushort buttonEntry, string text)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
                driverList[itr].SendUpdateButton(buttonEntry, text);
        }
        public void AddMessageMiddleToAll(string text, uint duration)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
                driverList[itr].AddMessageMiddle(text,duration);
        }
        public void RemoveButtonToAll(ushort buttonEntry)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
                driverList[itr].RemoveButton(buttonEntry);
        }
        public void RemoveButton(ushort buttonEntry, byte licenceId)
        {
            driverList[GetLicenceIndexNoBot(licenceId)].RemoveButton(buttonEntry);
        }
        private const uint TIMER_PING_PONG = 50000;
        private uint TimerPingPong = 48000;
        public void update(uint diff)
        {
            // For moment will test processPacket from the network thread! gave better reaction time.
            //ProcessReceivedPacket();

            if (TIMER_PING_PONG < (TimerPingPong += diff))
            {
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, new PacketTiny(1, Tiny_Type.TINY_PING)));
                ping.Sending(diff);
                TimerPingPong = 0;
            }


            vote.update(diff);
            for (byte itr = 1; itr < driverList.Count; ++itr)
                driverList[itr].update(diff);
            race.update(diff);


            //Delete Handle
            //Since im multithreading into processPacket Delete of: Race/Driver Should take place Bellow here.
            // Use Lock on the Object, since during Delete operation we can receive packet, this will pause other thread

        }

        #region Process packet
        new public void AddToTcpSendingQueud(Packet _serverPacket)
        {
            base.AddToTcpSendingQueud(_serverPacket);
        }
        new public void AddToUdpSendingQueud(Packet _serverPacket)
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
            base.processPacket(_packet); //Keep the Log

            //Since we create the driver index 0 in this.construstor()
            //will conflit with the Host NCN receive packet, so here is a overide!
            //will have to rethink this later, that is looking like a HackFix, suck CPU for nothing.
            if (IsExistLicenceId(_packet.tempLicenceId))
            {
                if (_packet.tempLicenceId == 0)
                {
                    driverList[0].Init(_packet);
                    return;
                }
                else
                {
                    Log.error(GetSessionNameForLog() + " New Licence Connection, But Override a Allready LicenceId, what to do if that Happen???");
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
            base.processPacket(_packet); //Keep the Log

            if (!IsExistLicenceId(_packet.tempLicenceId))
            {
                Log.error(GetSessionNameForLog() + " Licence Disconnection, But no LicenceID associated with It, What todo???");
                return;
            }

            //Prevent the Main thread from Doing the driverList.update()
            //Im not sure i love this design, since Mutex refresh time.
            byte itr;
            while ((itr = GetFirstLicenceIndex(_packet.tempLicenceId)) != 0)
                lock (this) {driverList.RemoveAt((int)itr); }
   
        }      // delete Connection
        protected sealed override void processPacket(PacketNPL _packet)
        {
            base.processPacket(_packet); //Keep the Log

            if (!IsExistLicenceId(_packet.tempLicenceId))
            {
                Log.error(GetSessionNameForLog() + " New Car Join Race, But Not LicenceId Associated What todo???");
                return;
            }

            Car _car;

            byte index;
            //this is first version, i expect some probleme into the case, we don't know Driver as Leave the Race or Disconnected... Let See.
            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) == Driver_Type_Flag.DRIVER_TYPE_AI)      //AI
            {
                if ((index = GetLicenceIndexWithName(_packet.tempLicenceId, _packet.driverName)) != 0)
                {
                    driverList[index].Init(_packet);
                    _car = ((Car)driverList[index]);
                }
                else
                {
                    Driver _driver = new Driver(this);
                    _driver.Init(_packet);
                    driverList.Add(_driver);
                    _car = ((Car)_driver);
                }
            }
            else    //Human
            {
                index = GetLicenceIndexWithName(_packet.tempLicenceId, _packet.driverName);
                driverList[index].Init(_packet);
            }
            race.AddToGrid(((CarMotion)driverList[index]));
        }      // New Car Join Race
        protected sealed override void processPacket(PacketPLL _packet)
        {
            base.processPacket(_packet); //Keep the Log

            byte itr;
            if ((itr = GetCarIndex(_packet.carId)) == 0)
            {
                Log.error(GetSessionNameForLog() + " Car Leave race, But no Car Association Found , what todo???");
                return;
            }

            //Do we delete the entire Driver on a Bot Leave Race???
            ((Car)driverList[itr]).leaveRace(_packet);
            race.RemoveFromGrid(((CarMotion)driverList[itr]));
        }      // Delete Car leave (spectate - loses slot)
        protected sealed override void processPacket(PacketMCI _packet)
        {
            //base.processPacket(_packet); // Will Reprocess the Old One


            CarInformation[] carInformation = _packet.carInformation;
            byte carIndex;
            for (byte itr = 0; itr < carInformation.Length; itr++)
            {
                if (carInformation[itr].carId == 0)
                    continue;

                carIndex = GetCarIndex(carInformation[itr].carId);
                if (driverList[carIndex].LicenceId == 0) // This Bypass the Host CAR.
                    continue;

                ((Car)driverList[carIndex]).ProcessCarInformation(carInformation[itr]);
                race.ProcessCarInformation(((Car)driverList[carIndex]));
            }
        }      // Multiple Car Information
        protected sealed override void processPacket(PacketMSO _packet)
        {
            //base.processPacket(_packet);
            //Chat_User_Type chatUserType = allo;

            //_packet.chatUserType;
            if (!IsExistLicenceId(_packet.tempLicenceId))
                return;

            Driver _driver = driverList[GetFirstLicenceIndex(_packet.tempLicenceId)];

            if (_packet.message[_packet.textStart] == commandPrefix)
            {
                Log.debug(GetSessionNameForLog() + " Received Command: " + _packet.message.Substring(_packet.textStart) + ", From LicenceUser: " + _driver.LicenceName + "\r\n");


                CommandExec(_driver.AdminFlag, _driver.LicenceName, _packet.message.Substring(_packet.textStart));
            }
            else if (_packet.chatUserType == Chat_User_Type.CHAT_USER_TYPE_SYSTEM)
            {
                Program.ircClient.SendToChannel(GetSessionNameForLog() + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03"));
                Log.chat(GetSessionNameForLog() + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03") + "\r\n");
            
            }
            else
            {
                Program.ircClient.SendToChannel(GetSessionNameForLog() + " " + _driver.DriverName.Replace("^", "\x03") + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03"));
                Log.chat(GetSessionNameForLog() + " " + _driver.DriverName + " Say: " + _packet.message.Substring(_packet.textStart).Replace("^", "\x03") + "\r\n");
            
            }
        }      // message out
        protected sealed override void processPacket(PacketREO _packet)
        {
            base.processPacket(_packet);
            race.Init(_packet);
        }      // Race Grid Order
        protected sealed override void processPacket(PacketRST _packet)
        {
            base.processPacket(_packet);
            race.Init(_packet);

            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                driverList[itr].ProcessRaceStart();
            }

            script.RaceStart();
        }      // Race Start
        protected sealed override void processPacket(PacketSTA _packet)
        {
            base.processPacket(_packet);
            if (_packet.currentCarId == 0)
                race.Init(_packet);

            clientConnectionCount = _packet.connectionCount;
            //else
            //    ;//driver Case

        }      // State Change race/car
        protected sealed override void processPacket(PacketTiny _packet)
        {
            //base.processPacket(_packet);
            switch (_packet.subTinyType)
            {
                case Tiny_Type.TINY_REPLY: PingReceived(); break;
                case Tiny_Type.TINY_REN: Log.debug(GetSessionNameForLog() + " RACE END RACE END RACE END.\r\n"); race.ProcessRaceEnd(); vote.ProcessRaceEnd(); break; //Return Setup Screen(RaceEND)
                case Tiny_Type.TINY_VTC: vote.ProcessVoteCancel(); break;
                case Tiny_Type.TINY_NONE: break;
                default: Log.missingDefinition(GetSessionNameForLog() + " Missing case for TinyPacket: " + _packet.subTinyType + "\r\n"); break;
            }
        }     // Multipurpose 
        protected sealed override void processPacket(PacketSmall _packet)
        {
            //base.processPacket(_packet);
            switch (_packet.subType)
            {
                case Small_Type.SMALL_VTA_VOTE_ACTION:
                {
                    vote.ProcessVoteAction((Vote_Action)_packet.uintValue);
                } break;
                default: Log.missingDefinition(GetSessionNameForLog() + " Missing case for SmallPacket: " + (Small_Type)_packet.subType + "\r\n"); break;
            }
        }    // Multipurpose 
        protected sealed override void processPacket(PacketLAP _packet)
        {
            base.processPacket(_packet);

            driverList[GetCarIndex(_packet.carId)].ProcessLapInformation(_packet);
        }      // Lap Completed
        protected sealed override void processPacket(PacketSPX _packet)
        {
            base.processPacket(_packet);

            driverList[GetCarIndex(_packet.carId)].ProcessSplitInformation(_packet);
        }      // Split Time Receive
        protected sealed override void processPacket(PacketRES _packet)
        {
            base.processPacket(_packet);
            race.ProcessResult(_packet);
        }      // Result, "Confimation" is only working for a race not qualify
        protected sealed override void processPacket(PacketFIN _packet)
        {
            base.processPacket(_packet);
            race.ProcessResult(_packet);
        }      // Final Result, Only into a Race
        protected sealed override void processPacket(PacketBTC _packet)
        {
            base.processPacket(_packet);

            byte driverIndex = GetLicenceIndexNoBot(_packet.licenceId);
            
            switch((Button_Entry)((Button)driverList[driverIndex]).GetButtonEntry(_packet.buttonId))
            {
                case Button_Entry.MOTD_BUTTON_DRIVE:
                {
                    ((Button)driverList[driverIndex]).RemoveGui((ushort)Gui_Entry.MOTD);
                } break;
                case Button_Entry.VOTE_OPTION_1: vote.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_1,_packet.licenceId); break;
                case Button_Entry.VOTE_OPTION_2: vote.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_2, _packet.licenceId); break;
                case Button_Entry.VOTE_OPTION_3: vote.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_3, _packet.licenceId); break;
                case Button_Entry.VOTE_OPTION_4: vote.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_4, _packet.licenceId); break;
                case Button_Entry.VOTE_OPTION_5: vote.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_5, _packet.licenceId); break;
                case Button_Entry.VOTE_OPTION_6: vote.ProcessVoteNotification(Vote_Action.VOTE_CUSTOM_6, _packet.licenceId); break;
                default:
                {
                    Log.error("We recevied a button ClickId, from unknow source, licenceName: " + driverList[driverIndex].LicenceName + ", LicenceIndex: "+driverIndex+"\r\n");
                } break;
            }
        }      // Button Click Receive
        protected sealed override void processPacket(PacketVTN _packet)
        {
            vote.ProcessVoteNotification(_packet.voteAction,_packet.tempLicenceId);
        }       //Vote Notification
        #endregion

        #region Driver/Car/Licence Association Tool
        private byte GetCarIndex(byte _carId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((ICar)driverList[itr]).CarId == _carId)
                    return itr;
            }
            return 0;
        }
        private List<byte> GetLicenceIndexs(byte _licenceId)
        {
            List<byte> _return = new List<byte>();

            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((ILicence)driverList[itr]).LicenceId == _licenceId)
                    _return.Add(itr);
            }
            return _return;
        }
        private byte GetLicenceIndexWithName(byte _licenceId, string _driverName)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((ILicence)driverList[itr]).LicenceId == _licenceId && ((IDriver)driverList[itr]).DriverName == _driverName)
                    return itr;
            }
            return 0;
        }
        private byte GetLicenceIndexNoBot(byte _licenceId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (driverList[itr].LicenceId == _licenceId && !driverList[itr].IsBot())
                    return itr;
            }
            return 0;
        }
        private byte GetFirstLicenceIndex(byte _licenceId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((ILicence)driverList[itr]).LicenceId == _licenceId)
                    return itr;
            }
            return 0;
        }
        private bool IsExistLicenceId(byte _licenceId)
        {
            int count = driverList.Count;
            for (byte itr = 0; itr < count; itr++)
            {
                if (((ILicence)driverList[itr]).LicenceId == _licenceId)
                    return true;
            }
            return false;
        }
        public byte GetNbrOfDrivers()
        {
            return (byte)(driverList.Count - 1); // -1 remove the Host but... maybe not good idea removing it from here.
        }
        public byte GetNbrOfConnection()
        {
            return (byte)(clientConnectionCount - 1); //Remove The Host
        }
        #endregion
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