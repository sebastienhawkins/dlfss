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

namespace Drive_LFSS.Script_
{
    public interface IButton
    {
        void AddMessageTop(string text, uint duration);
        void AddMessageMiddle(string text, uint duration);
        void AddTimedButton(ushort buttonEntry, uint time);
        void SendButton(ushort buttonEntry);
        void SendButton(byte buttonId, ushort buttonEntry, byte styleMask,  bool isAllwaysVisible, byte maxTextLength, byte left, byte top, byte width, byte height, string text, string textCaption);
        void SendUpdateButton(ushort buttonEntry, string text);
        byte RemoveButton(ushort buttonEntry);
        void SendGui(ushort guiEntry);
        void RemoveGui(ushort guiEntry);
    }
}
