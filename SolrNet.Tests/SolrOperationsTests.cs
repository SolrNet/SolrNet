using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrOperationsTests {
		public class TestDocumentWithoutUniqueKey : ISolrDocument {}

		public class TestDocumentWithUniqueKey : ISolrDocument {
			[SolrUniqueKey]
			public int id {
				get { return 0; }
			}
		}

		[Test]
		public void Commit() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<commit />")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Commit();
			mocks.VerifyAll();
		}

		[Test]
		public void CommitWithOptions() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<commit waitSearcher=\"true\" waitFlush=\"true\" />")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Commit(true, true);
			mocks.VerifyAll();
		}

		[Test]
		public void Optimize() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<optimize />")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Optimize();
			mocks.VerifyAll();
		}

		[Test]
		public void OptimizeWithOptions() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<optimize waitSearcher=\"true\" waitFlush=\"true\" />")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Optimize(true, true);
			mocks.VerifyAll();
		}

		[Test]
		public void SearchResults_ShouldBeIterable() {
			MockRepository mocks = new MockRepository();
			ISolrQueryResults<ISolrDocument> results = mocks.CreateMock<ISolrQueryResults<ISolrDocument>>();
			Assert.IsInstanceOfType(typeof (IEnumerable<ISolrDocument>), results);
		}

		[Test]
		public void Add() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<add><doc /></add>")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Add(new TestDocumentWithoutUniqueKey());
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof (NoUniqueKeyException))]
		public void DeleteDocumentWithoutUniqueKey_ShouldThrow() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Delete(new TestDocumentWithoutUniqueKey());
			mocks.VerifyAll();
		}

		[Test]
		public void DeleteDocumentWithUniqueKey() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<delete><id>0</id></delete>")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithUniqueKey> ops = new SolrServer<TestDocumentWithUniqueKey>(connection);
			ops.Delete(new TestDocumentWithUniqueKey());
			mocks.VerifyAll();
		}

		[Test]
		public void DeleteDocumentWithUniqueKeyWithParams() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<delete fromPending=\"true\" fromCommitted=\"false\"><id>0</id></delete>")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithUniqueKey> ops = new SolrServer<TestDocumentWithUniqueKey>(connection);
			ops.Delete(new TestDocumentWithUniqueKey(), true, false);
			mocks.VerifyAll();
		}

		[Test]
		public void DeleteByQuery() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<delete><query>id:123</query></delete>")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithUniqueKey> ops = new SolrServer<TestDocumentWithUniqueKey>(connection);
			ops.Delete(new SolrQuery<TestDocumentWithUniqueKey>("id:123"));
			mocks.VerifyAll();
		}

		[Test]
		public void DeleteByQueryWithParams() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<delete fromPending=\"true\" fromCommitted=\"true\"><query>id:123</query></delete>")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithUniqueKey> ops = new SolrServer<TestDocumentWithUniqueKey>(connection);
			ops.Delete(new SolrQuery<TestDocumentWithUniqueKey>("id:123"), true, true);
			mocks.VerifyAll();
		}

		[Test]
		public void QueryWithPagination() {
			const string qstring = "id:123";
			const int start = 10;
			const int rows = 20;

			MockRepository mocks = new MockRepository();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = qstring;
			query["start"] = start.ToString();
			query["rows"] = rows.ToString();
			Expect.Call(connection.Get("/select", query)).Repeat.Once().Return("");

			ISolrQueryResultParser<TestDocumentWithUniqueKey> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
			SetupResult.For(parser.Parse(null)).IgnoreArguments().Return(new SolrQueryResults<TestDocumentWithUniqueKey>());

			mocks.ReplayAll();
			SolrServer<TestDocumentWithUniqueKey> solr = new SolrServer<TestDocumentWithUniqueKey>(connection);
			solr.ResultParser = parser;
			ISolrQueryResults<TestDocumentWithUniqueKey> r = solr.Query(new SolrQuery<TestDocumentWithUniqueKey>(qstring), start, rows);
			mocks.VerifyAll();
		}

		[Test]
		public void QueryWithSort() {
			const string qstring = "id:123";

			MockRepository mocks = new MockRepository();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = qstring;
			query["sort"] = "id ASC,name DESC";
			Expect.Call(connection.Get("/select", query)).Repeat.Once().Return("");

			ISolrQueryResultParser<TestDocumentWithUniqueKey> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
			SetupResult.For(parser.Parse(null)).IgnoreArguments().Return(new SolrQueryResults<TestDocumentWithUniqueKey>());

			mocks.ReplayAll();
			SolrServer<TestDocumentWithUniqueKey> solr = new SolrServer<TestDocumentWithUniqueKey>(connection);
			solr.ResultParser = parser;
			ISolrQueryResults<TestDocumentWithUniqueKey> r = solr.Query(new SolrQuery<TestDocumentWithUniqueKey>(qstring), new SortOrder[] { new SortOrder("id", Order.ASC), new SortOrder("name", Order.DESC) });
			mocks.VerifyAll();
		}

		[Test]
		public void QueryWithSortAndPagination() {
			const string qstring = "id:123";
			const int start = 10;
			const int rows = 20;

			MockRepository mocks = new MockRepository();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = qstring;
			query["start"] = start.ToString();
			query["rows"] = rows.ToString();
			query["sort"] = "id ASC,name DESC";
			Expect.Call(connection.Get("/select", query)).Repeat.Once().Return("");

			ISolrQueryResultParser<TestDocumentWithUniqueKey> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
			SetupResult.For(parser.Parse(null)).IgnoreArguments().Return(new SolrQueryResults<TestDocumentWithUniqueKey>());

			mocks.ReplayAll();
			SolrServer<TestDocumentWithUniqueKey> solr = new SolrServer<TestDocumentWithUniqueKey>(connection);
			solr.ResultParser = parser;
			ISolrQueryResults<TestDocumentWithUniqueKey> r = solr.Query(new SolrQuery<TestDocumentWithUniqueKey>(qstring), start, rows, new SortOrder[] {new SortOrder("id", Order.ASC), new SortOrder("name", Order.DESC)  });
			mocks.VerifyAll();			
		}
	}
}