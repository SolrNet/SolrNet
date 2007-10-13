namespace SolrNet.Tests {
	public class DeleteByQueryParam<T> : ISolrDeleteParam {
		private ISolrQuery<T> query;

		public DeleteByQueryParam(ISolrQuery<T> q) {
			query = q;
		}

		public override string ToString() {
			return string.Format("<query>{0}</query>", query);
		}
	}
}