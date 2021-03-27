using GloBulkDataHelper.GloDb.GloData;

namespace GloBulkDataHelper.GloDb
{
    public static class PlssHelpers
    {
        public static string PlssString(LandDescription landDoc)
        {
            var standardName = $"{landDoc.StateCode} {landDoc.MeridianCode} " +
                               $"T{landDoc.TownshipNumber:0.#####}{landDoc.TownshipDirection} " +
                               $"R{landDoc.RangeNumber:0.#####}{landDoc.RangeDirection} " +
                               $"SEC {landDoc.SectionNumber}";

            if (int.TryParse(landDoc.AliquotParts, out var parsedAliquot)) return $"{standardName} LOT {parsedAliquot}";

            if (string.IsNullOrWhiteSpace(landDoc.AliquotParts))
                return $"{standardName} {landDoc.LandDescriptionRemarks}";

            return $"{standardName} ALIQ {landDoc.AliquotParts.Replace("½", "2")}";
        }
    }
}