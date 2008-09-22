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

    sealed class Grid
    {
        public Grid()
        {
        }
        ~Grid()
        {
            if (true == false) { }
        }
        public void Init(ushort _nodeCount)
        {
            nodeCount = _nodeCount;
        }
        public void ConfigApply()
        {
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

        public void Add(CarMotion car)
        {
            carContainer[car] = car.GetNode();
        }
        public void ProcessCarInformation(CarMotion car)
        {
           // if (car.GetSpeedMs() < 0.1) //About 0.01 m seconde
               // return;
            carContainer[car] = car.GetNode();
            checkCollision(car);
        }
        public void Remove(CarMotion car)
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

        //I think we have no choices but put Dynamic, Brake range Value for each Car
        //this seem to work pretty good for most car, but with a BF1, ho no! you have much time to brake in fact!
        //Have to remember , the real collision detection will occur if YellowFlag is trigger too...
        private const double speedColRatio = 0.717d;
        private void checkCollision(CarMotion car)
        {
            CarMotion[] carAround;
            carAround = getCarAround(car, 3, 3);
            for (byte itr = 0; itr < carAround.Length; itr++ )
            {
                if (carAround[itr].CarId == car.CarId) //Remove Self
                    continue;

                double speedDiff = ((car.GetSpeedKmh() - carAround[itr].GetSpeedKmh()));
                double dist = GetDistance2DSq(car, carAround[itr]);

                if ( speedDiff < 0 && GetDistanceZ(car, carAround[itr]) < 1.0d
                     && (-speedDiff / 100.0d * carBrakeValue[carAround[itr].CarPrefix]) > dist 
                     && HasCollisionPath(carAround[itr], car) )
                {
                    ((IButton)carAround[itr]).SendUpdateButton((ushort)Button_Entry.INFO_1, "^3CD " + (-speedDiff / 100.0d * carBrakeValue[carAround[itr].CarPrefix]));
                    ((IButton)carAround[itr]).SendUpdateButton((ushort)Button_Entry.INFO_2, "^3D ^7" + dist);
                    ((IButton)carAround[itr]).SendUpdateButton((ushort)Button_Entry.INFO_5, "^3Z ^7" + GetDistanceZ(car, carAround[itr]));
                    ((IButton)carAround[itr]).SendUpdateButton((ushort)Button_Entry.INFO_3, "^3SD ^7" + -speedDiff);
                    ((IButton)carAround[itr]).SendUpdateButton((ushort)Button_Entry.INFO_4, "^3B ^7" + carBrakeValue[carAround[itr].CarPrefix]);


                    //Debug
                    if (!carAround[itr].HasCollisionWarning())
                        carAround[itr].SendCollisionWarning("^1Pre Collision Into "+((Driver)car).DriverName + "!");
                    if (!car.HasCollisionWarning())
                        car.SendCollisionWarning("^^1Pre Collision From" + ((Driver)carAround[itr]).DriverName + "!");
                    //Log.debug("Car Is in Back:" + ((Driver)carAround[itr]).DriverName + ", And Dangerous To:" + ((Driver)car).DriverName + "\r\n");
                }
                    
            }

        }



        private double GetDistance3DSq(CarMotion car1, CarMotion car2)
        {
            double dx = car2.GetPosX() - car1.GetPosX();
            double dy = car2.GetPosY() - car1.GetPosY();
            double dz = car2.GetPosZ() - car1.GetPosZ();
            double bothSize = 1.0d;
            double distance = Math.Sqrt((dx * dx) + (dy * dy) + (dz * dz) - bothSize);
            return (distance > 0 ? distance : 0);
        }
        private double GetDistance3D(CarMotion car1, CarMotion car2)
        {
            double dx = car2.GetPosX() - car1.GetPosX();
            double dy = car2.GetPosY() - car1.GetPosY();
            double dz = car2.GetPosZ() - car1.GetPosZ();
            double bothSize = 1.0d;
            double distance = (dx * dx) + (dy * dy) + (dz * dz) - bothSize;
            return (distance > 0 ? distance : 0);
        }
        private double GetDistance2DSq(CarMotion car1, CarMotion car2)
        {
            double dx = (car2.GetPosX() - car1.GetPosX());
            double dy = (car2.GetPosY() - car1.GetPosY());
            double bothSize = 1.5d; //this is from center of the car so bothSize/2
            double distance = Math.Sqrt( ((dx * dx) + (dy * dy) - bothSize));
            return distance;
        }
        private double GetDistance2D(CarMotion car1, CarMotion car2)
        {
            double dx = car2.GetPosX() - car1.GetPosX();
            double dy = car2.GetPosY() - car1.GetPosY();
            double bothSize = 1.0d;
            double distance = (dx * dx) + (dy * dy) - bothSize;
            return (distance > 0 ? distance : 0);
        }
        private double GetDistanceZ(CarMotion fromCar, CarMotion tocar)
        {
            double dz = Math.Abs(tocar.GetPosZ() - fromCar.GetPosZ());
            double bothHeight = 0.5d;
            double distance = dz - bothHeight;
            return ( distance > 0 ? distance : 0);
}
        private double GetAngle(CarMotion fromCar, CarMotion toCar)
        {
            double dx = (toCar.GetPosX() - fromCar.GetPosX());
            double dy = (toCar.GetPosY() - fromCar.GetPosY());
            double angle = Math.Atan2(dy, dx);

            /*return for Multiturn Angle, but no need! keep to remember other possibility
            return (angle >= 0) ? angle : 2 * M_PI + angle;*/
            return angle * 180.0f / Math.PI;
        }
        //Has to go into Car class.
        private bool HasCollisionPath(CarMotion fromCar, CarMotion toCar)
        {
            double angle = GetAngle(fromCar, toCar);

            angle += 180.0f; //make it on 360

            //Rotate +90
            double rest = angle + 90.0d - 360.0d;
            if (rest > 0)
                angle = rest;
            else
                angle += 90.0d;

            double angleDiff = fromCar.GetTrajectory() - angle;

            //TODO: need to find the perfect angle, based on a static car ~size.
            //13.0d was too big at very high speed
            return ( (angleDiff <= 12.0d) && (angleDiff >= -12.0d ) );
        }
    }
}
