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
#define START_OUTSIM_INTERFACE

#undef START_OUTSIM_INTERFACE   // Comment this line, to Load File

#if START_OUTSIM_INTERFACE
namespace Drive_LFSS.OutSim
{
    //using System.Runtime.CompilerServices;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.InSimShared_;

    public sealed class OutSimInterface : Socket
    {
        private string _name = "Default OutSimInterface";

        public event OutSim_EventHandler OutSim_Received;

        public OutSimInterface(ushort Port)
        {
            base._port = Port;
        }

        protected override void DataReceived(byte[] data)
        {
            if (data.Length == 0x44)
            {
                OutSimPack destStruct = new OutSimPack();
                destStruct = (OutSimPack) PacketUtil.DataToPacket(data, destStruct);
                if (this.OutSim_Received != null)
                {
                    this.OutSim_Received(destStruct);
                }
            }
        }

        public string Name
        {
            get
            {
                return this._name;
            }
            set
            {
                this._name = value;
            }
        }

        public delegate void OutSim_EventHandler(OutSimPack os);
    }
}
#endif