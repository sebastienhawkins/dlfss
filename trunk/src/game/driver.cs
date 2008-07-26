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
namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;

    sealed class Driver : Car
    {
        public Driver() : base()
        {
            adminFlag = false;
            driverName = "";
            driverModel = 0;
            driverMask = 0;
            driverTypeMask = 0;
        }
        new public void Init(PacketNCN _packet)
        {
            adminFlag = _packet.adminStatus > 0 ? true : false;
            driverName = _packet.driverName;
            driverTypeMask = _packet.driverTypeMask;
            //_packet.total;// What is Total????
            
            base.Init(_packet);
        }
        new public void Init(PacketNPL _packet)
        {
            if(driverName != _packet.driverName)    //I think should be a check != null && != then Error... like custom cheater packet
                driverName = _packet.driverName;

            driverModel = _packet.driverModel;

            if (driverTypeMask != _packet.driverTypeMask)
                driverTypeMask = _packet.driverTypeMask;

            driverMask = _packet.driverMask;

            //_packet.SName; //What is that???
            //_packet.numberInRaceCar_NSURE // What is That???

            base.Init(_packet);
        }

        #region Timer
        private const uint INTERVAL_IM_A_TEST = 15000;
        private uint TIMER_IM_A_TEST = 0;
        
        #endregion

        new public void update(uint diff) 
        {
            if (TIMER_IM_A_TEST < diff) //Into Server.update() i use different approch for Timer Solution, so just see both and take the one you love more.
            {
                Program.Log.debug("Player: " + driverName + ", is a InGame Driver.\r\n");
                TIMER_IM_A_TEST = INTERVAL_IM_A_TEST;
            }
            else
                TIMER_IM_A_TEST -= diff;

            base.update(diff);
        }
        
        
        private bool adminFlag;
        private string driverName;
        private byte driverModel;
        private byte driverGender;
        private Driver_Flag driverMask;
        private Driver_Type_Flag driverTypeMask;

        public bool prAdminFlag
        {
            get { return adminFlag; }
            set { adminFlag = value; }
        }
        public string prDriverName
        {
            get { return driverName; }
            set { driverName = value; }
        }















        public bool IsBot()
        {
            return ((Driver_Type_Flag.AI & driverTypeMask) == Driver_Type_Flag.AI);
        }
    }
}
