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
    using System;
    using System.Runtime.InteropServices;
    using System.Collections.Generic;
    using Drive_LFSS.Definition_;
    using Drive_LFSS.InSim_;

    sealed public class Packet
    {
        //TODO: Packet_Size _packetSize, Packet_Type _packetType, are really not needed, rip this off.
        public Packet(Packet_Size _packetSize, Packet_Type _packetType, byte[] _data)
        {
            packetSize = (byte)_packetSize;
            packetType = _packetType;
            data = _data;
        }
        public Packet(Packet_Size _packetSize, Packet_Type _packetType, object _packet)
        {
            packetSize = (byte)_packetSize;
            packetType = _packetType;

            //Move From here at the end
            byte[] _byteBuffer = new byte[packetSize];
            IntPtr pData = Marshal.AllocHGlobal(packetSize);
            GCHandle pinData = GCHandle.Alloc(pData, GCHandleType.Pinned);
            Marshal.StructureToPtr(_packet, pData, false);
            Marshal.Copy(pData, _byteBuffer, 0, _byteBuffer.Length); 
            pinData.Free();
            Marshal.FreeHGlobal(pData);

            data = _byteBuffer;
        }
        public Packet(byte[] _data)
        {
            packetSize = _data[0];
            packetType = (Packet_Type)_data[1];
            data = _data;
        }
        public byte packetSize;
        public Packet_Type packetType;
        public byte[] data;
    }
    sealed public class PacketStructureList : Dictionary<Packet_Type, object>
    {
        public PacketStructureList()
        {
            //Idealy Packet Sended/Received More Ofent into Top -> Bottom
            Add(Packet_Type.PACKET_MCI_MULTICAR_INFORMATION, new PacketMCI());
            Add(Packet_Type.PACKET_MSO_CHAT_RECEIVED, new PacketMSO());
            Add(Packet_Type.PACKET_VER_VERSION_SERVER, new PacketVER());
            Add(Packet_Type.PACKET_NCN_NEW_CONNECTION, new PacketNCN());
            Add(Packet_Type.PACKET_CNL_PART_CONNECTION, new PacketCNL());
            Add(Packet_Type.PACKET_NPL_DRIVER_JOIN_RACE, new PacketNPL());
            Add(Packet_Type.PACKET_PLL_DRIVER_LEAVE_RACE, new PacketPLL());
            Add(Packet_Type.PACKET_TINY_MULTI_PURPOSE, new PacketTiny());
            Add(Packet_Type.PACKET_AXI_AUTOCROSS_LAYOUT, new PacketAXI());
            Add(Packet_Type.PACKET_AXO_DRIVER_HIT_AUTOCROSS_OBJECT, new PacketAXO());
            Add(Packet_Type.PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE, new PacketBFN());
            Add(Packet_Type.PACKET_BTC_BUTTON_CLICK, new PacketBTC());
            Add(Packet_Type.PACKET_BTN_BUTTON_DISPLAY, new PacketBTN());
            Add(Packet_Type.PACKET_BTT_BUTTON_TYPE_IN_TEXT_OK, new PacketBTT());
            Add(Packet_Type.PACKET_CCH_LICENCE_CAMERA_CHANGE, new PacketCCH());
            Add(Packet_Type.PACKET_CPP_CAMERA_POSITION, new PacketCPP());
            Add(Packet_Type.PACKET_CPR_LICENCE_DRIVER_RENAME, new PacketCPR());
            Add(Packet_Type.PACKET_CRS_DRIVER_RESET_CAR, new PacketCRS());
            Add(Packet_Type.PACKET_FIN_DRIVER_FINISH_RACE, new PacketFIN());
            Add(Packet_Type.PACKET_FLG_DRIVER_BLUE_YELLOW_FLAG, new PacketFLG());
            Add(Packet_Type.PACKET_III_HIDDEN_MESSAGE_I, new PacketIII());
            Add(Packet_Type.PACKET_ISI_INSIM_INITIALISE, new PacketISI());
            Add(Packet_Type.PACKET_RST_RACE_START, new PacketRST());
            Add(Packet_Type.PACKET_STA_DRIVER_RACE_STATE_CHANGE, new PacketSTA());
            Add(Packet_Type.PACKET_MST_SEND_NORMAL_CHAT, new PacketMST());
            Add(Packet_Type.PACKET_MSL_MESSAGE_TO_LOCAL, new PacketMSL());
            Add(Packet_Type.PACKET_MTC_CHAT_TO_LICENCE, new PacketMTC());
            Add(Packet_Type.PACKET_LAP_DRIVER_LAP_TIME, new PacketLAP());
            Add(Packet_Type.PACKET_SPX_DRIVER_SPLIT_TIME, new PacketSPX());
            Add(Packet_Type.PACKET_SMALL_MULTI_PURPOSE, new PacketSmall());
            Add(Packet_Type.PACKET_REO_RACE_GRID_ORDER, new PacketREO());
            Add(Packet_Type.PACKET_RES_RESULT_CONFIRMED, new PacketRES());
            Add(Packet_Type.PACKET_VTN_VOTE_NOTIFICATION, new PacketVTN());
            Add(Packet_Type.PACKET_PLP_ENTER_GARAGE, new PacketPLP());
            Add(Packet_Type.PACKET_MSX_SEND_BIG_CHAT, new PacketMSX());
        }
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketAXI
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte zero;
        internal byte axStart;
        internal byte NumCP;
        internal ushort NumO;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]internal string LName;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketAXO
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte PLID;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBFN
    {
        internal PacketBFN(byte _connectionId, byte _buttonId, Button_Function _buttonFunction)
        {
            packetSize = Packet_Size.PACKET_SIZE_BFN;
            packetType = Packet_Type.PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE;
            requestId = 0;
            buttonFunction = _buttonFunction;
            connectionId = _connectionId;
            buttonId = _buttonId;
            inst = 0;
            spare3 = 0;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        internal byte requestId;
        internal Button_Function buttonFunction;
        internal byte connectionId;
        internal byte buttonId;
        internal byte inst;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTC
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte connectionId;
        internal byte buttonId;
        internal byte extendedMask;
        internal Button_Click_Flag clickMask;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTN
    {
        internal PacketBTN(byte _connectionId, byte _requestId, byte _buttonId, Button_Styles_Flag _buttonStyle, bool _isAllwaysVisible, byte _maxTextLength, byte _left, byte _top, byte _width, byte _height, string _text)
        {
            packetSize = Packet_Size.PACKET_SIZE_BTN;
            packetType = Packet_Type.PACKET_BTN_BUTTON_DISPLAY;
            requestId = _requestId;
            connectionId = _connectionId;
            buttonId = _buttonId;
            inst = (byte)(_isAllwaysVisible ? 128 : 0);
            styleMask = _buttonStyle;
            maxTextLength = _maxTextLength; // if 0x80 mean DialogBox???? have to test this
            left = _left;
            top = _top;
            width = _width;
            height = _height;
            text = _text;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        internal byte requestId;
        internal byte connectionId;
        internal byte buttonId;
        internal byte inst;
        internal Button_Styles_Flag styleMask;
        internal byte maxTextLength;
        internal byte left;
        internal byte top;
        internal byte width;
        internal byte height;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=240)]internal string text;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTT
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte connectionId;
        internal byte buttonId;
        internal byte extendedMask;
        internal byte originalTextLength;
        internal byte spare3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=96)]internal string typedText;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCCH
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal Licence_Camera_Mode camera;
        internal byte spare1;
        internal byte spare2;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCNL
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte connectionId;
        internal Leave_Reason quitReason;
        internal byte total;
        internal byte spare2;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCPP
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal int X;
        internal int Y;
        internal int Z;
        internal ushort H;
        internal ushort P;
        internal ushort R;
        internal byte ViewPLID;
        internal byte InGameCam;
        internal float FOV;
        internal ushort Time;
        internal Licence_View_Flag Flags;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCPR
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte connectionId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]internal string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]internal string carPlate;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCRS
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte PLID;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketFIN
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal uint totalTime;
        internal uint fastestLap;
        internal byte spare0;
        internal byte pitStopCount;
        internal Confirm_Flag confirmMask;
        internal byte SpB;
        internal ushort totalLap;
        internal Driver_Flag driverMask;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketFLG
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal byte OffOn;
        internal Racing_Flag blueOrYellow;
        internal byte carIdBehind;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketIII
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte zero;
        internal byte connectionId;
        internal byte carId;
        internal byte spare2;
        internal byte spare3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]internal string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketISI
    {
        internal PacketISI(byte _resquestId, ushort _portUDP, ushort _mask, char _commandPrefix, ushort _updateInterval, string _password, string _interfaceName)
        {
            packetSize = Packet_Size.PACKET_SIZE_ISI;
            packetType = Packet_Type.PACKET_ISI_INSIM_INITIALISE;
            requestId = _resquestId;
            zero = 0;
            portUDP = _portUDP;
            mask = _mask;
            spare0 = 0;
            commandPrefix = (byte)_commandPrefix;
            updateInterval = _updateInterval;
            password = _password;
            interfaceName = _interfaceName;
        }
        private Packet_Size packetSize;
        private Packet_Type packetType;
        private byte requestId;
        private byte zero;
        private ushort portUDP;
        private ushort mask;
        private byte spare0;
        private byte commandPrefix;
        private ushort updateInterval;
        //Should be 16 , but not working so putted 15..
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]private string password;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]private string interfaceName;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketISM
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal byte Host;
        internal byte Sp1;
        internal byte Sp2;
        internal byte Sp3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]internal string HName;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketLAP
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal uint lapTime;
        internal uint totalTime;
        internal ushort lapCompleted;
        internal Driver_Flag driverMask;
        internal byte spare0;
        internal Penalty_Type currentPenality;
        internal byte pitStopTotal;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMCI
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte NumC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]internal CarInformation[] carInformation;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMOD
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal int Bits16;
        internal int RR;
        internal int Width;
        internal int Height;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMSL
    {
        internal PacketMSL(string _message, Message_Sound _sound)
        {
            packetSize = Packet_Size.PACKET_SIZE_MSL;
            packetType = Packet_Type.PACKET_MSL_MESSAGE_TO_LOCAL;
            requestId = 0;
            sound = _sound;
            /*if (!_message.EndsWith("\x00"))
            {
                if (_message.Length > 126)
                    _message = _message.Substring(0, _message.Length - 2);
                _message += "\x00";
            }*/
            message = _message;
            zero = 0;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        internal byte requestId;
        internal Message_Sound sound;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]internal string message;
        internal byte zero;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMSO
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal byte connectionId;
        internal byte PLID;
        internal Chat_User_Type chatUserType;
        internal byte textStart;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]internal string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMST
    {
        internal PacketMST(byte _requestId, string _message)
        {
            packetSize = Packet_Size.PACKET_SIZE_MST;
            packetType = Packet_Type.PACKET_MST_SEND_NORMAL_CHAT;
            requestId = _requestId;
            zero = 0;
            message = _message;

        }
        internal PacketMST(string _message)
        {
            packetSize = Packet_Size.PACKET_SIZE_MST;
            packetType = Packet_Type.PACKET_MST_SEND_NORMAL_CHAT;
            requestId = 0;
            zero = 0;
            message = _message;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        internal byte requestId;
        internal byte zero;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]internal string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMSX
    {
        internal PacketMSX(string _message)
        {
            packetSize = Packet_Size.PACKET_SIZE_MSX;
            packetType = Packet_Type.PACKET_MSX_SEND_BIG_CHAT;
            requestId = 0;
            zero = 0;
            message = _message;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        private byte requestId;
        internal byte zero;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=96)]internal string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMTC
    {
        internal PacketMTC(byte _carId, string _message)
        {
            packetSize = Packet_Size.PACKET_SIZE_MTC;
            packetType = Packet_Type.PACKET_MTC_CHAT_TO_LICENCE;
            requestId = 0;
            zero = 0;
            connectionId = 0;
            carId = _carId;
            spare1 = 0;
            spare2 = 0;
            message = _message;
        }
        internal PacketMTC(byte _connectionId, string _message, byte _requestId)
        {
            packetSize = Packet_Size.PACKET_SIZE_MTC;
            packetType = Packet_Type.PACKET_MTC_CHAT_TO_LICENCE;
            requestId = _requestId;
            zero = 0;
            connectionId = _connectionId;
            carId = 0;
            spare1 = 0;
            spare2 = 0;
            message = _message;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        internal byte requestId;
        internal byte zero;
        internal byte connectionId;
        internal byte carId;
        internal byte spare1;
        internal byte spare2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]internal string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketNCN
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte connectionId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]internal string licenceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]internal string driverName;
        internal byte adminStatus;
        internal byte total;
        internal Driver_Type_Flag driverTypeMask ;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketNLP
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte NumP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]internal NodeLap[] Info;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketNPL
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal byte connectionId;
        internal Driver_Type_Flag driverTypeMask;
        internal Driver_Flag driverMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]internal string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]internal string carPlate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]internal string carPrefix;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=16)]internal string skinName;
        internal Car_Tyres tyreRearLeft;
        internal Car_Tyres tyreRearRight;
        internal Car_Tyres tyreFrontLeft;
        internal Car_Tyres tyreFrontRight;
        internal byte addedMass;
        internal byte addedIntakeRestriction;
        internal byte driverModel;
        internal byte passenger;
        internal int spare;
        internal byte endChar0;
        internal byte numberInRaceCar_NSURE;
        internal byte endChar1;
        internal byte endChar2;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPEN
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte PLID;
        internal Penalty_Type OldPen;
        internal Penalty_Type NewPen;
        internal Penalty_Reason Reason;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPFL
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte PLID;
        internal Driver_Flag Flags;
        internal ushort Spare;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPIT
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte PLID;
        internal ushort LapsDone;
        internal Driver_Flag Flags;
        internal byte Sp0;
        internal byte Penalty;
        internal byte NumStops;
        internal byte Sp3;
        internal Car_Tyres RL_Changed;
        internal Car_Tyres RR_Changed;
        internal Car_Tyres FL_Changed;
        internal Car_Tyres FR_Changed;
        internal Pit_Work_Flag Work;
        internal uint Spare;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPLA
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte carId;
        internal Pit_Lane_State Fact;
        internal byte Sp1;
        internal byte Sp2;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPLL
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPLP
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPSF
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal uint STime;
        internal uint Spare;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketREO
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]internal byte[] carIds;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketRES
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]internal string licenceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]internal string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]internal string carPlate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]internal string skinPrefix;
        internal uint totalTime;
        internal uint fastestLapTime;
        internal byte spare0;
        internal byte pitStopCount;
        internal Confirm_Flag confirmMask;
        internal byte spare1;
        internal ushort lapCount;
        internal Driver_Flag driverMask;
        internal byte positionFinal;
        internal byte resultCount;
        internal ushort penalitySecTotal;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketRST
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal byte raceLaps;
        internal byte qualificationMinute;
        internal byte carCount;
        internal byte Spare;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]internal string trackPrefix;
        internal Weather_Status weatherStatus;
        internal Wind_Status windStatus;
        internal Race_Feature_Flag raceFeatureMask;
        internal ushort nodeCount;
        internal ushort nodeFinishIndex;
        internal ushort nodeSplit1Index;
        internal ushort nodeSplit2Index;
        internal ushort nodeSplit3Index;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSCC
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal byte ViewPLID;
        internal byte InGameCam;
        internal byte Sp2;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSCH
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal byte CharB;
        internal byte Flags;
        internal byte Spare2;
        internal byte Spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSFP
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte ReqI;
        internal byte Zero;
        internal ushort Flag;
        internal byte OffOn;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSmall
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal Small_Type subType;
        internal uint uintValue;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSPX
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal uint splitTime;
        internal uint totalTime;
        internal byte splitNode;
        internal Penalty_Type currentPenalty;
        internal byte pitStopTotal;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSTA
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte zero;
        internal float replaySpeed;
        internal Licence_View_Flag viewOptionMask;
        internal Licence_Camera_Mode cameraMode;
        internal byte currentCarId;
        internal byte carCount;
        internal byte connectionCount;
        internal byte finishedCount;  //Not sure if this is , Finished Position or Number of Car who finish.
        internal Race_In_Progress_Status raceInProgressStatus;
        internal byte qualificationMinute;

        // 0       : practice
        // 1-99    : number of laps...   laps  = rl
        // 100-190 : 100 to 1000 laps... laps  = (rl - 100) * 10 + 100
        // 191-238 : 1 to 48 hours...    hours = rl - 190
        internal byte raceLaps;
        internal byte spare2;
        internal byte spare3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]internal string trackPrefix;
        internal Weather_Status weatherStatus;
        internal Wind_Status windStatus;
        //internal byte zero2;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketTiny
    {
        public PacketTiny(byte _resquestId, Tiny_Type _subTinyType)
        {
            packetSize = Packet_Size.PACKET_SIZE_TINY;
            packetType = Packet_Type.PACKET_TINY_MULTI_PURPOSE;
            requestId = _resquestId;
            subTinyType = _subTinyType;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        internal byte requestId;
        internal Tiny_Type subTinyType;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketTOC
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte carId;
        internal byte oldTLID;
        internal byte newTLID;
        internal byte Sp2;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketVER
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte zero;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]internal string serverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]internal string productVersion;
        internal ushort inSimVersion;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketVTN
    {
        internal byte packetSize;
        internal byte packetType;
        internal byte requestId;
        internal byte zero;
        internal byte connectionId;
        internal Vote_Action voteAction;
        internal byte spare2;
        internal byte spare3;
    }
}
