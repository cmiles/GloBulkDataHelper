// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record LandDescriptionCsv
    {
        public string accession_nr { get; init; }

        public string aliquot_parts { get; init; }

        public string block_nr { get; init; }

        public int descrip_nr { get; init; }

        public string doc_class_code { get; init; }

        public string fractional_section { get; init; }

        public string ld_remarks { get; init; }

        public int meridian_code { get; init; }

        public string range_dir { get; init; }

        public decimal? range_nr { get; init; }

        public int? section_nr { get; init; }

        public string state_code { get; init; }

        public string survey_nr { get; init; }

        public string township_dir { get; init; }

        public decimal? township_nr { get; init; }
    }
}