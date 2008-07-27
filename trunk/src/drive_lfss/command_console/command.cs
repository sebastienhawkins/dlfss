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
namespace Drive_LFSS.CommandConsole_
{
    using Drive_LFSS;
    //using System.Collections.Generic;
    //using System.Text;

    static class CommandConsole
    {
        public static void Exec(string _commandText)
        {
            switch (_commandText)
            {
                case "exit": Exit(); break;
                default:
                {
                    if (_commandText != " ")
                        Program.log.error("Unknow Command: " + _commandText + "\r\n");
                    break;
                }
            }
        }
        private static void Exit()
        {
            Program.log.normal("Exiting Requested, Please Wait For All Thread Too Close...\n\r");
            Program.Exit();
        }

    }
}
