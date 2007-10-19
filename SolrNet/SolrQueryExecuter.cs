using System;
using System.Collections.Generic;

namespace SolrNet {
	/// <summary>
	/// Executable query
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrQueryExecuter<T> : ISolrQueryExecuter<T> where T : ISolrDocument, new() {
		private ISolrConnection connection;
		private ISolrQueryResultParser<T> resultParser = new SolrQueryResultParser<T>();
		private ISolrQuery<T> query;

		/// <summary>
		/// Connection to use
		/// </summary>
		public ISolrConnection Connection {
			get { return connection; }
			set { connection = value; }
		}

		public ISolrQuery<T> Query {
			get { return query; }
			set { query = value; }
		}

		public SolrQueryExecuter(ISolrConnection connection, ISolrQuery<T> query) {
			this.connection = connection;
			this.query = query;
		}

		public SolrQueryExecuter(ISolrConnection connection, string query) {
			this.connection = connection;
			this.query = new SolrQuery<T>(query);
		}

		/// <summary>
		/// Executes the query and returns results
		/// </summary>
		/// <returns>query results</returns>
		public ISolrQueryResults<T> Execute() {
			return Execute(-1, -1);
		}

		public ISolrQueryResults<T> Execute(int start, int rows) {
			IDictionary<string, string> param = new Dictionary<string, string>();
			param["q"] = query.Query;
			if (start != -1)
				param["start"] = start.ToString();
			if (rows != -1)
				param["rows"] = rows.ToString();
			string r = connection.Get("/select", param);
			Console.WriteLine(r);
			ISolrQueryResults<T> qr = ResultParser.Parse(r);
			return qr;
		}

		/// <summary>
		/// Solr response parser, default is XML response parser
		/// </summary>
		public ISolrQueryResultParser<T> ResultParser {
			get { return resultParser; }
			set { resultParser = value; }
		}
	}
}