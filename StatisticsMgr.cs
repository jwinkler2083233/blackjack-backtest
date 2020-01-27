using System;
using System.Collections.Generic;

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
        }

        public void RecordWin()
        {
            StampTensAtWins();
            Wins++;
            LosingStreak = 0;
            WinStreak++;
        }

        // record what the tens index is at the time of a loss
        public void RecordLoss()
        {
            WinStreak = 0;
            LosingStreak++;
            StampTensAtLosses();
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
        public void CalculateWinningSequences()
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
