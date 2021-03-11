// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record CountyCsv
    {
        public string accession_nr { get; set; }

        public string county_code { get; set; }

        public int descrip_nr { get; set; }

        public string doc_class_code { get; set; }

        public string state_code { get; set; }
    }
}