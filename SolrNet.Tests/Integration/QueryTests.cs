using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace SolrNet.Tests.Integration {
	[TestFixture]
	public class QueryTests {
		[Test]
		public void tt() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> server = new SolrServer<TestDocument>(conn);
			SolrExecutableQuery<TestDocument> query = new SolrExecutableQuery<TestDocument>(conn, "id:123456");
			ISolrQueryResults<TestDocument> r = server.Query(query);
			Assert.Greater(0, r.Count);
		}
	}
}
