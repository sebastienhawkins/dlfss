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
    using Definition_;
    using Storage_;
    using Game_;
    using Script_;
    using Ranking_;
    
    abstract class Restriction
    {
        protected Restriction(Session _session)
        {
            session = _session;
            race = (Race)this;
            track = (Track)this;
        }
        
        protected Session session;
        protected Race race;
        protected Track track;

        protected RestrictionJoinInfo restrictionJoinInfo = null;
        protected RestrictionRaceInfo restrictionRaceInfo = null;
        protected void LoadRestrictionJoin(uint entry)
        {
            if(entry == 0)
            {
                restrictionJoinInfo = null;
                return;
            }
            restrictionJoinInfo = Program.restrictionJoin.GetEntry(entry);
        }
        protected void LoadRestrictionRace(uint entry)
        {
            if (entry == 0)
            {
                restrictionRaceInfo = null;
                return;
            }
            restrictionRaceInfo = Program.restrictionRace.GetEntry(entry);
        }
        
        private Driver driver = null;
        private Rank rank = null;

        protected Penalty_Type_Ext CheckDriver(Driver _driver)
        {
            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(),driver.CarPrefix);
            Penalty_Type_Ext penType = Penalty_Type_Ext.NONE;
            Penalty_Type_Ext _penType = Penalty_Type_Ext.NONE;
            if (restrictionJoinInfo != null)
            {
                penType = SafePct();
                penType = (_penType = PBMin()) > penType ? _penType : penType;
                penType = (_penType = PBMax()) > penType ? _penType : penType;
                penType = (_penType = WRMin()) > penType ? _penType : penType;
                penType = (_penType = WRMax()) > penType ? _penType : penType;
                penType = (_penType = SkinName()) > penType ? _penType : penType;
                penType = (_penType = DriverName()) > penType ? _penType : penType;
                penType = (_penType = RankBestMin()) > penType ? _penType : penType;
                penType = (_penType = RankBestMax()) > penType ? _penType : penType;
                penType = (_penType = RankAvgMin()) > penType ? _penType : penType;
                penType = (_penType = RankAvgMax()) > penType ? _penType : penType;
                penType = (_penType = RankStaMin()) > penType ? _penType : penType;
                penType = (_penType = RankStaMax()) > penType ? _penType : penType;
                penType = (_penType = RankWinMin()) > penType ? _penType : penType;
                penType = (_penType = RankWinMax()) > penType ? _penType : penType;
                penType = (_penType = RankTotalMin()) > penType ? _penType : penType;
                penType = (_penType = RankTotalMax()) > penType ? _penType : penType;
            }
            ExecPenalityExt(penType);
            driver = null;
            rank = null;
            return penType;
        }
        private void ExecPenalityExt(Penalty_Type_Ext penType)
        {
            switch (penType)
            {
                case Penalty_Type_Ext.SPEC:
                    session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3Spec, Restriction Apply");
                    if (!driver.IsAdmin)
                        session.SendMSTMessage("/spec " + driver.LicenceName);
                    else
                        driver.SendMTCMessage("^5AdminDetected ^7restriction ^2Removed^7.");
                    break;
                case Penalty_Type_Ext.KICK:
                    session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3Kick, Restriction Apply");
                    if (!driver.IsAdmin)
                        session.SendMSTMessage("/kick " + driver.LicenceName);
                    else
                        driver.SendMTCMessage("^5AdminDetected ^7restriction ^2Removed^7.");
                    break;
                case Penalty_Type_Ext.NONE: break;
            }
        }
        
        public Penalty_Type_Ext SafePct(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            Penalty_Type_Ext penType = SafePct();
            driver = null;
            return penType;
        }
        private Penalty_Type_Ext SafePct()
        {
            if (driver.GetSafePct() < restrictionJoinInfo.SafeDrivingPct)
            {
                driver.SendMTCMessage("^1Safe Driving ^7restriction Apply.");
                driver.SendMTCMessage("^7A safe driving of ^3" + restrictionJoinInfo.SafeDrivingPct + "%^7 is needed.");
                driver.SendMTCMessage("^7You have a ^3safe driving^7 of ^3" + driver.GetSafePct() + "%^7.");
                driver.SendMTCMessage("^7Join 'Aleajecta School' and drive safe at all cost.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3Safe Driving, Restriction Apply");
                if (restrictionJoinInfo.SafeDrivingKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext PBMin(Driver _driver)
        {
            driver = _driver;
            Penalty_Type_Ext penType = PBMin();
            driver = null;
            return penType;
        }
        private Penalty_Type_Ext PBMin()
        {
            if (restrictionJoinInfo.PbMin != 0 && driver.pb != null && driver.pb.LapTime <= restrictionJoinInfo.PbMin)
            {
                driver.SendMTCMessage("^1PB ^7restriction Apply.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Higher Grade' Server.");
                driver.SendMTCMessage("^7PB of ^3" + ConvertX.MSTimeToHMSC(restrictionJoinInfo.PbMin, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7 is the fastest allowed.");
                driver.SendMTCMessage("^7You have a ^3PB^7 of ^3" + ConvertX.MSTimeToHMSC(driver.pb.LapTime, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3PBMin <=, Restriction Apply");
                if (restrictionJoinInfo.PbKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext PBMax(Driver _driver)
        {
            driver = _driver;
            Penalty_Type_Ext penType = PBMax();
            driver = null;

            return penType;
        }
        private Penalty_Type_Ext PBMax()
        {
            if (restrictionJoinInfo.PbMax != 0 && driver.pb != null && driver.pb.LapTime > restrictionJoinInfo.PbMax)
            {
                driver.SendMTCMessage("^1PB ^7restriction Apply.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                driver.SendMTCMessage("^7PB at least of ^3" + ConvertX.MSTimeToHMSC(restrictionJoinInfo.PbMax, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7 is needed.");
                driver.SendMTCMessage("^7You have a ^3PB^7 of ^3" + ConvertX.MSTimeToHMSC(driver.pb.LapTime, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3PBMax >, Restriction Apply");
                if (restrictionJoinInfo.PbKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext WRMin(Driver _driver)
        {
            driver = _driver;
            Penalty_Type_Ext penType = WRMin();
            driver = null;
            return penType;
        }
        private Penalty_Type_Ext WRMin()
        {
            int wrDiff = 0;
            if (restrictionJoinInfo.WrMin != 0 
                && driver.pb != null && driver.wr != null
                && (wrDiff = (int)driver.pb.LapTime - (int)driver.wr.LapTime) <= restrictionJoinInfo.WrMin)
            {
                driver.SendMTCMessage("^1WR ^7restriction Apply.");
                driver.SendMTCMessage("^7Join A^3leajecta ^7Higher Grade Server.");
                driver.SendMTCMessage("^7WR diff of ^3" + ConvertX.MSTimeToHMSC(restrictionJoinInfo.WrMin, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7 is the fastest allowed.");
                driver.SendMTCMessage("^7You have a ^3WR diff^7 of ^3" + ConvertX.MSTimeToHMSC(wrDiff, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3WRMin <=, Restriction Apply");
                if (restrictionJoinInfo.WrKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext WRMax(Driver _driver)
        {
            driver = _driver;
            Penalty_Type_Ext penType = WRMax();
            driver = null;
            return penType;
        }
        private Penalty_Type_Ext WRMax()
        {
            int wrDiff = 0;
            if (restrictionJoinInfo.WrMax != 0
                && driver.pb != null && driver.wr != null
                && (wrDiff = (int)driver.pb.LapTime - (int)driver.wr.LapTime) > restrictionJoinInfo.WrMax)
            {
                driver.SendMTCMessage("^1WR ^7restriction Apply.");
                driver.SendMTCMessage("^7Join A^3leajecta ^7Lower Grade Server.");
                driver.SendMTCMessage("^7WR diff of at least ^3" + ConvertX.MSTimeToHMSC(restrictionJoinInfo.WrMax, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7 is needed.");
                driver.SendMTCMessage("^7You have a ^3WR diff^7 of ^3" + ConvertX.MSTimeToHMSC(wrDiff, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3WRMax >, Restriction Apply");
                if (restrictionJoinInfo.WrKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext SkinName(Driver _driver)
        {
            driver = _driver;
            Penalty_Type_Ext penType = SkinName();
            driver = null;
            return penType;
        }
        private Penalty_Type_Ext SkinName()
        {
            if (restrictionJoinInfo.SkinName != "" 
                && driver.GetSkinName().IndexOf(restrictionJoinInfo.SkinName) == -1)
            {
                driver.SendMTCMessage("^1SkinName ^7restriction Apply.");
                driver.SendMTCMessage("^7SkinName must contain ^3" + restrictionJoinInfo.SkinName + "^7.");
                driver.SendMTCMessage("^7Your SkinName is ^3"+driver.GetSkinName());
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3SkinName !=, Restriction Apply");
                if (restrictionJoinInfo.SkinNameKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext DriverName(Driver _driver)
        {
            driver = _driver;
            Penalty_Type_Ext penType = DriverName();
            driver = null;
            return penType;
        }
        private Penalty_Type_Ext DriverName()
        {
            if (restrictionJoinInfo.DriverName != ""
                && driver.DriverName.IndexOf(restrictionJoinInfo.DriverName) == -1)
            {
                driver.SendMTCMessage("^1DriverName ^7restriction Apply.");
                driver.SendMTCMessage("^7DriverName must contain ^3" + restrictionJoinInfo.DriverName + "^7.");
                driver.SendMTCMessage("^7Your DriverName is ^3" + driver.DriverName);
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3DriverName !=, Restriction Apply");
                if (restrictionJoinInfo.DriverNameKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankBestMin(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankBestMin();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankBestMin()
        {
            if (restrictionJoinInfo.RankBestMin == 0)
                return Penalty_Type_Ext.NONE;
            if(rank == null)
            {
                driver.SendMTCMessage("^1Rank ^7'^1best^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3best^7' of ^3" + restrictionJoinInfo.RankBestMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have no Rank '^3best^7'.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankBest <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            if (rank.BestLap <= restrictionJoinInfo.RankBestMin)
            {
                driver.SendMTCMessage("^1Rank ^7'^1best^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3best^7' of ^3" + restrictionJoinInfo.RankBestMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have a Rank '^3best^7' of ^3" + rank.BestLap + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankBest <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankBestMax(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankBestMax();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankBestMax()
        {
            if (restrictionJoinInfo.RankBestMax == 0)
                return Penalty_Type_Ext.NONE;

            if (rank == null)
                return Penalty_Type_Ext.NONE;
            if (rank.BestLap > restrictionJoinInfo.RankBestMax)
            {
                driver.SendMTCMessage("^1Rank ^7'^1best^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3best^7' of ^3" + restrictionJoinInfo.RankBestMax + "^7 is the better allowed.");
                driver.SendMTCMessage("^7You have a Rank '^3best^7' of ^3" + rank.BestLap + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Higher Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankBest >, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankAvgMin(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankAvgMin();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankAvgMin()
        {
            if (restrictionJoinInfo.RankAvgMin == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
            {
                driver.SendMTCMessage("^1Rank ^7'^1avg^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3avg^7' of ^3" + restrictionJoinInfo.RankAvgMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have no Rank '^3avg^7'.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankAvg <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            if (rank.AverageLap <= restrictionJoinInfo.RankAvgMin)
            {
                driver.SendMTCMessage("^1Rank ^7'^1avg^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3avg^7' of ^3" + restrictionJoinInfo.RankAvgMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have a Rank '^3avg^7' of ^3" + rank.AverageLap + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankAvg <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankAvgMax(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankAvgMax();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankAvgMax()
        {
            if (restrictionJoinInfo.RankAvgMax == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
                return Penalty_Type_Ext.NONE;
            if (rank.AverageLap > restrictionJoinInfo.RankAvgMax)
            {
                driver.SendMTCMessage("^1Rank ^7'^1avg^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3avg^7' of ^3" + restrictionJoinInfo.RankAvgMax + "^7 is the better allowed.");
                driver.SendMTCMessage("^7You have a Rank '^3avg^7' of ^3" + rank.AverageLap + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Higher Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankAvg >, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankStaMin(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankStaMin();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankStaMin()
        {
            if (restrictionJoinInfo.RankStaMin == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
            {
                driver.SendMTCMessage("^1Rank ^7'^1sta^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3sta^7' of ^3" + restrictionJoinInfo.RankStaMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have no Rank '^3sta^7'.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankSta <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            if (rank.Stability <= restrictionJoinInfo.RankStaMin)
            {
                driver.SendMTCMessage("^1Rank ^7'^1sta^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3sta^7' of ^3" + restrictionJoinInfo.RankStaMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have a Rank '^3sta^7' of ^3" + rank.Stability + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankSta <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankStaMax(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankStaMax();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankStaMax()
        {
            if (restrictionJoinInfo.RankStaMax == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
                return Penalty_Type_Ext.NONE;
            if (rank.Stability > restrictionJoinInfo.RankStaMax)
            {
                driver.SendMTCMessage("^1Rank ^7'^1sta^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3sta^7' of ^3" + restrictionJoinInfo.RankStaMax + "^7 is the better allowed.");
                driver.SendMTCMessage("^7You have a Rank '^3sta^7' of ^3" + rank.BestLap + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Higher Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankSta >, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankWinMin(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankWinMin();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankWinMin()
        {
            if (restrictionJoinInfo.RankWinMin == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
            {
                driver.SendMTCMessage("^1Rank ^7'^1win^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3win^7' of ^3" + restrictionJoinInfo.RankWinMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have no Rank '^3win^7'.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankWin <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            if (rank.RaceWin <= restrictionJoinInfo.RankWinMin)
            {
                driver.SendMTCMessage("^1Rank ^7'^1win^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3win^7' of ^3" + restrictionJoinInfo.RankWinMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have a Rank '^3win^7' of ^3" + rank.RaceWin + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankWin <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankWinMax(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankWinMax();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankWinMax()
        {
            if (restrictionJoinInfo.RankWinMax == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
                return Penalty_Type_Ext.NONE;
            if (rank.RaceWin > restrictionJoinInfo.RankWinMax)
            {
                driver.SendMTCMessage("^1Rank ^7'^1win^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3win^7' of ^3" + restrictionJoinInfo.RankWinMax + "^7 is the better allowed.");
                driver.SendMTCMessage("^7You have a Rank '^3win^7' of ^3" + rank.RaceWin + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Higher Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankWin >, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankTotalMin(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankTotalMin();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankTotalMin()
        {
            if (restrictionJoinInfo.RankTotalMin == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
            {
                driver.SendMTCMessage("^1Rank ^7'^1total^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3total^7' of ^3" + restrictionJoinInfo.RankTotalMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have no Rank '^3total^7'.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankTotal <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            if (rank.Total <= restrictionJoinInfo.RankTotalMin)
            {
                driver.SendMTCMessage("^1Rank ^7'^1total^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3total^7' of ^3" + restrictionJoinInfo.RankTotalMin + "^7 is needed.");
                driver.SendMTCMessage("^7You have a Rank '^3total^7' of ^3" + rank.Total + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankTotal <=, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }

        public Penalty_Type_Ext RankTotalMax(Driver _driver)
        {
            if (restrictionJoinInfo == null)
                return Penalty_Type_Ext.NONE;

            driver = _driver;
            rank = driver.GetRank(session.GetRaceTrackPrefix(), driver.CarPrefix);
            Penalty_Type_Ext penType = RankTotalMax();
            driver = null;
            rank = null;
            return penType;
        }
        private Penalty_Type_Ext RankTotalMax()
        {
            if (restrictionJoinInfo.RankTotalMax == 0)
                return Penalty_Type_Ext.NONE;
            if (rank == null)
                return Penalty_Type_Ext.NONE;
            if (rank.Total > restrictionJoinInfo.RankTotalMax)
            {
                driver.SendMTCMessage("^1Rank ^7'^1total^7' restriction Apply.");
                driver.SendMTCMessage("^7A Rank ^7'^3total^7' of ^3" + restrictionJoinInfo.RankTotalMax + "^7 is the better allowed.");
                driver.SendMTCMessage("^7You have a Rank '^3total^7' of ^3" + rank.Total + "^7.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Higher Grade' Server.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3RankTotal >, Restriction Apply");
                if (restrictionJoinInfo.RankKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }
    }
}