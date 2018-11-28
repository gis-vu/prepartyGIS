using System;

namespace DTO
{
    [Serializable]
    public class GridElement
    {
        public RouteFeature[] Features { get; set; }
        public RouteFeature[] BorderFeatures { get; set; }
        public PointPosition[] Border { get; set; }
    }
}