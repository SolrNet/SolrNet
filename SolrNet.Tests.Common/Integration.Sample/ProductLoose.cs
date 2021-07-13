using System.Collections.Generic;
using SolrNet.Attributes;

namespace SolrNet.Tests.Integration.Sample {
    public class ProductLoose {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }

        [SolrField("score")]
        public double? Score { get; set; }

        [SolrField("*")]
        public IDictionary<string, object> OtherFields { get; set; }
        
    }
}
