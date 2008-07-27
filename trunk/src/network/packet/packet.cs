﻿/* 
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

    sealed public class Packet                               ///<summary>This is a test ho what a test!</summary><returns></returns>                                  
    {
        public Packet(Packet_Size _packetSize, Packet_Type _packetType, byte[] _data)
        {
            //((Server)this).log.debug("Packet(), PacketType->" + _packetType + "\r\n");

            packetSize = (byte)_packetSize;
            packetType = _packetType;
            data = _data;
        }
        public Packet(Packet_Size _packetSize, Packet_Type _packetType, object _packet)
        {
            //((Server)this).log.debug("Packet(), Destructor->PacketType->" + _packetType + "\r\n");

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
            //((Server)this).log.debug("Packet(), PacketType->" + (Packet_Type)_data[1] + "\r\n");

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
            Add(Packet_Type.PACKET_BFN_ASK_REMOVE_ADD_BUTTON, new PacketBFN());
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
        }
    }
    [StructLayout(LayoutKind.Sequential)]public struct CarInformation
    {
        public ushort trackNode;
        public ushort lapNumber;
        public byte carId;
        public byte position;
        public Car_Flag carFlag;
        internal byte Sp3;
        public int posX;
        public int posY;
        public int posZ;
        public ushort speed;
        public ushort direction;
        public ushort heading;
        public short angleVelocity;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketAXI
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public byte AXStart;
        public byte NumCP;
        public ushort NumO;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x20)]public string LName;
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
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public Button_Function SubT;
        public byte UCID;
        public byte ClickID;
        public byte Inst;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTC
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte UCID;
        public byte ClickID;
        public byte Inst;
        public Button_Click_Flag CFlags;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTN
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte UCID;
        public byte ClickID;
        public byte Inst;
        public Button_Styles_Flag BStyle;
        public byte TypeIn;
        public byte L;
        public byte T;
        public byte W;
        public byte H;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=240)]public string Text;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketBTT
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte UCID;
        public byte ClickID;
        public byte Inst;
        public byte TypeIn;
        internal byte Sp3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x60)]public string Text;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCCH
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
        public Licence_Camera_Mode Camera;
        internal byte Sp1;
        internal byte Sp2;
        internal byte Sp3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketCNL
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte tempLicenceId;
        public Leave_Reason Reason;
        public byte Total;
        internal byte Sp2;
        internal byte Sp3;
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
        public byte ReqI;
        public byte UCID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x18)]public string PName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]public string Plate;
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
        public byte ReqI;
        public byte PLID;
        public uint TTime;
        public uint BTime;
        public byte SpA;
        public byte NumStops;
        public byte Confirm;
        public byte SpB;
        public ushort LapsDone;
        public Penality_Confirmation_Flag Flags;
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
        public byte ReqI;
        internal byte Zero;
        public byte UCID;
        public byte PLID;
        internal byte Sp2;
        internal byte Sp3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40)]public string Msg;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketISI
    {
        public PacketISI(byte _resquestId, ushort _udpPort, ushort _flags, char _prefix, ushort _interval, string _admin, string _iname)
        {
            packetSize = Packet_Size.PACKET_SIZE_ISI;
            packetType = Packet_Type.PACKET_ISI_INSIM_INITIALISE;
            ReqI = _resquestId;
            Zero = 0;
            UDPPort = _udpPort;
            Flags = _flags;
            Sp0 = 0;
            Prefix = _prefix;
            Interval = _interval;
            Admin = _admin;
            IName = _iname;
        }
        internal Packet_Size packetSize;
        internal Packet_Type packetType;
        public byte ReqI;
        internal byte Zero;
        public ushort UDPPort;
        public ushort Flags;
        internal byte Sp0;
        public char Prefix;
        public ushort Interval;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]public string Admin;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]public string IName;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x20)]public string HName;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketLAP
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte PLID;
        public uint LTime;
        public uint ETime;
        public ushort LapsDone;
        public Driver_Flag Flags;
        internal byte Sp0;
        public byte Penalty;
        public byte NumStops;
        internal byte Sp3;
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
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public Chat_Console_Sound Sound;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x80)]public string Msg;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x80)]
        public string message;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMST
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40)]
        public string Msg;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMSX
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x60)]
        public string Msg;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketMTC
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        internal byte Zero;
        public byte UCID;
        public byte PLID;
        internal byte Sp2;
        internal byte Sp3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x40)]public string Msg;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketNCN
    {
        internal byte packetSize;
        internal byte packetType;
        public byte requestId;
        public byte tempLicenceId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x18)]public string licenceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x18)]public string driverName;
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x20)]public NodeLap[] Info;
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x18)]public string driverName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]public string carPlate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]public string licenceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]public string SName;
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
        public byte ReqI;
        public byte NumP;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x20)]
        public byte[] carId;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketRES
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte carId;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x18)]public string UName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x18)]public string PName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=8)]public string Plate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]public string CName;
        public uint TTime;
        public uint BTime;
        public byte SpA;
        public byte NumStops;
        public Penality_Confirmation_Flag Confirm;
        public byte SpB;
        public ushort LapsDone;
        public Driver_Flag Flags;
        public byte ResultNum;
        public byte NumRes;
        public ushort PSeconds;
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
        public byte ReqI;
        public Small_Type SubT;
        public uint UVal;
    }
    [StructLayout(LayoutKind.Sequential)]public struct PacketSPX
    {
        internal byte packetSize;
        internal byte packetType;
        public byte ReqI;
        public byte carId;
        public uint STime;
        public uint ETime;
        public byte Split;
        public Penalty_Type Penalty;
        public byte NumStops;
        internal byte Sp3;
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
        public Race_In_Progress_Status raceInProgress;
        public byte qualificationMinute;

        // 0       : practice
        // 1-99    : number of laps...   laps  = rl
        // 100-190 : 100 to 1000 laps... laps  = (rl - 100) * 10 + 100
        // 191-238 : 1 to 48 hours...    hours = rl - 190
        public byte raceLaps;
        public byte spare2;
        public byte spare3;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=6)]public string trackName;
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
        internal byte Zero;
        public byte tempLicenceId;
        public Vote_Action voteAction;
        public byte Spare2;
        public byte Spare3;
    }
    [StructLayout(LayoutKind.Sequential)]public struct NodeLap
    {
        public ushort trackNode;
        public ushort lap;
        public byte carId;
        public byte position;
    }
    [StructLayout(LayoutKind.Sequential)]public struct OutGaugePack
    {
        internal byte packetSize;
        internal byte packetType;
        public uint Time;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=4)]public string Car;
        public Out_Gauge_Flag Flags;
        public byte Gear;
        public byte SpareB;
        public float Speed;
        public float RPM;
        public float Turbo;
        public float EngTemp;
        public float Fuel;
        public float OilPress;
        public float Spare1;
        public float Spare2;
        public float Spare3;
        public float Throttle;
        public float Brake;
        public float Clutch;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]public string Display1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=0x10)]public string Display2;
        public int ID;
    }
    [StructLayout(LayoutKind.Sequential)]public struct OutSimPack
    {
        internal byte packetSize;
        internal byte packetType;
        public uint Time;
        public VectorF AngVel;
        public float Heading;
        public float Pitch;
        public float Roll;
        public VectorF Accel;
        public VectorF Vel;
        public VectorI Pos;
        public int ID;
    }
    [StructLayout(LayoutKind.Sequential)]public struct VectorF
    {
        public float x;
        public float y;
        public float z;
    }
    [StructLayout(LayoutKind.Sequential)]public struct VectorI
    {
        public int x;
        public int y;
        public int z;
    }
}