using System;
namespace Drive_LFSS.Game_
{
    public interface CarMotion
    {
        byte CarId { get; }
        ushort GetNode();
        double GetHeadindVelovity();
        double GetHeadingAngle();
        double GetTrajectoryAngle();
        double GetSpeedMs();
        double GetSpeedKmh();
        double GetPosX();
        double GetPosY();
        double GetPosZ();
        bool HasCollisionWarning();
        void SendCollisionWarning(string text);
    }
}
