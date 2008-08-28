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
    using Drive_LFSS.Storage_;
    using Drive_LFSS.Log_;
    public abstract class Button
    {
        private const byte BUTTON_MAX_COUNT = 240;
        public Button() 
        {
            //maybe a simple buttonList.Initialize(), will be ok
            for (byte itr = 0; itr < BUTTON_MAX_COUNT; itr++)
            {
                buttonList[itr] = 0;
            }
        }
        ~Button() 
        {
            if (true == false) { }
        }
        private ushort[] buttonList = new ushort[BUTTON_MAX_COUNT];
        private Dictionary<ushort, uint> buttonTimed = new Dictionary<ushort, uint>(BUTTON_MAX_COUNT);

        protected virtual void update(uint diff)
        {
            if (buttonTimed.Count > 0)
            {
                foreach (KeyValuePair<ushort, uint> keyPair in buttonTimed)
                {
                    if (keyPair.Value <= diff)
                    {
                        buttonTimed[keyPair.Key] = 0;
                        RemoveButton(keyPair.Key);
                    }
                    else
                        buttonTimed[keyPair.Key] -= diff;
                }

                Dictionary<ushort, uint>.Enumerator itr = buttonTimed.GetEnumerator();
                while (itr.MoveNext())
                {
                    if (itr.Current.Value == 0)
                    {
                        buttonTimed.Remove(itr.Current.Key);
                        itr = buttonTimed.GetEnumerator();
                    }
                }
            }
        }

        public void AddTimedButton(ushort buttonEntry, uint time, ButtonTemplateInfo buttonInfo)
        {
            SendButton(buttonInfo);
            buttonTimed.Add(buttonEntry, time);
        }
        public void RemoveButton(ushort buttonEntry)
        {
            if (((Driver)this).IsBot())
                return;

            byte buttonId = removeButtonEntry(buttonEntry);

            ((Driver)this).Session.AddToTcpSendingQueud
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_BFN,
                    Packet_Type.PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE,
                    new PacketBFN(((Driver)this).LicenceId, buttonId, Button_Function.BUTTON_FUNCTION_DEL)
                )
            );
        }
        public void SendUniqueButton(ButtonTemplateInfo buttonInfo)
        {
            if(!isButtonSended(buttonInfo.Entry))
                SendButton(buttonInfo);
        }
        public void SendButton(ButtonTemplateInfo buttonInfo)
        {
            SendButton(buttonInfo.Entry, buttonInfo.StyleMask, buttonInfo.MaxInputChar, buttonInfo.Left, buttonInfo.Top, buttonInfo.Width, buttonInfo.Height, buttonInfo.Text);
        }
        public void SendButton(ushort buttonEntry, Button_Styles_Flag buttonStyleMask, byte maxTextLength, byte left, byte top, byte width, byte height, string text)
        {
            if (((Driver)this).IsBot())
                return;
            
            byte buttonId = getNewButtonId(buttonEntry);
            if( buttonId == 0xFF )
            {
                Log.error("Button System, Button Max Count Reached for Driver:"+((Driver)this).DriverName+", Licence"+((Licence)this).LicenceName+"\r\n");
                return;
            }
            ((Driver)this).Session.AddToTcpSendingQueud
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_BTN,
                    Packet_Type.PACKET_BTN_BUTTON_DISPLAY,
                    new PacketBTN(((Driver)this).LicenceId, 1, buttonId, buttonStyleMask, maxTextLength, left, top, width, height, text)
                )
            );
        }

        private bool isButtonSended(ushort buttonEntry)
        {
            for (byte itr = 0; itr < BUTTON_MAX_COUNT; itr++)
            {
                if (buttonList[itr] == buttonEntry)
                {
                    return true;
                }
            }
            return false;
        }
        private byte getNewButtonId(ushort buttonEntry)
        {
            for (byte itr = 0; itr < BUTTON_MAX_COUNT; itr++)
            {
                if(buttonList[itr] == 0)
                {
                    buttonList[itr] = buttonEntry;
                    return itr;
                }
            }
            return 0xFF;
        }
        private byte removeButtonEntry(ushort buttonEntry)
        {
            for (byte itr = 0; itr < BUTTON_MAX_COUNT; itr++)
            {
                if (buttonList[itr] == buttonEntry)
                {
                    buttonList[itr] = 0;
                    return itr;
                }
            }
            return 0xFF;
        }
    }
}