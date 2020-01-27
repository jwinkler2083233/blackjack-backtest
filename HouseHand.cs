using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackBacktest
{
    public class HouseHand : Hand
    {
        // the second card dealt is always shown for the dealer's hand
        public Card CardShowing
        {
            get
            {
                return _cards[1];
            }
        }
        public override void Hit(Dealer dealer)
        {
            // when the house hits, it will keep hitting till the condition is met

            // check for blackjack
            if (IsBlackJack || IsBust)
                return;

            // this doesn't have the logic regarding a 'soft 17', used by some casinos

            while (SumCards <= 16)
            {
                dealer.DealCard(this);
            }

            // at this point the dealers cards are known, the status is 
            // set, and the collection is available
        }
    }
}
