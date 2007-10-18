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

		[Test]
		public void DeleteById() {
			const string id = "123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Post("/update", string.Format("<delete><id>{0}</id></delete>", id))).Return("");
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Delete.ById(id);
			mocks.VerifyAll();
		}

		[Test]
		public void Add() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Post("/update", "<add><doc /></add>")).Return("");
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Add(new TestDocument());
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

		private static readonly string response =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
";

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

		public delegate string Writer(string s, IDictionary<string, string> q);

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
		public void DeleteByQuery() {
			const string q = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q))).Return("");
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Delete.ByQuery(new SolrQuery<TestDocument>(q));
			mocks.VerifyAll();
		}

		[Test]
		public void DeleteByQueryString() {
			const string q = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q))).Return("");
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Delete.ByQuery<TestDocument>(q);
			mocks.VerifyAll();
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
	}
}