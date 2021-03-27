using System;

namespace GloBulkDataHelper.GloDb.GloData
{
    public class Patent
    {
        public string AccessionNumber { get; set; }
        public int? AltAccessionNumber { get; set; }
        public string AuthorityCode { get; set; }
        public string BlmSerialNumber { get; set; }
        public bool CancelledDocument { get; set; }
        public string CertificateOfLocation { get; set; }
        public string CoalEntryNumber { get; set; }
        public string DocumentClassCode { get; set; }
        public string DocumentNumber { get; set; }
        public string GeographicName { get; set; }
        public int Id { get; set; }
        public int ImagePageNumber { get; set; }
        public string IndianAllotmentNumber { get; set; }
        public int LandOfficeCode { get; set; }
        public bool MetesBounds { get; set; }
        public string MilitaryRank { get; set; }
        public string Militia { get; set; }
        public string MiscellaneousDocumentNumber { get; set; }
        public string Remarks { get; set; }
        public DateTime? SignatureDate { get; set; }
        public bool SignaturePresent { get; set; }
        public string StateCode { get; set; }
        public string StateDataFile { get; set; }
        public string StateInFavorOf { get; set; }
        public bool SubsurfaceReserved { get; set; }
        public string SupremeCourtScriptNumber { get; set; }
        public DateTime? SurveyDate { get; set; }
        public decimal TotalAcres { get; set; }
        public string Tribe { get; set; }
        public bool UsReservations { get; set; }
        public bool VerifyFlag { get; set; }
    }
}