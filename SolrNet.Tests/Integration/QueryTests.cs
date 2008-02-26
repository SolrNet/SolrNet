using HttpWebAdapters;
using NUnit.Framework;

namespace SolrNet.Tests.Integration {
	[TestFixture]
	public class QueryTests {
		[Test]
		[Category("Integration")]
		[Ignore]
		public void tt() {
			var conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			var server = new SolrServer<TestDocument>(conn);
			var query = new SolrQuery("id:123456");
			var r = server.Query(query);
			Assert.Greater(r.Count, 0);
			var doc = r[0];
			Assert.AreEqual(123456, doc.Id);
		}
	}
}