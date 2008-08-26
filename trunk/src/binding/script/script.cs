using System;
using System.Collections.Generic;
using System.Text;

namespace Drive_LFSS.Script_
{
    public sealed class Script
    {
        public Script()
        {
        }
        ~Script()
        {
            if (true == false) { }
        }
        public bool RaceStart()
        {
            return false;               //Mean There is no Custom Script Processing, True will mean you have done a script proccesing!
        }
        
        public bool CarFinishRace(ICar car)
        {
            return false;               //Mean There is no Custom Script Processing, True will mean you have done a script proccesing!
        }
        public bool CarAcceleration_0_100(ICar car, double finalAccelerationTime)
        {
            ((IDriver)car).SendMessage(" ^70-100Km/h In: ^2" + finalAccelerationTime + " ^0sec.");
            
            return true;
        }
    }
}
