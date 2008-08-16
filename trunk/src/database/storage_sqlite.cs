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
using System.Data;

//This is not usefull for the moment... since DB call are made from where they are needed for read/write access.
//This will become the Storage feature for Static Database Storage... for later usage... and will need rewrite offcourse.
namespace Drive_LFSS.Database_
{
    /*public static class DatabaseStorage
    {
        public static Race race;
        public static Driver driver;

        public static Dictionary<string,string> tableCheck = new Dictionary<string,string>();
        public static void Initialize()
        {
            tableCheck.Add("race", "SELECT `guid`,`entry_track`,`start_time`,`end_time`,`grid_order`,`start_car_count`,`end_car_count`,`start_connection_count`,`end_connection_count` FROM `race` LIMIT 1");
            tableCheck.Add("driver", "SELECT `guid`,`guid_licence`,`name` FROM `driver` LIMIT 1");
            race = new Race();
            driver = new Driver();
        }
    }

    public class Race : DatabaseSQLite
    {
        public Race() : base() 
        { 
            ExecuteNonQuery(DatabaseStorage.tableCheck["race"]);
            guidLast = GetLastRowId("race");
        }
        private uint guidLast;

        //This function is not complete
        public void Save(uint entry_track, uint start_time, uint end_time, string grid_order)
        {
        }
    }
    public class Driver : DatabaseSQLite
    {
        public Driver() : base()
        {
            ExecuteNonQuery(DatabaseStorage.tableCheck["driver"]);
            guidLast = GetLastRowId("driver");
        }

        private uint guidLast;
    }*/
}
