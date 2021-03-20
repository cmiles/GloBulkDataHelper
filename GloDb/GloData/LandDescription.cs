namespace GloBulkDataHelper.GloDb.GloData
{
    public class LandDescription
    {
        public string AccessionNumber { get; set; }
        public string AliquotParts { get; set; }
        public string BlockNumber { get; set; }
        public int DescriptionNumber { get; set; }
        public string DocumentClassCode { get; set; }
        public string FractionalSection { get; set; }
        public int Id { get; set; }
        public string LandDescriptionRemarks { get; set; }
        public int MeridianCode { get; set; }
        public string RangeDirection { get; set; }
        public decimal? RangeNumber { get; set; }
        public int? SectionNumber { get; set; }
        public string StateCode { get; set; }
        public string StateDataFile { get; set; }
        public string SurveyNumber { get; set; }
        public string TownshipDirection { get; set; }
        public decimal? TownshipNumber { get; set; }
    }
}