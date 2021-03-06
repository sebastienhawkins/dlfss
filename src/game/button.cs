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
    using Config_;

    internal abstract class Button
    {
        private const byte BUTTON_MAX_COUNT = 240;
        internal protected Button(Session _session) 
        {
            session = _session;
            driver = (Driver)this;

            TIMER_BUFFERED_BUTTON = (uint)Config.GetIntValue("Button", "CycleTime"); // 50
            MAX_BUTTON_BY_CYCLE = (uint)Config.GetIntValue("Button", "MaxPerCycle"); //5 
        }
        ~Button() 
        {
            if (true == false) { }
        }
        internal void ProcessBFNClearAll(bool sendMenu)
        {
            lock (bufferButtonPacket){bufferButtonPacket.Clear();}
            for (byte itr = 0; itr < BUTTON_MAX_COUNT; itr++)
            {
                buttonList[itr] = 0;
            }
            if (!driver.IsRacing())
            {
                SendBanner();
                SendTrackPrefix();
            }
            rankGuiCurrentDisplay = Button_Entry.NONE;
            currentGui = 0;
            if(sendMenu)
                SendMenuGui();
        }
        internal void ProcessBFNRequest()
        {
            SendMenuGui();
        }
        private ushort[] buttonList = new ushort[BUTTON_MAX_COUNT];

        private Driver driver;
        protected Session session;
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
        protected Gui_Entry currentGui = Gui_Entry.NONE;
        private Gui_Entry currentFlagGui = Gui_Entry.NONE;
        private Dictionary<Gui_Entry,uint> flagRaceGui = new Dictionary<Gui_Entry,uint>
        {
            {Gui_Entry.FLAG_GREEN,0},
            {Gui_Entry.FLAG_PIT_CLOSE,0},
            {Gui_Entry.FLAG_YELLOW_LOCAL,0},
            {Gui_Entry.FLAG_YELLOW_GLOBAL,0},
            {Gui_Entry.FLAG_REG_STOP_RACE,0},
            {Gui_Entry.FLAG_BLACK_PENALITY,0},
            {Gui_Entry.FLAG_BLUE_SLOW_CAR,0},
            {Gui_Entry.FLAG_WHITE_FINAL_LAP,0},
            {Gui_Entry.FLAG_BLACK_CAR_PROBLEM,0},
            {Gui_Entry.FLAG_BLACK_NO_SCORE,0},
            {Gui_Entry.FLAG_RACE_END,0},
        };
        protected bool HasFlagRace(Gui_Entry _flagRaceGui)
        {
            return (flagRaceGui[_flagRaceGui] > 1);
                
        }
        protected void RemoveFlagRaceGuiAll()
        {
            lock(flagRaceGui)
            {
                for (Gui_Entry itr = Gui_Entry.FLAG_BEGIN; itr < Gui_Entry.FLAG_MAX; itr++)
                {
                    if (flagRaceGui[itr] > 1)
                        flagRaceGui[itr] = 1;
                }
            }
        }
        private Button_Entry rankGuiCurrentDisplay = Button_Entry.NONE;
        private byte rankSearchDisplayCount = 0;
        private string rankSearchTrackPrefix = "";
        private string rankSearchCarPrefix = "";
        private Queue<Packet> bufferButtonPacket = new Queue<Packet>();

        private uint timerBufferedButton = 0;
        private uint TIMER_BUFFERED_BUTTON = 50;
        private uint MAX_BUTTON_BY_CYCLE = 5;
        private const uint TIMER_FLAG_RACE_UPDATE = 1500;
        private uint timerFlagRaceUpdate = TIMER_FLAG_RACE_UPDATE;
        private const uint TIMER_TIME_UPDATE = 1000;
        private uint timerTimeUpdate = 1000;
        private const uint TIMER_300 = 300;
        private uint timer300 = 0;
        private int fetchingWaitDisplayIndex = 0;
        private string[] fetchingWaitDisplay = new string[4] { "|", "/", "-", "\\"}; 

        protected virtual void update(uint diff)
        {
            if(timerBufferedButton < diff)
            {
                timerBufferedButton = TIMER_BUFFERED_BUTTON;
                lock(bufferButtonPacket)
                {
                    int buttonBufferCount = bufferButtonPacket.Count;
                    for(byte count = 0; count < buttonBufferCount ; count++)
                    {
                        ((Session)driver.ISession).AddToTcpSendingQueud(bufferButtonPacket.Dequeue());
                        if(count >= MAX_BUTTON_BY_CYCLE)
                            break;
                    }
                }
            }
            else
                timerBufferedButton -= diff; 
            
            if(HasAGuiDisplay())
            {
                if(timerTimeUpdate < diff)
                {
                    timerTimeUpdate = TIMER_TIME_UPDATE;
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_TIME,"^7"+DateTime.Now.ToLongTimeString());
                }
                else
                    timerTimeUpdate -= diff;
            }
            lock(buttonTimedList)
            {
                if (buttonTimedList.Count > 0)
                {
                    for (byte itr = 0; itr < buttonTimedList.Count; itr++ )
                    {
                        if ((int)buttonTimedList[itr].Time - diff > diff) //timer on button are maybe time to be rewrite, we have a dsync why this method, hack like.
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
            }
            lock (buttonMessageTop)
            {
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
            }
            lock (buttonMessageMiddle)
            {
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
            if (timerFlagRaceUpdate < diff)
            {
                timerFlagRaceUpdate = TIMER_FLAG_RACE_UPDATE;
                lock (flagRaceGui)
                {
                    bool send = false;
                    for (Gui_Entry itr = Gui_Entry.FLAG_BEGIN; itr < Gui_Entry.FLAG_MAX; itr++)
                    {
                        if(flagRaceGui[itr] > TIMER_FLAG_RACE_UPDATE)
                        {
                            flagRaceGui[itr] -= TIMER_FLAG_RACE_UPDATE;
                            if (!send && ((currentFlagGui == Gui_Entry.FLAG_MAX - 1 && itr < Gui_Entry.FLAG_MAX - 1)
                                || itr > currentFlagGui))
                            {
                                send = true;
                                SendGui(itr);
                            }
                        }
                        else if(flagRaceGui[itr] > 0)
                        {
                            flagRaceGui[itr] = 0;
                            if (currentFlagGui > 0 && currentFlagGui == itr)
                                RemoveGui(itr);
                        }
                    }
                    for (Gui_Entry itr = Gui_Entry.FLAG_BEGIN; itr < Gui_Entry.FLAG_MAX && !send; itr++)
                    {
                        if (flagRaceGui[itr] > TIMER_FLAG_RACE_UPDATE)
                        {
                            if (itr < currentFlagGui)
                            {
                                send = true;
                                SendGui(itr);
                                break;
                            }
                        }
                    }
                }
            }
            else
                timerFlagRaceUpdate -= diff;

            if (timer300 > TIMER_300)
            {
                timer300 = 0;
                if (isWaitingPST)
                {
                    if (fetchingWaitDisplayIndex > 3)
                        fetchingWaitDisplayIndex = 0;
                    SendUpdateButton(Button_Entry.MYSTATUS_FETCHING_REQUEST, "^7Retriving L^5FSW^7 Data ^4" + fetchingWaitDisplay[fetchingWaitDisplayIndex++]);
                }
            }
            else
                timer300 += diff;

        }

        public void SendGui(ushort guiEntry, string text)
        {
            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            guiInfo.Text = text;
            SendGui(guiInfo);
        }
        internal void SendUpdateGui(Gui_Entry guiEntry, string text)
        {
            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            guiInfo.Text = text;
            if(currentGui == guiEntry)
                RemoveGui(currentGui);
            SendGui(guiInfo);
        }
        internal void SendGui(Gui_Entry guiEntry)
        {
            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            SendGui(guiInfo);
        }
        private void SendGui(GuiTemplateInfo guiInfo)
        {
            if (driver.IsBot())
                return;

            if(guiInfo.Entry >= Gui_Entry.FLAG_BEGIN && guiInfo.Entry < Gui_Entry.FLAG_MAX)
            {
                if(guiInfo.Entry == currentFlagGui)
                    return;
                if(currentFlagGui > 0)
                    RemoveGui(currentFlagGui);
                currentFlagGui = guiInfo.Entry;
            }
            else
            {
                if (guiInfo.Entry == currentGui)
                    return;
                if(currentGui > 0)
                    RemoveGui(currentGui);
                currentGui = guiInfo.Entry;
            }
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

            switch (currentGui)
            {
                case Gui_Entry.MENU:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0Menu"); break;
                case Gui_Entry.RANK:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0Ranking Driver"); break;
                case Gui_Entry.TEXT:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0Text Display"); break;
                case Gui_Entry.RESULT:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0Result"); break;
                case Gui_Entry.CONFIG_USER:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0User Config"); break;
                case Gui_Entry.MYSTATUS:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0MyStatus"); break;
                case Gui_Entry.HELP:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0Help"); break;
                default:
                    SendUpdateButton(Button_Entry.TASKBAR_BUTTON_CURRENT, "^0"); break;
            }
        }
        public void RemoveGui(ushort guiEntry)
        {
            RemoveGui((Gui_Entry)guiEntry);
        }
        public void RemoveCurrentGui()
        {
            if(currentGui != Gui_Entry.NONE)
                RemoveGui(currentGui);
        }
        internal void RemoveGui(Gui_Entry guiEntry)
        {
            if (driver.IsBot())
                return;

            GuiTemplateInfo guiInfo = Program.guiTemplate.GetEntry((uint)guiEntry);
            
            if (guiInfo.Entry == Gui_Entry.MYSTATUS)
                isWaitingPST = false;
            
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

            if (guiInfo.Entry >= Gui_Entry.FLAG_BEGIN && guiInfo.Entry < Gui_Entry.FLAG_MAX)
                currentFlagGui = Gui_Entry.NONE;
            else
            {
                currentGui = Gui_Entry.NONE;
            }
        }
        internal protected byte RemoveButton(Button_Entry buttonEntry)
        {
            return RemoveButton((ushort)buttonEntry);
        }
        public byte RemoveButton(ushort buttonEntry)
        {
            if (driver.IsBot())
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
            if (driver.IsBot())  //Become Redondant with SendButton, but since they since timer better this way.
                return;

            lock (buttonMessageTop)
            {
                buttonMessageTop.Enqueue(new ButtonMessage(text, duration));
            }
        }
        public void AddMessageMiddle(string text, uint duration)
        {
            if (driver.IsBot()) //Become Redondant with SendButton, but since they since timer better this way.
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
            if (buttonInfo == null)
                return;

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
            if (driver.IsBot())
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
            lock (bufferButtonPacket) { bufferButtonPacket.Enqueue(_packet); }
        }

        internal void SendMotd()
        {
            string motd = session.GetMotdMessage();
            if (motd != "")
                SendGui((ushort)Gui_Entry.MOTD, motd);
            else
                SendGui(Gui_Entry.MOTD);
        }
        internal void SendRules()
        {
            string rules = session.GetRulesMessage();
            if (rules != "")
                SendGui((ushort)Gui_Entry.RULES, rules);
            else
                SendGui(Gui_Entry.RULES);
        }
        internal void SendBanner()
        {
            //Send Banner
            SendUniqueButton((ushort)Button_Entry.BANNER);
        }
        private void RemoveBanner()
        {
            RemoveButton((ushort)Button_Entry.BANNER);
        }
        internal void SendFlagRace(ushort guiEntry, uint time)
        {
            SendFlagRace((Gui_Entry)guiEntry, time);
        }
        protected void SendFlagRace(Gui_Entry guiEntry, uint time)
        {
            lock (flagRaceGui)
            {
                flagRaceGui[guiEntry] = (time < TIMER_FLAG_RACE_UPDATE ? TIMER_FLAG_RACE_UPDATE : time);
            }
        }
        internal void RemoveRaceFlag(ushort guiEntry)
        {
            RemoveFlagRace((Gui_Entry)guiEntry);
        }
        protected void RemoveFlagRace(Gui_Entry guiEntry)
        {
            lock (flagRaceGui)
            {
                flagRaceGui[guiEntry] = 1;
            }
        }
        internal void SendTrackPrefix()
        {
            ButtonTemplateInfo button = Program.buttonTemplate.GetEntry((uint)Button_Entry.TRACK_PREFIX);
            button.Text += button.Text + driver.ISession.GetRaceTrackPrefix();
            SendUpdateButton(button);
        }
        private void RemoveTrackPrefix()
        {
            RemoveButton((ushort)Button_Entry.TRACK_PREFIX);
        }
        internal void SendConfigGui()
        {
            SendGui(Gui_Entry.CONFIG_USER);
            
            SendUpdateButton(Button_Entry.CONFIG_USER_ACC_CURRENT, "^7" + driver.GetAccelerationStartSpeed() + "^2-^7" + driver.GetAccelerationEndSpeed() + " ^2Kmh");
            SendUpdateButton(Button_Entry.CONFIG_USER_ACC_ON, (driver.IsAccelerationOn() ? "^7" : "^8") + " Acceleration");
            SendUpdateButton(Button_Entry.CONFIG_USER_DRIFT_ON, (driver.IsDriftScoreOn() ? "^7" : "^8") + " Drift Score");
            SendUpdateButton(Button_Entry.CONFIG_USER_MAX_SPEED_ON, (driver.IsMaxSpeedDisplay ? "^7" : "^8") + " Max Speed");

            SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_ALL, ((driver.IsTimeDiffSplitDisplay && driver.IsTimeDiffLapDisplay) ? "^7" : "^8") + " Time Diff");
            SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_LAP, (driver.IsTimeDiffLapDisplay ? "^7" : "^8") + " PB vs lap");
            SendUpdateButton(Button_Entry.CONFIG_USER_TIMEDIFF_SPLIT, (driver.IsTimeDiffSplitDisplay ? "^7" : "^8") + " PB vs Split");

            if (driver.IsNodeTrajectory && driver.IsNodeSideDisplay && driver.IsNodeOrientation)
                SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^7Track Info");
            else
                SendUpdateButton(Button_Entry.CONFIG_NODE_TITLE, "^8Track Info");
            SendUpdateButton(Button_Entry.CONFIG_NODE_TRAJECTORY, (driver.IsNodeTrajectory ? "^7" : "^8") + "Trajectory");
            SendUpdateButton(Button_Entry.CONFIG_NODE_SIDE, (driver.IsNodeSideDisplay ? "^7" : "^8") + "Side Distance");
            SendUpdateButton(Button_Entry.CONFIG_NODE_ORIENTATION, (driver.IsNodeOrientation ? "^7" : "^8") + "Orientation");
        }
        internal void RemoveConfigGui()
        {
            RemoveGui(Gui_Entry.CONFIG_USER);
        }
        internal void SendMenuGui()
        {
            SendGui(Gui_Entry.MENU);
        }
        internal void RemoveMenuGui()
        {
            RemoveGui(Gui_Entry.MENU);
        }
        internal void SendHelpGui()
        {
            SendGui(Gui_Entry.HELP);
        }
        internal void RemoveHelpGui()
        {
            RemoveGui(Gui_Entry.HELP);
        }
        
        //Rank
        internal void SendRankGui(Button_Entry startWith)
        {
            SendGui(Gui_Entry.RANK);
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

            string trackPrefix = session.GetRaceTrackPrefix();
            //uint rankedCount = Ranking.GetRankedCount(trackPrefix,carPrefix);
            SendUpdateButton(Button_Entry.RANK_INFO,"^2Car: ^7Mixed, ^2Track:^7 "+trackPrefix+", ^2Count: ^7NA");

            List<Driver> driver = session.GetDriverList();
            
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
                Rank rank = driver[itr].GetRank(trackPrefix, driver[itr].CarPrefix);
                if(rank != null)
                {
                    bName.Text = "^2"+driver[itr].LicenceName +"^7 -> ^3"+driver[itr].CarPrefix;
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
                    bName.Text = "^2" + driver[itr].LicenceName + "^7 -> ^3" + driver[itr].CarPrefix;
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

            string trackPrefix = driver.ISession.GetRaceTrackPrefix();
            string carPrefix = ((ICar)this).CarPrefix;
            if(carPrefix == "")
                AddMessageMiddle("^2Ranking ^3Top20 ^2need you to enter a car first.",6000);
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
                string[] colum = row[itr].Split(((char)0));
               if(colum.Length != 6)
               {
                   Log.error("Button.SendRankTop10(), Found a bad Rank Row(" + row[itr] + ")\r\n");
                    continue;
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

        //Timing
        internal void SendTimingGui(Button_Entry startWith)
        {
            SendGui(Gui_Entry.TIMING);
            switch (startWith)
            {
                case Button_Entry.RANK_BUTTON_TOP20:
                SendRankTop20(); break;
                case Button_Entry.RANK_BUTTON_CURRENT:
                SendRankCurrent(0); break;
            }
        }
        internal void SendTimingTop20()
        {
            //if(freezeButton != 0)
            //    return;

            if (rankGuiCurrentDisplay != Button_Entry.NONE)
                ClearRankDisplay(false);

            rankGuiCurrentDisplay = Button_Entry.RANK_BUTTON_TOP20;
            SendUpdateButton(Button_Entry.RANK_BUTTON_TOP20, "^7Top20");
            SendUpdateButton(Button_Entry.RANK_BUTTON_SEARCH, "^2Search");
            SendUpdateButton(Button_Entry.RANK_BUTTON_CURRENT, "^2Current");

            string trackPrefix = driver.ISession.GetRaceTrackPrefix();
            string carPrefix = ((ICar)this).CarPrefix;
            if (carPrefix == "")
                AddMessageMiddle("^2Ranking ^3Top20 ^2need you to enter a car first.", 6000);
            uint rankedCount = Ranking.GetRankedCount(trackPrefix, carPrefix);
            SendUpdateButton(Button_Entry.RANK_INFO, "^2Car: ^7" + carPrefix + ", ^2Track:^7 " + trackPrefix + ", ^2Count: ^7" + rankedCount);
            if (rankedCount < 1)
                return;

            string[] row = Ranking.GetTop20(trackPrefix, carPrefix);

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
            for (int itr = 0; itr < row.Length; itr++)
            {
                string[] colum = row[itr].Split(((char)0));
                if (colum.Length != 6)
                {
                    Log.error("Button.SendRankTop10(), Found a bad Rank Row(" + row[itr] + ")\r\n");
                    continue;
                }

                bName.Text = "^2" + colum[0];
                bName.Top = (byte)((height * (itr + 1)) + top);
                bPB.Text = "^7" + colum[1];
                bPB.Top = (byte)((height * (itr + 1)) + top);
                bAverage.Text = "^2" + colum[2];
                bAverage.Top = (byte)((height * (itr + 1)) + top);
                bStability.Text = "^7" + colum[3];
                bStability.Top = (byte)((height * (itr + 1)) + top);
                bWin.Text = "^2" + colum[4];
                bWin.Top = (byte)((height * (itr + 1)) + top);
                bTotal.Text = "^7" + colum[5];
                bTotal.Top = (byte)((height * (itr + 1)) + top);
                bPosition.Text = "^2" + (itr + 1).ToString();
                bPosition.Top = (byte)((height * (itr + 1)) + top);

                SendButton(newButtonId(Button_Entry.RANK_NAME), bName);
                SendButton(newButtonId(Button_Entry.RANK_PB), bPB);
                SendButton(newButtonId(Button_Entry.RANK_AVERAGE), bAverage);
                SendButton(newButtonId(Button_Entry.RANK_STABILITY), bStability);
                SendButton(newButtonId(Button_Entry.RANK_WIN), bWin);
                SendButton(newButtonId(Button_Entry.RANK_TOTAL), bTotal);
                SendButton(newButtonId(Button_Entry.RANK_POSITION), bPosition);
            }
        }
        
        //MyStatus
        private bool isWaitingPST = false;
        private string myStatusLicenceName = "";
        internal void SendMyStatus()
        {
            myStatusLicenceName = driver.LicenceName;
            SendGui(Gui_Entry.MYSTATUS);
            isWaitingPST = true;
            fetchingWaitDisplayIndex = 0;
            //SendUpdateButton(Button_Entry.MYSTATUS_FETCHING_REQUEST, "^7Please Wait Fetching Data ^2" + fetchingWaitDisplay[fetchingWaitDisplayIndex]);
            driver.pst = Program.pubStats.GetPST(myStatusLicenceName, new PubStats_.PSTCallBack(CallBackPST));
            SendAleajectaData();
            SendTopBottom3Rank();
        }
        internal void SearchMyStatus(string _licenceName)
        {
            SendUpdateButton(Button_Entry.MYSTATUS_WIN, "^2First Place ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_SECOND, "^2Second Place ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_THIRD, "^2Third Place ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_FINISH, "^2Races Finish ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_QUAL, "^2Qual Join ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_POLE, "^2Pole Position ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_DRAGS, "^2Drag Join ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_DRAGS_WIN, "^2Drags Win ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_DISTANCE, "^2Distance ^7: ^1NA ^7Km");
            SendUpdateButton(Button_Entry.MYSTATUS_FUEL, "^2Fuel burnt ^7: ^1NA ^7L");
            SendUpdateButton(Button_Entry.MYSTATUS_LAPS, "^2Lap count ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_HOST_JOIN, "^2Host join ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_ONLINE, "^2Online Status ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_SERVER, "^2Last Server ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_LASTTIME, "^2Last Seen ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_TRACK, "^2Last Track ^7: ^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_CAR, "^2Last Car ^7: ^1NA");
                
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_TRACK, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_CAR, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_BEST, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_AVG, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_STA, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_WIN, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_TOTAL, "^1NA");

            SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_TRACK, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_CAR, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_BEST, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_AVG, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_STA, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_WIN, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_TOTAL, "^1NA");

            SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_TRACK, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_CAR, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_BEST, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_AVG, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_STA, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_WIN, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_TOTAL, "^1NA");

            SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_TRACK, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_CAR, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_BEST, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_AVG, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_STA, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_WIN, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_TOTAL, "^1NA");

            SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_TRACK, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_CAR, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_BEST, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_AVG, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_STA, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_WIN, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_TOTAL, "^1NA");


            SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_TRACK, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_CAR, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_BEST, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_AVG, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_STA, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_WIN, "^1NA");
            SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_TOTAL, "^1NA");

            myStatusLicenceName = _licenceName;
            isWaitingPST = true;
            fetchingWaitDisplayIndex = 0;
            //SendUpdateButton(Button_Entry.MYSTATUS_FETCHING_REQUEST, "^7Please Wait Fetching Data ^2" + fetchingWaitDisplay[fetchingWaitDisplayIndex]);
            driver.pst = Program.pubStats.GetPST(myStatusLicenceName, new PubStats_.PSTCallBack(CallBackPST));
            SendAleajectaData();
            SendTopBottom3Rank();
        }
        public void CallBackPST()
        {
            if (!isWaitingPST)
                return;
            isWaitingPST = false;
            RemoveButton(Button_Entry.MYSTATUS_FETCHING_REQUEST);
            driver.pst = Program.pubStats.GetPST(myStatusLicenceName, null);
            if(driver.pst != null)
                SendPSTData();
        }
        private void SendTopBottom3Rank()
        {
            Rank[] ranks = Ranking.GetTopBottom3Rank(myStatusLicenceName);
            if (ranks[0] != null)
            {
                SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_TRACK, "^3"+ranks[0].TrackPrefix);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_CAR, "^3" + ranks[0].CarPrefix);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_BEST, "^3" + ranks[0].BestLap);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_AVG, "^3" + ranks[0].AverageLap);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_STA, "^3" + ranks[0].Stability);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_WIN, "^3" + ranks[0].RaceWin);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKT1_TOTAL, "^3" + ranks[0].Total);
                if (ranks[1] != null)
                {
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_TRACK, "^3" + ranks[1].TrackPrefix);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_CAR, "^3" + ranks[1].CarPrefix);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_BEST, "^3" + ranks[1].BestLap);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_AVG, "^3" + ranks[1].AverageLap);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_STA, "^3" + ranks[1].Stability);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_WIN, "^3" + ranks[1].RaceWin);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKT2_TOTAL, "^3" + ranks[1].Total);

                    if (ranks[2] != null)
                    {
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_TRACK, "^3" + ranks[2].TrackPrefix);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_CAR, "^3" + ranks[2].CarPrefix);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_BEST, "^3" + ranks[2].BestLap);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_AVG, "^3" + ranks[2].AverageLap);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_STA, "^3" + ranks[2].Stability);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_WIN, "^3" + ranks[2].RaceWin);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKT3_TOTAL, "^3" + ranks[2].Total);
                    }
                }
            }
            
            if (ranks[3] != null)
            {
                SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_TRACK, "^3" + ranks[3].TrackPrefix);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_CAR, "^3" + ranks[3].CarPrefix);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_BEST, "^3" + ranks[3].BestLap);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_AVG, "^3" + ranks[3].AverageLap);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_STA, "^3" + ranks[3].Stability);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_WIN, "^3" + ranks[3].RaceWin);
                SendUpdateButton(Button_Entry.MYSTATUS_RANKB1_TOTAL, "^3" + ranks[3].Total);
                if (ranks[4] != null)
                {
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_TRACK, "^3" + ranks[4].TrackPrefix);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_CAR, "^3" + ranks[4].CarPrefix);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_BEST, "^3" + ranks[4].BestLap);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_AVG, "^3" + ranks[4].AverageLap);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_STA, "^3" + ranks[4].Stability);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_WIN, "^3" + ranks[4].RaceWin);
                    SendUpdateButton(Button_Entry.MYSTATUS_RANKB2_TOTAL, "^3" + ranks[4].Total);

                    if (ranks[5] != null)
                    {
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_TRACK, "^3" + ranks[5].TrackPrefix);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_CAR, "^3" + ranks[5].CarPrefix);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_BEST, "^3" + ranks[5].BestLap);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_AVG, "^3" + ranks[5].AverageLap);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_STA, "^3" + ranks[5].Stability);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_WIN, "^3" + ranks[5].RaceWin);
                        SendUpdateButton(Button_Entry.MYSTATUS_RANKB3_TOTAL, "^3" + ranks[5].Total);
                    }
                }
            }

        }
        private void SendAleajectaData()
        {
            string[] overall = Ranking.GetOverall(myStatusLicenceName);
            
            SendUpdateButton(Button_Entry.MYSTATUS_LICENCE_NAME, "^2Licence Name^7 : ^3" + myStatusLicenceName);
            SendUpdateButton(Button_Entry.MYSTATUS_WIN_OVERALL, "^2Race Experience^7 : ^3" + overall[0]);
            SendUpdateButton(Button_Entry.MYSTATUS_RANK_OVERALL, "^2Rank Overall^7 : ^3" + overall[1]);

            if (myStatusLicenceName == driver.LicenceName)
            {
                SendUpdateButton(Button_Entry.MYSTATUS_DRIVER_NAME, "^2Driver Name^7 : ^3" + driver.DriverName);
                SendUpdateButton(Button_Entry.MYSTATUS_SAFEPCT, "^2Safe Driving^7 : ^3" + driver.GetSafePct()+" ^7%");
                SendUpdateButton(Button_Entry.MYSTATUS_CHATWARN, "^2Chat Warning ^7: ^3" + (driver.GetFloodChatCount() + (driver.GetWarningChatCount()* 3)).ToString());
                //SendUpdateButton(Button_Entry.MYSTATUS_DRIFT_BEST, "^3");
                //SendUpdateButton(Button_Entry.MYSTATUS_TIME_RACED, "^3" );
                //SendUpdateButton(Button_Entry.MYSTATUS_TIME_SPEC, "^3" );
                //SendUpdateButton(Button_Entry.MYSTATUS_TIME_GARAGE, "^3" );
                //SendUpdateButton(Button_Entry.MYSTATUS_TIME_TOTAL, "^3" );
            }
            else
            {
                int badCount = 0;
                int badChat = 0;
                int floodChat = 0;
                int lapCount = 0;
                int finishCount = 0;
                string _driverName = "";
                Program.dlfssDatabase.Lock();
                {
                    System.Data.IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT `guid`,`driver_name`,`race_finish_count`,`lap_count`,`warning_driving_count`,`warning_chat_count`,`flood_chat_count` FROM `driver` WHERE `licence_name`LIKE'" + ConvertX.SQLString(myStatusLicenceName) + "'");
                    if (reader.Read())
                    {
                        uint _guid = (uint)reader.GetInt32(0);
                        _driverName = reader.GetString(1);
                        finishCount = reader.GetInt32(2);
                        lapCount = reader.GetInt32(3);
                        badCount = reader.GetInt32(4);
                        badChat = reader.GetInt32(5);
                        floodChat = reader.GetInt32(6);

                    }
                    reader.Close(); reader.Dispose();
                }
                Program.dlfssDatabase.Unlock();
                int _safePct = driver.SetSafePct(badCount, finishCount, lapCount);
                SendUpdateButton(Button_Entry.MYSTATUS_DRIVER_NAME, "^2Driver Name^7 : ^3" + _driverName);
                SendUpdateButton(Button_Entry.MYSTATUS_SAFEPCT, "^2Safe Driving^7 : ^3" + _safePct+" ^7%");
                SendUpdateButton(Button_Entry.MYSTATUS_CHATWARN, "^2Chat Warning ^7: ^3"+(floodChat+(badChat*3)).ToString());
            }
        }
        private void SendPSTData()
        {
            string onlineStatus = "NA";
            switch (driver.pst.Online)
            {
                case 0: onlineStatus = "offline"; break;
                case 1: onlineStatus = "spectate"; break;
                case 2: onlineStatus = "pits"; break;
                case 3: onlineStatus = "in-race"; break;
            }
            string lastSeen = "NA";
            //DateTime.UtcNow
            DateTime _lastSeen = ConvertX.UTCToDatetime(driver.pst.LastTime).AddHours(-5.0d);
            lastSeen = _lastSeen.ToShortDateString() +" "+ _lastSeen.ToShortTimeString();

            float winpc = 0;
            float secondpc = 0;
            float thirdpc = 0;
            float polepc = 0;
            float dragwinpc = 0;
            if (driver.pst.Finished > 0)
            {
                winpc = driver.pst.Win * 100 / driver.pst.Finished;
                secondpc = driver.pst.Second * 100 / driver.pst.Finished;
                thirdpc = driver.pst.Third * 100 / driver.pst.Finished;
            }
            if (driver.pst.Quals > 0)
                polepc = driver.pst.Pole * 100 / driver.pst.Quals;
            if (driver.pst.Drags > 0)
                dragwinpc = driver.pst.DragWins * 100 / driver.pst.Drags;

            SendUpdateButton(Button_Entry.MYSTATUS_WIN, "^2First Place ^7: ^3"+driver.pst.Win.ToString()+" ^7- ^3"+winpc+" ^7%");
            SendUpdateButton(Button_Entry.MYSTATUS_SECOND, "^2Second Place ^7: ^3" + driver.pst.Second.ToString() + " ^7- ^3" + secondpc + " ^7%");
            SendUpdateButton(Button_Entry.MYSTATUS_THIRD, "^2Third Place ^7: ^3" + driver.pst.Third.ToString() + " ^7- ^3" + thirdpc + " ^7%");
            SendUpdateButton(Button_Entry.MYSTATUS_FINISH, "^2Races Finish ^7: ^3" + driver.pst.Finished.ToString());
            SendUpdateButton(Button_Entry.MYSTATUS_QUAL, "^2Qual Join ^7: ^3" + driver.pst.Quals.ToString());
            SendUpdateButton(Button_Entry.MYSTATUS_POLE, "^2Pole Position ^7: ^3" + driver.pst.Pole.ToString() + " ^7- ^3" + polepc + " ^7%");
            SendUpdateButton(Button_Entry.MYSTATUS_DRAGS, "^2Drag Join ^7: ^3" + driver.pst.Drags.ToString());
            SendUpdateButton(Button_Entry.MYSTATUS_DRAGS_WIN, "^2Drags Win ^7: ^3" + driver.pst.DragWins.ToString() + " ^7- ^3" + dragwinpc + " ^7%");
            SendUpdateButton(Button_Entry.MYSTATUS_DISTANCE, "^2Distance ^7: ^3" + (driver.pst.Distance/1000).ToString()+" ^7Km");
            SendUpdateButton(Button_Entry.MYSTATUS_FUEL, "^2Fuel burnt ^7: ^3" + (driver.pst.Fuel/100).ToString() + " ^7L");
            SendUpdateButton(Button_Entry.MYSTATUS_LAPS, "^2Lap count ^7: ^3" + driver.pst.Laps.ToString());
            SendUpdateButton(Button_Entry.MYSTATUS_HOST_JOIN, "^2Host join ^7: ^3" + driver.pst.HostJoin.ToString());
            SendUpdateButton(Button_Entry.MYSTATUS_ONLINE, "^2Online Status ^7: ^3" + onlineStatus);
            SendUpdateButton(Button_Entry.MYSTATUS_SERVER, "^2Last Server ^7: ^3" + driver.pst.LastServer);
            SendUpdateButton(Button_Entry.MYSTATUS_LASTTIME, "^2Last Seen ^7: ^3" + lastSeen);
            SendUpdateButton(Button_Entry.MYSTATUS_TRACK, "^2Last Track ^7: ^3" + driver.pst.Track);
            SendUpdateButton(Button_Entry.MYSTATUS_CAR, "^2Last Car ^7: ^3" + driver.pst.Car);
        }
        
        internal void SendResultGui(Dictionary<string, int> scoringResultTextDisplay)
        {
            if(currentGui == Gui_Entry.RESULT)
                RemoveResultGui();

            //for (byte itr = 0; ++itr < 3; )
                //SendButton(Button_Entry.RESULT_BG);
                
            SendGui(Gui_Entry.RESULT);
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
                    buttonInfoCopy.Text = "^7" + itr.Current.Value.ToString() + "^2xp";
                    SendButton(newButtonId(buttonInfoCopy.Entry), buttonInfoCopy);
                    count++;
                }
            }
        }
        internal void RemoveResultGui()
        {
            RemoveGui(Gui_Entry.RESULT);
        }
        internal void RemoveNodeBar()
        {
            RemoveButton(Button_Entry.NODE_SIDE);
            RemoveButton(Button_Entry.NODE_ORIE_TO_TRACK);
            RemoveButton(Button_Entry.NODE_TRAJ_TO_TRACK);
        }
        protected void SendAllStaticButton()
        {
            SendBanner();
            SendTrackPrefix();
        }
        protected void ClearAllStaticButton()
        {
            RemoveTrackPrefix();
            RemoveBanner();
            RemoveFlagRaceGuiAll();
        }
        internal bool HasAGuiDisplay()
        {
            return currentGui != Gui_Entry.NONE;
        }
        internal bool HasGuiDisplay(Gui_Entry guiEntry)
        {
            return currentGui == guiEntry;
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
