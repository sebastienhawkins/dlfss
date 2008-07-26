namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;

    abstract class Licence
	{
        protected Licence()
        {
            licenceName = "";
            licenceId = 0;
            unkFlag = 0;
            quitReason = 0;
        }
        protected void Init(PacketNCN _packet)
        {
            licenceName = _packet.licenceName;
            licenceId = _packet.tempLicenceId;
        }
        protected void Init(PacketNPL _packet)
        {
            if (licenceName != _packet.licenceName)
                licenceName = _packet.licenceName;
            if (licenceId != _packet.tempLicenceId)
                licenceId = _packet.tempLicenceId;
        }

        protected virtual void update(uint diff)
        {
            //Don't see any feature that can be implement here... but futur will tell
        }

        private string licenceName;
        private byte licenceId;
        private byte unkFlag;
        private Leave_Reason quitReason;

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