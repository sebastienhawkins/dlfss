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
           command["reload"] = new CommandName(1, new CommandDelegate(Reload));
           command["help"] = new CommandName(0, new CommandDelegate(Help));
           command["config"] = new CommandName(0, new CommandDelegate(Config));
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
                driver.AddMessageMiddle("^7 Unknown command: ^3" + _commandText + ".", 4500);
                Log.command("Command.Exec(), Bad Command Call From User: " + driver.LicenceName + ", AccessLevel: " + (driver.IsAdmin ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
                return;
            }
            if( command[args[0]].level > 0 && !driver.IsAdmin )
            {
                driver.AddMessageMiddle("^7 Must be Admin to execute command: ^3" + args[0]+".",4500);
                Log.command("Command.Exec(), Bad Command Call From User: " + driver.LicenceName + ", AccessLevel: " + (driver.IsAdmin ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
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
                driver.AddMessageMiddle("^7 Bad parameters Count^7, Usage: ^2!kick ^3username",4500);
                return;
            }
            driver.AddMessageMiddle("^7 You Kicked username: ^3" + args[1]+".",4500);
            Log.command("Command.Kick(), User: " + driver.LicenceName + ", Kicked User: " + args[1] + "\r\n");
            driver.ISession.SendMSTMessage("/kick " + args[1]);
        }
        private void Reload(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7 Bad parameters Count^7, Usage: ^2!reload ^3tableName", 4500);
                return;
            }
            switch (args[1])
            {
                case "all":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("all"); }
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 everything", 4500);
                    } break;
                case "track":
                case "track_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("track_template"); }
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 " + args[1], 4500);
                    } break;
                case "car":
                case "car_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("car_template"); }
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 " + args[1], 4500);
                    } break;
                case "button":
                case "button_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("button_template"); }
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 " + args[1], 4500);
                    } break;
                case "race":
                case "race_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("race_template"); }
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 " + args[1], 4500);
                    } break;
                case "ban":
                case "driver_ban":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("driver_ban"); }
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 " + args[1], 4500);
                    } break;
                case "gui":
                case "gui_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("gui_template"); }
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 " + args[1], 4500);
                    } break;
                case "config":
                    {
                        Program.Reload("config");
                        driver.AddMessageMiddle("^7 Completed Reloading, ^3 " + args[1], 4500);
                    } break;
                default:
                    {
                        driver.AddMessageMiddle("^7 Unknown tableName, ^3 " + args[1], 4500);
                    } break;

                    Log.command("Command.Reload(), User: " + driver.LicenceName + ", reloaded: " + args[1] + "\r\n");
            }
        }
        private void Help(Driver driver, string[] args)
        {
            driver.SendHelpGui();
            //driver.AddMessageMiddle("^3Sorry, the help system is still TODO!, try !config",8000);
        }
        private void Config(Driver driver, string[] args)
        {
            driver.SendConfigGui();
        }
        #endregion
    }
}
