using HttpWebAdapters;
using NUnit.Framework;

namespace SolrNet.Tests.Integration {
	[TestFixture]
	public class QueryTests {
		[Test]
		public void tt() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> server = new SolrServer<TestDocument>(conn);
			ISolrQuery<TestDocument> query = new SolrQuery<TestDocument>("id:123456");
			ISolrQueryResults<TestDocument> r = server.Query(query);
			Assert.Greater(r.Count, 0);
			TestDocument doc = r[0];
			Assert.AreEqual(123456, doc.Id);
		}
	}
}