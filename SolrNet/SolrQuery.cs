namespace SolrNet {
	public class SolrQuery<T> : ISolrQuery<T> where T : ISolrDocument {
		private string query;

		public SolrQuery(string query) {
			this.query = query;
		}

		public string Query {
			get { return query; }
		}
	}
}