using System.Collections.Generic;
using Rhino.Commons;

namespace SolrNet.DSL {
	/// <summary>
	/// Solr DSL Entry point
	/// </summary>
	public class Solr {
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
			Add<T>(new T[] {document});
		}

		public static void Add<T>(IEnumerable<T> documents) where T : ISolrDocument {
			AddCommand<T> cmd = new AddCommand<T>(documents);
			cmd.Execute(Connection);
		}

		public static ISolrQueryResults<T> Query<T>(string s, int start, int rows) where T : ISolrDocument, new() {
			ISolrQueryExecuter<T> q = new SolrQueryExecuter<T>(Connection, s);
			return q.Execute(start, rows);
		}

		public static ISolrQueryResults<T> Query<T>(string s) where T : ISolrDocument, new() {
			ISolrQueryExecuter<T> q = new SolrQueryExecuter<T>(Connection, s);
			return q.Execute();
		}

		public static ISolrQueryResults<T> Query<T>(ISolrQuery<T> q) where T : ISolrDocument, new() {
			ISolrQueryExecuter<T> queryExecuter = new SolrQueryExecuter<T>(Connection, q.Query);
			return queryExecuter.Execute();
		}

		public static ISolrQueryResults<T> Query<T>(ISolrQuery<T> q, int start, int rows) where T : ISolrDocument, new() {
			ISolrQueryExecuter<T> queryExecuter = new SolrQueryExecuter<T>(Connection, q.Query);
			return queryExecuter.Execute(start, rows);
		}

		public static IDSLQuery<T> Query<T>() where T : ISolrDocument, new() {
			return new DSLQuery<T>(Connection);
		}

		public static void Commit() {
			CommitCommand cmd = new CommitCommand();
			cmd.Execute(Connection);
		}

		public static void Commit(bool waitFlush, bool waitSearcher) {
			CommitCommand cmd = new CommitCommand();
			cmd.WaitFlush = waitFlush;
			cmd.WaitSearcher = waitSearcher;
			cmd.Execute(Connection);
		}

		public static void Optimize() {
			OptimizeCommand cmd = new OptimizeCommand();
			cmd.Execute(Connection);
		}

		public static void Optimize(bool waitFlush, bool waitSearcher) {
			OptimizeCommand cmd = new OptimizeCommand();
			cmd.WaitFlush = waitFlush;
			cmd.WaitSearcher = waitSearcher;
			cmd.Execute(Connection);
		}
	}
}