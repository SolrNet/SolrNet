using System.Collections.Generic;

namespace SolrNet {
	public interface ISolrFacetQuery {
		IEnumerable<KeyValuePair<string, string>> Query { get; }
	}
}