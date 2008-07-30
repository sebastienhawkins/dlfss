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
    using Drive_LFSS.Script_;

    public abstract class Car : Licence
    {
        public Car(ref Session _session) : base(ref _session)
        {
            //Object
            session = _session;

            //Packet Data
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

            //Game Feature
            timerAcceleration_0_100 = 0;
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
        public void ProcessCarInformation(CarInformation _carInformation)
        {
            trackNode = _carInformation.trackNode;
            lapNumber = _carInformation.lapNumber;
            position = _carInformation.position;
            carFlag = _carInformation.carFlag;
            posX = _carInformation.posX;
            posY = _carInformation.posY;
            posZ = _carInformation.posZ;

            speed = _carInformation.speed;
            if (speed < 1 && timerAcceleration_0_100 != 0)
                timerAcceleration_0_100 = 1;

            direction = _carInformation.direction;
            heading = _carInformation.heading;
            angleVelocity = _carInformation.angleVelocity;

            //base.Init(_packet);
        }

        //Object
        private Session session;
        private iScriptCar script;
        
        //Packet data
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
        private ushort direction;
        private ushort heading;
        private short angleVelocity;

        //GameFeature
        private uint timerAcceleration_0_100;

        public byte CarId
        {
            get { return carId; }
        }
        #region Update(uint diff)
        new protected virtual void update(uint diff)
        {
            //Car Update Feature Process... For Car
            if (speed > 0 && timerAcceleration_0_100 != 0)
            {
                timerAcceleration_0_100 += diff;
                if (SpeedToKmh() > 99.9d)
                    Acceleration_0_100();
            }
            //Script.CarFinishRace((Driver)this);
            //session.log("");
            base.update(diff);
        }
        #endregion

        #region Game Feature Function

        public void FinishRace()
        {
            if(script.CarFinishRace(this))
                return;
        }
        public void LeaveRace(PacketPLL _packet)
        {
            carId = 0;
        }
        private void Acceleration_0_100()
        {
            //If Script then don't do normal Process!
            if (script.CarAcceleration_0_100(this))
            {
                timerAcceleration_0_100 = 0;
                return;
            }

            session.log.gameFeature(((Driver)this).prDriverName + ", Done  0-100Km/h In: " + (timerAcceleration_0_100 - (uint)session.GetReactionTime()));
            timerAcceleration_0_100 = 0;
        }
        #endregion

        #region Tool

        //Speed
        private double SpeedToKmh()
        {
            return speed / 327.68 * 3.6;
        }

        //X,Y,Z Coordonate
        private double PosXToCoord()
        {
            return posX / 65536.0d;
        }
        private double PosYToCoord()
        {
            return posY / 65536.0d;
        }
        private double PosZToCoord()
        {
            return posZ / 65536.0d;
        }

        //Trajectoire / Direction / Velocity
        private double DirectionToTrajectoryAngle()
        {
            return direction * 180.0d / 32768.0d;
        }
        private double HeadingToOnPathOrientation()
        {
            return heading * 180.0d / 32768.0d;
        }
        private double AngleVelocityToHeadindVelovity()
        {
            return angleVelocity * 180.0d / 8192.0d;
        }

        #endregion
    }
}
