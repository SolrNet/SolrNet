using System.Collections.Generic;
using SingleFacetResult = System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, int>>;

namespace SampleSolrApp.Models {
    public class ProductView {
        public SearchParameters Search { get; set; }
        public ICollection<Product> Products { get; set; }
        public int TotalCount { get; set; }
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> Facets { get; set; }
    }
}