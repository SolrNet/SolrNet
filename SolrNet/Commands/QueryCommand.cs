using System;

namespace SolrNet.Commands {
	public class QueryCommand: ISolrCommand {
		public string Query { get; set; }

		public QueryCommand(string query) {
			this.Query = query;
		}

		public string Execute(ISolrConnection connection) {
			throw new NotImplementedException(); // TODO finish!
			//ISolrQueryExecuter<T> q = new SolrQueryExecuter<T>(connection, query);
			//q.Execute();
		}
	}
}