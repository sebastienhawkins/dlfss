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
    using Drive_LFSS.Session_;

    public struct InSimSetting
    {
        public string serverName;
        public IPAddress ip;
        public ushort port;
        public string password;
        public char commandPrefix;
        public string appName;
        public InSim_Flag insimMask;
        public ushort requestInterval;
        public uint networkInterval;

        public InSimSetting(string _serverName, string _ip, ushort _port, string _password, char _commandPrefix, string _appName, InSim_Flag _insimMask, ushort _requestInterval, uint _networkInterval)
        {
            serverName = _serverName;
            ip = IPAddress.Parse(_ip);
            port = _port;
            password = _password;
            commandPrefix = _commandPrefix;
            appName = _appName;
            insimMask = _insimMask;
            requestInterval = _requestInterval;
            networkInterval = _networkInterval;
        }
    }

    public abstract class InSim : PacketHandler
    {
        public InSim(InSimSetting _inSimSetting)
        {
            inSimSetting = _inSimSetting;

            if (inSimSetting.password.Length > 16)
                Log.error(inSimSetting.serverName + " bad Configuration For: password must be 16 characters long maximun.\r\n");
            else if (inSimSetting.appName.Length > 16)
                Log.error(inSimSetting.serverName + " bad Configuration For: appName must be 16 characters long maximun.\r\n");
            else if (inSimSetting.port < 1024)
                Log.error(inSimSetting.serverName + " bad Configuration For: Port must be greater 1024.\r\n");
            else  //All Good
            {
                ConfigApply();
                threadSocketSendReceive = new Thread(new ThreadStart(SocketSendReceive));
            }
        }
        ~InSim()
        {
            if (true == false) { }
        }
        private InSimSetting inSimSetting;
        private bool runThreadSocketReceive = false;
        private Thread threadSocketSendReceive;
        private int networkThreadSleep;
        private TcpClient tcpClient = new TcpClient();
        private NetworkStream tcpSocket;
        private UdpClient udpClient;
        private IPEndPoint udpIpEndPoint;

        protected void ConfigApply()
        {
            networkThreadSleep = (int)inSimSetting.networkInterval;
        }
        public void exit()
        {
            if (tcpClient.Connected)
            {
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY ,Packet_Type.PACKET_TINY_MULTI_PURPOSE,new PacketTiny(1, Tiny_Type.TINY_CLOSE)));
            }
            if (udpClient != null)
            {
                byte[] dgram = new byte[1];
                udpClient.Send(dgram, 1, "localhost", inSimSetting.port);
            }

            if(threadSocketSendReceive.ThreadState == ThreadState.Running)
            {
                runThreadSocketReceive = false;
                threadSocketSendReceive.Join();
            }
        }
        //Complete ReWrite is needed here.
        public void connect()
        {
            //runThreadSocketReceive = false;
            //socketStatus = InSim_Socket_State.INSIM_SOCKET_DISCONNECTED;
            tcpClient.NoDelay = false;
            tcpClient.SendTimeout = 1000;
            tcpClient.ReceiveTimeout = 1000;
            tcpClient.ExclusiveAddressUse = false;
            try{ tcpClient.Connect(new IPEndPoint(inSimSetting.ip, inSimSetting.port)); }
            catch (SocketException _exception)
            {
                Log.error(((Session)this).GetSessionNameForLog()+" TCP Socket Initialization failded, Error was: " +_exception.Message+"\r\n");
                //Maybe a little sleep and retry connect later, this is a isolated thread Call.. so a sleep won't hurt other thread.
                ((Session)this).connectionRequest = true;
                Log.commandHelp(((Session)this).GetSessionNameForLog() + " retry connection\r\n");
                return;
            }
            tcpSocket = tcpClient.GetStream();
            
            PacketISI packetISI = new PacketISI(1, inSimSetting.port, (ushort)inSimSetting.insimMask, inSimSetting.commandPrefix, inSimSetting.requestInterval, inSimSetting.password, inSimSetting.appName);
            AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_ISI, Packet_Type.PACKET_ISI_INSIM_INITIALISE, packetISI));

            System.Threading.Thread.Sleep(1000);

            udpIpEndPoint = new IPEndPoint(IPAddress.Any, inSimSetting.port);
            try { udpClient = new UdpClient(udpIpEndPoint); /*udpClient.Connect(inSimSetting.ip, (int)inSimSetting.port);*/ }
            catch (SocketException _exception)
            {
                Log.error(((Session)this).GetSessionNameForLog() + " UDP Socket Initialization failded, Error was: " + _exception.Message+"\r\n");
                ((Session)this).connectionRequest = true;
                Log.commandHelp(((Session)this).GetSessionNameForLog() + " retry connection\r\n");
                return;
            }

            udpClient.Ttl = 10; // Try to influence a shorter route on the internet if possible, maybe a value from the MCI interval time, 10ms is pretty fast, and 50 it normal.

            if (tcpClient.Connected)
            {
                runThreadSocketReceive = true;

                if (threadSocketSendReceive.ThreadState == ThreadState.Unstarted)
                    threadSocketSendReceive.Start();
                else if (threadSocketSendReceive.ThreadState == ThreadState.Stopped)
                {
                    threadSocketSendReceive = new Thread(new ThreadStart(SocketSendReceive));
                    threadSocketSendReceive.Start();
                }

                PacketTiny _packet = new PacketTiny(1, Tiny_Type.TINY_NCN_NEW_LICENCE_CONNECTION);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_NPL);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_REO_GRID_ORDER);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_RST_RACE_START);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_SST_STATE_INFO);
                AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));


            }
            else
                ((Session)this).connectionRequest = true;
        }
        public bool IsConnected()
        {
            return (tcpClient.Connected);
        }
        private void SocketSendReceive()
        {
            while (runThreadSocketReceive && Program.MainRun)
            {
                System.Threading.Thread.Sleep(networkThreadSleep);
                TcpReceive();
                UdpReceive();
                TcpSend();
                //UdpSend();
            }
        }
        private void TcpSend()
        {
            
            byte[] _packet = NextTcpSendQueud();
            if (_packet == null) 
                return;

            if (!tcpClient.Connected)
            {
                Log.debug(((Session)this).GetSessionNameForLog() + " TcpSend(), Disconnection Detected...\r\n");
                return;
            }

            Log.network(((Session)this).GetSessionNameForLog() + " TcpSend(), Sending packet: " + (Packet_Type)_packet[1] + "\r\n");
            try{tcpSocket.Write(_packet,0,_packet.Length);}
            catch(Exception _exception)
            {
                tcpSocket.Dispose();
                tcpClient.Close();
                udpClient.Close();
                runThreadSocketReceive = false;
                threadSocketSendReceive.Join();
                tcpClient = new TcpClient();
                Log.error(((Session)this).GetSessionNameForLog() + " TcpSend(), Exception received when writing on the TCP socket, exception:" + _exception.Message + "\r\n");
                ((Session)this).connectionRequest = true;
                Log.commandHelp(((Session)this).GetSessionNameForLog() + " retry connection\r\n");
            }
        }
        private void UdpSend()
        {
            byte[] _packet = NextUdpSendQueud();
            if (_packet == null)
                return;

            Log.network(((Session)this).GetSessionNameForLog() + " UdpSend(), Sending packet: " + (Packet_Type)_packet[1] + "\r\n");
            udpClient.Send(_packet, _packet.Length);
        }
        private void TcpReceive()
        {
            if (!tcpClient.Connected || !tcpSocket.DataAvailable)
                return;

            byte packetSize = 0;
            try{packetSize = (byte)tcpSocket.ReadByte();}
            catch(Exception _exception)
            {
                tcpSocket.Dispose();
                tcpClient.Close();
                udpClient.Close();
                runThreadSocketReceive = false;
                threadSocketSendReceive.Join();
                tcpClient = new TcpClient();
                Log.error(((Session)this).GetSessionNameForLog() + " Tcpreceive(), Exception received when reading on the TCP socket, exception:" + _exception.Message + "\r\n");
                ((Session)this).connectionRequest = true;
                Log.commandHelp(((Session)this).GetSessionNameForLog() + " retry connection\r\n");
                return;
            }

            if (packetSize  < 4) //maybe add a size/4 Check to be sure the Size is Conform to Scawen standart
            {
                Log.network(((Session)this).GetSessionNameForLog() + " TcpReceive(), Droped packet, Too Short!, PacketSize->" + packetSize + "\r\n");
                return;
            }

            Packet_Type packetType = (Packet_Type)tcpSocket.ReadByte();

            //Log.network("TcpReceive, PacketSize->" + packetSize + ", PacketType->" + packetType + "\r\n");
            
            // Using the queud mean Main thread will processPacket.
            //AddToTcpReceiveQueud(new Packet((Packet_Size)packetSize, packetType, data));

            // This way make Each Server Process is Own .
            byte[] data = new byte[packetSize];
            data[0] = packetSize;
            data[1] = (byte)packetType;

            tcpSocket.Read(data, 2, (packetSize-2));

            if (!struturedPacket.ContainsKey((Packet_Type)data[1]))
            {
                Log.missingDefinition(((Session)this).GetSessionNameForLog() + " TcpReceive(), No Structure Define for this PacketType->" + (Packet_Type)data[1] + "\r\n");
                return;
            }

            ProcessPacket((Packet_Type)data[1], toStruct((Packet_Type)data[1], data));
        }
        private void UdpReceive()
        {
            if (udpClient != null && udpClient.Available < 1)
                return;

            byte[] data = new byte[udpClient.Available];
            data = udpClient.Receive(ref udpIpEndPoint);
            
            if (data[0] < 3)
            {
                Log.network(((Session)this).GetSessionNameForLog() + " UdpReceive(), Droped packet, Too Short!, PacketSize->" + data[0] + "\r\n");
                return;
            }
            //Log.network("UdpReceive(), PacketSize->" + packetSize + ", PacketType->" + (Packet_Type)data[1] + "\r\n");
            //AddToUdpReceiveQueud(new Packet(data));

            if (!struturedPacket.ContainsKey((Packet_Type)data[1]))
            {
                Log.missingDefinition(((Session)this).GetSessionNameForLog() + " UdpReceive(), No Structure Define for this PacketType->" + (Packet_Type)data[1] + "\r\n");
                return;
            }

            ProcessPacket((Packet_Type)data[1], toStruct((Packet_Type)data[1], data));
        }
    }
}
