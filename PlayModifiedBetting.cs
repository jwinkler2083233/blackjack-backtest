using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace BlackjackBacktest
{
    public class PlayModifiedBetting : PlayerBehavior
    {
        public class ModifiedBettingHand : Hand
        {
            public virtual Hand HandleSplit(Dealer dealer, BettingBehavior betting)
            {
                // create an additional hand, using one of the cards from this hand
                Hand secondHand = new Hand();
                secondHand.IsSplit = true;

                dealer.PlayerSplitHand = secondHand;

                Card secondHandFirstCard = PrepareForSplit();
                secondHand.AddCardShowing(secondHandFirstCard);

                // let the logic set the IsBust property as needed
                Card cardDealt = dealer.DealCard(this);
                dealer.Statistics.DiscardCard(cardDealt);

                // replace the second card for the primary hand
                cardDealt = dealer.DealCard(secondHand);
                dealer.Statistics.DiscardCard(cardDealt);

                // split on split is not allowed
                secondHand.IsSplittable = false;

                // set the default bet
                Bet = betting.Bet;
                secondHand.Bet = betting.Bet;

                // now apply the hit logic
                // determine if the bet should be modified
                if (betting.DetermineDoubleDown(dealer.DealerHand, this, dealer.Statistics))
                {
                    IsDoubledDown = true;

                    // okay to double the bet
                    Bet = betting.Bet * 2;

                    // deal one card only
                    // let the logic set the IsBust property as needed
                    cardDealt = dealer.DealCard(this);
                    dealer.Statistics.DiscardCard(cardDealt);
                }
                else
                {

                    // we'll keep hitting till the sum is at least X
                    while ((SumCards < 16 &&
                                (dealer.Statistics.TensIndex > 4)) ||
                            (SumCards < 12))
                    {
                        // break from the loop if the proportion of >= 10 cards is too low
                        if (SumCards > 11 && dealer.Statistics.TensIndex < 2)
                            break;

                        // don't try if we have a run of small cards
                        if (_cards.Count > 4 && SumCards > 11)
                            break;

                        // let the logic set the IsBust property as needed
                        cardDealt = dealer.DealCard(this);
                        dealer.Statistics.DiscardCard(cardDealt);

                        if (IsBust)
                            break;
                    }
                }

                // determine if the bet should be modified for the second hand
                if (betting.DetermineDoubleDown(dealer.DealerHand, this, dealer.Statistics))
                {
                    secondHand.IsDoubledDown = true;

                    // okay to double the bet
                    secondHand.Bet = betting.Bet * 2;

                    // deal one card only
                    // let the logic set the IsBust property as needed
                    cardDealt = dealer.DealCard(secondHand);
                    dealer.Statistics.DiscardCard(cardDealt);
                }
                else
                {

                    // we'll keep hitting till the sum is at least X
                    while ((secondHand.SumCards < 15 && dealer.DealerHand.CardShowing.Face > 6) ||
                            (secondHand.SumCards < 12) )
                    {
                        // break from the loop if the proportion of >= 10 cards is too low
                        if (secondHand.SumCards > 11 && dealer.Statistics.TensIndex < 3)
                            break;

                        // don't try if we have a run of small cards
                        if (secondHand.Cards.Count > 4 && secondHand.SumCards > 11)
                            break;

                        // let the logic set the IsBust property as needed
                        cardDealt = dealer.DealCard(secondHand);
                        dealer.Statistics.DiscardCard(cardDealt);

                        if (secondHand.IsBust)
                            break;
                    }
                }

                return secondHand;
            }

            // this is how to play, with this type of behavior
            public override void Hit(Dealer dealer, BettingBehavior betting)
            {
                if (IsBlackJack || IsBust)
                    return;

                if(betting.DetermineSplit(dealer.DealerHand, this, dealer.Statistics))
                {
                    HandleSplit(dealer, betting);

                    // at this point, the two bets were set, and the cards have been
                    // dealt.  The dealer knows about the hands
                    return;
                }

                // set the default bet
                Bet = betting.Bet;

                // modify the bet based on the last few cards (~12)
                {
                   float[] tensFactor =
                   {
                        0.7176587594f,
                        0.7482172244f,
                        1.008457417f,
                        0.8508591232f,
                        0.8222382664f,
                        0.8221459353f,
                        0.8482251933f,
                        0.9152756165f,
                        1.034960453f,
                        1.161035422f,
                        1.577427822f,
                        2.4375f,
                        2.0f
                   };

                    if(dealer.Statistics.TensIndex > 7)
                    {
                        int newbet = Bet;
                        newbet *= 1000;
                        Bet = newbet;
                    }
                   
                }

                // Intentionally not adding Insurance, because it's not material

                // determine if the bet should be modified
                if (betting.DetermineDoubleDown(dealer.DealerHand, this, dealer.Statistics))
                {
                    IsDoubledDown = true;

                    // okay to double the bet
                    Bet = betting.Bet * 2;

                    // deal one card only
                    // let the logic set the IsBust property as needed
                    Card cardDealt = dealer.DealCard(this);
                    dealer.Statistics.DiscardCard(cardDealt);
                }
                else
                {
                    // we'll keep hitting till the sum is at least 16
                    while ((SumCards < 16 && (dealer.Statistics.TensIndex > 1)) ||

                            // increase the threshold to 18 if player has an ace showing
                            (HasAce && _cards.Count < 3 && SumCards < 18) ||

                            // always hit if we're at 11 or below
                            SumCards < 12)
                    {
                        // let the logic set the IsBust property as needed
                        Card cardDealt = dealer.DealCard(this);
                        dealer.Statistics.DiscardCard(cardDealt);

                        if (IsBust)
                            break;
                    }
                }
            }
        }

        // this method will play a series of hands, using a specific behavior
        public override void PlayHands(int iterations, StatisticsMgr stats)
        {
            Dealer dlr = new Dealer(stats);
            BettingBehavior betting = new BettingBehavior();

            // this is done like this to avoid re-allocating hands
            ModifiedBettingHand bettingHand = new ModifiedBettingHand();

            // doing this here to avoid allocations
            Hand[] playerHands = new Hand[2];

            while (--iterations >= 0)
            {
                betting.DetermineBet(stats);

                bettingHand.Reset();

                dlr.DealHands(bettingHand);

                // tweak the stats before the doubledown bet
                foreach (Card playerCard in dlr.PlayerHand.Cards)
                {
                    // this records the card displayed, emulating a user that is
                    // counting the last 12 cards or so.  Counting more cards would
                    // result in better performance.
                    stats.DiscardCard(playerCard);
                }
                stats.DiscardCard(dlr.DealerHand.Cards[1]);
                stats.BlackJacks += (dlr.PlayerHand.IsBlackJack) ? 1 : 0;

                // apply the hit logic to the player's hand.  This will also
                // split if necessary
                dlr.PlayerHand.Hit(dlr, betting);

                // dealer only hits if the player doesn't go bust
                if (!dlr.PlayerHand.IsBust || 
                    (dlr.PlayerSplitHand != null && !dlr.PlayerSplitHand.IsBust))
                    dlr.DealerHand.Hit(dlr);

                if (iterations % 1000000 == 0)
                {
                    Debug.WriteLine("\tProfits:  {0}  Bet: {1}", stats.Profit, betting.Bet);
                }

                
                playerHands[0] = dlr.PlayerHand;
                if(null != dlr.PlayerSplitHand)
                {
                    // if the hand was split, add the split hand results as well
                    playerHands[1] = dlr.PlayerSplitHand;
                } else
                {
                    playerHands[1] = null;
                }

                foreach (var playerHand in playerHands)
                {
                    if (null == playerHand)
                        continue;

                    // keep track of the total amount bet since it's dynamic
                    stats.CumulativeBet += playerHand.Bet;

                    if (playerHand.IsBust)
                    {
                        stats.Profit -= playerHand.Bet;
                        stats.RecordLoss();
                        stats.LossesByBust++;
                        if (playerHand.IsDoubledDown)
                        {
                            stats.LossesByDoubleDown++;
                        }
                        if(playerHand.IsSplit)
                        {
                            stats.LossesBySplit++;
                        }
                    }
                    else if (playerHand.IsBlackJack && !dlr.DealerHand.IsBlackJack)
                    {
                        //  blackjack pays 3 to 2
                        stats.Profit += (int)(1.5 * playerHand.Bet);
                        stats.RecordWin();
                        if(playerHand.IsSplit)
                        {
                            stats.WinsBySplit++;
                        }
                        stats.WinSequences.Add(iterations);
                    } else if(!playerHand.IsBlackJack && dlr.DealerHand.IsBlackJack)
                    {
                        stats.Profit -= playerHand.Bet;
                        stats.RecordLoss();
                        if (playerHand.IsDoubledDown)
                        {
                            stats.LossesByDoubleDown++;
                        }
                        if (playerHand.IsSplit)
                        {
                            stats.LossesBySplit++;
                        }
                    }
                    else if (dlr.DealerHand.IsBust)
                    {
                        stats.Profit += playerHand.Bet;
                        stats.RecordWin();
                        stats.WinsByBust++;
                        if (playerHand.IsDoubledDown)
                        {
                            stats.WinsByDoubleDown++;
                        }
                        if(playerHand.IsSplit)
                        {
                            stats.WinsBySplit++;
                        }
                        stats.WinSequences.Add(iterations);
                    }
                    else if (playerHand.SumCards > dlr.DealerHand.SumCards)
                    {
                        stats.Profit += playerHand.Bet;
                        stats.RecordWin();
                        if (playerHand.IsDoubledDown)
                        {
                            stats.WinsByDoubleDown++;
                        }
                        if (playerHand.IsSplit)
                        {
                            stats.WinsBySplit++;
                        }
                        stats.WinSequences.Add(iterations);
                    }
                    else if (dlr.DealerHand.SumCards > playerHand.SumCards)
                    {
                        stats.Profit -= playerHand.Bet;
                        stats.RecordLoss();
                        if (playerHand.IsDoubledDown)
                        {
                            stats.LossesByDoubleDown++;
                        }
                        if (playerHand.IsSplit)
                        {
                            stats.LossesBySplit++;
                        }
                    }
                }

                // now keep track of the last few discarded cards from both players
                int index = 0;
                foreach(Card dlrCard in dlr.DealerHand.Cards)
                {
                    // skip the card that was initially shown because it was counted
                    // earlier
                    if (index++ == 1)
                        continue;

                    stats.DiscardCard(dlrCard);
                }
            }
        }
    }
}
