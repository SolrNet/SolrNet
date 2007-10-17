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

		[Test]
		public void Query() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(
				@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
");
			mocks.ReplayAll();
			Solr.Connection = conn;
			ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>("");
			Assert.AreEqual(1, r.NumFound);
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