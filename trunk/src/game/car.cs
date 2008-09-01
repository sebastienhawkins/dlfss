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
    using Drive_LFSS.Storage_;

    public abstract class Car : Licence, ICar, CarMotion
    {
        public Car() : base()
        {
        }
        ~Car()
        {
            if (true == false) { }
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
            carSkin = _packet.skinName;
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
            node = _carInformation.nodeTrack;
            lapNumber = _carInformation.lapNumber;
            position = _carInformation.position;
            carFlag = _carInformation.carFlag;
            x = _carInformation.posX / 65536.0d;
            y = _carInformation.posY / 65536.0d;
            z = _carInformation.posZ / 65536.0d;

            speedMs = _carInformation.speed / 327.68d;
            speedKmh = speedMs * 3.6d;

            directionAngle = _carInformation.direction;
            headingAngle = _carInformation.heading;
            angleVelocity = _carInformation.angleVelocity;


            featureAcceleration_0_100.Update(this);

            //base.Init(_packet);
        }

        //Packet data
        private byte carId = 0;
        private string carName = "";
        private string carPlate = "";
        private string carSkin = "";
        private byte addedMass = 0;
        private byte addedIntakeRestriction = 0;
        private byte passenger = 0;
        private Car_Tyres tyreFrontLeft = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private Car_Tyres tyreFrontRight = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private Car_Tyres tyreRearLeft = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private Car_Tyres tyreRearRight = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private ushort node = 0;
        private ushort lapNumber = 0;
        private byte position = 0;
        private Car_Racing_Flag carFlag = Car_Racing_Flag.CAR_RACING_FLAG_NONE;
        private double x = 0.0d;
        private double y = 0.0d;
        private double z = 0.0d;
        private double speedMs = 0.0d;
        private double speedKmh = 0.0d;
        private double directionAngle = 0.0d;
        private double headingAngle = 0.0d;
        private double angleVelocity = 0.0d;

        //Game Feature
        private uint collisionTimer = 0;
        private FeatureAcceleration_0_100 featureAcceleration_0_100 = new FeatureAcceleration_0_100();
        private sealed class FeatureAcceleration_0_100
        {
            private bool started = false;
            private long startTime = 0;

            internal void Update(Car car)
            {
                if (car.speedMs < 0.1 && (!started || startTime != 0))
                    Start();

                else if (car.speedMs > 0.1 && started && startTime == 0) //About 0Kmh
                    startTime = DateTime.Now.Ticks;

                else if (car.speedMs > 27.777777d && started) //About 100Kmh
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
                float finalAccelerationTime = (((float)timeElapsed - 5.0f) / 1000.0f); //5.0f Should be MCI interval /2 

                Log.feature(((Driver)car).DriverName + ", Done  0-100Km/h In: " + finalAccelerationTime + "sec.\r\n");

                End();

                if (((Driver)car).Session.script.CarAcceleration_0_100((ICar)car, finalAccelerationTime))
                    return;

                //Normal Process
                ((Driver)car).SendMessage("^7 0-100Km/h In: ^2" + finalAccelerationTime + " ^0 sec.");
            }
        }

        new protected virtual void update(uint diff)
        {
            if ( collisionTimer > 0 )
            {
                if (collisionTimer > diff)
                    collisionTimer -= diff;
                else
                {
                    collisionTimer = 0;
                    ((Button)this).RemoveButton((ushort)Button_Entry.COLLISION_WARNING);
                }
            }
            base.update(diff);
        }
        public bool HasCollisionWarning()
        {
            return (collisionTimer > 0);
        }
        public void SendCollisionWarning(string text)
        {
            collisionTimer = 2000;
            ((Button)this).SendUpdateButton((ushort)Button_Entry.COLLISION_WARNING, text);
        }
        public void finishRace()
        {
            if (((Driver)this).Session.script.CarFinishRace((ICar)this))
                return;
        }
        public void leaveRace(PacketPLL _packet) //to be called when a car is removed from a race
        {
            carId = 0;
            ButtonTemplateInfo banner = Program.buttonTemplate.GetEntry((uint)Button_Entry.BANNER);
            SendUniqueButton(banner);
        }
        
        public byte CarId
        {
            get { return carId; }
        }
        public string CarName
        {
            get { return carName; }
        }
        public ushort GetNode()
        {
            return node;
        }
        public byte GetPosition()
        {
            return position;
        }

        //Speed
        public double GetSpeedMs()
        {
            return speedMs;
        }
        public double GetSpeedKmh()
        {
            return speedKmh;
        }

        //X,Y,Z Coordonate
        public double GetPosX()
        {
            return x;
        }
        public double GetPosY()
        {
            return y;
        }
        public double GetPosZ()
        {
            return z;
        }

        //Trajectoire / Direction / Velocity
        public double GetTrajectoryAngle()
        {
            return (double)directionAngle * 360.0d / 65536.0d;
        }
        public double GetHeadingAngle()
        {
            return (double)headingAngle * 360.0d / 65536.0d;
        }
        public double GetHeadindVelovity()
        {
            return angleVelocity * 360.0f / 16384.0d;
        }
    }
}
