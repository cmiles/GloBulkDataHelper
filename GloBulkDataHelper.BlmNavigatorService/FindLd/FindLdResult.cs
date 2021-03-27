using System;
using System.Collections.Generic;
using FluentResults;
using NetTopologySuite.Features;
using RestSharp;

namespace GloBulkDataHelper.BlmNavigatorService.FindLd
{
    public class FindLdResult
    {
        public List<Feature> LdGeoFeatures { get; set; } = new();
        public string LdGeoJson { get; set; }
        public Uri RequestUrl { get; set; }
        public IRestResponse Response { get; set; }
        public Result<FindLdResponse> Result { get; set; }
    }
}