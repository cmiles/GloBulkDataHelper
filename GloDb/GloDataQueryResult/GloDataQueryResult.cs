using System;
using NetTopologySuite.Geometries;

namespace GloDb.GloDataQueryResult
{
    public class GloDataQueryResult
    {
        public string AccessionNumber { get; set; }
        public string ErrorLog { get; set; }
        public int Id { get; set; }
        public string LegalDescription { get; set; }
        public Polygon OwnershipArea { get; set; }
        public string Patentee { get; set; }
        public DateTime? PatentYear { get; set; }
        public string PlssDescription { get; set; }
    }
}