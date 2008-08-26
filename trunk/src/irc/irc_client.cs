﻿/* 
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
        }
        ~IrcClient()
        {
            if (true == false) { }
        }
        public void ConfigApply()
        {
            string[] confvalue = Config.GetStringValue("mIRC", "ConnectionInfo").Split(new char[]{';'});
            if (confvalue.Length != 7)
                throw new Exception("mIRC.ConnectionInfo has a Bad Value count\r\n");
            IPAddress  ipAddress;
            try{ipAddress = IPAddress.Parse(confvalue[0]);}
            catch(Exception){ipAddress = Dns.GetHostAddresses(confvalue[0])[0];}

            ircServerInfo = new IrcServerInfo(ipAddress, Convert.ToUInt16(confvalue[1]), confvalue[2], confvalue[3], confvalue[4], confvalue[5], Convert.ToUInt16(confvalue[6]));
        }

        private Socket socket;
        private IrcServerInfo ircServerInfo = null;
        private Thread thrdListener;
        private Thread thrdPinger;
        private bool registrationSend = false;

        public bool IsConnected
        {
            get { return (socket != null && socket.Connected); }
        } 

        #region Connection

        public void Connect()
        {
            IPEndPoint iepLocal = new IPEndPoint(IPAddress.Any, 0);
            IPEndPoint iepRemote = new IPEndPoint(ircServerInfo.ServerHostName, ircServerInfo.Port);
            DoConnect(iepLocal, iepRemote);
        }
        private void DoConnect(IPEndPoint iepLocal, IPEndPoint iepRemote)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Bind(iepLocal);
                socket.Connect(iepRemote);
            }
            catch (Exception){}

            //Start listening to incoming sockect data from server
            thrdListener = new Thread(new ThreadStart(StartSocketListening));
            thrdListener.IsBackground = true;
            thrdListener.Start();

            //Start pinging the server to keep connection alive
            thrdPinger = new Thread(new ThreadStart(RunPinging));
            thrdPinger.Start();
        }
        private void SendRegistration()
        {
            SendServerData("NICK " + ircServerInfo.NickName);
            SendServerData("USER " + ircServerInfo.NickName + " *!* * :Drive Live For Speed Server");
            registrationSend = true;
        }
        private void CompleteRegistration(string data)
        {
            SendServerData("PONG " + data);

            SendServerData("JOIN #test"); // for debug purpose will need to be at is right place.
            registrationSend = false;
        }
        public void Disconnect()
        {
            if (socket.Connected)
            {
                SendServerData("QUIT");
                if (socket != null && !socket.Connected)
                    socket.Close();
            }

            thrdListener.Join();
            thrdPinger.Join();
        }

        #endregion

        #region Thread
        //heu??? a thread for a ping... is that so important, lol... let get rid of this.
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
                        ProcessData(input);
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

        private void ProcessData(string rawLine)
        {
            string[] rawLines = rawLine.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            string rawData = "";

            IrcMessage ircMessage;

            for (byte lineCount = 0; lineCount < rawLines.Length; lineCount++)
            {
                rawData = rawLines[lineCount];
                ircMessage = new IrcMessage();

                //:from
                string[] rawDatas = rawData.Split(new char[] { ' ' }, 2);
                if (rawDatas[0][0] == ':')
                {
                    ircMessage.from = rawDatas[0].Substring(1); //Rip off the ':'
                    rawDatas = rawDatas[1].Split(new char[] { ' ' }, 2);
                }
                //Command or Reply Code
                try
                {
                    ircMessage.replyCode = (Irc_Reply_Code)Convert.ToUInt16(rawDatas[0]);
                    ircMessage.commands[0] = Irc_Command.REPLY_CODE;

                    rawDatas = rawDatas[1].Split(new char[] { ' ' }, 2);
                }
                catch (Exception)
                { ircMessage.commands[0] = ToCommandType(rawDatas[0]); }


                if (ircMessage.commands[0] == Irc_Command.NONE ||
                    ircMessage.commands[0] == Irc_Command.UNKNOW)
                {
                    Log.missingDefinition("Mirc Command is '" + rawDatas[0] + "', Detected as:" + ircMessage.commands[0] + "\r\n");
                    continue;
                }
                switch (ircMessage.commands[0])
                {
                    case Irc_Command.PONG:
                    {
                        rawDatas = rawDatas[1].Split(new char[] { ':' }, 2);
                        ircMessage.to = rawDatas[0];
                        ircMessage.data = rawDatas[1];
                    } break;
                    case Irc_Command.PING:
                    {
                        ircMessage.data = rawDatas[1];
                        if (registrationSend)
                            CompleteRegistration(ircMessage.data);
                    } break;
                    case Irc_Command.PRIVMSG:
                    {
                        rawDatas = rawDatas[1].Split(new char[] { ':' });
                        ircMessage.to = rawDatas[0].TrimEnd();
                        ircMessage.data = rawDatas[1];
                        if (ircMessage.to[0] == '#')
                            ReceiveFromChannel(ircMessage.data, ircMessage.from.Split(new char[] { '!' })[0], ircMessage.to);
                        else
                            ReceiveFromNick(ircMessage.data, ircMessage.from.Split(new char[]{'!'})[0]);

                    } break;
                    case Irc_Command.REPLY_CODE:
                    {
                        switch (ircMessage.replyCode)
                        {
                            case Irc_Reply_Code.NOT_REGISTERED:
                            {
                                SendRegistration();
                            } break;
                        }
                    } break;
                    default:
                    {
                        Log.missingDefinition("Mirc Missing Definition for Command: " + ircMessage.commands[0] + "\r\n");
                        ircMessage.data = String.Concat(rawDatas);
                    } break;
                }

                //Log.debug("From: " + ircMessage.from + ", To:" + ircMessage.to + ", Command[0]:" + ircMessage.commands[0] + ", Command[1]:" + ircMessage.commands[1] + ", Command[2]:" + ircMessage.commands[2] + ", ReplyCode:" + ircMessage.replyCode + ", Data:" + ircMessage.data + "\r\n");
            }
        }
        private void SendServerData(string data)
        {
            if (socket == null || !socket.Connected) return;

            byte[] buffer = null;
            buffer = Encoding.ASCII.GetBytes(data + "\r\n");

            socket.Send(buffer);

            //Log.debug("Send Data:"+data+"\r\n");
        }
        
        
        public void SendToChannel(string message)
        {
            SendServerData("PRIVMSG #"+ircServerInfo.Channel+" :"+message);
        }
        public void ReceiveFromChannel(string message, string from ,string forChannel)
        {
            Log.chat("[IRC!" + from + forChannel + "> " + message + "\r\n");
            Drive_LFSS.CommandConsole_.CommandConsole.Exec("say all [IRC!" + from + forChannel + "> " + message);
        }
        public void ReceiveFromNick(string message, string from)
        {
            Log.chat("[IRC!" + from + "> " + message + "\r\n");
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        private Irc_Command ToCommandType(string _command)
        {
            if (ircCommandMap.ContainsKey(_command))
                return ircCommandMap[_command];
            else
                return Irc_Command.UNKNOW;
        }
        private static Dictionary<string,Irc_Command> ircCommandMap = new Dictionary<string,Irc_Command>()
        {
            {"NOTICE", Irc_Command.NOTICE},
            {"AUTH", Irc_Command.AUTH},
            {"PING", Irc_Command.PING},
            {"PONG", Irc_Command.PONG},
            {"INFO", Irc_Command.INFO},
            {"PRIVMSG", Irc_Command.PRIVMSG},
            {"JOIN", Irc_Command.JOIN},
            {"MODE", Irc_Command.MODE},
        };
        private class IrcMessage
        {
            public IrcMessage()
            {
            }
            ~IrcMessage()
            {
                if (true == false) { }
            }
            public string from = "";
            public string to = "";
            public string data = "";
            public Irc_Command[] commands = new Irc_Command[] { Irc_Command.NONE, Irc_Command.NONE, Irc_Command.NONE };
            public Irc_Reply_Code replyCode = Irc_Reply_Code.NONE;
        }
    }
}
namespace Drive_LFSS.Irc_.Data_
{
    public class IrcServerInfo
    {
        private IPAddress _serverHostName;
        private int _port;
        private string _nickName;
        private string _fullName;
        private string _emailAddress;
        private string _channel;
        private ushort _configFlag;

        public IPAddress ServerHostName
        {
            get { return _serverHostName; }
        }

        public int Port
        {
            get { return _port; }
        }

        public string NickName
        {
            get { return _nickName; }
            set { _nickName = value; }
        }

        public string FullName
        {
            get { return _fullName; }
        }

        public string EmailAddress
        {
            get { return _emailAddress; }
        }

        public string Channel
        {
            get { return _channel; }
        }
        public ushort ConfigFlag
        {
            get { return _configFlag; }
        }
        public IrcServerInfo(IPAddress serverHostName, ushort port, string nickName, string fullName, string emailAddress, string channel, ushort configFlag)
        {
            this._serverHostName = serverHostName;
            this._port = port;
            this._nickName = nickName;
            this._fullName = fullName;
            this._emailAddress = emailAddress;
            this._channel = channel;
            this._configFlag = configFlag;
        }
    }
}