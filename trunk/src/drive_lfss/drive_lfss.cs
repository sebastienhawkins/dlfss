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
using System.Timers;
using System.Threading;
using System.Runtime.InteropServices;

namespace Drive_LFSS
{
    using Drive_LFSS.Config_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.CommandConsole_;
    using Drive_LFSS.Database_;
    using Mono.Data.SqliteClient;

    public static class Program
    {
        #region Loop / Console / TrapSIGNTerm
        public static readonly int sleep = 50; // Speed of Operation

        [DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine Handler, bool AddToReceiveQueud);
        public delegate bool HandlerRoutine(Ctrl_Types CtrlType);

        //MultiThread Console Command
        private static Thread ThreadCaptureConsoleCommand = new Thread(new ThreadStart(CaptureConsoleCommand));

        //Used to Stop The Program and is Thread!
        public static bool MainRun = true;
        #endregion

        #region Timer For Main Loop

        private static long ticks = DateTime.Now.Ticks;
        private static uint diff = 0;
        
        #endregion

        [MTAThread]
        public static void Main()
		{
            //Console Trap CTRL-C, //Remove Unhandle Exception
            Console.CancelKeyPress += new ConsoleCancelEventHandler(DisgraceExit);
            SetConsoleCtrlHandler(new HandlerRoutine(DisgraceExit), true);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnHandleException);

            //So user won't have working folder probleme.
            string processPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            processPath = processPath.Substring(0, processPath.LastIndexOf('\\'));

            //Write Startup Banner
            WriteBanner();

            //Configuration
            if (!Config.Initialize(processPath + "\\Drive_LFSS.cfg"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Can't Initialize the Config, Will now QUIT.!\r\n\r\n");
                System.Threading.Thread.Sleep(10000);
                CommandConsole.Exec("exit");
                return;
            } Log.normal("Config Initialized...\r\n\r\n");

            //Loggin
            if (!Log.Initialize(processPath+"\\dlfss.log"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Can't Initialize the Log System, Will now QUIT.!\r\n\r\n");
                System.Threading.Thread.Sleep(10000);
                CommandConsole.Exec("exit");
                return;
            } Log.normal("Log System Initialized...\r\n\r\n");

            //InGame Command System
            Log.normal("Initializating InGame Command.\r\n\r\n");
            //Command.Init();
            
            //Console System, Purpose for MultiThreading the Console Input
            Log.normal("Initializating Console Command.\r\n\r\n");
            ThreadCaptureConsoleCommand.Start();

            //Database Initialization
            Log.normal("Initializating Database...\r\n\r\n");
            DatabaseStorage.Initialize();

            //Create Object for All Configured Server
            Log.normal("Initializating Servers Config...\r\n\r\n");
            SessionList.LoadServerConfig( );
            
            //Session.InitializeServerList();
            Log.normal("Starting Normal Operation!\r\n\r\n");
            #region MainThread update
            uint TimerLogFlush = 0;
            while (MainRun)
            {
                //Timer Thing
                diff = (uint)(DateTime.Now.Ticks - ticks) / 10000; //Diff in MS
                ticks = DateTime.Now.Ticks;

                TimerLogFlush += diff;
                if (TimerLogFlush > 30000)
                {
                    TimerLogFlush = 0;
                    Log.flush();
                }
                
                //update This Thread Process
                SessionList.update(diff);

                //Slow down Operation , so cpu usage don't become a probleme.
               System.Threading.Thread.Sleep(sleep);
            }
            #endregion
		}
        private static void WriteBanner()
        {
            //Opening Banner
            Log.normal("              _____                             _      _____      __  \r\n");
            Log.normal("              /    )         ,                  /      /    '   /    )\r\n");
            Log.normal("          ---/----/---)__-------------__-------/------/__-------\\-----\r\n");
            Log.normal("            /    /   /   ) /   | /  /___)     /      /           \\    \r\n");
            Log.normal("          _/____/___/_____/____|/__(___ _____/____/_/________(____/___\r\n");
            Log.normal("                                                                  v0.1\r\n");
            //sLog.normal("                                                                      \r\n");       
            Log.normal("                    _______________________________________\r\n");
            Log.normal("                          __                               \r\n");
            Log.normal("                        /    )                             \r\n");
            Log.normal("                    ----\\--------__---)__---------__---)__-\r\n");
            Log.normal("                         \\     /___) /   ) | /  /___) /   )\r\n");
            Log.normal("                    _(____/___(___ _/______|/__(___ _/_____\r\n");
            Log.normal("\r\n");
            Log.normal("\r\n");
        }
        private static void CaptureConsoleCommand()
        {
            string consoleText;
            while (MainRun)
            {
                consoleText = Console.ReadLine();
                if (consoleText != null && consoleText.Length < 1) continue;
                CommandConsole.Exec(consoleText);
                System.Threading.Thread.Sleep(sleep);
            }
        }
        private static void ConsoleExitTimer()
        {
            Log.error("Exiting into 5 seconds.\r\n");

            byte secondeBeforeExit = 5;
            while(secondeBeforeExit > 0)
            {
                Console.Clear();
                Log.error("Exiting into " + --secondeBeforeExit + " seconds.\r\n");
                System.Threading.Thread.Sleep(1000);
            }
        }
        private static void UnHandleException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception error = (Exception)args.ExceptionObject;
            Log.error("Critical Error, Auto Shutdown Initiated, 30 Seconde... \"exit\".\r\n");
            Log.error("The critical error Was: " + error.Message + "\r\n");
            SessionList.exit();
            System.Threading.Thread.Sleep(25000);
            ConsoleExitTimer();
            ThreadCaptureConsoleCommand.Abort();
            Log.flush();
            MainRun = false;
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(0);
        }
        private static void DisgraceExit(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            SessionList.exit();
            Log.error("Application has been closed Disgracefully, please next time, type \"exit\", Going Shutdown into 15 secondes.\r\n");
            System.Threading.Thread.Sleep(10000);
            ConsoleExitTimer();
            ThreadCaptureConsoleCommand.Abort();
            Log.flush();
            MainRun = false;
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(0);
        }
        private static bool DisgraceExit(Ctrl_Types ctrlType)
        {
            SessionList.exit();
            Log.error("Application has been closed Disgracefully, please next time, type \"exit\", Going Shutdown into 15 secondes.\r\n");
            System.Threading.Thread.Sleep(10000);
            ConsoleExitTimer();
            ThreadCaptureConsoleCommand.Abort();
            Log.flush();
            MainRun = false;
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(0);
            return true;
        }
        public static void Exit()
        {
            MainRun = false;
            
            SessionList.exit();
            Log.flush();

            System.Environment.Exit(0);
        }
    }
}
