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
namespace Drive_LFSS.Server_
{
    using Log_;
    using Packet_;
    using Game_;
    using Definition_;
    using ChatModo_;
    using Storage_;

    sealed class CommandInGame
    {
        internal CommandInGame(Session _session)
        {
            session = _session;
            //       CommandName                CommandLevel                    CommandReference
           command["exit"] = new CommandName(2, new CommandDelegate(Exit));
           command["kick"] = new CommandName(1, new CommandDelegate(Kick));
           command["reload"] = new CommandName(1, new CommandDelegate(Reload));
           command["help"] = new CommandName(0, new CommandDelegate(Help));
           command["config"] = new CommandName(0, new CommandDelegate(Config));
           command["menu"] = new CommandName(0, new CommandDelegate(Menu));
           command["status"] = new CommandName(0, new CommandDelegate(Status));
           command["rank"] = new CommandName(0, new CommandDelegate(Rank));
           command["ranking"] = new CommandName(0, new CommandDelegate(Rank));
           command["top"] = new CommandName(0, new CommandDelegate(Rank));
           command["top10"] = new CommandName(0, new CommandDelegate(Rank));
           command["top20"] = new CommandName(0, new CommandDelegate(Rank));
           command["test"] = new CommandName(1, new CommandDelegate(Test));
           command["result"] = new CommandName(0, new CommandDelegate(Result));
           command["say"] = new CommandName(1, new CommandDelegate(Say));
           command["badword"] = new CommandName(2, new CommandDelegate(BadWord));
           command["endrace"] = new CommandName(1, new CommandDelegate(EndRace));
           command["end"] = new CommandName(1, new CommandDelegate(EndRace));
           command["searchrace"] = new CommandName(1, new CommandDelegate(SearchRace));
           command["loadrace"] = new CommandName(1, new CommandDelegate(LoadRace));
           command["yellowtime"] = new CommandName(1, new CommandDelegate(YellowTime));
        }
        ~CommandInGame()
        {
            if (true == false) { }
        }
        private struct CommandName
        {
            public CommandName(byte _level, CommandDelegate _cmd)
            {
                level = _level;
                cmd = _cmd;
            }
            public byte level;
            public CommandDelegate cmd;
        }
        private delegate void CommandDelegate(Driver driver, string[] args);
        private Dictionary<string, CommandName> command = new Dictionary<string, CommandName>();
        private readonly Session session;
        private const uint DISPLAY_TIME = 5500;
        
        public void Exec(Driver driver, string _commandText)
        {
            string[] args = _commandText.Split(' ');                 //Can Be little faster... since we need only left to first white space
            args[0] = args[0].Substring(1);                          //Remove "Prefix Command String".
            args[0] = args[0].ToLowerInvariant();
            if (args.Length < 1 || !command.ContainsKey(args[0]) ) 
            {
                driver.AddMessageMiddle("^7Unknown command: ^3" + _commandText + ".", DISPLAY_TIME);
                Log.command("Command.Exec(), Invalid command from User: " + driver.LicenceName + ", AccessLevel: " + (driver.IsAdmin ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
                return;
            }
            if( command[args[0]].level == 1 && !driver.IsAdmin )
            {
                driver.AddMessageMiddle("^7Must be admin to execute command: ^3" + args[0] + ".", DISPLAY_TIME);
                Log.command("Command.Exec(), Illegal command from User: " + driver.LicenceName + ", AccessLevel: " + (driver.IsAdmin ? "1" : "0") + ", CommandSend: " + _commandText + "\r\n");
                return;
            }
            if (command[args[0]].level == 2 && (driver.LicenceName != "forcemagic" && driver.LicenceName != "greenseed" && driver.LicenceName != "drive_lfss"))
            {
                driver.AddMessageMiddle("^2Must be ^7G^3reenseed ^2to ^1execute^2 that command: ^3" + args[0] + ".", DISPLAY_TIME);
                return;
            }
            command[args[0]].cmd(driver, args);
        }

        #region Commands
        private void Exit(Driver driver, string[] args)
        {
            Program.Exit();
        }
        private void Kick(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!kick ^3username", DISPLAY_TIME);
                return;
            }
            driver.AddMessageMiddle("^7You kicked username: ^3" + args[1] + ".", DISPLAY_TIME);
            Log.command("Command.Kick(), User: " + driver.LicenceName + ", Kicked User: " + args[1] + "\r\n");
            driver.ISession.SendMSTMessage("/kick " + args[1]);
        }
        private void Say(Driver driver, string[] args)
        {
            if (args.Length < 3)
            {
                driver.AddMessageMiddle("^2Invalid parameter count, Usage: ^5!say ^3serverName^2/^3all^2/^3irc commandText", DISPLAY_TIME);
                return;
            }

            string message = String.Join(" ", args, 2, args.Length - 2);

            if (args[1] == "all")
            {
                Dictionary<string, Session>.Enumerator itr = SessionList.Sessions.GetEnumerator();
                while (itr.MoveNext())
                    itr.Current.Value.AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(message)));
            }
            else if (args[1] == "irc")
            {
                if(!Program.ircClient.IsConnected)
                    driver.AddMessageMiddle("^2Command ^5say ^2to ^7irc^2, mIRC is ^1disable^2.", DISPLAY_TIME);
                else
                    Program.ircClient.SendToChannel(message);
            }
            else
            {
                string serverName = args[1];

                if (SessionList.Sessions.ContainsKey(serverName))
                    session.AddToTcpSendingQueud(new Packet(Packet_Size.PACKET_SIZE_MST, Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST(message)));
                else
                    driver.AddMessageMiddle("^2Command ^5say^2 serverName Not Found: ^7" + args[1] + "^2.", DISPLAY_TIME);
            }
        }
        private void Reload(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!reload ^3tableName", DISPLAY_TIME);
                return;
            }
            switch (args[1])
            {
                case "all":
                    {
                        Program.Reload("all");
                        driver.AddMessageMiddle("^7Completed reloading, ^3everything", DISPLAY_TIME);
                    } break;
                case "track":
                case "track_template":
                    {
                        Program.Reload("track_template");
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                    } break;
                case "car":
                case "car_template":
                    {
                        Program.Reload("car_template");
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                    } break;
                case "button":
                case "button_template":
                    {
                        Program.Reload("button_template");
                        driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                    } break;
                case "race":
                {
                    Program.Reload("race_template");
                    Program.Reload("race_map");
                    driver.AddMessageMiddle("^7Completed reloading, ^3race_template & race_map", DISPLAY_TIME);
                } break;
                case "race_template":
                {
                    Program.Reload("race_template");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                } break;
                case "restriction_race": 
                {
                    Program.Reload("restriction_race");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                }break;
                case "restriction_join": 
                {
                    Program.Reload("restriction_join");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                }break;
                case "race_map":
                {
                    Program.Reload("race_map");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                } break;
                case "bad":
                case "bad_word":
                {
                    Program.Reload("bad_word");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                }break;
                case "ban":
                case "driver_ban":
                {
                    Program.Reload("driver_ban");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                } break;
                case "gui":
                case "gui_template":
                {
                    Program.Reload("gui_template");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                } break;
                case "top20":
                case "rank":
                case "ranking":
                case "stats_rank_driver":
                {
                    Program.Reload("rank");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                } break;
                case "config":
                {
                    Program.Reload("config");
                    driver.AddMessageMiddle("^7Completed reloading, ^3" + args[1], DISPLAY_TIME);
                } break;
                default: 
                    driver.AddMessageMiddle("^7Unknown tableName, ^3" + args[1], DISPLAY_TIME); break;
            }
            Log.command("Command.Reload(), User: " + driver.LicenceName + ", reloaded: " + args[1] + "\r\n");
        }
        private void Help(Driver driver, string[] args)
        {
            driver.SendHelpGui();
            //driver.AddMessageMiddle("^3Sorry, the help system is still TODO!, try !config",8000);
        }
        private void Config(Driver driver, string[] args)
        {
            driver.SendConfigGui();
        }
        private void Menu(Driver driver, string[] args)
        {
            driver.SendMenuGui();
        }
        private void Status(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!status ^3serverName^7/^3all^7/^3current", DISPLAY_TIME);
                return;
            }

            if (args[1] == "all")
            {
                //Maybe you Real Iterator<Session>
                Dictionary<string, Session>.Enumerator itr = SessionList.Sessions.GetEnumerator();
                string textToSend = "Status for all Server\r\n";
                while (itr.MoveNext())
                {
                    if (itr.Current.Value.IsConnected())
                    {
                        string reactionTime;
                        long _rt = itr.Current.Value.GetReactionTime() ;
                        if (_rt == 0)
                            reactionTime = "^7" + _rt.ToString();
                        else if (_rt > 0)
                            reactionTime = "^4" + _rt.ToString();
                        else
                            reactionTime = "^3" + _rt.ToString();

                        textToSend+="^8ServerName ^7" + itr.Current.Key + ", ^8Status ^2online^8, ReactionTime ^7" + reactionTime + "^8ms" + ", DriversCount ^7" + itr.Current.Value.GetNbrOfDrivers()+"\r\n";
                    }
                    else
                        textToSend+="^8ServerName ^7" + itr.Current.Key + ", ^8Status ^1offline^8, ReactionTime ^7na^8ms, DriversCount ^7na\r\n";
                }

                driver.SendGui((ushort)Gui_Entry.TEXT, textToSend);
            }
            else
            {
                string serverName = args[1];
                if(serverName == "current")
                    serverName = session.GetSessionName();

                if (SessionList.Sessions.ContainsKey(serverName))
                {
                    Session _session = SessionList.Sessions[serverName];

                    if (_session.IsConnected())
                    {
                        string reactionTime;
                        long _rt = _session.GetReactionTime();
                        if (_rt == 0)
                            reactionTime = "^7" + _rt.ToString();
                        else if (_rt > 0)
                            reactionTime = "^4" + _rt.ToString();
                        else
                            reactionTime = "^3" + _rt.ToString();

                        driver.AddMessageMiddle("^8ServerName ^7" + serverName + ", ^8Status ^2online^8, ReactionTime ^7" + reactionTime + "^8ms" + ", DriversCount ^7" + _session.GetNbrOfDrivers(),4500);
                    }
                    else
                        driver.AddMessageMiddle("^8ServerName ^7" + serverName + ", ^8Status ^1offline^8, ReactionTime ^7na^8ms, DriversCount ^7na",4500);
                }
                else
                    driver.AddMessageMiddle("^8Status - serverName(^7"+serverName+")^8 not found.",4500);
            }
        }
        private void Rank(Driver driver, string[] args)
        {
            driver.SendRankGui(Button_Entry.NONE);
        }
        private void Test(Driver driver, string[] args)
        {
            driver.SendGui(Gui_Entry.MOTD);
        }
        private void Result(Driver driver, string[] args)
        {
            driver.SendResultGui(session.GetRaceLastResult());
        }
        private void BadWord(Driver driver, string[] args)
        {
            if (args.Length != 3)
            {
                driver.AddMessageMiddle("^2Usage: ^5!badword ^3word^7/^3flag^7=^30^2,^31^2,^32^2,^33 ^1SpaceChar^7=^3%", DISPLAY_TIME);
                return;
            }
            Program.dlfssDatabase.Lock();
            {
                Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `bad_word` VALUES('"+ConvertX.SQLString(args[1].Replace("%", " "))+"','"+args[2]+"') ON DUPLICATE KEY UPDATE `word`='"+ConvertX.SQLString(args[1].Replace("%"," "))+"',`mask`='"+ConvertX.SQLString(args[2])+"'");
                ChatModo.Initialize();
            }
            Program.dlfssDatabase.Unlock();
            
            driver.AddMessageMiddle("^2Add^7/^2Update ^3badword ^7:^2 '^3"+args[1].Replace("%"," ")+"^2' with flag^7=^3"+args[2], DISPLAY_TIME);
        }
        private void EndRace(Driver driver, string[] args)
        {
            driver.ISession.EndRace();
        }
        private void SearchRace(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!searchrace ^3$carPrefix.", DISPLAY_TIME);
                return;
            }
            if(args[1].Length != 3)
            {
                driver.AddMessageMiddle("^7SearchRace invalid carPrefix(^3"+args[1]+"^7).", DISPLAY_TIME);
                return;  
            }
            
            string carEntry = "";
            string textToSend = "";
            uint count = 0;
            uint itrMax = Program.carTemplate.GetMaxEntry();
            RaceTemplateInfo raceInfo = null;
            CarTemplateInfo carInfo = null;
            for(uint itr = 0; itr < itrMax; itr++)
            {
                carInfo = Program.carTemplate.GetEntry(itr);
                if(carInfo != null && carInfo.NamePrefix.ToLowerInvariant() == args[1].ToLowerInvariant())
                {
                    carEntry = carInfo.Entry.ToString();
                    break;
                }
            }
            if (carEntry == "")
            {
                driver.AddMessageMiddle("^7SearchRace not found carPrefix(^3"+args[1]+"^7).", DISPLAY_TIME);
                return;
            }
            
            itrMax = Program.raceTemplate.GetMaxEntry();
            string[] carList;
            for(uint itr = 0; itr < itrMax; itr++)
            {
                raceInfo = Program.raceTemplate.GetEntry(itr);
                if(raceInfo == null)
                    continue;
                
                carList = raceInfo.CarEntryAllowed.Split(new char[]{' '},StringSplitOptions.RemoveEmptyEntries);
                foreach(string entry in carList)
                {
                    string restriction = "";
                    if(entry == carEntry)
                    {
                        if(count == 2)
                        {
                            count = 1;
                            textToSend = textToSend.Substring(0,textToSend.Length-15);
                            textToSend += "\r\n";
                            restriction = "";
                            if(raceInfo.RestrictionJoinEntry != 0)
                                restriction = " ^3-> ^7"+Program.restrictionJoin.GetEntry(raceInfo.RestrictionJoinEntry).Description;
                            textToSend += "^7" + raceInfo.Entry + " ^3-> ^7" + raceInfo.Description +restriction+"   ^2|   ";
                        }
                        else
                        {
                            restriction = "";
                            if(raceInfo.RestrictionJoinEntry != 0)
                                restriction = " ^3-> ^7" + Program.restrictionJoin.GetEntry(raceInfo.RestrictionJoinEntry).Description;
                            textToSend += "^7" + raceInfo.Entry + " ^3-> ^7" + raceInfo.Description + restriction + "   ^2|   "; 
                            count++;
                        }
                    }
                }
            }
            if (textToSend != "")
            {
                textToSend = textToSend.Substring(0,textToSend.Length-9);
                driver.SendUpdateGui(Gui_Entry.TEXT, textToSend);
            }
            else
                driver.AddMessageMiddle("^7SearchRace no race found that has carPrefix(^3" + args[1] + "^7).", DISPLAY_TIME);
        }
        private void LoadRace(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!loadrace ^3#raceEntry", DISPLAY_TIME);
                return;
            }
            ushort entry = 0;
            try{entry = Convert.ToUInt16(args[1]);}
            catch(Exception)
            {
                driver.AddMessageMiddle("^1Invalid track entry : " + args[1], DISPLAY_TIME);
                return;
            }
            session.LoadRace(entry);
        }
        private void YellowTime(Driver driver, string[] args)
        {
            if (args.Length != 2)
            {
                driver.AddMessageMiddle("^7Invalid parameter count, Usage: ^2!yellowtime ^3#timeMaxSec", DISPLAY_TIME);
                return;
            }
            uint value = 0;
            try { value = Convert.ToUInt32(args[1]); }
            catch (Exception)
            {
                driver.AddMessageMiddle("^1Invalid timeMax value : " + args[1], DISPLAY_TIME);
                return;
            }
            if(value < 3)
            {
                driver.AddMessageMiddle("^1TimeMaxSec must be ^3> ^1then ^33^7.", DISPLAY_TIME);
                return; 
            }
            session.SetTimeYellowMax(value*1000);
            driver.AddMessageMiddle("^3Yellow time max is now : ^7" + value.ToString()+ "^3 seconds.", DISPLAY_TIME);
        }
        #endregion
    }
}
