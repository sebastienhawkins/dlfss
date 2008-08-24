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

using Drive_LFSS.Irc_.Data_;

namespace Drive_LFSS.Irc_
{
    public enum ServerEventType
    {
        Unknown,
        ConnectionStart,
        Join,
        Past,
        ChannelMessage,
        PrivateMessage,
        NicknameChanged
    }

    public class ServerEventData
    {
        public ServerEventData()
        {
        }

        private string _sender;
        private ServerEventType _eventType;
        private string _text;
        private int _rawNumber;
        private string _channel;

        public string Sender
        {
            get { return _sender; }
            set { _sender = value; }
        }

        public ServerEventType EventType
        {
            get { return _eventType; }
            set { _eventType = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public int RawNumber
        {
            get { return _rawNumber; }
            set { _rawNumber = value; }
        }

        public string Channel
        {
            get { return _channel; }
            set { _channel = value; }
        }
    }

    class EventInputHandler
    {
        public static ServerEventData GetServerEventData(string serverInput, IrcServerInfo ircServerInfo)
        {
            ServerEventData sData = new ServerEventData();

            if (string.IsNullOrEmpty(serverInput)) return sData;

            string inputLower = serverInput.ToLower();

            //Enter server message
            if (inputLower.StartsWith(string.Format(":{0} notice auth", ircServerInfo.ServerHostName.ToLower())))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
            }

            //You have not registered
            if (inputLower.StartsWith(string.Format(":{0} 451 ping", ircServerInfo.ServerHostName.ToLower())))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.EventType = ServerEventType.ConnectionStart;
                sData.RawNumber = 451;
            }

            //There are 26 users and 27 invisible on 2 servers
            if (inputLower.StartsWith(string.Format(":{0} 251 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 251;
            }

            //11 :operator(s) online
            if (inputLower.StartsWith(string.Format(":{0} 252 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 252;
            }

            //14 :channels formed
            if (inputLower.StartsWith(string.Format(":{0} 254 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 254;
            }

            //I have 34 clients and 1 servers
            if (inputLower.StartsWith(string.Format(":{0} 255 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 255;
            }

            //Current Local Users: 34  Max: 253
            if (inputLower.StartsWith(string.Format(":{0} 265 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 265;
            }

            //Current Global Users: 53  Max: 270
            if (inputLower.StartsWith(string.Format(":{0} 266 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 266;
            }

            //- irc.mIRCx.co.il Message of the Day - 
            if (inputLower.StartsWith(string.Format(":{0} 375 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 375;
            }

            //- irc.mIRCx.co.il Message of the Day - text
            if (inputLower.StartsWith(string.Format(":{0} 372 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 372;
            }

            //End of /MOTD command.
            if (inputLower.StartsWith(string.Format(":{0} 376 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 376;
            }

            //Channel title on join
            if (inputLower.StartsWith(string.Format(":{0} 332 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 332;
            }

            //Channel title by on join
            if (inputLower.StartsWith(string.Format(":{0} 333 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 333;
            }

            //Channel /NAMES list on join
            if (inputLower.StartsWith(string.Format(":{0} 353 {1}", ircServerInfo.ServerHostName.ToLower(), ircServerInfo.NickName)))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
                sData.RawNumber = 353;
            }

            //Pong returned from server
            if (inputLower.StartsWith(string.Format(":{0} pong", ircServerInfo.ServerHostName.ToLower())))
            {
                sData.Sender = ircServerInfo.ServerHostName.ToLower();
            }
            
            return sData;
        }
    }
}
