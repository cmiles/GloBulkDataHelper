// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloBulkDataHelper.GloDb.GloCsvFile
{
    public record PatenteeCsv
    {
        public string accession_nr { get; init; }

        public string doc_class_code { get; init; }

        public string patentee_first_name { get; init; }

        public string patentee_last_name { get; init; }

        public string patentee_middle_name { get; init; }

        public int patentee_seq_nr { get; init; }
    }
}