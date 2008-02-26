namespace SolrNet {
	/// <summary>
	/// Basic solr query
	/// </summary>	
	public class SolrQuery : ISolrQuery {
		private readonly string query;

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
		public string Query {
			get { return query; }
		}
	}
}