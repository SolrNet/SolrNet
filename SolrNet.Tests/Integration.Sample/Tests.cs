using System;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SolrNet.Commands.Parameters;

namespace SolrNet.Tests.Integration.Sample {
    /// <summary>
    /// These tests run against the sample schema that comes with solr
    /// </summary>
    [TestFixture]
    [Ignore("These tests require an actual solr instance running")]
    public class Tests {
        private const string serverURL = "http://localhost:8983/solr";

        [TestFixtureSetUp]
        public void FixtureSetup() {
            Startup.Init<Product>(serverURL);
        }

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

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All)
                .Add(p)
                .Commit();
            var products = solr.Query(new SolrQueryByRange<decimal>("price", 10m, 100m));
            Assert.AreEqual(1, products.Count);
            Assert.AreEqual("SP2514N", products[0].Id);
        }

        [Test]
        public void Highlighting() {
            Add();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(new SolrQueryByField("features", "noise"), new QueryOptions {
                Highlight = new HighlightingParameters {
                    Fields = new[] {"features"},
                }
            });
            Assert.IsNotNull(results.Highlights);
            Assert.AreEqual(1, results.Highlights.Count);
        }

        [Test]
        public void Ping() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            solr.Ping();
        }

        [Test]
        public void FilterQuery() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var r = solr.Query(SolrQuery.All, new QueryOptions {
                FilterQueries = new[] { new SolrQueryByRange<string>("price", "4", "*"), }
            });
            foreach (var product in r) {
                Console.WriteLine(product.Id);
            }
        }
    }
}