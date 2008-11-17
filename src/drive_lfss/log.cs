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
using System.Text;
using System.Threading;

namespace Drive_LFSS.Log_
{
    using Drive_LFSS.Config_;

    [Flags]public enum Log_Type : ushort
    {
        LOG_FULL = 0,
        LOG_CHAT = 1,
        LOG_COMMAND = 2,
        LOG_DEBUG = 4,
        LOG_MISSING_DEFINITION = 8, 
        LOG_NETWORK = 16,
        LOG_DATABASE = 32,
        LOG_PROGRESS = 64,
        LOG_FEATURE = 128,
        LOG_DISABLE = unchecked((ushort)-1)
    }
    static class Log
    {
        private static Log_Type logDisable = Log_Type.LOG_DISABLE;
        private static string logPath = "";
        private static string logFileName = "";

        private static Mutex mutexConsoleColor = new Mutex();
        private static System.IO.StreamWriter streamWriter;
        private static List<string> stringWriter = new List<string>();
        private static bool isInitialized = false;

        public static bool Initialize(string _logPath, string _logFileName)
        {
            if (isInitialized)
                return true;

            try { streamWriter = System.IO.File.CreateText(_logPath +System.IO.Path.DirectorySeparatorChar+ _logFileName); }
            catch (System.Exception error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(error.Message+".!\r\n\r\n");
                return false;
            }
            streamWriter.Dispose();
            isInitialized = true;
            logPath = _logPath;
            logFileName = _logFileName;
            ConfigApply();
            return true;
        }
        //To be called After Config System is Initialised.
        public static void ConfigApply()
        {
            //TODO: Log path
            logDisable = unchecked((Log_Type)Config.GetIntValue("Log", "Disable"));
        }
        public static void flush()
        {
            flush(false);
        }
        public static void flush(bool crashLog)
        {
            if (stringWriter.Count == 0)
                return;

            streamWriter = System.IO.File.AppendText(logPath + System.IO.Path.DirectorySeparatorChar + logFileName);
            
            lock(streamWriter)
            {
                List<string>.Enumerator itr = stringWriter.GetEnumerator();
                
                while (itr.MoveNext())
                    streamWriter.Write(itr.Current);

                streamWriter.Flush();
                streamWriter.Dispose();

                stringWriter.Clear();
            }
            if(crashLog)
                System.IO.File.Copy(logPath + System.IO.Path.DirectorySeparatorChar + logFileName, logPath + System.IO.Path.DirectorySeparatorChar+"Crash-"+DateTime.Now.Ticks/10000000+".log",true);
        }

        public static void error(string msg)
        {
            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " ERROR--: " + msg);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            mutexConsoleColor.ReleaseMutex();
        }
        public static void normal(string msg)
        {
            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " NORMAL-: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void chat(string msg)
        {
            if ((logDisable & Log_Type.LOG_CHAT) > 0) 
                return;

            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " CHAT----: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void command(string msg)
        {
            if ((logDisable & Log_Type.LOG_COMMAND) > 0) 
                return;

            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " COMMAND-: " + msg);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void commandHelp(string msg)
        {
            mutexConsoleColor.WaitOne();
            {
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.Write(msg);
                Console.ForegroundColor = ConsoleColor.Gray;
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void debug(string msg)
        {
            if ((logDisable & Log_Type.LOG_DEBUG) > 0) 
                return;

            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " DEBUG--: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void missingDefinition(string msg)
        {
            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now +  " MISSING: " + msg);
                if ((logDisable & Log_Type.LOG_MISSING_DEFINITION) == Log_Type.LOG_MISSING_DEFINITION)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(msg);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void network(string msg)
        {
            if ((logDisable & Log_Type.LOG_NETWORK) > 0) 
                return;

            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " NETWORK-: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void database(string msg)
        {
            if ((logDisable & Log_Type.LOG_DATABASE) > 0) 
                return;

            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " DATABASE: " + msg);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            } 
            mutexConsoleColor.ReleaseMutex();
        }
        public static void progress(string msg)
        {
            mutexConsoleColor.WaitOne();
            {
                if ((logDisable & Log_Type.LOG_PROGRESS) == 0)
                    stringWriter.Add(System.DateTime.Now + " PROGRESS: " + msg);

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            } mutexConsoleColor.ReleaseMutex();
        }
        public static void feature(string msg)
        {
            if ((logDisable & Log_Type.LOG_FEATURE) > 0)
                return;
            mutexConsoleColor.WaitOne();
            {
                stringWriter.Add(System.DateTime.Now + " FEATURE: " + msg);
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.Write(ToASCII(msg));
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            mutexConsoleColor.ReleaseMutex();
        }
        private static string ToASCII(string text)
        {
            byte[] utf8String = Encoding.UTF8.GetBytes(text);
            byte[] asciiString = Encoding.ASCII.GetBytes(text);

            // Write the UTF-8 and ASCII encoded byte arrays. 
            //output.WriteLine("UTF-8  Bytes: {0}", BitConverter.ToString(utf8String));
            //output.WriteLine("ASCII  Bytes: {0}", BitConverter.ToString(asciiString));


            // Convert UTF-8 and ASCII encoded bytes back to UTF-16 encoded  
            // string and write.

            //return Encoding.UTF8.GetString(utf8String);
            return Encoding.ASCII.GetString(asciiString);
        }
    }
}
