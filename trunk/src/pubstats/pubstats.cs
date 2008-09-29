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
using System.Timers;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Text;

namespace Drive_LFSS.PubStats_
{
    using Drive_LFSS.Config_;
    using Drive_LFSS.Log_;
    using Drive_LFSS.Definition_;

    public class PB
    {
        internal PB(string[] args)
        {
            trackPrefix = args[0];
            carPrefix = args[1];
            splits = new uint[4]
            {
               0,
               Convert.ToUInt32(args[2]),
               Convert.ToUInt32(args[3]),
               Convert.ToUInt32(args[4])
            };
            lapTime = Convert.ToUInt32(args[5]);
            lapCount = Convert.ToUInt32(args[6]);
            timeStamp = Convert.ToUInt32(args[7]);
        }
        string trackPrefix;
        string carPrefix;
        uint[] splits;


        uint lapTime;
        uint lapCount;
        uint timeStamp;
        public uint[] Splits
        {
            get { return splits; }
            set { splits = value; }
        }
        public uint LapTime
        {
            get { return lapTime; }
            set { lapTime = value; }
        }
        //<track> <car> <split1> <split2> <split3> <laptime> <lapcount> <timestamp>
    }
    public class WR
    {
        internal WR(string[] args)
        {
            id_wr = Convert.ToUInt32(args[0]);
            trackPrefix = args[1];
            carPrefix = args[2];
            splits = new uint[4]
            {
               0,
               Convert.ToUInt32(args[3]),
               Convert.ToUInt32(args[4]),
               Convert.ToUInt32(args[5])
            };
            lapTime = Convert.ToUInt32(args[6]);
            driverFlag = (Driver_Flag)Convert.ToUInt32(args[7]);
            licenceName = args[8];
        }
        uint id_wr;
        string trackPrefix;
        string carPrefix;
        uint[] splits;
        uint lapTime;
        Driver_Flag driverFlag;
        string licenceName;

        public uint[] Splits
        {
            get { return splits; }
            set { splits = value; }
        }
        public uint LapTime
        {
            get { return lapTime; }
            set { lapTime = value; }
        }
        public string LicenceName
        {
            get { return licenceName; }
            set { licenceName = value; }
        }
        //<id_wr> <track> <car> <split1> <split2> <split3> <laptime> <flags_hlaps> <racername>
    }

    public sealed class PubStats
    {
        public PubStats()
        {
            threadRequest = new Thread(new ThreadStart(ExecRequest));
            threadRequest.Name = "PubStats Request";
            //webClient.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
        }
        ~PubStats()
        {
            if (true == false) { }
        }
        public void ConfigApply()
        {
            LFSWKey = Config.GetStringValue("PubStats","IdentKey");
            if (LFSWKey.ToLowerInvariant() != "none")
                LFSWUrl += "&idk=" + LFSWKey;
            else
            {
                LFSWKey = "";

                LFSWUsername = Config.GetStringValue("PubStats","Username");
                LFSWPassword = Config.GetStringValue("PubStats","Password");
                if (LFSWUsername.ToLowerInvariant() != "none" && LFSWPassword.ToLowerInvariant() != "none")
                    LFSWUrl += "&user=" + LFSWUsername + "&pass=" + LFSWPassword;
                else
                {
                    LFSWUsername = LFSWPassword = "drive_lfss";
                    LFSWUrl += "&user=" + LFSWUsername + "&pass=" + LFSWPassword;
                }
            }
            if (threadRequest.ThreadState == ThreadState.Unstarted)
                threadRequest.Start();

            if (HasPremiumService())
                Log.commandHelp("  Premium Service Enable\r\n");
            else
            {
                requestTimer = requestInterval;
                Log.commandHelp("  Premium Service Disable\r\n");
            }
        }

        private uint requestInterval = 50;
        private uint requestTimer = 0;
        private string LFSWKey = "";
        private string LFSWUsername = "";
        private string LFSWPassword = "";
        private string LFSWUrl = "http://www.lfsworld.net/pubstat/get_stat2.php?ps=1&version=1.4";

        private Thread threadRequest;
        private delegate bool FetchDelegate(params string[] args);
        private struct Request : IDisposable
        {
            public Request(FetchDelegate _fetchDelegate, params string[] _args)
            {
                fetchDelegate = _fetchDelegate;
                args = _args;
            }
            private FetchDelegate fetchDelegate;
            private string[] args;
            public void Exec()
            {
                fetchDelegate(args);
                this.Dispose();
            }
            public void Dispose()
            {

            }
        }
        private Queue<Request> requestQueue = new Queue<Request>();
        private WebClient webClient = new WebClient();

        private Dictionary<string, PB> storagePB = new Dictionary<string, PB>();
        private Dictionary<string, WR> storageWR = new Dictionary<string, WR>();

        private bool HasPremiumService(params string[] args)
        {
            requestInterval = 50;

            string data = "";
            try
            {
                data = webClient.DownloadString(LFSWUrl + "&action=wr");
                data = webClient.DownloadString(LFSWUrl + "&action=wr");
            }
            catch (Exception) {}

            if (data.Length > 0 && data.IndexOf("reload this page") == -1)
                return true;

            requestInterval = 5050;
            return false;
        }
        private string LFSWTrackToTrackPrefix(string lfswTrack)
        {
            string trackPrefix = "";
            switch (lfswTrack[0])
            {
                case '0': trackPrefix += "BL"; break;
                case '1': trackPrefix += "SO"; break;
                case '2': trackPrefix += "FE"; break;
                case '3': trackPrefix += "AU"; break;
                case '4': trackPrefix += "KY"; break;
                case '5': trackPrefix += "WE"; break;
                case '6': trackPrefix += "AS"; break;
            }
            trackPrefix += (Convert.ToUInt32(lfswTrack[1].ToString()) + 1).ToString();
            trackPrefix += (lfswTrack[2] == '1' ? "R" : "");

            return trackPrefix;
        }
        private string FetchData(string urlAction)
        {
            string answer = "";
            try{answer = webClient.DownloadString(LFSWUrl + urlAction);}
            catch(Exception){}
            return answer;
        }

        private bool FetchLicencePB(params string[] args)
        {
            string data = FetchData("&action=pb&racer=" + args[0]);
            if (data == "")
                return false;
            string[] lines = data.Split(new string[] { "\n" }, StringSplitOptions.None);
            string[] datas;
            string key;
            int itrMax = lines.Length;
            for (uint itr = 0; itr < itrMax; itr++)
            {
                datas = lines[itr].Split(new char[] { ' ' }, 8);
                if (datas.Length < 8 || datas[0]=="can't")
                    continue;

                datas[0] = LFSWTrackToTrackPrefix(datas[0]);

                key = args[0]+datas[1] + datas[0];
                if (storagePB.ContainsKey(key))
                    storagePB[key] = new PB(datas);
                else
                    storagePB.Add(key, new PB(datas));
            }
            //<track> <car> <split1> <split2> <split3> <laptime> <lapcount> <timestamp>
            return true;
        }
        private bool FetchWR(params string[] args)
        {
            string data = FetchData("&action=wr");
            if (data == "")
                return false;
            string[] lines = data.Split(new string[] { "\n" },StringSplitOptions.None);
            string[] datas;
            string key;
            int itrMax = lines.Length;
            for (uint itr = 0; itr < itrMax; itr++)
            {
                datas = lines[itr].Split(new char[] { ' ' },9);
                if (datas.Length < 9)
                    continue;

                datas[1] = LFSWTrackToTrackPrefix(datas[1]);

                key = datas[2] + datas[1];
                if (storageWR.ContainsKey(key))
                    storageWR[key] = new WR(datas);
                else
                    storageWR.Add(key, new WR(datas));
            }
            //<id_wr> <track> <car> <split1> <split2> <split3> <laptime> <flags_hlaps> <racername>
            return true;
        }
        
        public PB GetPB(string licenceName, string carTrackPrefix)
        {
            if (licenceName == "AI")
                return null;

            string key = licenceName + carTrackPrefix;
            if (storagePB.ContainsKey(key))
                return storagePB[key];

            Request request = new Request( new FetchDelegate(FetchLicencePB), licenceName);
            if (!requestQueue.Contains(request))
            {
                //storagePB.Add(key, null); //this create a null index, so we don't ask for this PB anymore.
                requestQueue.Enqueue(request);
            }

            return null;
        }
        public WR GetWR(string carTrackPrefix)
        {
            if (storageWR.ContainsKey(carTrackPrefix))
                return storageWR[carTrackPrefix];

            Request request = new Request(new FetchDelegate(FetchWR), "");
            if (!requestQueue.Contains(request))
                requestQueue.Enqueue(request);

            return null;
        }

        public void update(uint diff)
        {
            //TODO make busy timer, and make Dispose/New webClient
           
            //Just testing... 
            /* PB? allo = GetLicenceLFSW_PB("Greenseed","XFGBL1");
            if (allo.HasValue)
                allo.Value.Debug();*/
        }

        private void ExecRequest()
        {
            uint diff;
            long ticks = DateTime.Now.Ticks;
            while (Program.MainRun)
            {
                diff = (uint)((DateTime.Now.Ticks - ticks) / Program.tickPerMs);
                ticks = DateTime.Now.Ticks;

                if (!webClient.IsBusy)
                {
                    if (requestTimer < diff)
                    {
                        if (requestQueue.Count > 0)
                            requestQueue.Dequeue().Exec();

                        requestTimer = requestInterval;
                    }
                    else
                        requestTimer -= diff;
                }

                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
