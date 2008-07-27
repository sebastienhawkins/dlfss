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
    using Drive_LFSS.Database_;

    public sealed class Session : Server
    {
        public  Session(ushort _serverId, InSimSetting _inSimSetting) : base(_serverId, _inSimSetting)
        {
            driverList = new List<Driver>();
            driverList.Capacity = 192;

            race = new Race();
            commandPrefix = _inSimSetting.CommandPrefix;
        }
        private Race race;
        private List<Driver> driverList;
        private char commandPrefix;

        private byte GetCarIndex(byte _carId)
        {
            for (byte itr = 0; itr < driverList.Count; itr++ )
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
                if ( ((Licence)driverList[itr]).LicenceId == _licenceId)
                    _return.Add(itr);
            }
            return _return;
        }
        private byte GetLicenceIndexWithName(byte _licenceId, string _driverName)
        {
            for (byte itr = 0; itr < driverList.Count; itr++)
            {
                if (((Licence)driverList[itr]).LicenceId == _licenceId && ((Driver)driverList[itr]).prDriverName == _driverName )
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
        new public void update(uint diff)
        {
            base.update(diff);
            foreach (Driver _driver in driverList)
                _driver.update(diff);
        }

        protected sealed override void processPacket(PacketNCN _packet)
        {
            base.processPacket(_packet); // Will Reprocess the Old One

            if (ExistLicenceId(_packet.tempLicenceId))
                log.error("New Licence Connection, But Override a Allready LicenceId, what to do if that Happen???");

            Driver _driver = new Driver();
            _driver.Init(_packet);

            driverList.Add(_driver);
        }
        protected sealed override void processPacket(PacketCNL _packet)
        {
            //TODO: use _packet.Total as a Debug check to be sure we have same racer count into our memory as the server do. 
            base.processPacket(_packet); // Will Reprocess the Old One

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
        protected sealed override void processPacket(PacketNPL _packet)
        {
            base.processPacket(_packet); // Will Reprocess the Old One

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
        protected sealed override void processPacket(PacketPLL _packet)
        {
            base.processPacket(_packet); // Will Reprocess the Old One

            byte itr;
            if ((itr = GetCarIndex(_packet.carId)) == 0)
            {
                log.error("Car Leave race, But no Car Association Found , what todo???");
                return;
            }

            //Do a Init in case we need a Action happen into Car when leave race....
            ((Car)driverList[itr]).Init(_packet); //Static Cast Car object since only him is needed into that case.
        }
        protected sealed override void processPacket(PacketMCI _packet)
        {
            base.processPacket(_packet); // Will Reprocess the Old One

            CarInformation[] carInformation = _packet.carInformation;
            for (byte itr = 0; itr < carInformation.Length; itr++)
                driverList[GetCarIndex(carInformation[itr].carId)].Init(carInformation[itr]);

            //log.network("Received MCI Packet... From Session\r\n");
        }
        protected sealed override void processPacket(PacketMSO _packet)
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

                
                ((Server)this).commandExec(_driver.prAdminFlag, _driver.LicenceName, _packet.message.Substring(_packet.textStart));
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