using System.Collections.Generic;

namespace GloDb.FindLd
{
    public class LdGeometry
    {
        public List<List<List<double>>> rings { get; set; }
        public LdSpatialReference spatialReference { get; set; }
    }
}