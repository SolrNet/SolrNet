using System;

namespace SolrNet.Commands {
	public class QueryCommand<T> : ISolrCommand where T : ISolrDocument, new() {
		private string query;

		public string Query {
			get { return query; }
			set { query = value; }
		}

		public QueryCommand(string query) {
			this.query = query;
		}

		public string Execute(ISolrConnection connection) {
			throw new NotImplementedException(); // TODO finish!
			//ISolrQueryExecuter<T> q = new SolrQueryExecuter<T>(connection, query);
			//q.Execute();
		}
	}
}