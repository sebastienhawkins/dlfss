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
    using Definition_;
    using Packet_;
    using Script_;
    using Session_;
    using Log_;
    using Storage_;
    using PubStats_;

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
            carPrefix = _packet.carName;
            if (carId != _packet.carId)
            {
                EnterTrackFirstTime();
            }
            carId = _packet.carId;
            carPlate = _packet.carPlate;
            carSkin = _packet.skinName;
            addedIntakeRestriction = _packet.addedIntakeRestriction;
            addedMass = _packet.addedMass;
            passenger = _packet.passenger;
            tyreFrontLeft = _packet.tyreFrontLeft;
            tyreFrontRight = _packet.tyreFrontRight;
            tyreRearLeft = _packet.tyreRearLeft;
            tyreRearRight =_packet.tyreRearRight;
            //
            EnterTrack();
            
            base.Init(_packet);

        }  //When joining Race
        public void ProcessCarInformation(CarInformation _carInformation)
        {
            node = _carInformation.nodeTrack;
            lapCompleted = _carInformation.lapNumber;
            racePosition = _carInformation.position;
            carFlag = _carInformation.carFlag;
            x = _carInformation.posX / 65536.0d;
            y = _carInformation.posY / 65536.0d;
            z = _carInformation.posZ / 65536.0d;

            speedMs = _carInformation.speed / 327.68d;
            speedKmh = speedMs * 3.6d;

            tracjectory = _carInformation.direction;
            orientation = _carInformation.heading;
            orientationSpeed = _carInformation.angleVelocity;

            featureAcceleration.Update((CarMotion)this);
            featureDriftScore.Update((CarMotion)this);

            //base.Init(_packet);
        }
        public void ProcessLeaveRace(PacketPLL _packet)  //to be called when a car is removed from a race
        {
            carId = 0;
            LeaveTrack();
        }
        public void ProcessEnterGarage()                //When a car enter garage.
        {
            LeaveTrack();
        }

        private byte carId = 0;
        private string carPrefix = "";
        private string carPlate = "";
        private string carSkin = "";
        private byte addedMass = 0;
        private byte addedIntakeRestriction = 0;
        private byte passenger = 0;
        private Car_Tyres tyreFrontLeft = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private Car_Tyres tyreFrontRight = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private Car_Tyres tyreRearLeft = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private Car_Tyres tyreRearRight = Car_Tyres.CAR_TYRE_NOTCHANGED;
        private Penalty_Type currentPenality = Penalty_Type.PENALTY_TYPE_NONE;
        private ushort lapCompleted = 0;
        private ushort node = 0;
        private byte racePosition = 0;
        private Car_Racing_Flag carFlag = Car_Racing_Flag.CAR_RACING_FLAG_NONE;
        private double x = 0.0d;
        private double y = 0.0d;
        private double z = 0.0d;
        private double speedMs = 0.0d;
        private double speedKmh = 0.0d;
        private ushort tracjectory = 0;
        private ushort orientation = 0;
        private short orientationSpeed = 0;
        private bool isOnTrack = false;
        private uint collisionTimer = 0;
        private FeatureAcceleration featureAcceleration = new FeatureAcceleration();
        private FeatureDriftScore featureDriftScore = new FeatureDriftScore();
        private sealed class FeatureAcceleration
        {
            private bool started = false;
            private long startTime = 0;

            internal void Update(CarMotion car)
            {
                if (car.GetSpeedMs() < 0.1 && (!started || startTime != 0))
                    Start();
                else if (car.GetSpeedMs() > 0.1 && started && startTime == 0) //About 0Kmh
                    startTime = DateTime.Now.Ticks;
                else if (car.GetSpeedMs() > 27.777777d && started) //About 100Kmh
                    Sucess(car);
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
            private void Sucess(CarMotion car)
            {
                double timeElapsed = ((DateTime.Now.Ticks - startTime) - 5.0d) / 10000000.0d;//5.0d Should be MCI interval /2
                Log.feature(((IDriver)car).DriverName + ", Done  0-100Km/h In: " + timeElapsed + "sec.\r\n");
                End();

                //This must be removed when we find why some Accelariont become pretty fast.
                if (timeElapsed < 1.0d)
                {
                    Log.error(((IDriver)car).DriverName + ", Done  0-100Km/h In: " + timeElapsed + "sec, TOO FAST TOO FAST!\r\n");
                    return;
                }

                if (((Driver)car).ISession.Script.CarAccelerationSucess((ICar)car, timeElapsed))
                    return;

                //If no Script
                ((IButton)car).AddMessageTop(" ^70-100Km/h In: ^2" + timeElapsed + " ^7sec.", 5000);
            }
        }
        private sealed class FeatureDriftScore
        {
            private const double MIN_SPEED = 30.0d; //CONFIG, speed lower then this will cancel a drift score and will never trigger a drift start.
            private const double START_ANGLE = 13.5d; //angle at witch we start calculating drift.
            private const double START_MAX_ANGLE = 75.0d; //maximun angle at drift start, block reverse user from starting a drift
            private const double BONUS_ANGLE = 35.0d; //CONFIG, If the drift don't goes at least this angle drift , scoring will not occur
            private const double BONUS_ANGLE_SCORE_RATIO = 1.2d; //Gave this bonus when reach needed angle
            private const double STOP_ANGLE = 10.5d; //System will stop a Drift Calculation when this angle diff is reached
            private const double MAX_COUNTER_ANGLE_SPEED = 100.0d; //110 was too Big... , If player do a correction higher then this reverse velosity angle, drift is cancel
            private const double MIN_COUNTER_ANGLE_SPEED = 3.0d; //Seem good, lower value make it harder to make big score, this reverse velocity angle is see as a correction
            private const uint SCORE_TICK_TIME_MS = 150; //Should Desactivate Scoring if ping goes higher, Changing this, will greatly change the scoring Value.
            private const uint MIN_DRIFT_TIME_MS = 600; //Drift Time Lower Then this are cancel.
            private const double SCORE_ANGLE_RATIO = 2.5d; //2-3 seem cool, scoreAngle By this Value
            private const double SCORE_SPEED_RATIO = 1.0d; //scoreSpeed By this Value

            private bool started = false;
            private long startTime = 0;
            private bool clockWise = false; //To be sure we catch a 360 and know witch side is a correction.
            private double maxAngleDiff = 0.0d;
            private double scoreAngle = 0.0d;
            private double startSpeed = 0.0d;
            private double scoreSpeed = 0.0d;
            private double score = 0;
            private uint scoreTick = 0;
            private uint counterCorrection = 0;
            private uint packetReceive = 0; //To make correction calculation from a % of receive packet with correction inside.

            internal void Update(CarMotion car)
            {
                packetReceive++;
                double speedKhm = car.GetSpeedKmh();
                double angleTotraj = car.GetShorterAngleToReachTraj();
                if (!started)
                {
                    if(speedKhm > MIN_SPEED)
                    {
                        if (angleTotraj > START_ANGLE && angleTotraj < START_MAX_ANGLE)
                            Start(car);
                    }
                }
                else
                {
                    //Player is Too Slow End without Score
                    if (speedKhm < MIN_SPEED)
                    {
                        End(car);
                        return;
                    }
                    //This will occur when orientation change is direction
                    //Into a perfect drift this happen only at the complete end and not much.
                    double oriSpeed = car.GetOrientationSpeed();
                    if(clockWise != (oriSpeed>0))
                    {
                        if(Math.Abs(oriSpeed) > MIN_COUNTER_ANGLE_SPEED)
                        {//he apply correction
                            counterCorrection++;
                            if (Math.Abs(oriSpeed) > MAX_COUNTER_ANGLE_SPEED)
                            {//This should mean a lost control, since is doing "S"
                                End(car);
                                return;
                            }
                        }
                    }

                    //Record Max Angle
                    if (maxAngleDiff < angleTotraj)
                        maxAngleDiff = angleTotraj;

                    //Process Score
                    ComputeScore(car);
                    
                    //Find END
                    if (angleTotraj < STOP_ANGLE)
                    {
                        if (score > 0 && scoreTick > (MIN_DRIFT_TIME_MS / SCORE_TICK_TIME_MS))
                            Sucess(car);
                        else
                            End(car);
                    }
                }
            }
            private void Start(CarMotion car)
            {
                started = true;
                startTime = DateTime.Now.Ticks;
                clockWise = (car.GetOrientationSpeed() > 0 ? true : false);
                maxAngleDiff = 0.0d;
                scoreAngle = 0.0d;
                startSpeed = car.GetSpeedKmh();
                scoreSpeed = 0;
                score = 0;
                scoreTick = 0;
                counterCorrection = 0;
                packetReceive = 0;
                ((Car)car).SendUpdateButton((ushort)Button_Entry.INFO_1, "^2Drift Start "+(clockWise?"":"^7-"));
                ((Car)car).SendUpdateButton((ushort)Button_Entry.INFO_2, "^7Score ^3" + score);
            }
            private void End(CarMotion car)
            {
                startTime = 0;
                started = false;
                ((Car)car).SendUpdateButton((ushort)Button_Entry.INFO_1, "^1Drift End");
            }
            private void Sucess(CarMotion car)
            {
                if (maxAngleDiff >= BONUS_ANGLE)
                    score *= BONUS_ANGLE_SCORE_RATIO;

                Log.feature(((IDriver)car).DriverName + ", Done  Drift Score: " + (uint)score + ".\r\n");               
                End(car);
                
                //Debug think
                ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_2, "^7Score ^2" + score);

                if (((Driver)car).ISession.Script.CarDriftScoring((ICar)car, (uint)score))
                    return;

                ((IButton)car).AddMessageMiddle("^3Drift Score ^2" + (uint)score, 2200);
            }
            private void ComputeScore(CarMotion car)
            {
                //Scoring calculation occur only at all SCORE_TICK_TIME_MS diff.
                long timeDiff = (DateTime.Now.Ticks - startTime)/10000;
                if (timeDiff / SCORE_TICK_TIME_MS < scoreTick)
                    return;

                //How many time we calculated the score
                scoreTick++;

                //Calculate correction ratio
                double correctionRatio = (100.0d-(counterCorrection * 100.0d/packetReceive))/100.0d;

                //Gave point for Angle
                double angleToReach = car.GetAngleToReachTraj(clockWise);
                scoreAngle += (360.0d - angleToReach) * SCORE_ANGLE_RATIO;
                
                //If player goes faster then start speed WOW, if not loose a little each time
                double _scoreSpeed = ((car.GetSpeedKmh() * 100.0d / startSpeed) - 100.0d) * SCORE_SPEED_RATIO;
                if(_scoreSpeed <0.0d)
                    _scoreSpeed += (startSpeed / 35.0d) * (double)scoreTick;
                scoreSpeed += _scoreSpeed;

                //All By Correction %, Can only Lower Score
                score = ((scoreAngle + scoreSpeed) * correctionRatio) - ((correctionRatio-1.0d)*40.0d);

                //Only for debug purpose
                ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_2, "^7Score ^3"+score);
                ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_3, "^3SA ^7" + scoreAngle);
                ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_4, "^3SS ^7" + scoreSpeed);
                ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_5, "^3CR ^7" + correctionRatio);
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
        /*public void FinishRace() //this serve nothing, was to make a script call when car finish a race
        {
            if (((Session)((Driver)this).ISession).script.CarFinishRace((ICar)this))
                return;
        }*/
        public void EnterPit()
        {

        }
        public void EnterTrack()
        {
            isOnTrack = true;

            RemoveTrackPrefix();
            RemoveBanner();
        }
        public void EnterTrackFirstTime()
        {
            ((Driver)this).wr = Program.pubStats.GetWR(carPrefix + ((Driver)this).ISession.GetRaceTrackPrefix());
            if (((Driver)this).wr != null)
            {
                //lapTime = lapTime.Insert();
                AddMessageMiddle("^2World Record, " + PubStats.MSToString(((Driver)this).wr.LapTime,"7^","^7") + ", ^2by^ " + ((Driver)this).wr.LicenceName, 7000);
            }
            ((Driver)this).pb = Program.pubStats.GetPB(LicenceName, carPrefix + ((Driver)this).ISession.GetRaceTrackPrefix());
            if (((Driver)this).pb != null && ((Driver)this).wr != null)
            {
                AddMessageMiddle("^2Your Record, " + PubStats.MSToString(((Driver)this).pb.LapTime, "7^", "^7") + ", ^3Diff " + PubStats.MSToString(((Driver)this).pb.LapTime - ((Driver)this).wr.LapTime, "7^", "^8"), 7000);
            }
        }
        public void LeaveTrack()
        {
            isOnTrack = false;
            SendBanner();
            SendTrackPrefix();
        }
        public bool IsOnTrack()
        {
            return (carId > 0 && isOnTrack);
        }
        public byte CarId
        {
            get { return carId; }
        }
        public string CarPrefix
        {
            get { return carPrefix; }
        }
        public Penalty_Type CurrentPenality
        {
            get { return currentPenality; }
        }
        public ushort LapCompleted
        {
            get { return lapCompleted; }
        }

        public ushort GetNode()
        {
            return node;
        }
        public byte GetRacePosition()
        {
            return racePosition;
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
        public double GetTrajectory()
        {
            return tracjectory * 360.0d / 65536.0d;
        }
        public double GetOrientation()
        {
            return orientation * 360.0d / 65536.0d;
        }
        public double GetOrientationSpeed()
        {
            return orientationSpeed * 360.0d / 16384.0d;
        }
        //This gave the Angle have to make to reach Trajectory.
        public double GetAngleToReachTraj(bool clockWise)
        {
            int temp;
            if(clockWise)
                temp = (65536 - orientation) - (65536 - tracjectory);
            else
                temp = (65536 - tracjectory) - (65536 - orientation);

            if (temp < 0)
                temp += 65536;

            return temp * 360.0d / 65536.0d;
        }
        public double GetShorterAngleToReachTraj()
        {
            int temp;
            if (orientation > tracjectory)
                temp = (65536 - tracjectory) - (65536 - orientation);
            else
                temp = (65536 - orientation) - (65536 - tracjectory);

            if (temp > 32768)
                temp -= 65535;

            if (temp < 0)
                temp *= -1;

            return temp * 180.0d / 32768.0d;
        }
    }
}
