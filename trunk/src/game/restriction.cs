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

        protected Penalty_Type_Ext CheckDriver(Driver _driver)
        {
            driver = _driver;
            Penalty_Type_Ext penType = Penalty_Type_Ext.NONE;
            Penalty_Type_Ext _penType = Penalty_Type_Ext.NONE;
            if (restrictionJoinInfo != null)
            {
                penType = SafePct();
                penType = (_penType = PBMin()) > penType ? _penType : penType;
                penType = (_penType = PBMax()) > penType ? _penType : penType;
                penType = (_penType = WRMin()) > penType ? _penType : penType;
                penType = (_penType = WRMax()) > penType ? _penType : penType;
            }
            switch(penType)
            {
                case Penalty_Type_Ext.SPEC:
                    session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3Spec, Restriction Apply");
                    session.SendMSTMessage("/spec " + driver.LicenceName);
                    break;
                case Penalty_Type_Ext.KICK:
                    session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3Kick, Restriction Apply");
                    session.SendMSTMessage("/kick " + driver.LicenceName);
                    break;
                case Penalty_Type_Ext.NONE:break;
            }
            driver = null;
            return penType;
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
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3PBMin, Restriction Apply");
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
            if (restrictionJoinInfo.PbMax != 0 && driver.pb != null && driver.pb.LapTime >= restrictionJoinInfo.PbMax)
            {
                driver.SendMTCMessage("^1PB ^7restriction Apply.");
                driver.SendMTCMessage("^7Join 'A^3leajecta ^7Lower Grade' Server.");
                driver.SendMTCMessage("^7PB at least of ^3" + ConvertX.MSTimeToHMSC(restrictionJoinInfo.PbMax, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7 is needed.");
                driver.SendMTCMessage("^7You have a ^3PB^7 of ^3" + ConvertX.MSTimeToHMSC(driver.pb.LapTime, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3PBMax, Restriction Apply");
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
                && (wrDiff = (int)driver.pb.LapTime - (int)driver.wr.LapTime) < restrictionJoinInfo.WrMin)
            {
                driver.SendMTCMessage("^1WR ^7restriction Apply.");
                driver.SendMTCMessage("^7Join A^3leajecta ^7Higher Grade Server.");
                driver.SendMTCMessage("^WR diff of ^3" + ConvertX.MSTimeToHMSC(restrictionJoinInfo.WrMin, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7 is the fastest allowed.");
                driver.SendMTCMessage("^7You have a ^3WR diff^7 of ^3" + ConvertX.MSTimeToHMSC(wrDiff, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3WRMin, Restriction Apply");
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
                driver.SendMTCMessage("^WR diff of at least ^3" + ConvertX.MSTimeToHMSC(restrictionJoinInfo.WrMax, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7 is needed.");
                driver.SendMTCMessage("^7You have a ^3WR diff^7 of ^3" + ConvertX.MSTimeToHMSC(wrDiff, Msg.COLOR_DIFF_EVENT, Msg.COLOR_DIFF_EVENT) + "^7.");
                session.SendMTCMessageToAllAdmin(driver.DriverName + " ^7-> ^3WRMax, Restriction Apply");
                if (restrictionJoinInfo.WrKick)
                    return Penalty_Type_Ext.KICK;
                return Penalty_Type_Ext.SPEC;
            }
            return Penalty_Type_Ext.NONE;
        }
    }
}