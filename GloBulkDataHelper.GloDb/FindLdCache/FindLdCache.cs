using System;
using System.ComponentModel.DataAnnotations;

namespace GloBulkDataHelper.GloDb.FindLdCache
{
    public class FindLdCache
    {
        public DateTime CachedOn { get; set; }

        public string OwnershipArea { get; set; }

        [Key] public string PlssIdentifier { get; set; }
    }
}