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
    using Log_;
    using Packet_;
    using Game_;
    using Session_;

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
                    Log.error("Unknown command: " + _commandText + "\r\n");
                    break;
                }
            }
        }
        private static void Status(string[] args)
        {
            if (args.Length != 2)
            {
                Log.normal("Command - status, Syntax error.\r\n  Usage:\r\n    status #serverName\r\n      #serverName can be \"all\".\r\n");
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
                        Log.normal("ServerName: " + serverName + ", Status: online, ReactionTime: " + session.GetLatency() + "ms" + ", DriversCount: " + session.GetNbrOfDrivers() + "\r\n");
                    else
                        Log.error("ServerName: " + serverName + ", Status: offline, ReactionTime: -ms, DriversCount: -\r\n");
                }
                else
                    Log.command("Command - status, ServerName not found.\r\n  Server requested was: " + serverName + "\r\n");
            }
        }
        private static void Say(string[] args)
        {
            if (args.Length < 3)
            {
                Log.commandHelp("Command - say, Syntax error.\r\n  Usage:\r\n    say #serverName $Message\r\n      #serverName can be \"all\".\r\n");
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
                    Log.command("Command - announce, serverName Not Found: " + args[1] + "\r\n");
            }
        }
        private static void Reload(string[] args)
        {
            if (args.Length < 2)
            {
                Log.commandHelp("Command - reload, Syntax error.\r\n  Usage:\r\n    reload #table_name\r\n      #table_name can be \"all\" or \"config\".\r\n");
                return;
            }
            switch (args[1])
            {
                case "all": 
                {
                    lock (Program.dlfssDatabase) { Program.Reload("all"); }
                } break;
                case "track":
                case "track_template":
                {
                    lock (Program.dlfssDatabase) { Program.Reload("track_template"); }
                } break;
                case "car":
                case "car_template":
                {
                     lock (Program.dlfssDatabase) { Program.Reload("car_template");}
                } break;
                case "button":
                case "button_template":
                {
                     lock (Program.dlfssDatabase) { Program.Reload("button_template");}
                } break;
                case "race":
                case "race_template":
                {
                     lock (Program.dlfssDatabase) { Program.Reload("race_template");}
                } break;
                case "ban":
                case "driver_ban":
                {
                     lock (Program.dlfssDatabase) { Program.Reload("driver_ban");}
                } break;
                case "gui":
                case "gui_template":
                {
                     lock (Program.dlfssDatabase) { Program.Reload("gui_template");}
                } break;
                case "config":
                {
                     lock (Program.dlfssDatabase) { Program.Reload("config");}
                } break;
                default:
                {
                    Log.commandHelp("Command - reload, unknown tableName: "+args[1]+".\r\n");
                    Log.commandHelp("Command - reload, Syntax Error.\r\n  Usage:\r\n    reload #table_name\r\n      #table_name can be \"all\" or \"config\".\r\n");
                } break;
            }
        }
        private static void Exit()
        {
            Program.Exit();
        }
    }
}
