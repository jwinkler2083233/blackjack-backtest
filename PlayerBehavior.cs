using System;
using System.Collections.Generic;
using System.Text;
using BlackjackBacktest;

namespace BlackjackBacktest
{
    // this is the base class for testing various behaviors
    public class PlayerBehavior
    {
        public PlayerBehavior() { }

        public virtual void PlayHands(int iterations, StatisticsMgr stats)
        {

        }
    }

}
