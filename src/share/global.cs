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

using System;
using System.Runtime.InteropServices;
namespace Drive_LFSS.Definition_
{ 
    public enum Config_User : byte
    {
        ACCELERATION_START = 0,
        ACCELERATION_STOP = 1,
        ACCELERATION_ON = 2,
        DRIFT_SCORE_ON = 3,
        TIMEDIFF_LAP = 4,
        TIMEDIFF_SPLIT = 5,
        MAX_SPEED_ON = 7,
        END = 8,
    }
    public enum Warning_Driving_Type : byte
    {
        NONE = 0,
        VISITOR = 1,
        VICTIM = 2,
        YELLOW_FLAG = 3,
        BAD_DRIVING = 4,
        
    }
    public enum Rank_Change_Mask : uint
    {
		NONE = 0,
		PB_LOW = 1,
		PB_HIGH = 2,
		AVG_LOW = 4,
		AVG_HIGH = 8,
		STA_LOW = 16,
		STA_HIGH = 32,
		WIN_LOW = 64,
		WIN_HIGH = 128,
		TOTAL_LOW = 256,
		TOTAL_HIGH = 512,
		POSITION_LOW = 1024,
		POSITION_HIGH = 2048
    }
    public enum Race_Template_Flag : ushort
    {
        NONE = 0,
        MID_RACE_JOIN = 1,
        MUST_PIT = 2,
        CAN_RESET = 4,
        FORCE_COCKPIT_VIEW = 8,
        ALLOW_WRONG_WAY = 16,
        CAN_VOTE = 32,
        CAN_SELECT_TRACK = 64,
        AUTO_KICK_KICK = 128,
        AUTO_KICK_BAN = 256,
        AUTO_KICK_SPEC = 512,
    }
    public enum Racing_Flag : byte
    {
        RACE_BLUE_FLAG = 1,
        RACE_YELLOW_FLAG = 2
    }
    public enum Race_Feature_Flag : ushort
    {
        RACE_FLAG_NONE = 0,
        RACE_FLAG_CAN_VOTE = 1,
        RACE_FLAG_CAN_SELECT_TRACK = 2,
        RACE_FLAG_UNK_1 = 4,
        RACE_FLAG_UNK_2 = 8,
        RACE_FLAG_UNK_3 = 16,
        RACE_FLAG_MID_RACE_JOIN = 32,
        RACE_FLAG_MUST_PIT = 64,
        RACE_FLAG_CAN_RESET = 128,
        RACE_FLAG_FORCE_COKPIT_VIEW = 256
    }
    
    public enum Button_Entry : ushort
    {
        NONE = 0,
        BANNER = 1,
        MESSAGE_BAR_TOP = 6,
        MESSAGE_BAR_MIDDLE = 7,
        COLLISION_WARNING = 8,
        VOTE_TITLE = 9,
        VOTE_OPTION_1 = 10,
        VOTE_OPTION_2 = 11,
        VOTE_OPTION_3 = 12,
        VOTE_OPTION_4 = 13,
        VOTE_OPTION_5 = 14,
        VOTE_OPTION_6 = 15,
        MOTD_BACKGROUND = 2,
        MOTD_BUTTON_DRIVE = 5,
        MOTD_BUTTON_HELP = 36,
        MOTD_BUTTON_CONFIG = 30,
        TRACK_PREFIX = 16,
        INFO_1 = 17,
        INFO_2 = 18,
        INFO_3 = 19,
        INFO_4 = 20,
        INFO_5 = 21,
        CONFIG_USER_BG = 22,
        CONFIG_USER_CLOSE = 23,
        CONFIG_USER_ACC_ON = 26,
        CONFIG_USER_ACC_START = 27,
        CONFIG_USER_ACC_END = 28,
        CONFIG_USER_ACC_CURRENT = 29,
        CONFIG_USER_DRIFT_ON = 38,
        CONFIG_USER_TIMEDIFF_ALL= 41,
        CONFIG_USER_TIMEDIFF_LAP = 42,
        CONFIG_USER_TIMEDIFF_SPLIT = 43,
        HELP_BG = 31,
        HELP_BUTTON_DRIVE = 34,
        HELP_BUTTON_CONFIG = 35,
        TEXT_BUTTON_DRIVE = 45,
        RANK_BG = 47,
        RANK_NAME = 50,
        RANK_PB = 51,
        RANK_AVERAGE = 52,
        RANK_STABILITY = 53,
        RANK_WIN = 54,
        RANK_TOTAL = 55,
        RANK_POSITION = 56,
        RANK_LAST_UPDATE = 57,
        RANK_NEXT_UPDATE = 58,
        RANK_BUTTON_TOP20 = 59,
        RANK_BUTTON_SEARCH = 60,
        RANK_BUTTON_CURRENT = 61,
        RANK_BUTTON_CLOSE = 62,
        RANK_INFO = 64,
        RANK_SEARCH_BUTTON_TRACK = 65,
        RANK_SEARCH_BUTTON_CAR = 66,
        RANK_SEARCH_BUTTON_LICENCE = 67,
        CANCEL_WARNING_DRIVING_1 = 68,
        CANCEL_WARNING_DRIVING_2 = 69,
        CANCEL_WARNING_DRIVING_3 = 70,
        RESULT_CLOSE_BUTTON =  71,
        RESULT_NAME_DISPLAY = 74,
        RESULT_SCORE_DISPLAY = 75, 
        CONFIG_USER_MAX_SPEED_ON = 77,
        RESULT_BG = 78,
    }
    public enum Button_Styles_Flag : byte
    {
        ISB_C1 = 1,         // you can choose a standard
        ISB_C2 = 2,         // interface colour using
        ISB_C4 = 4,         // these 3 lowest bits - see below
        ISB_CLICK = 8,
        ISB_LIGHT = 16,
        ISB_DARK = 32,
        ISB_LEFT = 64,      // align text to left
        ISB_RIGHT = 128,    // align text to right
    }
    public enum Gui_Entry : ushort
    {
        NONE = 0,
        MOTD = 1,
        CONFIG_USER = 2,
        HELP = 3,
        TEXT = 4,
        RANK = 5,
        RESULT = 6,
        GREEN_FLAG = 7,
        PIT_CLOSE_FLAG = 8,
        YELLOW_FLAG_LOCAL = 9,
        YELLOW_FLAG_GLOBAL = 10,
        REG_FLAG_STOP_RACE = 11,
        BLACK_FLAG_PENALITY = 12,
        BLUE_FLAG_SLOW_CAR = 13,
        WHITE_FLAG_FINAL_LAP = 14,
        BLACK_FLAG_CAR_PROBLEM = 15,
        BLACK_FLAG_NO_SCORE = 16,
        FLAG_RACE_END = 17,
        
    }
    
    public enum Button_Safe_Coord_Range
    {
        BUTTON_COORD_X_MIN = 0,
        BUTTON_COORD_X_MAX = 110,
        BUTTON_COORD_Y_MIN = 30,
        BUTTON_COORD_Y_MAX = 170,
    }
    public enum Button_Function : byte
    {
        BUTTON_FUNCTION_DEL = 0,
        BUTTON_FUNCTION_CLEAR = 1,
        BUTTON_FUNCTION_USER_CLEAR = 2,
        BUTTON_FUNCTION_REQUEST = 3,
    }
    public enum Ctrl_Types : ushort
    {
        CTRL_C_EVENT = 0,
        CTRL_BREAK_EVENT,
        CTRL_CLOSE_EVENT,
        CTRL_LOGOFF_EVENT = 5,
        CTRL_SHUTDOWN_EVENT
    }
    public enum Bot_GUI : uint
    {
        FIRST = 4294967167,
        LAST = 0xFFFFFFFF
    }
    public enum Protocol_Id
    {
        PROTO_UDP,
        PROTO_TCP
    }
    public enum Licence_Camera_Mode : byte
    {
        CAMERA_MODE_FOLLOW = 0,
        CAMERA_MODE_HELI = 1,
        CAMERA_MODE_CAM = 2,
        CAMERA_MODE_DRIVER = 3,
        CAMERA_MODE_CUSTOM = 4,
        CAMERA_MODE_ANOTHER = 255
    }
    public enum Message_Sound : byte
    {
        SOUND_CONSOLE_SILENT = 0,
        SOUND_CONSOLE_MESSAGE = 1,
        SOUND_CONSOLE_SYSMESSAGE = 2,
        SOUND_CONSOLE_INVALIDKEY = 3,
        SOUND_CONSOLE_ERROR = 4,
        SOUND_CONSOLE_MAX = 5,
    }
    public enum Chat_Type : byte
    {
        SYSTEM = 0,
        USER = 1,
        CHAT_USER_TYPE_PREFIX = 2,
        CHAT_USER_TYPE_O = 3,
        CHAT_USER_TYPE_MAX = 4,
    }
    
    public enum Tyre_Position : int
    {
        REAR_LEFT = 0,
        REAR_RIGHT = 1,
        FRONT_LEFT = 2,
        FRONT_RIGHT = 3,
        MAX = 4,
    }

    public enum Tyre_Compound : byte
    {
        R1 = 0,
        R2 = 1,
        R3 = 2,
        R4 = 3,
        ROAD_SUPER = 4,
        ROAD_NORMAL = 5,
        HYBRID = 6,
        KNOBBLY = 7,
        MAX = 8,
        CAR_TYRE_NOTCHANGED = 255
    }
    public enum Penalty_Type : byte
    {
        PENALTY_TYPE_NONE = 0,
        PENALTY_TYPE_DRIVE_THROUGH = 1,
        PENALTY_TYPE_DRIVE_THROUGHT_VALID = 2,
        PENALTY_TYPE_PIT_STOP = 3,
        PENALTY_TYPE_PIT_STOP_VALID = 4,
        PENALTY_TYPE_ADD_30_SEC = 5,
        PENALTY_TYPE_ADD_45_SEC = 6,
        PENALTY_TYPE_MAX = 7
    }
    public enum Penalty_Type_Ext : byte
    {
        NONE = 0,
        DRIVE_THROUGH = 1,
        PIT_STOP = 2,
        ADD_30_SEC = 3,
        ADD_45_SEC = 4,
        SPEC = 5,
        KICK = 6,
        MAX = 7
    }
    public enum Penalty_Reason : byte
    {
        PENALTY_REASON_UNK = 0,
        PENALTY_REASON_ADMIN = 1,
        PENALTY_REASON_WRONG_WAY = 2,
        PENALTY_REASON_FALSE_START = 3,
        PENALTY_REASON_PIT_SPEEDING = 4,
        PENALTY_REASON_STOP_LATE = 6,
        PENALTY_REASON_STOP_SHORT = 5,
        PENALTY_REASON_MAX = 7,
    }
    public enum Pit_Lane_State : byte
    {
        PITLANE_STATE_EXIT = 0,
        PITLANE_STATE_ENTER = 1,
        PITLANE_STATE_NO_PURPOSE = 2,
        PITLANE_STATE_DRIVE_THOUGHT_PENALITY = 3,
        PITLANE_STATE_PITSTOP_PENALITY = 4,
        PITLANE_STATE_NUM = 5
    }

    public enum Vote_Action : byte
    {
        VOTE_NONE = 0,
        VOTE_END = 1,
        VOTE_RESTART = 2,
        VOTE_QUALIFY = 3,
        VOTE_CUSTOM_1 = 4,
        VOTE_CUSTOM_2 = 5,
        VOTE_CUSTOM_3 = 6,
        VOTE_CUSTOM_4 = 7,
        VOTE_CUSTOM_5 = 8,
        VOTE_CUSTOM_6 = 9,
    }
    public enum Leave_Reason : byte
    {
        LEAVE_REASON_DISCONNECTED = 0,
        LEAVE_REASON_TIMEOUT = 1,
        LEAVE_REASON_LOSTCONNECTION = 2,
        LEAVE_REASON_KICKED = 3,
        LEAVE_REASON_BANNED = 4,
        LEAVE_REASON_SECURITY = 5,
        LEAVE_REASON_MAX = 6,
    }
    public enum Race_In_Progress_Status : byte
    {
        RACE_PROGRESS_NONE,
        RACE_PROGRESS_RACING,
        RACE_PROGRESS_QUALIFY
    }
    public enum Weather_Status : byte
    {
        WEATHER_CLEAR_DAY,      //Don't know
        WEATHER_SUN_SET,        //Don't know
        WEATHER_SUN_SLEEP       //Don't know
    }
    public enum Wind_Status : byte
    {
        WIND_NONE,
        WIND_WEAK,
        WIND_HIGH
    }
    public enum Grid_Start_Beviator : byte
    {
        GRID_START_BEVIATOR_NONE = 0,
    }
    public enum Car_Multiple_Flag : uint
    {
        CAR_TRACTION_NONE = 0,
        CAR_TRACTION_FRONT = 1,
        CAR_TRACTION_REAR = 2,
        CAR_TRACTION_4x4 = 1+2,
    }
    public enum Confirm_Flag : byte
    {
        CONFIRM_NONE = 0,
        CONFIRM_MENTIONED = 1,
        CONFIRM_CONFIRMED = 2,
        CONFIRM_PENALTY_DT = 4,
        CONFIRM_PENALTY_SG = 8,
        CONFIRM_PENALTY_30 = 16,
        CONFIRM_PENALTY_45 = 32,
        CONFIRM_DID_NOT_PIT = 64,
        CONFIRM_DISQ = 4+8+64,
        CONFIRM_TIME = 16+32
    }
    public enum Driver_Type_Flag : byte
    {
        DRIVER_TYPE_NONE = 0,
        DRIVER_TYPE_FEMALE = 1,
        DRIVER_TYPE_AI = 2,
        DRIVER_TYPE_REMOTE = 4
    }
    public enum Button_Click_Flag : byte
    {
        ISB_CTRL = 4,
        ISB_LMB = 1,
        ISB_RMB = 2,
        ISB_SHIFT = 8
    }
    public enum Car_Racing_Flag : byte
    {
        CAR_RACING_FLAG_NONE = 0,
        CAR_RACING_FLAG_BLUE = 1,
        CAR_RACING_FLAG_YELLOW = 2,
        CAR_RACING_FLAG_LAG = 32,
        CAR_RACING_FLAG_FIRST = 64,
        CAR_RACING_FLAG_LAST = 128,
    }
    public enum InSim_Flag : byte
    {
        INSIM_FLAG_SPARE_0 = 1,
        INSIM_FLAG_SPARE_1 = 2,
        INSIM_FLAG_LOCAL = 4,
        INSIM_FLAG_KEEP_MESSAGE_COLOR = 8,
        INSIM_FLAG_RECEIVE_NLP = 16,
        INSIM_FLAG_RECEIVE_MCI = 32
    }
    public enum Pit_Work_Flag : uint
    {
        NOTHING = 1,
        STOP = 2,
        FRONT_DMG = 4,
        FRONT_WHEEL = 8,
        LEFT_FRONT_DMG = 16,
        LEFT_FRONT_WHEEL = 32,
        RIGHT_FRONT_DMG = 64,
        RIGHT_FRONT_WHEEL = 128,
        REAR_DMG = 256,
        REAR_WHEEL = 512,
        LEFT_REAR_DMG = 1024,
        LEFT_REAR_WHEEL = 2048,
        RIGHT_REAR_DMG = 4096,
        RIGHT_REAR_WHEEL = 8192,
        BODY_MINOR = 16384,
        BODY_MAJOR = 32768,
        SETUP = 65536,
        REFUEL = 131072,
        MAX = 262144,
    }
    public enum Driver_Flag : ushort
    {
        NONE = 0,
        SWAPSIDE = 1,
        RESERVER_2 = 2,
        RESERVED_4 = 4,
        AUTOGEARS = 8,
        SHIFTER = 16,
        RESERVED_32 = 32,
        HELP_BRAKE = 64,
        AXIS_CLUTCH = 128,
        IS_IN_PITS = 256,
        AUTOCLUTCH = 512,
        MOUSE = 1024,
        KEYBOARD_NO_HELP = 2048,
        KEYBOARD_STABILISED = 4096,
        CUSTOM_VIEW = 8192
    }
    public enum Passenger_Flag : byte
    {
        NONE = 0,
        FRONT_FEMALE = 1,
        FRONT = 2,
        LEFT_REAR_FEMALE = 4,
        LEFT_REAR = 8,
        RIGHT_REAR_FEMALE = 16,
        RIGHT_REAR = 32,
    }
    public enum Single_Char_Flag : byte
    {
        KEY_CONTROL = 2,
        KEY_NONE = 0,
        KEY_SHIFT = 1
    }
    public enum Licence_View_Flag : ushort
    {
        ISS_FRONT_END = 0x100,
        ISS_GAME = 1,
        ISS_MPSPEEDUP = 0x400,
        ISS_MULTI = 0x200,
        ISS_PAUSED = 4,
        ISS_REPLAY = 2,
        ISS_SHIFTU = 8,
        ISS_SHIFTU_FOLLOW = 0x20,
        ISS_SHIFTU_HIGH = 0x10,
        ISS_SHIFTU_NO_OPT = 0x40,
        ISS_SHOW_2D = 0x80,
        ISS_SOUND_MUTE = 0x1000,
        ISS_VIEW_OVERRIDE = 0x2000,
        ISS_VISIBLE = 0x4000,
        ISS_WINDOWED = 0x800
    }
    public enum Out_Gauge_Flag : ushort
    {
        OUTGAUGE_1 = 0x400,
        OUTGAUGE_2 = 0x800,
        OUTGAUGE_3 = 0x1000,
        OUTGAUGE_4 = 0x2000,
        OUTGAUGE_BAR = 0x8000,
        OUTGAUGE_FULLBEAM = 2,
        OUTGAUGE_HANDBRAKE = 4,
        OUTGAUGE_HEADLIGHTS = 0x20,
        OUTGAUGE_KM = 0x4000,
        OUTGAUGE_OILWARN = 0x200,
        OUTGAUGE_PITSPEED = 8,
        OUTGAUGE_REDLINE = 0x100,
        OUTGAUGE_SHIFTLIGHT = 1,
        OUTGAUGE_SIGNAL_L = 0x40,
        OG_SIGNAL_R = 0x80,
        OUTGAUGE_TC = 0x10
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct CarInformation
    {
        /*CarInformation(byte[] _data)
        {
            trackNode = 0; lapNumber = 0; carId = 0; position = 0; carFlag = 0; Sp3 = 0;
            posX = 0; posY = 0; posZ = 0; speed = 0; direction = 0; heading = 0; angleVelocity = 0;

            IntPtr pStruct = Marshal.AllocHGlobal(Marshal.SizeOf(this));
            GCHandle hStruct = GCHandle.Alloc(pStruct, GCHandleType.Pinned);
            Marshal.Copy(_data, 0, pStruct, _data.Length);
            this = (CarInformation)Marshal.PtrToStructure(pStruct, this.GetType());
            hStruct.Free();
            Marshal.FreeHGlobal(pStruct);
        }*/
        public ushort nodeTrack;
        public ushort lapNumber;
        public byte carId;
        public byte position;
        public Car_Racing_Flag carFlag;
        internal byte Sp3;
        public int posX;
        public int posY;
        public int posZ;
        public ushort speed;
        public ushort direction;
        public ushort heading;
        public short angleVelocity;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct NodeLap
    {
        public ushort trackNode;
        public ushort lap;
        public byte carId;
        public byte position;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorF
    {
        public float x;
        public float y;
        public float z;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct VectorI
    {
        public int x;
        public int y;
        public int z;
    }
}