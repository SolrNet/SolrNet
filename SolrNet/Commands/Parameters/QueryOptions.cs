using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
	public class QueryOptions {
		public ICollection<string> Fields { get; set; }
		public ICollection<ISolrFacetQuery> FacetQueries { get; set; }
		public int? Start { get; set; }
		public int? Rows { get; set; }
		public ICollection<SortOrder> OrderBy { get; set; }
		public HighlightingParameters Highlight { get; set; }

		public QueryOptions() {
			OrderBy = new List<SortOrder>();
		}
	}
}