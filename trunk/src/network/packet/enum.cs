namespace Drive_LFSS.Packet_
{
    public enum Packet_Size : byte  //packetSize Must NOT Include: packetSize and PacketType
    {
        ISI_PACKET_SIZE = 42,
        TINY_PACKET_SIZE = 2
    }
    public enum Packet_Type : byte
    {
        NONE_NOT_USED,		                //  0					: not used
        ISI_INSIM_INITIALISE,		        //  1 - instruction		: insim initialise
        VER_SERVER_VERSION,		            //  2 - info			: version info
        TINY_MULTI_PURPOSE,		            //  3 - both ways		: multi purpose
        SMALL_MULTI_PURPOSE,		        //  4 - both ways		: multi purpose
        STA_DRIVER_RACE_STATE_CHANGE,	    //  5 - info			: state info
        SCH_SEND_SINGLE_CHARACTER,		    //  6 - instruction		: single character
        SFP_SEND_STATE_FLAG,		        //  7 - instruction		: state flags pack
        SCC_SEND_LICENCE_CAMERA_FLAG,	    //  8 - instruction		: set car camera
        CPP_CAMERA_POSITION,		        //  9 - both ways		: cam pos pack
        ISM_START_MULTIPLAYER,		        // 10 - info			: start multiplayer
        MSO_CHAT_RECEIVED,		            // 11 - info			: message out
        III_HIDDEN_MESSAGE_I,		        // 12 - info			: hidden /i message
        MST_SEND_NORMAL_CHAT,		        // 13 - instruction		: type message or /command
        MTC_CHAT_TO_LICENCE,		        // 14 - instruction		: message to a connection
        MOD_SEND_SCREEN_MODE,		        // 15 - instruction		: set screen mode
        VTN_VOTE_NOTIFICATION,		        // 16 - info			: vote notification
        RST_RACE_START,		                // 17 - info			: race start
        NCN_NEW_CONNECTION,		            // 18 - info			: new connection
        CNL_PART_CONNECTION,		        // 19 - info			: connection left
        CPR_LICENCE_DRIVER_RENAME,		    // 20 - info			: connection renamed
        NPL_DRIVER_JOIN_RACE,		        // 21 - info			: new player (joined race)
        PLP_DRIVER_LEAVE_PIT,		        // 22 - info			: player pit (keeps slot in race)
        PLL_DRIVER_LEAVE_RACE,		        // 23 - info			: player leave (spectate - loses slot)
        LAP_DRIVER_LAP_TIME,		        // 24 - info			: lap time
        SPX_DRIVER_SPLIT_TIME,		        // 25 - info			: split x time
        PIT_DRIVER_PITSTOP_START,		    // 26 - info			: pit stop start
        PSF_DRIVER_PITSTOP_FINISH,		    // 27 - info			: pit stop finish
        PLA_DRIVER_PIT_LANE_STATUS,		    // 28 - info			: pit lane enter / leave
        CCH_LICENCE_CAMERA_CHANGE,		    // 29 - info			: camera changed
        PEN_DRIVER_PENALITY_STATUS,		    // 30 - info			: penalty given or cleared
        TOC_LICENCE_TAKE_OVER_CAR,		    // 31 - info			: take over car
        FLG_DRIVER_BLUE_YELLOW_FLAG,	    // 32 - info			: flag (yellow or blue)
        PFL_DRIVER_FLAG,		            // 33 - info			: player flags (help flags)
        FIN_DRIVER_FINISH_RACE,		        // 34 - info			: finished race
        RES_RESULT_CONFIRMED_RESULT_OF_WHAT,// 35 - info			: result confirmed
        REO_RACE_GRID_ORDER,		        // 36 - both ways		: reorder (info or instruction)
        NLP_DRIVER_NODE_LAP,		        // 37 - info			: node and lap packet
        MCI_MULTICAR_INFORMATION,		    // 38 - info			: multi car info
        ISP_MSX,		                    // 39 - instruction		: type message
        MSL_MESSAGE_TO_CONSOLE,		        // 40 - instruction		: message to local computer
        CRS_DRIVER_RESET_CAR,		        // 41 - info			: car reset
        BFN_ASK_REMOVE_ADD_BUTTON,		    // 42 - both ways		: delete buttons / receive button requests
        AXI_AUTOCROSS_LAYOUT,		        // 43 - info			: autocross layout information
        AXO_DRIVER_HIT_AUTOCROSS_OBJECT,	// 44 - info			: hit an autocross object
        BTN_BUTTON_DISPLAY,		            // 45 - instruction		: show a button on local or remote screen
        BTC_BUTTON_CLICK,		            // 46 - info			: sent when a user clicks a button
        BTT_BUTTON_TYPE_IN_TEXT_OK		    // 47 - info			: sent after typing into a button
    }
    public enum Tiny_Type : byte
    {
        TINY_NONE,		//  0					: see "maintaining the connection"
        TINY_VER,		//  1 - info request	: get version
        TINY_CLOSE,		//  2 - instruction		: close insim
        TINY_PING,		//  3 - ping request	: external progam requesting a reply
        TINY_REPLY,		//  4 - ping reply		: reply to a ping request
        TINY_VTC,		//  5 - info			: vote cancelled
        TINY_SCP,		//  6 - info request	: send camera pos
        TINY_SST,		//  7 - info request	: send state info
        TINY_GTH,		//  8 - info request	: get time in hundredths (i.e. SMALL_RTP)
        TINY_MPE,		//  9 - info			: multi player end
        TINY_ISM,		// 10 - info request	: get multiplayer info (i.e. ISP_ISM)
        TINY_REN,		// 11 - info			: race end (return to game setup screen)
        TINY_CLR,		// 12 - info			: all players cleared from race
        TINY_NCN_NEW_LICENCE_CONNECTION,		// 13 - info			: get all connections
        TINY_NPL,		// 14 - info			: get all players
        TINY_RES,		// 15 - info			: get all results
        TINY_NLP,		// 16 - info request	: send an IS_NLP
        TINY_MCI,		// 17 - info request	: send an IS_MCI
        TINY_REO,		// 18 - info request	: send an IS_REO
        TINY_RST,		// 19 - info request	: send an IS_RST
        TINY_AXI,		// 20 - info request	: send an IS_AXI
        TINY_AXC,		// 21 - info			: autocross cleared
    }
    public enum Small_Type : byte
    {
        SMALL_NONE,		//  0					: not used
        SMALL_SSP,		//  1 - instruction		: start AddToSendingQueud positions
        SMALL_SSG,		//  2 - instruction		: start AddToSendingQueud gauges
        SMALL_VTA,		//  3 - report			: vote action
        SMALL_TMS,		//  4 - instruction		: time stop
        SMALL_STP,		//  5 - instruction		: time step
        SMALL_RTP,		//  6 - info			: race time packet (reply to GTH)
        SMALL_NLI,		//  7 - instruction		: set node lap interval
    }
}