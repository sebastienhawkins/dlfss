using System;
namespace Drive_LFSS.Game_
{
    public interface CarMotion
    {
        byte CarId { get; }
        ushort GetNode();
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
        bool HasCollisionWarning();
        void SendCollisionWarning(string text);
    }
}
