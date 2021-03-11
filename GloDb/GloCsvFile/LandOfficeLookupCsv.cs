// ReSharper disable InconsistentNaming - Matching Names in Actual File

namespace GloDb.GloCsvFile
{
    public record LandOfficeLookupCsv
    {
        public int l_o_code { get; set; }

        public string l_o_description { get; set; }

        public string state_code { get; set; }
    }
}