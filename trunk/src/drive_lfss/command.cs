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
            string[] args = _commandText.Split(new string[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);

            switch (args[0])
            {
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
                    if (itr.Current.Value.IsSocketStatus(InSim_Socket_State.INSIM_SOCKET_CONNECTED))
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

                    if (session.IsSocketStatus(InSim_Socket_State.INSIM_SOCKET_CONNECTED))
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
            if (args.Length != 2)
            {
                Log.normal("Command Say, Syntax Error.\r\n  Usage:\r\n    say #serverName $Message\r\n");
                return;
            }

            if (args[0] == "all")
            {
               Dictionary<string, Session>.Enumerator itr = SessionList.sessionList.GetEnumerator();
               while(itr.MoveNext())
                    itr.Current.Value.AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(args[1])));
            }
            else
            {
                string serverName = args[0];

                if (SessionList.sessionList.ContainsKey(serverName))
                    SessionList.sessionList[serverName].AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(args[1])));
                else
                    Log.command("Command Announce, serverName Not Found, Server Requested was: " + args[0] + "\r\n");
            }
        }
        private static void Exit()
        {
            Log.normal("Exiting Requested, Please Wait For All Thread Too Close...\n\r");
            Program.Exit();
        }
    }
}
