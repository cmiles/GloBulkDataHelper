// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloBulkDataHelper.GloDb.GloCsvFile
{
    public record WarranteeCsv
    {
        public string accession_nr { get; init; }

        public string doc_class_code { get; init; }

        public string warrantee_first_name { get; init; }

        public string warrantee_last_name { get; init; }

        public string warrantee_middle_name { get; init; }

        public int warrantee_seq_nr { get; init; }
    }
}