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
using System.Collections.Generic;
using System.Text;

namespace Drive_LFSS.Game_
{
    using Log_;
    using Config_;
    using Storage_;
    using Script_;
    using Definition_;
    using Map_;
    class Grid
    {
        internal Grid()
        {
        }
        ~Grid()
        {
            if (true == false) { }
        }
        internal void Init(ushort _nodeCount)
        {
            nodeCount = _nodeCount;
        }
        internal void ConfigApply()
        {
            //TODO:This is not reloaded , if we reload only table... have to fix this
            carBrakeValue.Clear();
            uint endItr = Program.carTemplate.GetMaxEntry()+1;
            for(uint itr = 1; itr < endItr;itr++)
            {
                CarTemplateInfo carInfo = Program.carTemplate.GetEntry(itr);
                if(carInfo != null)
                    carBrakeValue.Add(carInfo.NamePrefix,carInfo.BrakeDist);
            }
        }

        private ushort nodeCount = 0;
        private Dictionary<CarMotion, ushort> carContainer  = new Dictionary<CarMotion, ushort>();
        private Dictionary<string, ushort> carBrakeValue = new Dictionary<string,ushort>();

        internal void Add(CarMotion car)
        {
            carContainer[car] = car.GetNode();
        }
        internal void ProcessCarInformation(CarMotion car)
        {
           // if (car.GetSpeedMs() < 0.1) //About 0.01 m seconde
               // return;
            carContainer[car] = car.GetNode();

            checkNodePosition(car);
            checkCollision(car);
            checkLostControl(car);

            //if(((IDriver)car).IsAdmin)
                //((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_2, "^3Z ^7" + car.GetPosZ());
        }
        internal void Remove(CarMotion car)
        {
            carContainer.Remove(car);
        }

        private CarMotion[] getCarAround(CarMotion car, byte nextNodeCount, byte previousNodeCount)
        {
            List<CarMotion> carIds = new List<CarMotion>();
            byte itr;
            int newItr = 0;

            if(nextNodeCount > 0)
            {
                for (itr = 0; itr <= nextNodeCount; itr++)
                {
                    newItr = carContainer[car] + itr;
                    if (newItr > nodeCount)
                        newItr = 0; 
                    carIds.AddRange(findNodeCars((ushort)newItr));
                }
            }

            carIds.AddRange(findNodeCars(carContainer[car]));
            
            if (previousNodeCount > 0)
            {
                for (itr = 0; itr <= nextNodeCount; itr++)
                {
                    newItr = carContainer[car] - itr;
                    if (newItr < 0)
                        newItr = nodeCount + newItr; //Need to know witch is the first NODE, 0 or 1
                    carIds.AddRange(findNodeCars((ushort)newItr));
                }
            }
            return carIds.ToArray();
        }
        private CarMotion[] findNodeCars(ushort node)
        {
            List<CarMotion> carIds = new List<CarMotion>();
            Dictionary<CarMotion, ushort>.Enumerator itr = carContainer.GetEnumerator();
            while(itr.MoveNext())
            {
                if(itr.Current.Value == node)
                    carIds.Add(itr.Current.Key);
            }
            return carIds.ToArray();
        }

        //Feature
        private void checkLostControl(CarMotion car)
        {
            if (car.HasWarningDrivingCheck() && Math.Abs(car.GetOrientationSpeed()) > 115.0d)
                car.TrySendCancelWarning();
        }
        private void checkNodePosition(CarMotion car)
        {
            if (car.TrackPrefix.IndexOf("AU") != -1)
                return;

            NodeData node = Map.GetNode(car.TrackPrefix, car.GetNode());
            if (node == null)
            {
                //Log.error("checkNodePosition(car), Node requested don't exist, Track: " + car.TrackPrefix + ", Node:" + car.GetNode() + "\r\n");
                return;
            }
            double trackOrientation = node.GetOrientation();
            
            //Rotate -90
            double rest = trackOrientation - 90.0d;
            if (rest > 0)
                trackOrientation = rest;
            else
                trackOrientation = rest+360.0d;

            double tracjectoryDiff = GetAngleDiff(trackOrientation,car.GetTrajectory());
            car.SendTrajDisplay(GetTrajectoryDisplay(tracjectoryDiff));

            double orientationDiff = GetAngleDiff(trackOrientation, car.GetOrientation());
            car.SendOriDisplay( GetOrientationDisplay(orientationDiff) );

            double perpenDist = GetPerpendicularDist(car.GetPosX(), car.GetPosY(), node.centreX, node.centreY, node.centreX + node.dirX, node.centreY + node.dirY);
            car.SendPathDisplay(GetPathSideDisplay(node.driveLeft,node.driveRight, perpenDist,car));

            bool lostControl = false;//car.HasYellowFlagActive();
            bool outsideDrive = false;
            bool drift = false;

            if (node.driveLeft < node.driveRight)
            {
                if (perpenDist < node.driveLeft || perpenDist > node.driveRight)
                    outsideDrive = true;
            }
            else if (perpenDist > node.driveLeft || perpenDist < node.driveRight)
                outsideDrive = true;

            if (car.GetSpeedMs() > 0.1 && (Math.Abs(tracjectoryDiff) > 16 && Math.Abs(orientationDiff) > 19))
                lostControl = true;
            else if (car.GetSpeedMs() > 0.1 && Math.Abs(orientationDiff) > 19)
                drift = true;

            car.SetLostControl(lostControl);
            car.SetDrifting(drift);
            car.SetOutsideDrive(outsideDrive);
            
            double angle = Math.Abs(GetAngle(car, node.centreX, node.centreY));
            if (((Driver)car).IsAdmin)
            {
                //((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_1, drift ? "^1Drift":"^2Grip");
                //((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_2, "^7LIMIT " + (int)node.limitLeft + " | " + (int)node.limitRight);
                //((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_3, "^7DLIMIT " + (int)node.driveLeft + " | " +(int)node.driveRight);
                //((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_4, touchGrass ? "^3GrassOn" : "^2GrassOff");
                //((IButton)car).SendUpdateButton((ushort)Button_Entry.INFO_5, drift ? "^1Drift" : "^2Grip");
            }
        }
        private void checkCollision(CarMotion car)
        {
            if(car.HasWarningDrivingCheck())
                return;

            CarMotion[] carArounds;
            carArounds = getCarAround(car, 3, 3);
            for (byte itr = 0; itr < carArounds.Length; itr++ )
            {
                if (carArounds[itr].CarId == car.CarId || car.HasWarningDrivingCheck() ||carArounds[itr].HasWarningDrivingCheck()) //Remove Self
                    continue;
                
                CarMotion carAround = carArounds[itr];
                
                double speedDiff = ((car.GetSpeedKmh() - carAround.GetSpeedKmh()));
                double dist = GetDistance2DSq(car, carAround);

                //point of view  is CarAround going into me
                if ( speedDiff < 0 && GetDistanceZ(carAround,car) < 1.0d
                     && (-speedDiff / 100.0d * carBrakeValue[carAround.CarPrefix]) > dist 
                     && HasCollisionPath(carAround, car) )
                {
                    /*if (((Driver)carAround).IsAdmin)
                    {
                        ((IButton)carAround).SendUpdateButton((ushort)Button_Entry.INFO_1, "^3CD " + (-speedDiff / 100.0d * carBrakeValue[carAround.CarPrefix]));
                        ((IButton)carAround).SendUpdateButton((ushort)Button_Entry.INFO_2, "^3D ^7" + dist);
                        ((IButton)carAround).SendUpdateButton((ushort)Button_Entry.INFO_5, "^3Z ^7" + GetDistanceZ(car, carAround));
                        ((IButton)carAround).SendUpdateButton((ushort)Button_Entry.INFO_3, "^3SD ^7" + -speedDiff);
                        ((IButton)carAround).SendUpdateButton((ushort)Button_Entry.INFO_4, "^3B ^7" + carBrakeValue[carAround.CarPrefix]);
                    }*/
                    if (CarDriveOk(car))
                    {
                        car.SetWarningDrivingCheck(Warning_Driving_Type.VICTIM, carAround.CarId);
                        carAround.SetWarningDrivingCheck(Warning_Driving_Type.BAD_DRIVING, car.CarId);
                        Log.commandHelp("Warning Driving 1-DriverOk(" + ((Driver)car).LicenceName + "), DriverBad(" + ((Driver)carAround).LicenceName + ") Detected\r\n");
                        return;
                    }
                    if (!CarDriveOk(car) && CarDriveOk(carAround))
                    {
                        car.SetWarningDrivingCheck(Warning_Driving_Type.BAD_DRIVING,carAround.CarId);
                        carAround.SetWarningDrivingCheck(Warning_Driving_Type.VICTIM, car.CarId);
                        Log.commandHelp("Warning Driving Part 2-DriverOk(" + ((Driver)carAround).LicenceName + "), DriverBad(" + ((Driver)car).LicenceName + ") Detected\r\n");
                        return;
                    }
                    /*if (car.HasYellowFlagActive() && !carAround.HasYellowFlagActive() && !carAround.IsBlueFlagActive())
                    {
                        car.SetWarningDrivingCheck(Warning_Driving_Type.BAD_DRIVING, carAround.CarId);
                        carAround.SetWarningDrivingCheck(Warning_Driving_Type.VICTIM, car.CarId);
                        Log.commandHelp("Warning Driving Part 2 Detected\r\n");
                        return;
                    }

                    if (!car.HasYellowFlagActive() && !car.IsBlueFlagActive() && !car.IsLostControl())
                    {
                        car.SetWarningDrivingCheck(Warning_Driving_Type.VICTIM, carAround.CarId);
                        carAround.SetWarningDrivingCheck(Warning_Driving_Type.BAD_DRIVING, car.CarId);
                        Log.commandHelp("Warning Driving Part 3 Detected\r\n");
                        return;
                    }*/
                } 
            }
        }
        private bool CarDriveOk(CarMotion car)
        {
            return !car.IsBlueFlagActive() && !car.IsLostControl() && !car.IsOutsideDrive() && !car.HasYellowFlagActive();
        }
        //Badly need Car.GetSize()
        private double GetDistance3DSq(CarMotion car1, CarMotion car2)
        {
            double dx = car2.GetPosX() - car1.GetPosX();
            double dy = car2.GetPosY() - car1.GetPosY();
            double dz = car2.GetPosZ() - car1.GetPosZ();
            double bothSize = 3.0d;
            double distance = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz) - bothSize);
            return (distance > 0 ? distance : 0);
        }
        private double GetDistance3D(CarMotion car1, CarMotion car2)
        {
            double dx = car2.GetPosX() - car1.GetPosX();
            double dy = car2.GetPosY() - car1.GetPosY();
            double dz = car2.GetPosZ() - car1.GetPosZ();
            double bothSize = 3.0d;
            double distance = (dx * dx) + (dy * dy) + (dz * dz) - bothSize;
            return (distance > 0 ? distance : 0);
        }
        private double GetDistance2DSq(CarMotion car1, CarMotion car2)
        {
            double dx = (car2.GetPosX() - car1.GetPosX());
            double dy = (car2.GetPosY() - car1.GetPosY());
            double bothSize = 3.0d; //this is from center of the car so bothSize/2
            double distance = Math.Sqrt( ((dx * dx) + (dy * dy) - bothSize));
            return distance;
        }
        private double GetDistance2DAbs(CarMotion car1, CarMotion car2)
        {
            double dx = car2.GetPosX() - car1.GetPosX();
            double dy = car2.GetPosY() - car1.GetPosY();
            double bothSize = 3.0d;
            double distance = (dx * dx) + (dy * dy) - bothSize;
            return distance;
        }
        private double GetDistance2D(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return dx + dy;
        }
        private double GetDistance2D(CarMotion car, double x, double y)
        {
            double dx = x - car.GetPosX();
            double dy = y - car.GetPosY();
            return dx + dy;

        }
        private double GetDistanceZ(CarMotion fromCar, CarMotion tocar)
        {
            double dz = Math.Abs(tocar.GetPosZ() - fromCar.GetPosZ());
            double bothHeight = 1.5d; //bothHeight/2 in fact
            double distance = dz - bothHeight;
            return ( distance > 0 ? distance : 0);
}
        private double GetAngle(CarMotion fromCar, CarMotion toCar)
        {
            double dx = (toCar.GetPosX() - fromCar.GetPosX());
            double dy = (toCar.GetPosY() - fromCar.GetPosY());
            double angle = Math.Atan2(dy, dx);

            /*return for Multiturn Angle, no need! keep to remember other possibility
            return (angle >= 0) ? angle : 2 * M_PI + angle;*/
            return angle * 180.0f / Math.PI;
        }
        private double GetAngle(CarMotion fromCar, double x , double y)
        {
            double dx = (x - fromCar.GetPosX());
            double dy = (y - fromCar.GetPosY());
            double angle = Math.Atan2(dy, dx);

            /*return for Multiturn Angle, no need! keep to remember other possibility
            return (angle >= 0) ? angle : 2 * M_PI + angle;*/
            return angle * 180.0f / Math.PI;
        }
        private double GetAngleDiff(double angle1, double angle2)
        {
            double angleDiff = angle2 - angle1;
            if (angleDiff > 180.0d)
                angleDiff -= 360.0d;
            else if (angleDiff < -180.0d)
                angleDiff += 360.0d;
            return angleDiff;
        }
        private string GetTrajectoryDisplay(double tracjectoryDiff)
        {
            char[] tracjectoryDisplay = new char[91];
            string trajDisplay = "";
            int marque = (int)(tracjectoryDiff * 90.0d / 360.0d) + 45;
            for (byte itr = 0; itr < 91; itr++)
                tracjectoryDisplay[itr] = ' ';
            tracjectoryDisplay[marque] = '|';

            for (int itr = 90; itr > -1; itr--)
                trajDisplay += tracjectoryDisplay[itr];

            if (marque > 45)
                marque = marque - 45;
            else
                marque = 45 - marque;

            if (marque > 5)
                trajDisplay = "^1" + trajDisplay;
            else if (marque > 2)
                trajDisplay = "^3" + trajDisplay;
            else
                trajDisplay = "^2" + trajDisplay;

            return trajDisplay;
        }
        private string GetOrientationDisplay(double orientationDiff)
        {
            char[] orientationDisplay = new char[91];
            string oriDisplay = "";
            int marque = (int)(orientationDiff * 90.0d / 360.0d) + 45;
            for (byte itr = 0; itr < 91; itr++)
                orientationDisplay[itr] = ' ';
            orientationDisplay[marque] = '|';

            for (int itr = 90; itr > -1; itr--)
                oriDisplay += orientationDisplay[itr];

            if (marque > 45)
                marque = marque - 45;
            else
                marque = 45 - marque;

            if (marque > 5)
                oriDisplay = "^1" + oriDisplay;
            else if (marque > 2)
                oriDisplay = "^3" + oriDisplay;
            else
                oriDisplay = "^2" + oriDisplay;

            return oriDisplay;
        }
        private string GetPathSideDisplay(double left, double right, double carDist, CarMotion car)
        {
            char[] pathSideDisplay = new char[91];
            string pathDisplay = "";
            int bob;
            if (left < right)
                bob = (int)((Math.Abs(left) + (carDist)) / (Math.Abs(left) + right) * 90);
            else
                bob = (int)((Math.Abs(right) + (carDist)) / (Math.Abs(right) + left) * 90);

            if (bob < 0)
                bob = 0;
            else if (bob > 90)
                bob = 90;

            for (byte itr = 0; itr < 91; itr++)
                pathSideDisplay[itr] = ' ';
            pathSideDisplay[bob] = '|';

            for (int itr = 0; itr < 91; itr++)
                pathDisplay += pathSideDisplay[itr];

            if (left < right)
            {
                if (carDist < left || carDist > right)
                    pathDisplay = "^1" + pathDisplay;
                else if (carDist < left + 1 || carDist > right - 1)
                    pathDisplay = "^3" + pathDisplay;
                else
                    pathDisplay = "^2" + pathDisplay;
            }
            else
            {
                if (carDist > left || carDist < right)
                    pathDisplay = "^1" + pathDisplay;
                else if (carDist > left - 1 || carDist < right + 1)
                    pathDisplay = "^3" + pathDisplay;
                else
                    pathDisplay = "^2" + pathDisplay;
            }
            return pathDisplay;
        }
        private double GetPerpendicularDist(double x, double y, double x1, double y1, double x2, double y2)
        {
            return ((y1 - y) * (x2 - x1) - (x1 - x) * (y2 - y1))
                   / ((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1))
                   * Math.Sqrt(((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1)));
        }
        private bool HasCollisionPath(CarMotion fromCar, CarMotion toCar)
        {
            //TODO: GetAngle() is bad , should use Both TrackTrajectory and Perpendicular Distance
            // if about same trackTraj and Perpendicular Distance is < CarSideToSideSize == Collision Path 
            double angle = GetAngle(fromCar, toCar);

            angle += 180.0f; //make it on 360

            //Rotate +90, TODO reverse SIN and COS to make the rotation
            double rest = angle + 90.0d - 360.0d;
            if (rest > 0)
                angle = rest;
            else
                angle += 90.0d;

            double angleDiff = GetAngleDiff(fromCar.GetTrajectory(),angle);
            //TODO: need to find the perfect angle, based on a static car ~size.
            //5.0d was too big at very high speed
            return ( (angleDiff <= 4.0d) && (angleDiff >= -4.0d ) );
        }
    }
}
