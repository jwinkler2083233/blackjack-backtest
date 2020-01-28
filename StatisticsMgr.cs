using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BlackjackBacktest
{
    public class StatisticsMgr
    {
        public int Wins;
        public int Losses;
        public long Profit;

        // this is the number of tens in the last few cards collection
        public int TensIndex
        {
            get
            {
                int tens = 0;
                foreach(Card card in LastFewCards)
                {
                    if(card.Face >= 10)
                    {
                        tens++;
                    }
                }

                return tens;
            }
        }
        public int WinStreak;
        public int LosingStreak;
        public int WinsByBust;
        public int LossesByBust;
        public int WinsByDoubleDown;
        public int WinsAgainstAceShown;
        public int LossesByDoubleDown;
        public int BlackJacks;
        public int WinsBySplit;
        public int LossesBySplit;
        public long CumulativeBet;
        public List<Card> LastFewCards;
        public List<int> WinSequences;
        public Dictionary<int, int> MapSequences;
        public Dictionary<int, int> TensAtWins;
        public Dictionary<int, int> TensAtLosses;

        public class FeaturesContainer
        {
            public bool HasCalculateWinSequenceLengths;
            public bool HasTrackLastFewCards;
            public FeaturesContainer()
            {
                HasCalculateWinSequenceLengths = true;
                HasTrackLastFewCards = true;
            }
        }

        public FeaturesContainer Features;
        public StatisticsMgr()
        {
            WinStreak = 0;
            LosingStreak = 0;
            Profit = 0;
            LastFewCards = new List<Card>();
            WinSequences = new List<int>();
            MapSequences = new Dictionary<int, int>();
            TensAtWins = new Dictionary<int, int>();
            TensAtLosses = new Dictionary<int, int>();

            Features = new FeaturesContainer();
        }

        public void RecordWin(int iterations)
        {
            if (Features.HasTrackLastFewCards)
            {
                StampTensAtWins();
            }
            Wins++;
            LosingStreak = 0;
            WinStreak++;

            if (Features.HasCalculateWinSequenceLengths)
            {
                WinSequences.Add(iterations);
            }
        }

        // record what the tens index is at the time of a loss
        public void RecordLoss()
        {
            WinStreak = 0;
            LosingStreak++;
            if (Features.HasTrackLastFewCards)
            {
                StampTensAtLosses();
            }
            Losses++;
        }

        // record what the tens index is at the time of a win
        public void StampTensAtWins()
        {
            int curval = 0;
            if(!TensAtWins.TryGetValue(TensIndex, out curval))
            {
                TensAtWins[TensIndex] = 1;
            }
            else
            {
                TensAtWins[TensIndex] = curval + 1;
            }
        }
        public void StampTensAtLosses()
        {
            int curval = 0;
            if (!TensAtLosses.TryGetValue(TensIndex, out curval))
            {
                TensAtLosses[TensIndex] = 1;
            } else
            {
                TensAtLosses[TensIndex] = curval+1;
            }
        }

        public void CalculateDerivedStatistics()
        {
            if(Features.HasCalculateWinSequenceLengths)
            {
                CalculateWinningSequences();
            }
        }

        public void ReportStatistics()
        {
            if (Features.HasCalculateWinSequenceLengths)
            {
                foreach (var pair in MapSequences)
                {
                    Debug.WriteLine("  Sequence:  {0}    Count:  {1}", pair.Key, pair.Value);
                }
            }

            if (Features.HasTrackLastFewCards)
            {
                Debug.WriteLine("\n\nTensIndex at Win Time:");
                foreach (var pair in TensAtWins)
                {
                    Debug.WriteLine("  Tens: {0}    Count:  {1}", pair.Key, pair.Value);
                }

                Debug.WriteLine("\n\nTensIndex at Loss Time:");
                foreach (var pair in TensAtLosses)
                {
                    Debug.WriteLine("  Tens: {0}    Count:  {1}", pair.Key, pair.Value);
                }
            }

            Debug.WriteLine("\nWins: {0}    Losses: {1}  \n" +
                "Wins By Bust: {2}  Losses By Bust: {10}\n" +
                "Wins By DoubleDown: {3}  Losses By DoubleDown: {4}  \n" +
                "BlackJacks:  {5}   Wins By Split: {8} Losses by Split: {9}\n" +
                "Profit  %: {6}   \n" +
                "Cumulative Bets: {7}  \n\n" ,
                Wins, Losses, WinsByBust, WinsByDoubleDown,
                LossesByDoubleDown, BlackJacks, 100 * ((float)Profit / (float)CumulativeBet),
                CumulativeBet, WinsBySplit, LossesBySplit, LossesByBust);
        }
        protected void CalculateWinningSequences()
        {
            int priorWinSequence = 0;
            int sequenceCount = 0;
            foreach(var iter in WinSequences)
            {
                if(iter != priorWinSequence - 1)
                {
                    if (!MapSequences.ContainsKey(sequenceCount))
                    {
                        MapSequences[sequenceCount] = 1;
                    }
                    else
                    {
                        MapSequences[sequenceCount] = MapSequences[sequenceCount]+1;
                    }
                    sequenceCount = 0;
                } else
                {
                    sequenceCount++;
                }
                priorWinSequence = iter;
            }
        }

        public void DiscardCard(Card card)
        {
            LastFewCards.Add(card);

            if(LastFewCards.Count > 12)
            {
                LastFewCards.RemoveAt(0);
            }
        }
    }
}
