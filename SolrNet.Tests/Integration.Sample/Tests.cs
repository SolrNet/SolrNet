using System;
using NUnit.Framework;

namespace SolrNet.Tests.Integration.Sample {
	/// <summary>
	/// These tests run with the sample schema that comes with solr
	/// </summary>
	[TestFixture]
	public class Tests {
		private const string serverURL = "http://localhost:8983/solr";

		[Test]
		public void Add() {
			var p = new Product {
				Id = "SP2514N",
				Name = "Samsung SpinPoint P120 SP2514N - hard drive - 250 GB - ATA-133",
				Manufacturer = "Samsung Electronics Co. Ltd.",
        Categories = new[] {
        	"electronics",
          "hard drive",
				},
				Features = new[] {
					"7200RPM, 8MB cache, IDE Ultra ATA-133",
          "NoiseGuard, SilentSeek technology, Fluid Dynamic Bearing (FDB) motor",
				},
				Price = 92,
				Popularity = 6,
				InStock = true,
			};

			var solr = new SolrServer<Product>(serverURL);
			solr.Delete(new SolrQuery("*:*"));
			solr.Add(p);
			solr.Commit();
			var products = solr.Query(new SolrQueryByRange<decimal>("price", 10m, 100m));
			Assert.AreEqual(1, products.Count);
			Assert.AreEqual("SP2514N", products[0].Id);
		}

		[Test]
		public void Ping() {
			var solr = new SolrServer<Product>(serverURL);
			var response = solr.Ping();
			Console.WriteLine(response);
		}
	}
}