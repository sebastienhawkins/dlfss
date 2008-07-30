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
#define DATABASE_SQLITE
#if DATABASE_SQLITE

using System;
using System.Text;
using System.Data;
using Mono.Data.SqliteClient;

namespace Drive_LFSS.Database_
{
    public class DatabaseSQLite : Database
    {
        public DatabaseSQLite()
        {
            if (connection == null)
            {
                if (!System.IO.File.Exists("./dlfss.db"))
                    throw new Exception("SQLite Database can't be found, File: dlfss.db");
                connection = (IDbConnection)new SqliteConnection("URI=file:dlfss.db,version=3");
                connection.Open();
            }
            transaction = null;
            command = (IDbCommand)new SqliteCommand();
            command.Connection = connection;
        }
        private static IDbConnection connection = null;
        protected IDbTransaction transaction;  //TODO: Create In Transaction Query!
        protected IDbCommand command;

        protected bool IsExistTable(string tableName)
        {

            command.CommandText = "SELECT name FROM SQLITE_MASTER WHERE type = 'table' AND name = '" + tableName + "'";
            IDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return true;

            return false;
        }
        protected bool IsExistColum(string tableName, string colName)
        {
            //If a Try here and not for other... maybe will have to test it out...
            command.CommandText = "SELECT " + colName + " FROM " + tableName + " LIMIT 1";
            try
            {
                IDataReader reader = command.ExecuteReader();
                reader.Read();
            }
            catch
            {
                return false;
            };
            return true;
        }
        protected bool IsExistIndex(string indexName)
        {

            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'index' AND name = '" + indexName + "'";
            IDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return true;

            return false;
        }
        protected uint GetGuidLast(string _tableName)
        {
            IDataReader result = ExecuteQuery("SELECT MAX(`rowid`) FROM `" + _tableName + "`");
            if (result.Read())
                return (uint)result.GetInt32(0);

            return 0;
        }
        protected IDataReader ExecuteQuery(string _command)
        {
            command.CommandText = _command;
            return command.ExecuteReader();
        }
        protected long ExecuteNonQuery(string _command)
        {
            command.CommandText = _command;
            int i = command.ExecuteNonQuery();
            return i;
        }
    }
}
#endif