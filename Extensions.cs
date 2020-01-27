using System;
using System.Collections.Generic;
using System.Text;

namespace BlackjackBacktest
{
    static class Extensions
    {
        // seeding to the same value each time is convenient for testing.
        // This should be changed by backtesting code, however.
        public static Random rng = new Random(134);
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
