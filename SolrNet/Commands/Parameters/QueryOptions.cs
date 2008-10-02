using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
	public class QueryOptions {
		/// <summary>
		/// Fields to retrieve.
		/// By default, all stored fields are returned
		/// </summary>
		public ICollection<string> Fields { get; set; }

		/// <summary>
		/// Collection of facet queries
		/// </summary>
		public ICollection<ISolrFacetQuery> FacetQueries { get; set; }

		/// <summary>
		/// Offset in the complete result set for the queries where the set of returned documents should begin
		/// Default is 0
		/// </summary>
		public int? Start { get; set; }

		/// <summary>
		/// Maximum number of documents from the complete result set to return to the client for every request.
		/// Default is 10
		/// </summary>
		public int? Rows { get; set; }

		/// <summary>
		/// Sort order.
		/// By default, it's "score desc"
		/// </summary>
		public ICollection<SortOrder> OrderBy { get; set; }

		/// <summary>
		/// Highlighting parameters
		/// </summary>
		public HighlightingParameters Highlight { get; set; }

		public QueryOptions() {
			OrderBy = new List<SortOrder>();
		}
	}
}