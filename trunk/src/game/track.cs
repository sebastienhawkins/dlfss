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
using System.Data;
using System;
namespace Drive_LFSS.Game_
{
    using Packet_;
    using Definition_;
    using Database_;
    using Log_;
    using Script_;
    using Storage_;
    using Game_;
    using Config_;

    enum Vote_Track_Change : byte
    {
        USER = 0,
        VOTE = 1,
        NO_CHANGE = 2,
        AUTO = 4,
        SEQUENCE = 5,
    }
    abstract class Track : Restriction
    {
        protected Track(Session _session) : base(_session)
        {
        }
        protected virtual void ConfigApply()
        {
            switch (Config.GetStringValue("Vote", session.GetSessionName(), "TrackChange").ToUpperInvariant())
            {
                case "USER": trackChangeBeviator = Vote_Track_Change.USER; break;
                case "VOTE": trackChangeBeviator = Vote_Track_Change.VOTE; break;
                case "STATIC": trackChangeBeviator = Vote_Track_Change.NO_CHANGE; break;
                case "AUTO": trackChangeBeviator = Vote_Track_Change.AUTO; break;
                case "SEQUENCE": trackChangeBeviator = Vote_Track_Change.SEQUENCE; break;
                default: Log.error("Vote System config Error, unknown Value for Vote." + session.GetSessionName() + ".TrackChange = " + Config.GetStringValue("Vote", session.GetSessionName(), "TrackChange") + "\r\n"); break;
            }
            raceMapEntry = (ushort)Config.GetIntValue("Vote", session.GetSessionName(), "RaceMap");
            raceMap.Clear();
            if(raceMapEntry > 0)
            {
                Program.dlfssDatabase.Lock();
                {
                    IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT * FROM `race_map` WHERE `entry`='"+raceMapEntry+"'");
                    while (reader.Read())
                        raceMap.Add((ushort)reader.GetInt16(1));
                    reader.Dispose();
                }Program.dlfssDatabase.Unlock();
            }
        }
        internal void ProcessVoteNotification(Vote_Action voteAction, byte connectionId)
        {
            #if DEBUG
            Log.debug(session.GetSessionNameForLog() + " Vote notification was:" + voteAction + "\r\n");
            #endif

            ushort[] keys = new ushort[6] { 0, 0, 0, 0, 0, 0 };
            voteOptions.Keys.CopyTo(keys, 0);
            switch (voteAction) //Switch do , return and not break since we custom vote action at the end.
            {
                case Vote_Action.VOTE_RESTART:
                case Vote_Action.VOTE_QUALIFY:
                case Vote_Action.VOTE_END:
                {
                    if (race.GetGuid() != 0 && race.GetCarCount() > 1)
                    {
                        IDriver driver = session.GetDriverWithConnectionId(connectionId);
                        if (driver != null)
                        {
                            if (race.HasRacerOn() && !race.IsStillRacing(((ICar)driver).CarId))
                            {
                                ((IButton)driver).AddMessageMiddle("^7O^3nly ^2grid/racing ^3player can ^2restart^3/^2end^3/^2qualify ^3during a race",7000);
                                SendVoteCancel();
                            }
                        }
                    }
                }return;
                case Vote_Action.VOTE_CUSTOM_1: voteOptions[keys[0]]++; voteCount++; break;
                case Vote_Action.VOTE_CUSTOM_2: voteOptions[keys[1]]++; voteCount++; break;
                case Vote_Action.VOTE_CUSTOM_3: voteOptions[keys[2]]++; voteCount++; break;
                case Vote_Action.VOTE_CUSTOM_4: voteOptions[keys[3]]++; voteCount++; break;
                case Vote_Action.VOTE_CUSTOM_5: voteOptions[keys[4]]++; voteCount++; break;
                case Vote_Action.VOTE_CUSTOM_6: voteOptions[keys[5]]++; voteCount++; break;
            }
            if (voteInProgress)
            {
                session.RemoveButton((ushort)Button_Entry.VOTE_OPTION_1, connectionId);
                session.RemoveButton((ushort)Button_Entry.VOTE_OPTION_2, connectionId);
                session.RemoveButton((ushort)Button_Entry.VOTE_OPTION_3, connectionId);
                session.RemoveButton((ushort)Button_Entry.VOTE_OPTION_4, connectionId);
                session.RemoveButton((ushort)Button_Entry.VOTE_OPTION_5, connectionId);
                session.RemoveButton((ushort)Button_Entry.VOTE_OPTION_6, connectionId);
            }
        }
        internal void ProcessVoteCancel()
        {
            #if DEBUG
            Log.debug(session.GetSessionNameForLog() + " A VOTE was CANCEL.\r\n");
            #endif
        }

        protected Vote_Track_Change trackChangeBeviator = Vote_Track_Change.USER;
        protected ushort raceMapEntry = 0;
        protected List<ushort> raceMap = new List<ushort>();
        protected int raceMapStaticItr = 0;
        private Dictionary<ushort, byte> voteOptions = new Dictionary<ushort, byte>();
        private byte voteCount = 0;
        protected bool voteInProgress = false; //used to freeze System, no Change will be made when YES
        private byte licenceCount = 0;
        private uint voteTimer = 0;
        private uint voteTimerAdvert = 100;
        private uint nextRaceTimer = 0;
        protected RaceTemplateInfo raceInfo = new RaceTemplateInfo();
        //protected RestrictionJoinInfo restrictionJoinInfo = null;
        //protected RestrictionRaceInfo restrictionRaceInfo = null;
        private ushort  previousRaceEntry = 0;
        protected bool doEnd = false; //Will serve for Know when RaceEnd Really happen and, if we load auto the next track.

        public bool IsVoteInProgress()
        {
            return voteInProgress;
        }
        internal virtual void update(uint diff)
        {
            if (voteTimer != 0)
            {
                if (voteTimer <= diff || voteCount >= session.GetNbrOfConnection())
                {
                    voteTimer = 0;
                    EndNextTrackVote();
                }
                else
                {
                    voteTimer -= diff;

                    if (voteTimerAdvert < diff)
                    {
                        string _voteTimer = (voteTimer/100).ToString();
                        if (_voteTimer.Length > 0)
                            _voteTimer = _voteTimer.Insert(_voteTimer.Length-1, ".");
                        session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_TITLE, "^3Next Track Vote, ^2" + _voteTimer + "^3 ms.");
                        voteTimerAdvert = 300;
                    }
                    else
                        voteTimerAdvert -= diff;
                }
            }
            if (nextRaceTimer != 0)
            {
                if (nextRaceTimer <= diff)
                {
                    nextRaceTimer = 0;
                    voteInProgress = false;
                    EndRace();
                }
                else
                    nextRaceTimer -= diff;
            }
        }
        internal List<ushort> GetSmartRaceMap()
        {
            List<ushort> _raceMap = new List<ushort>(raceMap);
            if (raceMap.Count > 2)
            {
                if (_raceMap.Contains((ushort)previousRaceEntry))
                    _raceMap.Remove((ushort)previousRaceEntry);

                if (_raceMap.Contains((ushort)raceInfo.Entry))
                    _raceMap.Remove((ushort)raceInfo.Entry);
            }
            return _raceMap;
        }
        public void StartNextTrackVote()
        {
            //Script Call, Before Start.
            if (session.Script.NextTrackVoteStarted())
                return;

            voteInProgress = true;
            voteCount = 0;
            voteOptions.Clear();
            licenceCount = session.GetNbrOfConnection();

            List<ushort> _raceMap = GetSmartRaceMap();
            ushort raceMapMax = (ushort)_raceMap.Count;
            Random random = new Random();
            
            while (voteOptions.Count < 6 )
            {
                ushort next = (ushort)random.Next(raceMapMax);
                if (!voteOptions.ContainsKey(_raceMap[next]))
                    voteOptions.Add(_raceMap[next], 0);
                if (voteOptions.Count == raceMapMax)
                    break;
            }
            session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_TITLE, "^3Next Track Vote, ^240 ^3sec.");
            Dictionary<ushort,byte>.Enumerator itr = voteOptions.GetEnumerator();
            byte optionCount = 1;
            while(itr.MoveNext())
            {
                switch(optionCount)
                {
                    case 1: session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_OPTION_1, "^7" + (Program.raceTemplate.GetEntry(itr.Current.Key)).Description); break;
                    case 2: session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_OPTION_2, "^7" + (Program.raceTemplate.GetEntry(itr.Current.Key)).Description); break;
                    case 3: session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_OPTION_3, "^7" + (Program.raceTemplate.GetEntry(itr.Current.Key)).Description); break;
                    case 4: session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_OPTION_4, "^7" + (Program.raceTemplate.GetEntry(itr.Current.Key)).Description); break;
                    case 5: session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_OPTION_5, "^7" + (Program.raceTemplate.GetEntry(itr.Current.Key)).Description); break;
                    case 6: session.SendUpdateButtonToAll((ushort)Button_Entry.VOTE_OPTION_6, "^7" + (Program.raceTemplate.GetEntry(itr.Current.Key)).Description); break;
                }
                optionCount++;
            }
            voteTimer = 41000; // 40 Secondes
            //Program.raceTemplate.GetEntry();
        }
        private void EndNextTrackVote()
        {
            voteTimer = 0;
            Dictionary<ushort, byte>.Enumerator itr = voteOptions.GetEnumerator();
            byte maxVote = 0;
            ushort chosedMap = 0;
            while (itr.MoveNext())
            {
                if (itr.Current.Value > maxVote)
                {
                    maxVote = itr.Current.Value;
                    chosedMap = itr.Current.Key;
                }
            }

            session.RemoveButtonToAll((ushort)Button_Entry.VOTE_TITLE);
            session.RemoveButtonToAll((ushort)Button_Entry.VOTE_OPTION_1);
            session.RemoveButtonToAll((ushort)Button_Entry.VOTE_OPTION_2);
            session.RemoveButtonToAll((ushort)Button_Entry.VOTE_OPTION_3);
            session.RemoveButtonToAll((ushort)Button_Entry.VOTE_OPTION_4);
            session.RemoveButtonToAll((ushort)Button_Entry.VOTE_OPTION_5);
            session.RemoveButtonToAll((ushort)Button_Entry.VOTE_OPTION_6);

            if (session.Script.NextTrackVoteEnded((ITrack)this, ref chosedMap))
            {
                voteInProgress = false;
                return;
            }

            LoadNextTrack(chosedMap);
        }
        public void LoadNextTrack(ushort trackEntry)
        {
            if (trackEntry == 0)
            {
                //Hope to never see that message, but since ppl can customize script, can happen someone let go a 0.
                Log.error(session.GetSessionNameForLog() + " VOTE System, LoadNextTrack(ushort trackEntry == 0), this should never happen");
                return;
            }
            previousRaceEntry = (ushort)raceInfo.Entry;
            raceInfo = Program.raceTemplate.GetEntry(trackEntry);

            LoadRestrictionJoin(raceInfo.Restriction_join_entry);
            LoadRestrictionRace(raceInfo.Restriction_race_entry);

            session.AddMessageMiddleToAll("^7Next Track Will Be ^3" + raceInfo.Description + "^7.", 10000);
            nextRaceTimer = 10000;
        }
        protected void ExecNextTrack()
        {
            if (raceInfo.TrackEntry == 0)
                return;

            session.SendMSTMessage("/select " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.CAN_SELECT_TRACK) ? "ban" : "no"));
            session.SendMSTMessage("/track " + Program.trackTemplate.GetEntry(raceInfo.TrackEntry).NamePrefix);
            session.SendMSTMessage("/qual " + raceInfo.QualifyMinute);
            session.SendMSTMessage("/laps " + raceInfo.LapCount);
            session.SendMSTMessage("/weather " + (byte)raceInfo.Weather);
            session.SendMSTMessage("/wind " + (byte)raceInfo.Wind);
            session.SendMSTMessage("/rstend 0");
            session.SendMSTMessage("/vote " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.CAN_VOTE) ? "yes" : "no"));

            session.SendMSTMessage("/autokick " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.AUTO_KICK_BAN) ? "ban" : "no"));
            session.SendMSTMessage("/autokick " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.AUTO_KICK_KICK) ? "kick" : "no"));
            session.SendMSTMessage("/autokick " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.AUTO_KICK_SPEC) ? "spec" : "no"));

            session.SendMSTMessage("/clear");
            session.SendMSTMessage("/cruise " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.ALLOW_WRONG_WAY) ? "yes" : "no"));
            session.SendMSTMessage("/canreset " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.CAN_RESET) ? "yes" : "no"));
            session.SendMSTMessage("/fcv " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.FORCE_COCKPIT_VIEW) ? "yes" : "no"));
            session.SendMSTMessage("/midrace " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.MID_RACE_JOIN) ? "yes" : "no"));
            session.SendMSTMessage("/mustpit " + (raceInfo.HasRaceTemplateFlag(Race_Template_Flag.MUST_PIT) ? "yes" : "no"));

            string carPrefix = "";
            if(raceInfo.CarEntryAllowed.IndexOf("all",StringComparison.InvariantCultureIgnoreCase) != -1)
                carPrefix = "all";
            else
            {
                string[] carEntrys = raceInfo.CarEntryAllowed.Split(new char[] { ' ' });
                uint carEntry;
                for (byte itr = 0; itr < carEntrys.Length; itr++)
                {
                    carEntry = 0;
                    try { carEntry = Convert.ToUInt32(carEntrys[itr]); }
                    catch(Exception exception)
                    {
                        Log.error("LoadNextTrack(), Colum race_template.car_entry_allowed("+raceInfo.CarEntryAllowed+") has a bad value\r\n");
                        continue;
                    }
                    carPrefix += Program.carTemplate.GetEntry(carEntry).NamePrefix + "+";
                }
                carPrefix = carPrefix.TrimEnd(new char[] { '+' });
            }
            session.SendMSTMessage("/cars " + carPrefix);
        }
        protected void EndRace()
        {
            doEnd = true;
            if (!race.IsLobbyScreen())
                session.SendMSTMessage("/end");
            else
                ExecNextTrack();
        }
        protected void SendVoteCancel()
        {
            ((Session)session).AddToTcpSendingQueud
            (new Packet(Packet_Size.PACKET_SIZE_TINY, Packet_Type.PACKET_TINY_MULTI_PURPOSE,
                    new PacketTiny(1, Tiny_Type.TINY_VTC)));
        }
    }
}
