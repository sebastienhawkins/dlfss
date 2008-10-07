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
    using Log_;
    using Packet_;
    using Game_;
    using Definition_;

    sealed class CommandInGame
    {
        public CommandInGame(Session _session)
        {
            session = _session;
            //       CommandName                CommandLevel                    CommandReference
           command["exit"] = new CommandName(1, new CommandDelegate(Exit));
           command["kick"] = new CommandName(1, new CommandDelegate(Kick));
           command["reload"] = new CommandName(1, new CommandDelegate(Reload));
           command["help"] = new CommandName(0, new CommandDelegate(Help));
           command["config"] = new CommandName(0, new CommandDelegate(Config));
           command["status"] = new CommandName(0, new CommandDelegate(Status));
           command["rank"] = new CommandName(0, new CommandDelegate(Rank));
           command["top"] = new CommandName(0, new CommandDelegate(Rank));
           command["top10"] = new CommandName(0, new CommandDelegate(Rank));
           command["top20"] = new CommandName(0, new CommandDelegate(Rank));
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
        private readonly Session session;

        public void Exec(Driver driver, string _commandText)
        {
            string[] args = _commandText.Split(' ');                 //Can Be little faster... since we need only left to first white space
            args[0] = args[0].Substring(1);                          //Remove "Prefix Command String".
            args[0] = args[0].ToLowerInvariant();
            
            if (args.Length < 1 || !command.ContainsKey(args[0]) ) 
            {
                driver.AddMessageMiddle("^7Unknown command: ^3" + _commandText + ".", 4500);
                Log.command("Command.Exec(), Invalid command from User: " + driver.LicenceName + ", AccessLevel: " + (driver.IsAdmin ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
                return;
            }
            if( command[args[0]].level > 0 && !driver.IsAdmin )
            {
                driver.AddMessageMiddle("^7Must be admin to execute command: ^3" + args[0]+".",4500);
                Log.command("Command.Exec(), Illegal command from User: " + driver.LicenceName + ", AccessLevel: " + (driver.IsAdmin ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
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
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!kick ^3username",4500);
                return;
            }
            driver.AddMessageMiddle("^7You kicked username: ^3" + args[1]+".",4500);
            Log.command("Command.Kick(), User: " + driver.LicenceName + ", Kicked User: " + args[1] + "\r\n");
            driver.ISession.SendMSTMessage("/kick " + args[1]);
        }
        private void Reload(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!reload ^3tableName", 4500);
                return;
            }
            switch (args[1])
            {
                case "all":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("all"); }
                        driver.AddMessageMiddle("^7Completed reloading, ^3everything", 4500);
                    } break;
                case "track":
                case "track_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("track_template"); }
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], 4500);
                    } break;
                case "car":
                case "car_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("car_template"); }
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], 4500);
                    } break;
                case "button":
                case "button_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("button_template"); }
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], 4500);
                    } break;
                case "race":
                case "race_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("race_template"); }
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], 4500);
                    } break;
                case "ban":
                case "driver_ban":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("driver_ban"); }
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], 4500);
                    } break;
                case "gui":
                case "gui_template":
                    {
                        lock (Program.dlfssDatabase) { Program.Reload("gui_template"); }
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], 4500);
                    } break;
                case "config":
                    {
                        Program.Reload("config");
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], 4500);
                    } break;
                default:
                    {
                        driver.AddMessageMiddle("^7Unknown tableName, ^3" + args[1], 4500);
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
        private void Status(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!status ^3serverName^7/^3all^7/^3current", 7000);
                return;
            }

            if (args[1] == "all")
            {
                //Maybe you Real Iterator<Session>
                Dictionary<string, Session>.Enumerator itr = SessionList.Sessions.GetEnumerator();
                string textToSend = "Status for all Server\r\n";
                while (itr.MoveNext())
                {
                    if (itr.Current.Value.IsConnected())
                    {
                        string reactionTime;
                        long _rt = itr.Current.Value.GetReactionTime() ;
                        if (_rt == 0)
                            reactionTime = "^7" + _rt.ToString();
                        else if (_rt > 0)
                            reactionTime = "^4" + _rt.ToString();
                        else
                            reactionTime = "^3" + _rt.ToString();

                        textToSend+="^8ServerName ^7" + itr.Current.Key + ", ^8Status ^2online^8, ReactionTime ^7" + reactionTime + "^8ms" + ", DriversCount ^7" + itr.Current.Value.GetNbrOfDrivers()+"\r\n";
                    }
                    else
                        textToSend+="^8ServerName ^7" + itr.Current.Key + ", ^8Status ^1offline^8, ReactionTime ^7na^8ms, DriversCount ^7na\r\n";
                }

                driver.SendGui((ushort)Gui_Entry.TEXT, textToSend);
            }
            else
            {
                string serverName = args[1];
                if(serverName == "current")
                    serverName = session.GetSessionName();

                if (SessionList.Sessions.ContainsKey(serverName))
                {
                    Session _session = SessionList.Sessions[serverName];

                    if (_session.IsConnected())
                    {
                        string reactionTime;
                        long _rt = _session.GetReactionTime();
                        if (_rt == 0)
                            reactionTime = "^7" + _rt.ToString();
                        else if (_rt > 0)
                            reactionTime = "^4" + _rt.ToString();
                        else
                            reactionTime = "^3" + _rt.ToString();

                        driver.AddMessageMiddle("^8ServerName ^7" + serverName + ", ^8Status ^2online^8, ReactionTime ^7" + reactionTime + "^8ms" + ", DriversCount ^7" + _session.GetNbrOfDrivers(),4500);
                    }
                    else
                        driver.AddMessageMiddle("^8ServerName ^7" + serverName + ", ^8Status ^1offline^8, ReactionTime ^7na^8ms, DriversCount ^7na",4500);
                }
                else
                    driver.AddMessageMiddle("^8Status - serverName(^7"+serverName+")^8 not found.",4500);
            }
        }
        private void Rank(Driver driver, string[] args)
        {
            driver.SendRankGui(Button_Entry.NONE);
        }
        #endregion
    }
}
