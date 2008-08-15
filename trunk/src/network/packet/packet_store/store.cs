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
using System.Runtime.InteropServices;

namespace Drive_LFSS.PacketStore_
{
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Server_;
    using Drive_LFSS.Log_;

    public abstract class Store
    {
        public Store()
        {
            struturedPacket = new PacketStructureList();
            udpReceivedQueud = new List<Packet>();
            udpSendingQueud = new List<Packet>();
            tcpReceivedQueud = new List<Packet>();
            tcpSendingQueud = new List<Packet>();
        }
        private PacketStructureList struturedPacket;
        private List<Packet> udpReceivedQueud;      //Replace List with Queud
        private List<Packet> udpSendingQueud;       //Replace List with Queud
        private List<Packet> tcpReceivedQueud;      //Replace List with Queud
        private List<Packet> tcpSendingQueud;       //Replace List with Queud

        protected void AddToUdpReceiveQueud(Packet _serverPacket)
        {
            lock (udpReceivedQueud)
            {
                udpReceivedQueud.Add(_serverPacket);
            }
        }
        protected void AddToUdpSendingQueud(Packet _serverPacket)
        {
            lock (udpSendingQueud)
            {
                udpSendingQueud.Add(_serverPacket);
            }
        }

        protected void AddToTcpReceiveQueud(Packet _serverPacket)
        {
            lock (tcpReceivedQueud)
            {
                tcpReceivedQueud.Add(_serverPacket);
            }
        }
        protected void AddToTcpSendingQueud(Packet _serverPacket)
        {
            lock (tcpSendingQueud)
            {
                tcpSendingQueud.Add(_serverPacket);
            }
        }

        protected byte[] NextUdpSendQueud()
        {
            if (udpSendingQueud.Count < 1)
                return null;

            if (!struturedPacket.ContainsKey(udpSendingQueud[0].packetType))
            {
                Log.missingDefinition("NextUdpSendQueud(), No Structure Define for this PacketType->" + udpSendingQueud[0].packetType + "\r\n");
                lock (udpSendingQueud) { udpSendingQueud.Remove(udpSendingQueud[0]); }
                return null;
            }
            byte[] _return = udpSendingQueud[0].data;
            lock (udpSendingQueud) { udpSendingQueud.Remove(udpSendingQueud[0]); }
            return _return;
        }
        protected byte[] NextTcpSendQueud()
        {
            if (tcpSendingQueud.Count < 1)
                return null;

            if (!struturedPacket.ContainsKey(tcpSendingQueud[0].packetType))
            {
                Log.missingDefinition("NextTcpSendQueud(), No Structure Define for this PacketType->" + tcpSendingQueud[0].packetType + "\r\n");
                lock (tcpSendingQueud) { tcpSendingQueud.Remove(tcpSendingQueud[0]); }
                return null;
            }
            byte[] _return = tcpSendingQueud[0].data;
            lock (tcpSendingQueud) { tcpSendingQueud.Remove(tcpSendingQueud[0]); }
            return _return;
        }

        protected object[] NextUdpReceiveQueud(bool _returnStruct)
        {
            if (udpReceivedQueud.Count < 1)
                return null;
            
            if (!struturedPacket.ContainsKey(udpReceivedQueud[0].packetType))
            {
                Log.missingDefinition("NextUdpReceiveQueud(bool _returnStruct), No Structure Define for this PacketType->" + udpReceivedQueud[0].packetType + "\r\n");
                lock (udpReceivedQueud) { udpReceivedQueud.Remove(udpReceivedQueud[0]); }
                return null;
            }
            object[] _return = new object[2];
            _return[0] = udpReceivedQueud[0].packetType;
            _return[1] = toStruct(udpReceivedQueud[0].packetType, udpReceivedQueud[0].data);
            lock (udpReceivedQueud) { udpReceivedQueud.Remove(udpReceivedQueud[0]); }
            return _return;
        }
        protected byte[] NextUdpReceiveQueud()
        {
            if (udpReceivedQueud.Count < 1)
                return null;

            if (!struturedPacket.ContainsKey(udpReceivedQueud[0].packetType))
            {
                Log.missingDefinition("NextUdpReceiveQueud(), No Structure Define for this PacketType->" + udpReceivedQueud[0].packetType + "\r\n");
                lock (udpReceivedQueud) { udpReceivedQueud.Remove(udpReceivedQueud[0]); }
                return null;
            }
            byte[] _return = udpReceivedQueud[0].data;
            lock (udpReceivedQueud) { udpReceivedQueud.Remove(udpReceivedQueud[0]); }
            return _return;
        }
        protected object[] NextTcpReceiveQueud(bool _returnStruct)
        {
            if (tcpReceivedQueud.Count < 1)
                return null;

            if (!struturedPacket.ContainsKey(tcpReceivedQueud[0].packetType))
            {
                Log.missingDefinition("NextTcpReceiveQueud(bool _returnStruct), No Structure Define for this PacketType->" + tcpReceivedQueud[0].packetType + "\r\n");
                lock (tcpReceivedQueud) { tcpReceivedQueud.Remove(tcpReceivedQueud[0]); }
                return null;
            }
            object[] _return = new object[2];
            _return[0] = tcpReceivedQueud[0].packetType;

            _return[1] = toStruct(tcpReceivedQueud[0].packetType, tcpReceivedQueud[0].data);
            lock (tcpReceivedQueud) { tcpReceivedQueud.Remove(tcpReceivedQueud[0]); }
            return _return;
        }
        protected byte[] NextTcpReceiveQueud()
        {
            if (tcpReceivedQueud.Count < 1)
                return null;

            if (!struturedPacket.ContainsKey(tcpReceivedQueud[0].packetType))
            {
                Log.missingDefinition("NextTcpReceiveQueud(), No Structure Define for this PacketType->" + tcpReceivedQueud[0].packetType + "\r\n");
                lock (tcpReceivedQueud) { tcpReceivedQueud.Remove(tcpReceivedQueud[0]); }
                return null;
            }
            byte[] _return = tcpReceivedQueud[0].data;
            lock (tcpReceivedQueud) { tcpReceivedQueud.Remove(tcpReceivedQueud[0]); }
            return _return;
        }

        internal object toStruct(Packet_Type _packetType, byte[] _data)
        {
            //Program.Log.network("toStruct(), Constructor For packetType->" + _packetType + "\r\n");

            object _struct = Activator.CreateInstance(struturedPacket[_packetType].GetType(), new string[] { });

            IntPtr pStruct = Marshal.AllocHGlobal(Marshal.SizeOf(_struct));
            GCHandle hStruct = GCHandle.Alloc(pStruct, GCHandleType.Pinned);
            Marshal.Copy(_data, 0, pStruct, _data.Length);
            _struct = Marshal.PtrToStructure(pStruct, _struct.GetType());
            hStruct.Free();
            Marshal.FreeHGlobal(pStruct);

            return _struct;
        }

    }
}
//TypedReference __makeref(new int());Type __reftype();__refvalue;