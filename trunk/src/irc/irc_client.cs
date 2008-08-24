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
 * 
 * This File Original Author: Gilad Lavian
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


namespace Drive_LFSS.Irc_
{
    using Drive_LFSS.Irc_.Data_;
    using Drive_LFSS.Config_;
    using Drive_LFSS.Log_;

    public enum ServerType
    {
        Regular,
        Special
    }
    public sealed class IrcClient
    {
        public IrcClient()
        {
            this.eOnConnect += new ConnectHandler(IrcClient_OnConnect);
            this.eOnServerText += new ServerTextHandler(IrcClient_OnServerText);
        }
        public void ConfigApply()
        {
            string[] confvalue = Config.GetStringValue("mIRC", "ConnectionInfo").Split(new char[]{';'});
            if (confvalue.Length != 7)
                throw new Exception("mIRC.ConnectionInfo has a Bad Value count\r\n");

            ircServerInfo = new IrcServerInfo(confvalue[0],Convert.ToUInt16(confvalue[1]),confvalue[2],confvalue[3],confvalue[4],confvalue[5],Convert.ToUInt16(confvalue[6]));
        }

        protected Socket socket;
        protected IrcServerInfo ircServerInfo = null;
        protected Thread thrdListener;
        protected Thread thrdPinger;

        public bool IsConnected
        {
            get { return (socket != null && socket.Connected); }
        } 

        #region Connection

        public void Connect()
        {
            IPEndPoint iepLocal = CreateEndPoint(IPAddress.Any, 0);
            IPEndPoint iepRemote = CreateEndPoint(ircServerInfo.ServerHostName, ircServerInfo.Port);
            DoConnect(iepLocal, iepRemote);
        }
        private void DoConnect(IPEndPoint iepLocal, IPEndPoint iepRemote)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(iepLocal);
                socket.Connect(iepRemote);

                if (eOnConnect != null)
                    eOnConnect(ircServerInfo.NickName);
            }
            catch (Exception)
            {
                if (eOnConnectFailed != null)
                    eOnConnectFailed(ircServerInfo.NickName);
            }
        }
        
        //This function suck and is incomplete.
        private void InitializeServerNegotiation()
        {
            SendServerData("NICK "+ircServerInfo.NickName); //Rght after this, server should send us a PING :0F0F0F0F id , important we return with a PONG
            SendServerData("USER "+ircServerInfo.NickName+" *!* * :Drive Live For Speed Server");
            //SendServerData(string.Format(ConstCommands.JOIN, ircServerInfo.Channel));
        }
        private void IrcClient_OnConnect(string userName)
        {
            //Start listening to incoming sockect data from server
            thrdListener = new Thread(new ThreadStart(StartSocketListening));
            thrdListener.IsBackground = true;
            thrdListener.Start();

            //Start pinging the server to keep connection alive
            thrdPinger = new Thread(new ThreadStart(RunPinging));
            thrdPinger.Start();
        }
        private void IrcClient_OnServerText(string serverText)
        {
            ServerEventData eventData = EventInputHandler.GetServerEventData(serverText, ircServerInfo);

            switch (eventData.EventType)
            {
                case ServerEventType.ConnectionStart:
                    InitializeServerNegotiation();
                    break;
                case ServerEventType.Unknown:
                    break;
                case ServerEventType.Join:
                    break;
                case ServerEventType.Past:
                    break;
                case ServerEventType.ChannelMessage:
                    break;
                case ServerEventType.PrivateMessage:
                    break;
                case ServerEventType.NicknameChanged:
                    break;
                default:
                    break;
            }
        }
        public void Disconnect()
        {
            if (socket.Connected)
            {
                //Quite session within server
                SendServerData("QUIT");

                //Kill all proccess and abort threads
                thrdListener.Join();
                thrdPinger.Join();

                if (socket != null && !socket.Connected)
                    socket.Close();
            }

            if (eOnDisconnect != null)
                eOnDisconnect(ircServerInfo.NickName);
        }

        #endregion

        #region Thread
        private void RunPinging()
        {
            uint pingTimer = 8000;
            while (socket != null && socket.Connected && Program.MainRun)
            {
                if ((pingTimer += 100) > 10000)
                {
                    SendServerData(string.Format("PING : {0}", ircServerInfo.ServerHostName));
                    pingTimer = 0;
                }
                Thread.Sleep(100);
            }
        }
        private void StartSocketListening()
        {

            if (socket == null || !socket.Connected) 
                return;

            string input = null;

            if (socket.Poll(-1, SelectMode.SelectRead))
            {
                while (socket.Connected && Program.MainRun)
                {
                    while (!string.IsNullOrEmpty((input = GetRecievedServerData(socket))))
                    {
                        if (eOnServerText != null)
                            eOnServerText(input);

                        input = null;
                    }
                    Thread.Sleep(50);
                }
            }

        }
        private string GetRecievedServerData(Socket socket)
        {
            if (socket == null || !socket.Connected) 
                return string.Empty;

            string data = string.Empty;

            byte[] buffer = new byte[socket.Available];
            int bytesRead = socket.Receive(buffer);

            data = Encoding.Default.GetString(buffer, 0, bytesRead);

            return data;
        }
        #endregion

        public void SendServerData(string data)
        {
            if (socket == null || !socket.Connected) return;

            byte[] buffer = null;

            //If its server command
            /*if (data.StartsWith("/"))
            {
                string[] clientInput = data.Split(' ');

                switch (clientInput[0].ToLower())
                {
                    case "/join":
                        buffer = Encoding.ASCII.GetBytes(string.Format(ConstCommands.JOIN, clientInput[1]) + Environment.NewLine);
                        break;

                    case "/part":
                        buffer = Encoding.ASCII.GetBytes(string.Format(ConstCommands.PART, clientInput[1]) + Environment.NewLine);
                        break;

                    case "/nick":
                        buffer = Encoding.ASCII.GetBytes(string.Format(ConstCommands.NICK, clientInput[1]) + Environment.NewLine);
                        break;

                    default:
                        return;
                }
            }
            else*/
            {
                buffer = Encoding.ASCII.GetBytes(data + "\r\n");
            }

            socket.Send(buffer);

            if (eOnDataSend != null)
                eOnDataSend(data);
        }

        public void OnDisconnect(string userName)
        {

        }
        public void OnDataSend(string data)
        {

        }
        public void OnConnect(string userName)
        {
            Log.chat("Irc Client Connected with username: "+userName+"\r\n");
        }
        public void OnServerText(string serverText)
        {
            //This is the missing part for a sucess user Registration on IRC server
            if (serverText.StartsWith("PING :"))
            {
                SendServerData("PONG " + serverText.Split(new char[]{':'})[1]);
                SendServerData("JOIN #"+ircServerInfo.Channel);
            }
            Log.chat(serverText + "\r\n");
        }

        #region IRC Event Handlers
        public event ConnectHandler eOnConnect;
        public event ConnectHandler eOnConnectFailed;
        public event DisconnectHandler eOnDisconnect;
        public event JoinHandler eOnJoin;
        public event PartHandler eOnPart;
        public event ChannelMessageHandler eOnChannelMessage;
        public event PrivateMessageHandler eOnPrivateMessage;
        public event ServerTextHandler eOnServerText;
        public event NicknameChangeHandler eOnNicknameChanged;
        public event PingHandler eOnPing;
        public event DataSendHandler eOnDataSend;
        #endregion

        #region Create IP EndPoint
        IPEndPoint CreateEndPoint(IPAddress ipAddress, int port)
        {
            return new IPEndPoint(ipAddress, 0);
        }
        IPEndPoint CreateEndPoint(string serverHostName, int port)
        {
            long ipAddress;
            long.TryParse(Dns.Resolve(serverHostName).AddressList[0].Address.ToString(), out ipAddress);
            return new IPEndPoint(ipAddress, port);
        }
        #endregion
    }
}
