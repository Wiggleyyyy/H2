using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    internal static class RNG
    {
        private static Random s_random = new Random();
        public static int Range(int lower, int higher) { return s_random.Next(lower, higher); }
    }
}
