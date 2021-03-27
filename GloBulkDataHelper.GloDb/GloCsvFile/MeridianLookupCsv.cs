// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloBulkDataHelper.GloDb.GloCsvFile
{
    public record MeridianLookupCsv
    {
        public int meridian_code { get; init; }

        public string meridian_name { get; init; }

        public string state_code { get; init; }

        public string state_default { get; init; }
    }
}