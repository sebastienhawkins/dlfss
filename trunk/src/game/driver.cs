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

using System.Data;

namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Script_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Config_;
    using Drive_LFSS.Session_;

    public sealed class Driver : Car, IDriver
    {
        public Driver(Session _session) : base()
        {
            adminFlag = false;
            driverName = "";
            driverModel = 0;
            driverMask = 0;
            driverTypeMask = 0;
            session = _session;
            guid = 0;
            configMask = 0;
        }
        ~Driver()
        {
        }
        public static void ConfigApply()
        {
            SAVE_INTERVAL = (uint)Config.GetIntValue("Interval", "DriverSave");
        }

        //Loaded From DB
        private uint guid;
        private uint configMask;

        private void LoadFromDB()
        {
            IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `guid`,`config_mask` FROM `driver` WHERE `licence_name`='"+licenceName+"' AND `driver_name`='"+driverName+"'");
            if (reader.Read())
            {
                guid = (uint)reader.GetInt32(0);
                //...
            }

            reader.Dispose();
        }
        private void SaveToDB()
        {
            Program.dlfssDatabase.ExecuteNonQuery("DELETE FROM `driver` WHERE `guid`=" + guid);
            Program.dlfssDatabase.ExecuteNonQuery("INSERT INTO `driver` (`guid`,`licence_name`,`driver_name`,`config_mask`,`last_connection_time`) VALUES (" + guid + ", '" + licenceName + "','" + driverName + "', " + configMask + ", " + (System.DateTime.Now.Ticks / 10000000) + ")");
            driverSaveInterval = 0;
            Log.debug(session.GetSessionNameForLog()+" DriverGuid: " + guid + ", DriverName: " + driverName + ", licenceName:" + licenceName + ", saved To Database.\r\n");
        }
        private bool SetNewGuid()
        {
            IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT MAX(`guid`) FROM `driver`");
            if (reader.Read())
            {
                guid = reader.IsDBNull(0) ? 1 : (uint)reader.GetInt32(0)+1;
                reader.Dispose();
                return true;
            }
            reader.Dispose();
            return false;
        }
        
        //Loaded From packet
        private bool adminFlag;
        private string driverName;
        private byte driverModel;
        private byte driverGender;
        private Driver_Flag driverMask;
        private Driver_Type_Flag driverTypeMask;
        private Session session;

        new public void Init(PacketNCN _packet)
        {
            adminFlag = _packet.adminStatus > 0 ? true : false;
            driverName = _packet.driverName;
            driverTypeMask = _packet.driverTypeMask;
            //_packet.total;// What is Total????
 
            base.Init(_packet);

            if (IsBot())
                return;

            LoadFromDB();
            if (guid == 0)
            {
                if (SetNewGuid())
                    SaveToDB();
                else
                    Log.error("Error When Creating a New GUID for licenceName: " + licenceName + ", driverName: " + driverName + "\r\n");
            }
        }
        new public void Init(PacketNPL _packet)
        {
            if (driverName != _packet.driverName)    //I think should be a check != null && != then Error... like custom cheater packet
                driverName = _packet.driverName;

            driverModel = _packet.driverModel;

            if (driverTypeMask != _packet.driverTypeMask)
                driverTypeMask = _packet.driverTypeMask;

            driverMask = _packet.driverMask;

            //_packet.SName; //What is that???
            //_packet.numberInRaceCar_NSURE // What is That???

            base.Init(_packet);

            if (IsBot())
                return;
            if (guid == 0)
            {
                LoadFromDB();
                if (guid == 0)
                {
                    if (SetNewGuid())
                        SaveToDB();
                    else
                        Log.error("Error When Creating a New GUID for licenceName: " + licenceName + ", driverName: " + driverName + "\r\n");
                }
            }

        }
        

        private static uint SAVE_INTERVAL = 60000;
        private uint driverSaveInterval = 0;
        new public void update(uint diff) 
        {
            if (!IsBot())
            {
                if ((driverSaveInterval += diff) >= SAVE_INTERVAL) //Into Server.update() i use different approch for Timer Solution, so just see both and take the one you love more.
                    SaveToDB();
                //...
            }

            base.update(diff);
        }

        public Session Session
        {
            get { return session; }
        }

        public void SendMessage(string message)
        {
            //Serve no Purpose sending a Message to a Bot.
            if (IsBot()) return;

            Session.AddToTcpSendingQueud
            (
                new Packet(Packet_Size.PACKET_SIZE_MTC, Packet_Type.PACKET_MTC_CHAT_TO_LICENCE,
                    new PacketMTC(0, LicenceId, message)));
        }
        public bool AdminFlag
        {
            get { return adminFlag; }
            //set { adminFlag = value; }
        }
        public string DriverName
        {
            get { return driverName; }
           // set { driverName = value; }
        }
        public bool IsBot()
        {
            return ((Driver_Type_Flag.DRIVER_TYPE_AI & driverTypeMask) == Driver_Type_Flag.DRIVER_TYPE_AI || LicenceId == 0);
        }
    }
}
