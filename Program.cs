using System;
using System.Diagnostics;

namespace BlackjackBacktest
{
    class Program
    {
        
        static void Main(string[] args)
        {
            StatisticsMgr stats = new StatisticsMgr();

            // turning off a couple of time-consuming features
            stats.Features.HasCalculateWinSequenceLengths = false;
            stats.Features.HasTrackLastFewCards = false;

            PlayModifiedBetting play = new PlayModifiedBetting();

            play.PlayHands(5000000, stats);

            stats.CalculateDerivedStatistics();
            stats.ReportStatistics();
        }
    }
}
