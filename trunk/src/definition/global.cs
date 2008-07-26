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
namespace Drive_LFSS.Definition_
{
    using System;

    public enum Ctrl_Types
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
    public enum InSim_Socket_State : byte
    {
        INSIM_SOCKET_DISCONNECTED = 0,
        INSIM_SOCKET_CONNECTED = 1
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
    public enum Button_Function : byte
    {
        BUTTON_FUNCTION_DEL = 0,
        BUTTON_FUNCTION_CLEAR = 1,
        BUTTON_FUNCTION_USER_CLEAR = 2,
        BUTTON_FUNCTION_REQUEST = 3,
    }
    public enum Chat_Console_Sound : byte
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
    public enum Racing_Flag : byte
    {
        RACE_BLUE_FLAG = 1,
        RACE_YELLOW_FLAG = 2
    }
    public enum Vote_Action : byte
    {
        VOTE_END = 1,
        VOTE_NONE = 0,
        VOTE_NUM = 4,
        VOTE_QUALIFY = 3,
        VOTE_RESTART = 2
    }
    public enum Leave_Reason : byte
    {
        LEAVE_REASON_BANNED = 4,
        LEAVE_REASON_DISCONNECTED = 0,
        LEAVE_REASON_KICKED = 3,
        LEAVE_REASON_LOSTCONNECTION = 2,
        LEAVE_REASON_NUM = 6,
        LEAVE_REASON_SECURITY = 5,
        LEAVE_REASON_TIMEOUT = 1
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
    [Flags]public enum Driver_Type_Flag : byte
    {
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
    [Flags]public enum Button_Styles_Flag : byte
    {
        ISB_C1 = 1,
        ISB_C2 = 2,
        ISB_C4 = 4,
        ISB_CLICK = 8,
        ISB_DARK = 0x20,
        ISB_LEFT = 0x40,
        ISB_LIGHT = 0x10,
        ISB_RIGHT = 0x80
    }
    [Flags]public enum Car_Flag : byte
    {
        CCI_BLUE = 1,
        CCI_FIRST = 0x40,
        CCI_LAST = 0x80,
        CCI_YELLOW = 2
    }
    [Flags]public enum Penality_Confirmation_Flag : byte
    {
        CONF_CONFIRMED = 2,
        CONF_DID_NOT_PIT = 0x40,
        CONF_DISQ = 0x4c,
        CONF_MENTIONED = 1,
        CONF_PENALTY_30 = 0x10,
        CONF_PENALTY_45 = 0x20,
        CONF_PENALTY_DT = 4,
        CONF_PENALTY_SG = 8,
        CONF_TIME = 0x30
    }
    [Flags]public enum InSim_Flag : byte
    {
        ISF_LOCAL = 4,
        ISF_MCI = 0x20,
        ISF_MSO_COLS = 8,
        ISF_NLP = 0x10,
        ISF_RES_0 = 1,
        ISF_RES_1 = 2
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
    [Flags]public enum Race_Feature_Flag : ushort
    {
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
    [Flags]public enum Single_Char_Flag : byte
    {
        KEY_CONTROL = 2,
        KEY_NONE = 0,
        KEY_SHIFT = 1
    }
    [Flags]public enum Licence_View_Option : ushort
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
}