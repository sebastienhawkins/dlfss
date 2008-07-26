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
#define START_OUTGAUGE_INTERFACE

#undef START_OUTGAUGE_INTERFACE     // Comment this line, to Load File

#if (START_OUTGAUGE_INTERFACE)
namespace Drive_LFSS.OutGauge
{
    using Drive_LFSS.Packet_;
    using Drive_LFSS.InSimShared_;
    //using System;
    //using System.Runtime.CompilerServices;

    public sealed class OutGaugeInterface : Socket
    {
        private string _name = "Default OutGaugeInterface";

        public event OutGauge_EventHandler OutGauge_Received;

        public OutGaugeInterface(ushort Port)
        {
            base._port = Port;
        }

        protected override void DataReceived(byte[] data)
        {
            if (data.Length == 0x60)
            {
                OutGaugePack destStruct = new OutGaugePack();
                destStruct = (OutGaugePack) PacketUtil.DataToPacket(data, destStruct);
                if (this.OutGauge_Received != null)
                {
                    this.OutGauge_Received(destStruct);
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

        public delegate void OutGauge_EventHandler(OutGaugePack og);
    }
}
#endif