using System;
using System.Collections.Generic;
using System.Text;

namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Log_;

    sealed class Grid
    {
        public Grid(ushort _nodeCount)
        {
            carContainer = new Dictionary<CarMotion, ushort>();
            nodeCount = _nodeCount;
        }
        ~Grid()
        {
            if (true == false) { }
        }
        private ushort nodeCount;
        private Dictionary<CarMotion, ushort> carContainer;

        public void Add(CarMotion car)
        {
            carContainer[car] = car.GetNode();
        }
        public void ProcessCarInformation(CarMotion car)
        {
            if (car.GetSpeedMs() < 0.1) //About 0.01 m seconde
                return;
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
            //we repeat findNodeCars(), but that way we keep good position into the find!
            //if we don't need this, Makazazo, sugestion will work greater.
            //Maybe a synchro stack with node as key, i think this one will be faster and have both quality.
            if(nextNodeCount > 0)
            {
                for (itr = 1; itr <= nextNodeCount; itr++)
                {
                    newItr = carContainer[car] + itr;
                    if (newItr > nodeCount)
                        newItr = 1; //Need to know witch is the first NODE, 0 or 1
                    carIds.AddRange(findNodeCars((ushort)newItr));
                }
            }

            carIds.AddRange(findNodeCars(carContainer[car]));
            
            if (previousNodeCount > 0)
            {
                for (itr = 1; itr <= nextNodeCount; itr++)
                {
                    newItr = carContainer[car] - itr;
                    if (newItr < 1)
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

        private const double speedColRatio = 0.71d;
        private void checkCollision(CarMotion car)
        {
            CarMotion[] carAround;
            carAround = getCarAround(car, 2, 2);
            for (byte itr = 0; itr < carAround.Length; itr++ )
            {
                if (carAround[itr] == car) //Remove Self
                    continue;

                double speedDiff = ((car.GetSpeedKmh() - carAround[itr].GetSpeedKmh()));
                double dist = GetDistance2DSq(car, carAround[itr]);
                if (speedDiff < 0 && (dist * dist * speedColRatio) < -speedDiff //this is BAD, but need to find car brake range... :(
                    && GetDistanceZ(car, carAround[itr]) < 1.0d && HasCollisionPath(carAround[itr], car))
                {

                    if (!carAround[itr].HasCollisionWarning())
                        carAround[itr].SendCollisionWarning("^1What a Shame, Get out of the Car.");
                    if (!car.HasCollisionWarning())
                        car.SendCollisionWarning("^8Driver \"^2" + ((Driver)carAround[itr]).DriverName + "^8\" Seem to have no Brake!");
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
            double bothSize = 1.0d;
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

            //return (angle >= 0) ? angle : 2 * M_PI + angle;
            return angle * 180.0f / Math.PI;
        }
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

            //TODO: need to find the perfect angle, based on a static car size.
            return ( (angleDiff <= 14.0d) && (angleDiff >= -14.0d ) );
        }
    }
}
