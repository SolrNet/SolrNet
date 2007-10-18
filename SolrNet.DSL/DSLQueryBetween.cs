namespace SolrNet.DSL {
	public class DSLQueryBetween<T, RT> : IDSLQueryBetween<T, RT> where T : ISolrDocument, new() {
		private string fieldName;
		private ISolrConnection connection;
		private ISolrQuery<T> query;
		private RT from;

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