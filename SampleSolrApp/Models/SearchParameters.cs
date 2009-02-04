using System.Collections.Generic;

namespace SampleSolrApp.Models {
    public class SearchParameters {
        public SearchParameters() {
            Facets = new Dictionary<string, string>();
        }

        public string FreeSearch { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IDictionary<string, string> Facets { get; set; }

        public int FirstItemIndex {
            get {
                return (PageIndex-1)*PageSize;
            }
        }

        public int LastItemIndex {
            get {
                return FirstItemIndex + PageSize;
            }
        }
    }
}