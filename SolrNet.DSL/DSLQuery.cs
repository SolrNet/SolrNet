namespace SolrNet.DSL {
	public class DSLQuery<T> : IDSLQuery<T> where T : ISolrDocument, new() {
		private ISolrConnection connection;
		private ISolrQuery<T> query;

		public DSLQuery(ISolrConnection connection) {
			this.connection = connection;
		}

		public DSLQuery(ISolrConnection connection, ISolrQuery<T> query) {
			this.connection = connection;
			this.query = query;
		}

		public IDSLQuery<T> ByRange<RT>(string fieldName, RT from, RT to) {
			return
				new DSLQuery<T>(connection,
				                new SolrMultipleCriteriaQuery<T>(
				                	new ISolrQuery<T>[] {query, new SolrQueryByRange<T, RT>(fieldName, from, to)}));
		}

		public ISolrQueryResults<T> Run() {
			return new SolrExecutableQuery<T>(connection, query.Query).Execute();
		}

		public IDSLQueryBy<T> By(string fieldName) {
			return new DSLQueryBy<T>(fieldName, connection, query);
		}
	}
}