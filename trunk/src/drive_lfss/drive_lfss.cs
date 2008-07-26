using System;
using System.Threading;
using System.Runtime.InteropServices;

//using MySql.Data.MySqlClient;
//using MySql.Data.Types;

namespace Drive_LFSS
{
    using System.Timers;
    using System.Collections.Generic;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.CommandConsole_;
    using Drive_LFSS.Session_;
    using Drive_LFSS.dbs_;

    internal static class Program
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

        public static sLog Log;

        [MTAThread]
        public static void Main()
		{
            //Start the log System
            if (!sLog.Init()) return;
            Log = new sLog();

            //Console Trap CTRL-C
            Console.CancelKeyPress += new ConsoleCancelEventHandler(DisgraceExit);
            SetConsoleCtrlHandler(new HandlerRoutine(DisgraceExit), true);

            //Remove Unhandle Exception
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnHandleException);

            //Write Startup Banner
            WriteBanner();

            //InGame Command System
            Log.normal("Initialization of InGame Command.\r\n");
            //Command.Init();
            
            //Console System, Purpose for MultiThreading the Console Input
            Log.normal("Initialization of Console Command.\r\n");
            ThreadCaptureConsoleCommand.Start();


            //Create Object for All Configured Server
            Log.normal("Loading Servers Config.\r\n");
            SessionList.LoadServerConfig( );
            Log.normal("Initialize Servers Object.\r\n");
            //Session.InitializeServerList();

            Log.normal("Starting Normal Operation!\r\n");
            #region MainThread update
            uint TimerLogFlush = 0;
            while (MainRun)
            {
                //Timer Thing
                diff = (uint)(DateTime.Now.Ticks - ticks) / 10000; //Diff in MS
                ticks = DateTime.Now.Ticks;

                TimerLogFlush += diff;
                if (TimerLogFlush > 8000)
                {
                    TimerLogFlush = 0;
                    sLog.flush();
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
            //Program.log.normal("                                                                      \r\n");       
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
                switch (consoleText)
                {
                    case "exit": CommandConsole.Exit(); break;
                    default: Log.error("Unknow Command: " + consoleText + "\r\n"); break;
                }
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
            sLog.flush();
            MainRun = false;
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(-2);
        }
        private static void DisgraceExit(object sender, ConsoleCancelEventArgs args)
        {
            args.Cancel = true;
            SessionList.exit();
            Log.error("Application has been closed Disgracefully, please next time, type \"exit\", Going Shutdown into 15 secondes.\r\n");
            System.Threading.Thread.Sleep(10000);
            ConsoleExitTimer();
            ThreadCaptureConsoleCommand.Abort();
            sLog.flush();
            MainRun = false;
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(-1);
        }
        private static bool DisgraceExit(Ctrl_Types ctrlType)
        {
            SessionList.exit();
            Log.error("Application has been closed Disgracefully, please next time, type \"exit\", Going Shutdown into 15 secondes.\r\n");
            System.Threading.Thread.Sleep(10000);
            ConsoleExitTimer();
            ThreadCaptureConsoleCommand.Abort();
            sLog.flush();
            MainRun = false;
            System.Threading.Thread.Sleep(1000);
            System.Environment.Exit(-1);
            return true;
        }
        public static void Exit()
        {
            MainRun = false;
            
            SessionList.exit();
            sLog.flush();

            System.Environment.Exit(0);
        }
    }
}
/*public void InsertRow(string myConnectionString) 
{
  // If the connection string is null, use a default.
  if(myConnectionString == "Persist Security Info=False;database=MyDB;server=MySqlServer;user id=myUser;Password=myPass") 
  {
    myConnectionString = "Database=Test;Data Source=localhost;User Id=username;Password=pass";
  }
  MySqlConnection myConnection = new MySqlConnection(myConnectionString);
  string myInsertQuery = "INSERT INTO Orders (id, customerId, amount) Values(1001, 23, 30.66)";
  MySqlCommand myCommand = new MySqlCommand(myInsertQuery);
  myCommand.Connection = myConnection;
  myConnection.Open();
  myCommand.ExecuteNonQuery();
  myCommand.Connection.Exit();
}*/