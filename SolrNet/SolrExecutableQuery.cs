using System;
using System.Collections.Generic;

namespace SolrNet {
	public class SolrExecutableQuery<T> : ISolrExecutableQuery<T> {
		private ISolrConnection connection;
		private string query;

		public SolrExecutableQuery(ISolrConnection connection, string query) {
			this.connection = connection;
			this.query = query;
		}

		public ISolrConnection Connection {
			get { return connection; }
			set { connection = value; }
		}

		public ISolrQueryResults<T> Execute() {
			IDictionary<string, string> param = new Dictionary<string, string>();
			param["q"] = query;
			string r = connection.Get("/select/", param);
			Console.WriteLine(r);
			return null;
		}

		public string Query {
			get { return query; }
		}
	}
}