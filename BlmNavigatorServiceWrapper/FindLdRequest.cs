using FluentResults;
using GloBulkDataHelper.BlmNavigatorService.FindLd;
using Newtonsoft.Json;
using RestSharp;

namespace GloBulkDataHelper.BlmNavigatorService
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

    }
}