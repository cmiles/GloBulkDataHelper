using System.Collections.Generic;

// ReSharper disable InconsistentNaming - match response names

namespace GloBulkDataHelper.BlmNavigatorService.FindLd
{
    public class FindLdResponse
    {
        public List<LdFeature> features { get; set; }
        public List<string> generatedplss { get; set; }
        public string legaldescription { get; set; }
    }
}