#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

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
                Name = "Samsuñg SpinPoint P120 SP2514N - hárd drívè - 250 GB - ÁTÀ-133",
                // testing UTF
                Manufacturer = "Samsung Electronics Co. Ltd.",
                Categories = new[] {
                    "electronics",
                    "hard drive",
                },
                Features = new[] {
                    "7200RPM, 8MB cache, IDE Ultra ATA-133",
                    "NoiseGuard, SilentSeek technology, Fluid Dynamic Bearing (FDB) motor",
                    "áéíóúñç", // testing UTF
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
            foreach (var h in results.Highlights[results[0]]) {
                Console.WriteLine("{0}: {1}", h.Key, h.Value);
            }
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
                FilterQueries = new[] {new SolrQueryByRange<string>("price", "4", "*"),}
            });
            foreach (var product in r) {
                Console.WriteLine(product.Id);
            }
        }

        [Test]
        public void SpellChecking() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var r = solr.Query(new SolrQuery("hell untrasharp"), new QueryOptions {
                SpellCheck = new SpellCheckingParameters(),
            });
            Console.WriteLine("Products:");
            foreach (var product in r) {
                Console.WriteLine(product.Id);
            }
            Console.WriteLine();
            Console.WriteLine("Spell checking:");
            foreach (var sc in r.SpellChecking) {
                Console.WriteLine(sc.Query);
                foreach (var s in sc.Suggestions) {
                    Console.WriteLine(s);                    
                }
            }
        }

        [Test]
        public void RandomSorting() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(SolrQuery.All, new QueryOptions {
                OrderBy = new[] {new RandomSortOrder("random")}
            });
            foreach (var r in results)
                Console.WriteLine(r.Manufacturer);
        }
    }
}