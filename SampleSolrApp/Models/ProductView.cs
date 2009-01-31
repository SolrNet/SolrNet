using System.Collections.Generic;

namespace SampleSolrApp.Models {
    public class ProductView {
        public int FirstResultIndex { get; set; }
        public int LastResultIndex { get; set; }
        public ICollection<Product> Products { get; set; }
        public int TotalCount { get; set; }
    }
}