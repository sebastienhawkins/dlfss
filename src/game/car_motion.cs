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
    interface CarMotion
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
        bool HasWarningDrivingCheck();
        void SetWarningDrivingCheck(Warning_Driving_Type _warningDrivingType, byte referenceCarId);
        bool IsYellowFlagActive();
        bool IsBlueFlagActive();
        void TrySendCancelWarning();
    }
}
