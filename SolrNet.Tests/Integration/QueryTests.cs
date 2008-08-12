using System;
using HttpWebAdapters;
using NUnit.Framework;
using SolrNet.Commands.Parameters;

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

		[Test]
		[Category("Integration")]
		[Ignore]
		public void Query_with_result_fields() {
			var conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			var server = new SolrServer<TestDocument>(conn);
			var query = new SolrQuery("*:*");
			var r = server.Query(query, new QueryOptions {Fields = new[] {"id"}});
			Assert.Greater(r.Count, 0);
			var doc = r[0];
			Assert.IsNull(doc.State);
			Assert.IsNull(doc.Make);
			Assert.IsNull(doc.Model);
			Assert.IsNull(doc.Style);
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void RandomSort() {
			var conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			var server = new SolrServer<TestDocument>(conn);
			var query = new SolrQuery("*:*");
			var r = server.Query(query, new QueryOptions {
				OrderBy = SortOrder.Random,
				Rows = 5,
			});
			Assert.Greater(r.Count, 0);
			var doc = r[0];
			Console.WriteLine(r[0].Id);
		}

		[Test]
		[Category("Integration")]
		[Ignore]
		public void FacetField() {
			var conn = new SolrConnection("http://localhost:8983/solr", new HttpWebRequestFactory());
			var server = new SolrServer<TestDocument>(conn);
			var query = new SolrQueryByField("makedesc", "bmw");
			var r = server.Query(query, new QueryOptions {
				Rows = 5,
				FacetQueries = new ISolrFacetQuery[] {
					new SolrFacetFieldQuery("modeldesc"), 
				},
			});
			Console.WriteLine(r.Count);
			Console.WriteLine(r.FacetFields.Count);
			foreach (var fq in r.FacetFields["modeldesc"]) {
				Console.WriteLine("{0}: {1}", fq.Key, fq.Value);
			}
		}
	}
}