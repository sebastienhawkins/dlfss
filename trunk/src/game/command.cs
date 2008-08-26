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
namespace Drive_LFSS.Server_
{
    using Drive_LFSS.Log_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Game_;

    sealed class CommandInGame
    {
        public CommandInGame(string _serverName)
        {
            serverName = _serverName;

            //       CommandName                CommandLevel                    CommandReference
            command["exit"] = new CommandName(0, new CommandDelegate(Exit));
            command["kick"] = new CommandName(0, new CommandDelegate(Kick));
        }
        ~CommandInGame()
        {
            if (true == false) { }
        }
        private string serverName;
        private struct CommandName
        {
            public CommandName(byte _level, CommandDelegate _cmd)
            {
                level = _level;
                cmd = _cmd;
            }
            public byte level;
            public CommandDelegate cmd;
        }

        private delegate void CommandDelegate(bool _adminStatus, string _licenceName, string _commandText);
        
        private Dictionary<string,CommandName> command = new Dictionary<string,CommandName> ();

        public string GetServerName()
        {
            return serverName;
        }
        public void Exec(bool _adminStatus, string _licenceName, string _commandText)
        {
            string[] args = _commandText.Split(' ');                 //Can Be little faster... since we need only left to first white space
            args[0] = args[0].Substring(1);                         //Remove "Prefix Command String".

            if (args.Length < 1 || !command.ContainsKey(args[0].ToLower()) || (command[args[0]].level > 0 && !_adminStatus))
            {
                Log.debug("Command.Exec(), Bad Command Call From User: " + _licenceName + ", AccessLevel: " + (_adminStatus ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
                return;
            }
            command[args[0]].cmd(_adminStatus,_licenceName, _commandText);
        }
        #region Commands
        private void Exit(bool _adminStatus, string _licenceName, string _commandText)
        {
            Log.normal("Exiting Requested, Please Wait For All Thread Too Exit...\n\r");
            Program.Exit();
        }
        private void Kick(bool _adminStatus, string _licenceName, string _commandText)
        {

            string[] args = _commandText.Split(' ');
            if (args.Length != 2)
            {
                //Session.server[serverId].Send_MTC_MessageToConnection("bad parameters Count, Usage: !kick username", licence.GetConnectionUniqueId(), 0);
                return;
            }
            args[0] = args[0].Substring(1);                         //Remove "Prefix Command String".

            Log.command("Command.Kick(), User: " + _licenceName + ", Kicked User: " + args[1] + "\r\n");
            SessionList.sessionList[serverName].AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST("/kick " + args[1])));
        }
        #endregion
    }
}
