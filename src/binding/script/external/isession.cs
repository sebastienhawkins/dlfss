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

namespace Drive_LFSS.Script_
{
    public interface ISession
    {
        bool IsFreezeMotdSend();
        int GetLatency();
        int GetReactionTime();
        byte GetNbrOfDrivers();
        byte GetNbrOfConnection();
        uint GetRaceGuid();
        bool IsRaceInProgress();
        string GetRaceTrackPrefix();
        byte GetRaceLapCount();
        string GetSessionNameForLog();
        string GetSessionName();
        void SendMSTMessage(string message);
        void SendMSXMessage(string message);
        void SendMTCMessageToAllAdmin(string message);
        void SendUpdateButtonToAll(ushort buttonEntry, string text);
        void RemoveButtonToAll(ushort buttonEntry);
        void RemoveButton(ushort buttonEntry,byte licenceId);
        void AddMessageTopToAll(string text, uint duration);
        void AddMessageMiddleToAll(string text, uint duration);
        void SendFlagRaceToAll(ushort guiEntry, uint time);
        void RemoveFlagRaceToAll(ushort guiEntry);
        bool CanVote();
        void EndRace();
        void SendResultGuiToAll(Dictionary<string, int> scoringResultTextDisplay);
        IDriver GetDriverWithGuid(uint guid);
        IDriver GetCarId(byte carId);
        IDriver GetDriverWithConnectionId(byte connectionId);
        Script Script
        {
            get;
        }
    }
}
