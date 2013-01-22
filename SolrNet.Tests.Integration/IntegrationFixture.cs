#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Tests.Integration.Sample;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests.Integration {
    [TestFixture]
    [Category("Integration")]
    public class IntegrationFixture {
        private static readonly string serverURL = ConfigurationManager.AppSettings["solr"];
        private static readonly Lazy<object> init = new Lazy<object>(() => {
            Startup.Init<Product>(new LoggingConnection(new SolrConnection(serverURL)));
            return null;
        });
        private static readonly Lazy<object> initDict = new Lazy<object>(() => {
            Startup.Init<Dictionary<string, object>>(new LoggingConnection(new SolrConnection(serverURL)));
            return null;
        });
            
        [SetUp]
        public void Setup() {
            var x = init.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All);
            solr.Commit();
        }

        [Test]
        public void Add_then_query() {
            const string name = "Samsuñg SpinPoint P120 SP2514N - hárd drívè - 250 GB - ÁTÀ-133";
            var guid = new Guid("{78D734ED-12F8-44E0-8AA3-8CA3F353998D}");
            var p = new Product {
                Id = "SP2514N",
                Guid = guid,
                Name = name,
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
                PriceMoney = new Money(92m, "USD"),
                Popularity = 6,
                InStock = true,
                DynCategories = new Dictionary<string, ICollection<string>> {
                    {"t", new[] {"something"}},
                }
            };

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All);
            solr.AddWithBoost(p, 2.2);
            solr.Commit();

            solr.Query(new SolrQueryByField("name", @"3;Furniture"));
            var products = solr.Query(new SolrQueryByRange<decimal>("price", 10m, 100m).Boost(2));
            Assert.AreEqual(1, products.Count);
            Assert.AreEqual(name, products[0].Name);
            Assert.AreEqual("SP2514N", products[0].Id);
            Assert.AreEqual(guid, products[0].Guid);
            Assert.AreEqual(92m, products[0].Price);
            Assert.IsNotNull(products[0].Prices);
            Assert.AreEqual(3, products[0].Prices.Count);
            Assert.AreEqual(150m, products[0].Prices["regular"]);
            Assert.AreEqual(100m, products[0].Prices["afterrebate"]);
            Assert.IsNotNull(products.Header);
            Console.WriteLine("QTime is {0}", products.Header.QTime);
        }

        private static readonly IEnumerable<Product> products = new[] {
            new Product {
                Id = "DEL12345",
                Name = "Delete test product 1",
                Manufacturer = "Acme ltd",
                Categories = new[] {
                    "electronics",
                    "test products",
                },
                Features = new[] {
                    "feature 1",
                    "feature 2",
                },
                Prices = new Dictionary<string, decimal> {
                    {"regular", 150m},
                    {"afterrebate", 100m},
                },
                Price = 92,
                PriceMoney = new Money(123.44m, "EUR"),
                Popularity = 6,
                InStock = false,
            },
            new Product {
                Id = "DEL12346",
                Name = "Delete test product 2",
                Manufacturer = "Acme ltd",
                Categories = new[] {
                    "electronics",
                    "test products",
                },
                Features = new[] {
                    "feature 1",
                    "feature 3",
                },
                Prices = new Dictionary<string, decimal> {
                    {"regular", 150m},
                    {"afterrebate", 100m},
                },
                Price = 92,
                PriceMoney = new Money(123.44m, "ARS"),
                Popularity = 6,
                InStock = false,
            },
            new Product {
                Id = "DEL12347",
                Name = "Delete test product 3",
                Manufacturer = "Acme ltd",
                Categories = new[] {
                    "electronics",
                    "test products",
                },
                Features = new[] {
                    "feature 1",
                    "feature 3",
                },
                Prices = new Dictionary<string, decimal> {
                    {"regular", 150m},
                    {"afterrebate", 100m},
                },
                Price = 92,
                PriceMoney = new Money(123.44m, "GBP"),
                Popularity = 6,
                InStock = false,
            }
        };

        [Test]
        public void QueryByRangeMoney() {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.AddRange(products);
            solr.Commit();

            var results = solr.Query(new SolrQueryByRange<Money>("price_c", new Money(123, null), new Money(3000, "USD")));
            Assert.AreEqual(2, results.Count);
        }

        [Test]
        public void DeleteByIdAndOrQuery() {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();

            solr.AddRange(products);
            solr.Commit();
            
            solr.Delete(new[] { "DEL12345", "DEL12346" }, new SolrQueryByField("features", "feature 3"));
            solr.Commit();
            var productsAfterDelete = solr.Query(SolrQuery.All);

            Assert.AreEqual(0, productsAfterDelete.Count);
        }
        
         
        [Test]
        public void Highlighting() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(new SolrQueryByField("features", "fluid"), new QueryOptions {
                Highlight = new HighlightingParameters {
                    Fields = new[] {"features"},
                }
            });
            Assert.IsNotNull(results.Highlights);
            Assert.AreEqual(1, results.Highlights.Count);
            foreach (var h in results.Highlights[results[0].Id])
            {
                Console.WriteLine("{0}: {1}", h.Key, string.Join(", ", h.Value.ToArray()));
            }
        }

        [Test]
        public void HighlightingWrappedWithClass()
        {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(new SolrQueryByField("features", "fluid"), new QueryOptions
            {
                Highlight = new HighlightingParameters
                {
                    Fields = new[] { "features" },
                }
            });
            Assert.IsNotNull(results.Highlights);
            Assert.AreEqual(1, results.Highlights.Count);
            foreach (var h in results.Highlights[results[0].Id].Snippets)
            {
                Console.WriteLine("{0}: {1}", h.Key, string.Join(", ", h.Value.ToArray()));
            }
        }

        [Test]
        public void DateFacet() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(SolrQuery.All, new QueryOptions {
                Rows = 0,
                Facet = new FacetParameters {
                    Queries = new[] {
                        new SolrFacetDateQuery("timestamp", DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1), "+1DAY") {
                            HardEnd = true,
                            Other = new[] {FacetDateOther.After, FacetDateOther.Before}
                        },
                    }
                }
            });
            var dateFacetResult = results.FacetDates["timestamp"];
            Console.WriteLine(dateFacetResult.DateResults[0].Key);
            Console.WriteLine(dateFacetResult.DateResults[0].Value);
        }

        [Test]
        public void Ping() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            solr.Ping();
        }

        [Test]
        public void Dismax() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var products = solr.Query(new SolrQuery("samsung"), new QueryOptions { ExtraParams = new Dictionary<string, string> {
                {"qt", "dismax"},
                {"qf", "sku name^1.2 manu^1.1"},
            }});
            Assert.GreaterThan(products.Count, 0);
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
            Add_then_query();
            AddSampleDocs();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var r = solr.Query(new SolrQueryByField("name", "hell untrasharp"), new QueryOptions {
                SpellCheck = new SpellCheckingParameters(),
            });
            Console.WriteLine("Products:");
            foreach (var product in r) {
                Console.WriteLine(product.Id);
            }
            Console.WriteLine();
            Console.WriteLine("Spell checking:");
            Assert.GreaterThan(r.SpellChecking.Count, 0);
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

        [Test]
        public void MoreLikeThis() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Add(new Product {
                Id = "apache-cocoon",
                Categories = new[] {"framework", "java"},
                Name = "Apache Cocoon",
                Manufacturer = "Apache",
            });
            solr.Add(new Product {
                Id = "apache-hadoop",
                Categories = new[] { "framework", "java", "mapreduce" },
                Name = "Apache Hadoop",
                Manufacturer = "Apache",
            });
            solr.Commit();
            var results = solr.Query(new SolrQuery("apache"), new QueryOptions {
                MoreLikeThis = new MoreLikeThisParameters(new[] {"cat", "manu"}) {
                    MinDocFreq = 1,
                    MinTermFreq = 1,
                    //Count = 1,
                },
            });
            Assert.GreaterThan(results.SimilarResults.Count, 0);
            foreach (var r in results.SimilarResults) {
                Console.WriteLine("Similar documents to {0}", r.Key);
                foreach (var similar in r.Value)
                    Console.WriteLine(similar.Id);
                Console.WriteLine();
            }
        }

        [Test]
        public void Stats() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(SolrQuery.All, new QueryOptions {
                Rows = 0,
                Stats = new StatsParameters {
                    Facets = new[] { "inStock" },
                    // stats facet currently broken in Solr: https://issues.apache.org/jira/browse/SOLR-2976
                    //FieldsWithFacets = new Dictionary<string, ICollection<string>> {
                    //    {"popularity", new List<string> {"weight"}}
                    //}
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

        [Test]
        public void LocalParams() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            var results = solr.Query(new LocalParams {{"q.op", "AND"}} + "solr ipod");
            Assert.AreEqual(0, results.Count);
        }

        [Test]
        public void LocalParams2() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Query(new LocalParams { { "tag", "pp" } } + new SolrQueryByField("cat", "bla"));
        }

        [Test]
        public void LocalParams3() {
            Add_then_query();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Query(new LocalParams { { "tag", "pp" } } + new SolrQuery("cat:bla"));
        }

        [Test]
        public void LooseMapping() {
            Add_then_query();
            var _ = initDict.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            var results = solr.Query(SolrQuery.All);
            Assert.IsInstanceOfType<ArrayList>(results[0]["cat"]);
            Assert.IsInstanceOfType<string>(results[0]["id"]);
            Assert.IsInstanceOfType<bool>(results[0]["inStock"]);
            Assert.IsInstanceOfType<int>(results[0]["popularity"]);
            Assert.IsInstanceOfType<float>(results[0]["price"]);
            Assert.IsInstanceOfType<DateTime>(results[0]["timestamp"]);
            Assert.IsInstanceOfType<string>(((IList) results[0]["cat"])[0]);
            foreach (var r in results)
                foreach (var kv in r) {
                    Console.WriteLine("{0} ({1}): {2}", kv.Key, TypeOrNull(kv.Value), kv.Value);
                    if (kv.Value is IList) {
                        foreach (var e in (IList)kv.Value)
                            Console.WriteLine("\t{0} ({1})", e, TypeOrNull(e));
                    }
                }
        }
        
        [Test]
        [Ignore("Registering the connection in the container causes a side effect.")]
        public void LooseMappingAdd() {
            var _ = initDict.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            solr.Add(new Dictionary<string, object> {
                {"id", "id1234"},
                {"manu", "pepe"},
                {"popularity", 6},
            });
        }
        
        public Type TypeOrNull(object o) {
            if (o == null)
                return null;
            return o.GetType();
        }

        [Test]
        public void FieldCollapsing() {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = solr.Query(SolrQuery.All, new QueryOptions {
                Collapse = new CollapseParameters("manu_exact") { 
                    Type = CollapseType.Adjacent,
                    MaxDocs = 1,
                }
            });
            Console.WriteLine("CollapsedDocuments.Count {0}", results.Collapsing.CollapsedDocuments.Count);
        }


		[Test]
		public void FieldGrouping()
		{
			var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
			var results = solr.Query(SolrQuery.All, new QueryOptions
			{
				Grouping = new GroupingParameters()
				{
					Fields = new [] { "manu_exact" },
					Format = GroupingFormat.Grouped,
					Limit = 1,
				}
			});
			
			Console.WriteLine("Group.Count {0}", results.Grouping.Count);
			Assert.AreEqual(1, results.Grouping.Count);
			Assert.AreEqual(true, results.Grouping.ContainsKey("manu_exact"));
			Assert.GreaterThanOrEqualTo(results.Grouping["manu_exact"].Groups.Count,1);
		}

        private static readonly Lazy<object> initLoose = new Lazy<object>(() => {
            Startup.Init<ProductLoose>(new LoggingConnection(new SolrConnection(serverURL)));
            return null;
        });
            
            
        [Test]
        public void SemiLooseMapping() {
            Add_then_query();
            var _ = initLoose.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<ProductLoose>>();
            var products = solr.Query(SolrQuery.All, new QueryOptions {Fields = new[] {"*", "score"}});
            Assert.AreEqual(1, products.Count);
            var product = products[0];
            Assert.AreEqual("SP2514N", product.Id);
            Assert.IsTrue(product.Score.HasValue);
            Assert.IsFalse(product.OtherFields.ContainsKey("score"));
            Assert.IsNull(product.SKU);
            Assert.IsNotNull(product.Name);
            Assert.IsNotNull(product.OtherFields);
            Console.WriteLine(product.OtherFields.Count);
            foreach (var field in product.OtherFields)
                Console.WriteLine("{0}: {1} ({2})", field.Key, field.Value, TypeOrNull(field.Value));
            Assert.IsInstanceOfType(typeof(DateTime), product.OtherFields["timestamp"]);
            Assert.AreEqual(new DateTime(1,1,1), product.OtherFields["timestamp"]);
            Assert.IsInstanceOfType(typeof(ICollection), product.OtherFields["features"]);
            product.OtherFields["timestamp"] = new DateTime(2010, 1, 1);
            product.OtherFields["features"] = new[] {"a", "b", "c"};
            product.OtherFields.Remove("_version_"); // avoid optimistic locking for now https://issues.apache.org/jira/browse/SOLR-3178
            product.Score = null;
            solr.Add(product);
        }

        [Test]
        public void ExtractRequestHandler() {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            using (var file = File.OpenRead(@"..\..\test.pdf")) {
                var response = solr.Extract(new ExtractParameters(file, "abcd") {
                    ExtractOnly = true,
                    ExtractFormat = ExtractFormat.Text,
                });
                Console.WriteLine(response.Content);
                Assert.AreEqual("Your PDF viewing software works!\n\n\n", response.Content);
            }
        }

        public void AddSampleDocs() {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            var connection = ServiceLocator.Current.GetInstance<ISolrConnection>();
            var files = Directory.GetFiles(@"..\..\..\SampleSolrApp\exampledocs", "*.xml");
            foreach (var file in files) {
                connection.Post("/update", File.ReadAllText(file, Encoding.UTF8));
            }
            solr.Commit();            
        }

        [Test]
        public void MoreLikeThisHandler() {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All);
            solr.Commit();
            AddSampleDocs();
            var mltParams = new MoreLikeThisHandlerParameters(new[] {"cat", "name"}) {
                MatchInclude = true, 
                MinTermFreq = 1, 
                MinDocFreq = 1, 
                ShowTerms = InterestingTerms.List,
            };
            var q = SolrMLTQuery.FromQuery(new SolrQuery("id:UTF8TEST"));
            var results = solr.MoreLikeThis(q, new MoreLikeThisHandlerQueryOptions(mltParams));
            Assert.AreEqual(2, results.Count);
            Assert.IsNotNull(results.Match);
            Assert.AreEqual("UTF8TEST", results.Match.Id);
            Assert.GreaterThan(results.InterestingTerms.Count, 0);
            foreach (var t in results.InterestingTerms) {
                Console.WriteLine("Interesting term: {0} ({1})", t.Key, t.Value);
            }
            
        }
    }
}