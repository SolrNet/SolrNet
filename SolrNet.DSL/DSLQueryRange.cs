namespace SolrNet.DSL {
	public class DSLQueryRange<T, RT> : DSLQuery<T>, IDSLQueryRange<T> where T : ISolrDocument, new() {
		private readonly string fieldName;
		private readonly RT from;
		private readonly RT to;
		private readonly ISolrQuery prevQuery;

		public DSLQueryRange(ISolrConnection connection, ISolrQuery query, string fieldName, RT from, RT to) : base(connection) {
			this.query = new SolrMultipleCriteriaQuery(new[]
			                                              	{
			                                              		query,
			                                              		new SolrQueryByRange<T, RT>(fieldName, from, to)
			                                              	});
			prevQuery = query;
			this.fieldName = fieldName;
			this.from = from;
			this.to = to;
		}

		private ISolrQuery buildFinalQuery(bool inclusive) {
			return new SolrMultipleCriteriaQuery(new[]
			                                        	{
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