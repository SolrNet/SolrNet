using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet.DSL {
	public class DSLRun<T> : IDSLRun<T> where T : ISolrDocument, new() {
		private readonly ICollection<SortOrder> order = new List<SortOrder>();
		protected ISolrConnection connection;
		protected ISolrQuery query;

		public DSLRun(ISolrConnection connection) {
			this.connection = connection;
		}

		public DSLRun(ISolrConnection connection, ISolrQuery query) {
			this.connection = connection;
			this.query = query;
		}

		public DSLRun(ISolrConnection connection, ISolrQuery query, ICollection<SortOrder> order) {
			this.connection = connection;
			this.query = query;
			this.order = order;
		}

		public ISolrQueryResults<T> Run() {
			var exe = new SolrQueryExecuter<T>(connection, query) {Options = new QueryOptions {OrderBy = order}};
			return exe.Execute();
		}

		public ISolrQueryResults<T> Run(int start, int rows) {
			var exe = new SolrQueryExecuter<T>(connection, query)
			          	{
			          		Options = new QueryOptions {OrderBy = order, Start = start, Rows = rows}
			          	};
			return exe.Execute();
		}

		public IDSLRun<T> OrderBy(string fieldName) {
			var newOrder = new List<SortOrder>(order) {new SortOrder(fieldName)};
			return new DSLRun<T>(connection, query, newOrder);
		}

		public IDSLRun<T> OrderBy(string fieldName, Order o) {
			var newOrder = new List<SortOrder>(order) {new SortOrder(fieldName, o)};
			return new DSLRun<T>(connection, query, newOrder);
		}
	}
}