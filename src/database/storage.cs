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
