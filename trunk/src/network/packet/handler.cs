namespace Drive_LFSS.Packet_
{
    using Drive_LFSS.PacketStore_;
    using Drive_LFSS.Server_;

    public abstract class PacketHandler : Store
    {
        public void ProcessPacket(Packet_Type _packetType, object _packet)
        {
            switch (_packetType)
            {
                case Packet_Type.MCI_MULTICAR_INFORMATION:      processPacket((PacketMCI)_packet);break;
                case Packet_Type.MSO_CHAT_RECEIVED:             processPacket((PacketMSO)_packet);break;
                case Packet_Type.TINY_MULTI_PURPOSE:            processPacket((PacketTiny)_packet);break;
                case Packet_Type.STA_DRIVER_RACE_STATE_CHANGE:  processPacket((PacketSTA)_packet);break;
                case Packet_Type.NCN_NEW_CONNECTION:            processPacket((PacketNCN)_packet);break;
                case Packet_Type.CNL_PART_CONNECTION:           processPacket((PacketCNL)_packet);break;
                case Packet_Type.NPL_DRIVER_JOIN_RACE:          processPacket((PacketNPL)_packet);break;
                case Packet_Type.PLL_DRIVER_LEAVE_RACE:         processPacket((PacketPLL)_packet);break;
                case Packet_Type.RST_RACE_START:                processPacket((PacketRST)_packet);break;

                case Packet_Type.VER_SERVER_VERSION:            processPacket((PacketVER)_packet);break;
                default: ((Server)this).log.missingDefinition("ProcessPacket(), No Existing PacketHandler for packetType->" + _packetType + "\r\n"); break;
            }
        }

        protected virtual void processPacket(PacketVER _packet)
        {
            ((Server)this).log.debug("Process packet ->" + _packet + "\r\n");
            ((Server)this).log.normal("InSim v" + _packet.inSimVersion + ", Licence: " + _packet.productVersion + "." + _packet.serverVersion + "\r\n");
        }
        protected virtual void processPacket(PacketTiny _packet)
        {
            ((Server)this).log.debug("Process packet TINY\r\n");
        }
        protected virtual void processPacket(PacketMSO _packet)
        {
            ((Server)this).log.debug("Process packet MSO\r\n");
        }
        protected virtual void processPacket(PacketNCN _packet)
        {
            ((Server)this).log.debug("LicenceJoinsHost(), NCN -> UCID= " + _packet.tempLicenceId + ", UName=" + _packet.licenceName + ", PName=" + _packet.driverName + ", Flags=" + _packet.driverTypeMask + ", Admin=" + _packet.adminStatus + "\r\n");
        }
        protected virtual void processPacket(PacketCNL _packet)
        {
            ((Server)this).log.debug("CNL_ClientLeavesHost, CNL -> UCID= " + _packet.tempLicenceId + ", Reason=" + _packet.Reason + ", Total=" + _packet.Total + "\r\n");
        }
        protected virtual void processPacket(PacketCPR _packet)
        {
            ((Server)this).log.debug("CPR_ClientRenames(), CPR->UCID=" + _packet.UCID + ", PName=" + _packet.PName + ", Plate=" + _packet.Plate + ", ReqI=" + _packet.ReqI + "\n\r");
        }
        protected virtual void processPacket(PacketTOC _packet)
        {
            ((Server)this).log.debug("TOC_PlayerCarTakeOver(), TOC -> PLID=" + _packet.carId + ", OldUCID=" + _packet.oldTLID + ", NewUCID=" + _packet.newTLID + "\r\n");
        }
        protected virtual void processPacket(PacketPLL _packet)
        {
            ((Server)this).log.debug("PacketPLL, NPL -> PLID=" + _packet.carId + "\r\n");
        }
        // Player help settings changed.
        protected virtual void processPacket(PacketPFL _packet)
        {
            ((Server)this).log.debug("PFL_PlayerFlagsChanged(), PFL -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", Spare=" + _packet.Spare + "\r\n");
        }
        // A player goes to the garage (setup screen).
        protected virtual void processPacket(PacketPLP _packet)
        {
            ((Server)this).log.debug("PLP_PlayerGoesToGarage(), PLP -> PLID=" + _packet.carId + "\r\n");
        }
        // A player joins the race. If PLID already exists, then player leaves pit.
        protected virtual void processPacket(PacketNPL _packet)
        {
            ((Server)this).log.debug("NPL_PlayerJoinsRace(), NPL -> CName=" + _packet.licenceName + ", PName=" + _packet.driverName + ", PLID=" + _packet.carId + ", UCID=" + _packet.tempLicenceId + ", PType=" + _packet.driverTypeMask + "\r\n");
        }
        protected virtual void processPacket(PacketLAP _packet)
        {
            ((Server)this).log.debug("LAP_PlayerCompletesLap(), LAP -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", LTIME=" + _packet.LTime + " ms" + ", LapsDone=" + _packet.LapsDone + ", ETime=" + _packet.ETime + " ms" + " \r\n");
        }
        protected virtual void processPacket(PacketFIN _packet)
        {
            ((Server)this).log.debug("FIN_PlayerFinishedRaces(), FIN -> PLID=" + _packet.PLID + ", Confirm=" + _packet.Confirm + ", Flags=" + _packet.Flags + ", BTIME=" + _packet.BTime + " ms" + ", TTIME=" + _packet.TTime + " ms" + ", LapsDone=" + _packet.LapsDone + ", NumStops=" + _packet.NumStops + ",  \r\n");
        }
        protected virtual void processPacket(PacketCRS _packet)
        {
            ((Server)this).log.debug("CRS_PlayerResetsCar(), CRS -> PLID=" + _packet.PLID + "\r\n");
        }
        protected virtual void processPacket(PacketPIT _packet)
        {
            ((Server)this).log.debug("PIT_PlayerStopsAtPit(), PIT -> PLID=" + _packet.PLID + ", Flags=" + _packet.Flags + ", LapsDone=" + _packet.LapsDone + ", NumStops=" + _packet.NumStops + ", Penalty=" + _packet.Penalty + ", FL_Changed=" + _packet.FL_Changed + ", FR_Changed=" + _packet.FR_Changed + ", RL_Changed=" + _packet.RL_Changed + ", RR_Changed=" + _packet.RR_Changed + ", Work=" + _packet.Work + ", Spare=" + _packet.Spare + "\r\n");
        }
        protected virtual void processPacket(PacketPSF _packet)
        {
            ((Server)this).log.debug("PSF_PitStopFinished(), PSF -> PLID=" + _packet.carId + ", Spare=" + _packet.Spare + ", STime=" + _packet.STime + " ms" + "\r\n");
        }
        protected virtual void processPacket(PacketAXO _packet)
        {
            ((Server)this).log.debug("AXO_PlayerHitsAutocrossObject(), AXO -> PLID=" + _packet.PLID + "\r\n");
        }
        // A player pressed KEY_SHIFT+I or KEY_SHIFT+B
        protected virtual void processPacket(PacketBFN _packet)
        {
            ((Server)this).log.debug("BFN_PlayerRequestsButtons(), BFN -> UCID=" + _packet.UCID + ", ClickID=" + _packet.ClickID + ", SubT=" + _packet.SubT + "\r\n");
        }
        protected virtual void processPacket(PacketBTC _packet)
        {
            ((Server)this).log.debug("BTC_ButtonClicked(), BTC -> UCID=" + _packet.UCID + ",ClickID=" + _packet.ClickID + ", CFlags=" + _packet.CFlags + "\r\n");
        }
        protected virtual void processPacket(PacketBTT _packet)
        {
            ((Server)this).log.debug("BTT_TextBoxOkClicked(), BTT -> UCID=" + _packet.UCID + ", ClickID=" + _packet.ClickID + ", Text=" + _packet.Text + ", TypeIn=" + _packet.TypeIn + "\r\n");
        }
        protected virtual void processPacket(PacketPEN _packet)
        {
            ((Server)this).log.debug("PEN_PenaltyChanged(), PEN -> PLID=" + _packet.PLID + ", Reason=" + _packet.Reason + ", NewPen=" + _packet.NewPen + ", OldPen=" + _packet.OldPen + "\r\n");
        }
        // Yellow or blue flag changed
        protected virtual void processPacket(PacketFLG _packet)
        {
            ((Server)this).log.debug("FLG_FlagChanged(), FLG -> PLID=" + _packet.PLID + ", Flag=" + _packet.Flag + ", OffOn=" + _packet.OffOn + ", CarBehind=" + _packet.CarBehind + "\r\n");
        }
        // A player entered or left the pitlane
        protected virtual void processPacket(PacketPLA _packet)
        {
            ((Server)this).log.debug("PLA_PitLaneChanged(), PLA -> PLID=" + _packet.carId + ", Fact=" + _packet.Fact + "\r\n");
        }
        // A player crossed a lap split
        protected virtual void processPacket(PacketSPX _packet)
        {
            ((Server)this).log.debug("SPX_SplitTime(), SPX -> PLID=" + _packet.carId + ", Split=" + _packet.Split + ", STime=" + _packet.STime + " ms" + ", ETime=" + _packet.ETime + " ms" + ",  NumStops=" + _packet.NumStops + ", Penalty=" + _packet.Penalty + "\r\n");
        }
        // A player changed his camera
        protected virtual void processPacket(PacketCCH _packet)
        {
            ((Server)this).log.debug("CCH_CameraChanged(), CCH -> PLID=" + _packet.PLID + ", Camera=" + _packet.Camera + "\r\n");
        }
        // The server/race state changed
        protected virtual void processPacket(PacketSTA _packet)
        {
            ((Server)this).log.debug("STA_StateChanged(), STA -> Flags=" + _packet.viewOptionMask + ", RaceInProg=" + _packet.raceInProgress + ", ViewPLID=" + _packet.currentCarId + ", NumConns=" + _packet.connectionCount + ", NumP=" + _packet.carCount + ", QualMins=" + _packet.qualificationMinute + "\r\n");
        }
        // A host is started or joined
        protected virtual void processPacket(PacketISM _packet)
        {
            ((Server)this).log.debug("ISM_MultiplayerInformation(), ISM -> HName=" + _packet.HName + ", Host=" + _packet.Host + "\r\n");
        }
        // A host ends or leaves

        // Race got cleared with /clear
        protected virtual void processPacket()
        {
            ((Server)this).log.debug("CLR_RaceCleared(), Got a Ping Request, What ToDo With That???\r\n");
        }
        // Sent at the start of every race or qualifying session, listing the start order
        protected virtual void processPacket(PacketREO _packet)
        {
            ((Server)this).log.debug("REO_RaceStartOrder(), REO -> PLID=" + _packet.carId + ", NumP=" + _packet.NumP + "\r\n");
        }
        // Race start information
        protected virtual void processPacket(PacketRST _packet)
        {
            ((Server)this).log.debug("RST_RaceStart(), RST -> Track=" + _packet.trackName + ", Finish=" + _packet.nodeFinishIndex + ", NumP=" + _packet.carCount + ", NumNodes=" + _packet.nodeCount + ", Flags=" + _packet.raceFeatureMask + ", QualMins=" + _packet.qualificationMinute + ", RaceLaps=" + _packet.raceLaps + ", Spare=" + _packet.Spare + ", Weather=" + _packet.weatherStatus + ", Wind=" + _packet.windStatus + ", Split1=" + _packet.nodeSplit1Index + ", Split2=" + _packet.nodeSplit2Index + ", Split3=" + _packet.nodeSplit3Index + "\r\n");
        }
        // Qualify or confirmed finish
        protected virtual void processPacket(PacketRES _packet)
        {
            ((Server)this).log.debug("RES_RaceOrQualifyingResult(), RES -> PLID=" + _packet.carId + ", UName=" + _packet.UName + ", PName=" + _packet.PName + ", PSeconds=" + _packet.PSeconds + ", ResultNum=" + _packet.ResultNum + ", SpA=" + _packet.SpA + ", SpB=" + _packet.SpB + ", TTime=" + _packet.TTime + " ms" + ", BTime=" + _packet.BTime + " ms" + ", Plate=" + _packet.Plate + ", LapsDone=" + _packet.LapsDone + ", NumRes=" + _packet.NumRes + ", NumStops=" + _packet.NumStops + ", Flags=" + _packet.Flags + ", Confirm=" + _packet.Confirm + ", CName=" + _packet.CName + "\r\n");
        }
        // A race ends (return to game setup screen)
        // Current race time progress in hundredths
        protected virtual void processPacket(uint RTP)
        {
            ((Server)this).log.debug("RTP_RaceTime(), RTP -> RTP=" + RTP + "\r\n");
        }
        // Autocross got cleared
        
        // Request - autocross layout information
        protected virtual void processPacket(PacketAXI _packet)
        {
            ((Server)this).log.debug("AXI_AutocrossLayoutInformation(), AXI -> NumCP=" + _packet.NumCP + ", NumO=" + _packet.NumO + ", AXStart=" + _packet.AXStart + ", LName=" + _packet.LName + "\r\n");
        }
        // LFS reporting camera position and state
        protected virtual void processPacket(PacketCPP _packet)
        {
            ((Server)this).log.debug("CPP_CameraPosition(), CPP -> ViewPLID=" + _packet.ViewPLID + ", Flags=" + _packet.Flags + ", X=" + _packet.X + ", Y=" + _packet.Y + ", Z=" + _packet.Z + ", Time=" + _packet.Time + ", R=" + _packet.R + ", P=" + _packet.P + ", InGameCam=" + _packet.InGameCam + ", H=" + _packet.H + ", FOV=" + _packet.FOV + "\r\n");
        }
        // A vote completed
        protected virtual void processPacket(byte _packet)
        {
            ((Server)this).log.debug("VTA_VoteAction(), byte VTA=" + _packet + "\r\n");
        }
        // A vote got called
        protected virtual void processPacket(PacketVTN _packet)
        {
            ((Server)this).log.debug("VTN_VoteNotify(), VTN -> UCID=" + _packet.tempLicenceId + ", Action=" + _packet.voteAction + ", Spare2=" + _packet.Spare2 + ", Spare3=" + _packet.Spare3 + "\r\n");
        }
        // Detailed car information packet (max 8 per packet)
        protected virtual void processPacket(PacketMCI _packet)
        {
            ((Server)this).log.debug("MCI_CarInformation(), MCI -> Info=" + _packet.carInformation + ", NumC=" + _packet.NumC + "\r\n");
        }
        // Compact car information packet
        protected virtual void processPacket(PacketNLP _packet)
        {
            ((Server)this).log.debug("NLP_LapNode(), NLP -> Info=" + _packet.Info + ", NumP=" + _packet.NumP + "\r\n");
        }
        // A /i message got sent to this program
        protected virtual void processPacket(PacketIII _packet)
        {
            ((Server)this).log.debug("III_InSimInfo(), III -> Msg=" + _packet.Msg + ", PLID=" + _packet.PLID + ", UCID=" + _packet.UCID + "\r\n");
        }

    }
}