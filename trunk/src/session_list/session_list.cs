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
namespace Drive_LFSS.Session_
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.InSim_;
    using Drive_LFSS.Server_;
    using Drive_LFSS.Game_;
    using Drive_LFSS.Database_;


    public static class SessionList //Must become compatible with all Session type: ServerInSim, ClientOutGauge, ... Im not aware of all....
    {
        public struct SessionStruct
        {//(/*Racing_Flag.InSim_Flag.ISF_MSO_COLS |*/ InSim_Flag.ISF_MCI), '$', 10000, "dexxa", "Aleajecta S2", 5)))
            public SessionStruct(ushort _sessionId, InSimSetting _inSimSetting)
            {
                sessionId = _sessionId;
                inSimSetting = _inSimSetting;
                session = new Session(sessionId, inSimSetting);
            }
            public ushort sessionId;
            public InSimSetting inSimSetting;
            public Session session;
        }

        //ServerId To "Server"
        public static Dictionary<ushort, SessionStruct> sessionList = new Dictionary<ushort, SessionStruct>();

        public static void LoadServerConfig( )
        {
            sessionList.Add(5, new SessionStruct(5, new InSimSetting("91.121.7.73", 20003, 20003, InSim_Flag.ISF_MSO_COLS | InSim_Flag.ISF_MCI, '$', 10000, "yourpass", "DriveLFSS", 5)));
            sessionList.Add(1, new SessionStruct(1, new InSimSetting("67.212.66.26", 30001, 30001, InSim_Flag.ISF_MSO_COLS | InSim_Flag.ISF_MCI, '$', 10000, "dexxa", "DriveLFSS", 5)));
            sessionList.Add(2, new SessionStruct(2, new InSimSetting("67.212.66.26", 29999, 29999, InSim_Flag.ISF_MSO_COLS | InSim_Flag.ISF_MCI, '$', 10000, "dexxa", "DriveLFSS", 5)));
            sessionList.Add(3, new SessionStruct(3, new InSimSetting("67.212.66.26", 29989, 29989, InSim_Flag.ISF_MSO_COLS | InSim_Flag.ISF_MCI, '$', 10000, "dexxa", "DriveLFSS", 5)));
            sessionList.Add(4, new SessionStruct(4, new InSimSetting("67.212.66.26", 30000, 30000, InSim_Flag.ISF_MSO_COLS | InSim_Flag.ISF_MCI, '$', 10000, "dexxa", "DriveLFSS", 5)));
        }

        public static void update(uint diff)
        {
            foreach (KeyValuePair<ushort, SessionStruct> keyPair in sessionList)
            {
                if (keyPair.Value.session.connectionRequest)
                {
                    keyPair.Value.session.connectionRequest = false;

                    if (keyPair.Value.session.IsSocketStatus(InSim_Socket_State.INSIM_SOCKET_DISCONNECTED))
                        ConnectToServerId(keyPair.Key);
                    continue;
                }

                if ( keyPair.Value.session.IsSocketStatus(InSim_Socket_State.INSIM_SOCKET_DISCONNECTED) )
                    continue;

                keyPair.Value.session.update(diff);
            }
        }
        public static void ConnectToServerId(ushort serverId)
        {
            Thread ThreadConnectionProcess = new Thread(new ThreadStart(sessionList[serverId].session.connect));
            ThreadConnectionProcess.Start();
        }
        public static void exit()
        {
            foreach (KeyValuePair<ushort, SessionStruct> keyPair in sessionList)
            {
                keyPair.Value.session.exit();
            }
        }
    }
}
