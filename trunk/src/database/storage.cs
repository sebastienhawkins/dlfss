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

    public abstract class Storage
    {
        protected Storage(string[] tableFmt)
        {
            tableName = tableFmt[0];
            tableFormat = tableFmt[1].ToCharArray(); ;
        }
        private string tableName;
        private char[] tableFormat;
        private Dictionary<uint, object[]> data = new Dictionary<uint, object[]>();

        public bool Load()
        {
            bool returnValue = false;

            IDataReader reader = Program.dlfssDatabase.ExecuteQuery("SELECT * FROM `" + tableName + "` LIMIT 1");
            if (reader.Read())
            {
                if (reader.FieldCount == tableFormat.Length)
                {
                    
                    reader.Dispose();
                    reader = Program.dlfssDatabase.ExecuteQuery("SELECT * FROM `" + tableName + "`");

                    Log.commandHelp("  Loading Storage \"" + tableName + "\"\r\n");

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
                                    if (reader.GetFieldType(index) == typeof(UInt32) || reader.GetFieldType(index) == typeof(byte))
                                        value.Add(reader.IsDBNull(index) ? 0 : (uint)reader.GetInt32(index));
                                    else
                                        Log.error("\r\nUINT Unsuported Field Type For: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
                                    break;
                                case 'p':
                                    if (reader.GetFieldType(index) == typeof(UInt32))
                                    {
                                        value.Add(reader.IsDBNull(index) ? 0 : (uint)reader.GetInt32(index));
                                        dataIndex = (reader.IsDBNull(index) ? 0 : (uint)reader.GetInt32(index));
                                    }
                                    else
                                        Log.error("\r\nPrimary Key Unsuported Field Type For: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
                                    break;
                                case 'i':
                                    if (reader.GetFieldType(index) == typeof(Int32) || reader.GetFieldType(index) == typeof(byte))
                                        value.Add(reader.IsDBNull(index) ? 0 : reader.GetInt32(index));
                                    else
                                        Log.error("\r\nINT Unsuported Field Type For: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
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
                                        Log.error("\r\nUnsuported Field Type For: " + index + " FieldType is: " + reader.GetFieldType(index) + "\r\n");
                                    break;
                            }
                            valueIndex++;

                        }
                        data[dataIndex] = value.ToArray();

                    }
                    returnValue = true;
                }
                else
                    Log.error("Storage System, Database Table:'" + tableName + "', has a invalide field count, she has: " + reader.FieldCount + ", it should be at: " + tableFormat.Length + "\r\n");
            }
            else
                Log.error("Storage System, Database Table:'" + tableName + "', Is Empty.\r\n");

            reader.Dispose();
            return returnValue;
        }
        protected virtual object[] GetEntry(uint entry)
        {
            if (data.ContainsKey(entry))
                return data[entry];
            return null;
        }
    }
    public class ButtonTemplate : Storage
    {
        public ButtonTemplate(string[] buttonTemplateFmt) : base(buttonTemplateFmt) { }
        new public ButtonTemplateInfo GetEntry(uint entry)
        {
            object[] _temp = base.GetEntry(entry);
            if (_temp != null)
                return new ButtonTemplateInfo(_temp);
            else
                return null;
        }
    }
    public class ButtonTemplateInfo
    {
        public ButtonTemplateInfo(object[] buttonInfos)
        {
            entry = Convert.ToUInt16(buttonInfos[0]);
            description = (string)(buttonInfos[1]);
            styleMask = (Button_Styles_Flag)Convert.ToUInt16(buttonInfos[2]);
            maxInputChar = (byte)Convert.ToUInt16(buttonInfos[3]);
            left = (byte)Convert.ToUInt16(buttonInfos[4]);
            top = (byte)Convert.ToUInt16(buttonInfos[5]);
            width = (byte)Convert.ToUInt16(buttonInfos[6]);
            height = (byte)Convert.ToUInt16(buttonInfos[7]);
            text = (string)buttonInfos[8];
        }
        private ushort entry;
        private string description;
        private Button_Styles_Flag styleMask;
        private byte maxInputChar;
        private byte left;
        private byte top;
        private byte width;
        private byte height;
        private string text;

        public ushort Entry
        {
            get { return entry; }
        }
        public string Description
        {
            get { return description; }
        }
        public Button_Styles_Flag StyleMask
        {
            get { return styleMask; }
        }
        public byte MaxInputChar
        {
            get { return maxInputChar; }
        }
        public byte Left
        {
            get { return left; }
        }
        public byte Top
        {
            get { return top; }
        }
        public byte Width
        {
            get { return width; }
        }
        public byte Height
        {
            get { return height; }
        }
        public string Text
        {
            get { return text; }
        }
    }
}
