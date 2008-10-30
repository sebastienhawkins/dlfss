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
using System.Timers;
using System.Threading;
using System.Data;
using System.Net;
using System.IO;
using System.Text;

namespace Drive_LFSS.Ranking_
{
    using Drive_LFSS.Database_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Definition_;
    public class Rank
    {
        internal Rank(short _bestLap, short _averageLap, short _stability, int _raceWin, int _total, uint _position, uint _changeMask)
        {
            bestLap = _bestLap;
            averageLap = _averageLap;
            stability = _stability;
            raceWin = _raceWin;
            total =_total;
            position = _position;
            changeMask = (Rank_Change_Mask)_changeMask;
            
            rankGuiString = new string[6];
            if ((changeMask & Rank_Change_Mask.PB_HIGH) == Rank_Change_Mask.PB_HIGH)
                rankGuiString[0] = "^3"+bestLap;
            else if ((changeMask & Rank_Change_Mask.PB_LOW) == Rank_Change_Mask.PB_LOW)
                rankGuiString[0] = "^6" + bestLap;
            else
                rankGuiString[0] = "^7" + bestLap;

            if ((changeMask & Rank_Change_Mask.AVG_HIGH) == Rank_Change_Mask.AVG_HIGH)
                rankGuiString[1] = "^3" + averageLap;
            else if ((changeMask & Rank_Change_Mask.AVG_LOW) == Rank_Change_Mask.AVG_LOW)
                rankGuiString[1] = "^6" + averageLap;
            else
                rankGuiString[1] = "^7" + averageLap;

            if ((changeMask & Rank_Change_Mask.STA_HIGH) == Rank_Change_Mask.STA_HIGH)
                rankGuiString[2] = "^3" + stability;
            else if ((changeMask & Rank_Change_Mask.STA_LOW) == Rank_Change_Mask.STA_LOW)
                rankGuiString[2] = "^6" + stability;
            else
                rankGuiString[2] = "^7" + stability;

            if ((changeMask & Rank_Change_Mask.WIN_HIGH) == Rank_Change_Mask.WIN_HIGH)
                rankGuiString[3] = "^3" + raceWin;
            else if ((changeMask & Rank_Change_Mask.WIN_LOW) == Rank_Change_Mask.WIN_LOW)
                rankGuiString[3] = "^6" + raceWin;
            else
                rankGuiString[3] = "^7" + raceWin;

            if ((changeMask & Rank_Change_Mask.TOTAL_HIGH) == Rank_Change_Mask.TOTAL_HIGH)
                rankGuiString[4] = "^3" + total;
            else if ((changeMask & Rank_Change_Mask.TOTAL_LOW) == Rank_Change_Mask.TOTAL_LOW)
                rankGuiString[4] = "^6" + total;
            else
                rankGuiString[4] = "^7" + total;

            if ((changeMask & Rank_Change_Mask.POSITION_LOW) == Rank_Change_Mask.POSITION_LOW)
                rankGuiString[5] = "^3" + position;
            else if ((changeMask & Rank_Change_Mask.POSITION_HIGH) == Rank_Change_Mask.POSITION_HIGH)
                rankGuiString[5] = "^6" + position;
            else
                rankGuiString[5] = "^7" + position;
        }
        ~Rank()
        {
            if(true == false) {}
        }

        short bestLap = 0;
        short averageLap = 0;
        short stability = 0;
        int raceWin = 0;
        int total = 0;
        uint position = 0;
        Rank_Change_Mask changeMask = Rank_Change_Mask.NONE;
        private string[] rankGuiString;

        public short BestLap
        {
            get { return bestLap; }
        }
        public short AverageLap
        {
            get { return averageLap; }
        }
        public short Stability
        {
            get { return stability; }
        }
        public int RaceWin
        {
            get { return raceWin; }
        }
        public int Total
        {
            get { return total; }
        }
        public uint Position
        {
            get{return position;}
        }
        public string GetGrade()
        {
            if (total > 14640)   // 17125 x
                return "a+";
            if (total > 13972)   // 15700 a+
                return "a";
            if (total > 13213)   // 14713 a
                return "a-";
            if (total > 12399)   // 14113 a-
                return "b";
            if (total > 11156)   // 13456 b+
                return "c";
            if (total > 10000)  // 12874 b
                return "d+";
            if (total > 9517)  // 12017 b-
                return "d";

            return "d-";
        }
        public string GetGradeComment()
        {
            if (total > 14640)   // 17125 x
                return "is the \"^7ultimate(" + GetGrade() + ")^2\" driver";
            if (total > 13972)   // 15700 a+
                return "is a \"^7superior(" + GetGrade() + ")^2\" driver";
            if (total > 13213)   // 14713 a
                return "is a \"^7execlent(" + GetGrade() + ")^2\" driver";
            if (total > 12399)   // 14113 a-
                return "is a \"^7fine(" + GetGrade() + ")^2\" driver";
            if (total > 11156)   // 13456 b+
                return "is a \"^7good(" + GetGrade() + ")\"^2 driver";
            if (total > 10000)  // 12874 b
                return "is a \"^7classic(" + GetGrade() + ")^2\" driver";
            if (total > 9517)  // 12017 b-
                return "is a \"^7sunday(" + GetGrade() + ")^2\" driver";

            return "is a \"^7slow(" + GetGrade() + ")^2\" driver";
        }
        public string[] GetRankGuiString
        {
            get{return rankGuiString;}
        }
    } 
    sealed class Ranking
    {
        public Ranking()
        {

        }
        ~Ranking()
        {
            if (true == false) { }
        }
        internal static bool Initialize()
        {
            return false;
            isActived = false;
            if(!LoadDriverRankedCount())
                return false;

            if(!LoadTop20())
                return false;

            isActived = true;
            return true;
        }
        private static bool LoadTop20()
        {
            Log.commandHelp("  Start loading Top20 data.\r\n");
            top20ByTrackCar.Clear();

            List<string> trackPrefix = new List<string>();
            string query = "SELECT `name_prefix` FROM `track_template`;";
            IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
            while(reader.Read())
                trackPrefix.Add(reader.GetString(0));
            reader.Dispose();
            
            List<string> carPrefix = new List<string>();
            query = "SELECT `name_prefix` FROM `car_template`;";
            reader = Program.dlfssDatabase.ExecuteQuery(query);
            while(reader.Read())
                carPrefix.Add(reader.GetString(0));
            reader.Dispose();
            
            uint count = 0;
            foreach(string itrTrack in trackPrefix)
            {
                if(!top20ByTrackCar.ContainsKey(itrTrack))
                    top20ByTrackCar.Add(itrTrack,new Dictionary<string,List<string>>());

                foreach(string itrCar in carPrefix)
                {
                    query = "SELECT `licence_name`,`best_lap_rank`,`average_lap_rank`,`stability_rank`,`race_win_rank`,`total_rank` FROM `stats_rank_driver` WHERE `car_prefix`='"+itrCar+"' AND `track_prefix`='"+itrTrack+"' ORDER BY `total_rank` DESC LIMIT 20";
                    reader = Program.dlfssDatabase.ExecuteQuery(query);
                    while(reader.Read())
                    {
                        count++;
                        if(!top20ByTrackCar[itrTrack].ContainsKey(itrCar))
                            top20ByTrackCar[itrTrack].Add(itrCar,new List<string>());

                        top20ByTrackCar[itrTrack][itrCar].Add(reader.GetString(0) + ((char)0) + reader.GetString(1) + ((char)0) + reader.GetString(2) + ((char)0) + reader.GetString(3) + ((char)0) + reader.GetString(4) + ((char)0) + reader.GetString(5));
                    }
                    reader.Dispose();
                }
            }
            
            if(count > 0)
            {
                Log.commandHelp("  Loaded "+count+" Top20\r\n");
                return true;
            }
            else
                return false;
        }
        private static bool LoadDriverRankedCount()
        {
            Log.commandHelp("  Start loading driver ranked count.\r\n");
            uint count = 0;
            driverRankedCount.Clear();

            List<string> trackPrefix = new List<string>();
            string query = "SELECT `name_prefix` FROM `track_template`;";
            Program.dlfssDatabase.Lock();
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
                while(reader.Read())
                    trackPrefix.Add(reader.GetString(0));
            }
            Program.dlfssDatabase.Unlock();
            
            List<string> carPrefix = new List<string>();
            query = "SELECT `name_prefix` FROM `car_template`;";
            Program.dlfssDatabase.Lock();
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
                while(reader.Read())
                    carPrefix.Add(reader.GetString(0));
                reader.Dispose();
            }
            Program.dlfssDatabase.Unlock();
            
            foreach(string itrTrack in trackPrefix)
            {
                if(!driverRankedCount.ContainsKey(itrTrack))
                    driverRankedCount.Add(itrTrack,new Dictionary<string,uint>());

                foreach(string itrCar in carPrefix)
                {
                    if(!driverRankedCount[itrTrack].ContainsKey(itrCar))
                        driverRankedCount[itrTrack].Add(itrCar,0);

                    query = "SELECT DISTINCT COUNT(`licence_name`) FROM `stats_rank_driver` WHERE `track_prefix`='"+itrTrack+"' AND `car_prefix`='"+itrCar+"'";
                    Program.dlfssDatabase.Lock();
                    {
                        IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
                        if(reader.Read())
                        {
                           driverRankedCount[itrTrack][itrCar] = (uint)reader.GetInt32(0);
                           count += driverRankedCount[itrTrack][itrCar];
                        }
                    }
                    Program.dlfssDatabase.Unlock();
                }
            }
            Log.commandHelp("  driver ranked count for each Track/Car "+count.ToString()+".\r\n");
            if(count == 0)
                return false;
            return true;
        }
        
        //<track_prefix,car_prefix,licence_name best avg sta win total>
        private static Dictionary<string,Dictionary<string,uint>> driverRankedCount = new Dictionary<string,Dictionary<string,uint>>();
        private static Dictionary<string,Dictionary<string,List<string>>> top20ByTrackCar = new Dictionary<string,Dictionary<string,List<string>>>();  
        private static bool isActived = false;
        
        internal static bool IsActived()
        {
            return isActived;
        }
        internal static Rank GetRank(string trackPrefix, string carPrefix, string licenceName)
        {
            Rank rank = null;
            string query = "SELECT `best_lap_rank`,`average_lap_rank`,`stability_rank`,`race_win_rank`,`total_rank`,`position`,`change_mask` FROM `stats_rank_driver` WHERE `car_prefix`='"+carPrefix+"' AND `track_prefix`='"+trackPrefix+"' AND `licence_name`LIKE'"+ConvertX.SQLString(licenceName)+"'";
            Program.dlfssDatabase.Lock();
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
                if(reader.Read())
                    rank = new Rank(reader.GetInt16(0),reader.GetInt16(1),reader.GetInt16(2),reader.GetInt32(3),reader.GetInt32(4),(uint)reader.GetInt32(5),(uint)reader.GetInt32(6));
            }
            Program.dlfssDatabase.Unlock();
            
            return rank;
        }
        internal static Dictionary<string,Dictionary<string,Rank>> GetDriverRanks(string licenceName)
        {
            Dictionary<string,Dictionary<string,Rank>> data = new Dictionary<string,Dictionary<string,Rank>>();

            string query = "SELECT `track_prefix`,`car_prefix`,`best_lap_rank`,`average_lap_rank`,`stability_rank`,`race_win_rank`,`total_rank`,`position`,`change_mask` FROM `stats_rank_driver` WHERE `licence_name`='" + ConvertX.SQLString(licenceName) + "'";
            string trackPrefix = "";
            string carPrefix = "";
            
            Program.dlfssDatabase.Lock();
            {
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
                while(reader.Read())
                {
                    trackPrefix = reader.GetString(0);
                    carPrefix = reader.GetString(1);

                    if(!data.ContainsKey(trackPrefix))
                        data.Add(trackPrefix,new Dictionary<string,Rank>());
                    
                    if(!data[trackPrefix].ContainsKey(carPrefix))
                        data[trackPrefix].Add(carPrefix,null);

                    data[trackPrefix][carPrefix] = new Rank(reader.GetInt16(2), reader.GetInt16(3), reader.GetInt16(4), reader.GetInt32(5), reader.GetInt32(6), (uint)reader.GetInt32(7), (uint)reader.GetInt32(8));
                }
            }
            Program.dlfssDatabase.Unlock();
            
            return data;
        }
        internal static string[] GetTop20(string trackPrefix, string carPrefix)
        {
            if(!top20ByTrackCar.ContainsKey(trackPrefix) || !top20ByTrackCar[trackPrefix].ContainsKey(carPrefix))
                return new string[0];
            return top20ByTrackCar[trackPrefix][carPrefix].ToArray();
        }
        internal static uint GetRankedCount(string trackPrefix, string carPrefix)
        {
            if(driverRankedCount.ContainsKey(trackPrefix) && driverRankedCount[trackPrefix].ContainsKey(carPrefix))
                return driverRankedCount[trackPrefix][carPrefix];
            return 0;
        }
    }
}