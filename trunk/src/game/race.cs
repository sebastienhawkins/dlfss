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
using System.Collections.Generic;
namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Database_;
    using Drive_LFSS.Script_;
    using Drive_LFSS.Log_;

    public sealed class Race : IRace
	{
        public Race(string _serverName)
        {
            serverName = _serverName;
            gridOrder = "";
        }
        private string serverName;
        private bool isRacing;
        private uint timeStart;
        private static uint guid; //Have to be initialized at Start, and we increment at each RaceEnd(Saving)

        //LFS Insim Defined var
        private Race_Feature_Flag raceFeatureMask;
        private ushort nodeFinishIndex;
        private byte connectionCount;
        private byte finishedCount;
        private ushort nodeCount;
        private byte carCount;
        private byte qualificationMinute;
        private Race_In_Progress_Status raceInProgress;
        private byte racelaps;
        private float replaySpeed;
        private ushort nodeSplit1Index;
        private ushort nodeSplit2Index;
        private ushort nodeSplit3Index;
        private string trackName;
        private Weather_Status weatherStatus;
        private Wind_Status windStatus;
        private string gridOrder;

        public void Init(PacketSTA _packet)
        {
            connectionCount = _packet.connectionCount;
            finishedCount = _packet.finishedCount;
            carCount = _packet.carCount;
            qualificationMinute = _packet.qualificationMinute;
            raceInProgress = (Race_In_Progress_Status)_packet.raceInProgress;
            racelaps = _packet.raceLaps;
            replaySpeed = _packet.replaySpeed;
            trackName = _packet.trackName;
            weatherStatus = (Weather_Status)_packet.weatherStatus;
            windStatus = (Wind_Status)_packet.windStatus;
            
            //viewOptionMask = _packet.viewOptionMask;  //This is about the Driver/Car/Licence
            //cameraMode = _packet.cameraMode;          //This is about the Driver/Car/Licence
            //currentCarId = _packet.currentCarId;      //This is about the Driver/Car/Licence
        }
        public void Init(PacketRST _packet)
        {
            trackName = _packet.trackName;
            carCount = _packet.carCount;
            nodeCount = _packet.nodeCount;
            nodeFinishIndex = _packet.nodeFinishIndex;
            raceFeatureMask = _packet.raceFeatureMask;
            qualificationMinute = _packet.qualificationMinute;
            racelaps = _packet.raceLaps;
            weatherStatus = _packet.weatherStatus;
            windStatus = _packet.windStatus;
            nodeSplit1Index = _packet.nodeSplit1Index;
            nodeSplit2Index = _packet.nodeSplit2Index;
            nodeSplit3Index = _packet.nodeSplit3Index;

            RaceStart();
        }
        public void update(uint diff)
        {
            if (isRacing)
            {
                if (raceInProgress == Race_In_Progress_Status.RACE_PROGRESS_NONE)
                {
                    Log.error("System Think Race is Starded, But LFS Server Say:" + raceInProgress + "\r\n");
                    isRacing = false;
                }
            }
        }

        private void RaceStart()
        {
            timeStart = (uint)(System.DateTime.Now.Ticks / 10000);
            isRacing = true;
        }
        private void RaceFinish()
        {
            //DatabaseStorage.race.Save((uint)1, timeStart, (uint)(System.DateTime.Now.Ticks / 10000), gridOrder);
        }
        private void RaceEnd()
        {
            timeStart = 0;
            isRacing = false;
        }
	}
}
