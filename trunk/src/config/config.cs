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
namespace Drive_LFSS.Config_
{
    using System.Collections.Generic;
    using System.IO;
    using System;

    public static class Config
    {
        public struct ServerConfigStruct
        {
            public ServerConfigStruct(string _serverName, string _serverIP, ushort _portNumber, string _adminPass, char _commandPrefix, byte _insimOptionMask, ushort _netUpdateInterval, ushort _serverUpdateInterval)
            {
                serverName = _serverName;
                serverIP = _serverIP;
                portNumber = _portNumber;
                adminPass = _adminPass;
                commandPrefix = _commandPrefix;
                insimOptionMask = _insimOptionMask;
                netUpdateInterval = _netUpdateInterval;
                serverUpdateInterval = _netUpdateInterval;
            }
            private string serverName;
            private string serverIP;
            private ushort portNumber;
            private string adminPass;
            private char commandPrefix;
            private byte insimOptionMask;
            private ushort netUpdateInterval;
            private ushort serverUpdateInterval;
        }

        private const string CONFIG_FILE = "dlfss.cfg";
        private const string CONFIG_VERSION = "07282008";

        private static byte processPriority = 0;
        private static uint playerSaveInterval = 0;
        private static uint DLFSSSUpdateRate = 0;
        private static int logDisable = 0; //mask variable

        private static Dictionary<int, ServerConfigStruct> serverList = new Dictionary<int,ServerConfigStruct>();

        public static bool Initialize()
        {
            if (!File.Exists(CONFIG_FILE))
            {
                Program.log.error("Unable to find the config file, path : " + CONFIG_FILE + "\r\n");
                return false;
            }

            using (StreamReader sr = new StreamReader(CONFIG_FILE))
            {                
                ushort lineNumber = 0;
                string lineReaded = "";
                
                while (!sr.EndOfStream)
                {
                    lineReaded = sr.ReadLine();
                    lineNumber ++;

                    if (!lineReaded.StartsWith("#") && lineReaded.Trim(' ') != "")
                    {
                        if (lineReaded.IndexOf('=') != -1)
                        {
                            if (!ValidLineSetting(lineReaded, lineNumber)) { return false; }
                        }
                    }
                }
            }

            return true;
        }
        private static bool ValidLineSetting(string _line, ushort _lineNumber)
        {
            string[] args = _line.Substring(0, _line.IndexOf('=')).Trim(' ').Split(new string[] { "." }, 3, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length == 0)                        //Nothing Found
            {
                Program.log.error("Error in config file at line: " + _lineNumber + ".\r\n");
                return false;
            }
            else if (args.Length == 1)                  //Simple Config Var
            {
                switch (args[0])
                {
                    case "ConfVersion":
                    {
                        if (!ValidConfVersion(_line))
                            return false;
                    } break;
                    case "ProcessPriority":
                    {
                        if (!SetProcessPriority(_line, _lineNumber))
                            return false;
                    } break;
                    case "PlayerSaveInterval":
                    {
                        if (!SetPlayerSaveInterval(_line, _lineNumber))
                            return false;
                    } break;
                    case "DLFSSSUpdateRate":
                    {
                        if (!SetDLFSSSUpdateRate(_line, _lineNumber))
                            return false;
                    } break;
                    case "LogDisable":
                    {
                        if (!SetLogDisable(_line, _lineNumber))
                            return false;
                    } break;
                    default:
                    {
                        Program.log.error("Error in config file at line: " + _lineNumber + ", unknow config option : " + args[0] + "\r\n");
                        return false;
                    }
                }
            }
            else                                    //Config Array
            {
                switch (args[0])
                {
                    case "LFSServer":
                    {
                        if (!DispatchServerOptions(_line, _lineNumber))
                            return false;

                    } break;
                    default:
                    {
                        Program.log.error("Error in config file at line : " + _lineNumber + ", unknow config option : " + args[0] + "\r\n");
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool DispatchServerOptions(string _line, ushort _lineNumber)
        {
            string[] args = _line.Substring(0, _line.IndexOf('=')).Trim(' ').Split(new string[] { "." }, 3, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length < 3)
            {
                Program.log.error("Error in config file at line: " + _lineNumber + ".\r\n");
                return false;
            }
            switch (args[2])
            {
                case "ConnectionInfo": 
                {
                    if (!CreateServer(_line, _lineNumber, args[1]))
                        return false;
                } break;
                default:
                {
                    Program.log.error("Error in config file at line: " + _lineNumber + ", unknow config option: " + args[2] + ", For Server: "+args[1]+".\r\n");
                    return false;
                }
            }

            return true;
        }
        private static bool CreateServer(string _line, ushort _lineNumber, string _serverName)
        {
            string tempoConf = _line.Substring(_line.IndexOf('=') + 1, _line.Length - (_line.IndexOf('=') + 1)).Trim(' ');
            string[] args = tempoConf.Split(new string[] { ";" }, 7, StringSplitOptions.RemoveEmptyEntries);

            if (args.Length != 7)
            {
                Program.log.error("Error in config file at line: " + _lineNumber + ", incorrect parameter count: " + tempoConf + ".\r\n");
                return false;
            }

            try 
            {
                serverList.Add(serverList.Count + 1, new ServerConfigStruct(_serverName, args[0], Convert.ToUInt16(args[1]), args[2], Convert.ToChar(args[3]), Convert.ToByte(args[4]), Convert.ToUInt16(args[5]), Convert.ToUInt16(args[6])));
                //Program.log.normal("CreateServer name : " + _serverName + ", value : " + tempoConf + "\r\n");
                return true;
            }
            catch (Exception _exception)
            {
                Program.log.error("Error in config file at line: " + _lineNumber + ", incorrect parameter: " + tempoConf + "\r\n");
                return false;
            }
        }
        private static bool ValidConfVersion(string _line)
        {
            if (CONFIG_VERSION != (_line.Substring(_line.IndexOf('=') + 1, _line.Length - (_line.IndexOf('=') + 1)).Trim(' ')) )
                Program.log.error("Error Config File is Out of Date!, current version is : " + CONFIG_VERSION + "\r\n");
                
            return true;
        }
        private static bool SetProcessPriority(string _line, ushort _lineNumber)
        {
            string valueToConvert = _line.Substring(_line.IndexOf('=') + 1, _line.Length - (_line.IndexOf('=') + 1)).Trim(' ');
            
            try {processPriority = Convert.ToByte(valueToConvert);}
            catch (Exception _exception)
            {
                Program.log.error("Error in config file at line : " + _lineNumber + ", incorrect parameter : " + valueToConvert + "\r\n");
                return false;
            }

            return true;
        }
        private static bool SetPlayerSaveInterval(string _line, ushort _lineNumber)
        {
            string valueToConvert = _line.Substring(_line.IndexOf('=') + 1, _line.Length - (_line.IndexOf('=') + 1)).Trim(' ');

            try{playerSaveInterval = Convert.ToUInt32(valueToConvert);}
            catch (Exception _exception)
            {
                Program.log.error("Error in config file at line : " + _lineNumber + ", incorrect parameter : " + valueToConvert + "ms \r\n");
                return false;
            }

            return true;
        }
        private static bool SetDLFSSSUpdateRate(string _line, ushort _lineNumber)
        {
            string valueToConvert = _line.Substring(_line.IndexOf('=') + 1, _line.Length - (_line.IndexOf('=') + 1)).Trim(' ');
            
            try{DLFSSSUpdateRate = Convert.ToUInt32(valueToConvert);}
            catch (Exception _exception)
            {
                Program.log.error("Error in config file at line : " + _lineNumber + ", incorrect parameter : " + valueToConvert + "ms \r\n");
                return false;
            }
            return true;
        }
        private static bool SetLogDisable(string _line, ushort _lineNumber)
        {
            string valueToConvert = _line.Substring(_line.IndexOf('=') + 1, _line.Length - (_line.IndexOf('=') + 1)).Trim(' ');
            
            try{logDisable = Convert.ToInt32(valueToConvert);}
            catch (Exception _exception)
            {
                Program.log.error("Error in config file at line : " + _lineNumber + ", incorrect parameter : " + valueToConvert + "\r\n");
                return false;
            }
            return true;
        }
    }
}
