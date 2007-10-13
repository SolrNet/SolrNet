using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class DSLTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void tt() {
			Solr.Delete.ById("123456");
			Solr.Add(new TestDocument());
			ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>("");
		}
	}

	public class Solr {
		private static ISolrConnection connection;

		public static DeleteParam Delete {
			get { return new DeleteParam(Connection); }
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

		public static ISolrQueryResults<T> Query<T>(string s) where T : ISolrDocument {
			throw new NotImplementedException();
		}
	}

	public class DeleteParam {
		private ISolrConnection connection;

		public DeleteParam(ISolrConnection connection) {
			this.connection = connection;
		}

		public void ById(string id) {
			DeleteCommand cmd = new DeleteCommand();
			cmd.DeleteParam = new DeleteByIdParam(id);
			cmd.Execute(connection);
		}

		public void ByQuery<T>(ISolrQuery<T> q) {
			DeleteCommand cmd = new DeleteCommand();
			cmd.DeleteParam = new DeleteByQueryParam<T>(q);
			cmd.Execute(connection);
		}
	}
}