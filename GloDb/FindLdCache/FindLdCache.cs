using System;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;

namespace GloDb.FindLdCache
{
    public class FindLdCache
    {
        public DateTime CachedOn { get; set; }

        public Polygon OwnershipArea { get; set; }

        [Key] public string PlssIdentifier { get; set; }
    }
}