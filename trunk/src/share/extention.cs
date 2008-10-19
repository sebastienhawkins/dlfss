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
    sealed class ConvertX
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
        public static double MSToMph(double ms)
        {
            return ms * 2.251844d;
        }
        public static string SQLString(string value)
        {
            return value.Replace(@"\", @"\\").Replace(@"'", @"\'");
        }
        public static string RemoveColorCode(string value)
        {
            return value.Replace("^0","").Replace("^1", "").Replace("^2", "").
            Replace("^3", "").Replace("^4", "").Replace("^5", "").Replace("^6", "").
            Replace("^7", "").Replace("^8", "").Replace("^9", "");
        }
        public static string RemoveSpecialChar(string value)
        {
            char[] values = value.ToCharArray();
            string newValue = "";
            int maxItr = values.Length;
            for(int itr = 0; itr < maxItr; itr++)
            {
                byte charCode = (byte)values[itr];
                if( (charCode > 64 && charCode < 91) || (charCode > 96 && charCode < 123) ||  charCode == 32 )
                    newValue += values[itr];
                
            }
            return newValue;
        }
    }
}
    