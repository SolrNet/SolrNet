using System;
using HttpWebAdapters;
using NUnit.Framework;

namespace SolrNet.Tests.Integration {
	[TestFixture]
	public class AddCommandTests {
		[Test]
		[Category("Integration")]
		public void AddOne() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(conn);
			TestDocument doc = new TestDocument();
			doc.Category = "cat";
			doc.Id = 123456;
			doc.Fecha = DateTime.Now;
			solr.Add(doc);
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		public void DeleteAll() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(conn);
			solr.Delete(new SolrQuery<TestDocument>("id:[* TO *]"));
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		public void AddMany() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(conn);
			TestDocument doc = new TestDocument();
			doc.Category = "cat";
			solr.Add(new TestDocument[] {doc, doc});
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		public void QueryAll() {
			ISolrConnection conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(conn);
			ISolrQueryResults<TestDocument> r = solr.Query(new SolrQuery<TestDocument>("id[* TO *]"));
		}
	}
}