using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.DSL;

namespace SolrNet.DSL.Tests {
	[TestFixture]
	public class DSLTests {

		public class TestDocument : ISolrDocument {}

		[Test]
		public void DeleteById() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
			Solr.Delete.ById("123456");
		}

		[Test]
		public void Add() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.ServerURL).Return("");
			mocks.ReplayAll();
			Solr.Connection = conn;
			Solr.Add(new TestDocument());
			mocks.VerifyAll();
		}

		[Test]
		public void Query() {
			ISolrQueryResults<TestDocument> r = Solr.Query<TestDocument>("");
		}

		[Test]
		public void DeleteByQuery() {
			Solr.Delete.ByQuery(new SolrQuery<TestDocument>("id:123456"));
		}
	}
}