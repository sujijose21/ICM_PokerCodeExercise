using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PokerRanking.App_Code
{
    public class PokerRankModel
    {
        public int rank { get; set; }
        public string combination { get; set; }
        public string category { get; set; }
    }
    public class PokerResult
    {
        public int rank { get; set; }
        public int pairNo { get; set; }
        public int threeNo { get; set; }
        public int fourNo { get; set; }
        public int pair2No { get; set; }
        public List<int> cardVal { get; set; }
    }
    public class PokerModel
    {
        /// <summary>
        /// Poker Ranking Details
        /// </summary>
        /// <returns></returns>
        public static List<PokerRankModel> GetPokerRankingList()
        {
            List<PokerRankModel> pkRank = new List<PokerRankModel>();

            try
            {
                PokerRankModel pk = new PokerRankModel();
                //Five different numbers & Different Card types
                pk.rank = 1;
                pk.combination = "D";  
                pk.category = "D"; 
                pkRank.Add(pk);

                pk = new PokerRankModel();
                //Two cards of same value
                pk.rank = 2;
                pk.combination = "P";  
                pk.category = "D"; 
                pkRank.Add(pk);

                pk = new PokerRankModel();
                //Two different pairs
                pk.rank = 3;
                pk.combination = "2P";  
                pk.category = "D"; 
                pkRank.Add(pk);
                
                pk = new PokerRankModel();
                //Three of a kind
                pk.rank = 4;
                pk.combination = "T";  
                pk.category = "D"; 
                pkRank.Add(pk);
                
                pk = new PokerRankModel();
                //All five cards in consecutive value order
                pk.rank = 5;
                pk.combination = "FC";  
                pk.category = "D"; 
                pkRank.Add(pk);
                
                pk = new PokerRankModel();
                //All five cards having the same suit
                pk.rank = 6;
                pk.combination = "F";  
                pk.category = "S"; 
                pkRank.Add(pk);
                
                pk = new PokerRankModel();
                //Three of a kind and a Pair
                pk.rank = 7;
                pk.combination = "TP";  
                pk.category = "D"; 
                pkRank.Add(pk);
                
                pk = new PokerRankModel();
                //Four cards of the same value
                pk.rank = 8;
                pk.combination = "F";  
                pk.category = "D"; 
                pkRank.Add(pk);
                
                pk = new PokerRankModel();
                //All five cards in consecutive value order, with the same suit
                pk.rank = 9;
                pk.combination = "FC";  
                pk.category = "S"; 
                pkRank.Add(pk);
                
                pk = new PokerRankModel();
                //Ten, Jack, Queen, King and Ace in the same suit
                pk.rank = 10;
                pk.combination = "RF";  
                pk.category = "S"; 
                pkRank.Add(pk);
            }
            catch(Exception ex)
            {
            }
            return pkRank;
        }
    
        /// <summary>
        /// Player wise Result
        /// </summary>
        /// <param name="plyHand"></param>
        /// <returns></returns>
        public static PokerResult GetPokerResult(string plyHand)
        {
            PokerResult result = new PokerResult();

            result.rank = 0;
            result.pairNo = 0;
            result.threeNo = 0;
            result.fourNo = 0;
            result.pair2No = 0;
            result.cardVal = new List<int>();
           
            try 
            {
                var player = plyHand.Split(' ');
                //Card Type
                var pCT = player.Select(x => x.Substring(1, 1)).ToList(); 
                //Card Value
                List<int> pCV = player.Select(x => Convert.ToInt32(x.Substring(0, 1).Replace("T", "10").Replace("J", "11").Replace("Q", "12").Replace("K", "13").Replace("A", "14"))).ToList();
                pCV = pCV.OrderBy(x => x).ToList();
                var pSuit = pCT.GroupBy(x => x).Select(x => new { val = x.Key, ct = x.Count() }).ToList();
                var pVal = pCV.GroupBy(x => x).Select(x => new { val = x.Key, ct = x.Count() }).ToList();

                bool sameSuit = pSuit.Count == 1 ? true : false;
                bool IsConsecutive = pCV.Zip(pCV.Skip(1), (a, b) => (a + 1) == b).All(x => x);

                result.cardVal = pCV;

                if (sameSuit)
                {
                    //Consecutive
                    if (IsConsecutive)
                    {
                        if (pCV.Min(x => x) == 10 && pCV.Max(x => x) == 14) //Royal Flush
                            result.rank = 10;
                        else
                            result.rank = 9; //Straight flush
                    }
                    else
                        result.rank = 6; //Flush
                }
                else
                {
                    if(IsConsecutive)
                        result.rank = 5; //Straight 
                    else
                    {
                        if (pVal.Where(x => x.ct == 4).Any())//Four of a kind
                        {
                            result.rank = 8; 
                            result.fourNo = pVal.Where(x => x.ct == 4).Select(x => x.val).First();
                        }
                        else if (pVal.Where(x => x.ct == 3).Any() && pVal.Where(x => x.ct == 2).Any())//Full house
                        {
                            result.rank = 7;
                            result.threeNo = pVal.Where(x => x.ct == 3).Select(x => x.val).First();
                            result.pairNo = pVal.Where(x => x.ct == 2).Select(x => x.val).First();
                        }
                        else if (pVal.Where(x => x.ct == 3).Any() && !(pVal.Where(x => x.ct == 2).Any()))//Three of a kind
                        {
                            result.rank = 4;
                            result.threeNo = pVal.Where(x => x.ct == 3).Select(x => x.val).First();
                        }
                        else if (pVal.Where(x => x.ct == 2).Any()) 
                        {
                            if (pVal.Where(x => x.ct == 2).Count() == 2) //Two pairs
                            {
                                result.rank = 3;
                                result.pairNo = pVal.Where(x => x.ct == 2).Select(x => x.val).OrderByDescending(x => x).First();
                                result.pair2No = pVal.Where(x => x.ct == 2 && x.val != result.pairNo).Select(x => x.val).First();
                            }
                            else //Pair
                            {
                                result.rank = 2;
                                result.pairNo = pVal.Where(x => x.ct == 2).Select(x => x.val).First();
                            }
                        }
                        else //High card
                        {
                            result.rank = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// Player 1 or Player 2
        /// </summary>
        /// <param name="ply1"></param>
        /// <param name="ply2"></param>
        /// <returns></returns>
        public static int GetPlayerHand(PokerResult ply1, PokerResult ply2)
        {
            int result = 0;

            try
            {
                if (ply1.rank > ply2.rank)
                    result = 1;
                else if (ply1.rank < ply2.rank)
                    result = 2;
                else if(ply1.rank == ply2.rank)
                {
                    if ((new List<int> { 1,5,6,9,10}).Contains(ply1.rank))
                    {
                        for (int i = 4; i >= 0; i--)
                        {
                            if (ply1.cardVal[i] > ply2.cardVal[i])
                                result = 1;
                            else if (ply1.cardVal[i] < ply2.cardVal[i])
                                result = 2;

                            if (result > 0)
                                break;
                        }
                    }
                    else if ((new List<int> { 2,4,8 }).Contains(ply1.rank))
                    {
                        List<int> newPlay1 = new List<int>();
                        List<int> newPlay2 = new List<int>();

                        if(ply1.pairNo > 0 && ply2.pairNo > 0)
                        {
                            newPlay1 = ply1.cardVal.Where(x => x != ply1.pairNo).Select(x => x).OrderBy(x => x).ToList();
                            newPlay2 = ply2.cardVal.Where(x => x != ply1.pairNo).Select(x => x).OrderBy(x => x).ToList();

                            if (ply1.pairNo > ply2.pairNo)
                                result = 1;
                            else if (ply1.pairNo < ply2.pairNo)
                                result = 2;
                        }
                        else if (ply1.threeNo > 0 && ply2.threeNo > 0)
                        {
                            newPlay1 = ply1.cardVal.Where(x => x != ply1.threeNo).Select(x => x).OrderBy(x => x).ToList();
                            newPlay2 = ply2.cardVal.Where(x => x != ply1.threeNo).Select(x => x).OrderBy(x => x).ToList();
                            if (ply1.threeNo > ply2.threeNo)
                                result = 1;
                            else if (ply1.threeNo < ply2.threeNo)
                                result = 2;
                        }
                        else if (ply1.fourNo > 0 && ply2.fourNo > 0)
                        {
                            newPlay1 = ply1.cardVal.Where(x => x != ply1.fourNo).Select(x => x).OrderBy(x => x).ToList();
                            newPlay2 = ply2.cardVal.Where(x => x != ply1.fourNo).Select(x => x).OrderBy(x => x).ToList();
                            if (ply1.fourNo > ply2.fourNo)
                                result = 1;
                            else if (ply1.fourNo < ply2.fourNo)
                                result = 2;
                        }

                        if(result == 0)
                        {
                            for (int i = newPlay2.Count-1; i >= 0; i--)
                            {
                                if (newPlay1[i] > newPlay2[i])
                                    result = 1;
                                else if (newPlay1[i] < newPlay2[i])
                                    result = 2;

                                if (result > 0)
                                    break;
                            }
                        }
                    }
                    else if (ply1.rank == 7)
                    {
                        if (ply1.threeNo > ply2.threeNo)
                            result = 1;
                        else if (ply1.threeNo < ply2.threeNo)
                            result = 2;

                        if(result == 0)
                        {
                            if (ply1.pairNo > ply2.pairNo)
                                result = 1;
                            else if (ply1.pairNo < ply2.pairNo)
                                result = 2;
                        }
                    }
                    else
                    {
                        if (ply1.pairNo > ply2.pairNo)
                            result = 1;
                        else if (ply1.pairNo < ply2.pairNo)
                            result = 2;

                        if (result == 0)
                        {
                            if (ply1.pair2No > ply2.pair2No)
                                result = 1;
                            else if (ply1.pair2No < ply2.pair2No)
                                result = 2;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            return result;
        }
        /// <summary>
        /// Poker result output
        /// </summary>
        /// <param name="strFName"></param>
        /// <returns></returns>
        public static string GetPokerOutput(string strFName)
        {
            string result = string.Empty;

            try
            {
                StreamReader objStreamReader;
                objStreamReader = File.OpenText(strFName);
                string contents = objStreamReader.ReadToEnd();
                objStreamReader.Close();

                int hand1 = 0;
                int hand2 = 0;
                int tie = 0;
                if ((contents.Length > 0))
                {
                    var pokerInput = contents.Split('\n');
                    for (int i = 0; i < pokerInput.Length; i++)
                    {
                        PokerResult player1Result = PokerModel.GetPokerResult(pokerInput[i].Substring(0,14));
                        PokerResult player2Result = PokerModel.GetPokerResult(pokerInput[i].Substring(15,14));
                        int winningHand = PokerModel.GetPlayerHand(player1Result, player2Result);
                        hand1 = winningHand == 1 ? hand1 + 1 : hand1;
                        hand2 = winningHand == 2 ? hand2 + 1 : hand2;
                        tie = winningHand == 0 ? tie + 1 : tie;
                    }
                    result = "Player 1: " + Convert.ToString(hand1) + " hands\n";
                    result = result + "Player 2: " + Convert.ToString(hand2) + " hands\n";
                    result = tie > 0 ? result + Convert.ToString(tie) + " ties" : result;
                }
            }
            catch (Exception ex)
            {
                
            }
            return result;
        }
    }
}
