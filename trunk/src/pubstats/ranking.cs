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
    
    public sealed class Ranking
    {
        public Ranking()
        {

        }
        ~Ranking()
        {
            if (true == false) { }
        }
        public static void ConfigApply()
        {
            
        }
        public static bool Initialize()
        {
            if(!LoadTop10())
                return false;
            
            isActived = true;
            return true;
        }
        public static bool LoadTop10()
        {
            Log.commandHelp("  Start loading Top10 data.\r\n");
            
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
                if(!top10ByTrackCar.ContainsKey(itrTrack))
                    top10ByTrackCar.Add(itrTrack,new Dictionary<string,List<string>>());

                foreach(string itrCar in carPrefix)
                {
                    query = "SELECT `licence_name`,`best_lap_rank`,`average_lap_rank`,`stability_rank`,`race_win_rank`,`total_rank` FROM `stats_rank_driver` WHERE `car_prefix`='"+itrCar+"' AND `track_prefix`='"+itrTrack+"' ORDER BY `total_rank` DESC LIMIT 10";
                    reader = Program.dlfssDatabase.ExecuteQuery(query);
                    while(reader.Read())
                    {
                        count++;
                        if(!top10ByTrackCar[itrTrack].ContainsKey(itrCar))
                            top10ByTrackCar[itrTrack].Add(itrCar,new List<string>());
                        
                        top10ByTrackCar[itrTrack][itrCar].Add(reader.GetString(0)+" "+reader.GetString(1)+" "+reader.GetString(2)+" "+reader.GetString(3)+" "+reader.GetString(4)+" "+reader.GetString(5));
                    }
                    reader.Dispose();
                }
            }
            
            if(count > 0)
            {
                Log.commandHelp("  Loaded "+count+" Top10\r\n");
                return true;
            }
            else
                return false;
        }
        
        //<track_prefix,car_prefix,licence_name best avg sta win total>
        private static Dictionary<string,Dictionary<string,List<string>>> top10ByTrackCar = new Dictionary<string,Dictionary<string,List<string>>>();  
        private static bool isActived = false;
        
        public static bool IsActived()
        {
            return isActived;
        }
        public static string GetRank(string trackPrefix, string carPrefix, string licenceName)
        {
            string data = "";
            string query = "SELECT `best_lap_rank`,`average_lap_rank`,`stability_rank`,`race_win_rank`,`total_rank` FROM `stats_rank_driver` WHERE `car_prefix`='"+carPrefix+"' AND `track_prefix`='"+trackPrefix+"' AND `licence_name`='"+licenceName+"'";
            IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
            if(reader.Read())
                data = reader.GetString(0)+" "+reader.GetString(1)+" "+reader.GetString(2)+" "+reader.GetString(3)+" "+reader.GetString(4);
            reader.Dispose();
            
            return data;
        }
        public static string GetTop10(string trackPrefix, string carPrefix)
        {
            if(!top10ByTrackCar.ContainsKey(trackPrefix) || !top10ByTrackCar[trackPrefix].ContainsKey(carPrefix))
                return "";
            return String.Join("\r\n",top10ByTrackCar[trackPrefix][carPrefix].ToArray());
        }

    }
}