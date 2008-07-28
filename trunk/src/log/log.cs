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
using System.Threading;

namespace Drive_LFSS.Log_
{
    using Drive_LFSS.Session_;
    [Flags]public enum Log_Type : ushort
    {
        LOG_FULL = 0,
        LOG_NORMAL = 1,
        LOG_CHAT = 2,
        LOG_COMMAND = 4,
        LOG_ERROR = 8,
        LOG_DEBUG = 16,
        LOG_MISSING_DEFINITION = 32, 
        LOG_NETWORK = 64,
        LOG_DATABASE = 128,
        LOG_PING = 256,
        LOG_DISABLE = 0xFFFF
    }
    //If Color Not Allways Working Good, Cause log is Used by MultiThread! and Console Color Seem to be... MS way :)
    sealed public class sLog
    {
        public sLog(ushort _serverId)
        {
            if (!isInitialized)
            {
                throw new Exception("You can't Create Log Without Initializing, Please call: sLog.Init() First!");
            }
            serverId = _serverId;
        }
        public sLog()
        {
            if (!isInitialized)
            {
                throw new Exception("You can't Create Log Without Initializing, Please call: sLog.Init() First!");
            }
            serverId = 0;
        }
        public Log_Type logDisable = Log_Type.LOG_DISABLE;
        public ushort serverId = 0;


        private static Mutex mutexConsoleColor = new Mutex();
        private const string LOG_FILE_PATH = @".\drive_lfss.log";
        private static System.IO.StreamWriter streamWriter;
        private static bool isInitialized = false;

        public static bool Initialize()
        {
            isInitialized = true;
            try
            {
                streamWriter = System.IO.File.CreateText(LOG_FILE_PATH);
            }
            catch (System.Exception error)
            {
                Console.Write(System.DateTime.Now + " - ERROR---: " + error.Message + "\r\n");
                Console.Write("Please Check Error Comming from the Log System.\r\nDrive Life For Speed Server, can't continue without log system.\r\n");
                return false;
            }
            return true;
        }
        public static void flush()
        {
            streamWriter.Flush();
        }
        public void error(string msg)
        {
            
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                if ((logDisable & Log_Type.LOG_ERROR) == 0)
                    streamWriter.Write(System.DateTime.Now + _serverName + " ERROR--: " + msg);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            mutexConsoleColor.ReleaseMutex();
        }
        public void normal(string msg)
        {
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                if ((logDisable & Log_Type.LOG_NORMAL) == 0)
                    streamWriter.Write(System.DateTime.Now + _serverName + " NORMAL-: " + msg);
                
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public void chat(string msg)
        {
            if ((logDisable & Log_Type.LOG_CHAT) > 0) return;
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                streamWriter.Write(System.DateTime.Now + _serverName + " CHAT----: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public void command(string msg)
        {
            if ((logDisable & Log_Type.LOG_COMMAND) > 0) return;
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                streamWriter.Write(System.DateTime.Now + _serverName + " COMMAND-: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public void debug(string msg)
        {
            if ((logDisable & Log_Type.LOG_DEBUG) > 0) return;
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                streamWriter.Write(System.DateTime.Now + _serverName + " DEBUG--: " + msg);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public void missingDefinition(string msg)
        {
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                if ((logDisable & Log_Type.LOG_MISSING_DEFINITION) == 0)
                    streamWriter.Write(System.DateTime.Now + _serverName + " MISSING: " + msg);

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public void network(string msg)
        {
            if ((logDisable & Log_Type.LOG_NETWORK) > 0) return;
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                streamWriter.Write(System.DateTime.Now + " NETWORK-: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public void database(string msg)
        {
            if ((logDisable & Log_Type.LOG_DATABASE) > 0) return;
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                streamWriter.Write(System.DateTime.Now + " DATABASE: " + msg);
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public void ping(string msg)
        {
            if ((logDisable & Log_Type.LOG_PING) > 0) return;
            string _serverName = GetServerName();

            mutexConsoleColor.WaitOne();
            {
                streamWriter.Write(System.DateTime.Now + " PING----: " + msg);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(_serverName + msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }

        private string GetServerName()
        {
            if (serverId == 0)
                return "";

            //Create Function into Server Go that That Info
            if (SessionList.sessionList.ContainsKey(serverId))
                return "Server["+serverId+/*Session.server[serverId].settings.connectionName+*/"] -> ";

            return "Unknow -> ";
        }
    }
}
/*0   BLACK
1   BLUE
2   GREEN
3   CYAN
4   RED
5   MAGENTA
6   BROWN
7   LIGHTGRAY
8   DARKGRAY
9   LIGHTBLUE
10  LIGHTGREEN
11  LIGHTCYAN
12  LIGHTRED
13  LIGHTMAGENTA
14  YELLOW
15  WHITE
*/