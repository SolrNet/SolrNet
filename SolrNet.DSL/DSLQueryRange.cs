namespace SolrNet.DSL {
	public class DSLQueryRange<T, RT> : DSLQuery<T>, IDSLQueryRange<T> where T : ISolrDocument, new() {
		private string fieldName;
		private RT from;
		private RT to;
		private ISolrQuery<T> prevQuery;

		public DSLQueryRange(ISolrConnection connection, ISolrQuery<T> query, string fieldName, RT from, RT to) : base(connection) {
			this.query = new SolrMultipleCriteriaQuery<T>(new ISolrQuery<T>[] {
			                                                                  	query,
			                                                                  	new SolrQueryByRange<T, RT>(fieldName, from, to)
			                                                                  });
			prevQuery = query;
			this.fieldName = fieldName;
			this.from = from;
			this.to = to;
		}

		private ISolrQuery<T> buildFinalQuery(bool inclusive) {
			return new SolrMultipleCriteriaQuery<T>(new ISolrQuery<T>[] {
			                                                            	prevQuery,
			                                                            	new SolrQueryByRange<T, RT>(fieldName, from, to, inclusive)
			                                                            });
		}

		public IDSLQuery<T> Exclusive() {
			return new DSLQuery<T>(connection, buildFinalQuery(false));
		}

		public IDSLQuery<T> Inclusive() {
			return new DSLQuery<T>(connection, buildFinalQuery(true));
		}
	}
}