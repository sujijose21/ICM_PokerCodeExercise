using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PokerRanking.App_Code;

namespace PokerRanking
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<PokerRankModel> pkRank = new List<PokerRankModel>();
            pkRank = PokerModel.GetPokerRankingList();
            string strFName = string.Empty;
            Console.WriteLine("Enter FileName : ");
            strFName = Console.ReadLine();
            string pokerResult = PokerModel.GetPokerOutput(@strFName);
            Console.WriteLine(pokerResult);
            Console.ReadKey();
        }
    }
}
