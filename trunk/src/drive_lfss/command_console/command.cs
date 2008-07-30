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
namespace Drive_LFSS.CommandConsole_
{
    using Drive_LFSS;
    using System.Collections.Generic;
    using System;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Game_;
    using Drive_LFSS.Definition_;
    //using System.Collections.Generic;
    //using System.Text;

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
                    Program.log.error("Unknow Command: " + _commandText + "\r\n");
                    break;
                }
            }
        }
        private static void Status(string[] args)
        {
            if (args.Length != 2)
            {
                Program.log.normal("Command Status, Syntax Error.\r\n  Usage:\r\n    status #serverId\r\n");
                return;
            }

            if (args[1] == "*")
            {
                //Maybe you Real Iterator<Session>
                foreach (KeyValuePair<ushort, SessionList.SessionStruct> keyPair in SessionList.sessionList)
                {
                    Session session = SessionList.sessionList[keyPair.Key].session;

                    if(session.IsSocketStatus(InSim_Socket_State.INSIM_SOCKET_CONNECTED))
                        Program.log.normal("ServerId: " + keyPair.Key + ", Status: online, ReactionTime: " + session.GetReactionTime() + "ms" + ", DriversCount: " + session.GetNbrOfDrivers() + "\r\n");
                    else
                        Program.log.error("ServerId: " + keyPair.Key + ", Status: offline, ReactionTime+/-: -ms, DriversCount: -\r\n");
                }
            }
            else
            {
                ushort serverId;
                try { serverId = Convert.ToUInt16(args[1]); }
                catch (Exception _exception)
                {
                    Program.log.normal("Command Status, Syntax Error.\r\n  Usage:\r\n    status #serverId\r\n");
                    return;
                }

                if (SessionList.sessionList.ContainsKey(serverId))
                {
                    Session session = SessionList.sessionList[serverId].session;

                    if (session.IsSocketStatus(InSim_Socket_State.INSIM_SOCKET_CONNECTED))
                        Program.log.normal("ServerId: " + serverId + ", Status: online, Latency: " + session.GetLatency() + "ms" + ", DriversCount: " + session.GetNbrOfDrivers() + "\r\n");
                    else
                        Program.log.error("ServerId: " + serverId + ", Status: offline, Latency: -ms, DriversCount: -\r\n");
                }
                else
                    Program.log.command("Command Status, ServerId Not Found, Server Requested was: " + serverId + "\r\n");
            }
        }
        private static void Say(string[] args)
        {
            if (args.Length != 2)
            {
                Program.log.normal("Command Say, Syntax Error.\r\n  Usage:\r\n    say #serverId $Message\r\n");
                return;
            }

            if (args[0] == "*")
            {
                foreach (KeyValuePair<ushort, SessionList.SessionStruct> keyPair in SessionList.sessionList)
                    SessionList.sessionList[keyPair.Key].session.AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(args[1])));
            }
            else
            {
                ushort serverId;
                try { serverId = Convert.ToUInt16(args[0]); }
                catch (Exception _exception)
                {
                    Program.log.normal("Command Announce, Syntax Error.\r\n  Usage:\r\n    announce #serverId $Message\r\n");
                    return;
                }

                if (SessionList.sessionList.ContainsKey(serverId))
                    SessionList.sessionList[serverId].session.AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(args[1])));
                else
                    Program.log.command("Command Announce, ServerId Not Found, Server Requested was: " + args[0] + "\r\n");
            }
        }
        private static void Exit()
        {
            Program.log.normal("Exiting Requested, Please Wait For All Thread Too Close...\n\r");
            Program.Exit();
        }
    }
}
