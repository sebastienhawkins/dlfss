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
using System.IO;
using System.Text.RegularExpressions;

namespace Drive_LFSS.Config_
{
    using Drive_LFSS.Log_;

    sealed class Config
    {
        Config()
        {
        }
        ~Config()
        {
            if (true == false) { }
        }
        private static Dictionary<string, object> confValue = new Dictionary<string, object>();
        private static Regex rgxIdentifier = new Regex(@"^([a-z0-9]+){1}\.{0,1}([a-z0-9]+){0,1}\.{0,1}([a-z0-9]+){0,1}\.{0,1}([a-z0-9]+){0,1}\s*=\s*([^\s#\r\n]+[^#\r\n]*[^\s#\r\n]{1}|[^\s#\r\n]*){0,1}", RegexOptions.IgnoreCase);
        private void ReadToArray(string confFile)
        {
            // This is not garbage collected, if not into class compilant + good destructor call.
            // Important to call streamReader.Dispose() if not into a [assembly: ClSCompliant(true)]
            StreamReader streamReader = new StreamReader(confFile);

            string lineReaded = "";

            while (!streamReader.EndOfStream)
            {
                lineReaded = streamReader.ReadLine();

                Match match = rgxIdentifier.Match(lineReaded);

                //sLog.debug(match.Groups[5] + "\r\n");
                //continue;

                if (match.Success)
                {
                    if (match.Groups[4].Value != "")
                    {
                        if (!confValue.ContainsKey(match.Groups[1].Value))
                            confValue.Add(match.Groups[1].Value, new Dictionary<string, object>());
                        if (!((Dictionary<string, object>)confValue[match.Groups[1].Value]).ContainsKey(match.Groups[2].Value))
                            ((Dictionary<string, object>)confValue[match.Groups[1].Value]).Add(match.Groups[2].Value, new Dictionary<string, object>());
                        if (!((Dictionary<string, object>)((Dictionary<string, object>)confValue[match.Groups[1].Value])[match.Groups[2].Value]).ContainsKey(match.Groups[3].Value))
                            ((Dictionary<string, object>)((Dictionary<string, object>)confValue[match.Groups[1].Value])[match.Groups[2].Value]).Add(match.Groups[3].Value, new Dictionary<string, object>());
                        if (!((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[match.Groups[1].Value])[match.Groups[2].Value])[match.Groups[3].Value]).ContainsKey(match.Groups[4].Value))
                            ((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[match.Groups[1].Value])[match.Groups[2].Value])[match.Groups[3].Value]).Add(match.Groups[4].Value, match.Groups[5].Value);
                        continue;
                    }
                    if (match.Groups[3].Value != "")
                    {
                        if (!confValue.ContainsKey(match.Groups[1].Value))
                            confValue.Add(match.Groups[1].Value, new Dictionary<string, object>());
                        if (!((Dictionary<string, object>)confValue[match.Groups[1].Value]).ContainsKey(match.Groups[2].Value))
                            ((Dictionary<string, object>)confValue[match.Groups[1].Value]).Add(match.Groups[2].Value, new Dictionary<string, object>());
                        if (!((Dictionary<string, object>)((Dictionary<string, object>)confValue[match.Groups[1].Value])[match.Groups[2].Value]).ContainsKey(match.Groups[3].Value))
                            ((Dictionary<string, object>)((Dictionary<string, object>)confValue[match.Groups[1].Value])[match.Groups[2].Value]).Add(match.Groups[3].Value, match.Groups[5].Value);
                        continue;
                    }
                    if (match.Groups[2].Value != "")
                    {
                        if (!confValue.ContainsKey(match.Groups[1].Value))
                            confValue.Add(match.Groups[1].Value, new Dictionary<string, object>());
                        if (!((Dictionary<string, object>)confValue[match.Groups[1].Value]).ContainsKey(match.Groups[2].Value))
                            ((Dictionary<string, object>)confValue[match.Groups[1].Value]).Add(match.Groups[2].Value, match.Groups[5].Value);
                        continue;
                    }
                    if (match.Groups[1].Value != "")
                    {
                        if (!confValue.ContainsKey(match.Groups[1].Value))
                            confValue.Add(match.Groups[1].Value, match.Groups[5].Value);
                        continue;
                    }
                    //sLog.debug((string)((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[match.Groups[0].Value])[match.Groups[1].Value])[match.Groups[2].Value])[match.Groups[3].Value]);
                }
            }
            //streamReader.Dispose();
        }
        public static int GetIntValue(params string[] args)
        {
            if (args.Length == 4)
            {
                if (confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1])
                    && ((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]]).ContainsKey(args[2])
                    && ((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]])[args[2]]).ContainsKey(args[3]))
                    return Convert.ToInt32(((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]])[args[2]])[args[3]]);
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "." + args[2] + "." + args[3] + "\r\n");
                return -1;
            }
            if (args.Length == 3)
            {
                if (confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1])
                    && ((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]]).ContainsKey(args[2]))
                    return Convert.ToInt32(((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]])[args[2]]);
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "." + args[2] + "\r\n");
                return -1;
            }
            if (args.Length == 2)
            {
                if (confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1]))
                    return Convert.ToInt32(((Dictionary<string, object>)confValue[args[0]])[args[1]]);
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "\r\n");
                return -1;
            }
            if (args.Length == 1)
            {
                if (confValue.ContainsKey(args[0]))
                    return Convert.ToInt32(confValue[args[0]]);
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "\r\n");
                return -1;
            }
            return -1;
        }
        public static string GetStringValue(params string[] args)
        {
            if (args.Length == 4)
            {
                if (confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1])
                    && ((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]]).ContainsKey(args[2])
                    && ((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]])[args[2]]).ContainsKey(args[3]))
                    return (string)((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]])[args[2]])[args[3]];
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "."+args[2]+"."+args[3]+"\r\n");
                return null;
            }
            if (args.Length == 3)
            {
                if (confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1])
                    && ((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]]).ContainsKey(args[2]))
                    return (string)((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]])[args[2]];
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "."+args[2]+"\r\n");
                return null;
            }
            if (args.Length == 2)
            {
                if (confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1]))
                    return (string)((Dictionary<string, object>)confValue[args[0]])[args[1]];
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "\r\n");
                return null;
            }
            if (args.Length == 1)
            {
                if (confValue.ContainsKey(args[0]))
                    return (string)confValue[args[0]];
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "\r\n");
                return null;
            }
            return null;
        }
        public static List<string> GetIdentifierList(params string[] args)
        {
            List<string> temp = new List<string>();
            if (args.Length == 3)
            {
                if (confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1])
                    && ((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]]).ContainsKey(args[2]))
                {
                    Dictionary<string, object>.Enumerator itr = ((Dictionary<string, object>)((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]])[args[2]]).GetEnumerator();
                    while (itr.MoveNext())
                        temp.Add(itr.Current.Key);
                }
                    
                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "." + args[2] + "\r\n");
                return temp;
            }
            if (args.Length == 2)
            {
                if ( confValue.ContainsKey(args[0]) && ((Dictionary<string, object>)confValue[args[0]]).ContainsKey(args[1]) )
                {
                    Dictionary<string, object>.Enumerator itr = ((Dictionary<string, object>)((Dictionary<string, object>)confValue[args[0]])[args[1]]).GetEnumerator();
                    while (itr.MoveNext())
                        temp.Add(itr.Current.Key);
                }

                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "\r\n");
                return temp;
            }
            if (args.Length == 1)
            {
                if (confValue.ContainsKey(args[0]))
                {
                    Dictionary<string, object>.Enumerator itr = ((Dictionary<string, object>)confValue[args[0]]).GetEnumerator();
                    while (itr.MoveNext())
                        temp.Add(itr.Current.Key);
                }

                else
                    Log.error("Configuration unable to find value for key: " + args[0] + "." + args[1] + "\r\n");
                return temp;
            }
            return null;
        }
        public static bool Initialize(string confFile)
        {
            if (!File.Exists(confFile))
            {
                Log.error("Unable to find the config file: " + confFile + "\r\n");
                return false;
            }
            Config config = new Config();
            config.ReadToArray(confFile);
            return true;
        }
    }
}