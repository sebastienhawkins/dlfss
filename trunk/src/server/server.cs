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
namespace Drive_LFSS.Server_
{
    using System;
    using System.Collections.Generic;
    using Drive_LFSS.InSim_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Game_;

    public abstract class Server : InSim
	{
        public Server(ushort _serverId, InSimSetting _inSimSetting): base(_inSimSetting)
        {
            serverId = _serverId;
            command = new CommandServer(serverId);
        }

        public bool connectionRequest = true;           //System that goes with this, is not good made... have to think it better way... later
        private ushort serverId;
        public ushort ServerId
        {
            get { return serverId; }
        }
        private CommandServer command;

        #region update

        public void update(uint diff)                       //update called By Session
        {
            object[] _nextTcpPacket = NextTcpReceiveQueud(true);
            if (_nextTcpPacket != null)
                ProcessPacket((Packet_Type)_nextTcpPacket[0], _nextTcpPacket[1]);

            object[] _nextUdpPacket = NextUdpReceiveQueud(true);
            if (_nextUdpPacket != null)
                ProcessPacket((Packet_Type)_nextUdpPacket[0], _nextUdpPacket[1]);
        }
        
        #endregion

        new public void AddToTcpSendingQueud(Packet _serverPacket)
        {
            base.AddToTcpSendingQueud(_serverPacket);
        }
        protected void commandExec(bool _adminStatus, string _licenceName, string _commandText)
        {
            command.Exec(_adminStatus, _licenceName, _commandText);
        }
        private void SimiliButtonTesting(byte uCID)
        {
            //Send_BTN_CreateButton("Welcome On ^5Aleajecta", Button_Styles_Flag.ISB_DARK | Button_Styles_Flag.ISB_RIGHT, 20, 60, 0, 10, 1, uCID, 40, false);
            //Send_BTN_CreateButton("ButtonText", Button_Styles_Flag.ISB_DARK | Button_Styles_Flag.ISB_CLICK, 20, 60, 100, 10, 2, uCID, 40, false);
        } 
	}
}
