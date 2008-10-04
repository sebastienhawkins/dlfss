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

namespace Drive_LFSS
{
    //Real extention required .net 3.0 , since we don't need .net 3.0 for the rest , i prefer stay .net 2.0
    public static class ConvertX
    {
        public static string MSToString(uint msTime, string negativeColor, string positiveColor)
        {
            return MSToString((int)msTime,negativeColor, positiveColor);
        }
        public static string MSToString(int msTime,string negativeColor, string positiveColor)
        {
            string stringTime = "";
            bool isNegative = msTime < 0 ? true : false;
            msTime = Math.Abs(msTime);

            //Hours
            int _test = msTime / 3600000;
            if (_test > 0)
                stringTime += (_test > 9 ?_test.ToString():"0"+_test.ToString()) + ":";

            //Minute
            _test = msTime % 3600000 / 60000;
            stringTime += (_test < 10 ? "0" + _test.ToString() : _test.ToString())+ ":";
            
            //Seconde
            _test = msTime % 60000 / 1000;
            stringTime += (_test < 10 ? "0" + _test.ToString() : _test.ToString())+ ".";

            //Milieme
            _test = msTime % 1000;
            if (_test < 10)
                stringTime += "00" + _test.ToString();
            else if(_test < 100)
                stringTime += "0" + _test.ToString();
            else
                stringTime += _test.ToString();

            return isNegative ? negativeColor+"-" + stringTime : positiveColor+ stringTime;
        }
        public static double MSToKhm(double ms)
        {
            return ms * 3.624d;
        }
    }
}
    