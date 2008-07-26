namespace Drive_LFSS.CommandConsole_
{
    //using System;
    //using System.Collections.Generic;
    //using System.Text;

    sealed class CommandConsole
    {
        public static void Exit()
        {
            Program.Log.normal("Exiting Requested, Please Wait For All Thread Too Close...\n\r");
            Program.Exit();
        }

    }
}
