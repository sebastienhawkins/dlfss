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
using System.Text;
using System.Data;
using Mono.Data.SqliteClient;

namespace Drive_LFSS.Database_
{
    public sealed class DatabaseSQLite : Database, IDatabase
    {
        public DatabaseSQLite(string connectionInfo)
        {
            if (!System.IO.File.Exists("./" + connectionInfo))
                throw new Exception("SQLite Database can't be found, File: " + connectionInfo);

            connection = (IDbConnection)new SqliteConnection("URI=file:" + connectionInfo + ",version=3");
            connection.Open();
            transaction = null;

            command = (IDbCommand)new SqliteCommand();
            command.Connection = connection;
        }
        private IDbConnection connection = null;
        private IDbTransaction transaction;
        private IDbCommand command;

        public void CancelCommand()
        {
            try { command.Cancel(); }
            catch (Exception _exception) { }
        }
        public void NewTransaction()
        {
            transaction = connection.BeginTransaction(IsolationLevel.Serializable);
        }
        public void EndTransaction()
        {
            transaction.Commit();
            transaction.Dispose();
        }
        public bool IsExistTable(string tableName)
        {

            command.CommandText = "SELECT name FROM SQLITE_MASTER WHERE type = 'table' AND name = '" + tableName + "'";
            if (command.ExecuteReader().Read())
                return true;

            return false;
        }
        public bool IsExistColum(string tableName, string colName)
        {
            command.CommandText = "SELECT " + colName + " FROM " + tableName + " LIMIT 1";
            if(command.ExecuteReader().Read())
                return true;

            return false;
        }
        public uint GetLastRowId(string tableName)
        {
            IDataReader result = ExecuteQuery("SELECT MAX(ROWID) FROM `" + tableName + "`");
            if (result.Read())
                return (uint)result.GetInt32(0);

            return 0;
        }
        public IDataReader ExecuteQuery(string _command)
        {
            command.CommandText = _command;
            return command.ExecuteReader(CommandBehavior.SequentialAccess);
        }
        public int ExecuteNonQuery(string _command)
        {
            command.CommandText = _command;
            int i = command.ExecuteNonQuery();
            return i;
        }
    }
}
