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
    public struct Msg
    {
         public const string COLOR_DIFF_LOWER = "^3";
         public const string COLOR_DIFF_EVENT = "^8";
         public const string COLOR_DIFF_HIGHER = "^4";
         public const string COLOR_DIFF_TOP = "^7";
    }
    public enum Config_User : byte
    {
        ACCELERATION_START=0,
        ACCELERATION_STOP=1,
        ACCELERATION_ON=2,
        END=3,
    }
    [Flags]public enum Race_Template_Flag : byte
    {
        NONE = 0,
        MID_RACE_JOIN = 1,
        MUST_PIT = 2,
        CAN_RESET = 4,
        FORCE_COCKPIT_VIEW = 8,
        ALLOW_WRONG_WAY = 16
    }
    public enum Racing_Flag : byte
    {
        RACE_BLUE_FLAG = 1,
        RACE_YELLOW_FLAG = 2
    }
    [Flags]public enum Race_Feature_Flag : ushort
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
    }
    [Flags]public enum Button_Styles_Flag : byte
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
    public enum Chat_User_Type : byte
    {
        CHAT_USER_TYPE_SYSTEM = 0,
        CHAT_USER_TYPE_USER = 1,
        CHAT_USER_TYPE_PREFIX = 2,
        CHAT_USER_TYPE_O = 3,
        CHAT_USER_TYPE_MAX = 4,
    }
    public enum Car_Tyres : byte
    {
        CAR_TYRE_R1 = 0,
        CAR_TYRE_R2 = 1,
        CAR_TYRE_R3 = 2,
        CAR_TYRE_R4 = 3,
        CAR_TYRE_ROAD_SUPER = 4,
        CAR_TYRE_ROAD_NORMAL = 5,
        CAR_TYRE_HYBRID = 6,
        CAR_TYRE_KNOBBLY = 7,
        CAR_TYRE_MAX = 8,
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
    [Flags]public enum Car_Multiple_Flag : uint
    {
        CAR_TRACTION_NONE = 0,
        CAR_TRACTION_FRONT = 1,
        CAR_TRACTION_REAR = 2,
        CAR_TRACTION_4x4 = 1+2,
    }
    [Flags]public enum Confirm_Flag : byte
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
    [Flags]public enum Driver_Type_Flag : byte
    {
        DRIVER_TYPE_NONE = 0,
        DRIVER_TYPE_FEMALE = 1,
        DRIVER_TYPE_AI = 2,
        DRIVER_TYPE_REMOTE = 4
    }
    [Flags]public enum Button_Click_Flag : byte
    {
        ISB_CTRL = 4,
        ISB_LMB = 1,
        ISB_RMB = 2,
        ISB_SHIFT = 8
    }
    [Flags]public enum Car_Racing_Flag : byte
    {
        CAR_RACING_FLAG_NONE = 0,
        CAR_RACING_FLAG_BLUE = 1,
        CAR_RACING_FLAG_YELLOW = 2,
        CAR_RACING_FLAG_LAG = 32,
        CAR_RACING_FLAG_FIRST = 64,
        CAR_RACING_FLAG_LAST = 128,
    }
    [Flags]public enum InSim_Flag : byte
    {
        INSIM_FLAG_SPARE_0 = 1,
        INSIM_FLAG_SPARE_1 = 2,
        INSIM_FLAG_LOCAL = 4,
        INSIM_FLAG_KEEP_MESSAGE_COLOR = 8,
        INSIM_FLAG_RECEIVE_NLP = 16,
        INSIM_FLAG_RECEIVE_MCI = 32
    }
    [Flags]public enum Pit_Work_Flag : uint
    {
        PSE_BODY_MAJOR = 0x8000,
        PSE_BODY_MINOR = 0x4000,
        PSE_FR_DAM = 4,
        PSE_FR_WHL = 8,
        PSE_LE_FR_DAM = 0x10,
        PSE_LE_FR_WHL = 0x20,
        PSE_LE_RE_DAM = 0x400,
        PSE_LE_RE_WHL = 0x800,
        PSE_NOTHING = 1,
        PSE_RE_DAM = 0x100,
        PSE_RE_WHL = 0x200,
        PSE_REFUEL = 0x20000,
        PSE_RI_FR_DAM = 0x40,
        PSE_RI_FR_WHL = 0x80,
        PSE_RI_RE_DAM = 0xffe,
        PSE_RI_RE_WHL = 0x2000,
        PSE_SETUP = 0x10000,
        PSE_STOP = 2
    }
    [Flags]public enum Driver_Flag : ushort
    {
        DRIVER_FLAG_NONE = 0,
        DRIVER_FLAG_SWAPSIDE = 1,
        DRIVER_FLAG_GC_CUT = 2,
        DRIVER_FLAG_GC_BLIP = 4,
        DRIVER_FLAG_AUTOGEARS = 8,
        DRIVER_FLAG_SHIFTER = 16,
        DRIVER_FLAG_RESERVED = 32,
        DRIVER_FLAG_HELP_BRAKE = 64,
        DRIVER_FLAG_AXIS_CLUTCH = 128,
        DRIVER_FLAG_IS_IN_PITS = 256,
        DRIVER_FLAG_AUTOCLUTCH = 512,
        DRIVER_FLAG_MOUSE = 1024,
        DRIVER_FLAG_KEYBOARD_NO_HELP = 2048,
        DRIVER_FLAG_KEYBOARD_STABILISED = 4096,
        DRIVER_FLAG_CUSTOM_VIEW = 8192
    }

    [Flags]public enum Single_Char_Flag : byte
    {
        KEY_CONTROL = 2,
        KEY_NONE = 0,
        KEY_SHIFT = 1
    }
    [Flags]public enum Licence_View_Flag : ushort
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
    [Flags]public enum Out_Gauge_Flag : ushort
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