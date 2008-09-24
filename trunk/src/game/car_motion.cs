using System;
namespace Drive_LFSS.Game_
{
    public interface CarMotion
    {
        byte CarId { get; }
        string CarPrefix { get; }
        ushort GetNode();
        byte GetRacePosition();
        double GetOrientationSpeed();
        double GetOrientation();
        double GetTrajectory();
        double GetAngleToReachTraj(bool clockWise);
        double GetShorterAngleToReachTraj();
        double GetSpeedMs();
        double GetSpeedKmh();
        double GetPosX();
        double GetPosY();
        double GetPosZ();
        bool HasCollisionWarning();//Debug purpose
        void SendCollisionWarning(string text);//debug purpose
    }
}
