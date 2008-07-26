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
namespace Drive_LFSS.InSim_
{
    using System;
    using System.IO;
    using System.ComponentModel;
    using System.Net;
    using System.Net.Sockets;
    //using System.Runtime.CompilerServices;
    using System.Threading;
    //using Drive_LFSS.PacketStore_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Server_;
    using Drive_LFSS.Database_;

    public abstract class InSim : PacketHandler
    {
        public InSim(InSimSetting _inSimSetting)
        {
            string errorMessage = "";
            if (_inSimSetting.adminPassword.Length > 0x10)
            {
                errorMessage += "Administrator Password must be 16 characters long maximun.\r\n";
            }
            if (_inSimSetting.connectionName.Length > 0x10)
            {
                errorMessage += "Connection Name must be 16 characters long maximun.\r\n";
            }
            if (_inSimSetting.tcpPort < 1024)
            {
                errorMessage += "TCP Port number must be > 1024.\r\n";
            }
            if (_inSimSetting.udpPort != 0 && _inSimSetting.udpPort < 1024)
            {
                errorMessage += "UDP Port number must be > 1024.\r\n";
            }
            if (errorMessage != "")
            {
                //TODO
                //throw new InvalidConfigurationException(errorMessage + "\r\nVerify you InSimSettings parameters for incorrect values.");
            }
            inSimSetting = _inSimSetting;
            threadSocketReceive = new Thread(new ThreadStart(SocketReceive));
            //packetStore = new Store();
        }
       // public InSim() { }

        /*public ushort serverId
        {
            get { return ((Server)this).serverId; }
        }*/
        private InSim_Socket_State socketStatus = InSim_Socket_State.INSIM_SOCKET_DISCONNECTED;
        private InSimSetting inSimSetting;
        private bool runThreadSocketReceive = false;
        private Thread threadSocketReceive;
        private TcpClient tcpClient;
        private NetworkStream tcpSocket;
        private UdpClient udpClient;
        private IPEndPoint udpIpEndPoint;

        public void exit()
        {
            if (tcpClient.Connected)
            {
                AddToTcpSendingQueud(new Packet(Protocol_Id.PROTO_TCP, Packet_Size.PACKET_SIZE_TINY ,Packet_Type.PACKET_TINY_MULTI_PURPOSE,new PacketTiny(1, Tiny_Type.TINY_CLOSE)));
            }
            if (udpClient != null)
            {
                byte[] dgram = new byte[1];
                udpClient.Send(dgram, 1, "localhost", inSimSetting.udpPort);
            }

            socketStatus = InSim_Socket_State.INSIM_SOCKET_DISCONNECTED;

            if(threadSocketReceive.ThreadState == ThreadState.Running)
            {
                runThreadSocketReceive = false;
                threadSocketReceive.Join();
            }
        }
        public void connect()
        {
            runThreadSocketReceive = false;
            socketStatus = InSim_Socket_State.INSIM_SOCKET_DISCONNECTED;

            tcpClient = new TcpClient();
            try{ tcpClient.Connect(new IPEndPoint(inSimSetting.serverIp, inSimSetting.tcpPort)); }
            catch (SocketException _exception)
            {
                return;//throw new Exception("TCP Socket Initialization failded, Error was: " +_exception.Message); 
            }
            tcpSocket = tcpClient.GetStream();
            
            PacketISI packetISI = new PacketISI(1, inSimSetting.udpPort, (ushort)inSimSetting.Flags, inSimSetting.CommandPrefix, inSimSetting.MCI_NLP_Interval, inSimSetting.adminPassword, inSimSetting.connectionName);
            AddToTcpSendingQueud(new Packet(Protocol_Id.PROTO_TCP, Packet_Size.PACKET_SIZE_ISI, Packet_Type.PACKET_ISI_INSIM_INITIALISE, packetISI));

            System.Threading.Thread.Sleep(1000);

            udpIpEndPoint = new IPEndPoint(IPAddress.Any, inSimSetting.udpPort);
            try { udpClient = new UdpClient(udpIpEndPoint); }
            catch (SocketException _exception)
            {
                return;//throw new Exception("UDP Socket Initialization failed, Error was: "+_exception.Message); 
            }

            if (tcpClient.Connected)
            {
                socketStatus = InSim_Socket_State.INSIM_SOCKET_CONNECTED;
                runThreadSocketReceive = true;

                if (threadSocketReceive.ThreadState == ThreadState.Unstarted)
                {
                    threadSocketReceive.Start();
                }
                else if (this.threadSocketReceive.ThreadState == ThreadState.Stopped)
                {
                    this.threadSocketReceive = new Thread(new ThreadStart(SocketReceive));
                    this.threadSocketReceive.Start();
                }

                PacketTiny _packet = new PacketTiny(1,Tiny_Type.TINY_NCN_NEW_LICENCE_CONNECTION);
                AddToTcpSendingQueud(new Packet(Protocol_Id.PROTO_TCP,Packet_Size.PACKET_SIZE_TINY,Packet_Type.PACKET_TINY_MULTI_PURPOSE,_packet));

                _packet = new PacketTiny(1, Tiny_Type.TINY_NPL);
                AddToTcpSendingQueud(new Packet(Protocol_Id.PROTO_TCP, Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE, _packet));
            }
        }
        public bool IsSocketStatus(InSim_Socket_State _isStatus)
        {
            return (socketStatus == _isStatus);
        }
        private void SocketReceive()
        {
            while (runThreadSocketReceive && Program.MainRun)
            {
                System.Threading.Thread.Sleep(Program.sleep);
                TcpReceive();
                UdpReceive();
                TcpSend();
                UdpSend();
            }
        }
        private void TcpSend()
        {
            byte[] _packet = NextTcpSendQueud();
            if (_packet == null) 
                return;

            if (!tcpClient.Connected)
            {
                ((Server)this).log.debug("TcpSend(), Disconnection Detected...\r\n");
                return;
            }

            ((Server)this).log.network("TcpSend(), Sending packet: " + (Packet_Type)_packet[1] + "\r\n");
            try{tcpSocket.Write(_packet,0,_packet.Length);}
            catch(Exception _exception)
            {
                tcpSocket.Dispose();
                tcpClient.Close();
                ((Server)this).log.error("TcpSend(), Exception received when writing on the TCP socket, exception:"+_exception+"\r\n");
            }
        }
        private void UdpSend() // This is very not usefull, UDP is a receive only .... from what i know now!
        {
            byte[] _packet = NextUdpSendQueud();
            if (_packet == null)
                return;

            ((Server)this).log.network("UdpSend(), Sending packet: " + (Packet_Type)_packet[1] + "\r\n");
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
                ((Server)this).log.error("Tcpreceive(), Exception received when reading on the TCP socket, exception:"+_exception+"\r\n");
            }

            if (packetSize  < 4) //maybe add a size/4 Check to be sure the Size is Conform to Scawen standart
            {
                ((Server)this).log.network("TcpReceive(), Droped packet, Too Short!, PacketSize->" + packetSize + "\r\n");
                return;
            }

            Packet_Type packetType = (Packet_Type)tcpSocket.ReadByte();

            ((Server)this).log.network("TcpReceive, PacketSize->" + packetSize + ", PacketType->" + packetType + "\r\n");

            byte[] data = new byte[packetSize-2];
            tcpSocket.Read(data, 0, (packetSize-2));
            AddToTcpReceiveQueud(new Packet(Protocol_Id.PROTO_TCP,(Packet_Size)packetSize, packetType, data));
        }
        private void UdpReceive()
        {
            if (udpClient.Available < 1)
                return;

            byte[] data = new byte[udpClient.Available];
            data = udpClient.Receive(ref udpIpEndPoint);
            
            byte packetSize;
            if ((packetSize = data[0]) < 3)
            {
                ((Server)this).log.network("UdpReceive(), Droped packet, Too Short!, PacketSize->" + packetSize + "\r\n");
                return;
            }
            ((Server)this).log.network("UdpReceive(), PacketSize->" + packetSize + ", PacketType->" + (Packet_Type)data[1] + "\r\n");
            AddToUdpReceiveQueud(new Packet(Protocol_Id.PROTO_UDP, data));
        }
    }

    //This class is non sence, is keeped cause of Time related question
    //She will be removed, and DB setting should goes into the Master DB Object.
    public class InSimSetting
    {
        private string _adminpass = "";
        private string _appname = "";
        private uint _autoreconnectdelay = 0xea60;
        private char _commandprefix = '!';
        private InSim_Flag _flags;
        private ushort _mci_nlp_interval = 500;
        private ushort _portnumber = 0x752f;
        private IPAddress _serverip = IPAddress.Any;
        private ushort _udpreplyport = 0x2716;

        public InSimSetting(string ServerIP, ushort PortNumber, ushort UDPReplyPort, InSim_Flag Flags, char CommandPrefix, ushort MCI_NLP_Interval, string AdminPass, string AppName, uint AutoReconnectDelayInSeconds )
        {
            this._serverip = IPAddress.Parse(ServerIP);
            this._portnumber = PortNumber;
            this._udpreplyport = UDPReplyPort;
            this._flags = Flags;
            this._commandprefix = CommandPrefix;
            this._mci_nlp_interval = MCI_NLP_Interval;
            this._adminpass = AdminPass;
            this._appname = AppName;
            this._autoreconnectdelay = (AutoReconnectDelayInSeconds == 0) ? 0 : (AutoReconnectDelayInSeconds * 0x3e8);
        }
        public string adminPassword
        {
            get
            {
                return this._adminpass;
            }
            set
            {
                this._adminpass = value;
            }
        }

        public string connectionName
        {
            get
            {
                return this._appname;
            }
            set
            {
                this._appname = value;
            }
        }

        public uint AutoReconnectDelay
        {
            get
            {
                return this._autoreconnectdelay;
            }
            set
            {
                this._autoreconnectdelay = value;
            }
        }

        public char CommandPrefix
        {
            get
            {
                return this._commandprefix;
            }
            set
            {
                this._commandprefix = value;
            }
        }

        public InSim_Flag Flags
        {
            get
            {
                return this._flags;
            }
            set
            {
                this._flags = value;
            }
        }

        public ushort MCI_NLP_Interval
        {
            get
            {
                return this._mci_nlp_interval;
            }
            set
            {
                this._mci_nlp_interval = value;
            }
        }

        public ushort tcpPort
        {
            get
            {
                return this._portnumber;
            }
            set
            {
                this._portnumber = value;
            }
        }

        public IPAddress serverIp
        {
            get
            {
                return this._serverip;
            }
            set
            {
                this._serverip = value;
            }
        }

        public ushort udpPort
        {
            get
            {
                return this._udpreplyport;
            }
            set
            {
                this._udpreplyport = value;
            }
        }
    }
}