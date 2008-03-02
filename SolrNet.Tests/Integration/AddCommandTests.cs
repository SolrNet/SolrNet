using System;
using NUnit.Framework;

namespace SolrNet.Tests.Integration {
	[TestFixture]
	public class AddCommandTests {
		private const string serverURL = "http://nippur:8081/solr";

		[Test]
		[Category("Integration")]
		[Ignore]
		public void AddOne() {
			var solr = new SolrServer<TestDocument>(serverURL);
			var doc = new TestDocument {Category = "cat", Id = 123456, Fecha = DateTime.Now};
			solr.Add(doc);
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void AddOneWithArray() {
			var solr = new SolrServer<TestDocument>(serverURL);
			var doc = new TestDocument {
				Id = 123,
				Series = new[] {1, 2, 3},
			};
			solr.Add(doc);
			Console.WriteLine(solr.Commit());
		}


		[Test]
		[Category("Integration")]
		[Ignore]
		public void DeleteAll() {
			var solr = new SolrServer<TestDocument>(serverURL);
			solr.Delete(new SolrQuery("id:[* TO *]"));
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void AddMany() {
			var solr = new SolrServer<TestDocument>(serverURL);
			var doc = new TestDocument {Category = "cat"};
			solr.Add(new[] {doc, doc});
			Console.WriteLine(solr.Commit());
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void QueryAll() {
			var solr = new SolrServer<TestDocument>(serverURL);
			var r = solr.Query(new SolrQuery("id[* TO *]"));
		}
	}
}