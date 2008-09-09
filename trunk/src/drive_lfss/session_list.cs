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

namespace Drive_LFSS
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.InSim_;
    using Drive_LFSS.Config_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Session_;

    public class SessionList //Must become compatible with all Session type: ServerInSim, ClientOutGauge, ... Im not aware of all....
    {
        public static Dictionary<string, Session> sessionList = new Dictionary<string, Session>();

        public static void ConfigApply()
        {
            List<string> lfsServer = Config.GetIdentifierList("LFSServer");

            List<string>.Enumerator itr = lfsServer.GetEnumerator();
            while (itr.MoveNext())
            {
                string[] serverOptions = Config.GetStringValue("LFSServer", itr.Current, "ConnectionInfo").Split(';');
                if (serverOptions.Length != 8)
                {
                    Log.error("Configuration Error for Servername: " + itr.Current + ", Bad Option Count, Must be 8.\r\n");
                    continue;
                }
                InSimSetting inSimSetting = new InSimSetting(itr.Current, serverOptions[0], Convert.ToUInt16(serverOptions[1]), serverOptions[2], 
                                                                 Convert.ToChar(serverOptions[3]), serverOptions[4], (InSim_Flag)Convert.ToUInt32(serverOptions[5]), 
                                                                  Convert.ToUInt16(serverOptions[6]), Convert.ToUInt16(serverOptions[7]));

                inSimSetting.requestInterval = (ushort)(inSimSetting.requestInterval < 10 ? 10 : inSimSetting.requestInterval);
                inSimSetting.networkInterval = (uint)(inSimSetting.networkInterval < 1 ? 1 : inSimSetting.networkInterval);

                if (inSimSetting.password.Length > 16)
                    Log.error(inSimSetting.serverName + " bad Configuration For: password must be max 16 characters long.\r\n");
                else if (inSimSetting.appName.Length > 16)
                    Log.error(inSimSetting.serverName + " bad Configuration For: appName must be max 16 characters long.\r\n");
                else if (inSimSetting.port < 1024)
                    Log.error(inSimSetting.serverName + " bad Configuration For: Port must be greater 1024.\r\n");
                else
                {
                    Session session = new Session(itr.Current, inSimSetting);
                    session.ConfigApply();
                    sessionList.Add(itr.Current,session);
                }
            }
        }

        private static uint TimerReconnection = 30000;
        public static void update(uint diff)
        {
            if (TimerReconnection < diff)
                TimerReconnection = 30000;
            else
                TimerReconnection -= diff;

            foreach (KeyValuePair<string, Session> keyPair in sessionList)
            {
                if (!keyPair.Value.IsConnected() )
                {
                    if (TimerReconnection == 30000 && !keyPair.Value.IsConnecting())
                        keyPair.Value.DoConnect();
                    continue;
                }
                keyPair.Value.update(diff);
            }
        }
        //Move this to Drive_LFSS.Exit(bool)
        public static void DisconnectAll()
        {
            foreach (KeyValuePair<string, Session> keyPair in sessionList)
            {
                keyPair.Value.Disconnect();
            }
        }
    }
}
