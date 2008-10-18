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

        //Not Yet Thread Safe, They will need a Mutex.
        public void NewTransaction()
        {
            transaction = connection.BeginTransaction(IsolationLevel.Serializable);
        }
        public void EndTransaction()
        {
            transaction.Commit();
            transaction.Dispose();
        }
        public IAsyncResult NewExecuteNonQuery()
        {
            command.ExecuteNonQuery();
            return null;
        }
        public int EndExecuteNonQuery(IAsyncResult _iaSyncResult)
        {
            return 0;
        }
        public void Lock(){}
        public void Unlock(){}
        //Thread Safe
        public void CancelCommand()
        {
            try { lock (command) { command.Cancel(); } }
            catch (Exception) { }
        }
        public IDataReader ExecuteQuery(string _command)
        {
            IDataReader reader;
            lock (command)
            {
                command.CommandText = _command;
                reader = command.ExecuteReader(CommandBehavior.SequentialAccess);
            }
            return reader;
        }
        public int ExecuteNonQuery(string _command)
        {
            int i;
            lock (command)
            {
                command.CommandText = _command;
                i = command.ExecuteNonQuery();
            }
            return i;
        }
    }
}
