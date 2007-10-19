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
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Commit();
		}

		[Test]
		public void CommitWithOptions() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("/update", "<commit waitFlush=\"true\" waitSearcher=\"true\"/>")).Repeat.Once().Return(
				null);
			mocks.ReplayAll();
			ISolrOperations<TestDocumentWithoutUniqueKey> ops = new SolrServer<TestDocumentWithoutUniqueKey>(connection);
			ops.Commit(true, true);
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
		public void PaginatedQuery() {
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
	}
}