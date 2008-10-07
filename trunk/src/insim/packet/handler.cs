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
    using Drive_LFSS.Game_;
    using Drive_LFSS.Log_;

    public abstract class PacketHandler : Store
    {
        protected PacketHandler()
        {
        }
        ~PacketHandler()
        {
            if (true == false) { }
        }
        protected void ProcessPacket(Packet_Type _packetType, object _packet)
        {
            switch (_packetType)
            {
                case Packet_Type.PACKET_MCI_MULTICAR_INFORMATION:      processPacket((PacketMCI)_packet);break;
                case Packet_Type.PACKET_MSO_CHAT_RECEIVED:             processPacket((PacketMSO)_packet);break;
                case Packet_Type.PACKET_TINY_MULTI_PURPOSE:            processPacket((PacketTiny)_packet);break;
                case Packet_Type.PACKET_SMALL_MULTI_PURPOSE:           processPacket((PacketSmall)_packet);break;
                case Packet_Type.PACKET_STA_DRIVER_RACE_STATE_CHANGE:  processPacket((PacketSTA)_packet);break;
                case Packet_Type.PACKET_VTN_VOTE_NOTIFICATION:         processPacket((PacketVTN)_packet);break;
                case Packet_Type.PACKET_SPX_DRIVER_SPLIT_TIME:         processPacket((PacketSPX)_packet);break;
                case Packet_Type.PACKET_LAP_DRIVER_LAP_TIME:           processPacket((PacketLAP)_packet);break;
                case Packet_Type.PACKET_NCN_NEW_CONNECTION:            processPacket((PacketNCN)_packet);break;
                case Packet_Type.PACKET_CNL_PART_CONNECTION:           processPacket((PacketCNL)_packet);break;
                case Packet_Type.PACKET_NPL_DRIVER_JOIN_RACE:          processPacket((PacketNPL)_packet);break;
                case Packet_Type.PACKET_PLL_DRIVER_LEAVE_RACE:         processPacket((PacketPLL)_packet);break;
                case Packet_Type.PACKET_RST_RACE_START:                processPacket((PacketRST)_packet);break;
                case Packet_Type.PACKET_VER_VERSION_SERVER:            processPacket((PacketVER)_packet);break;
                case Packet_Type.PACKET_REO_RACE_GRID_ORDER:           processPacket((PacketREO)_packet);break;
                case Packet_Type.PACKET_FIN_DRIVER_FINISH_RACE:        processPacket((PacketFIN)_packet);break;
                case Packet_Type.PACKET_RES_RESULT_CONFIRMED:          processPacket((PacketRES)_packet);break;
                case Packet_Type.PACKET_BTC_BUTTON_CLICK:              processPacket((PacketBTC)_packet);break;
                case Packet_Type.PACKET_BTT_BUTTON_TYPE_IN_TEXT_OK:    processPacket((PacketBTT)_packet);break;
                case Packet_Type.PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE: processPacket((PacketBFN)_packet);break;
                case Packet_Type.PACKET_PLP_ENTER_GARAGE:              processPacket((PacketPLP)_packet);break;
                case Packet_Type.PACKET_CPR_LICENCE_DRIVER_RENAME:     processPacket((PacketCPR)_packet);break;

                default: Log.missingDefinition("ProcessPacket(), No existing PacketHandler for packetType->" + _packetType + "\r\n"); break;
            }
        }

        protected void processPacket(PacketVER _packet)
        {
            #if DEBUG
            Log.debug(((Session)this).GetSessionNameForLog() + " Process packet -> " + _packet + "\r\n");
            #endif
            Log.normal(((Session)this).GetSessionNameForLog() + " InSim v" + _packet.inSimVersion + ", Licence: " + _packet.productVersion + "." + _packet.serverVersion + "\r\n");
        }
        protected virtual void processPacket(PacketTiny _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " Process packet TINY\r\n");
        }
        protected virtual void processPacket(PacketSmall _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " Process packet SMALL\r\n");
        }
        protected virtual void processPacket(PacketMSO _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " Process packet MSO\r\n");
        }
        protected virtual void processPacket(PacketNCN _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " LicenceJoinsHost(), NCN -> UCID= " + _packet.connectionId + ", UName=" + _packet.licenceName + ", PName=" + _packet.driverName + ", Flags=" + _packet.driverTypeMask + ", Admin=" + _packet.adminStatus + "\r\n");
        }
        protected virtual void processPacket(PacketCNL _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " CNL_ClientLeavesHost, CNL -> UCID= " + _packet.connectionId + ", Reason=" + _packet.quitReason + ", Total=" + _packet.total + "\r\n");
        }
        protected virtual void processPacket(PacketCPR _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " CPR_ClientRenames(), CPR->UCID=" + _packet.connectionId + ", PName=" + _packet.driverName + ", Plate=" + _packet.carPlate + ", ReqI=" + _packet.requestId + "\n\r");
        }
        protected virtual void processPacket(PacketTOC _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " TOC_PlayerCarTakeOver(), TOC -> PLID=" + _packet.carId + ", OldUCID=" + _packet.oldTLID + ", NewUCID=" + _packet.newTLID + "\r\n");
        }
        protected virtual void processPacket(PacketPLL _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " PacketPLL, NPL -> PLID=" + _packet.carId + "\r\n");
        }
        // Player help settings changed.
        protected virtual void processPacket(PacketPFL _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " PFL_PlayerFlagsChanged(), PFL -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", Spare=" + _packet.Spare + "\r\n");
        }
        // A player goes to the garage (setup screen).
        protected virtual void processPacket(PacketPLP _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " PLP_PlayerGoesToGarage(), PLP -> PLID=" + _packet.carId + "\r\n");
        }
        // A player joins the race. If PLID already exists, then player leaves pit.
        protected virtual void processPacket(PacketNPL _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " NPL_PlayerJoinsRace(), NPL -> CName=" + _packet.carPrefix + ", PName=" + _packet.driverName + ", PLID=" + _packet.carId + ", UCID=" + _packet.connectionId + ", PType=" + _packet.driverTypeMask + "\r\n");
        }
        protected virtual void processPacket(PacketLAP _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " LAP_PlayerCompletesLap(), LAP -> PLID=" + _packet.carId + ", Flags=" + _packet.driverMask + ", LTIME=" + _packet.lapTime + " ms" + ", LapsDone=" + _packet.lapCompleted + ", ETime=" + _packet.totalTime + " ms" + " \r\n");
        }
        protected virtual void processPacket(PacketFIN _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " FIN_PlayerFinishedRaces(), FIN -> PLID=" + _packet.carId + ", Confirm=" + _packet.confirmMask + ", Flags=" + _packet.driverMask + ", BTIME=" + _packet.fastestLap + " ms" + ", TTIME=" + _packet.totalTime + " ms" + ", LapsDone=" + _packet.totalLap + ", NumStops=" + _packet.pitStopCount + ",  \r\n");
        }
        protected virtual void processPacket(PacketCRS _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " CRS_PlayerResetsCar(), CRS -> PLID=" + _packet.PLID + "\r\n");
        }
        protected virtual void processPacket(PacketPIT _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " PIT_PlayerStopsAtPit(), PIT -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", LapsDone=" + _packet.LapsDone + ", NumStops=" + _packet.NumStops + ", Penalty=" + _packet.Penalty + ", FL_Changed=" + _packet.FL_Changed + ", FR_Changed=" + _packet.FR_Changed + ", RL_Changed=" + _packet.RL_Changed + ", RR_Changed=" + _packet.RR_Changed + ", Work=" + _packet.Work + ", Spare=" + _packet.Spare + "\r\n");
        }
        protected virtual void processPacket(PacketPSF _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " PSF_PitStopFinished(), PSF -> PLID=" + _packet.carId + ", Spare=" + _packet.Spare + ", STime=" + _packet.STime + " ms" + "\r\n");
        }
        protected virtual void processPacket(PacketAXO _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " AXO_PlayerHitsAutocrossObject(), AXO -> PLID=" + _packet.PLID + "\r\n");
        }
        // A player pressed KEY_SHIFT+I or KEY_SHIFT+B
        protected virtual void processPacket(PacketBFN _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " BFN_PlayerRequestsButtons(), BFN -> UCID=" + _packet.connectionId + ", MultiId=" + _packet.buttonId + ", ButtonFunc=" + _packet.buttonFunction + "\r\n");
        }
        protected virtual void processPacket(PacketBTC _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " BTC_ButtonClicked(), extendedmask=" +_packet.extendedMask+", requestId="+_packet.requestId+", UCID=" + _packet.connectionId + ",ClickID=" + _packet.buttonId + ", CFlags=" + _packet.clickMask + "\r\n");
        }
        protected virtual void processPacket(PacketBTT _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " BTT_TextBoxOkClicked(), BTT -> UCID=" + _packet.connectionId + ", ClickID=" + _packet.buttonId + ", Text=" + _packet.typedText + ", TypeIn=" + _packet.originalTextLength + "\r\n");
        }
        protected virtual void processPacket(PacketPEN _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " PEN_PenaltyChanged(), PEN -> PLID=" + _packet.PLID + ", Reason=" + _packet.Reason + ", NewPen=" + _packet.NewPen + ", OldPen=" + _packet.OldPen + "\r\n");
        }
        // Yellow or blue flag changed
        protected virtual void processPacket(PacketFLG _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " FLG_FlagChanged(), FLG -> PLID=" + _packet.PLID + ", Flag=" + _packet.Flag + ", OffOn=" + _packet.OffOn + ", CarBehind=" + _packet.CarBehind + "\r\n");
        }
        // A player entered or left the pitlane
        protected virtual void processPacket(PacketPLA _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " PLA_PitLaneChanged(), PLA -> PLID=" + _packet.carId + ", Fact=" + _packet.Fact + "\r\n");
        }
        // A player crossed a lap split
        protected virtual void processPacket(PacketSPX _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " SPX_SplitTime(), SPX -> PLID=" + _packet.carId + ", Split=" + _packet.splitNode + ", STime=" + _packet.splitTime + " ms" + ", ETime=" + _packet.totalTime + " ms" + ",  NumStops=" + _packet.pitStopTotal + ", Penalty=" + _packet.currentPenalty + "\r\n");
        }
        // A player changed his camera
        protected virtual void processPacket(PacketCCH _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " CCH_CameraChanged(), CCH -> PLID=" + _packet.carId + ", Camera=" + _packet.camera + "\r\n");
        }
        // The server/race state changed
        protected virtual void processPacket(PacketSTA _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " STA_StateChanged() -> ViewOptionMask=" + _packet.viewOptionMask + ", RaceInProgStatus=" + _packet.raceInProgressStatus + ", ViewPLID=" + _packet.currentCarId + ", NumConns=" + _packet.connectionCount + ", NumCar=" + _packet.carCount + ", FinishCount="+_packet.finishedCount+", QualMins=" + _packet.qualificationMinute + ", raceLaps="+ _packet.raceLaps+", ReplaySpeed="+_packet.replaySpeed+", TrackName="+_packet.trackPrefix+", WeatherStatus="+_packet.weatherStatus+"\r\n");
        }
        // A host is started or joined
        protected virtual void processPacket(PacketISM _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " ISM_MultiplayerInformation(), ISM -> HName=" + _packet.HName + ", Host=" + _packet.Host + "\r\n");
        }
        // A host ends or leaves

        // Race got cleared with /clear
        protected virtual void processPacket()
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " CLR_RaceCleared(), Got a ping request, but what to do with that?\r\n");
        }
        // Sent at the start of every race or qualifying session, listing the start order
        protected virtual void processPacket(PacketREO _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " REO_RaceStartOrder(), REO -> PLID=" + _packet.carIds + ", NumP=" + _packet.carCount + "\r\n");
        }
        // Race start information
        protected virtual void processPacket(PacketRST _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " RST_RaceStart(), RST -> Track=" + _packet.trackPrefix + ", Finish=" + _packet.nodeFinishIndex + ", NumP=" + _packet.carCount + ", NumNodes=" + _packet.nodeCount + ", Flags=" + _packet.raceFeatureMask + ", QualMins=" + _packet.qualificationMinute + ", RaceLaps=" + _packet.raceLaps + ", Spare=" + _packet.Spare + ", Weather=" + _packet.weatherStatus + ", Wind=" + _packet.windStatus + ", Split1=" + _packet.nodeSplit1Index + ", Split2=" + _packet.nodeSplit2Index + ", Split3=" + _packet.nodeSplit3Index + "\r\n");
        }
        // Qualify or confirmed finish
        protected virtual void processPacket(PacketRES _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " RES_RaceOrQualifyingResult(),-> PLID=" + _packet.carId + ", UName=" + _packet.licenceName + ", PName=" + _packet.driverName + ", PSeconds=" + _packet.penalitySecTotal + ", PositionFinal=" + _packet.positionFinal + ", TTime=" + _packet.totalTime + " ms" + ", bestTime=" + _packet.fastestLapTime + " ms" + ", Plate=" + _packet.carPlate + ", LapsDone=" + _packet.lapCount + ", NumRes=" + _packet.resultCount + ", NumStops=" + _packet.pitStopCount + ", driverMask=" + _packet.driverMask + ", Confirm=" + _packet.confirmMask + ", skinName=" + _packet.skinPrefix + "\r\n");
        }
        // A race ends (return to game setup screen)
        // Current race time progress in hundredths
        protected virtual void processPacket(uint RTP)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " RTP_RaceTime(), RTP -> RTP=" + RTP + "\r\n");
        }
        // Autocross got cleared
        
        // Request - autocross layout information
        protected virtual void processPacket(PacketAXI _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " AXI_AutocrossLayoutInformation(), AXI -> NumCP=" + _packet.NumCP + ", NumO=" + _packet.NumO + ", AXStart=" + _packet.axStart + ", LName=" + _packet.LName + "\r\n");
        }
        // LFS reporting camera position and state
        protected virtual void processPacket(PacketCPP _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " CPP_CameraPosition(), CPP -> ViewPLID=" + _packet.ViewPLID + ", Flags=" + _packet.Flags + ", X=" + _packet.X + ", Y=" + _packet.Y + ", Z=" + _packet.Z + ", Time=" + _packet.Time + ", R=" + _packet.R + ", P=" + _packet.P + ", InGameCam=" + _packet.InGameCam + ", H=" + _packet.H + ", FOV=" + _packet.FOV + "\r\n");
        }
        // A vote completed
        protected virtual void processPacket(byte _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " VTA_VoteAction(), byte VTA=" + _packet + "\r\n");
        }
        // A vote got called
        protected virtual void processPacket(PacketVTN _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " VTN_VoteNotify(), VTN -> UCID=" + _packet.connectionId + ", Action=" + _packet.voteAction + ", Spare2=" + _packet.spare2 + ", Spare3=" + _packet.spare3 + "\r\n");
        }
        // Detailed car information packet (max 8 per packet)
        protected virtual void processPacket(PacketMCI _packet)
        {
            //Log.debug("MCI_CarInformation(), MCI -> Info=" + _packet.carInformation + ", NumC=" + _packet.NumC + "\r\n");
        }
        // Compact car information packet
        protected virtual void processPacket(PacketNLP _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " NLP_LapNode(), NLP -> Info=" + _packet.Info + ", NumP=" + _packet.NumP + "\r\n");
        }
        // A /i message got sent to this program
        protected virtual void processPacket(PacketIII _packet)
        {
            Log.debug(((Session)this).GetSessionNameForLog() + " III_InSimInfo(), III -> Msg=" + _packet.message + ", PLID=" + _packet.carId + ", UCID=" + _packet.connectionId + "\r\n");
        }

    }
}
