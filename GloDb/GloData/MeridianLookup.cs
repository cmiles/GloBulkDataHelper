namespace GloDb.GloData
{
    public class MeridianLookup
    {
        public int Id { get; set; }
        public int MeridianCode { get; set; }
        public string MeridianName { get; set; }
        public string StateCode { get; set; }
        public string StateDataFile { get; set; }
        public string StateDefault { get; set; }
    }
}