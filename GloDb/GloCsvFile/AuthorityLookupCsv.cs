// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record AuthorityLookupCsv
    {
        public string act_treaty { get; init; }

        public string authority_code { get; init; }

        public string entry_class { get; init; }

        public string statutory_ref { get; init; }
    }
}