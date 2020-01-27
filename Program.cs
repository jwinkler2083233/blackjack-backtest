using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace BlackjackBacktest
{
    

    class Program
    {
        // I will have multiple types of back testing, so this could be
        // the base class
        public class BackTest
        {
            public BackTest()
            {
            }
        }
        static void Main(string[] args)
        {
            StatisticsMgr stats = new StatisticsMgr();
            PlayModifiedBetting play = new PlayModifiedBetting();

            play.PlayHands(5000000, stats);

            stats.CalculateWinningSequences();

            foreach(var pair in stats.MapSequences)
            {
                Debug.WriteLine("  Sequence:  {0}    Count:  {1}", pair.Key, pair.Value);
            }
            Debug.WriteLine("\n\nTensIndex at Win Time:");
            foreach (var pair in stats.TensAtWins)
            {
                Debug.WriteLine("  Tens: {0}    Count:  {1}", pair.Key, pair.Value);
            }

            Debug.WriteLine("\n\nTensIndex at Loss Time:");
            foreach (var pair in stats.TensAtLosses)
            {
                Debug.WriteLine("  Tens: {0}    Count:  {1}", pair.Key, pair.Value);
            }

            Debug.WriteLine("\nWins: {0}    Losses: {1}      Wins By Bust: {2}" +
                "    Wins By DoubleDown: {3}  Losses By DoubleDown: {4}  \n" +
                "Black Jacks:  {5}    Profit  %: {6}   Cumulative Bets: {7}  Wins By Split: {8}" +
                "   Losses by Split: {9}  Losses By Bust: {10}", 
                stats.Wins, stats.Losses, stats.WinsByBust, stats.WinsByDoubleDown,
                stats.LossesByDoubleDown, stats.BlackJacks, 100 * ((float)stats.Profit / (float)stats.CumulativeBet),
                stats.CumulativeBet, stats.WinsBySplit, stats.LossesBySplit, stats.LossesByBust);
        }
    }
}
