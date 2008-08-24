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

namespace Drive_LFSS.Irc_
{
    public delegate void ConnectHandler(string userName);
    public delegate void DisconnectHandler(string userName);
    public delegate void ServerTextHandler(string serverText);
    public delegate void PartHandler(string userName, string channel);
    public delegate void JoinHandler(string userName, string channel);
    public delegate void NicknameChangeHandler(string userName, string channel);
    public delegate void ChannelMessageHandler(string userName, string channel);
    public delegate void PrivateMessageHandler(string userName, string channel);
    public delegate void PingHandler(string ping);
    public delegate void DataSendHandler(string data);
}
