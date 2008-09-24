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
    using Drive_LFSS.Storage_;
    using Drive_LFSS.Irc_;
    using Drive_LFSS.Irc_.Data_;
    using Drive_LFSS.PubStats_;

    public sealed class Program
    {
        public static int sleep = 50; // Speed of Operation
        public static long tickPerMs = TimeSpan.TicksPerMillisecond;

        /*[DllImport("Kernel32")]
        public static extern bool SetConsoleCtrlHandler(HandlerRoutine handler, bool add);
        public delegate bool HandlerRoutine(ushort ctrlType);*/


        //Console Command Thread
        private static readonly Thread threadCaptureConsoleCommand = new Thread(new ThreadStart(CaptureConsoleCommand));

        //Used to Stop The Program and is Thread!
        public static bool MainRun = true;

        //Database
        public static IDatabase dlfssDatabase = null;

        //Storage, All Storage FMT need a 'p' value, this is representing is Unique Index into the Array, the value must be a uint32
        public static readonly ButtonTemplate buttonTemplate = new ButtonTemplate(new string[2] { "button_template", "pxuuuuuuuss" });
        public static readonly GuiTemplate guiTemplate = new GuiTemplate(new string[2] { "gui_template", "pxsus" });
        public static readonly TrackTemplate trackTemplate = new TrackTemplate(new string[2] { "track_template", "psssuuuuu" });
        public static readonly CarTemplate carTemplate = new CarTemplate(new string[2] { "car_template", "pssuu" });
        public static readonly RaceTemplate raceTemplate = new RaceTemplate(new string[2] { "race_template", "psusuuuuuu" });
        public static readonly DriverBan driverBan = new DriverBan(new string[2] { "driver_ban", "psssuuu" });
        
        //mIRC
        public static readonly IrcClient ircClient = new IrcClient();
        
        //PubStats
        public static readonly PubStats pubStats = new PubStats();

        //Local Path
        public static string processPath = "";

        [MTAThread]
        private static void Main()
		{
            //Console Trap CTRL-C,
            Console.CancelKeyPress += new ConsoleCancelEventHandler(DisgraceExit);
            
            //Unhandle exception
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnHandleException);

            //Put static working folder.
            processPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            processPath = processPath.Substring(0, processPath.LastIndexOf(System.IO.Path.DirectorySeparatorChar));

            //Write Startup Banner
            WriteBanner();

            //Configuration
            if (!Config.Initialize(processPath + System.IO.Path.DirectorySeparatorChar + "Drive_LFSS.cfg"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Can't Initialize the Config, Will now QUIT.!\r\n\r\n");
                System.Threading.Thread.Sleep(10000);
                CommandConsole.Exec("exit");
                return;
            } Log.normal("Config Initialized...\r\n\r\n");
            Program.ConfigApply();

            //Logging
            if (!Log.Initialize(processPath + System.IO.Path.DirectorySeparatorChar + "Drive_LFSS.log"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Can't Initialize the Log System, Will now QUIT.!\r\n\r\n");
                System.Threading.Thread.Sleep(10000);
                CommandConsole.Exec("exit");
                return;
            } Log.normal("Log System Initialized...\r\n\r\n");

           
            //Console System, Purpose for MultiThreading the Console Input
            Log.normal("Initializating Console Command.\r\n\r\n");
            threadCaptureConsoleCommand.Name = "Console Capture";
            threadCaptureConsoleCommand.Start();

            //Database Initialization
            Log.normal("Initializating Database...\r\n");

            List<string> databaseChoices = Config.GetIdentifierList("Database");
            if (databaseChoices.Contains("MySQL"))
            {
                string[] infos = Config.GetStringValue("Database","MySQL","ConnectionInfo").Split(';');
                if (infos.Length != 6)
                    throw new Exception("Configuration Error, Invalide Value Count For: Database.MySQL.ConnectionInfo");

                Log.commandHelp("  Using MySQL Database.\r\n");
                dlfssDatabase = new DatabaseMySQL("Database=" + infos[4] + ";Data Source=" + infos[0] + ";Port=" + infos[1] + ";User Id=" + infos[2] + ";Password=" + infos[3] + ";Use Compression=" + infos[5] + ";Pooling=false");
            }
            else
            {
                Log.commandHelp("  Using SQLite Database.\r\n");
                dlfssDatabase = new DatabaseSQLite(Config.GetStringValue("Database", "SQLite", "ConnectionInfo"));
            }
            Log.normal("Completed Initialize Database...\r\n\r\n");

            //Initialize Storage
            Log.normal("Initializating Storage...\r\n");
            trackTemplate.Load(true);
            carTemplate.Load(true);
            buttonTemplate.Load(true);
            guiTemplate.Load(true);
            raceTemplate.Load(false);
            driverBan.Load(false);
            Log.normal("Completed Initialize Storage.\r\n\r\n");

            //Irc Client
            Log.normal("Initializating mIRC Client...\r\n");
            string isActivated = Config.GetStringValue("mIRC", "Activate").ToLowerInvariant();
            if (isActivated == "yes" || isActivated == "1" || isActivated == "true" || isActivated == "on" || isActivated == "activate")
            {
                ircClient.ConfigApply();
                ircClient.Connect();
            }
            else
                Log.commandHelp("  mIRC Client is Disable.\r\n");
            Log.normal("Completed Initialize mIRC Client.\r\n\r\n");

            //PubStats Initialization
            Log.normal("Initializating PubStats...\r\n");
            pubStats.ConfigApply();
            Log.normal("Completed Initialize PubStats.\r\n\r\n");

            //Create Object for All Configured Server
            Log.normal("Initializating Servers Config...\r\n\r\n");
            SessionList.ConfigApply( );
            
            //Session.InitializeServerList();
            Log.normal("DEPART|");
            for (float itr = 200; (itr /= 1.1f) > 1;)
            {
                Log.progress(".");
                System.Threading.Thread.Sleep((int)itr);
            }
            Log.normal("|FINISH\r\n\r\n");

            if(Config.GetIntValue("CPU","Priority") > 0)
                System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.High;
           
            #region MainThread update

            long ticks = DateTime.Now.Ticks;
            uint diff = 0;
            
            uint TimerLogFlush = 0;
            while (MainRun)
            {
                //Timer Thing, Really have to test .net fastest solution.
                diff = (uint)((DateTime.Now.Ticks - ticks) / tickPerMs); //Diff in MS
                ticks = DateTime.Now.Ticks;

                TimerLogFlush += diff;
                if (TimerLogFlush > 30000)
                {
                    TimerLogFlush = 0;
                    Log.flush();
                }

                //update This Thread Process
                SessionList.update(diff);
                pubStats.update(diff);

                //Keep Alive on Database
                ((Database)dlfssDatabase).update(diff);

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
            Log.normal("                                                                  v0.2\r\n");
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
                System.Threading.Thread.Sleep(200);
            }
        }
        public static void ConfigApply()
        {
            sleep = Config.GetIntValue("Interval", "GameThreadUpdate");
            if (sleep < 1)
                sleep = 1;
        }
        public static void Reload(string what)
        {
            Log.command("Start Reloading of " + what + ".\r\n");
            lock(dlfssDatabase)
            {switch (what){
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
                        Config.Initialize(Program.processPath + System.IO.Path.DirectorySeparatorChar + "Drive_LFSS.cfg");
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
                case "track_template":
                    {
                        Program.trackTemplate.Load(true);
                        Log.commandHelp("track_template reloaded.\r\n");
                    } break;
                case "car_template":
                    {
                        Program.carTemplate.Load(true);
                        Log.commandHelp("car_template reloaded.\r\n");
                    } break;
                case "button_template":
                    {
                        Program.buttonTemplate.Load(true);
                        Log.commandHelp("button_template reloaded.\r\n");
                    } break;
                case "race_template":
                    {
                        Program.raceTemplate.Load(false);
                        Log.commandHelp("race_template reloaded.\r\n");
                    } break;
                case "driver_ban":
                    {
                        Program.driverBan.Load(false);
                        Log.commandHelp("driver_ban reloaded.\r\n");
                    } break;
                case "gui_template":
                    {
                        Program.guiTemplate.Load(true);
                        Log.commandHelp("gui_template reloaded.\r\n");
                    } break;
                case "config":
                    {
                        Log.normal("Initializating DLFSS Client...\r\n");
                        Config.Initialize(Program.processPath + System.IO.Path.DirectorySeparatorChar + "Drive_LFSS.cfg");
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
            } }
            Log.command("End Reloading of " + what + ".\r\n");
        }
        public static void Exit()
        {
            Exit(true);
        }
        private static void Exit(bool real)
        {
            Log.normal("Exiting Requested, Please Wait For All Thread Too Exit...\n\r");

            MainRun = false;

            SessionList.DisconnectAll();

            if (ircClient != null && ircClient.IsConnected)
                ircClient.Disconnect();

            Log.flush();

            //Abording a thread will create a UnhandleException, catching this exception , will what ever report a message.
            //This way completely Hiden from user + gave only 1 exception at a time! better this way.
            AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(UnHandleException);
            threadCaptureConsoleCommand.Abort();

            if (real)
                System.Environment.Exit(0);
        }

        //make ppl with disgrace exit thing 5 seconde about what they just made ;)
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
            Log.error("Critical Error, Auto Shutdown in 20 Seconde...\r\n");
            Log.error("Exception Was: " + args.ExceptionObject.ToString() + "\r\n");
            Exit(false);
            System.Threading.Thread.Sleep(15000);
            ConsoleExitTimer();
            System.Environment.Exit(0);
        }
        private static void DisgraceExit(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            DisgraceExit();
            System.Environment.Exit(0);
        }
        private static bool DisgraceExit(ushort ctrlType)
        {
            DisgraceExit();
            return true;
        }
        private static void DisgraceExit()
        {
            Log.error("Application has been closed Disgracefully, please next time, type \"exit\", Going Shutdown into 15 secondes.\r\n");
            Exit(false);
            System.Threading.Thread.Sleep(10000);
            ConsoleExitTimer();
            System.Environment.Exit(0);
        }
    }
}
