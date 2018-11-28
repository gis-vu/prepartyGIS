using System;
using System.Linq;

namespace Helpers
{
    public static class DistanceHelpers
    {
        public const double DistanceDiff = 10f / 10000000;

        public static bool AreClose(double[] first, double[] second)
        {
            if (GetDistance(first, second) < DistanceDiff)
                return true;

            return false;
        }

        private static double GetDistance(double[] first, double[] second)
        {
            return Math.Sqrt(Math.Pow(first.First() - second.First(), 2) + Math.Pow(first.Last() - second.Last(), 2));
        }
    }
}
