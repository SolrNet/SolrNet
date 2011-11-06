using System.Collections.Generic;

namespace SolrNet.Impl {
    public abstract class AbstractSolrQueryResults<T> : List<T> {
        public int NumFound { get; set; }
        public double? MaxScore { get; set; }

        public ResponseHeader Header { get; set; }

        public IDictionary<string, int> FacetQueries { get; set; }
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> FacetFields { get; set; }
        public IDictionary<string, DateFacetingResult> FacetDates { get; set; }
        public IDictionary<string, IList<Pivot>> FacetPivots { get; set; }

        public AbstractSolrQueryResults() {
            FacetQueries = new Dictionary<string, int>();
            FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            FacetDates = new Dictionary<string, DateFacetingResult>();
            FacetPivots = new Dictionary<string, IList<Pivot>>();
        }
    }
}