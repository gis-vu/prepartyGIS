using System;
using System.Collections.Generic;

namespace Models
{
    [Serializable]
    public class RouteFeature
    {
        public FeatureData Data { get; set; } = new FeatureData();
        public List<RouteFeature> Neighbours { get; set; } = new List<RouteFeature>();
    }
}
