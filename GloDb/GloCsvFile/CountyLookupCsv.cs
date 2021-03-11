// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record CountyLookupCsv
    {
        public string county_code { get; set; }

        public string county_name { get; set; }

        public string state_code { get; set; }
    }
}