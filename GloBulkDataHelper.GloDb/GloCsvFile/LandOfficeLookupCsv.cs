// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloBulkDataHelper.GloDb.GloCsvFile
{
    public record LandOfficeLookupCsv
    {
        public int l_o_code { get; init; }

        public string l_o_description { get; init; }

        public string state_code { get; init; }
    }
}