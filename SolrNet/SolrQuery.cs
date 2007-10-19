namespace SolrNet {
	/// <summary>
	/// Basic solr query
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrQuery<T> : AbstractSolrQuery<T> where T : ISolrDocument {
		private string query;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="query">solr query to execute</param>
		public SolrQuery(string query) {
			this.query = query;
		}

		/// <summary>
		/// query to execute
		/// </summary>
		public override string Query {
			get { return query; }
		}
	}
}