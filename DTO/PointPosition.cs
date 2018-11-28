using System;

namespace DTO
{
    [Serializable]
    public class PointPosition
    {
        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double[] ToArray()
        {
            return new[] { Longitude, Latitude };
        }
    }
}