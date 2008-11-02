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
using System.Collections.Generic;
using System.Data;

namespace Drive_LFSS.Storage_
{
    using Drive_LFSS.Definition_;
    using Drive_LFSS.Log_;

    abstract class Storage
    {
        protected Storage(string[] tableFmt)
        {
            tableName = tableFmt[0];
            tableFormat = tableFmt[1].ToCharArray();
        }
        ~Storage()
        {
            if (true == false) { }
        }
        private string tableName;
        private char[] tableFormat;
        private uint maxEntry = 0;
        private Dictionary<uint, object[]> data = new Dictionary<uint, object[]>();

        public bool Load(bool errorOnEmpty)
        {
            bool returnValue = false;
            data.Clear();

            IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT * FROM `" + tableName + "` LIMIT 1");
            if (reader.Read())
            {
                if (reader.FieldCount == tableFormat.Length)
                {
                    reader.Dispose();
                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT * FROM `" + tableName + "`");

                    Log.commandHelp("  Loading storage \"" + tableName + "\"\r\n");

                    List<object> value = new List<object>();
                    int valueIndex = 0;
                    uint dataIndex = 0;
                    while (reader.Read())
                    {
                        valueIndex = 0;
                        dataIndex = 0;
                        value.Clear();
                        for (byte index = 0; index < tableFormat.Length; index++)
                        {
                            if (tableFormat[index] == 'x')
                                continue;

                            switch (tableFormat[index])
                            {
                                case 'u':
                                    if (reader.GetFieldType(index) == typeof(UInt32) || reader.GetFieldType(index) == typeof(byte) || reader.GetFieldType(index) == typeof(UInt16))
                                        value.Add(reader.IsDBNull(index) ? 0 : (uint)reader.GetInt32(index));
                                    else
                                        Log.error("  UINT unsupported field type for: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
                                    break;
                                case 'p':
                                    if (reader.GetFieldType(index) == typeof(UInt32) || reader.GetFieldType(index) == typeof(byte) || reader.GetFieldType(index) == typeof(UInt16))
                                    {
                                        value.Add(reader.IsDBNull(index) ? 0 : (uint)reader.GetInt32(index));
                                        dataIndex = (reader.IsDBNull(index) ? 0 : (uint)reader.GetInt32(index));
                                    }
                                    else
                                        Log.error("  Primary key unsupported field type for: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
                                    break;
                                case 'i':
                                    if (reader.GetFieldType(index) == typeof(Int32) || reader.GetFieldType(index) == typeof(byte))
                                        value.Add(reader.IsDBNull(index) ? 0 : reader.GetInt32(index));
                                    else
                                        Log.error("  INT unsupported field type for: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
                                    break;
                                case 's':
                                    value.Add((reader.IsDBNull(index) ? "NULL" : "" + reader.GetString(index)));
                                    break;
                                case 'f':
                                    if (reader.GetFieldType(index) == typeof(Double))
                                        value.Add((float)(reader.IsDBNull(index) ? 0 : reader.GetDouble(index)));
                                    else if (reader.GetFieldType(index) == typeof(Decimal))
                                        value.Add((float)(reader.IsDBNull(index) ? 0 : reader.GetDecimal(index)));
                                    else
                                        Log.error("  Unsupported field type for: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
                                    break;
                            }
                            valueIndex++;

                        }
                        data[dataIndex] = value.ToArray();

                    }
                    returnValue = true;
                }
                else
                    Log.error("  Storage system - Database table:'" + tableName + "', has an invalid field count, has: " + reader.FieldCount + ", should be: " + tableFormat.Length + "\r\n");
            }
            else if (errorOnEmpty)
                Log.error("  Storage system - Database table:'" + tableName + "', is empty.\r\n");
            else
                Log.commandHelp("  Loading storage \"" + tableName + "\"\r\n");

            reader.Dispose();
            reader = null;

            SetMaxEntry();
            return returnValue;
        }
        protected virtual object[] GetEntry(uint entry)
        {
            if (data.ContainsKey(entry))
                return data[entry];
            return null;
        }
        
        protected virtual object[] GetCustom(int fieldIndex, string equalTo)
        {
            Dictionary<uint,object[]>.Enumerator itr = data.GetEnumerator();
            while(itr.MoveNext())
            {
                if (itr.Current.Value.Length > fieldIndex && (string)itr.Current.Value[fieldIndex] == equalTo)
                    return itr.Current.Value;
            }
            return null;
        }
        protected virtual object[] GetCustom(int fieldIndex, uint equalTo)
        {
            Dictionary<uint, object[]>.Enumerator itr = data.GetEnumerator();
            while (itr.MoveNext())
            {
                if (itr.Current.Value.Length > fieldIndex && (uint)itr.Current.Value[fieldIndex] == equalTo)
                    return itr.Current.Value;
            }
            return null;
        }
        protected virtual object[] GetCustom(int fieldIndex, int equalTo)
        {
            Dictionary<uint, object[]>.Enumerator itr = data.GetEnumerator();
            while (itr.MoveNext())
            {
                if (itr.Current.Value.Length > fieldIndex && (int)itr.Current.Value[fieldIndex] == equalTo)
                    return itr.Current.Value;
            }
            return null;
        }
        
        protected virtual object[] GetCustom(int[] fieldIndex, string[] equalTo)
        {
            Dictionary<uint,object[]>.Enumerator itr = data.GetEnumerator();
            while(itr.MoveNext())
            {
                bool good = true;
                foreach(int index in fieldIndex)
                {
                    if (itr.Current.Value.Length > fieldIndex[index] && (string)itr.Current.Value[fieldIndex[index]] == equalTo[index])
                        continue;
                    good = false;
                    break;
                }
                if(good)
                    return itr.Current.Value;
            }
            return null;
        }
        protected virtual object[] GetCustom(int[] fieldIndex, uint[] equalTo)
        {
            Dictionary<uint,object[]>.Enumerator itr = data.GetEnumerator();
            while(itr.MoveNext())
            {
                bool good = true;
                foreach(int index in fieldIndex)
                {
                    if (itr.Current.Value.Length > fieldIndex[index] && (uint)itr.Current.Value[fieldIndex[index]] == equalTo[index])
                        continue;
                    good = false;
                    break;
                }
                if(good)
                    return itr.Current.Value;
            }
            return null;
        }
        protected virtual object[] GetCustom(int[] fieldIndex, int[] equalTo)
        {
            Dictionary<uint,object[]>.Enumerator itr = data.GetEnumerator();
            while(itr.MoveNext())
            {
                bool good = true;
                foreach(int index in fieldIndex)
                {
                    if (itr.Current.Value.Length > fieldIndex[index] && (int)itr.Current.Value[fieldIndex[index]] == equalTo[index])
                        continue;
                    good = false;
                    break;
                }
                if(good)
                    return itr.Current.Value;
            }
            return null;
        }

        private void SetMaxEntry()
        {
            Dictionary<uint, object[]>.KeyCollection keys = data.Keys;
            foreach(uint entry in keys)
            {
                if (maxEntry < entry)
                    maxEntry = entry;
            }
        }
        public uint GetCount()
        {
            return (uint)data.Count;
        }
        public uint GetMaxEntry()
        {
            return maxEntry;
        }
    }

    //Normaly they should not have a "public sealed class ButtonTemplate : Storage"
    //I tryed with Storage<T> and Storage<ButtonTemplate>, but Template into c# are fucking limited
    sealed class ButtonTemplate : Storage
    {
        public ButtonTemplate(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~ButtonTemplate()
        {
            if (true == false) { }
        }
        new public ButtonTemplateInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new ButtonTemplateInfo(_temp);
            else
                return null;
        }
    }
    sealed class ButtonTemplateInfo : ICloneable
    {
        public ButtonTemplateInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt16(rowInfos[0]);
            styleMask = (Button_Styles_Flag)Convert.ToUInt16(rowInfos[1]);
            isAllwaysVisible = Convert.ToUInt16(rowInfos[2]) > 0 ? true : false;
            maxInputChar = (byte)Convert.ToUInt16(rowInfos[3]);
            left = (byte)Convert.ToUInt16(rowInfos[4]);
            top = (byte)Convert.ToUInt16(rowInfos[5]);
            width = (byte)Convert.ToUInt16(rowInfos[6]);
            height = (byte)Convert.ToUInt16(rowInfos[7]);
            text = (string)rowInfos[8];
            textCaption = (string)rowInfos[9];
        }
        ~ButtonTemplateInfo()
        {
            if (true == false) { }
        }
        public object Clone() // ICloneable implementation
        {
            return this.MemberwiseClone() as ButtonTemplateInfo;
        }
        private ushort entry;
        private Button_Styles_Flag styleMask;
        private bool isAllwaysVisible;
        private byte maxInputChar;
        private byte left;
        private byte top;
        private byte width;
        private byte height;
        private string text;
        private string textCaption;

        public ushort Entry
        {
            get { return entry; }
        }
        public Button_Styles_Flag StyleMask
        {
            get { return styleMask; }
            set { styleMask = value; }
        }
        public bool IsAllwaysVisible
        {
            get { return isAllwaysVisible; }
            set { isAllwaysVisible = value; }
        }
        public byte MaxInputChar
        {
            get { return maxInputChar; }
            set { maxInputChar = value; }
        }
        public byte Left
        {
            get { return left; }
            set { left = value; }
        }
        public byte Top
        {
            get { return top; }
            set { top = value; }
        }
        public byte Width
        {
            get { return width; }
            set { width = value; }
        }
        public byte Height
        {
            get { return height; }
            set { height = value; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
        public string TextCaption
        {
            get { return textCaption; }
        }

    }

    sealed class GuiTemplate : Storage
    {
        public GuiTemplate(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~GuiTemplate()
        {
            if (true == false) { }
        }
        new public GuiTemplateInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new GuiTemplateInfo(_temp);
            else
                return null;
        }
    }
    sealed class GuiTemplateInfo
    {
        public GuiTemplateInfo(object[] rowInfos)
        {
            entry = (Gui_Entry)Convert.ToUInt16(rowInfos[0]);
            buttonEntry = (string)rowInfos[1];
            buttonEntryExt = (string)rowInfos[2];
            textButtonEntry = Convert.ToUInt16(rowInfos[3]);
            text = (string)rowInfos[4];
        }
        ~GuiTemplateInfo()
        {
            if (true == false) { }
        }
        private Gui_Entry entry;
        private string buttonEntry;
        private string buttonEntryExt;
        private ushort textButtonEntry;
        private string text;

        public Gui_Entry Entry
        {
            get { return entry; }
        }
        public string ButtonEntry
        {
            get { return buttonEntry; }
        }
        public string ButtonEntryExt
        {
            get { return buttonEntryExt; }
        }
        public ushort TextButtonEntry
        {
            get { return textButtonEntry; }
        }
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }

    sealed class TrackTemplate : Storage
    {
        public TrackTemplate(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~TrackTemplate()
        {
            if (true == false) { }
        }
        new public TrackTemplateInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new TrackTemplateInfo(_temp);
            else
                return null;
        }
    }
    sealed class TrackTemplateInfo
    {
        public TrackTemplateInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt32(rowInfos[0]);
            namePrefix = (string)rowInfos[1];
            name = (string)rowInfos[2];
            configuration = (string)rowInfos[3];
            reverse = (Convert.ToUInt32(rowInfos[4]) == 1 ? true : false);
            nodeIndex = new byte[3]
            {
                (byte)Convert.ToUInt16(rowInfos[5]),
                (byte)Convert.ToUInt16(rowInfos[6]),
                (byte)Convert.ToUInt16(rowInfos[7])
            };
            totalLength = (uint)Convert.ToUInt16(rowInfos[8]);

        }
        ~TrackTemplateInfo()
        {
            if (true == false) { }
        }
        private uint entry;
        private string namePrefix;
        private string name;
        private string configuration;
        private bool reverse;
        private byte[] nodeIndex;
        private uint totalLength;

        public string NamePrefix
        {
            get { return namePrefix; }
        }
        public string Name
        {
            get { return name; }
        }
        public string Configuration
        {
            get { return configuration; }
        }
        public bool Reverse
        {
            get { return reverse; }
        }
        public byte[] NodeIndex
        {
            get { return nodeIndex; }
        }
        public uint TotalLength
        {
            get { return totalLength; }
        }
    }

    sealed class CarTemplate : Storage
    {
        public CarTemplate(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~CarTemplate()
        {
            if (true == false) { }
        }
        new public CarTemplateInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new CarTemplateInfo(_temp);

            return null;
        }
        public CarTemplateInfo GetCarPrefix(string carPrefix)
        {
            object[] _temp = base.GetCustom(1, carPrefix);
            if(_temp != null)
                return new CarTemplateInfo(_temp);

            return null;
        }
    }
    sealed class CarTemplateInfo
    {
        public CarTemplateInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt32(rowInfos[0]);
            namePrefix = (string)rowInfos[1];
            name = (string)rowInfos[2];
            brakeDist = Convert.ToUInt16(rowInfos[3]);
            mask = (Car_Multiple_Flag)Convert.ToUInt32(rowInfos[4]);
        }
        ~CarTemplateInfo()
        {
            if (true == false) { }
        }
        private uint entry;
        private string namePrefix;
        private string name;
        private ushort brakeDist;
        private Car_Multiple_Flag mask;
        
        public uint Entry
        {
            get{return entry;}
        }
        public string NamePrefix
        {
            get { return namePrefix; }
        }
        public string Name
        {
            get { return name; }
        }
        public ushort BrakeDist
        {
            get { return brakeDist; }
        }
        public Car_Multiple_Flag Mask
        {
            get { return mask; }
        }
    }

    sealed class RaceTemplate : Storage
    {
        public RaceTemplate(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~RaceTemplate()
        {
            if (true == false) { }
        }
        new public RaceTemplateInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new RaceTemplateInfo(_temp);
            else
                return null;
        }
    }
    sealed class RaceTemplateInfo
    {
        public RaceTemplateInfo()
        {

        }
        public RaceTemplateInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt32(rowInfos[0]);
            description = (string)(rowInfos[1]);
            trackEntry = (byte)Convert.ToUInt16(rowInfos[2]);
            carEntryAllowed = (string)rowInfos[3];
            weather = (Weather_Status)Convert.ToUInt16(rowInfos[4]);
            wind = (Wind_Status)Convert.ToUInt16(rowInfos[5]);
            lapCount = (byte)Convert.ToUInt16(rowInfos[6]);
            qualifyMinute = (byte)Convert.ToUInt16(rowInfos[7]);
            maximunRaceEnd = Convert.ToUInt16(rowInfos[8]);
            raceTemplateMask = (Race_Template_Flag)Convert.ToUInt16(rowInfos[9]);
            restriction_join_entry = Convert.ToUInt32(rowInfos[10]);
            restriction_race_entry = Convert.ToUInt32(rowInfos[11]);
        }
        ~RaceTemplateInfo()
        {
            if (true == false) { }
        }
        private uint entry = 0;
        private string description = "";
        private byte trackEntry = 0;
        private string carEntryAllowed = "";
        private Weather_Status weather = Weather_Status.WEATHER_CLEAR_DAY;
        private Wind_Status wind = Wind_Status.WIND_NONE;
        private byte lapCount = 0;
        private byte qualifyMinute = 0;
        private ushort maximunRaceEnd = 0;
        private Race_Template_Flag raceTemplateMask = Race_Template_Flag.NONE;
        private uint restriction_join_entry = 0;
        private uint restriction_race_entry = 0;
        public uint Entry
        {
            get { return entry; }
        }
        public string Description
        {
            get { return description; }
        }
        public byte TrackEntry
        {
            get { return trackEntry; }
        }
        public string CarEntryAllowed
        {
            get { return carEntryAllowed; }
        }
        public Weather_Status Weather
        {
            get { return weather; }
        }
        public Wind_Status Wind
        {
            get { return wind; }
        }
        public byte LapCount
        {
            get { return lapCount; }
        }
        public byte QualifyMinute
        {
            get { return qualifyMinute; }
        }
        public ushort maximunFinishCount
        {
            get { return maximunRaceEnd; }
        }
        public bool HasRaceTemplateFlag(Race_Template_Flag flag)
        {
            return (raceTemplateMask & flag) == flag;
        }
        public uint RestrictionJoinEntry
        {
            get { return restriction_join_entry; }
        }
        public uint Restriction_race_entry
        {
            get { return restriction_race_entry; }
        }
    }

    sealed class DriverBan : Storage
    {
        public DriverBan(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~DriverBan()
        {
            if (true == false) { }
        }
        new public DriverBanInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new DriverBanInfo(_temp);
            else
                return null;
        }
    }
    sealed class DriverBanInfo
    {
        public DriverBanInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt32(rowInfos[0]);
            licenceName = (string)rowInfos[1];
            fromLicenceName = (string)rowInfos[2];
            reason = (string)rowInfos[3];
            startTime = Convert.ToUInt32(rowInfos[4]);
            endTime = Convert.ToUInt32(rowInfos[5]);
            expired = Convert.ToUInt16(rowInfos[6]) == 1 ? true : false;
        }
        ~DriverBanInfo()
        {
            if (true == false) { }
        }
        private uint entry;
        private string licenceName;
        private string fromLicenceName;
        private string reason;
        private uint startTime;
        private uint endTime;
        private bool expired;
    }

    sealed class RestrictionRace : Storage
    {
        public RestrictionRace(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~RestrictionRace()
        {
            if (true == false) { }
        }
        new public RestrictionRaceInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new RestrictionRaceInfo(_temp);
            else
                return null;
        }
    }
    sealed class RestrictionRaceInfo
    {
        public RestrictionRaceInfo()
        {

        }
        public RestrictionRaceInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt32(rowInfos[0]);
            description = (string)rowInfos[1];
            speedMsMax = Convert.ToDouble(rowInfos[2]);
            speedMsMaxLapNumber = (string)rowInfos[3];
            speedMsMaxPen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[4]);
            tyre[(int)Tyre_Position.REAR_LEFT] = (Tyre_Compound)Convert.ToUInt16(rowInfos[5]);
            tyre[(int)Tyre_Position.REAR_RIGHT] = (Tyre_Compound)Convert.ToUInt16(rowInfos[6]);
            tyre[(int)Tyre_Position.FRONT_LEFT] = (Tyre_Compound)Convert.ToUInt16(rowInfos[7]);
            tyre[(int)Tyre_Position.FRONT_RIGHT] = (Tyre_Compound)Convert.ToUInt16(rowInfos[8]);
            tyrePen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[9]);
            pitWork1 = (Pit_Work_Flag)Convert.ToUInt16(rowInfos[10]);
            pitWork1Pen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[11]);
            pitWork2 = (Pit_Work_Flag)Convert.ToUInt16(rowInfos[12]);
            pitWork2Pen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[13]);
            passenger = (Passenger_Flag)Convert.ToUInt16(rowInfos[14]);
            passengerPen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[15]);
            addedMass = (byte)Convert.ToUInt16(rowInfos[16]);
            addedMassPen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[17]);
            intakeRestriction = (byte)Convert.ToUInt16(rowInfos[18]);
            intakeRestrictionPen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[19]);
            penalityReason = (Penalty_Reason)Convert.ToUInt16(rowInfos[20]);
            penalityReasonPen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[21]);
            driverMask = (Driver_Flag)Convert.ToUInt16(rowInfos[22]);
            driverMaskPen = (Penalty_Type_Ext)Convert.ToUInt16(rowInfos[23]);
            
        }
        ~RestrictionRaceInfo()
        {
            if (true == false) { }
        }
        private uint entry = 0;
        private string description = "";
        
        private double speedMsMax = 0;
        private string speedMsMaxLapNumber = "";
        private Penalty_Type_Ext speedMsMaxPen = Penalty_Type_Ext.NONE;
        
        private Tyre_Compound[] tyre = new Tyre_Compound[(int)Tyre_Position.MAX];
        private Penalty_Type_Ext tyrePen = Penalty_Type_Ext.NONE;
        
        private Pit_Work_Flag pitWork1 = Pit_Work_Flag.NOTHING;
        private Penalty_Type_Ext pitWork1Pen = Penalty_Type_Ext.NONE;
        private Pit_Work_Flag pitWork2 = Pit_Work_Flag.NOTHING;
        private Penalty_Type_Ext pitWork2Pen = Penalty_Type_Ext.NONE;
        
        private Passenger_Flag passenger = Passenger_Flag.NONE;
        private Penalty_Type_Ext passengerPen = Penalty_Type_Ext.NONE;
        
        private byte addedMass = 0;
        private Penalty_Type_Ext addedMassPen = Penalty_Type_Ext.NONE;

        private byte intakeRestriction = 0;
        private Penalty_Type_Ext intakeRestrictionPen = Penalty_Type_Ext.NONE;

        private Penalty_Reason penalityReason = Penalty_Reason.NONE;
        private Penalty_Type_Ext penalityReasonPen = Penalty_Type_Ext.NONE;
        
        private Driver_Flag driverMask = Driver_Flag.NONE;
        private Penalty_Type_Ext driverMaskPen = Penalty_Type_Ext.NONE;
    }

    sealed class RestrictionJoin : Storage
    {
        public RestrictionJoin(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~RestrictionJoin()
        {
            if (true == false) { }
        }
        new public RestrictionJoinInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new RestrictionJoinInfo(_temp);
            else
                return null;
        }
    }
    sealed class RestrictionJoinInfo
    {
        public RestrictionJoinInfo()
        {

        }
        public RestrictionJoinInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt32(rowInfos[0]);
            description = (string)rowInfos[1];
            safeDrivingPct = Convert.ToByte(rowInfos[2]);
            safeDrivingKick = Convert.ToBoolean(rowInfos[3]);
            badlanguagePct = Convert.ToByte(rowInfos[4]);
            badlanguagePctKick = Convert.ToBoolean(rowInfos[5]);
            pbMin = Convert.ToUInt32(rowInfos[6]);
            pbMax = Convert.ToUInt32(rowInfos[7]);
            pbKick = Convert.ToBoolean(rowInfos[8]);
            wrMin = Convert.ToUInt32(rowInfos[9]);
            wrMax = Convert.ToUInt32(rowInfos[10]);
            wrKick = Convert.ToBoolean(rowInfos[11]);
            skinName = (string)rowInfos[12];
            skinNameKick = Convert.ToBoolean(rowInfos[13]);
            driverName = (string)rowInfos[14];
            driverNameKick = Convert.ToBoolean(rowInfos[15]);
            rankBestMin = Convert.ToUInt16(rowInfos[16]);
            rankBestMax = Convert.ToUInt16(rowInfos[17]);
            rankAvgMin = Convert.ToUInt16(rowInfos[18]);
            rankAvgMax = Convert.ToUInt16(rowInfos[19]);
            rankStaMin = Convert.ToUInt16(rowInfos[20]);
            rankStaMax = Convert.ToUInt16(rowInfos[21]);
            rankWinMin = Convert.ToUInt16(rowInfos[22]);
            rankWinMax = Convert.ToUInt16(rowInfos[23]);
            rankTotalMin = Convert.ToUInt16(rowInfos[24]);
            rankTotalMax = Convert.ToUInt16(rowInfos[25]);
            rankKick = Convert.ToBoolean(rowInfos[26]);

        }
        ~RestrictionJoinInfo()
        {
            if (true == false) { }
        }
        private uint entry = 0;
        private string description = "";
        private byte safeDrivingPct = 0;
        private bool safeDrivingKick = false;
        private byte badlanguagePct = 0;
        private bool badlanguagePctKick = false;
        private uint pbMin = 0;
        private uint pbMax = 0;
        private bool pbKick = false;
        private uint wrMin = 0;
        private uint wrMax = 0;
        private bool wrKick = false;
        private string skinName = "";
        private bool skinNameKick = false;
        private string driverName = "";
        private bool driverNameKick = false;
        private ushort rankBestMin = 0;
        private ushort rankBestMax = 0;
        private ushort rankAvgMin = 0;
        private ushort rankAvgMax = 0;
        private ushort rankStaMin = 0;
        private ushort rankStaMax = 0;
        private ushort rankWinMin = 0;
        private ushort rankWinMax = 0;
        private ushort rankTotalMin = 0;
        private ushort rankTotalMax = 0;
        private bool rankKick = false;
        
        public string Description
        {
            get{return description;}
        }
        public byte SafeDrivingPct
        {
            get { return safeDrivingPct; }
        }
        public bool SafeDrivingKick
        {
            get { return safeDrivingKick; }
        }
        public byte BadlanguagePct
        {
            get { return badlanguagePct; }
        }
        public bool BadlanguagePctKick
        {
            get { return badlanguagePctKick; }
        }
        public uint PbMin
        {
            get { return pbMin; }
        }
        public uint PbMax
        {
            get { return pbMax; }
        }
        public bool PbKick
        {
            get { return pbKick; }
        }
        public uint WrMin
        {
            get { return wrMin; }
        }
        public uint WrMax
        {
            get { return wrMax; }
        }
        public bool WrKick
        {
            get { return wrKick; }
        }
        public string SkinName
        {
            get { return skinName; }
        }
        public bool SkinNameKick
        {
            get { return skinNameKick; }
        }
        public string DriverName
        {
            get { return driverName; }
        }
        public bool DriverNameKick
        {
            get { return driverNameKick; }
        }
        public ushort RankBestMin
        {
            get { return rankBestMin; }
        }
        public ushort RankBestMax
        {
            get { return rankBestMax; }
        }
        public ushort RankAvgMin
        {
            get { return rankAvgMin; }
        }
        public ushort RankAvgMax
        {
            get { return rankAvgMax; }
        }
        public ushort RankStaMin
        {
            get { return rankStaMin; }
        }
        public ushort RankStaMax
        {
            get { return rankStaMax; }
        }
        public ushort RankWinMin
        {
            get { return rankWinMin; }
        }
        public ushort RankWinMax
        {
            get { return rankWinMax; }
        }
        public ushort RankTotalMin
        {
            get { return rankTotalMin; }
        }
        public ushort RankTotalMax
        {
            get { return rankTotalMax; }
        }
        public bool RankKick
        {
            get { return rankKick; }
        }
 
    }
    
    
    
    
    
    /*public sealed class DriverRank : Storage
    {
        public DriverRank(string[] tableTemplateFmt) : base(tableTemplateFmt) { }
        ~DriverRank()
        {
            if (true == false) { }
        }
        new public DriverRankInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new DriverRankInfo(_temp);
            else
                return null;
        }
        public DriverRankInfo GetCarPrefix(string carPrefix)
        {
            object[] _temp = base.GetCustom(1, carPrefix);
            if(_temp != null)
                return new DriverRankInfo(_temp);

            return null;
        }
    }
    public sealed class DriverRankInfo
    {
        public DriverRankInfo(object[] rowInfos)
        {
            entry = Convert.ToUInt32(rowInfos[0]);
            licenceName = (string)rowInfos[1];
            fromLicenceName = (string)rowInfos[2];
            reason = (string)rowInfos[3];
            startTime = Convert.ToUInt32(rowInfos[4]);
            endTime = Convert.ToUInt32(rowInfos[5]);
            expired = Convert.ToUInt16(rowInfos[6]) == 1 ? true : false;
        }
        ~DriverRankInfo()
        {
            if (true == false) { }
        }
        private uint entry;
        private string licenceName;
        private string fromLicenceName;
        private string reason;
        private uint startTime;
        private uint endTime;
        private bool expired;
    }*/
    
}
