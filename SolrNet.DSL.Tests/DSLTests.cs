using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Exceptions;

namespace SolrNet.DSL.Tests {
	/// <summary>
	/// These tests are more to define DSL syntax than anything else.
	/// </summary>
	[TestFixture]
	public class DSLTests {
		public class TestDocument : ISolrDocument {}

		public class TestDocumentWithId : ISolrDocument {
			private int id;

			[SolrField]
			public int Id {
				get { return id; }
				set { id = value; }
			}
		}

		private static readonly string response =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
";

		public delegate string Writer(string s, IDictionary<string, string> q);

		[Test]
		public void Add() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", "<add><doc /></add>")).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Add(new TestDocument());
			});
		}

		[Test]
		public void Commit() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Commit();
		}

		[Test]
		public void CommitWithParams() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Commit(true, true);
		}

		[Test]
		public void DeleteById() {
			const string id = "123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", string.Format("<delete><id>{0}</id></delete>", id))).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Delete.ById(id);
			});
		}

		[Test]
		public void DeleteByQuery() {
			const string q = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q))).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Delete.ByQuery(new SolrQuery<TestDocument>(q));
			});
		}

		[Test]
		public void DeleteByQueryString() {
			const string q = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q))).Return("");
			}).Verify(delegate {
				Solr.Connection = conn;
				Solr.Delete.ByQuery<TestDocument>(q);
			});
		}

		[Test]
		public void Optimize() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Optimize();
		}

		[Test]
		public void OptimizeWithParams() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Optimize(true, true);
		}

		[Test]
		public void OrderBy() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				IDictionary<string, string> query = new Dictionary<string, string>();
				query["q"] = "Id:123456";
				query["sort"] = "id asc";
				Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			}).Verify(delegate {
				Solr.Connection = conn;
				TestDocumentWithId doc = new TestDocumentWithId();
				doc.Id = 123456;
				Solr.Query<TestDocumentWithId>().ByExample(doc).OrderBy("id").Run();
			});
		}

		[Test]
		public void OrderBy2() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			const string queryString = "id:123";
			query["q"] = queryString;
			query["sort"] = "id asc";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query(new SolrQuery<TestDocument>(queryString), new SortOrder("id", Order.ASC));
			mocks.VerifyAll();
		}

		[Test]
		public void OrderBy2Multiple() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			const string queryString = "id:123";
			query["q"] = queryString;
			query["sort"] = "id asc,name desc";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query(new SolrQuery<TestDocument>(queryString), new SortOrder[] {new SortOrder("id", Order.ASC), new SortOrder("name", Order.DESC)});
			mocks.VerifyAll();
		}

		[Test]
		public void OrderByAscDesc() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "Id:123456";
			query["sort"] = "id asc";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			TestDocumentWithId doc = new TestDocumentWithId();
			doc.Id = 123456;
			Solr.Query<TestDocumentWithId>().ByExample(doc).OrderBy("id", Order.ASC).Run();
			mocks.VerifyAll();
		}

		[Test]
		public void OrderByAscDescMultiple() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "Id:123456";
			query["sort"] = "id asc,name desc";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			TestDocumentWithId doc = new TestDocumentWithId();
			doc.Id = 123456;
			Solr.Query<TestDocumentWithId>().ByExample(doc).OrderBy("id", Order.ASC).OrderBy("name", Order.DESC).Run();
			mocks.VerifyAll();
		}

		[Test]
		public void Query() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>("");
			Assert.AreEqual(1, r.NumFound);
			mocks.VerifyAll();
		}

		[Test]
		[ExpectedException(typeof (FieldNotFoundException))]
		public void Query_InvalidField_ShouldThrow() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(
				@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc><str name=""advancedview""/><str name=""basicview""/><int name=""id"">123456</int></doc></result>
</response>
");
			mocks.ReplayAll();
			Solr.Connection = conn;
			ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>("");
			mocks.VerifyAll();
		}

		[Test]
		public void QueryByAnyField() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "id:123456";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query<TestDocument>().By("id").Is("123456").Run();
			mocks.VerifyAll();
		}

		[Test]
		public void QueryByExample() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "Id:123456";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			TestDocumentWithId doc = new TestDocumentWithId();
			doc.Id = 123456;
			Solr.Query<TestDocumentWithId>().ByExample(doc).Run();
			mocks.VerifyAll();
		}

		[Test]
		public void QueryByRange() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "id:[123 TO 456]";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query<TestDocument>().ByRange("id", 123, 456).Run();
			mocks.VerifyAll();
		}

		[Test]
		public void QueryByRange_AnotherSyntax() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "id:[123 TO 456]";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query<TestDocument>().By("id").Between(123).And(456).Run();
			mocks.VerifyAll();
		}

		[Test]
		public void QueryByRangeConcatenable() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "id:[123 TO 456] p:[a TO z]";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query<TestDocument>().ByRange("id", 123, 456).ByRange("p", "a", "z").Run();
			mocks.VerifyAll();
		}

		[Test]
		public void QueryByRangeExclusive() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "id:{123 TO 456}";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query<TestDocument>().ByRange("id", 123, 456).Exclusive().Run();
			mocks.VerifyAll();
		}

		[Test]
		public void QueryByRangeInclusive() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			query["q"] = "id:[123 TO 456]";
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query<TestDocument>().ByRange("id", 123, 456).Inclusive().Run();
			mocks.VerifyAll();
		}

		[Test]
		public void QueryISolrQuery() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			const string queryString = "id:123";
			query["q"] = queryString;
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query(new SolrQuery<TestDocument>(queryString));
			mocks.VerifyAll();
		}

		[Test]
		public void QueryISolrQueryWithPagination() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			IDictionary<string, string> query = new Dictionary<string, string>();
			const string queryString = "id:123";
			query["q"] = queryString;
			query["start"] = 10.ToString();
			query["rows"] = 20.ToString();
			Expect.Call(conn.Get("/select", query)).Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Query(new SolrQuery<TestDocument>(queryString), 10, 20);
			mocks.VerifyAll();
		}

		[Test]
		public void QueryWithPagination() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(response);
			mocks.ReplayAll();
			Solr.Connection = conn;
			ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>("", 10, 20);
			Assert.AreEqual(1, r.NumFound);
			mocks.VerifyAll();
		}
	}
}