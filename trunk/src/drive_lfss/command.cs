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

namespace Drive_LFSS.CommandConsole_
{
    using Drive_LFSS.Log_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Game_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Session_;

    static class CommandConsole
    {
        public static void Exec(string _commandText)
        {
            _commandText = _commandText.TrimStart(new char[] { ' ' });
            if (_commandText == "")
                return;

            string[] args = _commandText.Split(new char[]{' '});

            switch (args[0])
            {
                case "reload": Reload(args); break;
                case "status": Status(args); break;
                case "say": Say(args); break;
                case "exit": Exit(); break;
                default:
                {
                    Log.error("Unknow Command: " + _commandText + "\r\n");
                    break;
                }
            }
        }
        private static void Status(string[] args)
        {
            if (args.Length != 2)
            {
                Log.normal("Command Status, Syntax Error.\r\n  Usage:\r\n    status #serverName\r\n      #serverName can be \"all\".\r\n");
                return;
            }

            if (args[1] == "all")
            {
                //Maybe you Real Iterator<Session>
                Dictionary<string,Session>.Enumerator itr = SessionList.sessionList.GetEnumerator();
                while(itr.MoveNext())
                {
                    if (itr.Current.Value.IsConnected())
                        Log.normal("ServerName: " + itr.Current.Key + ", Status: online, ReactionTime: " + itr.Current.Value.GetReactionTime() + "ms" + ", DriversCount: " + itr.Current.Value.GetNbrOfDrivers() + "\r\n");
                    else
                        Log.error("ServerName: " + itr.Current.Key + ", Status: offline, ReactionTime+/-: -ms, DriversCount: -\r\n");
                }
            }
            else
            {
                string serverName = args[1];

                if (SessionList.sessionList.ContainsKey(serverName))
                {
                    Session session = SessionList.sessionList[serverName];

                    if (session.IsConnected())
                        Log.normal("serverName: " + serverName + ", Status: online, ReactionTime: " + session.GetLatency() + "ms" + ", DriversCount: " + session.GetNbrOfDrivers() + "\r\n");
                    else
                        Log.error("serverName: " + serverName + ", Status: offline, ReactionTime: -ms, DriversCount: -\r\n");
                }
                else
                    Log.command("Command Status, ServerName Not Found, Server Requested was: " + serverName + "\r\n");
            }
        }
        private static void Say(string[] args)
        {
            if (args.Length < 3)
            {
                Log.commandHelp("Command Say, Syntax Error.\r\n  Usage:\r\n    say #serverName $Message\r\n      #serverName can be \"all\".\r\n");
                return;
            }

            string message = String.Join(" ", args, 2, args.Length-2);

            if (args[1] == "all")
            {
               Dictionary<string, Session>.Enumerator itr = SessionList.sessionList.GetEnumerator();
               while(itr.MoveNext())
                   itr.Current.Value.AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(message)));
            }
            else if (args[1] == "irc")
            {
                Program.ircClient.SendToChannel(message);
            }
            else
            {
                string serverName = args[1];

                if (SessionList.sessionList.ContainsKey(serverName))
                    SessionList.sessionList[serverName].AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(message)));
                else
                    Log.command("Command Announce, serverName Not Found, Server Requested was: " + args[1] + "\r\n");
            }
        }
        private static void Reload(string[] args)
        {
            if (args.Length < 2)
            {
                Log.commandHelp("Command Reload, Syntax Error.\r\n  Usage:\r\n    reload #table_name\r\n      #table_name can be \"all\" or \"config\".\r\n");
                return;
            }
            Log.command("Start Reloading of "+args[1]+".\r\n");
            switch (args[1])
            {
                case "all": 
                {
                    Program.trackTemplate.Load(true);
                    Log.commandHelp("  track_template reloaded.\r\n");
                    Program.carTemplate.Load(true);
                    Log.commandHelp("  car_template reloaded.\r\n");
                    Program.buttonTemplate.Load(true);
                    Log.commandHelp("  button_template reloaded.\r\n");
                    Program.guiTemplate.Load(true);
                    Log.commandHelp("  gui_template reloaded.\r\n");
                    Program.raceTemplate.Load(false);
                    Log.commandHelp("  race_template reloaded.\r\n");
                    Program.driverBan.Load(false);
                    Log.commandHelp("  driver_ban reloaded.\r\n");

                    Log.normal("Initializating DLFSS Client...\r\n");
                    Program.ConfigApply();
                    Log.normal("Completed Initialize DLFSS Client.\r\n\r\n");

                    Log.normal("Initializating mIRC Client...\r\n");
                    Program.ircClient.ConfigApply();
                    Log.normal("Completed Initialize mIRC Client.\r\n\r\n");

                    Log.normal("Initializating PubStats...\r\n");
                    Program.pubStats.ConfigApply();
                    Log.normal("Completed Initialize PubStats.\r\n\r\n");

                    Log.normal("Initializating Servers Config...\r\n\r\n");
                    SessionList.ConfigApply();
                    Log.normal("Complete Servers Config.\r\n\r\n");
                } break;
                case "track":
                case "track_template":
                {
                    Program.trackTemplate.Load(true);
                    Log.commandHelp("track_template reloaded.\r\n");
                } break;
                case "car":
                case "car_template":
                {
                    Program.carTemplate.Load(true);
                    Log.commandHelp("car_template reloaded.\r\n");
                } break;
                case "button":
                case "button_template":
                {
                    Program.buttonTemplate.Load(true);
                    Log.commandHelp("button_template reloaded.\r\n");
                } break;
                case "race":
                case "race_template":
                {
                    Program.raceTemplate.Load(false);
                    Log.commandHelp("race_template reloaded.\r\n");
                } break;
                case "ban":
                case "driver_ban":
                {
                    Program.driverBan.Load(false);
                    Log.commandHelp("driver_ban reloaded.\r\n");
                } break;
                case "gui":
                case "gui_template":
                {
                    Program.guiTemplate.Load(true);
                    Log.commandHelp("gui_template reloaded.\r\n");
                } break;
                case "config":
                {
                    Log.normal("Initializating DLFSS Client...\r\n");
                    Program.ConfigApply();
                    Log.normal("Completed Initialize DLFSS Client.\r\n\r\n");

                    Log.normal("Initializating mIRC Client...\r\n");
                    Program.ircClient.ConfigApply();
                    Log.normal("Completed Initialize mIRC Client.\r\n\r\n");

                    Log.normal("Initializating PubStats...\r\n");
                    Program.pubStats.ConfigApply();
                    Log.normal("Completed Initialize PubStats.\r\n\r\n");
                   
                    Log.normal("Initializating Servers Config...\r\n\r\n");
                    SessionList.ConfigApply();
                    Log.normal("Complete Servers Config.\r\n\r\n");
                } break;
                default:
                {
                    Log.commandHelp("Command Reload, Unknow tableName: "+args[1]+".\r\n");
                    Log.commandHelp("Command Reload, Syntax Error.\r\n  Usage:\r\n    reload #table_name\r\n      #table_name can be \"all\" or \"config\".\r\n");
                } break;
            }
            Log.command("End Reloading of " + args[1] + ".\r\n");
        }
        private static void Exit()
        {
            Program.Exit();
        }
    }
}
