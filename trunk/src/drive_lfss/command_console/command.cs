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
    using Drive_LFSS.Session_;
    using System.Collections.Generic;
    using System;
    using Drive_LFSS.Packet_;
    //using System.Collections.Generic;
    //using System.Text;

    static class CommandConsole
    {
        public static void Exec(string _commandText)
        {
            string[] args = _commandText.Split(new string[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);

            switch (args[0])
            {
                case "announce": Announce(args[1]); break;
                case "exit": Exit(); break;
                default:
                {
                    Program.log.error("Unknow Command: " + _commandText + "\r\n");
                    break;
                }
            }
        }
        private static void Announce(string _commandText)
        {
            string[] args = _commandText.Split(new string[] { " " }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length != 2)
            {
                Program.log.normal("Command Announce, Syntax Error.\r\n  Usage:\r\n    announce #serverId $Message\r\n");
                return;
            }

            if (args[0] == "*")
            {
                foreach (KeyValuePair<ushort, Drive_LFSS.Session_.SessionList.SessionStruct> keyPair in SessionList.sessionList)
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
