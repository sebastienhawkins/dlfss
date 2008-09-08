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
        public CommandInGame()
        {
            //       CommandName                CommandLevel                    CommandReference
           command["exit"] = new CommandName(1, new CommandDelegate(Exit));
           command["kick"] = new CommandName(1, new CommandDelegate(Kick));
        }
        ~CommandInGame()
        {
            if (true == false) { }
        }
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
        private delegate void CommandDelegate(Driver driver, string[] args);
        private Dictionary<string, CommandName> command = new Dictionary<string, CommandName>();

        public void Exec(Driver driver, string _commandText)
        {
            string[] args = _commandText.Split(' ');                 //Can Be little faster... since we need only left to first white space
            args[0] = args[0].Substring(1);                         //Remove "Prefix Command String".

            if (args.Length < 1 || !command.ContainsKey(args[0].ToLowerInvariant()) ) 
            {
                driver.AddMessageMiddle("^7 Bad Command Call.", 7000);
                Log.command("Command.Exec(), Bad Command Call From User: " + driver.LicenceName + ", AccessLevel: " + (driver.AdminFlag ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
                return;
            }
            if( command[args[0]].level > 0 && !driver.AdminFlag )
            {
                driver.AddMessageMiddle("^7 Must be Admin to excute command: ^7" + args[0]+"^1.",7000);
                Log.command("Command.Exec(), Bad Command Call From User: " + driver.LicenceName + ", AccessLevel: " + (driver.AdminFlag ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
                return;
            }

            command[args[0]].cmd(driver, args);
        }
        #region Commands
        private void Exit(Driver driver, string[] args)
        {
            Program.Exit();
        }
        private void Kick(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7 Bad parameters Count^7, Usage: ^2!kick ^3username",7000);
                return;
            }
            driver.AddMessageMiddle("^7 You Kicked username:" + args[1]+"^7.",7000);
            Log.command("Command.Kick(), User: " + driver.LicenceName + ", Kicked User: " + args[1] + "\r\n");
            driver.ISession.SendMSTMessage("/kick " + args[1]);
        }
        #endregion
    }
}
