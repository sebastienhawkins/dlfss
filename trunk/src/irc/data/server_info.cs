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

namespace Drive_LFSS.Irc_.Data_
{
    public class IrcServerInfo
    {
        private string _serverHostName;
        private int _port;
        private string _nickName;
        private string _fullName;
        private string _emailAddress;
        private string _channel;
        private ushort _configFlag;

        public string ServerHostName
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
        public IrcServerInfo(string serverHostName, ushort port, string nickName, string fullName, string emailAddress, string channel, ushort configFlag)
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
