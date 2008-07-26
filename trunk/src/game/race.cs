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
namespace Drive_LFSS.Game_
{
    using System.Collections.Generic;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Definition_;

    public sealed class Race
	{
        private Licence_View_Option viewOptionMask;
        private Race_Feature_Flag raceFeatureMask;
        private ushort nodeFinishIndex;
        private Licence_Camera_Mode cameraMode;
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

        public void Init(PacketSTA _packet)
        {
            connectionCount = _packet.connectionCount;
            finishedCount = _packet.finishedCount;
            carCount = _packet.carCount;
            qualificationMinute = _packet.qualificationMinute;
            raceInProgress = _packet.raceInProgress;
            racelaps = _packet.raceLaps;
            replaySpeed = _packet.replaySpeed;
            trackName = _packet.trackName;
            weatherStatus = _packet.weatherStatus;
            windStatus = _packet.windStatus;
            
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
        }
        public void update(uint diff)
        {

        }
	}
}
