// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record LandDescriptionCsv
    {
        public string accession_nr { get; set; }

        public string aliquot_parts { get; set; }

        public string block_nr { get; set; }

        public int descrip_nr { get; set; }

        public string doc_class_code { get; set; }

        public string fractional_section { get; set; }

        public string ld_remarks { get; set; }

        public int meridian_code { get; set; }

        public string range_dir { get; set; }

        public decimal? range_nr { get; set; }

        public int? section_nr { get; set; }

        public string state_code { get; set; }

        public string survey_nr { get; set; }

        public string township_dir { get; set; }

        public decimal? township_nr { get; set; }
    }
}