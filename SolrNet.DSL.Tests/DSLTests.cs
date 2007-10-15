using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.DSL;

namespace SolrNet.DSL.Tests {
	[TestFixture]
	public class DSLTests {
		[TestFixtureSetUp]
		public void setup() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Solr.Connection = conn;
		}

		public class TestDocument : ISolrDocument {}

		[Test]
		public void DeleteById() {
			Solr.Delete.ById("123456");
		}

		[Test]
		public void Add() {
			Solr.Add(new TestDocument());
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