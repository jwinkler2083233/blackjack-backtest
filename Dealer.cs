using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackBacktest
{
    public class Dealer
    {
        protected List<Card> _shoe;

        public HouseHand DealerHand;

        // this is the primary hand for the player
        public Hand PlayerHand;

        // this is the split hand for the player, if one exists
        public Hand PlayerSplitHand;

        // this captures statistics while the hands are played
        public StatisticsMgr Statistics;

        public Dealer(StatisticsMgr stats)
        {
            Statistics = stats;
            InitializeDeck();
            Extensions.Shuffle(_shoe);
        }

        public void InitializeDeck()
        {
            // initialize the deck
            _shoe = new List<Card>();

            // using a 6 deck shoe
            for (int i = 0; i < 6; i++)
            {
                for (byte x = 1; x < 14; x++)
                {
                    _shoe.Add(new Card(0, x));
                    _shoe.Add(new Card(1, x));
                    _shoe.Add(new Card(2, x));
                    _shoe.Add(new Card(3, x));
                }
            }
        }

        public void Shuffle()
        {
            InitializeDeck();
            Extensions.Shuffle(_shoe);
        }

        public Card DealCard(Hand hand)
        {
            hand.AddCardShowing(_shoe[0]);
            Card retval = _shoe[0];
            _shoe.RemoveAt(0);

            if (_shoe.Count < 52)
            {
                Shuffle();
            }
            return retval;
        }

        // the first hand returned is for the player, the second is for the house
        public Tuple<Hand, Hand> DealHands(Hand hand)
        {
            // use the default if hand is null
            if (null == hand)
                hand = new Hand();

            DealerHand = new HouseHand();

            // get two cards for both hands
            hand.AddCardShowing(_shoe[0]);
            _shoe.RemoveAt(0);

            hand.AddCardShowing(_shoe[0]);
            _shoe.RemoveAt(0);

            // now create the hand for the house
            DealerHand.AddCard(_shoe[0]);
            _shoe.RemoveAt(0);

            DealerHand.AddCardShowing(_shoe[0]);
            _shoe.RemoveAt(0);
            if (_shoe.Count < 52)
            {
                Shuffle();
            }
            PlayerHand = hand;
            PlayerSplitHand = null;
            return new Tuple<Hand, Hand>(hand, DealerHand);
        }
    }
}
