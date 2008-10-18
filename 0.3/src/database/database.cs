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
using System.Data;

namespace Drive_LFSS.Database_
{
    using Log_;
    public sealed class NullDataReader : IDataReader, IDataRecord, IDisposable
    {
        internal NullDataReader()
        {

        }
        ~NullDataReader()
        {
            if (true == false) { }
        }

        public void Close()
        {
        }
        public DataTable GetSchemaTable()
        {
            throw new Exception();
        }
        public bool NextResult()
        {
            return false;
        }
        public bool Read()
        {
            return false;
        }
        public void Dispose()
        {
        }
        public string GetName(int value) { return null; }
        public string GetDataTypeName(int value) { return null; }
        public string GetFieldType(int value) { return null; }

        public int Depth
        {
            get { return 0; }
        }
        public bool IsClosed
        {
            get { return true; }
        }
        public int RecordsAffected
        {
            get { return 0; }
        }
        public int FieldCount
        {
            get { return 0; }
        }

        public bool GetBoolean(int i)
        {
            return false;
        }
        public byte GetByte(int i)
        {
            return 0;
        }
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return 0;
        }
        public char GetChar(int i)
        {
            return ' ';
        }
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return 0;
        }
        public IDataReader GetData(int i)
        {
            return null;
        }
        public DateTime GetDateTime(int i)
        {
            return new DateTime();
        }
        public decimal GetDecimal(int i)
        {
            return 0.0M;
        }
        public double GetDouble(int i)
        {
            return 0.0d;
        }
        Type IDataRecord.GetFieldType(int i)
        {
            return typeof(Nullable);
        }
        public float GetFloat(int i)
        {
            return 0.0f;
        }
        public Guid GetGuid(int i)
        {
            return new Guid();
        }
        public short GetInt16(int i)
        {
            return 0;
        }
        public int GetInt32(int i)
        {
            return 0;
        }
        public long GetInt64(int i)
        {
            return 0;
        }
        public int GetOrdinal(string name)
        {
            return 0;
        }
        public string GetString(int i)
        {
            return "";
        }
        public object GetValue(int i)
        {
            return 0;
        }
        public int GetValues(object[] values)
        {
            return 0;
        }
        public bool IsDBNull(int i)
        {
            return false;
        }
        public object this[string name]
        {
            get { return null; }
        }
        public object this[int i]
        {
            get { return null; }
        }

    }
    public abstract class Database
    {
        private uint TimerKeepAlive = 0;
        public void update(uint diff)
        {
            TimerKeepAlive += diff;
            if (TimerKeepAlive > 840000) //25 Minute
            {
                TimerKeepAlive = 0;
                Log.progress("Ping database\r\n");
                ((IDatabase)this).Lock();
                {
                    ((IDatabase)this).ExecuteNonQuery("SELECT 1");
                }
                ((IDatabase)this).Unlock();

            }
        }
        protected void ResetTimerKeepAlive()
        {
            TimerKeepAlive = 0;
        }
    }
}
