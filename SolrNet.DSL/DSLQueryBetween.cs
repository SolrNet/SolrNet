namespace SolrNet.DSL {
	public class DSLQueryBetween<T, RT> : IDSLQueryBetween<T, RT> where T : ISolrDocument, new() {
		private readonly string fieldName;
		private readonly ISolrConnection connection;
		private readonly ISolrQuery<T> query;
		private readonly RT from;

		public DSLQueryBetween(string fieldName, ISolrConnection connection, ISolrQuery<T> query, RT from) {
			this.fieldName = fieldName;
			this.connection = connection;
			this.query = query;
			this.from = from;
		}

		public IDSLQuery<T> And(RT i) {
			return new DSLQuery<T>(connection,
			                       new SolrMultipleCriteriaQuery<T>(
			                       	new ISolrQuery<T>[] {
			                       	                    	query,
			                       	                    	new SolrQueryByRange<T, RT>(fieldName, from, i)
			                       	                    }));
		}
	}
}