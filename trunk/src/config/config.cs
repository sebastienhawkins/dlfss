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
