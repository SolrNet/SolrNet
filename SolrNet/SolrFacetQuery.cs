using System.Collections.Generic;

namespace SolrNet {
	public class SolrFacetQuery : ISolrFacetQuery {
		public readonly string query;

		public SolrFacetQuery(ISolrQuery q) {
			query = q.Query;
		}

		public IEnumerable<KeyValuePair<string, string>> Query {
			get { yield return new KeyValuePair<string, string>("facet.query", query); }
		}
	}
}