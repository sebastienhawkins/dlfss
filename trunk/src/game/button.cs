﻿/* 
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
    using Definition_;
    using Packet_;
    using Game_;
    using Script_;
    using Storage_;
    using Log_;
    using Ranking_;

    internal abstract class Button
    {
        private const byte BUTTON_MAX_COUNT = 240;
        internal protected Button() 
        {
        }
        ~Button() 
        {
            if (true == false) { }
        }
        internal void ProcessBFNClearAll(bool sendConfig)
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
            rankGuiCurrentDisplay = Button_Entry.NONE;
            currentGui = 0;
            if(sendConfig)
                SendConfigGui();
        }
        internal void ProcessBFNRequest()
        {
            SendConfigGui();
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
        private Gui_Entry currentGui = Gui_Entry.NONE;
        private const uint FREEZE_BUTTON_CLEAR = 1000;
        private const uint FREEZE_BUTTON = 1000;
        private uint freezeButtonClear = 0;
        private uint freezeButton = 0;
        private Button_Entry rankGuiCurrentDisplay = Button_Entry.NONE;
        private byte rankSearchDisplayCount = 0;
        private string rankSearchTrackPrefix = "";
        private string rankSearchCarPrefix = "";
        private Queue<Packet> bufferButtonPacket = new Queue<Packet>();
        private uint timerBufferedButton = 0;
        private const uint MAX_BUTTON_BY_CYCLE = 10;
        
        new protected virtual void update(uint diff)
        {
            lock(bufferButtonPacket)
            {
                int buttonBufferCount = bufferButtonPacket.Count;
                if(timerBufferedButton < diff)
                {
                    timerBufferedButton = 0;
                    for(byte count = 0; count < buttonBufferCount ; count++)
                    {
                        ((Session)((Driver)this).ISession).AddToTcpSendingQueud(bufferButtonPacket.Dequeue());
                        if(count >= MAX_BUTTON_BY_CYCLE)
                        {
                            timerBufferedButton = 100;
                            break;
                        }
                    }
                    
                }
                else
                   timerBufferedButton -= diff; 
            }

            
            if(freezeButtonClear > 0)
            {
                if(freezeButtonClear < diff)
                {
                    SendUpdateButton(Button_Entry.RANK_BUTTON_TOP20,"^2Top20");
                    SendUpdateButton(Button_Entry.RANK_BUTTON_SEARCH,"^2Search");
                    SendUpdateButton(Button_Entry.RANK_BUTTON_CURRENT,"^2Current");
                    freezeButtonClear = 0;
                }
                else
                    freezeButtonClear -= diff;
            }
            if(freezeButton > 0)
            {
                if(freezeButton < diff)
                {
                    freezeButton = 0;

                }
                else
                    freezeButton -= diff; 
            }
            if (buttonTimedList.Count > 0)
            {
                for (byte itr = 0; itr < buttonTimedList.Count; itr++ )
                {
                    if (buttonTimedList[itr].Time - diff > diff) //timer on button are maybe time to be rewrite, we have a dsync why this method, hack like.
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
                    AddTimedButton(currentButton.Time, newButton);
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
                    AddTimedButton(currentButton.Time, newButton); //Since time is not very important, use this way to be sure im sync with timedButton
                    currentButton.Text = "";
                }

                if (currentButton.Time > diff)
                    currentButton.Time -= diff;
                else
                    buttonMessageMiddle.Dequeue();
            }
        }

        internal protected void SendGui(ushort guiEntry, string text)
        {
            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            guiInfo.Text = text;
            SendGui(guiInfo);
        }
        public void SendGui(ushort guiEntry)
        {
            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            SendGui(guiInfo);
        }
        private void SendGui(GuiTemplateInfo guiInfo)
        {
            if (((Driver)this).IsBot())
                return;
            if (guiInfo.Entry == currentGui)
                return;

            if(currentGui > 0)
                RemoveGui(currentGui);
            
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
            currentGui = guiInfo.Entry;
        }
        public void RemoveGui(ushort guiEntry)
        {
            RemoveGui((Gui_Entry)guiEntry);
        }
        internal void RemoveGui(Gui_Entry guiEntry)
        {
            if (((Driver)this).IsBot())
                return;

            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            string[] buttonEntrys = guiInfo.ButtonEntry.Split(new char[] { ' ' },StringSplitOptions.RemoveEmptyEntries);
            System.Collections.IEnumerator itr = buttonEntrys.GetEnumerator();
            ushort buttonEntry;
            while (itr.MoveNext())
            {
                buttonEntry = System.Convert.ToUInt16(itr.Current);
                RemoveButton(buttonEntry);
            }
            string[] buttonEntryExts = guiInfo.ButtonEntryExt.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            itr = buttonEntryExts.GetEnumerator();
            while (itr.MoveNext())
            {
                buttonEntry = System.Convert.ToUInt16(itr.Current);
                RemoveButton(buttonEntry);
            }

            if (guiInfo.TextButtonEntry > 0 /*&& guiInfo.Text.Length > 0*/)
            {
                RemoveButton(guiInfo.TextButtonEntry);
            }
            currentGui = Gui_Entry.NONE;
        }
        internal protected byte RemoveButton(Button_Entry buttonEntry)
        {
            return RemoveButton((ushort)buttonEntry);
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
            AddNextBufferedButton
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_BFN,
                    Packet_Type.PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE,
                    new PacketBFN(((IDriver)this).ConnectionId, buttonId, Button_Function.BUTTON_FUNCTION_DEL)
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
        private void AddTimedButton(ushort buttonEntry, uint time)
        {
            AddTimedButton(time, Program.buttonTemplate.GetEntry(buttonEntry));
        }
        private void AddTimedButton(uint time, ButtonTemplateInfo buttonInfo)
        {
            lock (buttonTimedList) 
            {
                buttonTimedList.Add(new ButtonTimed(buttonInfo.Entry, time));
            }
            //SendButton(newButtonId(buttonInfo.Entry),buttonInfo);
            SendUpdateButton(buttonInfo);
        }
        private void SendUniqueButton(ushort buttonEntry)
        {
            ButtonTemplateInfo buttonInfo = Program.buttonTemplate.GetEntry((uint)buttonEntry);
            SendUniqueButton(buttonInfo);
        }
        private void SendUniqueButton(ButtonTemplateInfo buttonInfo)
        {
            if (GetButtonId(buttonInfo.Entry) == 0xFF)
                SendButton(newButtonId(buttonInfo.Entry), buttonInfo);
        }
        private void SendUpdateButton(ButtonTemplateInfo buttonInfo)
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
        protected void SendUpdateButton(Button_Entry buttonEntry)
        {
            SendUpdateButton((ushort)buttonEntry, "");
        }
        internal void SendUpdateButton(Button_Entry buttonEntry, string text)
        {
            SendUpdateButton((ushort)buttonEntry,text);
        }
        public void SendUpdateButton(ushort buttonEntry, string text)
        {
            ButtonTemplateInfo buttonInfo = Program.buttonTemplate.GetEntry((uint)buttonEntry);
            if(text != "")
                buttonInfo.Text = text;
            SendUpdateButton(buttonInfo);
        }
        public void SendButton(ushort buttonEntry)
        {
            SendButton((Button_Entry)buttonEntry);
        }
        protected void SendButton(Button_Entry buttonEntry)
        {
            ButtonTemplateInfo buttonInfo = Program.buttonTemplate.GetEntry((uint)buttonEntry);
            SendButton(newButtonId(buttonInfo.Entry), buttonInfo);
        }
        private void SendButton(byte buttonId, ButtonTemplateInfo buttonInfo)
        {
            SendButton(buttonId, buttonInfo.Entry, (byte)buttonInfo.StyleMask, buttonInfo.IsAllwaysVisible, buttonInfo.MaxInputChar, buttonInfo.Left, buttonInfo.Top, buttonInfo.Width, buttonInfo.Height, buttonInfo.Text, buttonInfo.TextCaption);
        }
        private void SendButton(byte buttonId, ushort buttonEntry, byte buttonStyleMask, bool isAllwaysVisible, byte maxTextLength, byte left, byte top, byte width, byte height, string text, string textCaption)
        {
            if (((Driver)this).IsBot())
                return;

            if( buttonId == 0xFF )
            {
                Log.error("Button system> Max button count reached for Driver: " + ((IDriver)this).DriverName + ", Licence: " + ((IDriver)this).LicenceName + "\r\n");
                return;
            }

            if (maxTextLength > 0 && textCaption != "")
            {
                text = ((char)0).ToString() + textCaption + ((char)0).ToString() + text + ((char)0).ToString();
            }
            AddNextBufferedButton
            (
                new Packet
                (
                    Packet_Size.PACKET_SIZE_BTN,
                    Packet_Type.PACKET_BTN_BUTTON_DISPLAY,
                    new PacketBTN(((IDriver)this).ConnectionId, 1, buttonId, (Button_Styles_Flag)buttonStyleMask, isAllwaysVisible, maxTextLength, left, top, width, height, text)
                )
            );
        }

        private void AddNextBufferedButton(Packet _packet)
        {
            bufferButtonPacket.Enqueue(_packet);
        }

        internal void SendBanner()
        {
            //Send Banner
            SendUniqueButton((ushort)Button_Entry.BANNER);
        }
        internal void RemoveBanner()
        {
            RemoveButton((ushort)Button_Entry.BANNER);
        }
        internal void SendTrackPrefix()
        {
            ButtonTemplateInfo button = Program.buttonTemplate.GetEntry((uint)Button_Entry.TRACK_PREFIX);
            button.Text += button.Text + ((Driver)this).ISession.GetRaceTrackPrefix();
            SendUpdateButton(button);
        }
        internal void RemoveTrackPrefix()
        {
            RemoveButton((ushort)Button_Entry.TRACK_PREFIX);
        }
        internal void SendConfigGui()
        {
            SendGui((ushort)Gui_Entry.CONFIG_USER);
            
            SendUpdateButton(Button_Entry.CONFIG_USER_ACC_CURRENT, "^7" + ((Driver)this).GetAccelerationStartSpeed() + "^2-^7" + ((Driver)this).GetAccelerationEndSpeed() + " ^2Kmh");
            SendUpdateButton(Button_Entry.CONFIG_USER_ACC_ON, (((Driver)this).IsAccelerationOn() ? "^7" : "^8") + " Acceleration");
            SendUpdateButton(Button_Entry.CONFIG_USER_DRIFT_ON, (((Driver)this).IsDriftScoreOn() ? "^7" : "^8") + " Drift Score");
            SendUpdateButton(Button_Entry.CONFIG_USER_MAX_SPEED_ON, (((Driver)this).IsMaxSpeedDisplay ? "^7" : "^8") + " Max Speed");

            SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_ALL, ((((Driver)this).IsTimeDiffSplitDisplay && ((Driver)this).IsTimeDiffLapDisplay) ? "^7" : "^8") + " Time Diff");
            SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_LAP, (((Driver)this).IsTimeDiffLapDisplay ? "^7" : "^8") + " PB vs lap");
            SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_SPLIT, (((Driver)this).IsTimeDiffSplitDisplay ? "^7" : "^8") + " PB vs Split");
        }
        internal void RemoveConfigGui()
        {
            RemoveGui(Gui_Entry.CONFIG_USER);
        }
        internal void SendHelpGui()
        {
            SendGui((ushort)Gui_Entry.HELP);
        }
        internal void RemoveHelpGui()
        {
            RemoveGui(Gui_Entry.HELP);
        }
        internal void SendRankGui(Button_Entry startWith)
        {
            SendGui((ushort)Gui_Entry.RANK);
            switch(startWith)
            {
                case Button_Entry.RANK_BUTTON_TOP20:
                    SendRankTop20();break;
                case Button_Entry.RANK_BUTTON_CURRENT:
                    SendRankCurrent(0);break;
            }
            
        }
        private void ClearRankDisplay(bool searchDisplay)
        {
            SendUpdateButton(Button_Entry.RANK_BUTTON_TOP20,"^1Top20");
            SendUpdateButton(Button_Entry.RANK_BUTTON_SEARCH,"^1Search");
            SendUpdateButton(Button_Entry.RANK_BUTTON_CURRENT,"^1Current");
            
            if(!searchDisplay)
            {
                if(rankGuiCurrentDisplay == Button_Entry.RANK_BUTTON_SEARCH)
                {
                    RemoveButton(Button_Entry.RANK_SEARCH_BUTTON_CAR);
                    RemoveButton(Button_Entry.RANK_SEARCH_BUTTON_LICENCE);
                    RemoveButton(Button_Entry.RANK_SEARCH_BUTTON_TRACK);
                }
                rankGuiCurrentDisplay = Button_Entry.NONE;
            }
            RemoveButton(Button_Entry.RANK_NAME);
            RemoveButton(Button_Entry.RANK_PB);
            RemoveButton(Button_Entry.RANK_AVERAGE);
            RemoveButton(Button_Entry.RANK_STABILITY);
            RemoveButton(Button_Entry.RANK_WIN);
            RemoveButton(Button_Entry.RANK_TOTAL);
            RemoveButton(Button_Entry.RANK_POSITION);
            SendUpdateButton(Button_Entry.RANK_INFO,"^1Cleared");
        }
        internal void SendRankCurrent(byte witchPage)
        {
            if(rankGuiCurrentDisplay != Button_Entry.NONE)
                 ClearRankDisplay(false);

            rankGuiCurrentDisplay = Button_Entry.RANK_BUTTON_CURRENT;
            SendUpdateButton(Button_Entry.RANK_BUTTON_TOP20,"^2Top20");
            SendUpdateButton(Button_Entry.RANK_BUTTON_SEARCH,"^2Search");
            SendUpdateButton(Button_Entry.RANK_BUTTON_CURRENT,"^7Current");
            
            string trackPrefix = ((Driver)this).ISession.GetRaceTrackPrefix();
            string carPrefix = ((ICar)this).CarPrefix;
            uint rankedCount = Ranking.GetRankedCount(trackPrefix,carPrefix);
            SendUpdateButton(Button_Entry.RANK_INFO,"^2Car: ^7"+carPrefix+", ^2Track:^7 "+trackPrefix+", ^2Count: ^7"+rankedCount);
            if(rankedCount < 1)
                return;
            List<Driver> driver = ((Session)((Driver)this).ISession).GetDriverList();
            
            ButtonTemplateInfo bName = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_NAME);
            ButtonTemplateInfo bPB = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_PB);
            ButtonTemplateInfo bAverage = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_AVERAGE);
            ButtonTemplateInfo bStability = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_STABILITY);
            ButtonTemplateInfo bWin = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_WIN);
            ButtonTemplateInfo bTotal = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_TOTAL);
            ButtonTemplateInfo bPosition = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_POSITION);
            SendButton(Button_Entry.RANK_NAME);
            SendButton(Button_Entry.RANK_PB);
            SendButton(Button_Entry.RANK_AVERAGE);
            SendButton(Button_Entry.RANK_STABILITY);
            SendButton(Button_Entry.RANK_WIN);
            SendButton(Button_Entry.RANK_TOTAL);
            SendButton(Button_Entry.RANK_POSITION);

            byte top = bName.Top;
            byte height = bName.Height;
            byte rankedCountSended = 0;
            witchPage = (byte)(20*witchPage); 
            for(byte itr = witchPage; itr < driver.Count && itr < witchPage+20; itr++)
            {
                if(driver[itr].IsBot())
                    continue;
                Rank rank = driver[itr].GetRank(trackPrefix,carPrefix);
                if(rank != null)
                {
                   
                   bName.Text = "^2"+driver[itr].LicenceName;
                   bName.Top = (byte)((height*(rankedCountSended+1))+top);
                   bPB.Text = rank.GetRankGuiString[0];
                   bPB.Top = (byte)((height*(rankedCountSended+1))+top);
                   bAverage.Text = rank.GetRankGuiString[1];
                   bAverage.Top = (byte)((height*(rankedCountSended+1))+top);
                   bStability.Text = rank.GetRankGuiString[2];
                   bStability.Top = (byte)((height*(rankedCountSended+1))+top);
                   bWin.Text = rank.GetRankGuiString[3];
                   bWin.Top = (byte)((height*(rankedCountSended+1))+top);
                   bTotal.Text = rank.GetRankGuiString[4];
                   bTotal.Top = (byte)((height*(rankedCountSended+1))+top);
                   bPosition.Text = rank.GetRankGuiString[5];
                   bPosition.Top = (byte)((height*(rankedCountSended+1))+top);
                   
                   SendButton(newButtonId(Button_Entry.RANK_NAME), bName);
                   SendButton(newButtonId(Button_Entry.RANK_PB), bPB);
                   SendButton(newButtonId(Button_Entry.RANK_AVERAGE), bAverage);
                   SendButton(newButtonId(Button_Entry.RANK_STABILITY), bStability);
                   SendButton(newButtonId(Button_Entry.RANK_WIN), bWin);
                   SendButton(newButtonId(Button_Entry.RANK_TOTAL), bTotal);
                   SendButton(newButtonId(Button_Entry.RANK_POSITION), bPosition); 
                   
                   rankedCountSended++;
                }
                else
                {
                   bName.Text = "^2"+driver[itr].LicenceName;
                   bName.Top = (byte)((height*(rankedCountSended+1))+top);
                   bPB.Text = "^80";
                   bPB.Top = (byte)((height*(rankedCountSended+1))+top);
                   bAverage.Text = "^80";
                   bAverage.Top = (byte)((height*(rankedCountSended+1))+top);
                   bStability.Text = "^80";
                   bStability.Top = (byte)((height*(rankedCountSended+1))+top);
                   bWin.Text = "^80";
                   bWin.Top = (byte)((height*(rankedCountSended+1))+top);
                   bTotal.Text = "^80";
                   bTotal.Top = (byte)((height*(rankedCountSended+1))+top);
                   bPosition.Text = "^1NA";
                   bPosition.Top = (byte)((height*(rankedCountSended+1))+top);
                   
                   SendButton(newButtonId(Button_Entry.RANK_NAME), bName);
                   SendButton(newButtonId(Button_Entry.RANK_PB), bPB);
                   SendButton(newButtonId(Button_Entry.RANK_AVERAGE), bAverage);
                   SendButton(newButtonId(Button_Entry.RANK_STABILITY), bStability);
                   SendButton(newButtonId(Button_Entry.RANK_WIN), bWin);
                   SendButton(newButtonId(Button_Entry.RANK_TOTAL), bTotal);
                   SendButton(newButtonId(Button_Entry.RANK_POSITION), bPosition); 
                   
                   rankedCountSended++; 
                }
                    
            }
            
        }
        internal void SendRankSearch()
        {
            if(rankGuiCurrentDisplay != Button_Entry.NONE)
                ClearRankDisplay(false);

            rankGuiCurrentDisplay = Button_Entry.RANK_BUTTON_SEARCH;
            rankSearchDisplayCount = 0;
            SendUpdateButton(Button_Entry.RANK_BUTTON_TOP20,"^2Top20");
            SendUpdateButton(Button_Entry.RANK_BUTTON_SEARCH,"^7Search");
            SendUpdateButton(Button_Entry.RANK_BUTTON_CURRENT,"^2Current");
            SendButton(Button_Entry.RANK_SEARCH_BUTTON_TRACK);
            SendButton(Button_Entry.RANK_SEARCH_BUTTON_CAR);
            SendButton(Button_Entry.RANK_SEARCH_BUTTON_LICENCE);
            uint rankedCount = Ranking.GetRankedCount(rankSearchTrackPrefix,rankSearchCarPrefix);
            SendUpdateButton(Button_Entry.RANK_INFO,"^2Car: ^7"+rankSearchCarPrefix+", ^2Track:^7 "+rankSearchTrackPrefix+", ^2Count: ^7"+rankedCount);
            SendButton(Button_Entry.RANK_NAME);
            SendButton(Button_Entry.RANK_PB);
            SendButton(Button_Entry.RANK_AVERAGE);
            SendButton(Button_Entry.RANK_STABILITY);
            SendButton(Button_Entry.RANK_WIN);
            SendButton(Button_Entry.RANK_TOTAL);
            SendButton(Button_Entry.RANK_POSITION);
        }
        internal void RankSearchTrack(string trackPrefix)
        {
            if(trackPrefix != rankSearchTrackPrefix)
            {
                rankSearchDisplayCount = 0;
                rankSearchTrackPrefix = trackPrefix;
                uint rankedCount = Ranking.GetRankedCount(rankSearchTrackPrefix,rankSearchCarPrefix);
                ClearRankDisplay(true);
                SendUpdateButton(Button_Entry.RANK_INFO,"^2Car: ^7"+rankSearchCarPrefix+", ^2Track:^7 "+rankSearchTrackPrefix+", ^2Count: ^7"+rankedCount);
                SendButton(Button_Entry.RANK_NAME);
                SendButton(Button_Entry.RANK_PB);
                SendButton(Button_Entry.RANK_AVERAGE);
                SendButton(Button_Entry.RANK_STABILITY);
                SendButton(Button_Entry.RANK_WIN);
                SendButton(Button_Entry.RANK_TOTAL);
                SendButton(Button_Entry.RANK_POSITION);
            }
        }
        internal void RankSearchCar(string carPrefix)
        {
            if(carPrefix != rankSearchCarPrefix)
            {
                rankSearchDisplayCount = 0;
                rankSearchCarPrefix = carPrefix;
                uint rankedCount = Ranking.GetRankedCount(rankSearchTrackPrefix,rankSearchCarPrefix);
                ClearRankDisplay(true);
                SendUpdateButton(Button_Entry.RANK_INFO,"^2Car: ^7"+rankSearchCarPrefix+", ^2Track:^7 "+rankSearchTrackPrefix+", ^2Count: ^7"+rankedCount);
                SendButton(Button_Entry.RANK_NAME);
                SendButton(Button_Entry.RANK_PB);
                SendButton(Button_Entry.RANK_AVERAGE);
                SendButton(Button_Entry.RANK_STABILITY);
                SendButton(Button_Entry.RANK_WIN);
                SendButton(Button_Entry.RANK_TOTAL);
                SendButton(Button_Entry.RANK_POSITION);
            }
        }
        internal void RankSearchAdd(string _licenceName)
        {
            if(rankSearchDisplayCount > 19)
            {
                AddMessageTop("^1Max rank display^2, please clear, click ^7search.",3500);
                return; 
            }
            if(rankSearchTrackPrefix == "" )
            {
                AddMessageTop("^1You must set the ^2track prefix ^1first.",3500);
                return;
            }
            if(rankSearchCarPrefix == "" )
            {
                AddMessageTop("^1You must set the ^2car prefix ^1first.",3500);
                return;
            }
            Rank _rank = Ranking.GetRank(rankSearchTrackPrefix,rankSearchCarPrefix,_licenceName);

            if(_rank == null)
            {
                AddMessageTop(_licenceName+" ^2is ^1not ranked ^2on ^7"+rankSearchTrackPrefix+" with ^7"+rankSearchCarPrefix+".",4500);
                return;
            }
            
            ButtonTemplateInfo bName = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_NAME);
            ButtonTemplateInfo bPB = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_PB);
            ButtonTemplateInfo bAverage = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_AVERAGE);
            ButtonTemplateInfo bStability = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_STABILITY);
            ButtonTemplateInfo bWin = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_WIN);
            ButtonTemplateInfo bTotal = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_TOTAL);
            ButtonTemplateInfo bPosition = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_POSITION);
            
            byte top = bName.Top;
            byte height = bName.Height;
            
            bName.Text = "^2"+_licenceName;
            bName.Top = (byte)((height*(rankSearchDisplayCount+1))+top);
            bPB.Text = _rank.GetRankGuiString[0];
            bPB.Top = (byte)((height*(rankSearchDisplayCount+1))+top);
            bAverage.Text = _rank.GetRankGuiString[1];
            bAverage.Top = (byte)((height*(rankSearchDisplayCount+1))+top);
            bStability.Text = _rank.GetRankGuiString[2];
            bStability.Top = (byte)((height*(rankSearchDisplayCount+1))+top);
            bWin.Text = _rank.GetRankGuiString[3];
            bWin.Top = (byte)((height*(rankSearchDisplayCount+1))+top);
            bTotal.Text = _rank.GetRankGuiString[4];
            bTotal.Top = (byte)((height*(rankSearchDisplayCount+1))+top);
            bPosition.Text = _rank.GetRankGuiString[5];
            bPosition.Top = (byte)((height*(rankSearchDisplayCount+1))+top);

            SendButton(newButtonId(Button_Entry.RANK_NAME), bName);
            SendButton(newButtonId(Button_Entry.RANK_PB), bPB);
            SendButton(newButtonId(Button_Entry.RANK_AVERAGE), bAverage);
            SendButton(newButtonId(Button_Entry.RANK_STABILITY), bStability);
            SendButton(newButtonId(Button_Entry.RANK_WIN), bWin);
            SendButton(newButtonId(Button_Entry.RANK_TOTAL), bTotal);
            SendButton(newButtonId(Button_Entry.RANK_POSITION), bPosition); 

            rankSearchDisplayCount++;

        }
        internal void SendRankTop20()
        {
           //if(freezeButton != 0)
            //    return;

            if(rankGuiCurrentDisplay != Button_Entry.NONE)
                ClearRankDisplay(false);

            rankGuiCurrentDisplay = Button_Entry.RANK_BUTTON_TOP20;
            SendUpdateButton(Button_Entry.RANK_BUTTON_TOP20,"^7Top20");
            SendUpdateButton(Button_Entry.RANK_BUTTON_SEARCH,"^2Search");
            SendUpdateButton(Button_Entry.RANK_BUTTON_CURRENT,"^2Current");

            string trackPrefix = ((Driver)this).ISession.GetRaceTrackPrefix();
            string carPrefix = ((ICar)this).CarPrefix;
            uint rankedCount = Ranking.GetRankedCount(trackPrefix,carPrefix);
            SendUpdateButton(Button_Entry.RANK_INFO,"^2Car: ^7"+carPrefix+", ^2Track:^7 "+trackPrefix+", ^2Count: ^7"+rankedCount);
            if(rankedCount < 1)
                return;

            string[] row = Ranking.GetTop20(trackPrefix,carPrefix);

            ButtonTemplateInfo bName = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_NAME);
            ButtonTemplateInfo bPB = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_PB);
            ButtonTemplateInfo bAverage = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_AVERAGE);
            ButtonTemplateInfo bStability = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_STABILITY);
            ButtonTemplateInfo bWin = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_WIN);
            ButtonTemplateInfo bTotal = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_TOTAL);
            ButtonTemplateInfo bPosition = Program.buttonTemplate.GetEntry((uint)Button_Entry.RANK_POSITION);
            SendButton(Button_Entry.RANK_NAME);
            SendButton(Button_Entry.RANK_PB);
            SendButton(Button_Entry.RANK_AVERAGE);
            SendButton(Button_Entry.RANK_STABILITY);
            SendButton(Button_Entry.RANK_WIN);
            SendButton(Button_Entry.RANK_TOTAL);
            SendButton(Button_Entry.RANK_POSITION);
            byte top = bName.Top;
            byte height = bName.Height;
            for(int itr = 0; itr < row.Length; itr++)
            {
               string[] colum = row[itr].Split(new char[]{' '});
               if(colum.Length != 6)
               {
                    Log.error("Button.SendRankTop10(), Found a bad Rank Row\r\n");
                    return;
               }
               
               bName.Text = "^2"+colum[0];
               bName.Top = (byte)((height*(itr+1))+top);
               bPB.Text = "^7"+colum[1];
               bPB.Top = (byte)((height*(itr+1))+top);
               bAverage.Text = "^2"+colum[2];
               bAverage.Top = (byte)((height*(itr+1))+top);
               bStability.Text = "^7"+colum[3];
               bStability.Top = (byte)((height*(itr+1))+top);
               bWin.Text = "^2"+colum[4];
               bWin.Top = (byte)((height*(itr+1))+top);
               bTotal.Text = "^7"+colum[5];
               bTotal.Top = (byte)((height*(itr+1))+top);
               bPosition.Text = "^2"+(itr+1).ToString();
               bPosition.Top = (byte)((height*(itr+1))+top);
               
               SendButton(newButtonId(Button_Entry.RANK_NAME), bName);
               SendButton(newButtonId(Button_Entry.RANK_PB), bPB);
               SendButton(newButtonId(Button_Entry.RANK_AVERAGE), bAverage);
               SendButton(newButtonId(Button_Entry.RANK_STABILITY), bStability);
               SendButton(newButtonId(Button_Entry.RANK_WIN), bWin);
               SendButton(newButtonId(Button_Entry.RANK_TOTAL), bTotal);
               SendButton(newButtonId(Button_Entry.RANK_POSITION), bPosition);
            }
        }
        internal void RemoveRankGui()
        {
            rankGuiCurrentDisplay = Button_Entry.NONE;
            RemoveGui(Gui_Entry.RANK);
        }
        internal void SendResultGui(Dictionary<string, int> scoringResultTextDisplay)
        {
            if(currentGui == Gui_Entry.RESULT)
                RemoveResultGui();

            //for (byte itr = 0; ++itr < 3; )
                //SendButton(Button_Entry.RESULT_BG);
                
            SendGui((ushort)Gui_Entry.RESULT);
            {
                Dictionary<string, int>.Enumerator itr =  scoringResultTextDisplay.GetEnumerator();
                ButtonTemplateInfo buttonName = Program.buttonTemplate.GetEntry((uint)Button_Entry.RESULT_NAME_DISPLAY);
                ButtonTemplateInfo buttonScore = Program.buttonTemplate.GetEntry((uint)Button_Entry.RESULT_SCORE_DISPLAY);
                ButtonTemplateInfo buttonInfoCopy;
                
                byte count = 0;
                while(itr.MoveNext())
                {
                    buttonInfoCopy = (ButtonTemplateInfo)buttonName.Clone();
                    buttonInfoCopy.Top = (byte)(((count) * buttonName.Height) + buttonName.Top + 1);
                    buttonInfoCopy.Text = itr.Current.Key;
                    SendButton(newButtonId(buttonInfoCopy.Entry), buttonInfoCopy);

                    buttonInfoCopy = (ButtonTemplateInfo)buttonScore.Clone();
                    buttonInfoCopy.Top = (byte)(((count) * buttonScore.Height) + buttonScore.Top + 1);
                    buttonInfoCopy.Text = "^7" + itr.Current.Value.ToString() + "^2pt";
                    SendButton(newButtonId(buttonInfoCopy.Entry), buttonInfoCopy);
                    count++;
                }
            }
        }
        internal void RemoveResultGui()
        {
            RemoveGui(Gui_Entry.RESULT);
        }
        
        internal bool HasGuiDisplay()
        {
            return currentGui != Gui_Entry.NONE;
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
        private byte newButtonId(Button_Entry buttonEntry)
        {
            return newButtonId((ushort)buttonEntry);
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
        internal ushort GetButtonEntry(byte buttonId)
        {
            return buttonList[buttonId];
        }

    }
}
