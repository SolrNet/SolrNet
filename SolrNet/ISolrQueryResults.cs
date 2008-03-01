using System.Collections.Generic;

namespace SolrNet {
	/// <summary>
	/// Query results.
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrQueryResults<T> : IList<T> where T : ISolrDocument {
		/// <summary>
		/// Total documents found
		/// </summary>
		int NumFound { get; }

		IDictionary<string, int> FacetQueries { get; set; }
		IDictionary<string, int> FacetFields { get; set; }
	}
}