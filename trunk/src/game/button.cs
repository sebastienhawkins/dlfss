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
namespace Drive_LFSS.Game_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Packet_;
    using Drive_LFSS.Session_;
    using Drive_LFSS.Script_;
    using Drive_LFSS.Storage_;
    using Drive_LFSS.Log_;

    public abstract class Button : IButton
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
        public void ProcessBFNClearAll()
        {
            for (byte itr = 0; itr < BUTTON_MAX_COUNT; itr++)
            {
                buttonList[itr] = 0;
            }
            if (!((ICar)this).IsOnTrack())
            {
                SendBanner();
                SendTrackPrefix();
            }
        }
        private ushort[] buttonList = new ushort[BUTTON_MAX_COUNT];

        private class ButtonTimed
        {
            public ButtonTimed(ushort _buttonEntry,uint _time)
            {
                buttonEntry = _buttonEntry;
                time = _time;
            }
            ~ButtonTimed()
            {
                if (true == false) { }
            }
            private ushort buttonEntry;
            private uint time;

            public ushort ButtonEntry
            {
                get { return buttonEntry; }
                set { buttonEntry = value; }
            }
            public uint Time
            {
                get { return time; }
                set { time = value; }
            }

        }
        private List<ButtonTimed> buttonTimedList = new List<ButtonTimed>(BUTTON_MAX_COUNT);
        private class ButtonMessage
        {
            public ButtonMessage(string _text,uint _time)
            {
                text = _text;
                time = _time;
            }
            ~ButtonMessage()
            {
                if (true == false) { }
            }
            private string text;
            private uint time;

            public string Text
            {
                get { return text; }
                set { text = value; }
            }
            public uint Time
            {
                get { return time; }
                set { time = value; }
            }
        }
        private Queue<ButtonMessage> buttonMessageTop = new Queue<ButtonMessage>(BUTTON_MAX_COUNT);
        private Queue<ButtonMessage> buttonMessageMiddle = new Queue<ButtonMessage>(BUTTON_MAX_COUNT);

        protected virtual void update(uint diff)
        {
            if (buttonTimedList.Count > 0)
            {
                for (byte itr = 0; itr < buttonTimedList.Count; itr++ )
                {
                    if (buttonTimedList[itr].Time > diff)
                        buttonTimedList[itr].Time -= diff;
                    else
                    {
                        buttonTimedList[itr].Time = 0;
                        RemoveButton(buttonTimedList[itr].ButtonEntry);
                    }
                }
                List<ButtonTimed>.Enumerator clrItr = buttonTimedList.GetEnumerator();
                while (clrItr.MoveNext())
                {
                    if (clrItr.Current.Time == 0)
                    {
                        buttonTimedList.Remove(clrItr.Current);
                        clrItr = buttonTimedList.GetEnumerator();
                    }
                }
            }
            if (buttonMessageTop.Count > 0)
            {
                ButtonMessage currentButton = buttonMessageTop.Peek();
                if (currentButton.Text != "")
                {
                    ButtonTemplateInfo newButton = Program.buttonTemplate.GetEntry((uint)Button_Entry.MESSAGE_BAR_TOP);
                    newButton.Text = currentButton.Text;
                    AddTimedButton(currentButton.Time - 200, newButton);//Since time is not very important, use this way to be sure im sync with timedButton
                    currentButton.Text = "";
                }

                if (currentButton.Time > diff)
                    currentButton.Time -= diff;
                else
                    buttonMessageTop.Dequeue();
            }
            if (buttonMessageMiddle.Count > 0)
            {
                ButtonMessage currentButton = buttonMessageMiddle.Peek();
                if (currentButton.Text != "")
                {
                    ButtonTemplateInfo newButton = Program.buttonTemplate.GetEntry((uint)Button_Entry.MESSAGE_BAR_MIDDLE);
                    newButton.Text = currentButton.Text;
                    AddTimedButton(currentButton.Time-200, newButton); //Since time is not very important, use this way to be sure im sync with timedButton
                    currentButton.Text = "";
                }

                if (currentButton.Time > diff)
                    currentButton.Time -= diff;
                else
                    buttonMessageMiddle.Dequeue();
            }
        }


        public void SendGui(ushort guiEntry)
        {
            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            SendGui(guiInfo);
        }
        public void SendGui(GuiTemplateInfo guiInfo)
        {
            if (((Driver)this).IsBot())
                return;

            string[] buttonEntrys = guiInfo.ButtonEntry.Split(new char[] { ' ' });
            ButtonTemplateInfo buttonInfo;
            
            System.Collections.IEnumerator itr =  buttonEntrys.GetEnumerator();
            ushort buttonEntry;
            while (itr.MoveNext())
            {
                buttonEntry = System.Convert.ToUInt16(itr.Current);
                buttonInfo = Program.buttonTemplate.GetEntry(buttonEntry);
                SendButton(newButtonId(buttonEntry), buttonInfo);
            }
            if (guiInfo.TextButtonEntry > 0 && guiInfo.Text.Length > 0)
            {
                buttonInfo = Program.buttonTemplate.GetEntry(guiInfo.TextButtonEntry);
                ButtonTemplateInfo buttonInfoCopy;

                string[] lines = guiInfo.Text.Split(new string[]{Environment.NewLine},StringSplitOptions.RemoveEmptyEntries);
                for (byte lineItr = 0; lineItr < lines.Length; lineItr++)
                {
                    buttonInfoCopy = (ButtonTemplateInfo)buttonInfo.Clone();
                    buttonInfoCopy.Top = (byte) (((lineItr) * buttonInfoCopy.Height) + buttonInfoCopy.Top + 1);
                    buttonInfoCopy.Text = lines[lineItr];
                    SendButton(newButtonId(buttonInfoCopy.Entry), buttonInfoCopy);
                }
            }
        }
        public void RemoveGui(ushort guiEntry)
        {
            if (((Driver)this).IsBot())
                return;

            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry(guiEntry);
            string[] buttonEntrys = guiInfo.ButtonEntry.Split(new char[] { ' ' });

            System.Collections.IEnumerator itr = buttonEntrys.GetEnumerator();
            ushort buttonEntry;
            while (itr.MoveNext())
            {
                buttonEntry = System.Convert.ToUInt16(itr.Current);
                RemoveButton(buttonEntry);
            }
            if (guiInfo.TextButtonEntry > 0 && guiInfo.Text.Length > 0)
            {
                RemoveButton(guiInfo.TextButtonEntry);
            }

        }
        public byte RemoveButton(ushort buttonEntry)
        {
            if (((Driver)this).IsBot())
                return 0xFF;

            byte previousId;
            byte buttonId = previousId = removeButtonEntry(buttonEntry);
            while (buttonId != 0xFF)
            {
                SendRemoveButton(buttonId);
                previousId = buttonId;
                buttonId = removeButtonEntry(buttonEntry);
            }
            return previousId;
        }
        private void SendRemoveButton(byte buttonId)
        {
            ((Session)((Driver)this).ISession).AddToTcpSendingQueud
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_BFN,
                    Packet_Type.PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE,
                    new PacketBFN(((Driver)this).LicenceId, buttonId, Button_Function.BUTTON_FUNCTION_DEL)
                )
            );
        }
        public void AddMessageTop(string text, uint duration)
        {
            if (((Driver)this).IsBot())  //Become Redondant with SendButton, but since they since timer better this way.
                return;

            lock (buttonMessageTop)
            {
                buttonMessageTop.Enqueue(new ButtonMessage(text, duration));
            }
        }
        public void AddMessageMiddle(string text, uint duration)
        {
            if (((Driver)this).IsBot()) //Become Redondant with SendButton, but since they since timer better this way.
                return;
            lock (buttonMessageMiddle)
            {
                buttonMessageMiddle.Enqueue(new ButtonMessage(text, duration));
            }
        }
        public void AddTimedButton(ushort buttonEntry, uint time)
        {
            AddTimedButton(time, Program.buttonTemplate.GetEntry(buttonEntry));
        }
        public void AddTimedButton(uint time, ButtonTemplateInfo buttonInfo)
        {
            lock (buttonTimedList) 
            {
                buttonTimedList.Add(new ButtonTimed(buttonInfo.Entry, time));
            }
            SendButton(newButtonId(buttonInfo.Entry),buttonInfo);
        }
        public void SendUniqueButton(ushort buttonEntry)
        {
            ButtonTemplateInfo buttonInfo = Program.buttonTemplate.GetEntry((uint)buttonEntry);
            SendUniqueButton(buttonInfo);
        }
        public void SendUniqueButton(ButtonTemplateInfo buttonInfo)
        {
            if (GetButtonId(buttonInfo.Entry) == 0xFF)
                SendButton(newButtonId(buttonInfo.Entry), buttonInfo);
        }
        public void SendUpdateButton(ButtonTemplateInfo buttonInfo)
        {
            byte buttonId = GetButtonId(buttonInfo.Entry);
            if (buttonId != 0xFF)
            {
                buttonInfo.Height = 0;
                SendButton(buttonId,buttonInfo);
            }
            else
            {
                buttonId = newButtonId(buttonInfo.Entry);
                SendButton(buttonId,buttonInfo);
            }
        }
        public void SendUpdateButton(ushort buttonEntry, string text)
        {
            ButtonTemplateInfo buttonInfo = Program.buttonTemplate.GetEntry((uint)buttonEntry);
            buttonInfo.Text = text;
            SendUpdateButton(buttonInfo);
        }
        public void SendButton(ushort buttonEntry)
        {
            ButtonTemplateInfo buttonInfo = Program.buttonTemplate.GetEntry((uint)buttonEntry);
            SendButton(newButtonId(buttonInfo.Entry),buttonInfo);
        }
        public void SendButton(byte buttonId, ButtonTemplateInfo buttonInfo)
        {
            SendButton(buttonId, buttonInfo.Entry, (byte)buttonInfo.StyleMask, buttonInfo.IsAllwaysVisible, buttonInfo.MaxInputChar, buttonInfo.Left, buttonInfo.Top, buttonInfo.Width, buttonInfo.Height, buttonInfo.Text);
        }
        public void SendButton(byte buttonId, ushort buttonEntry, byte buttonStyleMask, bool isAllwaysVisible, byte maxTextLength, byte left, byte top, byte width, byte height, string text)
        {
            if (((Driver)this).IsBot())
                return;

            if( buttonId == 0xFF )
            {
                Log.error("Button System, Button Max Count Reached for Driver:"+((Driver)this).DriverName+", Licence"+((Licence)this).LicenceName+"\r\n");
                return;
            }
            ((Session)((Driver)this).ISession).AddToTcpSendingQueud
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_BTN,
                    Packet_Type.PACKET_BTN_BUTTON_DISPLAY,
                    new PacketBTN(((Licence)this).LicenceId, 1, buttonId, (Button_Styles_Flag)buttonStyleMask, isAllwaysVisible, maxTextLength, left, top, width, height, text)
                )
            );
        }

        public void SendBanner()
        {
            //Send Banner
            SendUniqueButton((ushort)Button_Entry.BANNER);
        }
        public void RemoveBanner()
        {
            RemoveButton((ushort)Button_Entry.BANNER);
        }
        public void SendTrackPrefix()
        {
            SendUpdateButton((ushort)Button_Entry.TRACK_PREFIX, "^8"+((Driver)this).ISession.GetRaceTrackPrefix());
        }
        public void RemoveTrackPrefix()
        {
            RemoveButton((ushort)Button_Entry.TRACK_PREFIX);
        }

        private byte GetButtonId(ushort buttonEntry)
        {
            for (byte itr = 0; itr < BUTTON_MAX_COUNT; itr++)
            {
                if (buttonList[itr] == buttonEntry)
                {
                    return itr;
                }
            }
            return 0xFF;
        }
        private byte newButtonId(ushort buttonEntry)
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
        public ushort GetButtonEntry(byte buttonId)
        {
            return buttonList[buttonId];
        }

    }
}