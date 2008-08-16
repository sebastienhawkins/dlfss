using System;
using System.Collections.Generic;
using System.Text;

namespace Drive_LFSS.Script_
{
    public sealed class Script
    {
        public bool CarFinishRace(ICar _car)
        {
            return false;               //Mean There is no Custom Script Processing, True will mean you have done a script proccesing!
        }
        public bool CarAcceleration_0_100(ICar car, double finalAccelerationTime)
        {
            ((IDriver)car).SendMessage("^7 0-100Km/h In: ^2" + finalAccelerationTime + " ^0 sec.");
            
            return true;
        }
    }
}
