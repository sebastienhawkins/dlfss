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
    public enum Packet_Size : byte  //packetSize Must NOT Include: packetSize and PacketType
    {
        PACKET_SIZE_BTN = 252,
        PACKET_SIZE_BFN = 8,
        PACKET_SIZE_MTC = 72,
        PACKET_SIZE_MST = 68,
        PACKET_SIZE_ISI = 44,
        PACKET_SIZE_REO = 36,
        PACKET_SIZE_TINY = 4
    }
    public enum Packet_Type : byte
    {
        PACKET_NONE,		                        //  0					: not used
        PACKET_ISI_INSIM_INITIALISE,		        //  1 - instruction		: insim initialise
        PACKET_VER_VERSION_SERVER,		            //  2 - info			: version info
        PACKET_TINY_MULTI_PURPOSE,		            //  3 - both ways		: multi purpose
        PACKET_SMALL_MULTI_PURPOSE,		            //  4 - both ways		: multi purpose
        PACKET_STA_DRIVER_RACE_STATE_CHANGE,	    //  5 - info			: state info
        PACKET_SCH_SEND_SINGLE_CHARACTER,		    //  6 - instruction		: single character
        PACKET_SFP_SEND_STATE_FLAG,		            //  7 - instruction		: state flags pack
        PACKET_SCC_SEND_LICENCE_CAMERA_FLAG,	    //  8 - instruction		: set car camera
        PACKET_CPP_CAMERA_POSITION,		            //  9 - both ways		: cam pos pack
        PACKET_ISM_START_MULTIPLAYER,		        // 10 - info			: start multiplayer
        PACKET_MSO_CHAT_RECEIVED,		            // 11 - info			: message out
        PACKET_III_HIDDEN_MESSAGE_I,		        // 12 - info			: hidden /i message
        PACKET_MST_SEND_NORMAL_CHAT,		        // 13 - instruction		: type message or /command
        PACKET_MTC_CHAT_TO_LICENCE,		            // 14 - instruction		: message to a connection
        PACKET_MOD_SEND_SCREEN_MODE,		        // 15 - instruction		: set screen mode
        PACKET_VTN_VOTE_NOTIFICATION,		        // 16 - info			: vote notification
        PACKET_RST_RACE_START,		                // 17 - info			: race start
        PACKET_NCN_NEW_CONNECTION,		            // 18 - info			: new connection
        PACKET_CNL_PART_CONNECTION,		            // 19 - info			: connection left
        PACKET_CPR_LICENCE_DRIVER_RENAME,		    // 20 - info			: connection renamed
        PACKET_NPL_DRIVER_JOIN_RACE,		        // 21 - info			: new player (joined race)
        PACKET_PLP_DRIVER_LEAVE_PIT,		        // 22 - info			: player pit (keeps slot in race)
        PACKET_PLL_DRIVER_LEAVE_RACE,		        // 23 - info			: player leave (spectate - loses slot)
        PACKET_LAP_DRIVER_LAP_TIME,		            // 24 - info			: lap time
        PACKET_SPX_DRIVER_SPLIT_TIME,		        // 25 - info			: split x time
        PACKET_PIT_DRIVER_PITSTOP_START,		    // 26 - info			: pit stop start
        PACKET_PSF_DRIVER_PITSTOP_FINISH,		    // 27 - info			: pit stop finish
        PACKET_PLA_DRIVER_PIT_LANE_STATUS,		    // 28 - info			: pit lane enter / leave
        PACKET_CCH_LICENCE_CAMERA_CHANGE,		    // 29 - info			: camera changed
        PACKET_PEN_DRIVER_PENALITY_STATUS,		    // 30 - info			: penalty given or cleared
        PACKET_TOC_LICENCE_TAKE_OVER_CAR,		    // 31 - info			: take over car
        PACKET_FLG_DRIVER_BLUE_YELLOW_FLAG,	        // 32 - info			: flag (yellow or blue)
        PACKET_PFL_DRIVER_FLAG,		                // 33 - info			: player flags (help flags)
        PACKET_FIN_DRIVER_FINISH_RACE,		        // 34 - info			: finished race
        PACKET_RES_RESULT_CONFIRMED, // 35 - info			: result confirmed
        PACKET_REO_RACE_GRID_ORDER,		            // 36 - both ways		: reorder (info or instruction)
        PACKET_NLP_DRIVER_NODE_LAP,		            // 37 - info			: node and lap packet
        PACKET_MCI_MULTICAR_INFORMATION,		    // 38 - info			: multi car info
        PACKET_ISP_MSX,		                        // 39 - instruction		: type message
        PACKET_MSL_MESSAGE_TO_CONSOLE,		        // 40 - instruction		: message to local computer
        PACKET_CRS_DRIVER_RESET_CAR,		        // 41 - info			: car reset
        PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE,		    // 42 - both ways		: delete buttons / receive button requests
        PACKET_AXI_AUTOCROSS_LAYOUT,		        // 43 - info			: autocross layout information
        PACKET_AXO_DRIVER_HIT_AUTOCROSS_OBJECT,	    // 44 - info			: hit an autocross object
        PACKET_BTN_BUTTON_DISPLAY,		            // 45 - instruction		: show a button on local or remote screen
        PACKET_BTC_BUTTON_CLICK,		            // 46 - info			: sent when a user clicks a button
        PACKET_BTT_BUTTON_TYPE_IN_TEXT_OK		    // 47 - info			: sent after typing into a button
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
        TINY_NPL,		                        // 14 - info			: get all players
        TINY_RES,		                        // 15 - info			: get all results
        TINY_NLP,		                        // 16 - info request	: send an IS_NLP
        TINY_MCI,		                        // 17 - info request	: send an IS_MCI
        TINY_REO,		                        // 18 - info request	: send an IS_REO
        TINY_RST,		                        // 19 - info request	: send an IS_RST
        TINY_AXI,		                        // 20 - info request	: send an IS_AXI
        TINY_AXC_AUTOX_CLEARED,                 // 21 - info			: autocross cleared
    }
    public enum Small_Type : byte
    {
        SMALL_NONE,		                        //  0					: not used
        SMALL_SSP_ADD_SEND_QUEUD_POSITION,	    //  1 - instruction		: start sending positions
        SMALL_SSG_ADD_SEND_QUEUD_GAUGE,		    //  2 - instruction		: start sending gauges
        SMALL_VTA_VOTE_ACTION,		            //  3 - report			: vote action
        SMALL_TMS_TIME_STOP,		            //  4 - instruction		: time stop
        SMALL_STP_TIME_STEP,		            //  5 - instruction		: time step
        SMALL_RTP_RACE_TIME,		            //  6 - info			: race time packet (reply to GTH)
        SMALL_NLI_NODE_LAP_INTERVALE,		    //  7 - instruction		: set node lap interval
    }
}
