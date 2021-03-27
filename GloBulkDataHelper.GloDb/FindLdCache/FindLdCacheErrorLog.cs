using System;

namespace GloBulkDataHelper.GloDb.FindLdCache
{
    public class FindLdCacheErrorLog
    {
        public DateTime ErroredOn { get; set; }
        public string ErrorMessage { get; set; }
        public int Id { get; set; }
        public string PlssIdentifier { get; set; }
    }
}