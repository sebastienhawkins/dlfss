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
    using Game_;
    using Log_;
    using Storage_;
    using PubStats_;
    using Ranking_;

    partial class Driver
    {
        internal void ProcessCarInformation(CarInformation _carInformation)
        {
            node = _carInformation.nodeTrack;
            lapCompleted = _carInformation.lapNumber;
            racePosition = _carInformation.position;
            flagRace = _carInformation.carFlag;
            

            x = _carInformation.posX / 65536.0d;
            y = _carInformation.posY / 65536.0d;
            z = _carInformation.posZ / 65536.0d;
            tracjectory = _carInformation.direction;
            orientation = _carInformation.heading;
            orientationSpeed = _carInformation.angleVelocity;


            speedMs = _carInformation.speed / 327.68d;
            if (maxSpeedMs < speedMs)
                maxSpeedMs = speedMs;

            speedKmh = ConvertX.MSToKhm(speedMs);

            featureAcceleration.Update(this);
            featureDriftScore.Update(this);
        }
        private byte carId = 0;
        private string carPrefix = "";
        private string trackPrefix = "";
        private string carPlate = "";
        private string carSkin = "";
        private byte addedMass = 0;
        private byte addedIntakeRestriction = 0;
        private byte passenger = 0;
        private Tyre_Compound tyreFrontLeft = Tyre_Compound.CAR_TYRE_NOTCHANGED;
        private Tyre_Compound tyreFrontRight = Tyre_Compound.CAR_TYRE_NOTCHANGED;
        private Tyre_Compound tyreRearLeft = Tyre_Compound.CAR_TYRE_NOTCHANGED;
        private Tyre_Compound tyreRearRight = Tyre_Compound.CAR_TYRE_NOTCHANGED;
        private Penalty_Type currentPenality = Penalty_Type.PENALTY_TYPE_NONE;
        protected double maxSpeedMs = 0.0d;
        private ushort lapCompleted = 0;
        private ushort node = 0;
        private Flag_Race flagRace = Flag_Race.NONE;
        private double x = 0.0d;
        private double y = 0.0d;
        private double z = 0.0d;
        private double xOld = 0.0d;
        private double yOld = 0.0d;
        private double speedMs = 0.0d;
        private double speedKmh = 0.0d;
        private double speedMph = 0.0d;
        private ushort tracjectory = 0;
        private ushort orientation = 0;
        private short orientationSpeed = 0;
        private bool isOnTrack = false;
        private bool isMoving = false;
        private uint timeIldeOnTrack = 0;
        private FeatureAcceleration featureAcceleration = new FeatureAcceleration();
        private FeatureDriftScore featureDriftScore = new FeatureDriftScore();

        private sealed class FeatureAcceleration
        {
            private bool started = false;
            private double effectiveStartSpeed = 0.0d;
            private bool finish = false;
            private DateTime startTime = DateTime.Now;
            private double startSpeed = 0.9;
            private double endSpeed = 100.0;
            private bool isOn = true;

            internal void Update(Driver driver)
            {
                if(!isOn)
                    return;
                if (started && driver.GetSpeedKmh() < startSpeed)
                    End();
                else if (!started && driver.GetSpeedKmh() >= startSpeed)
                    Start(driver);
                else if (!finish && started && driver.GetSpeedKmh() >= endSpeed)
                    Sucess(driver);
            }
            private void Start(Driver driver)
            {
                effectiveStartSpeed = driver.GetSpeedKmh();
                startTime = DateTime.Now;
                started = true;
            }
            private void End()
            {
                finish = false;
                started = false;
            }
            private void Sucess(Driver driver)
            {
                finish = true;
                TimeSpan timeElapsed = DateTime.Now - startTime;

                effectiveStartSpeed = endSpeed - effectiveStartSpeed;
                double _temp = endSpeed - startSpeed;
                _temp = timeElapsed.TotalSeconds / effectiveStartSpeed * _temp;


                Log.feature(driver.DriverName + ", Done  " + (ushort)startSpeed + "-" + (ushort)endSpeed + " Km/h In: " + (decimal)_temp + "sec.\r\n");

                //This must be removed when we find why some Accelariont become pretty fast.
                if (timeElapsed.TotalSeconds < 1.0d)
                {
                    Log.error(driver.DriverName + ", Done  " + (ushort)startSpeed + "-" + (ushort)endSpeed + " Km/h In: " + _temp + "sec, TOO FAST TOO FAST!\r\n");
                    return;
                }

                if (driver.ISession.Script.CarAccelerationSucess((ICar)driver, (ushort)startSpeed, (ushort)endSpeed, _temp))
                    return;
            }
            internal double StartSpeed
            {
                get { return startSpeed; }
                set 
                { 
                    startSpeed = value;
                    End();
                }
            }
            internal double EndSpeed
            {
                get { return endSpeed; }
                set 
                { 
                    endSpeed = value;
                    End();
                }
            }
            internal void SetOnOff(bool on)
            {
                isOn = on;
                End();
            }
            internal bool IsOn()
            {
                return isOn;
            }
        }
        internal bool IsAccelerationOn()
        {
            return featureAcceleration.IsOn();
        }
        internal ushort GetAccelerationStartSpeed()
        {
            return (featureAcceleration.StartSpeed < 1.0d ? (ushort)0 : (ushort)featureAcceleration.StartSpeed);
        }
        internal ushort GetAccelerationEndSpeed()
        {
            return (ushort)featureAcceleration.EndSpeed;
        }
        internal void SetAccelerationStartSpeed(ushort startKmh)
        {
            featureAcceleration.StartSpeed = (startKmh > 0 ? (double)startKmh : 0.9d);
            SetConfigValue(Config_User.ACCELERATION_START, startKmh.ToString());
        }
        internal void SetAccelerationEndSpeed(ushort endKmh)
        {
            featureAcceleration.EndSpeed = (double)endKmh;
            SetConfigValue(Config_User.ACCELERATION_STOP, endKmh.ToString());
        }
        internal void SetAccelerationOn(bool isOn)
        {
            featureAcceleration.SetOnOff(isOn);
            SetConfigValue(Config_User.ACCELERATION_ON, (isOn ? "1" : "0"));
        }

        sealed class FeatureDriftScore
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

            private bool isOn = true;
            private bool started = false;
            private DateTime startTime = DateTime.Now;
            private bool clockWise = false; //To be sure we catch a 360 and know witch side is a correction.
            private double maxAngleDiff = 0.0d;
            private double scoreAngle = 0.0d;
            private double startSpeed = 0.0d;
            private double scoreSpeed = 0.0d;
            private double score = 0;
            private uint scoreTick = 0;
            private uint counterCorrection = 0;
            private uint packetReceive = 0; //To make correction calculation from a % of receive packet with correction inside.

            internal void Update(Driver car)
            {
                if(!isOn)
                    return;

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
            private void Start(Driver driver)
            {
                started = true;
                startTime = DateTime.Now;
                clockWise = (driver.GetOrientationSpeed() > 0 ? true : false);
                maxAngleDiff = 0.0d;
                scoreAngle = 0.0d;
                startSpeed = driver.GetSpeedKmh();
                scoreSpeed = 0;
                score = 0;
                scoreTick = 0;
                counterCorrection = 0;
                packetReceive = 0;
                /*if(driver.IsAdmin)
                {
                    driver.SendUpdateButton((ushort)Button_Entry.INFO_1, "^2Drift Start "+(clockWise?"":"^7-"));
                    driver.SendUpdateButton((ushort)Button_Entry.INFO_2, "^7Score ^3" + score);
                }*/
            }
            private void End(Driver driver)
            {
                started = false;
                /*if(driver.IsAdmin)
                {
                    driver.SendUpdateButton((ushort)Button_Entry.INFO_1, "^1Drift End");
                }*/
            }
            private void Sucess(Driver driver)
            {
                if (maxAngleDiff >= BONUS_ANGLE)
                    score *= BONUS_ANGLE_SCORE_RATIO;

                Log.feature(((IDriver)driver).DriverName + ", Done  Drift Score: " + (uint)score + ".\r\n");               
                End(driver);
                
                //Debug think
                /*if(((Driver)driver).IsAdmin)
                {
                    ((IButton)driver).SendUpdateButton((ushort)Button_Entry.INFO_2, "^7Score ^2" + score);
                }*/
                ((Driver)driver).driftScoreByTime += (uint)score;
                if (((Driver)driver).ISession.Script.CarDriftScoring((ICar)driver, (uint)score))
                    return;
            }
            private void ComputeScore(Driver driver)
            {
                //Scoring calculation occur only at all SCORE_TICK_TIME_MS diff.
                TimeSpan timeDiff = DateTime.Now - startTime;
                if (timeDiff.TotalMilliseconds / SCORE_TICK_TIME_MS < scoreTick)
                    return;

                //How many time we calculated the score
                scoreTick++;

                //Calculate correction ratio
                double correctionRatio = (100.0d-(counterCorrection * 100.0d/packetReceive))/100.0d;

                //Gave point for Angle
                double angleToReach = driver.GetAngleToReachTraj(clockWise);
                scoreAngle += (360.0d - angleToReach) * SCORE_ANGLE_RATIO;
                
                //If player goes faster then start speed WOW, if not loose a little each time
                double _scoreSpeed = ((driver.GetSpeedKmh() * 100.0d / startSpeed) - 100.0d) * SCORE_SPEED_RATIO;
                if(_scoreSpeed <0.0d)
                    _scoreSpeed += (startSpeed / 35.0d) * (double)scoreTick;
                scoreSpeed += _scoreSpeed;

                //All By Correction %, Can only Lower Score
                score = ((scoreAngle + scoreSpeed) * correctionRatio) - ((correctionRatio-1.0d)*40.0d);

                //Only for debug purpose
                /*if(((Driver)car).IsAdmin)
                {
                    ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_2, "^7Score ^3"+score);
                    ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_3, "^3SA ^7" + scoreAngle);
                    ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_4, "^3SS ^7" + scoreSpeed);
                    ((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_5, "^3CR ^7" + correctionRatio);
                }*/
            }
            public bool IsOn
            {
                get { return isOn; }
                set 
                { 
                    isOn = value;
                    started = false;
                }
            }
        }
        internal bool IsDriftScoreOn()
        {
            return featureDriftScore.IsOn;
        }
        internal void SetDriftScoreOn(bool isOn)
        {
            featureDriftScore.IsOn = isOn;
            ((Driver)this).SetConfigValue(Config_User.DRIFT_SCORE_ON, (isOn ? "1" : "0"));
        }


        /*public void FinishRace() //this serve nothing, was to make a script call when car finish a race
        {
            if (((Session)((Driver)this).ISession).script.CarFinishRace((ICar)this))
                return;
        }*/
        private void EnterPit()
        {
        }
        private void EnterTrack(bool firstTime)
        {
            isOnTrack = true;

            RemoveTrackPrefix();
            RemoveBanner();
            
            if(firstTime)
                EnterTrackFirstTime();
        }
        //should be into driver
        private void EnterTrackFirstTime()
        {
            wr = Program.pubStats.GetWR(carPrefix + ISession.GetRaceTrackPrefix());
            if (wr != null)
            {
                //lapTime = lapTime.Insert();
                AddMessageMiddle("^2WR " + ConvertX.MSToString(wr.LapTime, Msg.COLOR_DIFF_TOP, Msg.COLOR_DIFF_TOP) + " ^2by^ " + wr.LicenceName, 6000);
            }

            pb = Program.pubStats.GetPB(licenceName, carPrefix + ISession.GetRaceTrackPrefix());
            if (pb != null && wr != null)
            {
                AddMessageMiddle("^2PB " + ConvertX.MSToString(pb.LapTime, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + " ^2WR " + ConvertX.MSToString(pb.LapTime - wr.LapTime, Msg.COLOR_DIFF_LOWER, Msg.COLOR_DIFF_HIGHER), 7000);
            }
            else if (pb != null)
            {
                AddMessageMiddle("^2PB " + ConvertX.MSToString(pb.LapTime, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT), 7000);
            }

            Rank _rank = GetRank(ISession.GetRaceTrackPrefix(),CarPrefix);
            if(_rank != null)
            {
                if (!ISession.IsFreezeMotdSend())
                    ISession.SendMSTMessage("/msg "+driverName+" ^2" + _rank.GetGradeComment() + "^2 with ^7" + carPrefix);
                //AddMessageTop("^2Rank Detail, ^2BL^7"+_rank.BestLap+" ^2AV^7"+_rank.AverageLap+" ^2ST^7"+_rank.Stability+" ^2WI^7"+_rank.RaceWin,5000);
            }
            else
            {
                if (!ISession.IsFreezeMotdSend())
                    ISession.SendMSTMessage("/msg "+driverName+" ^2is "+( IsBot() ? "a ^7BOT" : "^7new")+"^2 with ^7" + carPrefix);
                //AddMessageTop("^2Rank Detail, you have no rank for ^7"+((Driver)this).ISession.GetRaceTrackPrefix()+" ^2with car ^7"+carPrefix,3000);
            }
            
            if(!IsBot())
            {
                float pctBad = ((float)badDrivingCount / (totalRaceFinishCount > 0 ? (float)totalRaceFinishCount : 1.0f));
                pctBad *= 100.0f;
                pctBad = 101.0f - pctBad;
                if(pctBad > 100.0f)
                    pctBad = 100.0f;
                if (!ISession.IsFreezeMotdSend())
                    ISession.SendMSTMessage("/msg ^2    and '^7" + Math.Round(pctBad,0) + "%^2' safe.");
            }
        }
        internal void LeaveTrack()
        {
            isMoving = false;
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
        
        public bool IsMoving()
        {
            return isMoving;
        }
    }
}
