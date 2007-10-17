using System.Collections.Generic;

namespace SolrNet.DSL {
	/// <summary>
	/// TODO use ayende's semi-statics
	/// http://www.ayende.com/Blog/archive/7447.aspx
	/// https://rhino-tools.svn.sourceforge.net/svnroot/rhino-tools/trunk/rhino-commons/Rhino.Commons/LocalDataImpl/
	/// </summary>
	public class Solr {
		private static ISolrConnection connection;

		public static DeleteBy Delete {
			get { return new DeleteBy(Connection); }
		}

		public static ISolrConnection Connection {
			get { return connection; }
			set { connection = value; }
		}

		public static void Add<T>(T document) where T : ISolrDocument {
			Add<T>(new T[] {document});
		}

		public static void Add<T>(IEnumerable<T> documents) where T : ISolrDocument {
			AddCommand<T> cmd = new AddCommand<T>(documents);
			cmd.Execute(Connection);
		}

		public static ISolrQueryResults<T> Query<T>(string s) where T : ISolrDocument, new() {
			ISolrExecutableQuery<T> q = new SolrExecutableQuery<T>(Connection, s);
			return q.Execute();
		}

		public static ISolrQueryResults<T> Query<T>(ISolrQuery<T> q) where T : ISolrDocument, new() {
			ISolrExecutableQuery<T> query = new SolrExecutableQuery<T>(Connection, q.Query);
			return query.Execute();
		}

		public static void Commit() {
			CommitCommand cmd = new CommitCommand();
			cmd.Execute(connection);
		}

		public static void Commit(bool waitFlush, bool waitSearcher) {
			CommitCommand cmd = new CommitCommand();
			cmd.WaitFlush = waitFlush;
			cmd.WaitSearcher = waitSearcher;
			cmd.Execute(connection);
		}

		public static void Optimize() {
			OptimizeCommand cmd = new OptimizeCommand();
			cmd.Execute(connection);
		}

		public static void Optimize(bool waitFlush, bool waitSearcher) {
			OptimizeCommand cmd = new OptimizeCommand();
			cmd.WaitFlush = waitFlush;
			cmd.WaitSearcher = waitSearcher;
			cmd.Execute(connection);
		}
	}
}