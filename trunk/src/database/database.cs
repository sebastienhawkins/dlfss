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

    //In fact connection should be a Object inside the ServerDatabase Object, so other object related to the DB
    //become Child of each other...
    public static class Database
    {
        private static IDbConnection connection;
        private static IDbTransaction transaction;  //TODO: Create In Transaction Query!
        private static IDbCommand command;

        //Create a Initialisation Proc for the DB...
        public static void Initialize()
        {
            //When config.cs is complete will take this value from there!
            connection = (IDbConnection)new SqliteConnection("URI=file:dlfss.sqlite,version=3");
            connection.Open();
            command = connection.CreateCommand();
            CreateEachTable();
        }
        //This is not good desing i think,
        //must be created from Array, Const, Config  or External SQL file, What ever and have just 1 Proc doing the job.
        //I think i got the idea, simply send the .db file with all the structure allready there! more simple and 
        //better for us when we change the DB.
        private static void CreateEachTable()
        {
            if (!IsExistTable("pb_set"))
            {
                Program.log.database("Creating Database Table pb_set...\r\n");
                ExecuteNonQuery("CREATE TABLE pb_set( "
                                        + "setid INTEGER PRIMARY KEY"
                                        + ",setname VARCHAR( 24 )"
                                        + ")"
                );
                ExecuteNonQuery("CREATE INDEX i_pb_set1 ON pb_set( setname )");
            }
            if (!IsExistTable("uid_license"))
            {
                Program.log.database("Creating Database Table uid_license...\r\n");
                ExecuteNonQuery("CREATE TABLE uid_license( "
                                    + "uid INTEGER PRIMARY KEY"
                                    + ",username VARCHAR( 24 )"
                                    + ",nickname VARCHAR( 24 )"
                                    + ")"
                );
                ExecuteNonQuery("CREATE INDEX i_uid_license1 ON uid_license( username )");
                ExecuteNonQuery("CREATE INDEX i_uid_license2 ON uid_license( nickname )");
            }
            if (!IsExistTable("pb_racer"))
            {
                Program.log.database("Creating Database Table pb_racer...\r\n");
                ExecuteNonQuery("CREATE TABLE pb_racer( "
                                    + "setid INTEGER(4)"
                                    + ",uid INTEGER(4)"
                                    + ",carname VARCHAR(4)"
                                    + ",trackname VARCHAR(4)"
                                    + ",laps INTEGER( 4 )"
                                    + ",pb_date VARCHAR(10)"
                                    + ",pb_time VARCHAR(6)"
                                    + ",pb_laptime INT(4)"
                                    + ",pb_split1 INT(4)"
                                    + ",pb_split2 INT(4)"
                                    + ",pb_split3 INT(4)"
                                    + ",pb_sector_split1 INT(4)"
                                    + ",pb_sector_split2 INT(4)"
                                    + ",pb_sector_split3 INT(4)"
                                    + ",pb_sector_split_last INT(4)"
                                    + ")");
                ExecuteNonQuery("CREATE UNIQUE INDEX i_pb_racer1 ON pb_racer( uid,setid,trackname,carname )");
            }
            if (!IsExistTable("info_race"))
            {
                Program.log.database("Creating Database Table info_race...\r\n");
                ExecuteNonQuery("CREATE TABLE info_race( "
                                    + "setid INTEGER(4)"
                                    + ",racid INTEGER PRIMARY KEY"
                                    + ",race_date INTEGER(10)"
                                    + ",race_time INTEGER(6)"
                                    + ",race_laps INTEGER(1)"
                                    + ",qual_mins INTEGER(1)"
                                    + ",num_player INTEGER(1)"
                                    + ",trackname VARCHAR(6)"
                                    + ",pb_time VARCHAR(6)"
                                    + ",weather INT(1)"
                                    + ",wind INT(1)"
                                    + ",flags INT(1)"
                                    + ")");
            }
            if (!IsExistTable("lap_race"))
            {
                Program.log.database("Creating Database Table lap_race...\r\n");
                ExecuteNonQuery("CREATE TABLE lap_race( "
                                    + "setid INTEGER(4)"
                                    + ",raceid INTEGER(4)"
                                    + ",uid INTEGER(4)"
                                    + ",split1 INTEGER(4)"
                                    + ",split2 INTEGER(4)"
                                    + ",split3 INTEGER(4)"
                                    + ",lap_time INTEGER(4)"
                                    + ",total_time INTEGER(4)"
                                    + ",laps_done INTEGER(2)"
                                    + ",player_flags INTEGER(2)"
                                    + ",penalty INTEGER(1)"
                                    + ",pits_stop INTEGER(1)"
                                    + ")");
            }
        }

        public static IDataReader ExecuteQuery(string _command)
        {
            command.CommandText = _command;
            return command.ExecuteReader();
        }
        public static long ExecuteNonQuery(string _command)
        {
            command.CommandText = _command;
            int i = command.ExecuteNonQuery();
            return i;
        }

        public static bool IsExistTable(string tableName)
        {

            command.CommandText = "SELECT name FROM SQLITE_MASTER WHERE type = 'table' AND name = '" + tableName + "'";
            IDataReader reader = command.ExecuteReader();
            if(reader.Read())
                return true;

            return false;
        }
        public static bool IsExistColum(string tableName, string colName)
        {
            //If a Try here and not for other... maybe will have to test it out...
            command.CommandText = "SELECT " + colName + " FROM " + tableName + " LIMIT 1";
            try
            {
                IDataReader reader = command.ExecuteReader();
                reader.Read(); 
            }
            catch { 
                return false; 
            };
            return true;
        }
        public static bool IsExistIndex(string indexName)
        {

            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'index' AND name = '" + indexName + "'";
            IDataReader reader = command.ExecuteReader();
            if (reader.Read())
                return true;

            return false;
        }

        //Specific LFS action should not be mixed with the DB... i mean here should be only DB related question IMPLICITE
        //So this Proc should goes into Player Connection, or something like this!
        //I will probaly create this into Licence.cs!
        public static long retreiveRacerUID(string userName, string nickName)
        {
            IDataReader reader;
            while (true)
            {
                reader = ExecuteQuery("SELECT uid FROM uid_license WHERE username = '" + userName.ToLower() + "'"
                                        + " AND nickname = '" + nickName + "'");
                if (reader.Read())
                    return reader.GetInt64(reader.GetOrdinal("uid"));
                else
                    ExecuteNonQuery("INSERT INTO uid_license ( username,nickname ) VALUES ( '" + userName.ToLower() + "','" + nickName + "')");
            }
        }
    }
}
