// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloBulkDataHelper.GloDb.GloCsvFile
{
    public record CountyCsv
    {
        public string accession_nr { get; init; }

        public string county_code { get; init; }

        public int descrip_nr { get; init; }

        public string doc_class_code { get; init; }

        public string state_code { get; init; }
    }
}