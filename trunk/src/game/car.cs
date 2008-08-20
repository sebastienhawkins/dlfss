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

namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Script_;
    using Drive_LFSS.Log_;

    public abstract class Car : Licence, ICar
    {
        public Car() : base()
        {
            //game Feature
            featureAcceleration_0_100 = new FeatureAcceleration_0_100();

            //Packet Data
            carId = 0;
            carPlate = "";
            skin = "";
            addedMass = 0;
            addedIntakeRestriction = 0;
            passenger = 0;
            tyreFrontLeft = 0;
            tyreFrontRight = 0;
            tyreRearLeft = 0;
            tyreRearRight = 0;
        }
        ~Car()
        {
        }
        new protected void Init(PacketNCN _packet)
        {
            base.Init(_packet);
        } //When Connection
        new protected void Init(PacketNPL _packet)
        {
            carId = _packet.carId;
            carName = _packet.carName;
            carPlate = _packet.carPlate;
            addedIntakeRestriction = _packet.addedIntakeRestriction;
            addedMass = _packet.addedMass;
            passenger = _packet.passenger;
            tyreFrontLeft = _packet.tyreFrontLeft;
            tyreFrontRight = _packet.tyreFrontRight;
            tyreRearLeft = _packet.tyreRearLeft;
            tyreRearRight =_packet.tyreRearRight;
            base.Init(_packet);
        }  //When joining Race
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
            speedKhm = SpeedToKmh();

            direction = _carInformation.direction;
            heading = _carInformation.heading;
            angleVelocity = _carInformation.angleVelocity;


            featureAcceleration_0_100.Update(this);

            //base.Init(_packet);
        }

        //Packet data
        private byte carId;
        private string carName;
        private string carPlate;
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
        private double speedKhm;
        private ushort direction;
        private ushort heading;
        private short angleVelocity;

        //Game Feature
        private FeatureAcceleration_0_100 featureAcceleration_0_100;

    #region Update
        new protected virtual void update(uint diff)
        {
            //Script.CarFinishRace((Driver)this);
            //session.log("");
            base.update(diff);
        }
    #endregion

    #region Game Feature
        private class FeatureAcceleration_0_100
        {
            public FeatureAcceleration_0_100()
            {
                started = false;
                startTime = 0;
            }
            private bool started;
            private long startTime;

            public void Update(Car car)
            {
                if (car.speedKhm < 0.1d && (!started || startTime != 0))
                    Start();

                else if (car.speedKhm > 0.1d && started && startTime == 0)
                    startTime = DateTime.Now.Ticks;

                else if (car.speedKhm > 99.9d && started)
                    Sucess(ref car);
            }
            private void Start()
            {
                startTime = 0;
                started = true;
            }
            private void End()
            {
                startTime = 0;
                started = false;
            }
            private void Sucess(ref Car car)
            {
                long timeElapsed = (DateTime.Now.Ticks - startTime) / 10000;
                double finalAccelerationTime = (((double)timeElapsed - (double)Math.Abs(((Driver)car).Session.GetReactionTime())) / 1000.0d);

                Log.feature(((Driver)car).DriverName + ", Done  0-100Km/h In: " + finalAccelerationTime + "sec.\r\n");

                End();


                if (((Driver)car).Session.script.CarAcceleration_0_100((ICar)car, finalAccelerationTime))
                    return;

                //Normal Process
                ((Driver)car).SendMessage("^7 0-100Km/h In: ^2" + finalAccelerationTime + " ^0 sec.");
            }
        }
        public void FinishRace()
        {
            if(((Driver)this).Session.script.CarFinishRace((ICar)this))
                return;
        }
        public void LeaveRace(PacketPLL _packet)
        {
            carId = 0;
        }
    #endregion

    #region Script Interface

        public byte CarId
        {
            get { return carId; }
        }
        public string CarName
        {
            get { return carName; }
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
