using System;

namespace Models
{
    [Serializable]
    public class PointPosition
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double[] ToDoubleArray()
        {
            return new[] { Longitude, Latitude };
        }
    }
}