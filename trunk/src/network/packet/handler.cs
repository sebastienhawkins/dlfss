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
namespace Drive_LFSS.Packet_
{
    using Drive_LFSS.PacketStore_;
    using Drive_LFSS.Server_;
    using Drive_LFSS.Log_;

    public abstract class PacketHandler : Store
    {
        public void ProcessPacket(Packet_Type _packetType, object _packet)
        {
            switch (_packetType)
            {
                case Packet_Type.PACKET_MCI_MULTICAR_INFORMATION:      processPacket((PacketMCI)_packet);break;
                case Packet_Type.PACKET_MSO_CHAT_RECEIVED:             processPacket((PacketMSO)_packet);break;
                case Packet_Type.PACKET_TINY_MULTI_PURPOSE:            processPacket((PacketTiny)_packet);break;
                case Packet_Type.PACKET_STA_DRIVER_RACE_STATE_CHANGE:  processPacket((PacketSTA)_packet);break;
                case Packet_Type.PACKET_NCN_NEW_CONNECTION:            processPacket((PacketNCN)_packet);break;
                case Packet_Type.PACKET_CNL_PART_CONNECTION:           processPacket((PacketCNL)_packet);break;
                case Packet_Type.PACKET_NPL_DRIVER_JOIN_RACE:          processPacket((PacketNPL)_packet);break;
                case Packet_Type.PACKET_PLL_DRIVER_LEAVE_RACE:         processPacket((PacketPLL)_packet);break;
                case Packet_Type.PACKET_RST_RACE_START:                processPacket((PacketRST)_packet);break;

                case Packet_Type.PACKET_VER_VERSION_SERVER:            processPacket((PacketVER)_packet);break;
                default: Log.missingDefinition("ProcessPacket(), No Existing PacketHandler for packetType->" + _packetType + "\r\n"); break;
            }
        }

        protected virtual void processPacket(PacketVER _packet)
        {
            Log.debug("Process packet ->" + _packet + "\r\n");
            Log.normal("InSim v" + _packet.inSimVersion + ", Licence: " + _packet.productVersion + "." + _packet.serverVersion + "\r\n");
        }
        protected virtual void processPacket(PacketTiny _packet)
        {
            Log.debug("Process packet TINY\r\n");
        }
        protected virtual void processPacket(PacketMSO _packet)
        {
            Log.debug("Process packet MSO\r\n");
        }
        protected virtual void processPacket(PacketNCN _packet)
        {
            Log.debug("LicenceJoinsHost(), NCN -> UCID= " + _packet.tempLicenceId + ", UName=" + _packet.licenceName + ", PName=" + _packet.driverName + ", Flags=" + _packet.driverTypeMask + ", Admin=" + _packet.adminStatus + "\r\n");
        }
        protected virtual void processPacket(PacketCNL _packet)
        {
            Log.debug("CNL_ClientLeavesHost, CNL -> UCID= " + _packet.tempLicenceId + ", Reason=" + _packet.Reason + ", Total=" + _packet.Total + "\r\n");
        }
        protected virtual void processPacket(PacketCPR _packet)
        {
            Log.debug("CPR_ClientRenames(), CPR->UCID=" + _packet.UCID + ", PName=" + _packet.PName + ", Plate=" + _packet.Plate + ", ReqI=" + _packet.ReqI + "\n\r");
        }
        protected virtual void processPacket(PacketTOC _packet)
        {
            Log.debug("TOC_PlayerCarTakeOver(), TOC -> PLID=" + _packet.carId + ", OldUCID=" + _packet.oldTLID + ", NewUCID=" + _packet.newTLID + "\r\n");
        }
        protected virtual void processPacket(PacketPLL _packet)
        {
            Log.debug("PacketPLL, NPL -> PLID=" + _packet.carId + "\r\n");
        }
        // Player help settings changed.
        protected virtual void processPacket(PacketPFL _packet)
        {
            Log.debug("PFL_PlayerFlagsChanged(), PFL -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", Spare=" + _packet.Spare + "\r\n");
        }
        // A player goes to the garage (setup screen).
        protected virtual void processPacket(PacketPLP _packet)
        {
            Log.debug("PLP_PlayerGoesToGarage(), PLP -> PLID=" + _packet.carId + "\r\n");
        }
        // A player joins the race. If PLID already exists, then player leaves pit.
        protected virtual void processPacket(PacketNPL _packet)
        {
            Log.debug("NPL_PlayerJoinsRace(), NPL -> CName=" + _packet.carName + ", PName=" + _packet.driverName + ", PLID=" + _packet.carId + ", UCID=" + _packet.tempLicenceId + ", PType=" + _packet.driverTypeMask + "\r\n");
        }
        protected virtual void processPacket(PacketLAP _packet)
        {
            Log.debug("LAP_PlayerCompletesLap(), LAP -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", LTIME=" + _packet.LTime + " ms" + ", LapsDone=" + _packet.LapsDone + ", ETime=" + _packet.ETime + " ms" + " \r\n");
        }
        protected virtual void processPacket(PacketFIN _packet)
        {
            Log.debug("FIN_PlayerFinishedRaces(), FIN -> PLID=" + _packet.PLID + ", Confirm=" + _packet.Confirm + ", Flags=" + _packet.Flags + ", BTIME=" + _packet.BTime + " ms" + ", TTIME=" + _packet.TTime + " ms" + ", LapsDone=" + _packet.LapsDone + ", NumStops=" + _packet.NumStops + ",  \r\n");
        }
        protected virtual void processPacket(PacketCRS _packet)
        {
            Log.debug("CRS_PlayerResetsCar(), CRS -> PLID=" + _packet.PLID + "\r\n");
        }
        protected virtual void processPacket(PacketPIT _packet)
        {
            Log.debug("PIT_PlayerStopsAtPit(), PIT -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", LapsDone=" + _packet.LapsDone + ", NumStops=" + _packet.NumStops + ", Penalty=" + _packet.Penalty + ", FL_Changed=" + _packet.FL_Changed + ", FR_Changed=" + _packet.FR_Changed + ", RL_Changed=" + _packet.RL_Changed + ", RR_Changed=" + _packet.RR_Changed + ", Work=" + _packet.Work + ", Spare=" + _packet.Spare + "\r\n");
        }
        protected virtual void processPacket(PacketPSF _packet)
        {
            Log.debug("PSF_PitStopFinished(), PSF -> PLID=" + _packet.carId + ", Spare=" + _packet.Spare + ", STime=" + _packet.STime + " ms" + "\r\n");
        }
        protected virtual void processPacket(PacketAXO _packet)
        {
            Log.debug("AXO_PlayerHitsAutocrossObject(), AXO -> PLID=" + _packet.PLID + "\r\n");
        }
        // A player pressed KEY_SHIFT+I or KEY_SHIFT+B
        protected virtual void processPacket(PacketBFN _packet)
        {
            Log.debug("BFN_PlayerRequestsButtons(), BFN -> UCID=" + _packet.UCID + ", ClickID=" + _packet.ClickID + ", SubT=" + _packet.SubT + "\r\n");
        }
        protected virtual void processPacket(PacketBTC _packet)
        {
            Log.debug("BTC_ButtonClicked(), BTC -> UCID=" + _packet.UCID + ",ClickID=" + _packet.ClickID + ", CFlags=" + _packet.CFlags + "\r\n");
        }
        protected virtual void processPacket(PacketBTT _packet)
        {
            Log.debug("BTT_TextBoxOkClicked(), BTT -> UCID=" + _packet.UCID + ", ClickID=" + _packet.ClickID + ", Text=" + _packet.Text + ", TypeIn=" + _packet.TypeIn + "\r\n");
        }
        protected virtual void processPacket(PacketPEN _packet)
        {
            Log.debug("PEN_PenaltyChanged(), PEN -> PLID=" + _packet.PLID + ", Reason=" + _packet.Reason + ", NewPen=" + _packet.NewPen + ", OldPen=" + _packet.OldPen + "\r\n");
        }
        // Yellow or blue flag changed
        protected virtual void processPacket(PacketFLG _packet)
        {
            Log.debug("FLG_FlagChanged(), FLG -> PLID=" + _packet.PLID + ", Flag=" + _packet.Flag + ", OffOn=" + _packet.OffOn + ", CarBehind=" + _packet.CarBehind + "\r\n");
        }
        // A player entered or left the pitlane
        protected virtual void processPacket(PacketPLA _packet)
        {
            Log.debug("PLA_PitLaneChanged(), PLA -> PLID=" + _packet.carId + ", Fact=" + _packet.Fact + "\r\n");
        }
        // A player crossed a lap split
        protected virtual void processPacket(PacketSPX _packet)
        {
            Log.debug("SPX_SplitTime(), SPX -> PLID=" + _packet.carId + ", Split=" + _packet.Split + ", STime=" + _packet.STime + " ms" + ", ETime=" + _packet.ETime + " ms" + ",  NumStops=" + _packet.NumStops + ", Penalty=" + _packet.Penalty + "\r\n");
        }
        // A player changed his camera
        protected virtual void processPacket(PacketCCH _packet)
        {
            Log.debug("CCH_CameraChanged(), CCH -> PLID=" + _packet.PLID + ", Camera=" + _packet.Camera + "\r\n");
        }
        // The server/race state changed
        protected virtual void processPacket(PacketSTA _packet)
        {
            Log.debug("STA_StateChanged(), STA -> Flags=" + _packet.viewOptionMask + ", RaceInProg=" + _packet.raceInProgress + ", ViewPLID=" + _packet.currentCarId + ", NumConns=" + _packet.connectionCount + ", NumP=" + _packet.carCount + ", QualMins=" + _packet.qualificationMinute + "\r\n");
        }
        // A host is started or joined
        protected virtual void processPacket(PacketISM _packet)
        {
            Log.debug("ISM_MultiplayerInformation(), ISM -> HName=" + _packet.HName + ", Host=" + _packet.Host + "\r\n");
        }
        // A host ends or leaves

        // Race got cleared with /clear
        protected virtual void processPacket()
        {
            Log.debug("CLR_RaceCleared(), Got a Ping Request, What ToDo With That???\r\n");
        }
        // Sent at the start of every race or qualifying session, listing the start order
        protected virtual void processPacket(PacketREO _packet)
        {
            Log.debug("REO_RaceStartOrder(), REO -> PLID=" + _packet.carId + ", NumP=" + _packet.NumP + "\r\n");
        }
        // Race start information
        protected virtual void processPacket(PacketRST _packet)
        {
            Log.debug("RST_RaceStart(), RST -> Track=" + _packet.trackName + ", Finish=" + _packet.nodeFinishIndex + ", NumP=" + _packet.carCount + ", NumNodes=" + _packet.nodeCount + ", Flags=" + _packet.raceFeatureMask + ", QualMins=" + _packet.qualificationMinute + ", RaceLaps=" + _packet.raceLaps + ", Spare=" + _packet.Spare + ", Weather=" + _packet.weatherStatus + ", Wind=" + _packet.windStatus + ", Split1=" + _packet.nodeSplit1Index + ", Split2=" + _packet.nodeSplit2Index + ", Split3=" + _packet.nodeSplit3Index + "\r\n");
        }
        // Qualify or confirmed finish
        protected virtual void processPacket(PacketRES _packet)
        {
            Log.debug("RES_RaceOrQualifyingResult(), RES -> PLID=" + _packet.carId + ", UName=" + _packet.UName + ", PName=" + _packet.PName + ", PSeconds=" + _packet.PSeconds + ", ResultNum=" + _packet.ResultNum + ", SpA=" + _packet.SpA + ", SpB=" + _packet.SpB + ", TTime=" + _packet.TTime + " ms" + ", BTime=" + _packet.BTime + " ms" + ", Plate=" + _packet.Plate + ", LapsDone=" + _packet.LapsDone + ", NumRes=" + _packet.NumRes + ", NumStops=" + _packet.NumStops + ", Flags=" + _packet.Flags + ", Confirm=" + _packet.Confirm + ", CName=" + _packet.CName + "\r\n");
        }
        // A race ends (return to game setup screen)
        // Current race time progress in hundredths
        protected virtual void processPacket(uint RTP)
        {
            Log.debug("RTP_RaceTime(), RTP -> RTP=" + RTP + "\r\n");
        }
        // Autocross got cleared
        
        // Request - autocross layout information
        protected virtual void processPacket(PacketAXI _packet)
        {
            Log.debug("AXI_AutocrossLayoutInformation(), AXI -> NumCP=" + _packet.NumCP + ", NumO=" + _packet.NumO + ", AXStart=" + _packet.AXStart + ", LName=" + _packet.LName + "\r\n");
        }
        // LFS reporting camera position and state
        protected virtual void processPacket(PacketCPP _packet)
        {
            Log.debug("CPP_CameraPosition(), CPP -> ViewPLID=" + _packet.ViewPLID + ", Flags=" + _packet.Flags + ", X=" + _packet.X + ", Y=" + _packet.Y + ", Z=" + _packet.Z + ", Time=" + _packet.Time + ", R=" + _packet.R + ", P=" + _packet.P + ", InGameCam=" + _packet.InGameCam + ", H=" + _packet.H + ", FOV=" + _packet.FOV + "\r\n");
        }
        // A vote completed
        protected virtual void processPacket(byte _packet)
        {
            Log.debug("VTA_VoteAction(), byte VTA=" + _packet + "\r\n");
        }
        // A vote got called
        protected virtual void processPacket(PacketVTN _packet)
        {
            Log.debug("VTN_VoteNotify(), VTN -> UCID=" + _packet.tempLicenceId + ", Action=" + _packet.voteAction + ", Spare2=" + _packet.Spare2 + ", Spare3=" + _packet.Spare3 + "\r\n");
        }
        // Detailed car information packet (max 8 per packet)
        protected virtual void processPacket(PacketMCI _packet)
        {
            Log.debug("MCI_CarInformation(), MCI -> Info=" + _packet.carInformation + ", NumC=" + _packet.NumC + "\r\n");
        }
        // Compact car information packet
        protected virtual void processPacket(PacketNLP _packet)
        {
            Log.debug("NLP_LapNode(), NLP -> Info=" + _packet.Info + ", NumP=" + _packet.NumP + "\r\n");
        }
        // A /i message got sent to this program
        protected virtual void processPacket(PacketIII _packet)
        {
            Log.debug("III_InSimInfo(), III -> Msg=" + _packet.Msg + ", PLID=" + _packet.PLID + ", UCID=" + _packet.UCID + "\r\n");
        }

    }
}