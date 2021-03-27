// ReSharper disable InconsistentNaming - Matching Names in Actual File

using System;

namespace GloBulkDataHelper.GloDb.GloCsvFile
{
    public record PatentCsv
    {
        public string accession_nr { get; init; }

        public int? alt_accession_nr { get; init; }

        public string authority_code { get; init; }

        public string blm_serial_nr { get; init; }

        public bool cancelled_doc { get; init; }

        public string certificate_of_location { get; init; }

        public string coal_entry_nr { get; init; }

        public string doc_class_code { get; init; }

        public string document_nr { get; init; }

        public string geographic_name { get; init; }

        public int image_page_nr { get; init; }

        public string indian_allotment_nr { get; init; }

        public int l_o_code { get; init; }

        public bool metes_bounds { get; init; }

        public string military_rank { get; init; }

        public string militia { get; init; }

        public string misc_document_nr { get; init; }

        public string remarks { get; init; }

        public DateTime? signature_date { get; init; }

        public bool signature_present { get; init; }

        public string state_code { get; init; }

        public string state_in_favor_of { get; init; }

        public bool subsurface_reserved { get; init; }

        public string supreme_court_script_nr { get; init; }

        public DateTime? survey_date { get; init; }

        public decimal total_acres { get; init; }

        public string tribe { get; init; }

        public bool us_reservations { get; init; }

        public bool verify_flag { get; init; }
    }
}