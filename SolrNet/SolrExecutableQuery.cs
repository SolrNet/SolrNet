using System;
using System.Collections.Generic;

namespace SolrNet {
	public class SolrExecutableQuery<T> : ISolrExecutableQuery<T> where T : ISolrDocument, new() {
		private ISolrConnection connection;
		private string query;
		private ISolrQueryResultParser<T> resultParser = new SolrQueryResultParser<T>();

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
			string r = connection.Get("/select", param);
			Console.WriteLine(r);
			ISolrQueryResults<T> qr = ResultParser.Parse(r);
			return qr;
		}

		public string Query {
			get { return query; }
		}

		public ISolrQueryResultParser<T> ResultParser {
			get { return resultParser; }
			set { resultParser = value; }
		}
	}
}