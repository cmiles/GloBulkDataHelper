using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using GloBulkDataHelper.BlmNavigatorService.FindLd;
using NetTopologySuite;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Newtonsoft.Json;
using ProjNet.CoordinateSystems.Transformations;
using RestSharp;

namespace GloBulkDataHelper.BlmNavigatorService
{
    public static class FindLdRequest
    {
        public static async Task<FindLdResult> FindLd(string code, bool returnAllLevels = false)
        {
            var client =
                new RestClient(
                    "https://gis.blm.gov/arcgis/rest/services/Cadastral/BLM_Natl_PLSS_CadNSDI/MapServer/exts/CadastralSpecialServices");

            var request = new RestRequest("FindLD/", Method.POST);
            request.AddParameter("legaldescription", code); // adds to POST or URL querystring based on Method
            request.AddParameter("returnalllevels", returnAllLevels); // adds to POST or URL querystring based on Method
            request.AddParameter("f", "json"); // adds to POST or URL querystring based on Method

            var constructedUri = client.BuildUri(request);
            var response = await client.ExecuteGetAsync(request);

            var returnResult = new FindLdResult
            {
                Response = response,
                RequestUrl = constructedUri
            };

            if (!response.IsSuccessful)
            {
                returnResult.Result = Result.Fail<FindLdResponse>(response.ErrorMessage);
                return returnResult;
            }

            if (response.Content.StartsWith("{\"error\""))
            {
                var errorResponse = JsonConvert.DeserializeObject<FindLdError>(response.Content);

                returnResult.Result = Result.Fail<FindLdResponse>(
                    $"Submitted: {code} - Response: {errorResponse.error.code}: {errorResponse.error.message}");

                return returnResult;
            }

            var ld = JsonConvert.DeserializeObject<FindLdResponse>(response.Content);
            returnResult.Result = Result.Ok(ld);

            var gf = NtsGeometryServices.Instance.CreateGeometryFactory(4326);

            var transformCoordinateFactory = new CoordinateTransformationFactory();

            var geoFeatures = new FeatureCollection();

            foreach (var loopFeatures in ld.features)
            {
                var coordinateTransform =
                    CoordinateHelpers.CoordinateTransformTo4326(loopFeatures.geometry.spatialReference.latestWkid);

                var ringList = new List<LinearRing>();

                foreach (var loopRing in loopFeatures.geometry.rings)
                {
                    var linearRingCoordinates = new List<Coordinate>();
                    foreach (var loopRingCoordinates in loopRing)
                    {
                        var (xTransformed, yTransformed) =
                            coordinateTransform.MathTransform.Transform(loopRingCoordinates[0], loopRingCoordinates[1]);
                        linearRingCoordinates.Add(new Coordinate(xTransformed, yTransformed));
                    }

                    ringList.Add(gf.CreateLinearRing(linearRingCoordinates.ToArray()));
                }

                var finalShape = ringList.Count > 1
                    ? gf.CreatePolygon(ringList.First(), ringList.Skip(1).ToArray())
                    : gf.CreatePolygon(ringList.First());

                var finalAttributes = new AttributesTable(new Dictionary<string, object>
                {
                    {"fromWkid", loopFeatures.geometry.spatialReference.wkid},
                    {"fromLatestWkid", loopFeatures.geometry.spatialReference.latestWkid},
                    {"name", loopFeatures.attributes.landdescription}
                });

                var finalFeature = new Feature(finalShape,
                    finalAttributes);

                returnResult.LdGeoFeatures.Add(finalFeature);

                geoFeatures.Add(finalFeature);
            }


            var serializer = GeoJsonSerializer.Create();
            await using var stringWriter = new StringWriter();
            using var jsonWriter = new JsonTextWriter(stringWriter);
            serializer.Serialize(jsonWriter, geoFeatures);
            returnResult.LdGeoJson = stringWriter.ToString();

            return returnResult;
        }
    }
}