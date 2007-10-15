using System.Collections.Generic;

namespace SolrNet.DSL {
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
	}
}