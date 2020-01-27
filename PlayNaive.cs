using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackBacktest
{
    public class PlayNaive : PlayerBehavior
    {
        public class NaiveHand : Hand
        {
            public NaiveHand()
            {
            }

            // this is how to play a simple hand, with this type of behavior
            public override void Hit(Dealer dealer)
            {
                if (IsBlackJack || IsBust)
                    return;

                // we'll keep hitting till the sum is at least 16
                while (SumCards < 16)
                {
                    // just take a card and let the logic set the IsBust property as needed
                    dealer.DealCard(this);

                    if (IsBust)
                        break;
                }
            }
        }

        public override void PlayHands(int iterations, StatisticsMgr stats)
        {
            Dealer dlr = new Dealer(stats);

            while (--iterations >= 0)
            {
                var hands = dlr.DealHands(new NaiveHand());

                hands.Item1.Hit(dlr);

                if (!hands.Item1.IsBust)
                    hands.Item2.Hit(dlr);

                if (hands.Item1.IsBust)
                {
                    stats.Losses++;
                }
                else if (hands.Item1.IsBlackJack && !hands.Item2.IsBlackJack)
                {
                    stats.Wins++;
                }
                else if (hands.Item2.IsBust)
                {
                    stats.Wins++;
                    stats.WinsByBust++;
                }
                else if (hands.Item1.SumCards > hands.Item2.SumCards)
                {
                    stats.Wins++;
                }
                else if (hands.Item2.SumCards > hands.Item1.SumCards)
                {
                    stats.Losses++;
                }
            }
        }
    }
}
