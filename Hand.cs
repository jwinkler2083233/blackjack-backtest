using System;
using System.Collections.Generic;

namespace BlackjackBacktest
{
    public class Hand
    {
        public bool IsBust;
        public bool IsBlackJack;
        public bool IsSplittable;
        public bool IsSplit;
        public bool IsDoubledDown;
        public bool IsDoubleDownable;
        public bool HasAce;
        public int Bet;  // amount in dollars

        public List<Card> _cards;
        protected byte _sum;

        public byte SumCards
        {
            get
            {
                return _sum;
            }
            set
            {
                _sum = value;
            }
        }

        public List<Card> Cards
        {
            get
            {
                return _cards;
            }
        }

        public Hand()
        {
            IsBlackJack = false;
            IsBust = false;
            IsSplittable = false;
            IsSplit = false;
            IsDoubleDownable = true;
            IsDoubledDown = false;
            Bet = 0;
            _sum = 0;
            HasAce = false;
            _cards = new List<Card>();
        }

        public void Reset()
        {
            IsBlackJack = false;
            IsBust = false;
            IsSplittable = false;
            IsSplit = false;
            IsDoubleDownable = true;
            IsDoubledDown = false;
            Bet = 0;
            HasAce = false;
            _cards.Clear();
            _sum = 0;
        }

        // add a face up card to the collection
        public void AddCardShowing(Card card)
        {
            card.IsFaceUp = true;
            AddCard(card);
        }

        // remove a card from the hand, adjust the object, return the
        // split card
        public Card PrepareForSplit()
        {
            if(_cards.Count != 2)
            {
                return null;
            }
            Card retval = _cards[1];

            _sum = _cards[0].Face;

            // redo the sum
            if (_sum == 1)
            {
                _sum += 10;
            } else if(_sum > 10)
            {
                _sum = 10;
            }

            // hand is no longer splittable
            IsSplittable = false;
            IsSplit = true;

            return retval;
        }

        public void AddCard(Card card)
        {
            _cards.Add(card);
            if (card.Face == (byte)Card.Faces.Ace)
            {
                HasAce = true;
            }
            _sum += Math.Min((byte)10, card.Face);
            if (_sum <= 11 && HasAce == true)
            {
                _sum += 10;
            }

            if (_sum > 21 && !HasAce)
            {
                IsBust = true;
                return;
            }
            else if (HasAce && _sum > 21)
            {
                _sum = 0;
                foreach (var c in _cards)
                {
                    _sum += Math.Min((byte)10, c.Face);
                }

                // check to see if an ace caused the bust
                if (_sum > 21)
                {
                    IsBust = true;
                    return;
                }
            }

            if (_cards.Count == 2)
            {
                if (_cards[0].Face == (byte)Card.Faces.Ace &&
                    _cards[1].Face >= (byte)Card.Faces.Ten)
                {
                    IsBlackJack = true;
                }
                else if (_cards[1].Face == (byte)Card.Faces.Ace &&
                    _cards[0].Face >= (byte)Card.Faces.Ten)
                {
                    IsBlackJack = true;
                } else if (_cards[0].Face == _cards[1].Face)
                {
                    IsSplittable = true;
                }
            } else if (_cards.Count > 2)
            {
                IsDoubleDownable = false;
            }
        }

        public virtual void Hit(Dealer dealer, BettingBehavior betting)
        {
            if (IsBlackJack || IsBust)
                return;

            // just take a card and let the logic set the IsBust property as needed
            dealer.DealCard(this);

            // additional logic to hit again will be in derived classes
        }

        // this logic is in derived classes to determine when to stop hitting, and
        // how to alter the bet (optionally)
        public virtual void Hit(Dealer dealer)
        {
            if (IsBlackJack || IsBust)
                return;

            // just take a card and let the logic set the IsBust property as needed
            dealer.DealCard(this);

            // additional logic to hit again will be in derived classes
        }
    }
}
