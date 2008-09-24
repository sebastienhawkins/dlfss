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

        //diff == the time passed from the last call here.
        //anything you need to timer.
        public void update(uint diff)
        {
        }

        //this happen when a race is completed success
        public bool RaceCompleted(string startOrder, string finishOrder)
        {
            return false;
        }
        
        //Any action you wich to do when a track vote start.
        public bool NextTrackVoteStarted(ISession iSession)
        {
            iSession.SendMSTMessage("/pit_all");

            return false; //return true, will never start a vote
        }

        //Any action you wich do do when a track vote is ended.
        //trackEntry is passed by a reference, so you can change it.
        public bool NextTrackVoteEnded(IVote vote,ref ushort trackEntry)
        {
            if (trackEntry != 0)
                return false;                                    //return false, will prepare the trackEntry

            vote.StartNextTrackVote();              //restart The vote, since no track has been Selected(trackEntry == 0)
            return true;                                          //return true, will not try to prepare the next track.
        }

        //this happen when a player got a drift score
        public bool CarDriftScoring(ICar car, uint score)
        {
            //((IButton)car).AddMessageMiddle("^3Drift Score ^2" + (uint)score, 2200);

            return false;                            //true or false change nothing.
        }
        
        //this happen when a player do a 0-100Kmh acceleration.
        public bool CarAccelerationSucess(ICar car, ushort startKmh, ushort endKmh, double finalAccelerationTime)
        {
            ((IButton)car).AddMessageTop(" ^7 " + startKmh + "^2 - ^7" + endKmh + " ^2Km/h In: ^7" + Math.Round(finalAccelerationTime, 3) + " ^2sec.", 4500);

           return false;                             //true or false change nothing.
        }
    }
}
