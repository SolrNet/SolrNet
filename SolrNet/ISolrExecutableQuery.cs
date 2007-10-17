namespace SolrNet {
	/// <summary>
	/// Executable query
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrExecutableQuery<T> : ISolrQuery<T> where T : ISolrDocument {
		/// <summary>
		/// Connection to use
		/// </summary>
		ISolrConnection Connection { get; set; }

		/// <summary>
		/// Executes the query and returns results
		/// </summary>
		/// <returns>query results</returns>
		ISolrQueryResults<T> Execute();
	}
}