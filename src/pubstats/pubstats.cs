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
        }
        public uint LapTime
        {
            get { return lapTime; }
        }
        public string LicenceName
        {
            get { return licenceName; }
        }
        //<id_wr> <track> <car> <split1> <split2> <split3> <laptime> <flags_hlaps> <racername>
    }
    public class PST
    {
        internal PST(string _licenceName, string[] args)
        {
            licenceName = _licenceName;
            try
            {
                distance = Convert.ToUInt32(args[0]);
                fuel = Convert.ToUInt32(args[1]);
                laps = Convert.ToUInt32(args[2]);
                hostJoin = Convert.ToUInt32(args[3]);
                win = Convert.ToUInt32(args[4]);
                second = Convert.ToUInt32(args[5]);
                third = Convert.ToUInt32(args[6]);
                finished = Convert.ToUInt32(args[7]);
                quals = Convert.ToUInt32(args[8]);
                pole = Convert.ToUInt32(args[9]);
                drags = Convert.ToUInt32(args[10]);
                dragWins = Convert.ToUInt32(args[11]);
                country = args[12];
                online = Convert.ToByte(args[13]);
                lastServer = args[14];
                lastTime = Convert.ToUInt32(args[15]);
                track = args[16];
                car = args[17];
            }
            catch (Exception)
            {
                Log.error("PST(), Licence: " + licenceName + " has a bad PST data.\r\n");
                distance = 0;
                fuel = 0;
                laps = 0;
                hostJoin = 0;
                win = 0;
                second = 0;
                third = 0;
                finished = 0;
                quals = 0;
                pole = 0;
                drags = 0;
                dragWins = 0;
                country = args[12];
                online = 0;
                lastServer = args[14];
                lastTime = 0;
                track = args[16];
                car = args[17];
            }
        }
        ~PST()
        {
            if (false == true) { }
        }
        string licenceName;
        string LicenceName
        {
            get { return licenceName; }
        }
        private uint distance;
        public uint Distance
        {
            get { return distance; }
        }
        private uint fuel;
        public uint Fuel
        {
            get { return fuel; }
        }
        private uint laps;
        public uint Laps
        {
            get { return laps; }
        }
        private uint hostJoin;
        public uint HostJoin
        {
            get { return hostJoin; }
        }
        private uint win;
        public uint Win
        {
            get { return win; }
        }
        private uint second;
        public uint Second
        {
            get { return second; }
        }
        private uint third;
        public uint Third
        {
            get { return third; }
        }
        private uint finished;
        public uint Finished
        {
            get { return finished; }
        }
        private uint quals;
        public uint Quals
        {
            get { return quals; }
        }
        private uint pole;
        public uint Pole
        {
            get { return pole; }
        }
        private uint drags;
        public uint Drags
        {
            get { return drags; }
        }
        private uint dragWins;
        public uint DragWins
        {
            get { return dragWins; }
        }
        private string country;
        public string Country
        {
            get { return country; }
        }
        private byte online;
        public byte Online
        {
            get { return online; }
        }
        private string lastServer;
        public string LastServer
        {
            get { return lastServer; }
        }
        private uint lastTime;
        public uint LastTime
        {
            get { return lastTime; }
        }
        private string track;
        public string Track
        {
            get { return track; }
        }
        private string car;
        public string Car
        {
            get { return car; }
        }
    }
    public delegate void PSTCallBack();
    public sealed class PubStats
    {
        public PubStats()
        {
            threadRequest = new Thread(new ThreadStart(ExecRequest));
            threadRequest.SetApartmentState(ApartmentState.STA);
            threadRequest.Priority = ThreadPriority.BelowNormal;
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

            Log.commandHelp("  Testing if Premium Service Available...\r\n");
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
            public Request(FetchDelegate _fetchDelegate, PSTCallBack _pstCallBack, params string[] _args)
            {
                fetchDelegate = _fetchDelegate;
                pstCallBack = _pstCallBack;
                args = _args;
            }
            private FetchDelegate fetchDelegate;
            private PSTCallBack pstCallBack;
            private string[] args;
            public void Exec()
            {
                bool fetchGood = fetchDelegate(args);
                if (pstCallBack != null)
                {
                    pstCallBack();
                }
                   

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
        private Dictionary<string, PST> storagePST = new Dictionary<string, PST>();

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

        private bool FetchPB(params string[] args)
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
        private bool FetchPST(params string[] args)
        {
            string data = FetchData("&action=pst&racer=" + args[0]);
            if (data == "")
                return false;

            string[] datas = data.Split(new string[] { "\n" }, StringSplitOptions.None);
            if(datas.Length != 18)
                return false;

            datas[16] = LFSWTrackToTrackPrefix(datas[16]);

            if (storagePST.ContainsKey(args[0]))
                storagePST[args[0]] = new PST(args[0],datas);
            else
                storagePST.Add(args[0], new PST(args[0],datas));

            return true;
        }
        
        public PB GetPB(string licenceName, string carTrackPrefix)
        {
            if (licenceName == "AI")
                return null;

            string key = licenceName + carTrackPrefix;
            if (storagePB.ContainsKey(key))
                return storagePB[key];

            Request request = new Request( new FetchDelegate(FetchPB), null, licenceName);
            if (!requestQueue.Contains(request))
            {
                //storagePB.Add(key, null); //this create a null index, so we don't ask for this PB anymore.
                lock(requestQueue){requestQueue.Enqueue(request);}
            }

            return null;
        }
        public WR GetWR(string carTrackPrefix)
        {
            if (storageWR.ContainsKey(carTrackPrefix))
                return storageWR[carTrackPrefix];

            Request request = new Request(new FetchDelegate(FetchWR),null, "");
            if (!requestQueue.Contains(request))
            {
                lock(requestQueue){requestQueue.Enqueue(request);}
            }
            return null;
        }
        public PST GetPST(string licenceName, PSTCallBack _pstCallBack)
        {
            //Maybe we should put PST into struct format , cause if 2 player search the same driver
            // one will overwrite during the display process of the other.
            if (licenceName == "AI")
                return null;

            string key = licenceName;
            if (storagePST.ContainsKey(key) && _pstCallBack == null)
                return storagePST[key];

            Request request = new Request(new FetchDelegate(FetchPST), _pstCallBack, licenceName);
            if (!requestQueue.Contains(request))
            {
                //storagePB.Add(key, null); //this create a null index, so we don't ask for this PB anymore.
                lock (requestQueue) { requestQueue.Enqueue(request); }
            }

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
