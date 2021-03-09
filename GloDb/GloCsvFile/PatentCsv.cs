using System;

namespace GloDb.GloCsvFile
{
    public class PatentCsv
    {
        public string accession_nr { get; set; }

        public int? alt_accession_nr { get; set; }

        public string authority_code { get; set; }

        public string blm_serial_nr { get; set; }

        public bool cancelled_doc { get; set; }

        public string certificate_of_location { get; set; }

        public string coal_entry_nr { get; set; }

        public string doc_class_code { get; set; }

        public string document_nr { get; set; }

        public string geographic_name { get; set; }

        public int image_page_nr { get; set; }

        public string indian_allotment_nr { get; set; }

        public int l_o_code { get; set; }

        public bool metes_bounds { get; set; }

        public string military_rank { get; set; }

        public string militia { get; set; }

        public string misc_document_nr { get; set; }

        public string remarks { get; set; }

        public DateTime? signature_date { get; set; }

        public bool signature_present { get; set; }

        public string state_code { get; set; }

        public string state_in_favor_of { get; set; }

        public bool subsurface_reserved { get; set; }

        public string supreme_court_script_nr { get; set; }

        public DateTime? survey_date { get; set; }

        public decimal total_acres { get; set; }

        public string tribe { get; set; }

        public bool us_reservations { get; set; }

        public bool verify_flag { get; set; }
    }
}