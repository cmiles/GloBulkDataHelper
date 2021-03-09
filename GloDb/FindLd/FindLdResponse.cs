using System.Collections.Generic;

namespace GloDb.FindLd
{
    public class FindLdResponse
    {
        public List<LdFeature> features { get; set; }
        public List<string> generatedplss { get; set; }
        public string legaldescription { get; set; }
    }
}