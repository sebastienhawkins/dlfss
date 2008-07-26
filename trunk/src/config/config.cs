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
namespace Drive_LFSS.Config_
{
    //using System;
    //using System.Collections.Generic;
    //using System.Text;

    sealed class config
    {
        private const string configPath = ".\\config.cfg";

        // Delete the file if it exists.
       /* public config()
        {
            if (!File.Exists(configPath)) 
            {
                // Create the file.
                using (FileStream fileStream = File.Create(configPath)) 
                {
                    Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");

                    // AddToReceiveQueud some information to the file.
                    fileStream.Write(info, 0, info.Length);
                }
            }
            // Open the stream and read it back.
            using (FileStream fs = File.OpenRead(path))
            {
                byte[] b = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                while (fs.Read(b, 0, b.Length) > 0)
                {
                    Console.WriteLine(temp.GetString(b));
                }
            }
        }*/
    }
}
