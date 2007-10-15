using NUnit.Framework;
using SolrNet.DSL;

namespace SolrNet.Tests {
	[TestFixture]
	public class DSLTests {
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