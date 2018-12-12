using System;
using System.Collections.Generic;
using System.Linq;

namespace Helpers
{
    public static class DistanceHelpers
    {
        private const double DistanceDiff = 10f / 10000000;


        public static bool IsInside(double[] p, double[][] polygon)
        {
            double minX = polygon[0][0];
            double maxX = polygon[0][0];
            double minY = polygon[0][1];
            double maxY = polygon[0][1];
            for (int i = 1; i < polygon.Length; i++)
            {
                var q = polygon[i];
                minX = Math.Min(q[0], minX);
                maxX = Math.Max(q[0], maxX);
                minY = Math.Min(q[1], minY);
                maxY = Math.Max(q[1], maxY);
            }

            if (p[0] < minX || p[0] > maxX || p[1] < minY || p[1] > maxY)
            {
                return false;
            }

            // http://www.ecse.rpi.edu/Homepages/wrf/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i][1] > p[1]) != (polygon[j][1] > p[1]) &&
                    p[0] < (polygon[j][0] - polygon[i][0]) * (p[1] - polygon[i][1]) / (polygon[j][1] - polygon[i][1]) + polygon[i][0])
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public static bool AreClose(double[] first, double[] second)
        {
            return GetDistance(first, second) < DistanceDiff;
        }

        public static bool IsCloseToLine(Tuple<double[], double[]> lineSegment, double[] second)
        {
            return GetDistanceToLine(lineSegment, second) < DistanceDiff;
        }

        public static double GetDistance(double[] first, double[] second)
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

        public static Tuple<Tuple<double[],double[]>, double[]> GetProjectionOnFeature(double[][] featureCoordinates, double[] point)
        {
            var sublines = SplitFeatureIntoLineSegments(featureCoordinates);
            var minDistance = GetDistanceToLine(sublines.First(), point);
            var closetsplitline = sublines.First(); 

            foreach (var s in sublines.Skip(1))
            {
                var distance = GetDistanceToLine(s, point);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closetsplitline = s;
                }
            }

            return new Tuple<Tuple<double[], double[]>, double[]>(closetsplitline, GetProjectionOnLine(closetsplitline, point));
        }

        public static double[] GetProjectionOnLine(Tuple<double[], double[]> lineSegment, double[] point)
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
            return new []{xx,yy};
        }

        public static double CalcualteDistanceToFeature(double[][] featureCoordinates, double[] coordinate)
        {
            var lineSegments = SplitFeatureIntoLineSegments(featureCoordinates);

            return lineSegments.Select(c => GetDistanceToLine(c, coordinate)).Min();
        }

        public static double CalcualteDistanceToFeatureInMeters(double[][] featureCoordinates, double[] coordinate)
        {
            var lineSegments = SplitFeatureIntoLineSegments(featureCoordinates);

            return lineSegments.Select(c =>
            {
                var projection = GetProjectionOnLine(c, coordinate);
                return DistanceBetweenCoordinates(projection, coordinate);
            }).Min();
        }

        public static bool AreNeighbours(double[][] routeFeature, double[][] testRouteFeature)
        {
            if (routeFeature == testRouteFeature)
                return false;

            var startPoint1 = routeFeature.First();
            var endPoint1 = routeFeature.Last();

            var startPoint2 = testRouteFeature.First();
            var endPoint2 = testRouteFeature.Last();

            if (AreClose(startPoint1, startPoint2))
                return true;

            if (AreClose(startPoint1, endPoint2))
                return true;

            if (AreClose(endPoint1, startPoint2))
                return true;

            if (AreClose(endPoint1, endPoint2))
                return true;

            return false;
        }

        public static double DistanceBetweenCoordinates(double[] start, double[] end)
        {
            double lat1 = start[1], lng1 = start[0], lat2 = end[1], lng2 = end[0];

            double earthRadius = 6371000; //meters
            double dLat = ToRadians(lat2 - lat1);
            double dLng = ToRadians(lng2 - lng1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double dist = (double)(earthRadius * c);

            return dist;
        }

        
        public static double ToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
}
}