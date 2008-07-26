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
using System.Collections;
using Mono.Data.SqliteClient;
using Drive_LFSS.Packet_;


//This must be in fact into each Object into witch she belongue!

//if will PB will probaly be into Driver or Car, really depend on how we see a TOC! 
// PB follor Car when TOC occur so i think must be Driver! but not Licence!

// In fact PB will be loaded when Server Start, and then Treated from Memory, Saveds at Interval, but no Select Request
// Only INSERT, since we don't save PB, but LAP , so we COmpute PB at Load.. and Run it From memory! Not all Lap, but the PB.
namespace Drive_LFSS.Database_

{
   /* public class racerPB
    {
        public long uid;
        public string userName;
        public string nickName;
        public string CName;
        public string TName;
        public long laps = 0;
        public string PBDate = "";
        public string PBTime = "";
        public long PBLapTime = 0;
        public long PBSplit1 = 0;
        public long PBSplit2 = 0;
        public long PBSplit3 = 0;
        public long PBSectorSplit1 = 0;
        public long PBSectorSplit2 = 0;
        public long PBSectorSplit3 = 0;
        public long PBSectorSplitLast = 0;
    }
    class dbsSet
    {
        IDataReader reader;
        private string setName;
        private long setId;
        private static Database myDb;
        public dbsSet(Database pmyDb, string psetName)
        {


            this.myDb = pmyDb;
            this.setName = psetName;
            while (true)
            {
                reader = myDb.executeQuery("SELECT setid FROM pb_set WHERE setname = '" + setName + "'");
                if (reader.Read())
                {
                    setId = reader.GetInt64(reader.GetOrdinal("setid"));
                    break;
                }
                else
                {
                    myDb.executeNonQuery("INSERT INTO pb_set ( setname ) VALUES ( '" + setName + "')");
                }
            }


        }
        public racerPB retreiveRacerPB( string userName, string nickName, string CName, string TName )
        {
            racerPB currRacer = new racerPB( );
            long UID = myDb.retreiveRacerUID(userName, nickName);
            reader = myDb.executeQuery("SELECT * FROM pb_racer WHERE setid = " + setId 
                                        + " AND uid = " + UID
                                        + " AND carname = '" + CName + "'"
                                        + " AND trackname = '" + TName + "'"
                                        );
            currRacer.userName = userName;
            currRacer.nickName = nickName;
            if (!reader.Read())
            {
                currRacer.uid = UID;
                currRacer.CName = CName;
                currRacer.TName = TName;
            }
            else
            {
                currRacer.uid = UID;
                currRacer.CName = reader.GetString(reader.GetOrdinal("carname"));
                currRacer.TName = reader.GetString(reader.GetOrdinal("trackname"));
                currRacer.laps = reader.GetInt64(reader.GetOrdinal("laps"));
                currRacer.PBDate = reader.GetString(reader.GetOrdinal("pb_date"));
                currRacer.PBTime = reader.GetString(reader.GetOrdinal("pb_time"));
                currRacer.PBLapTime = reader.GetInt64(reader.GetOrdinal("pb_laptime"));
                currRacer.PBSplit1 = reader.GetInt64(reader.GetOrdinal("pb_split1"));
                currRacer.PBSplit2 = reader.GetInt64(reader.GetOrdinal("pb_split2"));
                currRacer.PBSplit3 = reader.GetInt64(reader.GetOrdinal("pb_split3"));
                currRacer.PBSectorSplit1 = reader.GetInt64(reader.GetOrdinal("pb_sector_split1"));
                currRacer.PBSectorSplit2 = reader.GetInt64(reader.GetOrdinal("pb_sector_split2"));
                currRacer.PBSectorSplit3 = reader.GetInt64(reader.GetOrdinal("pb_sector_split3"));
                currRacer.PBSectorSplitLast = reader.GetInt64(reader.GetOrdinal("pb_sector_split_last"));
            }
            return currRacer;

        }
        public void updateRacerPB( racerPB currRacer ){
            long updatedRow;

            if (currRacer.PBLapTime == 0)
                return;
            updatedRow = myDb.executeNonQuery( "UPDATE pb_racer SET "
                                            + " laps = " + currRacer.laps
                                            + ",pb_date = '" + currRacer.PBDate + "'"
                                            + ",pb_time = '" + currRacer.PBTime + "'"
                                            + ",pb_laptime = " + currRacer.PBLapTime
                                            + ",pb_split1 = " + currRacer.PBSplit1
                                            + ",pb_split2 = " + currRacer.PBSplit2
                                            + ",pb_split3 = " + currRacer.PBSplit3
                                            + ",pb_sector_split1 = " + currRacer.PBSectorSplit1
                                            + ",pb_sector_split2 = " + currRacer.PBSectorSplit2
                                            + ",pb_sector_split3 = " + currRacer.PBSectorSplit3
                                            + ",pb_sector_split_last = " + currRacer.PBSectorSplitLast
                                            + " WHERE setid = " + setId
                                            + " AND uid = " + currRacer.uid
                                            + " AND carname = '" + currRacer.CName + "'"
                                            + " AND trackname = '" + currRacer.TName + "'"

            );
            if (updatedRow == 0)
            {
                myDb.executeNonQuery("INSERT INTO pb_racer ("
                                                        + "setid"
                                                        + ",uid"
                                                        + ",carname"
                                                        + ",trackname"
                                                        + ",laps"
                                                        + ",pb_date"
                                                        + ",pb_time"
                                                        + ",pb_laptime"
                                                        + ",pb_split1"
                                                        + ",pb_split2"
                                                        + ",pb_split3"
                                                        + ",pb_sector_split1"
                                                        + ",pb_sector_split2"
                                                        + ",pb_sector_split3"
                                                        + ",pb_sector_split_last"
                                                    + ") VALUES ("
                                                        + "'" + setId + "'"
                                                        + "," + currRacer.uid
                                                        + ",'" + currRacer.CName + "'"
                                                        + ",'" + currRacer.TName + "'"
                                                        + "," + currRacer.laps
                                                        + ",'" + currRacer.PBDate + "'"
                                                        + ",'" + currRacer.PBTime + "'"
                                                        + "," + currRacer.PBLapTime
                                                        + "," + currRacer.PBSplit1
                                                        + "," + currRacer.PBSplit2
                                                        + "," + currRacer.PBSplit3
                                                        + "," + currRacer.PBSectorSplit1
                                                        + "," + currRacer.PBSectorSplit2
                                                        + "," + currRacer.PBSectorSplit3
                                                        + "," + currRacer.PBSectorSplitLast
                                                        + ")"
               );
            }
        }
        class raceData
        {
            public void newRace( PacketRST packet )
            {
            }
        }



        public bool ImportLapperPB(string filepath) // importation du fichier Lapper des PB
        {

            myDb.executeNonQuery("BEGIN TRANSACTION");
            myDb.executeNonQuery("DELETE FROM pb_racer WHERE setid = " + setId);
            using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
            {
                string userName, nickName, datePb, timePb, time, carName, trackName;
                int laps;
                long LTime;
                long[] PBsplit = new long[3];
                long[] PBBestSplitDiff = new long[3];
                long PBBestSplitDiffLast;

                string pbVersion = sr.ReadLine();
                if (pbVersion.IndexOf("USERNAME") == -1) // Si pas fichier avec Username, on efface et on recrer
                    return false;
                while (true)
                {
                    userName = sr.ReadLine();
                    nickName = sr.ReadLine();
                    try
                    {
                        laps = int.Parse(sr.ReadLine());
                    }
                    catch { laps = 0; }
                    datePb = sr.ReadLine();
                    timePb = sr.ReadLine();
                    if (nickName == null)
                        break;
                    carName = sr.ReadLine();
                    LTime = HMSToLong(sr.ReadLine());
                    trackName = sr.ReadLine();
                    PBsplit[0] = HMSToLong(sr.ReadLine());
                    PBsplit[1] = HMSToLong(sr.ReadLine());
                    PBsplit[2] = HMSToLong(sr.ReadLine());
                    PBBestSplitDiff[0] = HMSToLong(sr.ReadLine());
                    PBBestSplitDiff[1] = HMSToLong(sr.ReadLine());
                    PBBestSplitDiff[2] = HMSToLong(sr.ReadLine());
                    PBBestSplitDiffLast = HMSToLong(sr.ReadLine());
                    myDb.executeNonQuery("INSERT INTO pb_racer ("
                                + "setid"
                                + ",uid"
                                + ",carname"
                                + ",trackname"
                                + ",laps"
                                + ",pb_date"
                                + ",pb_time"
                                + ",pb_laptime"
                                + ",pb_split1"
                                + ",pb_split2"
                                + ",pb_split3"
                                + ",pb_sector_split1"
                                + ",pb_sector_split2"
                                + ",pb_sector_split3"
                                + ",pb_sector_split_last"
                            + ") VALUES ("
                                + "'" + setId + "'"
                                + "," + myDb.retreiveRacerUID(userName, nickName)
                                + ",'" + carName + "'"
                                + ",'" + trackName + "'"
                                + "," + laps
                                + ",'" + datePb + "'"
                                + ",'" + timePb + "'"
                                + "," + LTime
                                + "," + PBsplit[0]
                                + "," + PBsplit[1]
                                + "," + PBsplit[2]
                                + "," + PBBestSplitDiff[0]
                                + "," + PBBestSplitDiff[1]
                                + "," + PBBestSplitDiff[2]
                                + "," + PBBestSplitDiffLast
                            + ")");
                }
            }
            myDb.executeNonQuery("COMMIT TRANSACTION");

            return true;
        }
        public static long HMSToLong(int m, int s, int d)
        {
            return (long)((m * 60 * 100) + s * 100 + d) * 10;
        }
        public static long HMSToLong(string time)
        {
            try
            {
                string[] sp = time.Trim().Split('.');
                int m = Int32.Parse(sp[0]);
                int s = Int32.Parse(sp[1]);
                int d = Int32.Parse(sp[2]);
                if (time[0] == '-')
                {
                    m = Math.Abs(m);
                    return -HMSToLong(m, s, d);
                }
                else
                    return HMSToLong(m, s, d);
            }
            catch
            {
                return HMSToLong(0, 0, 0);
            }
        }
    }*/
}
