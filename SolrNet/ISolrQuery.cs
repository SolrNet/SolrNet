namespace SolrNet {
	/// <summary>
	/// Base query interface
	/// </summary>
	public interface ISolrQuery {
		/// <summary>
		/// query string
		/// </summary>
		string Query { get; }

	}
}