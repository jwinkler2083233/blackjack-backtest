using System;

namespace BlackjackBacktest
{
    public class BettingBehavior
    {
        // this is the previous amount to bet
        public int Bet;

        public BettingBehavior()
        {
            Bet = 100;
        }

        // this method will determine the amount to bet on the next
        // hand
        public virtual void DetermineBet(StatisticsMgr stats)
        {
            int bet = Bet;
            // likelihood of a win or lose streak greater than 1 is 66%.  As the
            // streak continues, the chance of another win/lose inside the streak
            // is about 40%.
            if (stats.WinStreak == 1)
            {
                bet = 2000;
            } else if(stats.WinStreak == 2)
            {
                bet = 1000;
            } else if (stats.WinStreak > 2)
            {
                bet -= (int)(bet * .42);
            } else if (stats.LosingStreak == 1)
            {
                bet = 100;
            }
            else if (stats.LosingStreak > 2)
            {
                bet += (int)(bet * .42);
            }

            bet = Math.Max(bet, 100);
            bet = Math.Min(bet, 100000);
            Bet = bet;
        }

        // return true = should doubledown
        public virtual bool DetermineDoubleDown(HouseHand dealerHand, Hand playerHand, StatisticsMgr stats)
        {
            if (stats.WinStreak > 3 || 
                !playerHand.IsDoubleDownable)
            {
                // just don't
                return false;
            }

            // if the dealer's shown card is not a 10 pt or ace, continue
            if (dealerHand.CardShowing.Face == 1 ||
                (dealerHand.CardShowing.Face > 9 && stats.TensIndex > 2))
            {
                return false;
            }

            if (playerHand.HasAce && playerHand.SumCards < 14 && stats.TensIndex > 4)
            {
                return true;
            }

            // if the card sum is between 9 and 12
            if (playerHand.SumCards < 12 &&
                playerHand.SumCards >= 9)
            {
                if (dealerHand.CardShowing.Face < 9)
                {
                    return true;
                }
            }

            return false;
        }

        // this is logic to determine if the player should split the hand
        public virtual bool DetermineSplit(HouseHand dealerHand, Hand playerHand, StatisticsMgr stats)
        {
            // don't do anything special if it's not likely to work
            if (
                stats.WinStreak > 4
                ||
                (stats.LosingStreak > 0 && stats.LosingStreak < 3)
                || !playerHand.IsSplittable
                )
            {
                return false;
            }

            // split a pair of aces
            if(playerHand.SumCards == 2)
            {
                return true;
            }

            // avoid pair of 4s, pair of 5s
            if (
                // more likely to have 18 or 20 in these cases, so don't  split
                 (
                    stats.TensIndex < 5 &&
                    (playerHand.SumCards == 8 || playerHand.SumCards == 10)
                    )
                )
            {
                return false;
            }

            if(Math.Min(dealerHand.CardShowing.Face, (byte)10) >= playerHand.Cards[0].Face)
            {
                return false;
            }

            if (dealerHand.CardShowing.Face == 1)
            {
                return false;
            }

            return true;
        }
    }
}
