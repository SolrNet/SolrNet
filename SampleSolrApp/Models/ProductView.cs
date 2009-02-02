using System.Collections.Generic;

namespace SampleSolrApp.Models {
    public class ProductView {
        public SearchParameters Search { get; set; }
        public ICollection<Product> Products { get; set; }
        public int TotalCount { get; set; }
    }
}