using System.Collections.Generic;
using Rhino.Commons;
using SolrNet.Commands;

namespace SolrNet.DSL {
	/// <summary>
	/// Solr DSL Entry point
	/// </summary>
	public static class Solr {
		public static DeleteBy Delete {
			get { return new DeleteBy(Connection); }
		}

		public static readonly string SolrConnectionKey = "ISolrConnection";

		/// <summary>
		/// thread-local or webcontext-local connection
		/// </summary>
		/// <seealso cref="http://www.ayende.com/Blog/archive/7447.aspx"/>
		/// <seealso cref="http://rhino-tools.svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/LocalDataImpl/"/>
		public static ISolrConnection Connection {
			private get { return (ISolrConnection) Local.Data[SolrConnectionKey]; }
			set { Local.Data[SolrConnectionKey] = value; }
		}

		public static void Add<T>(T document) where T : ISolrDocument {
			Add<T>(new[] {document});
		}

		public static void Add<T>(IEnumerable<T> documents) where T : ISolrDocument {
			var cmd = new AddCommand<T>(documents);
			cmd.Execute(Connection);
		}

		public static ISolrQueryResults<T> Query<T>(string s, int start, int rows) where T : ISolrDocument, new() {
			var q = new SolrQueryExecuter<T>(Connection, s);
			return q.Execute(start, rows);
		}

		public static ISolrQueryResults<T> Query<T>(string s) where T : ISolrDocument, new() {
			var q = new SolrQueryExecuter<T>(Connection, s);
			return q.Execute();
		}

		public static ISolrQueryResults<T> Query<T>(string s, SortOrder order) where T : ISolrDocument, new() {
			return Query<T>(s, new[] {order});
		}

		public static ISolrQueryResults<T> Query<T>(string s, ICollection<SortOrder> order) where T : ISolrDocument, new() {
			var q = new SolrQueryExecuter<T>(Connection, s) {OrderBy = order};
			return q.Execute();
		}

		public static ISolrQueryResults<T> Query<T>(string s, SortOrder order, int start, int rows) where T : ISolrDocument, new() {
			return Query<T>(s, new[] {order}, start, rows);
		}

		public static ISolrQueryResults<T> Query<T>(string s, ICollection<SortOrder> order, int start, int rows) where T : ISolrDocument, new() {
			var q = new SolrQueryExecuter<T>(Connection, s) {OrderBy = order};
			return q.Execute(start, rows);
		}

		public static ISolrQueryResults<T> Query<T>(ISolrQuery q) where T : ISolrDocument, new() {
			var queryExecuter = new SolrQueryExecuter<T>(Connection, q.Query);
			return queryExecuter.Execute();
		}

		public static ISolrQueryResults<T> Query<T>(ISolrQuery q, int start, int rows) where T : ISolrDocument, new() {
			var queryExecuter = new SolrQueryExecuter<T>(Connection, q.Query);
			return queryExecuter.Execute(start, rows);
		}

		public static ISolrQueryResults<T> Query<T>(SolrQuery query, SortOrder order) where T : ISolrDocument, new() {
			return Query<T>(query, new[] {order});
		}

		public static ISolrQueryResults<T> Query<T>(SolrQuery query, ICollection<SortOrder> orders) where T : ISolrDocument, new() {
			var queryExecuter = new SolrQueryExecuter<T>(Connection, query.Query) {OrderBy = orders};
			return queryExecuter.Execute();
		}

		public static IDSLQuery<T> Query<T>() where T : ISolrDocument, new() {
			return new DSLQuery<T>(Connection);
		}

		public static void Commit() {
			var cmd = new CommitCommand();
			cmd.Execute(Connection);
		}

		public static void Commit(bool waitFlush, bool waitSearcher) {
			var cmd = new CommitCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
			cmd.Execute(Connection);
		}

		public static void Optimize() {
			var cmd = new OptimizeCommand();
			cmd.Execute(Connection);
		}

		public static void Optimize(bool waitFlush, bool waitSearcher) {
			var cmd = new OptimizeCommand {WaitFlush = waitFlush, WaitSearcher = waitSearcher};
			cmd.Execute(Connection);
		}
	}
}