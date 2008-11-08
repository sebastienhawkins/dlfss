﻿/*
    LFSLapper, Insim Race and qualification Manager for Live For Speed Game
    Copyright (C) 2007  Robert B. alias Gai-Luron and Monkster: lfsgailuron@free.fr

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.Text;

namespace Drive_LFSS.Codepage_
{
    public class CodePage
    {
        public static int[] EToUni = {
            8364,0,8218,0,8222,8230,8224,8225,0,8240,352,8249,346,356,381,377,0,8216,8217,8220,8221,8226,8211,8212,0,8482,353,8250,347,357,382,378,160,711,728,321,164,260,166,167,168,169,350,171,172,173,174,379,176,177,731,322,180,181,182,183,184,261,351,187,317,733,318,380,340,193,194,258,196,313,262,199,268,201,280,203,282,205,206,270,272,323,327,211,212,336,214,215,344,366,218,368,220,221,354,223,341,225,226,259,228,314,263,231,269,233,281,235,283,237,238,271,273,324,328,243,244,337,246,247,345,367,250,369,252,253,355,729};
        public static int[] CToUni = {
            1026,1027,8218,1107,8222,8230,8224,8225,8364,8240,1033,8249,1034,1036,1035,1039,1106,8216,8217,8220,8221,8226,8211,
            8212,0,8482,1113,8250,1114,1116,1115,1119,
            160,1038,1118,1032,164,1168,166,167,1025,169,1028,171,172,173,174,1031,176,177,1030,1110,
            1169,181,182,183,1105,8470,1108,187,1112,1029,1109,1111,1040,1041,1042,1043,1044,1045,1046,1047,
            1048,1049,1050,1051,1052,1053,1054,1055,1056,1057,
            1058,1059,1060,1061,1062,1063,1064,1065,1066,1067,
            /*220*/1068,1069,1070,1071,1072,1073,1074,1075,1076,1077,
            /*230*/1078,1079,1080,1081,1082,1083,1084,1085,1086,1087,
            /*240*/1088,1089,1090,1091,1092,1093,1094,1095,1096,1097,
            /*250*/1098,1099,1100,1101,1102,1103};
        public static int[] LToUni = {
            8364,0,8218,402,8222,8230,8224,8225,710,8240,352,8249,
            /*140*/338,0,381,0,0,8216,8217,8220,8221,8226,
            /*150*/8211,8212,732,8482,353,8250,339,0,382,376,
            /*160*/160,161,162,163,164,165,166,167,168,169,
            /*170*/170,171,172,173,174,175,176,177,178,179,
            /*180*/180,181,182,183,184,185,186,187,188,189,
            /*190*/190,191,192,193,194,195,196,197,198,199,
            /*200*/200,201,202,203,204,205,206,207,208,209,
            /*210*/210,211,212,213,214,215,216,217,218,219,
            /*220*/220,221,222,223,224,225,226,227,228,229,
            /*230*/230,231,232,233,234,235,236,237,238,239,
            /*240*/240,241,242,243,244,245,246,247,248,249,
            /*250*/250,251,252,253,254,255
        };
        public static int[] GToUni = {
            /*128*/8364,0,8218,402,8222,8230,8224,8225,0,8240,0,8249,
            0,0,0,0,0,8216,8217,8220,8221,8226,
            /*150*/8211,8212,0,8482,0,8250,0,0,0,0,
            /*160*/160,901,902,163,164,165,166,167,168,169,
            0,171,172,173,174,8213,176,177,178,179,
            /*180*/900,181,182,183,904,905,906,187,908,189,
            /*190*/910,911,912,913,914,915,916,917,918,919,
            /*200*/920,921,922,923,924,925,926,927,928,929,
            /*210*/0,931,932,933,934,935,936,937,938,939,
            /*220*/940,941,942,943,944,945,946,947,948,949,
            /*230*/950,951,952,953,954,955,956,957,958,959,
            /*240*/960,961,962,963,964,965,966,967,968,969,
            /*250*/970,971,972,973,974,0
        };
        public static int[] TToUni = {
            /*128*/8364,0,8218,402,8222,8230,8224,8225,710,8240,352,8249,
            /*140*/338,0,0,0,0,8216,8217,8220,8221,8226,
            /*150*/8211,8212,732,8482,353,8250,339,0,0,376,
            /*160*/160,161,162,163,164,165,166,167,168,169,
            /*170*/170,171,172,173,174,175,176,177,178,179,
            /*180*/180,181,182,183,184,185,186,187,188,189,
            /*190*/190,191,192,193,194,195,196,197,198,199,
            /*200*/200,201,202,203,204,205,206,207,286,209,
            /*210*/210,211,212,213,214,215,216,217,218,219,
            /*220*/220,304,350,223,224,225,226,227,228,229,
            /*230*/230,231,232,233,234,235,236,237,238,239,
            /*240*/287,241,242,243,244,245,246,247,248,249,
            /*250*/250,251,252,305,351,255
        };
        public static int[] BToUni = {
            /*128*/8364,0,8218,0,8222,8230,8224,8225,0,8240,0,8249,
            0,168,711,184,0,8216,8217,8220,8221,8226,
            8211,8212,0,8482,0,8250,0,175,731,0,
            /*160*/160,0,162,163,164,0,166,167,216,169,
            /*170*/342,171,172,173,174,198,176,177,178,179,
            /*180*/180,181,182,183,248,185,343,187,188,189,
            /*190*/190,230,260,302,256,262,196,197,280,274,
            /*200*/268,201,377,278,290,310,298,315,352,323,
            /*210*/325,211,332,213,214,215,370,321,346,362,
            /*220*/220,379,381,223,261,303,257,263,228,229,
            /*230*/281,275,269,233,378,279,291,311,299,316,
            /*240*/353,324,326,243,333,245,246,247,371,322,
            /*250*/347,363,252,380,382,729
        };
        public static int[] JToUni = {
            /*128*/8364,0,0,0,0,8230,8224,8225,169,163,0,8249,
            0,0,0,0,0,0,0,0,0,8226,
            0,0,174,8482,127,8250,177,185,178,179,
            0,65377,65378,65379,65380,65381,65382,65383,65384,65385,
            /*170*/65386,65387,65388,65389,65390,65391,65392,65393,65394,65395,
            /*180*/65396,65397,65398,65399,65400,65401,65402,65403,65404,65405,
            /*190*/65406,65407,65408,65409,65410,65411,65412,65413,65414,65415,
            /*200*/65416,65417,65418,65419,65420,65421,65422,65423,65424,65425,
            /*210*/65426,65427,65428,65429,65430,65431,65432,65433,65434,65435,
            /*220*/65436,65437,65438,65439,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0
        };

        /// <summary>
        /// Gets unicode string from LFS encoded string (bytes).
        /// </summary>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static string GetString(byte[] pack)
        {
            return GetString(pack, 0, pack.Length);
        }

        /// <summary>
        /// Gets unicode string from LFS encoded string (bytes).
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="offset"></param>
        /// <param name="len"></param>
        /// <returns></returns>

        public static string chr(char charset, int val)
        {
            int uniVal = 32;
            switch (charset)
            {
                case 'E': //east european
                    uniVal = EToUni[val - 128];
                    break;
                case 'C': //cyrillic
                    uniVal = CToUni[val - 128];
                    break;
                case 'L':
                    uniVal = LToUni[val - 128];
                    break;
                case 'G':
                    uniVal = GToUni[val - 128];
                    break;
                case 'T':
                    uniVal = TToUni[val - 128];
                    break;
                case 'B':
                    uniVal = BToUni[val - 128];
                    break;
                case 'J':
                    uniVal = JToUni[val - 128];
                    break;
                default:
                    throw new System.Exception("Unknown codepage in chr");

            }
            byte[] conv = new byte[2];
            conv[1] = (byte)(uniVal / 256);
            conv[0] = (byte)(uniVal % 256);
            return System.Text.Encoding.Unicode.GetString(conv);
        }

        public static string GetString(byte[] pack, int offset, int len)
        {
            string temp = "";
            char codepage = 'L';
            bool specchar = false;

            for (int i = offset; i < offset + len; i++)
            {
                if (pack[i] == 0)
                    break;
                else if (pack[i] == '^')
                {
                    temp += (char)pack[i];
                    specchar = true;
                }
                else
                    if (pack[i] > 127)
                    {
                        //System.Console.WriteLine(pack[i]);

                        switch (codepage)
                        {
                            case 'E': //east european
                                temp += (char)EToUni[pack[i] - 128];
                                break;
                            case 'C': //cyrillic
                                temp += (char)CToUni[pack[i] - 128];
                                break;
                            case 'L':
                                temp += (char)LToUni[pack[i] - 128];
                                break;
                            case 'G':
                                temp += (char)GToUni[pack[i] - 128];
                                break;
                            case 'T':
                                temp += (char)TToUni[pack[i] - 128];
                                break;
                            case 'B':
                                temp += (char)BToUni[pack[i] - 128];
                                break;
                            case 'J':
                                temp += (char)JToUni[pack[i] - 128];
                                break;
                            default:
                                throw new System.Exception("Unknown codepage");
                        }
                    }
                    else
                    {
                        if (specchar)
                        {
                            switch (pack[i])
                            {
                                case (byte)'E':
                                case (byte)'C':
                                case (byte)'L':
                                case (byte)'G':
                                case (byte)'T':
                                case (byte)'B':
                                case (byte)'J':
                                    codepage = (char)pack[i];
                                    break;

                                default: //might be just color
                                    break;
                            }

                            specchar = false;
                        }

                        temp += (char)pack[i];
                    }
            }

            return temp;
        }

        public static void GetBytes(string value, int startIndex, int charCount, byte[] targetBytes, int targetIndex)
        {
            /*
             *      case 'E':
                    case 'C':
                    case 'L':
                    case 'G':
                    case 'T':
                    case 'B':
                    case 'J':*/
            char codepage = 'L';
            bool specchar = false;

            for (int i = startIndex; i < startIndex + charCount; i++)
            {
                if (value[i] == '^')
                {
                    targetBytes[targetIndex++] = (byte)value[i];
                    specchar = true;
                }
                else if (value[i] > 127)
                {
                    switch (codepage)
                    {
                        case 'E':
                            switch ((int)value[i])
                            {
                                case 8364: targetBytes[targetIndex++] = 128; break;
                                case 8218: targetBytes[targetIndex++] = 130; break;
                                case 8222: targetBytes[targetIndex++] = 132; break;
                                case 8230: targetBytes[targetIndex++] = 133; break;
                                case 8224: targetBytes[targetIndex++] = 134; break;
                                case 8225: targetBytes[targetIndex++] = 135; break;
                                case 8240: targetBytes[targetIndex++] = 137; break;
                                case 352: targetBytes[targetIndex++] = 138; break;
                                case 8249: targetBytes[targetIndex++] = 139; break;
                                case 346: targetBytes[targetIndex++] = 140; break;
                                case 356: targetBytes[targetIndex++] = 141; break;
                                case 381: targetBytes[targetIndex++] = 142; break;
                                case 377: targetBytes[targetIndex++] = 143; break;
                                case 8216: targetBytes[targetIndex++] = 145; break;
                                case 8217: targetBytes[targetIndex++] = 146; break;
                                case 8220: targetBytes[targetIndex++] = 147; break;
                                case 8221: targetBytes[targetIndex++] = 148; break;
                                case 8226: targetBytes[targetIndex++] = 149; break;
                                case 8211: targetBytes[targetIndex++] = 150; break;
                                case 8212: targetBytes[targetIndex++] = 151; break;
                                case 8482: targetBytes[targetIndex++] = 153; break;
                                case 353: targetBytes[targetIndex++] = 154; break;
                                case 8250: targetBytes[targetIndex++] = 155; break;
                                case 347: targetBytes[targetIndex++] = 156; break;
                                case 357: targetBytes[targetIndex++] = 157; break;
                                case 382: targetBytes[targetIndex++] = 158; break;
                                case 378: targetBytes[targetIndex++] = 159; break;
                                case 160: targetBytes[targetIndex++] = 160; break;
                                case 711: targetBytes[targetIndex++] = 161; break;
                                case 728: targetBytes[targetIndex++] = 162; break;
                                case 321: targetBytes[targetIndex++] = 163; break;
                                case 164: targetBytes[targetIndex++] = 164; break;
                                case 260: targetBytes[targetIndex++] = 165; break;
                                case 166: targetBytes[targetIndex++] = 166; break;
                                case 167: targetBytes[targetIndex++] = 167; break;
                                case 168: targetBytes[targetIndex++] = 168; break;
                                case 169: targetBytes[targetIndex++] = 169; break;
                                case 350: targetBytes[targetIndex++] = 170; break;
                                case 171: targetBytes[targetIndex++] = 171; break;
                                case 172: targetBytes[targetIndex++] = 172; break;
                                case 173: targetBytes[targetIndex++] = 173; break;
                                case 174: targetBytes[targetIndex++] = 174; break;
                                case 379: targetBytes[targetIndex++] = 175; break;
                                case 176: targetBytes[targetIndex++] = 176; break;
                                case 177: targetBytes[targetIndex++] = 177; break;
                                case 731: targetBytes[targetIndex++] = 178; break;
                                case 322: targetBytes[targetIndex++] = 179; break;
                                case 180: targetBytes[targetIndex++] = 180; break;
                                case 181: targetBytes[targetIndex++] = 181; break;
                                case 182: targetBytes[targetIndex++] = 182; break;
                                case 183: targetBytes[targetIndex++] = 183; break;
                                case 184: targetBytes[targetIndex++] = 184; break;
                                case 261: targetBytes[targetIndex++] = 185; break;
                                case 351: targetBytes[targetIndex++] = 186; break;
                                case 187: targetBytes[targetIndex++] = 187; break;
                                case 317: targetBytes[targetIndex++] = 188; break;
                                case 733: targetBytes[targetIndex++] = 189; break;
                                case 318: targetBytes[targetIndex++] = 190; break;
                                case 380: targetBytes[targetIndex++] = 191; break;
                                case 340: targetBytes[targetIndex++] = 192; break;
                                case 193: targetBytes[targetIndex++] = 193; break;
                                case 194: targetBytes[targetIndex++] = 194; break;
                                case 258: targetBytes[targetIndex++] = 195; break;
                                case 196: targetBytes[targetIndex++] = 196; break;
                                case 313: targetBytes[targetIndex++] = 197; break;
                                case 262: targetBytes[targetIndex++] = 198; break;
                                case 199: targetBytes[targetIndex++] = 199; break;
                                case 268: targetBytes[targetIndex++] = 200; break;
                                case 201: targetBytes[targetIndex++] = 201; break;
                                case 280: targetBytes[targetIndex++] = 202; break;
                                case 203: targetBytes[targetIndex++] = 203; break;
                                case 282: targetBytes[targetIndex++] = 204; break;
                                case 205: targetBytes[targetIndex++] = 205; break;
                                case 206: targetBytes[targetIndex++] = 206; break;
                                case 270: targetBytes[targetIndex++] = 207; break;
                                case 272: targetBytes[targetIndex++] = 208; break;
                                case 323: targetBytes[targetIndex++] = 209; break;
                                case 327: targetBytes[targetIndex++] = 210; break;
                                case 211: targetBytes[targetIndex++] = 211; break;
                                case 212: targetBytes[targetIndex++] = 212; break;
                                case 336: targetBytes[targetIndex++] = 213; break;
                                case 214: targetBytes[targetIndex++] = 214; break;
                                case 215: targetBytes[targetIndex++] = 215; break;
                                case 344: targetBytes[targetIndex++] = 216; break;
                                case 366: targetBytes[targetIndex++] = 217; break;
                                case 218: targetBytes[targetIndex++] = 218; break;
                                case 368: targetBytes[targetIndex++] = 219; break;
                                case 220: targetBytes[targetIndex++] = 220; break;
                                case 221: targetBytes[targetIndex++] = 221; break;
                                case 354: targetBytes[targetIndex++] = 222; break;
                                case 223: targetBytes[targetIndex++] = 223; break;
                                case 341: targetBytes[targetIndex++] = 224; break;
                                case 225: targetBytes[targetIndex++] = 225; break;
                                case 226: targetBytes[targetIndex++] = 226; break;
                                case 259: targetBytes[targetIndex++] = 227; break;
                                case 228: targetBytes[targetIndex++] = 228; break;
                                case 314: targetBytes[targetIndex++] = 229; break;
                                case 263: targetBytes[targetIndex++] = 230; break;
                                case 231: targetBytes[targetIndex++] = 231; break;
                                case 269: targetBytes[targetIndex++] = 232; break;
                                case 233: targetBytes[targetIndex++] = 233; break;
                                case 281: targetBytes[targetIndex++] = 234; break;
                                case 235: targetBytes[targetIndex++] = 235; break;
                                case 283: targetBytes[targetIndex++] = 236; break;
                                case 237: targetBytes[targetIndex++] = 237; break;
                                case 238: targetBytes[targetIndex++] = 238; break;
                                case 271: targetBytes[targetIndex++] = 239; break;
                                case 273: targetBytes[targetIndex++] = 240; break;
                                case 324: targetBytes[targetIndex++] = 241; break;
                                case 328: targetBytes[targetIndex++] = 242; break;
                                case 243: targetBytes[targetIndex++] = 243; break;
                                case 244: targetBytes[targetIndex++] = 244; break;
                                case 337: targetBytes[targetIndex++] = 245; break;
                                case 246: targetBytes[targetIndex++] = 246; break;
                                case 247: targetBytes[targetIndex++] = 247; break;
                                case 345: targetBytes[targetIndex++] = 248; break;
                                case 367: targetBytes[targetIndex++] = 249; break;
                                case 250: targetBytes[targetIndex++] = 250; break;
                                case 369: targetBytes[targetIndex++] = 251; break;
                                case 252: targetBytes[targetIndex++] = 252; break;
                                case 253: targetBytes[targetIndex++] = 253; break;
                                case 355: targetBytes[targetIndex++] = 254; break;
                                case 729: targetBytes[targetIndex++] = 255; break;

                                default:
                                    targetBytes[targetIndex++] = (byte)'?';
                                    break;
                                //throw new System.Exception("Could not convert from unicode to CodePage E");
                            }
                            break;

                        case 'C':
                            switch ((int)value[i])
                            {
                                case 1026: targetBytes[targetIndex++] = 128; break;
                                case 1027: targetBytes[targetIndex++] = 129; break;
                                case 8218: targetBytes[targetIndex++] = 130; break;
                                case 1107: targetBytes[targetIndex++] = 131; break;
                                case 8222: targetBytes[targetIndex++] = 132; break;
                                case 8230: targetBytes[targetIndex++] = 133; break;
                                case 8224: targetBytes[targetIndex++] = 134; break;
                                case 8225: targetBytes[targetIndex++] = 135; break;
                                case 8364: targetBytes[targetIndex++] = 136; break;
                                case 8240: targetBytes[targetIndex++] = 137; break;
                                case 1033: targetBytes[targetIndex++] = 138; break;
                                case 8249: targetBytes[targetIndex++] = 139; break;
                                case 1034: targetBytes[targetIndex++] = 140; break;
                                case 1036: targetBytes[targetIndex++] = 141; break;
                                case 1035: targetBytes[targetIndex++] = 142; break;
                                case 1039: targetBytes[targetIndex++] = 143; break;
                                case 1106: targetBytes[targetIndex++] = 144; break;
                                case 8216: targetBytes[targetIndex++] = 145; break;
                                case 8217: targetBytes[targetIndex++] = 146; break;
                                case 8220: targetBytes[targetIndex++] = 147; break;
                                case 8221: targetBytes[targetIndex++] = 148; break;
                                case 8226: targetBytes[targetIndex++] = 149; break;
                                case 8211: targetBytes[targetIndex++] = 150; break;
                                case 8212: targetBytes[targetIndex++] = 151; break;
                                case 8482: targetBytes[targetIndex++] = 153; break;
                                case 1113: targetBytes[targetIndex++] = 154; break;
                                case 8250: targetBytes[targetIndex++] = 155; break;
                                case 1114: targetBytes[targetIndex++] = 156; break;
                                case 1116: targetBytes[targetIndex++] = 157; break;
                                case 1115: targetBytes[targetIndex++] = 158; break;
                                case 1119: targetBytes[targetIndex++] = 159; break;
                                case 160: targetBytes[targetIndex++] = 160; break;
                                case 1038: targetBytes[targetIndex++] = 161; break;
                                case 1118: targetBytes[targetIndex++] = 162; break;
                                case 1032: targetBytes[targetIndex++] = 163; break;
                                case 164: targetBytes[targetIndex++] = 164; break;
                                case 1168: targetBytes[targetIndex++] = 165; break;
                                case 166: targetBytes[targetIndex++] = 166; break;
                                case 167: targetBytes[targetIndex++] = 167; break;
                                case 1025: targetBytes[targetIndex++] = 168; break;
                                case 169: targetBytes[targetIndex++] = 169; break;
                                case 1028: targetBytes[targetIndex++] = 170; break;
                                case 171: targetBytes[targetIndex++] = 171; break;
                                case 172: targetBytes[targetIndex++] = 172; break;
                                case 173: targetBytes[targetIndex++] = 173; break;
                                case 174: targetBytes[targetIndex++] = 174; break;
                                case 1031: targetBytes[targetIndex++] = 175; break;
                                case 176: targetBytes[targetIndex++] = 176; break;
                                case 177: targetBytes[targetIndex++] = 177; break;
                                case 1030: targetBytes[targetIndex++] = 178; break;
                                case 1110: targetBytes[targetIndex++] = 179; break;
                                case 1169: targetBytes[targetIndex++] = 180; break;
                                case 181: targetBytes[targetIndex++] = 181; break;
                                case 182: targetBytes[targetIndex++] = 182; break;
                                case 183: targetBytes[targetIndex++] = 183; break;
                                case 1105: targetBytes[targetIndex++] = 184; break;
                                case 8470: targetBytes[targetIndex++] = 185; break;
                                case 1108: targetBytes[targetIndex++] = 186; break;
                                case 187: targetBytes[targetIndex++] = 187; break;
                                case 1112: targetBytes[targetIndex++] = 188; break;
                                case 1029: targetBytes[targetIndex++] = 189; break;
                                case 1109: targetBytes[targetIndex++] = 190; break;
                                case 1111: targetBytes[targetIndex++] = 191; break;
                                case 1040: targetBytes[targetIndex++] = 192; break;
                                case 1041: targetBytes[targetIndex++] = 193; break;
                                case 1042: targetBytes[targetIndex++] = 194; break;
                                case 1043: targetBytes[targetIndex++] = 195; break;
                                case 1044: targetBytes[targetIndex++] = 196; break;
                                case 1045: targetBytes[targetIndex++] = 197; break;
                                case 1046: targetBytes[targetIndex++] = 198; break;
                                case 1047: targetBytes[targetIndex++] = 199; break;
                                case 1048: targetBytes[targetIndex++] = 200; break;
                                case 1049: targetBytes[targetIndex++] = 201; break;
                                case 1050: targetBytes[targetIndex++] = 202; break;
                                case 1051: targetBytes[targetIndex++] = 203; break;
                                case 1052: targetBytes[targetIndex++] = 204; break;
                                case 1053: targetBytes[targetIndex++] = 205; break;
                                case 1054: targetBytes[targetIndex++] = 206; break;
                                case 1055: targetBytes[targetIndex++] = 207; break;
                                case 1056: targetBytes[targetIndex++] = 208; break;
                                case 1057: targetBytes[targetIndex++] = 209; break;
                                case 1058: targetBytes[targetIndex++] = 210; break;
                                case 1059: targetBytes[targetIndex++] = 211; break;
                                case 1060: targetBytes[targetIndex++] = 212; break;
                                case 1061: targetBytes[targetIndex++] = 213; break;
                                case 1062: targetBytes[targetIndex++] = 214; break;
                                case 1063: targetBytes[targetIndex++] = 215; break;
                                case 1064: targetBytes[targetIndex++] = 216; break;
                                case 1065: targetBytes[targetIndex++] = 217; break;
                                case 1066: targetBytes[targetIndex++] = 218; break;
                                case 1067: targetBytes[targetIndex++] = 219; break;
                                case 1068: targetBytes[targetIndex++] = 220; break;
                                case 1069: targetBytes[targetIndex++] = 221; break;
                                case 1070: targetBytes[targetIndex++] = 222; break;
                                case 1071: targetBytes[targetIndex++] = 223; break;
                                case 1072: targetBytes[targetIndex++] = 224; break;
                                case 1073: targetBytes[targetIndex++] = 225; break;
                                case 1074: targetBytes[targetIndex++] = 226; break;
                                case 1075: targetBytes[targetIndex++] = 227; break;
                                case 1076: targetBytes[targetIndex++] = 228; break;
                                case 1077: targetBytes[targetIndex++] = 229; break;
                                case 1078: targetBytes[targetIndex++] = 230; break;
                                case 1079: targetBytes[targetIndex++] = 231; break;
                                case 1080: targetBytes[targetIndex++] = 232; break;
                                case 1081: targetBytes[targetIndex++] = 233; break;
                                case 1082: targetBytes[targetIndex++] = 234; break;
                                case 1083: targetBytes[targetIndex++] = 235; break;
                                case 1084: targetBytes[targetIndex++] = 236; break;
                                case 1085: targetBytes[targetIndex++] = 237; break;
                                case 1086: targetBytes[targetIndex++] = 238; break;
                                case 1087: targetBytes[targetIndex++] = 239; break;
                                case 1088: targetBytes[targetIndex++] = 240; break;
                                case 1089: targetBytes[targetIndex++] = 241; break;
                                case 1090: targetBytes[targetIndex++] = 242; break;
                                case 1091: targetBytes[targetIndex++] = 243; break;
                                case 1092: targetBytes[targetIndex++] = 244; break;
                                case 1093: targetBytes[targetIndex++] = 245; break;
                                case 1094: targetBytes[targetIndex++] = 246; break;
                                case 1095: targetBytes[targetIndex++] = 247; break;
                                case 1096: targetBytes[targetIndex++] = 248; break;
                                case 1097: targetBytes[targetIndex++] = 249; break;
                                case 1098: targetBytes[targetIndex++] = 250; break;
                                case 1099: targetBytes[targetIndex++] = 251; break;
                                case 1100: targetBytes[targetIndex++] = 252; break;
                                case 1101: targetBytes[targetIndex++] = 253; break;
                                case 1102: targetBytes[targetIndex++] = 254; break;
                                case 1103: targetBytes[targetIndex++] = 255; break;

                                default:
                                    targetBytes[targetIndex++] = (byte)'?';
                                    break;
                                //throw new System.Exception("Could not convert from unicode to CodePage E");
                            }
                            break;

                        case 'L':
                            switch ((int)value[i])
                            {
                                case 8364: targetBytes[targetIndex++] = 128; break;
                                case 8218: targetBytes[targetIndex++] = 130; break;
                                case 402: targetBytes[targetIndex++] = 131; break;
                                case 8222: targetBytes[targetIndex++] = 132; break;
                                case 8230: targetBytes[targetIndex++] = 133; break;
                                case 8224: targetBytes[targetIndex++] = 134; break;
                                case 8225: targetBytes[targetIndex++] = 135; break;
                                case 710: targetBytes[targetIndex++] = 136; break;
                                case 8240: targetBytes[targetIndex++] = 137; break;
                                case 352: targetBytes[targetIndex++] = 138; break;
                                case 8249: targetBytes[targetIndex++] = 139; break;
                                case 338: targetBytes[targetIndex++] = 140; break;
                                case 381: targetBytes[targetIndex++] = 142; break;
                                case 8216: targetBytes[targetIndex++] = 145; break;
                                case 8217: targetBytes[targetIndex++] = 146; break;
                                case 8220: targetBytes[targetIndex++] = 147; break;
                                case 8221: targetBytes[targetIndex++] = 148; break;
                                case 8226: targetBytes[targetIndex++] = 149; break;
                                case 8211: targetBytes[targetIndex++] = 150; break;
                                case 8212: targetBytes[targetIndex++] = 151; break;
                                case 732: targetBytes[targetIndex++] = 152; break;
                                case 8482: targetBytes[targetIndex++] = 153; break;
                                case 353: targetBytes[targetIndex++] = 154; break;
                                case 8250: targetBytes[targetIndex++] = 155; break;
                                case 339: targetBytes[targetIndex++] = 156; break;
                                case 382: targetBytes[targetIndex++] = 158; break;
                                case 376: targetBytes[targetIndex++] = 159; break;
                                case 160: targetBytes[targetIndex++] = 160; break;
                                case 161: targetBytes[targetIndex++] = 161; break;
                                case 162: targetBytes[targetIndex++] = 162; break;
                                case 163: targetBytes[targetIndex++] = 163; break;
                                case 164: targetBytes[targetIndex++] = 164; break;
                                case 165: targetBytes[targetIndex++] = 165; break;
                                case 166: targetBytes[targetIndex++] = 166; break;
                                case 167: targetBytes[targetIndex++] = 167; break;
                                case 168: targetBytes[targetIndex++] = 168; break;
                                case 169: targetBytes[targetIndex++] = 169; break;
                                case 170: targetBytes[targetIndex++] = 170; break;
                                case 171: targetBytes[targetIndex++] = 171; break;
                                case 172: targetBytes[targetIndex++] = 172; break;
                                case 173: targetBytes[targetIndex++] = 173; break;
                                case 174: targetBytes[targetIndex++] = 174; break;
                                case 175: targetBytes[targetIndex++] = 175; break;
                                case 176: targetBytes[targetIndex++] = 176; break;
                                case 177: targetBytes[targetIndex++] = 177; break;
                                case 178: targetBytes[targetIndex++] = 178; break;
                                case 179: targetBytes[targetIndex++] = 179; break;
                                case 180: targetBytes[targetIndex++] = 180; break;
                                case 181: targetBytes[targetIndex++] = 181; break;
                                case 182: targetBytes[targetIndex++] = 182; break;
                                case 183: targetBytes[targetIndex++] = 183; break;
                                case 184: targetBytes[targetIndex++] = 184; break;
                                case 185: targetBytes[targetIndex++] = 185; break;
                                case 186: targetBytes[targetIndex++] = 186; break;
                                case 187: targetBytes[targetIndex++] = 187; break;
                                case 188: targetBytes[targetIndex++] = 188; break;
                                case 189: targetBytes[targetIndex++] = 189; break;
                                case 190: targetBytes[targetIndex++] = 190; break;
                                case 191: targetBytes[targetIndex++] = 191; break;
                                case 192: targetBytes[targetIndex++] = 192; break;
                                case 193: targetBytes[targetIndex++] = 193; break;
                                case 194: targetBytes[targetIndex++] = 194; break;
                                case 195: targetBytes[targetIndex++] = 195; break;
                                case 196: targetBytes[targetIndex++] = 196; break;
                                case 197: targetBytes[targetIndex++] = 197; break;
                                case 198: targetBytes[targetIndex++] = 198; break;
                                case 199: targetBytes[targetIndex++] = 199; break;
                                case 200: targetBytes[targetIndex++] = 200; break;
                                case 201: targetBytes[targetIndex++] = 201; break;
                                case 202: targetBytes[targetIndex++] = 202; break;
                                case 203: targetBytes[targetIndex++] = 203; break;
                                case 204: targetBytes[targetIndex++] = 204; break;
                                case 205: targetBytes[targetIndex++] = 205; break;
                                case 206: targetBytes[targetIndex++] = 206; break;
                                case 207: targetBytes[targetIndex++] = 207; break;
                                case 208: targetBytes[targetIndex++] = 208; break;
                                case 209: targetBytes[targetIndex++] = 209; break;
                                case 210: targetBytes[targetIndex++] = 210; break;
                                case 211: targetBytes[targetIndex++] = 211; break;
                                case 212: targetBytes[targetIndex++] = 212; break;
                                case 213: targetBytes[targetIndex++] = 213; break;
                                case 214: targetBytes[targetIndex++] = 214; break;
                                case 215: targetBytes[targetIndex++] = 215; break;
                                case 216: targetBytes[targetIndex++] = 216; break;
                                case 217: targetBytes[targetIndex++] = 217; break;
                                case 218: targetBytes[targetIndex++] = 218; break;
                                case 219: targetBytes[targetIndex++] = 219; break;
                                case 220: targetBytes[targetIndex++] = 220; break;
                                case 221: targetBytes[targetIndex++] = 221; break;
                                case 222: targetBytes[targetIndex++] = 222; break;
                                case 223: targetBytes[targetIndex++] = 223; break;
                                case 224: targetBytes[targetIndex++] = 224; break;
                                case 225: targetBytes[targetIndex++] = 225; break;
                                case 226: targetBytes[targetIndex++] = 226; break;
                                case 227: targetBytes[targetIndex++] = 227; break;
                                case 228: targetBytes[targetIndex++] = 228; break;
                                case 229: targetBytes[targetIndex++] = 229; break;
                                case 230: targetBytes[targetIndex++] = 230; break;
                                case 231: targetBytes[targetIndex++] = 231; break;
                                case 232: targetBytes[targetIndex++] = 232; break;
                                case 233: targetBytes[targetIndex++] = 233; break;
                                case 234: targetBytes[targetIndex++] = 234; break;
                                case 235: targetBytes[targetIndex++] = 235; break;
                                case 236: targetBytes[targetIndex++] = 236; break;
                                case 237: targetBytes[targetIndex++] = 237; break;
                                case 238: targetBytes[targetIndex++] = 238; break;
                                case 239: targetBytes[targetIndex++] = 239; break;
                                case 240: targetBytes[targetIndex++] = 240; break;
                                case 241: targetBytes[targetIndex++] = 241; break;
                                case 242: targetBytes[targetIndex++] = 242; break;
                                case 243: targetBytes[targetIndex++] = 243; break;
                                case 244: targetBytes[targetIndex++] = 244; break;
                                case 245: targetBytes[targetIndex++] = 245; break;
                                case 246: targetBytes[targetIndex++] = 246; break;
                                case 247: targetBytes[targetIndex++] = 247; break;
                                case 248: targetBytes[targetIndex++] = 248; break;
                                case 249: targetBytes[targetIndex++] = 249; break;
                                case 250: targetBytes[targetIndex++] = 250; break;
                                case 251: targetBytes[targetIndex++] = 251; break;
                                case 252: targetBytes[targetIndex++] = 252; break;
                                case 253: targetBytes[targetIndex++] = 253; break;
                                case 254: targetBytes[targetIndex++] = 254; break;
                                case 255: targetBytes[targetIndex++] = 255; break;

                                default:
                                    targetBytes[targetIndex++] = (byte)'?';
                                    break;
                                //throw new System.Exception("Could not convert from unicode to CodePage E");
                            }
                            break;
                        case 'G':
                            switch ((int)value[i])
                            {
                                case 8364: targetBytes[targetIndex++] = 128; break;
                                case 8218: targetBytes[targetIndex++] = 130; break;
                                case 402: targetBytes[targetIndex++] = 131; break;
                                case 8222: targetBytes[targetIndex++] = 132; break;
                                case 8230: targetBytes[targetIndex++] = 133; break;
                                case 8224: targetBytes[targetIndex++] = 134; break;
                                case 8225: targetBytes[targetIndex++] = 135; break;
                                case 8240: targetBytes[targetIndex++] = 137; break;
                                case 8249: targetBytes[targetIndex++] = 139; break;
                                case 8216: targetBytes[targetIndex++] = 145; break;
                                case 8217: targetBytes[targetIndex++] = 146; break;
                                case 8220: targetBytes[targetIndex++] = 147; break;
                                case 8221: targetBytes[targetIndex++] = 148; break;
                                case 8226: targetBytes[targetIndex++] = 149; break;
                                case 8211: targetBytes[targetIndex++] = 150; break;
                                case 8212: targetBytes[targetIndex++] = 151; break;
                                case 8482: targetBytes[targetIndex++] = 153; break;
                                case 8250: targetBytes[targetIndex++] = 155; break;
                                case 160: targetBytes[targetIndex++] = 160; break;
                                case 901: targetBytes[targetIndex++] = 161; break;
                                case 902: targetBytes[targetIndex++] = 162; break;
                                case 163: targetBytes[targetIndex++] = 163; break;
                                case 164: targetBytes[targetIndex++] = 164; break;
                                case 165: targetBytes[targetIndex++] = 165; break;
                                case 166: targetBytes[targetIndex++] = 166; break;
                                case 167: targetBytes[targetIndex++] = 167; break;
                                case 168: targetBytes[targetIndex++] = 168; break;
                                case 169: targetBytes[targetIndex++] = 169; break;
                                case 171: targetBytes[targetIndex++] = 171; break;
                                case 172: targetBytes[targetIndex++] = 172; break;
                                case 173: targetBytes[targetIndex++] = 173; break;
                                case 174: targetBytes[targetIndex++] = 174; break;
                                case 8213: targetBytes[targetIndex++] = 175; break;
                                case 176: targetBytes[targetIndex++] = 176; break;
                                case 177: targetBytes[targetIndex++] = 177; break;
                                case 178: targetBytes[targetIndex++] = 178; break;
                                case 179: targetBytes[targetIndex++] = 179; break;
                                case 900: targetBytes[targetIndex++] = 180; break;
                                case 181: targetBytes[targetIndex++] = 181; break;
                                case 182: targetBytes[targetIndex++] = 182; break;
                                case 183: targetBytes[targetIndex++] = 183; break;
                                case 904: targetBytes[targetIndex++] = 184; break;
                                case 905: targetBytes[targetIndex++] = 185; break;
                                case 906: targetBytes[targetIndex++] = 186; break;
                                case 187: targetBytes[targetIndex++] = 187; break;
                                case 908: targetBytes[targetIndex++] = 188; break;
                                case 189: targetBytes[targetIndex++] = 189; break;
                                case 910: targetBytes[targetIndex++] = 190; break;
                                case 911: targetBytes[targetIndex++] = 191; break;
                                case 912: targetBytes[targetIndex++] = 192; break;
                                case 913: targetBytes[targetIndex++] = 193; break;
                                case 914: targetBytes[targetIndex++] = 194; break;
                                case 915: targetBytes[targetIndex++] = 195; break;
                                case 916: targetBytes[targetIndex++] = 196; break;
                                case 917: targetBytes[targetIndex++] = 197; break;
                                case 918: targetBytes[targetIndex++] = 198; break;
                                case 919: targetBytes[targetIndex++] = 199; break;
                                case 920: targetBytes[targetIndex++] = 200; break;
                                case 921: targetBytes[targetIndex++] = 201; break;
                                case 922: targetBytes[targetIndex++] = 202; break;
                                case 923: targetBytes[targetIndex++] = 203; break;
                                case 924: targetBytes[targetIndex++] = 204; break;
                                case 925: targetBytes[targetIndex++] = 205; break;
                                case 926: targetBytes[targetIndex++] = 206; break;
                                case 927: targetBytes[targetIndex++] = 207; break;
                                case 928: targetBytes[targetIndex++] = 208; break;
                                case 929: targetBytes[targetIndex++] = 209; break;
                                case 931: targetBytes[targetIndex++] = 211; break;
                                case 932: targetBytes[targetIndex++] = 212; break;
                                case 933: targetBytes[targetIndex++] = 213; break;
                                case 934: targetBytes[targetIndex++] = 214; break;
                                case 935: targetBytes[targetIndex++] = 215; break;
                                case 936: targetBytes[targetIndex++] = 216; break;
                                case 937: targetBytes[targetIndex++] = 217; break;
                                case 938: targetBytes[targetIndex++] = 218; break;
                                case 939: targetBytes[targetIndex++] = 219; break;
                                case 940: targetBytes[targetIndex++] = 220; break;
                                case 941: targetBytes[targetIndex++] = 221; break;
                                case 942: targetBytes[targetIndex++] = 222; break;
                                case 943: targetBytes[targetIndex++] = 223; break;
                                case 944: targetBytes[targetIndex++] = 224; break;
                                case 945: targetBytes[targetIndex++] = 225; break;
                                case 946: targetBytes[targetIndex++] = 226; break;
                                case 947: targetBytes[targetIndex++] = 227; break;
                                case 948: targetBytes[targetIndex++] = 228; break;
                                case 949: targetBytes[targetIndex++] = 229; break;
                                case 950: targetBytes[targetIndex++] = 230; break;
                                case 951: targetBytes[targetIndex++] = 231; break;
                                case 952: targetBytes[targetIndex++] = 232; break;
                                case 953: targetBytes[targetIndex++] = 233; break;
                                case 954: targetBytes[targetIndex++] = 234; break;
                                case 955: targetBytes[targetIndex++] = 235; break;
                                case 956: targetBytes[targetIndex++] = 236; break;
                                case 957: targetBytes[targetIndex++] = 237; break;
                                case 958: targetBytes[targetIndex++] = 238; break;
                                case 959: targetBytes[targetIndex++] = 239; break;
                                case 960: targetBytes[targetIndex++] = 240; break;
                                case 961: targetBytes[targetIndex++] = 241; break;
                                case 962: targetBytes[targetIndex++] = 242; break;
                                case 963: targetBytes[targetIndex++] = 243; break;
                                case 964: targetBytes[targetIndex++] = 244; break;
                                case 965: targetBytes[targetIndex++] = 245; break;
                                case 966: targetBytes[targetIndex++] = 246; break;
                                case 967: targetBytes[targetIndex++] = 247; break;
                                case 968: targetBytes[targetIndex++] = 248; break;
                                case 969: targetBytes[targetIndex++] = 249; break;
                                case 970: targetBytes[targetIndex++] = 250; break;
                                case 971: targetBytes[targetIndex++] = 251; break;
                                case 972: targetBytes[targetIndex++] = 252; break;
                                case 973: targetBytes[targetIndex++] = 253; break;
                                case 974: targetBytes[targetIndex++] = 254; break;

                                default:
                                    targetBytes[targetIndex++] = (byte)'?';
                                    break;
                                //throw new System.Exception("Could not convert from unicode to CodePage E");
                            }
                            break;
                        case 'T':
                            switch ((int)value[i])
                            {
                                case 8364: targetBytes[targetIndex++] = 128; break;
                                case 8218: targetBytes[targetIndex++] = 130; break;
                                case 402: targetBytes[targetIndex++] = 131; break;
                                case 8222: targetBytes[targetIndex++] = 132; break;
                                case 8230: targetBytes[targetIndex++] = 133; break;
                                case 8224: targetBytes[targetIndex++] = 134; break;
                                case 8225: targetBytes[targetIndex++] = 135; break;
                                case 710: targetBytes[targetIndex++] = 136; break;
                                case 8240: targetBytes[targetIndex++] = 137; break;
                                case 352: targetBytes[targetIndex++] = 138; break;
                                case 8249: targetBytes[targetIndex++] = 139; break;
                                case 338: targetBytes[targetIndex++] = 140; break;
                                case 8216: targetBytes[targetIndex++] = 145; break;
                                case 8217: targetBytes[targetIndex++] = 146; break;
                                case 8220: targetBytes[targetIndex++] = 147; break;
                                case 8221: targetBytes[targetIndex++] = 148; break;
                                case 8226: targetBytes[targetIndex++] = 149; break;
                                case 8211: targetBytes[targetIndex++] = 150; break;
                                case 8212: targetBytes[targetIndex++] = 151; break;
                                case 732: targetBytes[targetIndex++] = 152; break;
                                case 8482: targetBytes[targetIndex++] = 153; break;
                                case 353: targetBytes[targetIndex++] = 154; break;
                                case 8250: targetBytes[targetIndex++] = 155; break;
                                case 339: targetBytes[targetIndex++] = 156; break;
                                case 376: targetBytes[targetIndex++] = 159; break;
                                case 160: targetBytes[targetIndex++] = 160; break;
                                case 161: targetBytes[targetIndex++] = 161; break;
                                case 162: targetBytes[targetIndex++] = 162; break;
                                case 163: targetBytes[targetIndex++] = 163; break;
                                case 164: targetBytes[targetIndex++] = 164; break;
                                case 165: targetBytes[targetIndex++] = 165; break;
                                case 166: targetBytes[targetIndex++] = 166; break;
                                case 167: targetBytes[targetIndex++] = 167; break;
                                case 168: targetBytes[targetIndex++] = 168; break;
                                case 169: targetBytes[targetIndex++] = 169; break;
                                case 170: targetBytes[targetIndex++] = 170; break;
                                case 171: targetBytes[targetIndex++] = 171; break;
                                case 172: targetBytes[targetIndex++] = 172; break;
                                case 173: targetBytes[targetIndex++] = 173; break;
                                case 174: targetBytes[targetIndex++] = 174; break;
                                case 175: targetBytes[targetIndex++] = 175; break;
                                case 176: targetBytes[targetIndex++] = 176; break;
                                case 177: targetBytes[targetIndex++] = 177; break;
                                case 178: targetBytes[targetIndex++] = 178; break;
                                case 179: targetBytes[targetIndex++] = 179; break;
                                case 180: targetBytes[targetIndex++] = 180; break;
                                case 181: targetBytes[targetIndex++] = 181; break;
                                case 182: targetBytes[targetIndex++] = 182; break;
                                case 183: targetBytes[targetIndex++] = 183; break;
                                case 184: targetBytes[targetIndex++] = 184; break;
                                case 185: targetBytes[targetIndex++] = 185; break;
                                case 186: targetBytes[targetIndex++] = 186; break;
                                case 187: targetBytes[targetIndex++] = 187; break;
                                case 188: targetBytes[targetIndex++] = 188; break;
                                case 189: targetBytes[targetIndex++] = 189; break;
                                case 190: targetBytes[targetIndex++] = 190; break;
                                case 191: targetBytes[targetIndex++] = 191; break;
                                case 192: targetBytes[targetIndex++] = 192; break;
                                case 193: targetBytes[targetIndex++] = 193; break;
                                case 194: targetBytes[targetIndex++] = 194; break;
                                case 195: targetBytes[targetIndex++] = 195; break;
                                case 196: targetBytes[targetIndex++] = 196; break;
                                case 197: targetBytes[targetIndex++] = 197; break;
                                case 198: targetBytes[targetIndex++] = 198; break;
                                case 199: targetBytes[targetIndex++] = 199; break;
                                case 200: targetBytes[targetIndex++] = 200; break;
                                case 201: targetBytes[targetIndex++] = 201; break;
                                case 202: targetBytes[targetIndex++] = 202; break;
                                case 203: targetBytes[targetIndex++] = 203; break;
                                case 204: targetBytes[targetIndex++] = 204; break;
                                case 205: targetBytes[targetIndex++] = 205; break;
                                case 206: targetBytes[targetIndex++] = 206; break;
                                case 207: targetBytes[targetIndex++] = 207; break;
                                case 286: targetBytes[targetIndex++] = 208; break;
                                case 209: targetBytes[targetIndex++] = 209; break;
                                case 210: targetBytes[targetIndex++] = 210; break;
                                case 211: targetBytes[targetIndex++] = 211; break;
                                case 212: targetBytes[targetIndex++] = 212; break;
                                case 213: targetBytes[targetIndex++] = 213; break;
                                case 214: targetBytes[targetIndex++] = 214; break;
                                case 215: targetBytes[targetIndex++] = 215; break;
                                case 216: targetBytes[targetIndex++] = 216; break;
                                case 217: targetBytes[targetIndex++] = 217; break;
                                case 218: targetBytes[targetIndex++] = 218; break;
                                case 219: targetBytes[targetIndex++] = 219; break;
                                case 220: targetBytes[targetIndex++] = 220; break;
                                case 304: targetBytes[targetIndex++] = 221; break;
                                case 350: targetBytes[targetIndex++] = 222; break;
                                case 223: targetBytes[targetIndex++] = 223; break;
                                case 224: targetBytes[targetIndex++] = 224; break;
                                case 225: targetBytes[targetIndex++] = 225; break;
                                case 226: targetBytes[targetIndex++] = 226; break;
                                case 227: targetBytes[targetIndex++] = 227; break;
                                case 228: targetBytes[targetIndex++] = 228; break;
                                case 229: targetBytes[targetIndex++] = 229; break;
                                case 230: targetBytes[targetIndex++] = 230; break;
                                case 231: targetBytes[targetIndex++] = 231; break;
                                case 232: targetBytes[targetIndex++] = 232; break;
                                case 233: targetBytes[targetIndex++] = 233; break;
                                case 234: targetBytes[targetIndex++] = 234; break;
                                case 235: targetBytes[targetIndex++] = 235; break;
                                case 236: targetBytes[targetIndex++] = 236; break;
                                case 237: targetBytes[targetIndex++] = 237; break;
                                case 238: targetBytes[targetIndex++] = 238; break;
                                case 239: targetBytes[targetIndex++] = 239; break;
                                case 287: targetBytes[targetIndex++] = 240; break;
                                case 241: targetBytes[targetIndex++] = 241; break;
                                case 242: targetBytes[targetIndex++] = 242; break;
                                case 243: targetBytes[targetIndex++] = 243; break;
                                case 244: targetBytes[targetIndex++] = 244; break;
                                case 245: targetBytes[targetIndex++] = 245; break;
                                case 246: targetBytes[targetIndex++] = 246; break;
                                case 247: targetBytes[targetIndex++] = 247; break;
                                case 248: targetBytes[targetIndex++] = 248; break;
                                case 249: targetBytes[targetIndex++] = 249; break;
                                case 250: targetBytes[targetIndex++] = 250; break;
                                case 251: targetBytes[targetIndex++] = 251; break;
                                case 252: targetBytes[targetIndex++] = 252; break;
                                case 305: targetBytes[targetIndex++] = 253; break;
                                case 351: targetBytes[targetIndex++] = 254; break;
                                case 255: targetBytes[targetIndex++] = 255; break;

                                default:
                                    targetBytes[targetIndex++] = (byte)'?';
                                    break;
                                //throw new System.Exception("Could not convert from unicode to CodePage E");
                            }
                            break;
                        case 'B':
                            switch ((int)value[i])
                            {
                                case 8364: targetBytes[targetIndex++] = 128; break;
                                case 8218: targetBytes[targetIndex++] = 130; break;
                                case 8222: targetBytes[targetIndex++] = 132; break;
                                case 8230: targetBytes[targetIndex++] = 133; break;
                                case 8224: targetBytes[targetIndex++] = 134; break;
                                case 8225: targetBytes[targetIndex++] = 135; break;
                                case 8240: targetBytes[targetIndex++] = 137; break;
                                case 8249: targetBytes[targetIndex++] = 139; break;
                                case 168: targetBytes[targetIndex++] = 141; break;
                                case 711: targetBytes[targetIndex++] = 142; break;
                                case 184: targetBytes[targetIndex++] = 143; break;
                                case 8216: targetBytes[targetIndex++] = 145; break;
                                case 8217: targetBytes[targetIndex++] = 146; break;
                                case 8220: targetBytes[targetIndex++] = 147; break;
                                case 8221: targetBytes[targetIndex++] = 148; break;
                                case 8226: targetBytes[targetIndex++] = 149; break;
                                case 8211: targetBytes[targetIndex++] = 150; break;
                                case 8212: targetBytes[targetIndex++] = 151; break;
                                case 8482: targetBytes[targetIndex++] = 153; break;
                                case 8250: targetBytes[targetIndex++] = 155; break;
                                case 175: targetBytes[targetIndex++] = 157; break;
                                case 731: targetBytes[targetIndex++] = 158; break;
                                case 160: targetBytes[targetIndex++] = 160; break;
                                case 162: targetBytes[targetIndex++] = 162; break;
                                case 163: targetBytes[targetIndex++] = 163; break;
                                case 164: targetBytes[targetIndex++] = 164; break;
                                case 166: targetBytes[targetIndex++] = 166; break;
                                case 167: targetBytes[targetIndex++] = 167; break;
                                case 216: targetBytes[targetIndex++] = 168; break;
                                case 169: targetBytes[targetIndex++] = 169; break;
                                case 342: targetBytes[targetIndex++] = 170; break;
                                case 171: targetBytes[targetIndex++] = 171; break;
                                case 172: targetBytes[targetIndex++] = 172; break;
                                case 173: targetBytes[targetIndex++] = 173; break;
                                case 174: targetBytes[targetIndex++] = 174; break;
                                case 198: targetBytes[targetIndex++] = 175; break;
                                case 176: targetBytes[targetIndex++] = 176; break;
                                case 177: targetBytes[targetIndex++] = 177; break;
                                case 178: targetBytes[targetIndex++] = 178; break;
                                case 179: targetBytes[targetIndex++] = 179; break;
                                case 180: targetBytes[targetIndex++] = 180; break;
                                case 181: targetBytes[targetIndex++] = 181; break;
                                case 182: targetBytes[targetIndex++] = 182; break;
                                case 183: targetBytes[targetIndex++] = 183; break;
                                case 248: targetBytes[targetIndex++] = 184; break;
                                case 185: targetBytes[targetIndex++] = 185; break;
                                case 343: targetBytes[targetIndex++] = 186; break;
                                case 187: targetBytes[targetIndex++] = 187; break;
                                case 188: targetBytes[targetIndex++] = 188; break;
                                case 189: targetBytes[targetIndex++] = 189; break;
                                case 190: targetBytes[targetIndex++] = 190; break;
                                case 230: targetBytes[targetIndex++] = 191; break;
                                case 260: targetBytes[targetIndex++] = 192; break;
                                case 302: targetBytes[targetIndex++] = 193; break;
                                case 256: targetBytes[targetIndex++] = 194; break;
                                case 262: targetBytes[targetIndex++] = 195; break;
                                case 196: targetBytes[targetIndex++] = 196; break;
                                case 197: targetBytes[targetIndex++] = 197; break;
                                case 280: targetBytes[targetIndex++] = 198; break;
                                case 274: targetBytes[targetIndex++] = 199; break;
                                case 268: targetBytes[targetIndex++] = 200; break;
                                case 201: targetBytes[targetIndex++] = 201; break;
                                case 377: targetBytes[targetIndex++] = 202; break;
                                case 278: targetBytes[targetIndex++] = 203; break;
                                case 290: targetBytes[targetIndex++] = 204; break;
                                case 310: targetBytes[targetIndex++] = 205; break;
                                case 298: targetBytes[targetIndex++] = 206; break;
                                case 315: targetBytes[targetIndex++] = 207; break;
                                case 352: targetBytes[targetIndex++] = 208; break;
                                case 323: targetBytes[targetIndex++] = 209; break;
                                case 325: targetBytes[targetIndex++] = 210; break;
                                case 211: targetBytes[targetIndex++] = 211; break;
                                case 332: targetBytes[targetIndex++] = 212; break;
                                case 213: targetBytes[targetIndex++] = 213; break;
                                case 214: targetBytes[targetIndex++] = 214; break;
                                case 215: targetBytes[targetIndex++] = 215; break;
                                case 370: targetBytes[targetIndex++] = 216; break;
                                case 321: targetBytes[targetIndex++] = 217; break;
                                case 346: targetBytes[targetIndex++] = 218; break;
                                case 362: targetBytes[targetIndex++] = 219; break;
                                case 220: targetBytes[targetIndex++] = 220; break;
                                case 379: targetBytes[targetIndex++] = 221; break;
                                case 381: targetBytes[targetIndex++] = 222; break;
                                case 223: targetBytes[targetIndex++] = 223; break;
                                case 261: targetBytes[targetIndex++] = 224; break;
                                case 303: targetBytes[targetIndex++] = 225; break;
                                case 257: targetBytes[targetIndex++] = 226; break;
                                case 263: targetBytes[targetIndex++] = 227; break;
                                case 228: targetBytes[targetIndex++] = 228; break;
                                case 229: targetBytes[targetIndex++] = 229; break;
                                case 281: targetBytes[targetIndex++] = 230; break;
                                case 275: targetBytes[targetIndex++] = 231; break;
                                case 269: targetBytes[targetIndex++] = 232; break;
                                case 233: targetBytes[targetIndex++] = 233; break;
                                case 378: targetBytes[targetIndex++] = 234; break;
                                case 279: targetBytes[targetIndex++] = 235; break;
                                case 291: targetBytes[targetIndex++] = 236; break;
                                case 311: targetBytes[targetIndex++] = 237; break;
                                case 299: targetBytes[targetIndex++] = 238; break;
                                case 316: targetBytes[targetIndex++] = 239; break;
                                case 353: targetBytes[targetIndex++] = 240; break;
                                case 324: targetBytes[targetIndex++] = 241; break;
                                case 326: targetBytes[targetIndex++] = 242; break;
                                case 243: targetBytes[targetIndex++] = 243; break;
                                case 333: targetBytes[targetIndex++] = 244; break;
                                case 245: targetBytes[targetIndex++] = 245; break;
                                case 246: targetBytes[targetIndex++] = 246; break;
                                case 247: targetBytes[targetIndex++] = 247; break;
                                case 371: targetBytes[targetIndex++] = 248; break;
                                case 322: targetBytes[targetIndex++] = 249; break;
                                case 347: targetBytes[targetIndex++] = 250; break;
                                case 363: targetBytes[targetIndex++] = 251; break;
                                case 252: targetBytes[targetIndex++] = 252; break;
                                case 380: targetBytes[targetIndex++] = 253; break;
                                case 382: targetBytes[targetIndex++] = 254; break;
                                case 729: targetBytes[targetIndex++] = 255; break;

                                default:
                                    targetBytes[targetIndex++] = (byte)'?';
                                    break;
                                //throw new System.Exception("Could not convert from unicode to CodePage E");
                            }
                            break;
                        case 'J':
                            switch ((int)value[i])
                            {
                                case 8364: targetBytes[targetIndex++] = 128; break;
                                case 8230: targetBytes[targetIndex++] = 133; break;
                                case 8224: targetBytes[targetIndex++] = 134; break;
                                case 8225: targetBytes[targetIndex++] = 135; break;
                                case 169: targetBytes[targetIndex++] = 136; break;
                                case 163: targetBytes[targetIndex++] = 137; break;
                                case 8249: targetBytes[targetIndex++] = 139; break;
                                case 8226: targetBytes[targetIndex++] = 149; break;
                                case 174: targetBytes[targetIndex++] = 152; break;
                                case 8482: targetBytes[targetIndex++] = 153; break;
                                case 127: targetBytes[targetIndex++] = 154; break;
                                case 8250: targetBytes[targetIndex++] = 155; break;
                                case 177: targetBytes[targetIndex++] = 156; break;
                                case 185: targetBytes[targetIndex++] = 157; break;
                                case 178: targetBytes[targetIndex++] = 158; break;
                                case 179: targetBytes[targetIndex++] = 159; break;
                                case 65377: targetBytes[targetIndex++] = 161; break;
                                case 65378: targetBytes[targetIndex++] = 162; break;
                                case 65379: targetBytes[targetIndex++] = 163; break;
                                case 65380: targetBytes[targetIndex++] = 164; break;
                                case 65381: targetBytes[targetIndex++] = 165; break;
                                case 65382: targetBytes[targetIndex++] = 166; break;
                                case 65383: targetBytes[targetIndex++] = 167; break;
                                case 65384: targetBytes[targetIndex++] = 168; break;
                                case 65385: targetBytes[targetIndex++] = 169; break;
                                case 65386: targetBytes[targetIndex++] = 170; break;
                                case 65387: targetBytes[targetIndex++] = 171; break;
                                case 65388: targetBytes[targetIndex++] = 172; break;
                                case 65389: targetBytes[targetIndex++] = 173; break;
                                case 65390: targetBytes[targetIndex++] = 174; break;
                                case 65391: targetBytes[targetIndex++] = 175; break;
                                case 65392: targetBytes[targetIndex++] = 176; break;
                                case 65393: targetBytes[targetIndex++] = 177; break;
                                case 65394: targetBytes[targetIndex++] = 178; break;
                                case 65395: targetBytes[targetIndex++] = 179; break;
                                case 65396: targetBytes[targetIndex++] = 180; break;
                                case 65397: targetBytes[targetIndex++] = 181; break;
                                case 65398: targetBytes[targetIndex++] = 182; break;
                                case 65399: targetBytes[targetIndex++] = 183; break;
                                case 65400: targetBytes[targetIndex++] = 184; break;
                                case 65401: targetBytes[targetIndex++] = 185; break;
                                case 65402: targetBytes[targetIndex++] = 186; break;
                                case 65403: targetBytes[targetIndex++] = 187; break;
                                case 65404: targetBytes[targetIndex++] = 188; break;
                                case 65405: targetBytes[targetIndex++] = 189; break;
                                case 65406: targetBytes[targetIndex++] = 190; break;
                                case 65407: targetBytes[targetIndex++] = 191; break;
                                case 65408: targetBytes[targetIndex++] = 192; break;
                                case 65409: targetBytes[targetIndex++] = 193; break;
                                case 65410: targetBytes[targetIndex++] = 194; break;
                                case 65411: targetBytes[targetIndex++] = 195; break;
                                case 65412: targetBytes[targetIndex++] = 196; break;
                                case 65413: targetBytes[targetIndex++] = 197; break;
                                case 65414: targetBytes[targetIndex++] = 198; break;
                                case 65415: targetBytes[targetIndex++] = 199; break;
                                case 65416: targetBytes[targetIndex++] = 200; break;
                                case 65417: targetBytes[targetIndex++] = 201; break;
                                case 65418: targetBytes[targetIndex++] = 202; break;
                                case 65419: targetBytes[targetIndex++] = 203; break;
                                case 65420: targetBytes[targetIndex++] = 204; break;
                                case 65421: targetBytes[targetIndex++] = 205; break;
                                case 65422: targetBytes[targetIndex++] = 206; break;
                                case 65423: targetBytes[targetIndex++] = 207; break;
                                case 65424: targetBytes[targetIndex++] = 208; break;
                                case 65425: targetBytes[targetIndex++] = 209; break;
                                case 65426: targetBytes[targetIndex++] = 210; break;
                                case 65427: targetBytes[targetIndex++] = 211; break;
                                case 65428: targetBytes[targetIndex++] = 212; break;
                                case 65429: targetBytes[targetIndex++] = 213; break;
                                case 65430: targetBytes[targetIndex++] = 214; break;
                                case 65431: targetBytes[targetIndex++] = 215; break;
                                case 65432: targetBytes[targetIndex++] = 216; break;
                                case 65433: targetBytes[targetIndex++] = 217; break;
                                case 65434: targetBytes[targetIndex++] = 218; break;
                                case 65435: targetBytes[targetIndex++] = 219; break;
                                case 65436: targetBytes[targetIndex++] = 220; break;
                                case 65437: targetBytes[targetIndex++] = 221; break;
                                case 65438: targetBytes[targetIndex++] = 222; break;
                                case 65439: targetBytes[targetIndex++] = 223; break;

                                default:
                                    targetBytes[targetIndex++] = (byte)'?';
                                    break;
                                //throw new System.Exception("Could not convert from unicode to CodePage E");
                            }
                            break;


                        default:
                            targetBytes[targetIndex++] = (byte)'?';
                            break;
                    }
                }
                else
                {
                    targetBytes[targetIndex++] = (byte)value[i];

                    if (specchar)
                    {
                        switch (value[i])
                        {
                            case 'E':
                            case 'C':
                            case 'L':
                            case 'G':
                            case 'T':
                            case 'B':
                            case 'J':
                                codepage = value[i];
                                break;

                            default: break;
                        }
                        specchar = false;
                    }
                }
            }
        }
    }
}
