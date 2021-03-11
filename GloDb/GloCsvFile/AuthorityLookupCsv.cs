// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record AuthorityLookupCsv
    {
        public string act_treaty { get; set; }

        public string authority_code { get; set; }

        public string entry_class { get; set; }

        public string statutory_ref { get; set; }
    }
}