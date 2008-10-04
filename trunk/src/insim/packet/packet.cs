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
        public byte requestId;
        internal byte zero;
        public byte axStart;
        public byte NumCP;
        public ushort NumO;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]public string LName;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketAXO
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBFN
    {
        public PacketBFN(byte _licenceId, byte _buttonId, Button_Function _buttonFunction)
        {
            packetSize = Packet_Size.PACKET_SIZE_BFN;
            packetType = Packet_Type.PACKET_BFN_BUTTON_TRIGGER_AND_REMOVE;
            requestId = 0;
            buttonFunction = _buttonFunction;
            licenceId = _licenceId;
            buttonId = _buttonId;
            inst = 0;
            spare3 = 0;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        public byte requestId;
        public Button_Function buttonFunction;
        public byte licenceId;
        public byte buttonId;
        internal byte inst;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTC
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte licenceId;
        public byte buttonId;
        public byte extendedMask;
        public Button_Click_Flag clickMask;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTN
    {
        public PacketBTN(byte _licenceId, byte _requestId, byte _buttonId, Button_Styles_Flag _buttonStyle, bool _isAllwaysVisible, byte _maxTextLength, byte _left, byte _top, byte _width, byte _height, string _text)
        {
            packetSize = Packet_Size.PACKET_SIZE_BTN;
            packetType = Packet_Type.PACKET_BTN_BUTTON_DISPLAY;
            requestId = _requestId;
            licenceId = _licenceId;
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
        public byte requestId;
        public byte licenceId;
        public byte buttonId;
        public byte inst;
        public Button_Styles_Flag styleMask;
        public byte maxTextLength;
        public byte left;
        public byte top;
        public byte width;
        public byte height;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=240)]public string text;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTT
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte licenceId;
        public byte buttonId;
        public byte extendedMask;
        public byte originalTextLength;
        internal byte spare3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=96)]public string typedText;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCCH
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        public Licence_Camera_Mode camera;
        internal byte spare1;
        internal byte spare2;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCNL
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte tempLicenceId;
        public Leave_Reason quitReason;
        public byte total;
        internal byte spare2;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCPP
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public int X;
        public int Y;
        public int Z;
        public ushort H;
        public ushort P;
        public ushort R;
        public byte ViewPLID;
        public byte InGameCam;
        public float FOV;
        public ushort Time;
        public Licence_View_Flag Flags;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCPR
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte tempLicenceId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]public string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]public string carPlate;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCRS
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketFIN
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        public uint totalTime;
        public uint fastestLap;
        public byte spare0;
        public byte pitStopCount;
        public Confirm_Flag confirmMask;
        public byte SpB;
        public ushort totalLap;
        public Driver_Flag driverMask;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketFLG
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
        public byte OffOn;
        public Racing_Flag Flag;
        public byte CarBehind;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketIII
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        internal byte zero;
        public byte licenceId;
        public byte carId;
        internal byte spare2;
        internal byte spare3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]public string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketISI
    {
        public PacketISI(byte _resquestId, ushort _portUDP, ushort _mask, char _commandPrefix, ushort _updateInterval, string _password, string _interfaceName)
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
        public byte ReqI;
        internal byte Zero;
        public byte Host;
        internal byte Sp1;
        internal byte Sp2;
        internal byte Sp3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)]public string HName;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketLAP
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        public uint lapTime;
        public uint totalTime;
        public ushort lapCompleted;
        public Driver_Flag driverMask;
        internal byte spare0;
        public Penalty_Type currentPenality;
        public byte pitStopTotal;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMCI
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte NumC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]public CarInformation[] carInformation;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMOD
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public int Bits16;
        public int RR;
        public int Width;
        public int Height;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMSL
    {
        public PacketMSL(string _message, Message_Sound _sound)
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
        public byte requestId;
        public Message_Sound sound;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]public string message;
        internal byte zero;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMSO
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public byte tempLicenceId;
        public byte PLID;
        public Chat_User_Type chatUserType;
        public byte textStart;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=128)]
        public string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMST
    {
        public PacketMST(byte _requestId, string _message)
        {
            packetSize = Packet_Size.PACKET_SIZE_MST;
            packetType = Packet_Type.PACKET_MST_SEND_NORMAL_CHAT;
            requestId = _requestId;
            zero = 0;
            message = _message;

        }
        public PacketMST(string _message)
        {
            packetSize = Packet_Size.PACKET_SIZE_MST;
            packetType = Packet_Type.PACKET_MST_SEND_NORMAL_CHAT;
            requestId = 0;
            zero = 0;
            message = _message;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        public byte requestId;
        internal byte zero;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]public string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMSX
    {
        public PacketMSX(string _message)
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=96)]public string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMTC
    {
        public PacketMTC(byte _carId, string _message)
        {
            packetSize = Packet_Size.PACKET_SIZE_MTC;
            packetType = Packet_Type.PACKET_MTC_CHAT_TO_LICENCE;
            requestId = 0;
            zero = 0;
            licenceId = 0;
            carId = _carId;
            spare1 = 0;
            spare2 = 0;
            message = _message;
        }
        public PacketMTC(byte _licenceId, string _message, byte _requestId)
        {
            packetSize = Packet_Size.PACKET_SIZE_MTC;
            packetType = Packet_Type.PACKET_MTC_CHAT_TO_LICENCE;
            requestId = _requestId;
            zero = 0;
            licenceId = _licenceId;
            carId = 0;
            spare1 = 0;
            spare2 = 0;
            message = _message;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        public byte requestId;
        internal byte zero;
        public byte licenceId;
        public byte carId;
        internal byte spare1;
        internal byte spare2;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=64)]public string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketNCN
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte tempLicenceId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]public string licenceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]public string driverName;
        public byte adminStatus;
        public byte total;
        public Driver_Type_Flag driverTypeMask ;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketNLP
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte NumP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]public NodeLap[] Info;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketNPL
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        public byte tempLicenceId;
        public Driver_Type_Flag driverTypeMask;
        public Driver_Flag driverMask;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]public string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]public string carPlate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]public string carPrefix;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=16)]public string skinName;
        public Car_Tyres tyreRearLeft;
        public Car_Tyres tyreRearRight;
        public Car_Tyres tyreFrontLeft;
        public Car_Tyres tyreFrontRight;
        public byte addedMass;
        public byte addedIntakeRestriction;
        public byte driverModel;
        public byte passenger;
        public int spare;
        internal byte endChar0;
        public byte numberInRaceCar_NSURE;
        internal byte endChar1;
        internal byte endChar2;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPEN
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
        public Penalty_Type OldPen;
        public Penalty_Type NewPen;
        public Penalty_Reason Reason;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPFL
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
        public Driver_Flag Flags;
        public ushort Spare;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPIT
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
        public ushort LapsDone;
        public Driver_Flag Flags;
        internal byte Sp0;
        public byte Penalty;
        public byte NumStops;
        internal byte Sp3;
        public Car_Tyres RL_Changed;
        public Car_Tyres RR_Changed;
        public Car_Tyres FL_Changed;
        public Car_Tyres FR_Changed;
        public Pit_Work_Flag Work;
        public uint Spare;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPLA
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte carId;
        public Pit_Lane_State Fact;
        internal byte Sp1;
        internal byte Sp2;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPLL
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPLP
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketPSF
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        public uint STime;
        public uint Spare;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketREO
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]public byte[] carIds;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketRES
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]public string licenceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=24)]public string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]public string carPlate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]public string skinPrefix;
        public uint totalTime;
        public uint fastestLapTime;
        public byte spare0;
        public byte pitStopCount;
        public Confirm_Flag confirmMask;
        public byte spare1;
        public ushort lapCount;
        public Driver_Flag driverMask;
        public byte positionFinal;
        public byte resultCount;
        public ushort penalitySecTotal;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketRST
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public byte raceLaps;
        public byte qualificationMinute;
        public byte carCount;
        public byte Spare;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]public string trackName;
        public Weather_Status weatherStatus;
        public Wind_Status windStatus;
        public Race_Feature_Flag raceFeatureMask;
        public ushort nodeCount;
        public ushort nodeFinishIndex;
        public ushort nodeSplit1Index;
        public ushort nodeSplit2Index;
        public ushort nodeSplit3Index;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSCC
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public byte ViewPLID;
        public byte InGameCam;
        internal byte Sp2;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSCH
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public byte CharB;
        public byte Flags;
        public byte Spare2;
        public byte Spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSFP
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public ushort Flag;
        public byte OffOn;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSmall
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public Small_Type subType;
        public uint uintValue;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSPX
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        public uint splitTime;
        public uint totalTime;
        public byte splitNode;
        public Penalty_Type currentPenalty;
        public byte pitStopTotal;
        internal byte spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSTA
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        internal byte zero;
        public float replaySpeed;
        public Licence_View_Flag viewOptionMask;
        public Licence_Camera_Mode cameraMode;
        public byte currentCarId;
        public byte carCount;
        public byte connectionCount;
        public byte finishedCount;  //Not sure if this is , Finished Position or Number of Car who finish.
        public Race_In_Progress_Status raceInProgressStatus;
        public byte qualificationMinute;

        // 0       : practice
        // 1-99    : number of laps...   laps  = rl
        // 100-190 : 100 to 1000 laps... laps  = (rl - 100) * 10 + 100
        // 191-238 : 1 to 48 hours...    hours = rl - 190
        public byte raceLaps;
        public byte spare2;
        public byte spare3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]public string trackPrefix;
        public Weather_Status weatherStatus;
        public Wind_Status windStatus;
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
        public byte requestId;
        public Tiny_Type subTinyType;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketTOC
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte carId;
        public byte oldTLID;
        public byte newTLID;
        internal byte Sp2;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketVER
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        internal byte zero;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]public string serverVersion;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]public string productVersion;
        public ushort inSimVersion;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketVTN
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        internal byte zero;
        public byte tempLicenceId;
        public Vote_Action voteAction;
        internal byte spare2;
        internal byte spare3;
    }
}
