using FluentResults;
using GloDb.FindLd;
using GloDb.GloData;
using Newtonsoft.Json;
using RestSharp;

namespace GloDb
{
    public static class FindLdRequest
    {
        public static Result<FindLdResponse> FindLd(string code)
        {
            var client =
                new RestClient(
                    "https://gis.blm.gov/arcgis/rest/services/Cadastral/BLM_Natl_PLSS_CadNSDI/MapServer/exts/CadastralSpecialServices");

            var request = new RestRequest("FindLD/", Method.POST);
            request.AddParameter("legaldescription", code); // adds to POST or URL querystring based on Method
            request.AddParameter("returnalllevels", true); // adds to POST or URL querystring based on Method
            request.AddParameter("f", "json"); // adds to POST or URL querystring based on Method

            var response = client.Execute(request);

            if (!response.IsSuccessful) return Result.Fail<FindLdResponse>(response.ErrorMessage);

            if (response.Content.StartsWith("{\"error\""))
            {
                var errorResponse = JsonConvert.DeserializeObject<FindLdError>(response.Content);

                return Result.Fail<FindLdResponse>(
                    $"Submitted: {code} - Response: {errorResponse.error.code}: {errorResponse.error.message}");
            }

            var ld = JsonConvert.DeserializeObject<FindLdResponse>(response.Content);

            return Result.Ok(ld);
        }

        public static string PlssString(LandDescription landDoc)
        {
            var standardName = $"{landDoc.StateCode} {landDoc.MeridianCode} " +
                               $"T{landDoc.TownshipNumber:0.#####}{landDoc.TownshipDir} " +
                               $"R{landDoc.RangeNumber:0.#####}{landDoc.RangeDir} " +
                               $"SEC {landDoc.SectionNumber}";

            if (int.TryParse(landDoc.AliquotParts, out var parsedAliquot)) return $"{standardName} LOT {parsedAliquot}";

            if (string.IsNullOrWhiteSpace(landDoc.AliquotParts)) return $"{standardName} {landDoc.LdRemarks}";

            return $"{standardName} ALIQ {landDoc.AliquotParts.Replace("½", "2")}";
        }
    }
}