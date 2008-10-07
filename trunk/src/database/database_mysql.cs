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
using MySql.Data.MySqlClient;
using System.Threading;
/*
 *             string[] info = Config.GetStringValue("MySQL", "ConnectionInfo").Split(';');
            string connectionInfo = "Database=" + info[4] + ";Data Source=" + info[0] + ";Port=" + info[1] + ";User Id=" + info[2] + ";Password=" + info[3] + ";Use Compression=" + info[5];
 * */
namespace Drive_LFSS.Database_
{
    using Drive_LFSS.Config_;
    using Drive_LFSS.Log_;

    public sealed class DatabaseMySQL : Database, IDatabase
    {
        public DatabaseMySQL(string _connectionInfo)
        {
            connection = new MySqlConnection(_connectionInfo);
            connection.Open();

            command = connection.CreateCommand();
            transaction = null;
            Log.commandHelp("  MySQL using compression: " + ((MySqlConnection)connection).UseCompression + "\r\n");
        }
        private IDbConnection connection;
        private IDbCommand command;
        private IDbTransaction transaction;
        private Mutex mutexDataReader = new Mutex();
        private IDataReader dataReader = null;
        
        //Thread ???
        public void CancelCommand()
        {
            try { command.Cancel(); }
            catch (Exception) { }
        }
        public void Lock()
        {
            mutexDataReader.WaitOne();
            command.Dispose();
        }
        public void Unlock()
        {
            if(dataReader != null)
            {
                dataReader.Dispose();
                dataReader = null;
            }
            mutexDataReader.ReleaseMutex();
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
        public int ExecuteNonQuery(string _command)
        {
            int _return;
            command = connection.CreateCommand();
            command.CommandText = _command;
            ResetTimerKeepAlive();
            try{_return = command.ExecuteNonQuery(/*CommandBehavior.SequentialAccess*/);}
            catch(Exception exception)
            { 
                Log.error("MySQL Error with query: "+_command+". Exception was:"+exception.Message+"\r\n");
                return 0;
            }
            
            
            return _return;
        }
        public IDataReader ExecuteQuery(string _command)
        {
            
            command = connection.CreateCommand();
            command.CommandText = _command;
            ResetTimerKeepAlive();
            try{dataReader = command.ExecuteReader(/*CommandBehavior.SequentialAccess*/);}
            catch(Exception exception)
            { 
                Log.error("MySQL Error with query: "+_command+". Exception was:"+exception.Message+"\r\n");
                return new NullDataReader();
            }

            return dataReader;
        }

        //Thread ???
        /*public IAsyncResult NewExecuteNonQuery()
        {
            return command.BeginExecuteNonQuery();
        }
        public int EndExecuteNonQuery(IAsyncResult _iaSyncResult)
        {
            return command.EndExecuteNonQuery(_iaSyncResult);
        }*/


    }
}
