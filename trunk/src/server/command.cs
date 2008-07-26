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
namespace Drive_LFSS.Server_
{
    using System;
    using System.Collections.Generic;
    //using System.Text;
    using Drive_LFSS.Game_;
    using Drive_LFSS.Session_;

    sealed class CommandServer
    {
        public CommandServer(ushort _serverId)
        {
            serverId = _serverId;

            //       CommandName                CommandLevel                    CommandReference
            command["exit"] = new CommandName(0, new CommandDelegate(Exit));
            command["kick"] = new CommandName(0, new CommandDelegate(Kick));
        }
        
        private ushort serverId;
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

        private delegate void CommandDelegate(bool _adminStatus, string _driverName, string _commandText);
        
        private Dictionary<string,CommandName> command = new Dictionary<string,CommandName> ();

        public ushort GetServerId()
        {
            return serverId;
        }
        public void Exec(bool _adminStatus, string _driverName, string _commandText)
        {
           /* string[] args = commandText.Split(' ');                 //Can Be little faster... since we need only left to first white space
            args[0] = args[0].Substring(1);                         //Remove "Prefix Command String".

            if (args.Length < 1 || !command.ContainsKey(args[0].ToLower()) || (command[args[0]].level > 0 && !licence.admin()))
            {
                SessionList.serverList[serverId].server.log.debug("Command.Exec(), Bad Command Call From User: " + licence.GetConnectionPlayerName() + ", AccessLevel: " + (licence.IsAdmin()?"1":"0") + ", CommandSend: " + commandText + "\r\n");
                return;
            }
            command[args[0]].cmd(licence, commandText);*/
        }
        #region Commands
        private void Exit(bool _adminStatus, string _driverName, string _commandText)
        {
            //Program.log.normal("Exiting Requested, Please Wait For All Thread Too Exit...\n\r");
            Program.Exit();
        }
        private void Kick(bool _adminStatus, string _driverName, string _commandText)
        {

            string[] args = _commandText.Split(' ');
            if (args.Length != 2)
            {
                //Session.server[serverId].Send_MTC_MessageToConnection("bad parameters Count, Usage: !kick username", licence.GetConnectionUniqueId(), 0);
                return;
            }
            args[0] = args[0].Substring(1);                         //Remove "Prefix Command String".

            //Program.log.command("Command.Kick(), User: " + client.pName + ", Kicked User: " + args[1] + "\r\n");

            //Session.server[serverId].Send_MST_Message("/kick " + args[1]);
        }
        #endregion
    }
}
