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
    using Definition_;
    using Packet_;
    using Script_;
    using Storage_;
    using Log_;

    //Maybe flush this object and pout everything inside Driver, let see after making TOC
    public abstract class Licence : Button, ILicence
	{
        public Licence() : base()
        {
        }
        ~Licence() 
        {
            if (true == false) { }
        }
        protected void Init(PacketNCN _packet)
        {
            licenceName = _packet.licenceName;
            licenceId = _packet.tempLicenceId;
           
            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";
        }
        protected void Init(PacketNPL _packet)
        {
            if (licenceId != _packet.tempLicenceId)
            {
                Log.error("Licence.Init(PacketNPL _packet), current licenceId was not same as packet licenceId, LicenceName: "+licenceName+".\r\n");
                licenceId = _packet.tempLicenceId;
            }

            if ((_packet.driverTypeMask & Driver_Type_Flag.DRIVER_TYPE_AI) > 0)
                licenceName = "AI";
        }
        
        protected string licenceName = "";
        private byte licenceId = 0;
        private byte unkFlag = 0;
        private Leave_Reason quitReason = Leave_Reason.LEAVE_REASON_DISCONNECTED;

        new protected virtual void update(uint diff)
        {
            //Don't see any feature that can be implement here... but futur will tell
            base.update(diff);
        }

        public string LicenceName 
        { 
            set { licenceName = value; } 
            get { return licenceName; } 
        }
        public byte LicenceId 
        { 
            set { licenceId = value; } 
            get { return licenceId; } 
        }
        protected byte UnkFlag
        {
            get { return unkFlag; }
            set { unkFlag = value; }
        }
        protected Leave_Reason QuitReason 
        { 
            set { quitReason = value; } 
            get { return quitReason; } 
        } 
	}
}