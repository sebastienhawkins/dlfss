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

namespace Drive_LFSS.Database_
{
    using Log_;
    public abstract class Database
    {
        private uint TimerKeepAlive = 0;
        public void update(uint diff)
        {
            TimerKeepAlive += diff;
            if (TimerKeepAlive > 840000) //25 Minute
            {
                TimerKeepAlive = 0;
                Log.progress("Ping database\r\n");
                ((IDatabase)this).ExecuteNonQuery("SELECT 1");

            }
        }
        protected void ResetTimerKeepAlive()
        {
            TimerKeepAlive = 0;
        }
    }
}
