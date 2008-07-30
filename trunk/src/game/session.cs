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
    using Drive_LFSS.Server_;
    using Drive_LFSS.InSim_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Script_;
    

    public sealed class Session : Server
    {
        public  Session(ushort _serverId, InSimSetting _inSimSetting) : base(_serverId, _inSimSetting)
        {
            commandPrefix = _inSimSetting.CommandPrefix;
            race = new Race(_serverId);

            driverList = new List<Driver>();
            driverList.Add(new Driver()); //put Default Driver 0, will save some If.
            driverList.Capacity = 192;

            script = new ScriptSession();
            //script
        }
        struct Ping
        {
            public Ping(uint _diff)
            {
                pingTime = DateTime.Now.Ticks;
                pingRequestId = 1;
                sessionLatency = 0;
                diff = _diff;
            }
            public long Received()
            {
                return (sessionLatency = (DateTime.Now.Ticks - pingTime)/10000);
            }
            public long GetReactionTime()
            {
                return sessionLatency - diff;
            }
            private long pingTime;
            private byte pingRequestId;
            private long sessionLatency;
            private uint diff;
            public long GetSessionLatency
            {
                get{return sessionLatency;}
            }
        }
        private char commandPrefix;

        private iScriptSession script;
        private Race race;
        private List<Driver> driverList;

        private Ping ping;

        #region Update/Timer

        private const uint TIMER_PING_PONG = 30000;
        private uint TimerPingPong = 30000;

        new public void update(uint diff)
        {
            if (TIMER_PING_PONG < (TimerPingPong += diff))
            {
                log.ping("Ping!\r\n");
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, new PacketTiny(1, Tiny_Type.TINY_PING)));
                ping = new Ping(diff);
                TimerPingPong = 0;
                //script.CarFinishRace(driverList[0]);
            }

            base.update(diff);

            foreach (Driver _driver in driverList)
                _driver.update(diff);

            race.update(diff);
        }
        #endregion

        #region Process packet
        protected sealed override void processPacket(PacketNCN _packet)
        {
            base.processPacket(_packet); //Keep the Log

            //Since we create the driver index 0 in this.construstor()
            //will conflit with the Host NCN receive packet, so here is a overide!
            //will have to rethink this later, that is looking like a HackFix, suck CPU for nothing.
            if (ExistLicenceId(_packet.tempLicenceId))
            {
                if (_packet.tempLicenceId == 0)
                {
                    driverList[0].Init(_packet);
                }
                else
                {
                    log.error("New Licence Connection, But Override a Allready LicenceId, what to do if that Happen???");
                    return;
                }
            }
            Driver _driver = new Driver();
            _driver.Init(_packet);

            driverList.Add(_driver);
        }
        protected sealed override void processPacket(PacketCNL _packet)
        {
            //TODO: use _packet.Total as a Debug check to be sure we have same racer count into our memory as the server do. 
            base.processPacket(_packet); //Keep the Log

            if (!ExistLicenceId(_packet.tempLicenceId))
            {
                log.error("Licence Disconnection, But no LicenceID associated with It, What todo???");
                return;
            }

            //Driver object for this one, will be totaly destroyed, from is Most Base Class to the Top Most.
            //If we need to excute a function onto disconnection, i sugest doing into Class Destructor! for Each Class ;) Rock And Roll!
            byte itr;
            while((itr = GetFirstLicenceIndex(_packet.tempLicenceId)) != 0)          
                driverList.RemoveAt((int)itr);
        }
        protected sealed override void processPacket(PacketNPL _packet) //New Car Join Race
        {
            base.processPacket(_packet); //Keep the Log

            if (!ExistLicenceId(_packet.tempLicenceId))
            {
                log.error("New Car Join Race, But Not LicenceId Associated What todo???");
                return;
            }

            //this is first version, i expect some probleme into the case, we don't know Driver as Leave the Race or Disconnected... Let See.
            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) == Driver_Type_Flag.DRIVER_TYPE_AI)      //AI
            {
                byte itr;
                if ((itr = GetLicenceIndexWithName(_packet.tempLicenceId, _packet.driverName)) != 0)
                {
                    driverList[itr].Init(_packet);
                }
                else
                {
                    Driver _driver = new Driver();
                    _driver.Init(_packet);
                    driverList.Add(_driver);
                }
            }
            else                                                                            //Human
            {
                driverList[GetLicenceIndexWithName(_packet.tempLicenceId, _packet.driverName)].Init(_packet);
            }
        }
        protected sealed override void processPacket(PacketPLL _packet) // player leave (spectate - loses slot)
        {
            base.processPacket(_packet); //Keep the Log

            byte itr;
            if ((itr = GetCarIndex(_packet.carId)) == 0)
            {
                log.error("Car Leave race, But no Car Association Found , what todo???");
                return;
            }

            //Do a Init in case we need a Action happen into Car when leave race....
            ((Car)driverList[itr]).LeaveRace(_packet);
        }
        protected sealed override void processPacket(PacketMCI _packet) // Multiple Car Information
        {
            base.processPacket(_packet); // Will Reprocess the Old One

            CarInformation[] carInformation = _packet.carInformation;
            for (byte itr = 0; itr < carInformation.Length; itr++)
                ((Car)driverList[GetCarIndex(carInformation[itr].carId)]).ProcessCarInformation(carInformation[itr]);
        }
        protected sealed override void processPacket(PacketMSO _packet) //message out
        {
            base.processPacket(_packet);
            //Chat_User_Type chatUserType = allo;
            
            //_packet.chatUserType;
            if (!ExistLicenceId(_packet.tempLicenceId))
                return;

            Driver _driver = driverList[GetFirstLicenceIndex(_packet.tempLicenceId)];

            if (_packet.message[_packet.textStart] == commandPrefix)
            {
                log.debug("Received Command: " + _packet.message.Substring(_packet.textStart) + ", From LicenceUser: " + _driver.LicenceName + "\r\n");

                
                commandExec(_driver.prAdminFlag, _driver.LicenceName, _packet.message.Substring(_packet.textStart));
            }
            else
            {
                log.chat(_driver.prDriverName + " Say: " + _packet.message.Substring(_packet.textStart) + "\r\n");
            }
        }
        protected sealed override void processPacket(PacketRST _packet) 
        {
            base.processPacket(_packet);
            race.Init(_packet);
        }
        protected sealed override void processPacket(PacketSTA _packet)
        {
            base.processPacket(_packet);
            race.Init(_packet);
            //Can receive some info about the Car that triggered this State Change...
            //TODO: Add this Init into Driver If carId is Found into STA.
        }
        protected sealed override void processPacket(PacketTiny _packet)
        {
            base.processPacket(_packet);
            switch (_packet.subTinyType)
            {
                case Tiny_Type.TINY_REPLY: PingReceived(); break;
                case Tiny_Type.TINY_NONE: break;
                default: Program.log.missingDefinition("Missing case for TinyPacket: " + _packet.subTinyType+"\r\n"); break;
            }
        }
        #endregion

        #region Other
        private void PingReceived()
        {
            log.ping("Pong! " + ping.Received() + "ms\r\n");
        }
        public long GetLatency()
        {
            return ping.GetSessionLatency;
        }
        public long GetReactionTime()
        {
            return ping.GetReactionTime();
        }
        #endregion

        #region Driver/Car/Licence Association Tool
        private byte GetCarIndex(byte _carId)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
            {
                if (driverList[itr].prCarId == _carId)
                    return itr;
            }
            return 0;
        }
        private List<byte> GetLicenceIndex(byte _licenceId)
        {
            List<byte> _return = new List<byte>();

            for (byte itr = 0; itr < driverList.Count; itr++)
            {
                if (((Licence)driverList[itr]).LicenceId == _licenceId)
                    _return.Add(itr);
            }
            return _return;
        }
        private byte GetLicenceIndexWithName(byte _licenceId, string _driverName)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
            {
                if (((Licence)driverList[itr]).LicenceId == _licenceId && ((Driver)driverList[itr]).prDriverName == _driverName)
                    return itr;
            }
            return 0;
        }
        private byte GetFirstLicenceIndex(byte _licenceId)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
            {
                if (((Licence)driverList[itr]).LicenceId == _licenceId)
                    return itr;
            }
            return 0;
        }
        private bool ExistLicenceId(byte _licenceId)
        {
            byte itrEnd = (byte)driverList.Count;

            for (byte itr = 0; itr < itrEnd; itr++)
            {
                if (((Licence)driverList[itr]).LicenceId == _licenceId)
                    return true;
            }
            return false;
        }
        public int GetNbrOfDrivers()
        {
            return driverList.Count - 1;
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
