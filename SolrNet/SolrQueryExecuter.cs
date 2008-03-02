using System.Collections.Generic;
using SolrNet.Commands.Parameters;
using SolrNet.Utils;

namespace SolrNet {
	/// <summary>
	/// Executable query
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class SolrQueryExecuter<T> : ISolrQueryExecuter<T> where T : ISolrDocument, new() {
		private ISolrQueryResultParser<T> resultParser = new SolrQueryResultParser<T>();

		/// <summary>
		/// Connection to use
		/// </summary>
		public ISolrConnection Connection { get; set; }

		public ISolrQuery Query { get; set; }

		public QueryOptions Options { get; set; }

		public SolrQueryExecuter(ISolrConnection connection, ISolrQuery query) {
			Connection = connection;
			Query = query;
		}

		public SolrQueryExecuter(ISolrConnection connection, string query) {
			Connection = connection;
			Query = new SolrQuery(query);
		}

		/// <summary>
		/// Executes the query and returns results
		/// </summary>
		/// <returns>query results</returns>
		public ISolrQueryResults<T> Execute() {
			IDictionary<string, string> param = new Dictionary<string, string>();
			param["q"] = Query.Query;
			if (Options != null) {
				if (Options.Start.HasValue)
					param["start"] = Options.Start.ToString();
				if (Options.Rows.HasValue)
					param["rows"] = Options.Rows.ToString();
				if (Options.OrderBy != null && Options.OrderBy.Count > 0)
					param["sort"] = Func.Join(",", Options.OrderBy);				
			}
			string r = Connection.Get("/select", param);
			var qr = ResultParser.Parse(r);
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