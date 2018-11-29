using System;

namespace Models
{
    [Serializable]
    public class GridCell
    {
        public PointPosition[] Border { get; set; }
        public string Index { get; set; }
    }
}