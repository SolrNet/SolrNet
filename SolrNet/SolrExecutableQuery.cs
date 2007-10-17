using System;
using System.Collections.Generic;

namespace SolrNet {
	/// <summary>
	/// Executable query
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrExecutableQuery<T> : ISolrExecutableQuery<T> where T : ISolrDocument, new() {
		private ISolrConnection connection;
		private string query;
		private ISolrQueryResultParser<T> resultParser = new SolrQueryResultParser<T>();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="connection">connection to use</param>
		/// <param name="query">query to execute</param>
		public SolrExecutableQuery(ISolrConnection connection, string query) {
			this.connection = connection;
			this.query = query;
		}

		/// <summary>
		/// Connection to use
		/// </summary>
		public ISolrConnection Connection {
			get { return connection; }
			set { connection = value; }
		}

		/// <summary>
		/// Executes the query and returns results
		/// </summary>
		/// <returns>query results</returns>
		public ISolrQueryResults<T> Execute() {
			IDictionary<string, string> param = new Dictionary<string, string>();
			param["q"] = query;
			string r = connection.Get("/select", param);
			Console.WriteLine(r);
			ISolrQueryResults<T> qr = ResultParser.Parse(r);
			return qr;
		}

		/// <summary>
		/// query string
		/// </summary>
		public string Query {
			get { return query; }
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