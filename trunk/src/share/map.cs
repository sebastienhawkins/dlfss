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
using System.IO;
using System.Runtime.InteropServices;

namespace Drive_LFSS.Map_
{
    using Definition_;
    using Log_;
    enum PTH_Format : int
    {
        FILE_TYPE = 0,
        VERSION = 6,
        REVISION = 7,
        NODE_COUNT = 8,
        FINISH_LINE = 12,
        NODE_START = 16,
        NODE_LENGTH = 40,
        CENTER_X = 0,
        CENTER_Y = 4,
        CENTER_Z = 8,
        DIR_X = 12,
        DIR_Y = 16,
        DIR_Z =20,
        LIMIT_LEFT = 24,
        LIMIT_RIGHT = 28,
        DRIVE_LEFT = 32,
        DRIVE_RIGHT = 36,
    }

    class MapData
    {
        internal MapData(int _nodeCount, int _finishLine)
        {
            nodeCount = _nodeCount;
            finishLine = _finishLine;
            nodeData = new NodeData[nodeCount];
        }
        internal void SetNode(int nodeIndex, int _centerX, int _centerY, int _centerZ,
                              float _dirX, float _dirY, float _dirZ, float _limitLeft, 
                              float _limitRight, float _driveLeft, float _driveRight)
        {
            nodeData[nodeIndex].centreX = _centerX;
            nodeData[nodeIndex].centreY = _centerY;
            nodeData[nodeIndex].centreZ = _centerZ;
            nodeData[nodeIndex].dirX = _dirX;
            nodeData[nodeIndex].dirY = _dirY;
            nodeData[nodeIndex].dirZ = _dirZ;
            nodeData[nodeIndex].limitLeft = _limitLeft;
            nodeData[nodeIndex].limitRight = _limitRight;
            nodeData[nodeIndex].driveLeft = _driveLeft;
            nodeData[nodeIndex].driveRight = _driveRight;
        }
        private int nodeCount;
        private int finishLine;
        private struct NodeData
        {
            internal int centreX;                    // fp
            internal int centreY;                    // fp
            internal int centreZ;                    // fp
            internal float dirX;                     // float
            internal float dirY;                     // float
            internal float dirZ;                     // float
            internal float limitLeft;                // outer limit
            internal float limitRight;               // outer limit
            internal float driveLeft;                // road limit
            internal float driveRight;               // road limit
        }
        private NodeData[] nodeData;
    }
    class Map
    {
        private static Dictionary<string,MapData> maps = new Dictionary<string,MapData>();
        internal static bool Initialize()
        {
            string[] files = System.IO.Directory.GetFiles(Program.dataPath + Path.DirectorySeparatorChar + "map", "*.pth");
            byte[] buffer;
            int nodeCount;
            int finishNode;
            for(int itr = 0; itr < files.Length; itr++)
            {
                buffer = File.ReadAllBytes(files[itr]);
                if(buffer.Length > 12)
                {
                    if (GetString(buffer, (int)PTH_Format.FILE_TYPE, 6) != "LFSPTH")
                    {
                        Log.error("  Invalide FileType map -> "+files[itr]+"\r\n");
                        return false;
                    }
                    if (buffer[(int)PTH_Format.VERSION] > 0)
                    {
                        Log.error("  Invalide Version map -> " + files[itr] + "\r\n");
                        return false;
                    }
                    if (buffer[(int)PTH_Format.REVISION] > 0)
                    {
                        Log.error("  Invalide Revision map -> " + files[itr] + "\r\n");
                        return false;
                    }
                    nodeCount = GetInt(buffer, (int)PTH_Format.NODE_COUNT);
                    finishNode = GetInt(buffer, (int)PTH_Format.FINISH_LINE);
                    MapData mapData = new MapData(nodeCount,finishNode);
                    int firstIndex;
                    for(int nodeItr = 0; nodeItr < nodeCount; nodeItr++)
                    {
                        firstIndex = ((int)PTH_Format.NODE_START + ((int)PTH_Format.NODE_LENGTH * nodeItr));
                        mapData.SetNode
                        (
                            nodeItr,
                            GetInt(buffer, firstIndex + (int)PTH_Format.CENTER_X),
                            GetInt(buffer, firstIndex + (int)PTH_Format.CENTER_Y),
                            GetInt(buffer, firstIndex + (int)PTH_Format.CENTER_Z),
                            GetFloat(buffer, firstIndex + (int)PTH_Format.DIR_X),
                            GetFloat(buffer, firstIndex + (int)PTH_Format.DIR_Y),
                            GetFloat(buffer, firstIndex + (int)PTH_Format.DIR_Z),
                            GetFloat(buffer, firstIndex + (int)PTH_Format.LIMIT_LEFT),
                            GetFloat(buffer, firstIndex + (int)PTH_Format.LIMIT_RIGHT),
                            GetFloat(buffer, firstIndex + (int)PTH_Format.DRIVE_LEFT),
                            GetFloat(buffer, firstIndex + (int)PTH_Format.DRIVE_RIGHT)
                        );
                    }
                    string trackPrefix = files[itr].Substring(files[itr].LastIndexOf('\\')+1);
                    trackPrefix = trackPrefix.Replace(".pth","");
                    maps.Add(trackPrefix,mapData);
                }
                
            }
            Log.commandHelp("  Loaded "+files.Length+" Maps.\r\n");
            
            return true;
        }
        private static string GetString(byte[] buffer, int start, int length)
        {
            string value = "";
            for(int itr = start; itr < length; itr++)
            {
                value += (char)buffer[itr];
            }
            return value;
        }
        private static int GetInt(byte[] buffer, int start)
        {
            return BitConverter.ToInt32(buffer, start);
        }
        private static float GetFloat(byte[] buffer, int start)
        {
            return BitConverter.ToSingle(buffer,start);
        }
    }
}