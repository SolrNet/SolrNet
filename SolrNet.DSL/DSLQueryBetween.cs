namespace SolrNet.DSL {
	public class DSLQueryBetween<T, RT> : IDSLQueryBetween<T, RT> where T : ISolrDocument, new() {
		private readonly string fieldName;
		private readonly ISolrConnection connection;
		private readonly ISolrQuery query;
		private readonly RT from;

		public DSLQueryBetween(string fieldName, ISolrConnection connection, ISolrQuery query, RT from) {
			this.fieldName = fieldName;
			this.connection = connection;
			this.query = query;
			this.from = from;
		}

		public IDSLQuery<T> And(RT i) {
			return new DSLQuery<T>(connection,
			                       new SolrMultipleCriteriaQuery<T>(
			                       	new ISolrQuery[] {
			                       	                    	query,
			                       	                    	new SolrQueryByRange<T, RT>(fieldName, from, i)
			                       	                    }));
		}
	}
}