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
    using Packet_;
    using Definition_;
    using Game_;
    using Log_;
    using InSim_;

    public abstract class Store
    {
        protected Store()
        {
        }
        ~Store()
        {
            if (true == false) { }
        }
        protected PacketStructureList struturedPacket = new PacketStructureList();
        private Queue<Packet> udpReceivedQueud = new Queue<Packet>();     
        private Queue<Packet> udpSendingQueud = new Queue<Packet>();       
        private Queue<Packet> tcpReceivedQueud = new Queue<Packet>();     
        private Queue<Packet> tcpSendingQueud = new Queue<Packet>();   

        protected void AddToUdpReceiveQueud(Packet _packet)
        {
            lock (udpReceivedQueud)
            {
                udpReceivedQueud.Enqueue(_packet);
            }
        }
        protected void AddToUdpSendingQueud(Packet _packet)
        {
            lock (udpSendingQueud)
            {
                udpSendingQueud.Enqueue(_packet);
            }
        }

        protected void AddToTcpReceiveQueud(Packet _packet)
        {
            lock (tcpReceivedQueud)
            {
                tcpReceivedQueud.Enqueue(_packet);
            }
        }
        protected void AddToTcpSendingQueud(Packet _packet)
        {
            if ((int)_packet.packetType < 1)
                Log.error("AddToUdpSendingQueud(Packet), packetType->" + _packet.packetType + ", packetSize->" + _packet.packetSize);

            if (!((InSimClient)this).IsConnected())
                return;
            lock (tcpSendingQueud)
            {
                tcpSendingQueud.Enqueue(_packet);
            }
        }

        protected byte[] NextUdpSendQueud()
        {
            Packet packet = null;
            lock(udpSendingQueud)
            {
                if (udpSendingQueud.Count > 0)
                {
                    packet = udpSendingQueud.Dequeue();
                    if (!struturedPacket.ContainsKey(packet.packetType))
                    {
                        Log.missingDefinition(((Session)this).GetSessionNameForLog() + " NextUdpSendQueud(), No structure defined for this PacketType->" + packet.packetType + "\r\n");
                        return null;
                    }
                }
            }
            if(packet != null)
                return packet.data;
            return null;
        }
        protected byte[] NextTcpSendQueud()
        {
            Packet packet = null;
            lock(tcpSendingQueud)
            {
                if (tcpSendingQueud.Count > 0)
                {
                    packet = tcpSendingQueud.Dequeue();
                    //This is a patented fix for a Flooding probleme occuring...
                    
                    if(packet == null)
                    {
                        Log.error("NextTcpSendQueud(), Strange Flooding of Null Packet Occur, Cleanup Engaged.\r\n");
                        while(tcpSendingQueud.Count > 0)
                        {
                            packet = tcpSendingQueud.Dequeue();
                            if(packet != null)
                                break;
                        }
                    }
                }
            }

            if (packet != null)
            {
                if (!struturedPacket.ContainsKey(packet.packetType))
                {
                    Log.missingDefinition(((Session)this).GetSessionNameForLog() + " NextTcpSendQueud(), No structure defined for this PacketType->" + packet.packetType + "\r\n");
                    return null;
                }
                return packet.data;
            }
            return null;
        }

        protected object[] NextUdpReceiveQueud(bool _returnStruct)
        {
            if (udpReceivedQueud.Count < 1)
                return null;

            Packet packet = udpReceivedQueud.Dequeue();
            if (!struturedPacket.ContainsKey(packet.packetType))
            {
                Log.missingDefinition(((Session)this).GetSessionNameForLog() + " NextUdpReceiveQueud(bool _returnStruct), No structure defined for this PacketType->" + packet.packetType + "\r\n");
                return null;
            }
            object[] _return = new object[2];
            _return[0] = packet.packetType;
            _return[1] = toStruct(packet.packetType, packet.data);
            return _return;
        }
        protected byte[] NextUdpReceiveQueud()
        {
            if (udpReceivedQueud.Count < 1)
                return null;

            Packet packet = udpReceivedQueud.Dequeue();
            if (!struturedPacket.ContainsKey(packet.packetType))
            {
                Log.missingDefinition(((Session)this).GetSessionNameForLog() + " NextUdpReceiveQueud(), No structure defined for this PacketType->" + packet.packetType + "\r\n");
                return null;
            }
            return packet.data;
        }
        protected object[] NextTcpReceiveQueud(bool _returnStruct)
        {
            if (tcpReceivedQueud.Count < 1)
                return null;

            Packet packet = tcpReceivedQueud.Dequeue();
            if (!struturedPacket.ContainsKey(tcpReceivedQueud.Peek().packetType))
            {
                Log.missingDefinition(((Session)this).GetSessionNameForLog() + " NextTcpReceiveQueud(bool _returnStruct), No structure defined for this PacketType->" + packet.packetType + "\r\n");
                return null;
            }
            object[] _return = new object[2];
            _return[0] = packet.packetType;
            _return[1] = toStruct(packet.packetType, packet.data);
            return _return;
        }
        protected byte[] NextTcpReceiveQueud()
        {
            if (tcpReceivedQueud.Count < 1)
                return null;

            Packet packet = tcpReceivedQueud.Dequeue();
            if (!struturedPacket.ContainsKey(packet.packetType))
            {
                Log.missingDefinition(((Session)this).GetSessionNameForLog() + " NextTcpReceiveQueud(), No structure defined for this PacketType->" + packet.packetType + "\r\n");
                return null;
            }
           return packet.data;
        }

        protected object toStruct(Packet_Type _packetType, byte[] _data)
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
