using System.Collections.Generic;

// ReSharper disable InconsistentNaming - match response names

namespace GloBulkDataHelper.BlmNavigatorService.FindLd
{
    public class LdGeometry
    {
        public List<List<List<double>>> rings { get; set; }
        public LdSpatialReference spatialReference { get; set; }
    }
}