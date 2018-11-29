using System;

namespace Models
{
    [Serializable]
    public class CellData
    {
        public RouteFeature[] Features { get; set; }
        public RouteFeature[] BorderFeatures { get; set; }
        //public PointPosition[] Border { get; set; }
    }
}