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

namespace Drive_LFSS.Script_
{
    //return true == Stop Default Core Action
    //return false == Continue Default Core Action
    public sealed class Script
    {
        public Script()
        {
        }
        ~Script()
        {
            if (true == false) { }
        }

        //Session
        public bool BeforeVoteStart(ISession iSession)
        {
            iSession.SendMSTMessage("/pit_all");

            //There is no default action to stop from the core for this.
            return false;
        }

        //Driver
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
