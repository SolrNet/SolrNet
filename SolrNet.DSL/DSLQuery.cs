using System;
using SolrNet;

namespace SolrNet.DSL {
	public class DSLQuery<T> : IDSLQuery<T> where T : ISolrDocument, new() {
		protected ISolrConnection connection;
		protected ISolrQuery<T> query;

		public DSLQuery(ISolrConnection connection) {
			this.connection = connection;
		}

		public DSLQuery(ISolrConnection connection, ISolrQuery<T> query) {
			this.connection = connection;
			this.query = query;
		}

		public IDSLQueryRange<T> ByRange<RT>(string fieldName, RT from, RT to) {
			return new DSLQueryRange<T, RT>(connection, query, fieldName, from, to);
		}

		public virtual ISolrQueryResults<T> Run() {
			return new SolrQueryExecuter<T>(connection, query).Execute();
		}

		public virtual ISolrQueryResults<T> Run(int start, int rows) {
			return new SolrQueryExecuter<T>(connection, query).Execute(start, rows);
		}

		public IDSLQueryBy<T> By(string fieldName) {
			return new DSLQueryBy<T>(fieldName, connection, query);
		}

		public IDSLQuery<T> ByExample(T doc) {
			return new DSLQuery<T>(connection,
			                       new SolrMultipleCriteriaQuery<T>(new ISolrQuery<T>[] {
			                                                                            	query,
			                                                                            	new SolrQueryByExample<T>(doc)
			                                                                            }));
		}
	}
}