using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public static class DistanceHelpers
    {
        private const double DistanceDiff = 10f / 10000000;

        public static bool AreClose(double[] first, double[] second)
        {
            return GetDistance(first, second) < DistanceDiff;
        }

        public static bool IsCloseToLine(Tuple<double[], double[]> lineSegment, double[] second)
        {
            return GetDistanceToLine(lineSegment, second) < DistanceDiff;
        }

        private static double GetDistance(double[] first, double[] second)
        {
            return Math.Sqrt(Math.Pow(first.First() - second.First(), 2) + Math.Pow(first.Last() - second.Last(), 2));
        }

        public static Tuple<double[], double[]>[] SplitFeatureIntoLineSegments(double[][] coords)
        {
            var lineSegments = new List<Tuple<double[], double[]>>();

            var lastPosition = coords.First();

            foreach (var c in coords.Skip(1))
            {
                lineSegments.Add(new Tuple<double[], double[]>(lastPosition, c));
                lastPosition = c;
            }

            return lineSegments.ToArray();
        }

        public static double GetDistanceToLine(Tuple<double[], double[]> lineSegment, double[] point)
        {
            double x = point[0],
                y = point[1],
                x1 = lineSegment.Item1[0],
                y1 = lineSegment.Item1[1],
                x2 = lineSegment.Item2[0],
                y2 = lineSegment.Item2[1];

            var A = x - x1;
            var B = y - y1;
            var C = x2 - x1;
            var D = y2 - y1;

            var dot = A * C + B * D;
            var len_sq = C * C + D * D;
            double param = -1;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            double xx, yy;

            if (param < 0)
            {
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            var dx = x - xx;
            var dy = y - yy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static double CalcualteDistanceToFeature(double[][] featureCoordinates, double[] coordinate)
        {
            var lineSegments = SplitFeatureIntoLineSegments(featureCoordinates);

            var distance = GetDistanceToLine(lineSegments.First(), coordinate);

            return lineSegments.Skip(1).Select(c => GetDistanceToLine(c, coordinate)).Concat(new[] {distance}).Min();
        }
    }
}