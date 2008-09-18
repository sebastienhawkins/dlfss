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
        public bool CarDriftScoring(ICar car, uint score)
        {
            ((IButton)car).AddMessageMiddle("^3Drift Score ^2" + (uint)score, 2200);

            return true;
        }
        public bool CarAccelerationSucess(ICar car, double finalAccelerationTime)
        {
            ((IButton)car).AddMessageTop(" ^70-100Km/h In: ^2" + Math.Round(finalAccelerationTime,3) + " ^7sec.",5000);
            
            return true;
        }
    }
}
