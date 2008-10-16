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
                string query = "SELECT * FROM `bad_word;";
                Program.dlfssDatabase.Lock();
                {
                    int count = 0;
                    IDataReader reader = Program.dlfssDatabase.ExecuteQuery(query);
                    while(reader.Read())
                    {
                        ++count;
                        wordScoreList.Add(reader.GetString(0), reader.GetByte(1));
                    }
                    Log.commandHelp("  loaded "+count+" \"bad word\".\r\n");
                }
                Program.dlfssDatabase.Unlock();
            }
        }
        private static bool isActivated = false;
        private Thread threadUpdate;
        private ISession iSession;
        private static Dictionary<string,byte> wordScoreList = new Dictionary<string,byte>();
        private static int wordScoreListCount = 0;
        private Queue<string> licenceNameTextList = new Queue<string>();
        
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
        private void update()
        {
            uint diff;
            long ticks = DateTime.Now.Ticks;
            while (Program.MainRun)
            {
                diff = (uint)((DateTime.Now.Ticks - ticks) / Program.tickPerMs);
                ticks = DateTime.Now.Ticks;
                AnalyseNextLine();
                System.Threading.Thread.Sleep(100);
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
            if(indexStart > -1)
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
                        sentenceMask |= (Word_Flag)jtr.Current.Value;
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
                    jtr = wordScoreList.GetEnumerator();
                    while (jtr.MoveNext())
                    {
                        _levenScore = levenstein(words[itr].ToLowerInvariant(), jtr.Current.Key);
                        if (_levenScore < 5 && _levenScore < jtr.Current.Key.Length / 2 && levenScore > _levenScore)
                        {
                            levenScore = _levenScore;
                            sentenceMask |= (Word_Flag)jtr.Current.Value;
                        }
                        //Log.commandHelp("that word(" + words[itr] + ") again that word(" + jtr.Current.Key + ") return(" + levenScore + ")\r\n");
                    }

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
                        sentenceMask |= (Word_Flag)jtr.Current.Value;
                }
            }

            if ((sentenceMask & Word_Flag.IS_BAD) == Word_Flag.IS_BAD)
            {
                iSession.SendMSTMessage(licenceName+" ^1UNDESIRABLE ^7chat detected.^3DEBUG");
            }
            else if ((sentenceMask & Word_Flag.IF_DESIGNATION) == Word_Flag.IF_DESIGNATION)
            {
                iSession.SendMSTMessage(licenceName + " ^3PLEASE ^7correct your language.^3DEBUG");
            }
            
            //Log.commandHelp("that line(" + lineOftext + ") scored (" + sentenceMask + ")\r\n");

        }

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
