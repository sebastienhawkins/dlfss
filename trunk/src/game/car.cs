namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;

    abstract class Car : Licence
    {
        protected Car() : base()
        {
            carId = 0;
            plate = "";
            skin = "";
            addedMass = 0;
            addedIntakeRestriction = 0;
            passenger = 0;
            tyreFrontLeft = 0;
            tyreFrontRight = 0;
            tyreRearLeft = 0;
            tyreRearRight = 0;
        }
        new protected void Init(PacketNCN _packet)
        {
            base.Init(_packet);
        }
        new protected void Init(PacketNPL _packet)
        {
            carId = _packet.carId;
            plate = _packet.carPlate;
            addedIntakeRestriction = _packet.addedIntakeRestriction;
            addedMass = _packet.addedMass;
            passenger = _packet.passenger;
            tyreFrontLeft = _packet.tyreFrontLeft;
            tyreFrontRight = _packet.tyreFrontRight;
            tyreRearLeft = _packet.tyreRearLeft;
            tyreRearRight =_packet.tyreRearRight;

            base.Init(_packet);
        }
        public void Init(CarInformation _carInformation)
        {
            trackNode = _carInformation.trackNode;
            lapNumber = _carInformation.lapNumber;
            position = _carInformation.position;
            carFlag = _carInformation.carFlag;
            posX = _carInformation.posX;
            posY = _carInformation.posY;
            posZ = _carInformation.posZ;
            speed = _carInformation.speed;
            Direction = _carInformation.direction;
            heading = _carInformation.heading;
            angleVelocity = _carInformation.angleVelocity;




            //base.Init(_packet);
        }
        public void Init(PacketPLL _packet)
        {
            carId = 0;
        }

        protected virtual void update(uint diff)
        {
            //Car Update Feature Process... For Car

            base.update(diff);
        }
        
        private byte carId;
        private string plate;
        private string skin;
        private byte addedMass;
        private byte addedIntakeRestriction;
        private byte passenger;
        private Car_Tyres tyreFrontLeft;
        private Car_Tyres tyreFrontRight;
        private Car_Tyres tyreRearLeft;
        private Car_Tyres tyreRearRight;

        private ushort trackNode;
        private ushort lapNumber;
        private byte position;
        private Car_Flag carFlag;
        private int posX;
        private int posY;
        private int posZ;
        private ushort speed;
        private ushort Direction;
        private ushort heading;
        private short angleVelocity;

        public byte prCarId
        {
            get { return carId; }
        }
	}
}
