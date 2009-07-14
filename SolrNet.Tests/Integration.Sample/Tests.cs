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
using System.Collections.Generic;
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Commands.Parameters;

namespace SolrNet.Tests.Integration.Sample {
    /// <summary>
    /// These tests run against the sample schema that comes with solr
    /// </summary>
    [TestFixture]
    public class Tests {
        private const string serverURL = "http://localhost:8983/solr";

        [FixtureSetUp]
        public void FixtureSetup() {
            Startup.Init<Product>(serverURL);
        }

        [Test]
        [Ignore("This test requires an actual solr instance running")]
        public void Add_then_query() {
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
                    "áéíóúñç & two", // testing UTF
                    @"ÚóÁ⌠╒""ĥÛē…<>ܐóジャストシステムは、日本で初めてユニコードベースのワードプロセ ッサーを開発しました。このことにより、10年以上も前から、日本のコンピューターユーザーはユニコード、特に日中韓の統合漢 字の恩恵を享受してきました。ジャストシステムは現在、”xfy”というJava環境で稼働する 先進的なXML関連製品の世界市場への展開を積極的に推進していますが、ユニコードを基盤としているために、”xfy”は初めから国際化されているのです。ジャストシステムは、ユニコードの普遍的な思想とアーキテクチャに 感謝するとともに、その第5版の刊行を心から歓迎します",
                    @"control" + (char)0x07 + (char)0x01 + (char)0x0E +(char)0x1F + (char)0xFFFE, // testing control chars
                },
                Prices = new Dictionary<string, decimal> {
                    {"regular", 150m},
                    {"afterrebate", 100m},
                },
                Price = 92,
                Popularity = 6,
                InStock = true,
            };

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All)
                .AddWithBoost(p, 2.2)
                .Commit();
            solr.Query(new SolrQueryByField("name", @"3;Furniture"));
            var products = solr.Query(new SolrQueryByRange<decimal>("price", 10m, 100m).Boost(2));
            Assert.AreEqual(1, products.Count);
            Assert.AreEqual("SP2514N", products[0].Id);
            Assert.AreEqual(92m, products[0].Price);
            Assert.IsNotNull(products[0].Prices);
            Assert.AreEqual(2, products[0].Prices.Count);
            Assert.AreEqual(150m, products[0].Prices["regular"]);
            Assert.AreEqual(100m, products[0].Prices["afterrebate"]);
        }

        [Test]
        [Ignore("This test requires an actual solr instance running")]
        public void Highlighting() {
            Add_then_query();
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
        [Ignore("This test requires an actual solr instance running")]
        public void Ping()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            solr.Ping();
        }

        [Test]
        [Ignore("This test requires an actual solr instance running")]
        public void FilterQuery()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var r = solr.Query(SolrQuery.All, new QueryOptions {
                FilterQueries = new[] {new SolrQueryByRange<string>("price", "4", "*"),}
            });
            foreach (var product in r) {
                Console.WriteLine(product.Id);
            }
        }

        [Test]
        [Ignore("This test requires an actual solr instance running")]
        public void SpellChecking()
        {
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
        [Ignore("This test requires an actual solr instance running")]
        public void RandomSorting()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(SolrQuery.All, new QueryOptions {
                OrderBy = new[] {new RandomSortOrder("random")}
            });
            foreach (var r in results)
                Console.WriteLine(r.Manufacturer);
        }

        [Test]
        [Ignore("This test requires an actual solr instance running")]
        public void MoreLikeThis() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(new SolrQuery("apache"), new QueryOptions {
                MoreLikeThis = new MoreLikeThisParameters(new[] {"cat", "manu"}) {
                    MinDocFreq = 1,
                    MinTermFreq = 1,
                    //Count = 1,
                },
            });
            foreach (var r in results.SimilarResults) {
                Console.WriteLine("Similar documents to {0}", r.Key.Id);
                foreach (var similar in r.Value)
                    Console.WriteLine(similar.Id);
                Console.WriteLine();
            }
        }

        [Test]
        [Ignore("This test requires an actual solr instance running")]
        public void Stats() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(SolrQuery.All, new QueryOptions {
                Rows = 0,
                Stats = new StatsParameters {
                    Facets = new[] { "inStock" },
                    FieldsWithFacets = new Dictionary<string, ICollection<string>> {
                        {"popularity", new List<string> {"price"}}
                    }
                }
            });
            Assert.IsNotNull(results.Stats);
            foreach (var kv in results.Stats) {
                Console.WriteLine("Field {0}: ", kv.Key);
                DumpStats(kv.Value, 1);
            }
        }

        public void print(int tabs, string format, params object[] args) {
            Console.Write(new string('\t', tabs));
            Console.WriteLine(format, args);
        }

        public void DumpStats(StatsResult s, int tabs) {
            print(tabs, "Min: {0}", s.Min);
            print(tabs, "Max: {0}", s.Max);
            print(tabs, "Sum of squares: {0}", s.SumOfSquares);
            foreach (var f in s.FacetResults) {
                print(tabs, "Facet: {0}", f.Key);
                foreach (var fv in f.Value) {
                    print(tabs+1, "Facet value: {0}", fv.Key);
                    DumpStats(fv.Value, tabs+2);
                }
            }
        }
    }
}