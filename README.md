# blackjack-backtest
This is a testing framework for blackjack systems.

This is for entertainment purposes only.  It shows a system I've developed for betting on blackjack.  The results were initially really bad, with a 25% loss ratio.  Over time, and with lots of small improvements, the latest revision has a positive profit ratio of about 1.3%.  I tested it with several million hands per test.  The house win ratio is about 52% now, but the profit ratio has steadily improved with changes to betting on splits and doubledowns.

The latest revision has support for:

* a betting formula to determine how much to bet on the next hand.
* a formula to determine when to doubledown -- it currently tests at a 3-2 win ratio
* a formula to determine when to split -- it currently tests at a 8-7 win ratio

Some other assumptions in this testing framework:  I'm using a pseudo-random generator to shuffle the cards, so the card distribution is very randomized.  When there is less than 52 cards remaining in the shoe, the cards are re-shuffled.

A few interesting observations:

* The likelihood of a losing or winning streak of only 1 win/loss is lower than 2+ wins/losses
* The likelihood of a win goes up dramatically if there are more than 66%  10s or face cards showing, prior to betting
* The likelihood of the dealer going bust is about 22%



