namespace SolrNet {
	/// <summary>
	/// Query results parser interface
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public interface ISolrQueryResultParser<T> {
		/// <summary>
		/// Parses solr's response
		/// </summary>
		/// <param name="r">solr response</param>
		/// <returns>query results</returns>
		ISolrQueryResults<T> Parse(string r);
	}
}