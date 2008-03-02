namespace SolrNet.DSL {
	public class DSLQueryBy<T> : IDSLQueryBy<T> where T : ISolrDocument, new() {
		private readonly string fieldName;
		private readonly ISolrConnection connection;
		private readonly ISolrQuery query;

		public DSLQueryBy(string fieldName, ISolrConnection connection, ISolrQuery query) {
			this.fieldName = fieldName;
			this.connection = connection;
			this.query = query;
		}

		public IDSLQuery<T> Is(string s) {
			return new DSLQuery<T>(connection,
			                       new SolrMultipleCriteriaQuery(new[] {
			                       	query,
			                       	new SolrQueryByField(fieldName, s)
			                       }));
		}

		public IDSLQueryBetween<T, RT> Between<RT>(RT i) {
			return new DSLQueryBetween<T, RT>(fieldName, connection, query, i);
		}
	}
}