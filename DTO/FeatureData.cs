using System;
using System.Collections.Generic;

namespace DTO
{
    [Serializable]
    public class FeatureData
    {
        public PointPosition[] Coordinates { get; set; }
        public IDictionary<string, dynamic> Properties { get; set; } = new Dictionary<string, dynamic>();
    }
}