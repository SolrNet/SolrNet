using System;
using HttpWebAdapters;
using NUnit.Framework;

namespace SolrNet.Tests.Integration {
	[TestFixture]
	public class AddCommandTests {

		private static readonly string serverURL = "http://localhost:8983/solr";

		[Test]
		[Category("Integration")]
		[Ignore]
		public void AddOne() {
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(serverURL);
			TestDocument doc = new TestDocument();
			doc.Category = "cat";
			doc.Id = 123456;
			doc.Fecha = DateTime.Now;
			solr.Add(doc);
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void DeleteAll() {
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(serverURL);
			solr.Delete(new SolrQuery<TestDocument>("id:[* TO *]"));
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void AddMany() {
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(serverURL);
			TestDocument doc = new TestDocument();
			doc.Category = "cat";
			solr.Add(new TestDocument[] {doc, doc});
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void QueryAll() {
			ISolrOperations<TestDocument> solr = new SolrServer<TestDocument>(serverURL);
			ISolrQueryResults<TestDocument> r = solr.Query(new SolrQuery<TestDocument>("id[* TO *]"));
		}
	}
}