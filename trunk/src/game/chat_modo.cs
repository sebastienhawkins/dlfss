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
using System.Text;
using System.Threading;
using System.Data;

namespace Drive_LFSS.ChatModo_
{
    using Script_;
    using Config_;
    using Log_;
    using Database_;
    using Game_;

    [Flags]enum Word_Flag : byte
    {
        NONE = 0,
        DESIGNATION = 1,
        IF_DESIGNATION = 2,
        IS_BAD = 3,
    }
    class ChatModo
    {
        internal ChatModo(ISession _iSession)
        {
            if(isActivated)
            {
                iSession = _iSession;
                threadUpdate = new Thread(new ThreadStart(update));
                threadUpdate.SetApartmentState(ApartmentState.STA);
                threadUpdate.Name = "ChatModo";
                threadUpdate.Priority = ThreadPriority.Lowest;
                threadUpdate.Start();
            }
        }
        ~ChatModo()
        {
            if(true == false){}
        }
        internal static bool Initialize()
        {
            isActivated = false;
            LoadBadWordTable();
            wordScoreListCount = wordScoreList.Count;
            if (wordScoreListCount > 0)
                isActivated = true;
            return isActivated;
        }
        internal static void LoadBadWordTable()
        {
            lock(wordScoreList)
            {
                wordScoreList.Clear();
                string query = "SELECT * FROM `bad_word`;";
                int count = 0;
                IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
                while(reader.Read())
                {
                    ++count;
                    wordScoreList.Add(reader.GetString(0), reader.GetByte(1));
                }
                reader.Dispose();
                Log.commandHelp("  loaded "+count+" \"bad word\".\r\n");
            }
        }
        private static bool isActivated = false;
        private Thread threadUpdate;
        private ISession iSession;
        private static Dictionary<string,byte> wordScoreList = new Dictionary<string,byte>();
        private static int wordScoreListCount = 0;
        private Queue<string> licenceNameTextList = new Queue<string>();
        private Dictionary<string, Dictionary<string, uint[]>> floodList = new Dictionary<string, Dictionary<string, uint[]>>();
        private Dictionary<string, uint[]> sosoWordList = new Dictionary<string, uint[]>();
        private Dictionary<string, uint[]> badWordList = new Dictionary<string, uint[]>();
        
        private int levenstein(string source, string filterWord)
        {
            int sourceLength = source.Length;
            int filterWordLength = filterWord.Length;
            int[,] matrixContainer = new int[sourceLength + 1, filterWordLength + 1];
            int cost;

            //Step 1
            if(sourceLength == 0) return filterWordLength;
            if(filterWordLength == 0) return sourceLength;
            
            //Step 2
            for(int i = 0; i <= sourceLength; matrixContainer[i, 0] = i++) ;
            for(int j = 0; j <= filterWordLength; matrixContainer[0, j] = j++) ;
            
            //Step 3
            for(int i = 1; i <= sourceLength; i++)
            {
                //Step 4
                for(int j = 1; j <= filterWordLength; j++)
                {
                    //Step 5
                    cost = (filterWord.Substring(j - 1, 1) == source.Substring(i - 1, 1) ? 0 : 1);
                    //Step 6
                    matrixContainer[i, j] = System.Math.Min(System.Math.Min(matrixContainer[i - 1, j] + 1, matrixContainer[i, j - 1] + 1),matrixContainer[i - 1, j - 1] + cost);
                }
            }
            //Step 7
            return matrixContainer[sourceLength, filterWordLength];
        }
        
        private const ushort TIMER_FLOOD = 4500;
        private const ushort FLOOD_MAX_COUNT = 3;
        private const ushort TIMER_SOSO_WORD = 12000;
        private const ushort SOSO_WORD_MAX_COUNT = 3;
        private const uint TIMER_BAD_WORD = 300000;
        private const ushort BAD_WORD_MAX_COUNT = 1;

        private void update()
        {
            uint SLEEP = 100;
            while (Program.MainRun)
            {
                AnalyseNextLine();
                
                //Flood Check
                if (floodList.Count > 0)
                {
                    Dictionary<string, Dictionary<string, uint[]>>.KeyCollection.Enumerator itr1 = floodList.Keys.GetEnumerator();
                    while(itr1.MoveNext())
                    {
                        if (floodList[itr1.Current].Count > 0)
                        {
                            Dictionary<string, uint[]>.KeyCollection.Enumerator itr2 = floodList[itr1.Current].Keys.GetEnumerator();
                            while (itr2.MoveNext())
                            {
                                if (floodList[itr1.Current][itr2.Current][0] >= FLOOD_MAX_COUNT)
                                {
                                    //floodList[itr1.Current].Remove(itr2.Current);
                                    //itr2 = floodList[itr1.Current].Keys.GetEnumerator();
                                    floodList[itr1.Current][itr2.Current][0] -= 1;
                                    floodList[itr1.Current][itr2.Current][1] = TIMER_FLOOD;

                                    IDriver driver = iSession.GetDriverWithLicenceName(itr1.Current);
                                    if (driver != null)
                                    {
                                        driver.AddFloodChat();
                                        driver.SendMTCMessage("^1STOP Flooding.");
                                        driver.SendMTCMessage("^1STOP Flooding.");
                                        driver.SendMTCMessage("^1STOP Flooding.");
                                        driver.SendMTCMessage("^1STOP Flooding.");
                                        driver.SendMTCMessage("^1STOP Flooding.");
                                        iSession.SendMTCMessageToAllAdmin("/msg " + driver.DriverName + " ^7-> is ^3flood ^7warned.");
                                    }
                                }
                                else if (floodList[itr1.Current][itr2.Current][1] < SLEEP)
                                {
                                    floodList[itr1.Current].Remove(itr2.Current);
                                    itr2 = floodList[itr1.Current].Keys.GetEnumerator();
                                }
                                else
                                    floodList[itr1.Current][itr2.Current][1] -= (uint)SLEEP;
                            }
                        }
                        else
                        {
                            floodList.Remove(itr1.Current);
                            itr1 = floodList.Keys.GetEnumerator();
                        }
                    }
                    
                }
                //Bad Check
                if (badWordList.Count > 0)
                {
                    Dictionary<string, uint[]>.KeyCollection.Enumerator itr = badWordList.Keys.GetEnumerator();
                    while (itr.MoveNext())
                    {
                        if(badWordList[itr.Current][0] > BAD_WORD_MAX_COUNT)
                        {
                            //badWordList.Remove(itr.Current);
                            //itr = badWordList.Keys.GetEnumerator();
                            badWordList[itr.Current][0] -= 1; 
                            badWordList[itr.Current][1] = TIMER_BAD_WORD;
                            IDriver driver = iSession.GetDriverWithLicenceName(itr.Current);
                            if (driver != null)
                            {
                                driver.AddWarningChat();
                                iSession.SendMTCMessageToAllAdmin(driver.DriverName + " ^1Appears Belligerent.");
                                driver.SendMTCMessage("^7Keep ^2good talking ^7at all Time.");
                                //if (!driver.IsAdmin)
                                 //   iSession.SendMSTMessage("/msg ^7C^3hatModo ^7: ^1Ban ^8" + driver.DriverName + "^7?");
                            }

                        }
                        else if (badWordList[itr.Current][1] < SLEEP)
                        {
                            badWordList.Remove(itr.Current);
                            itr = badWordList.Keys.GetEnumerator();
                        }
                        else
                            badWordList[itr.Current][1] -= (ushort)SLEEP;
                    }

                }
                //SoSo Check
                if (sosoWordList.Count > 0)
                {
                    Dictionary<string, uint[]>.KeyCollection.Enumerator itr = sosoWordList.Keys.GetEnumerator();
                    while (itr.MoveNext())
                    {
                        if (sosoWordList[itr.Current][0] > SOSO_WORD_MAX_COUNT)
                        {
                            sosoWordList[itr.Current][0] -= 1;
                            sosoWordList[itr.Current][1] = TIMER_SOSO_WORD;
                            
                            IDriver driver = iSession.GetDriverWithLicenceName(itr.Current);
                            if (driver != null)
                            {
                                driver.AddWarningChat();
                                driver.SendMTCMessage("^7Keep ^2good talking ^7at all Time.");
                                iSession.SendMTCMessageToAllAdmin(driver.DriverName + " ^3Seems annoying");
                                //iSession.SendMSTMessage("/msg " + driver.DriverName + " ^8-> ^3check your language.");
                            }
                        }
                        else if (sosoWordList[itr.Current][1] < SLEEP)
                        {
                            sosoWordList.Remove(itr.Current);
                            itr = sosoWordList.Keys.GetEnumerator();
                        }
                        else
                            sosoWordList[itr.Current][1] -= (uint)SLEEP;
                    }

                }
                System.Threading.Thread.Sleep((int)SLEEP);
            }
        }

        private void AnalyseNextLine()
        {
            if(licenceNameTextList.Count < 1)
                return;
            
            string[] words;
            string lineOftext;
            string licenceName;
            string pmLicenceName = "";
            Word_Flag sentenceMask = Word_Flag.NONE;
            lock(licenceNameTextList){lineOftext = licenceNameTextList.Dequeue();}
            licenceName = lineOftext.Split((char)0)[0];
            lineOftext = lineOftext.Split((char)0)[1];

            int indexStart = lineOftext.IndexOf(licenceName + " ^7: ^8")+(licenceName + " ^7: ^8").Length;
            int indexLength = 0;
            if (indexStart > -1 && indexStart < lineOftext.Length )
                indexLength = lineOftext.IndexOf(" ^7- ^9", indexStart) - indexStart;
            if(indexLength > 0)
                pmLicenceName = lineOftext.Substring(indexStart , indexLength);
            if (pmLicenceName != "")
                sentenceMask = Word_Flag.DESIGNATION;

            lineOftext = ConvertX.RemoveSpecialChar(ConvertX.RemoveColorCode(lineOftext));
            words = lineOftext.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //Log.commandHelp(licenceName + " typed this: " + lineOftext + ", it was a PM to:(" + pmLicenceName + ")\r\n");
            
            //part 1
            int wordCount = words.Length;
            Dictionary<string,byte>.Enumerator jtr;
            for (int itr = 0; itr < wordCount; itr++)
            {
                jtr = wordScoreList.GetEnumerator();
                while (jtr.MoveNext())
                {
                    if(words[itr].IndexOf(jtr.Current.Key,StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        #if DEBUG
                        //iSession.SendMSTMessage("/msg ^7C^3hatModo^51 ^7:^2 " + words[itr] + "^7->^2" + jtr.Current.Key);
                        #endif
                        sentenceMask |= (Word_Flag)jtr.Current.Value;
                    }
                }

                if ((sentenceMask & Word_Flag.IS_BAD) == Word_Flag.IS_BAD)
                    break;
            }
            //part 2
            if ((sentenceMask & Word_Flag.IS_BAD) != Word_Flag.IS_BAD)
            {
                for (int itr = 0; itr < wordCount; itr++)
                {
                    int levenScore = 32;
                    int _levenScore;
                    Word_Flag wordMask = Word_Flag.NONE;
                    jtr = wordScoreList.GetEnumerator();
                    while (jtr.MoveNext())
                    {
                        _levenScore = levenstein(words[itr].ToLowerInvariant(), jtr.Current.Key);
                        if (_levenScore < 5 && _levenScore < jtr.Current.Key.Length / 2 && levenScore > _levenScore)
                        {
                            #if DEBUG
                            //iSession.SendMSTMessage("/msg ^7C^3hatModo^52 ^7:^2 " + words[itr] + "^7->^2'" + jtr.Current.Key+"'");
                            #endif
                            levenScore = _levenScore;
                            wordMask = (Word_Flag)jtr.Current.Value;
                        }
                        //Log.commandHelp("that word(" + words[itr] + ") again that word(" + jtr.Current.Key + ") return(" + levenScore + ")\r\n");
                    }
                    sentenceMask |= wordMask;
                    if ((sentenceMask & Word_Flag.IS_BAD) == Word_Flag.IS_BAD)
                        break;
                }
            }
            //part 3
            //This is ugly and it to try catch a Spaced word, like "f u c k"
            if ((sentenceMask & Word_Flag.IS_BAD) != Word_Flag.IS_BAD)
            {
                jtr = wordScoreList.GetEnumerator();
                while (jtr.MoveNext())
                {
                    if (lineOftext.Replace(" ", "").IndexOf(ConvertX.RemoveSpecialChar(jtr.Current.Key), StringComparison.InvariantCultureIgnoreCase) > -1)
                    {
                        #if DEBUG
                        //iSession.SendMSTMessage("/msg ^7C^3hatModo^53 ^7:^2 CompleteLine ^7->^2" + jtr.Current.Key);
                        #endif
                        sentenceMask |= (Word_Flag)jtr.Current.Value;
                    }
                }
            }
            //part 4 Flood
            if(floodList.ContainsKey(licenceName))
            {
                if(floodList[licenceName].ContainsKey(lineOftext))
                    floodList[licenceName][lineOftext][0] += 1;
                else
                    floodList[licenceName].Add(lineOftext, new uint[2] { 1, TIMER_FLOOD });
            }
            else
            {
                floodList.Add(licenceName, new Dictionary<string, uint[]>());
                floodList[licenceName].Add(lineOftext, new uint[2] { 1, TIMER_FLOOD });
            }    
            
            if ((sentenceMask & Word_Flag.IS_BAD) == Word_Flag.IS_BAD)
                Gotcha(true,licenceName);
            else if ((sentenceMask & Word_Flag.IF_DESIGNATION) == Word_Flag.IF_DESIGNATION)
                Gotcha(false,licenceName);
            
            //Log.commandHelp("that line(" + lineOftext + ") scored (" + sentenceMask + ")\r\n");

        }
        
        private void Gotcha(bool badWord, string licenceName)
        {
            if(badWord)
            {
                if(badWordList.ContainsKey(licenceName))
                    badWordList[licenceName][0] += 1;
                else
                    badWordList.Add(licenceName, new uint[2] { 1, TIMER_BAD_WORD });

                //iSession.SendMSTMessage("/msg ^7C^3hatModo ^7: ^1UNDESIRABLE ^7chat detected.");
            }
            else
            {
                if (sosoWordList.ContainsKey(licenceName))
                    sosoWordList[licenceName][0] += 1;
                else
                    sosoWordList.Add(licenceName, new uint[2] { 1, TIMER_SOSO_WORD });

                //iSession.SendMSTMessage("/msg ^7C^3hatModo ^7: ^3PLEASE ^7correct your language.");
            }
        }
        /*private void FloodCheck()
        {
            floodList
        }*/
        public void AddNewLine(string licenceName, string lineOfText)
        {
            if(licenceName == "" || lineOfText == "")
                return;

            licenceNameTextList.Enqueue(licenceName+ ((char)0).ToString() +lineOfText);
        }
        public static bool IsActivated()
        {
            return isActivated;
        }
    }
}
