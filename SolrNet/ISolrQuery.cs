namespace SolrNet {
	/// <summary>
	/// Base query interface
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrQuery<T> where T : ISolrDocument {
		/// <summary>
		/// query string
		/// </summary>
		string Query { get; }

	}
}