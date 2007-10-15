namespace SolrNet.Tests {
	public class DeleteByQueryParam<T> : ISolrDeleteParam where T : ISolrDocument {
		private ISolrQuery<T> query;

		public DeleteByQueryParam(ISolrQuery<T> q) {
			query = q;
		}

		public string ToXmlString() {
			return string.Format("<query>{0}</query>", query);
		}
	}
}