// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record CountyLookupCsv
    {
        public string county_code { get; init; }

        public string county_name { get; init; }

        public string state_code { get; init; }
    }
}