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
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;

using System.Threading;

namespace Drive_LFSS.InSim_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Server_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Config_;
    using Drive_LFSS.Game_;

    public struct InSimSetting
    {
        internal string serverName;
        internal IPAddress ip;
        internal ushort tcpPort;
        internal ushort udpPort;
        internal string password;
        internal char commandPrefix;
        internal string appName;
        internal InSim_Flag insimMask;
        internal ushort requestInterval;
        internal uint networkInterval;

        internal InSimSetting(string _serverName, string _ip, ushort _tcpPort, ushort _udpPort, string _password, char _commandPrefix, string _appName, InSim_Flag _insimMask, ushort _requestInterval, uint _networkInterval)
        {
            serverName = _serverName;
            ip = IPAddress.Parse(_ip);
            tcpPort = _tcpPort;
            udpPort = _udpPort;
            password = _password;
            commandPrefix = _commandPrefix;
            appName = _appName;
            insimMask = _insimMask;
            requestInterval = _requestInterval;
            networkInterval = _networkInterval;
        }
    }

    abstract class InSimClient : PacketHandler
    {
        internal InSimClient(InSimSetting _inSimSetting)
        {
            threadSocketSendReceive = new Thread(new ThreadStart(SocketSendReceive));
            threadSocketSendReceive.Priority = ThreadPriority.Highest;
            threadSocketSendReceive.Name = inSimSetting.serverName + " Network Thread";
            
            threadConnectionProcess = new Thread(new ThreadStart(Connect));
            threadConnectionProcess.SetApartmentState(ApartmentState.STA);
            threadConnectionProcess.Priority = ThreadPriority.BelowNormal;
            //No need to name here, she named into DoConnect().

            inSimSetting = _inSimSetting;
            //inSimSetting.ip = Dns.Resolve(inSimSetting.ip.ToString()).AddressList[0];

            ConfigApply();
            
            threadSocketSendReceive.Start();

        }
        ~InSimClient()
        {
            if (true == false) { }
        }
        private InSimSetting inSimSetting;
        private bool runThreadSocketReceive = false;
        private Thread threadSocketSendReceive;
        private Thread threadConnectionProcess;
        private int networkThreadSleep = 49;
        private TcpClient tcpClient;
        private NetworkStream tcpSocket;
        private UdpClient udpClient;
        private IPEndPoint udpIpEndPoint;
        private bool ISISended = false;

        protected void ConfigApply()
        {
            networkThreadSleep = (int)inSimSetting.networkInterval;
        }
        protected void SetInSimSetting(InSimSetting _inSimSetting)
        {
            inSimSetting = _inSimSetting;
        }

        private void Connect()
        {
            if (InitUdpSocket() && InitTcpSocket())
            {
                SendConnectISI();

                PacketTiny _packet = new PacketTiny(1, Tiny_Type.TINY_SST_STATE_INFO);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_NCN_NEW_LICENCE_CONNECTION);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_NPL);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_REO_GRID_ORDER);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_RST_RACE_START);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_SST_STATE_INFO);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));
                
                _packet = new PacketTiny(1, Tiny_Type.TINY_RES);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));
            }
            else
                SetDisconnected();
        }
        private bool InitTcpSocket()
        {
            tcpClient = new TcpClient();
            tcpClient.NoDelay = false;
            tcpClient.SendTimeout = 1100;
            tcpClient.ReceiveTimeout = 1100;
            //tcpClient.ExclusiveAddressUse = false; //Create Crash, see ticket #3
            try 
            { 
                tcpClient.Connect(new IPEndPoint(inSimSetting.ip, inSimSetting.tcpPort)); 
            }
            catch (SocketException _exception)
            {
                Log.error(((Session)this).GetSessionNameForLog() + " TCP socket initialization failed, error was: " + _exception.Message + "\r\n");
                return false;
            }
            tcpSocket = tcpClient.GetStream();
            return true;
        }
        private bool InitUdpSocket()
        {
            udpIpEndPoint = new IPEndPoint(IPAddress.Any, inSimSetting.udpPort);
            try { udpClient = new UdpClient(udpIpEndPoint); }
            catch (SocketException _exception)
            {
                Log.error(((Session)this).GetSessionNameForLog() + " UDP socket initialization failed, error was: " + _exception.Message + "\r\n");
                return false;
            }
            udpClient.Ttl = 10;
            return true;
        }
        private void SendConnectISI()
        {
            ISISended = true;
            PacketISI packetISI = new PacketISI(1, inSimSetting.udpPort, (ushort)inSimSetting.insimMask, inSimSetting.commandPrefix, inSimSetting.requestInterval, inSimSetting.password, inSimSetting.appName);
            AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_ISI, Packet_Type.PACKET_ISI_INSIM_INITIALISE, packetISI));
        }
        private void SetDisconnected()
        {
            ISISended = false;
            if (tcpClient != null)
                tcpClient.Close();

            if (tcpSocket != null)
                tcpSocket.Dispose();

            if (udpClient != null)
                udpClient.Close();
        }

        internal void DoConnect()
        {
            if (threadConnectionProcess.ThreadState != ThreadState.Unstarted)
            {
                threadConnectionProcess = new Thread(new ThreadStart(Connect));
                threadConnectionProcess.SetApartmentState(ApartmentState.STA);
                threadConnectionProcess.Priority = ThreadPriority.BelowNormal;
            }

            threadConnectionProcess.Name = inSimSetting.serverName + " Connection Thread";
            threadConnectionProcess.Start();
        }
        internal bool IsConnecting()
        {
            return (threadConnectionProcess.ThreadState == ThreadState.Running);
        }
        internal bool IsConnected()
        {
            //Important "ISISended" stay first and "tcpClient" 
            //is not called when false , since can mean tcpClient is destroy
            if (ISISended && tcpClient != null && tcpClient.Connected)
                return true;

            return false;
        }
        internal void Disconnect()
        {
            if (IsConnected())
            {
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, new PacketTiny(1, Tiny_Type.TINY_CLOSE)));
                TcpSend();
            }
            SetDisconnected();
        }

        private void SocketSendReceive()
        {
            while(Program.MainRun)
            {
                while (IsConnected())
                {
                    TcpReceive();
                    UdpReceive();
                    TcpSend();
                    UdpSend();
                    System.Threading.Thread.Sleep(networkThreadSleep);
                }
                System.Threading.Thread.Sleep(networkThreadSleep);
            }
        }
        private void TcpSend()
        {
            byte[] _packet;
            while ((_packet = NextTcpSendQueud()) != null)
            {
                #if DEBUG
                Log.network(((Session)this).GetSessionNameForLog() + " TcpSend(), Sending packet: " + (Packet_Type)_packet[1] + "\r\n");
                #endif

                try{tcpSocket.Write(_packet,0,_packet.Length);}
                catch(Exception _exception)
                {
                    SetDisconnected();
                    Log.error(((Session)this).GetSessionNameForLog() + " TcpSend(), Exception received when writing on the TCP socket, exception:" + _exception.Message + "\r\n");
                    return;
                }
            }
        }
        private void UdpSend()
        {
            byte[] _packet;
            while( (_packet = NextUdpSendQueud()) != null)
            {
                #if DEBUG
                Log.network(((Session)this).GetSessionNameForLog() + " UdpSend(), Sending packet: " + (Packet_Type)_packet[1] + "\r\n");
                #endif

                udpClient.Send(_packet, _packet.Length);
            }
        }
        private void TcpReceive()
        {
            while (tcpSocket.DataAvailable)
            {
                byte packetSize = 0;
                try { packetSize = (byte)tcpSocket.ReadByte(); }
                catch (Exception _exception)
                {
                    SetDisconnected();
                    Log.error(((Session)this).GetSessionNameForLog() + " TcpReceive(), Exception received when reading on the TCP socket, exception:" + _exception.Message + "\r\n");
                    return;
                }

                if (packetSize < 4) //maybe add a size/4 Check to be sure the Size is Conform to Scawen standart
                {
                    #if DEBUG
                    Log.network(((Session)this).GetSessionNameForLog() + " TcpReceive(), Dropped packet, too Small!, PacketSize->" + packetSize + "\r\n");
                    #endif
                    
                    continue;
                }

                Packet_Type packetType = (Packet_Type)tcpSocket.ReadByte();
                byte[] data = new byte[packetSize];
                data[0] = packetSize;
                data[1] = (byte)packetType;
                tcpSocket.Read(data, 2, (packetSize - 2));
                if (!struturedPacket.ContainsKey((Packet_Type)data[1]))
                {
                    Log.missingDefinition(((Session)this).GetSessionNameForLog() + " TcpReceive(), No structure defined for this PacketType->" + (Packet_Type)data[1] + "\r\n");
                    continue;
                }
                ProcessPacket((Packet_Type)data[1], toStruct((Packet_Type)data[1], data));
            }
        }
        private void UdpReceive()
        {
            if(!IsConnected())
                return;
            while (udpClient.Available > 1)
            {
                byte[] data = new byte[udpClient.Available];
                data = udpClient.Receive(ref udpIpEndPoint);

                if (data[0] < 3)
                {
                    #if DEBUG
                    Log.network(((Session)this).GetSessionNameForLog() + " UdpReceive(), Dropped packet, too small!, PacketSize->" + data[0] + "\r\n");
                    #endif
                    continue;
                }
                if (!struturedPacket.ContainsKey((Packet_Type)data[1]))
                {
                    Log.missingDefinition(((Session)this).GetSessionNameForLog() + " UdpReceive(), No structure defined for this PacketType->" + (Packet_Type)data[1] + "\r\n");
                    continue;
                }
                ProcessPacket((Packet_Type)data[1], toStruct((Packet_Type)data[1], data));
            }
        }
    }
}
