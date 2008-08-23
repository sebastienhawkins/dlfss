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
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Script_;

    public abstract class Button 
    {
        public Button() { }
        ~Button() { }

        private List<byte> permanentList = new List<byte>();
        private List<byte> nonPermanentList = new List<byte>();
        
        public void SendButton(bool isPermanent, byte buttonId, Button_Styles_Flag buttonStyleMask, byte maxTextLength, byte left, byte top, byte width, byte height, string text)
        {
            
            ((Driver)this).Session.AddToTcpSendingQueud
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_BTN,
                    Packet_Type.PACKET_BTN_BUTTON_DISPLAY,
                    new PacketBTN(((Driver)this).LicenceId,0,buttonId,buttonStyleMask,maxTextLength,left,top,width,height,text)
                )
            );

            if (isPermanent)
                permanentList.Add(buttonId);
            else
                nonPermanentList.Add(buttonId);

        }
    
    }
}