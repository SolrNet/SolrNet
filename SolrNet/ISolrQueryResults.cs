using System.Collections.Generic;

namespace SolrNet {
	/// <summary>
	/// Query results.
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrQueryResults<T> : IList<T>  {
		/// <summary>
		/// Total documents found
		/// </summary>
		int NumFound { get; }
		double? MaxScore { get; }

		IDictionary<string, int> FacetQueries { get; set; }
		IDictionary<string, ICollection<KeyValuePair<string, int>>> FacetFields { get; set; }

		ResponseHeader Header { get; set; }
		IDictionary<T, IDictionary<string, string>> Highlights { get; set; }
	}
}