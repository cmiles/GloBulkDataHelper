// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloBulkDataHelper.GloDb.GloCsvFile
{
    public record DocumentClassLookupCsv
    {
        public string doc_class_code { get; init; }

        public string doc_class_display_name { get; init; }

        public string document_class_description { get; init; }
    }
}