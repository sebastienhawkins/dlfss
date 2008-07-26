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
    public class DBConnection
    {
        public IDbConnection dbCon;
        private IDbCommand dbCmd;
        private string connectionString ;

        public DBConnection( string nameDb )
        {
            connectionString = "URI=file:" + nameDb + ",version=3";
            dbCon = (IDbConnection)new SqliteConnection(connectionString);
            dbCon.Open();
            dbCmd = dbCon.CreateCommand();
            this.createDbs();
            this.alter(); // Todo : function do to an automatic alter of the dbs for the future extension
        }

        //Specific LFS action should not be mixed with the DB... i mean here should be only DB related question IMPLICITE
        public long retreiveRacerUID( string userName, string nickName ){
            IDataReader reader;
            while( true ){
                reader = executeQuery( "SELECT uid FROM uid_license WHERE username = '" + userName.ToLower() + "'"
                                        + " AND nickname = '" + nickName + "'" );
                if (reader.Read())
                    return reader.GetInt64( reader.GetOrdinal( "uid" ));
                else
                    executeNonQuery("INSERT INTO uid_license ( username,nickname ) VALUES ( '" + userName.ToLower() + "','" + nickName + "')");
            }
        }
        //Create a Initialisation Proc for the DB...
        private void createDbs()
        {
            if (!isTableExist("pb_set"))
            {
                executeNonQuery("CREATE TABLE pb_set( "
                                        + "setid INTEGER PRIMARY KEY"
                                        + ",setname VARCHAR( 24 )"
                                        + ")"
                );
                executeNonQuery("CREATE INDEX i_pb_set1 ON pb_set( setname )");
                Console.WriteLine("Create table pb_set");
            }
            if (!isTableExist("uid_license"))
            {
                executeNonQuery("CREATE TABLE uid_license( "
                                    + "uid INTEGER PRIMARY KEY"
                                    + ",username VARCHAR( 24 )"
                                    + ",nickname VARCHAR( 24 )"
                                    + ")"
                );
                executeNonQuery("CREATE INDEX i_uid_license1 ON uid_license( username )");
                executeNonQuery("CREATE INDEX i_uid_license2 ON uid_license( nickname )");
                Console.WriteLine("Create table uid_license");
            }
            if (!isTableExist("pb_racer"))
            {
                executeNonQuery("CREATE TABLE pb_racer( "
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
                executeNonQuery("CREATE UNIQUE INDEX i_pb_racer1 ON pb_racer( uid,setid,trackname,carname )");
                Console.WriteLine("Create table pb_racer");
            }
            if (!isTableExist("info_race"))
            {
                executeNonQuery("CREATE TABLE info_race( "
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
                Console.WriteLine("Create table info_race");
            }
            if (!isTableExist("lap_race"))
            {
                executeNonQuery("CREATE TABLE lap_race( "
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
                Console.WriteLine("Create table lap_race");
            }
        }
        private void alter(){

        }
        public IDataReader executeQuery(string sql)
        {
            dbCmd.CommandText = sql;
            return dbCmd.ExecuteReader();
        }
        public long executeNonQuery(string sql)
        {
            dbCmd.CommandText = sql;
            int i = dbCmd.ExecuteNonQuery();
            return i;
        }
        public bool isTableExist(string tableName)
        {

            dbCmd.CommandText = "SELECT name FROM SQLITE_MASTER WHERE type = 'table' AND name = '" + tableName + "'";
            IDataReader reader = dbCmd.ExecuteReader();
            while (reader.Read())
            {
                return true;
            }
            return false;
        }
        public bool isColumnExist(string tableName, string colName )
        {

            dbCmd.CommandText = "SELECT " + colName + " FROM " + tableName + " LIMIT 1";
            try
            {
                IDataReader reader = dbCmd.ExecuteReader();
                reader.Read(); 
            }
            catch { 
                return false; 
            };
            return true;
        }
        public bool isIndexExist(string indexName)
        {

            dbCmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'index' AND name = '" + indexName + "'";
            IDataReader reader = dbCmd.ExecuteReader();
            while (reader.Read())
            {
                return true;
            }
            return false;
        }
    }
}
