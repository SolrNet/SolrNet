using System.Collections.Generic;

namespace SolrNet.DSL {
	public class DSLRun<T> : IDSLRun<T> where T : ISolrDocument, new() {
		private ICollection<SortOrder> order = new List<SortOrder>();
		protected ISolrConnection connection;
		protected ISolrQuery<T> query;

		public DSLRun(ISolrConnection connection) {
			this.connection = connection;
		}

		public DSLRun(ISolrConnection connection, ISolrQuery<T> query) {
			this.connection = connection;
			this.query = query;
		}

		public DSLRun(ISolrConnection connection, ISolrQuery<T> query, ICollection<SortOrder> order) {
			this.connection = connection;
			this.query = query;
			this.order = order;
		}

		public ISolrQueryResults<T> Run() {
			ISolrQueryExecuter<T> exe = new SolrQueryExecuter<T>(connection, query);
			exe.OrderBy = order;
			return exe.Execute();
		}

		public ISolrQueryResults<T> Run(int start, int rows) {
			ISolrQueryExecuter<T> exe = new SolrQueryExecuter<T>(connection, query);
			exe.OrderBy = order;
			return exe.Execute(start, rows);
		}

		public IDSLRun<T> OrderBy(string fieldName) {
			List<SortOrder> newOrder = new List<SortOrder>(order);
			newOrder.Add(new SortOrder(fieldName));
			return new DSLRun<T>(connection, query, newOrder);
		}

		public IDSLRun<T> OrderBy(string fieldName, Order o) {
			List<SortOrder> newOrder = new List<SortOrder>(order);
			newOrder.Add(new SortOrder(fieldName, o));
			return new DSLRun<T>(connection, query, newOrder);
		}
	}
}