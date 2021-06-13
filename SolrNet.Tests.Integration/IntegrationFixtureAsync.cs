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
using Xunit;
using CommonServiceLocator;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Tests.Integration.Sample;
using SolrNet.Tests;
using SolrNet.Tests.Utils;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace SolrNet.Tests.Integration
{
    [Trait("Category", "Integration")]
    [TestCaseOrderer(MethodDefTestCaseOrderer.Type, MethodDefTestCaseOrderer.Assembly)]
    public class IntegrationFixtureAsync
    {
        private readonly ITestOutputHelper testOutputHelper;
        
        public IntegrationFixtureAsync(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            var x = IntegrationFixture.init.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Delete(SolrQuery.All);
            solr.Commit();
        }

        [Fact]
        public async Task Add_then_queryAsync()
        {
            const string name = "Samsuñg SpinPoint P120 SP2514N - hárd drívè - 250 GB - ÁTÀ-133";
            var guid = new Guid("{78D734ED-12F8-44E0-8AA3-8CA3F353998D}");
            var p = new Product
            {
                Id = "SP2514N",
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
                Price = 92,
                PriceMoney = new Money(92m, "USD"),
                Popularity = 6,
                InStock = true,
            };

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.DeleteAsync(SolrQuery.All);
            await solr.AddWithBoostAsync(p, 2.2);
            await solr.CommitAsync();

            await solr.QueryAsync(new SolrQueryByField("name", @"3;Furniture"));
            var products = await solr.QueryAsync(new SolrQueryByRange<decimal>("price", 10m, 100m).Boost(2));
            Assert.Single(products);
            Assert.Equal(name, products[0].Name);
            Assert.Equal("SP2514N", products[0].Id);
            Assert.Equal(92m, products[0].Price);
            Assert.NotNull(products.Header);
            testOutputHelper.WriteLine("QTime is {0}", products.Header.QTime);
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
                Price = 92,
                PriceMoney = new Money(123.44m, "GBP"),
                Popularity = 6,
                InStock = false,
            }
        };

        [Fact]
        public async Task QueryByRangeMoneyAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.AddRangeAsync(products);
            await solr.CommitAsync();

            var results = await solr.QueryAsync(new SolrQueryByRange<Money>("price_c", new Money(123, null), new Money(3000, "USD")));
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public async Task DeleteByIdAndOrQueryAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();

            await solr.AddRangeAsync(products);
            await solr.CommitAsync();

            await solr.DeleteAsync(new[] { "DEL12345", "DEL12346" }, new SolrQueryByField("features", "feature 3"));
            await solr.CommitAsync();
            var productsAfterDelete = await solr.QueryAsync(SolrQuery.All);

            Assert.Empty(productsAfterDelete);
        }


        [Fact]
        public async Task HighlightingAsync()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(new SolrQueryByField("features", "fluid"), new QueryOptions
            {
                Highlight = new HighlightingParameters
                {
                    Fields = new[] { "features" },
                }
            });
            Assert.NotNull(results.Highlights);
            Assert.Equal(1, results.Highlights.Count);
            foreach (var h in results.Highlights[results[0].Id])
            {
                testOutputHelper.WriteLine("{0}: {1}", h.Key, string.Join(", ", h.Value.ToArray()));
            }
        }

        [Fact]
        public async Task HighlightingWrappedWithClassAsync()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(new SolrQueryByField("features", "fluid"), new QueryOptions
            {
                Highlight = new HighlightingParameters
                {
                    Fields = new[] { "features" },
                }
            });
            Assert.NotNull(results.Highlights);
            Assert.Equal(1, results.Highlights.Count);
            foreach (var h in results.Highlights[results[0].Id].Snippets)
            {
                testOutputHelper.WriteLine("{0}: {1}", h.Key, string.Join(", ", h.Value.ToArray()));
            }
        }

        [Fact]
        public async Task DateFacetAsync()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                Facet = new FacetParameters
                {
                    Queries = new[] {
                        new SolrFacetDateQuery("timestamp", DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1), "+1DAY") {
                            HardEnd = true,
                            Other = new[] {FacetDateOther.After, FacetDateOther.Before}
                        },
                    }
                }
            });
            var dateFacetResult = results.FacetDates["timestamp"];
            testOutputHelper.WriteLine(dateFacetResult.DateResults[0].Key.ToString());
            testOutputHelper.WriteLine(dateFacetResult.DateResults[0].Value.ToString());
        }

        [Fact]
        public async Task PingAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            await solr.PingAsync();
        }

        [Fact]
        public async Task DismaxAsync()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var products = await solr.QueryAsync(new SolrQuery("samsung"), new QueryOptions
            {
                ExtraParams = new Dictionary<string, string> {
                {"qt", "dismax"},
                {"qf", "sku name^1.2 manu^1.1"},
            }
            });
            Assert.True(products.Count > 0);
        }

        [Fact]
        public async Task FilterQueryAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var r = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                FilterQueries = new[] { new SolrQueryByRange<string>("price", "4", "*"), }
            });
            foreach (var product in r)
            {
                testOutputHelper.WriteLine(product.Id);
            }
        }

        [Fact]
        public async Task SpellCheckingAsync()
        {
            await Add_then_queryAsync();
            await AddSampleDocsAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var r = await solr.QueryAsync(new SolrQueryByField("name", "hell untrasharp"), new QueryOptions
            {
                SpellCheck = new SpellCheckingParameters(),
            });
            testOutputHelper.WriteLine("Products:");
            foreach (var product in r)
            {
                testOutputHelper.WriteLine(product.Id);
            }
            testOutputHelper.WriteLine("Spell checking:");
            Assert.True(r.SpellChecking.Count > 0);
            foreach (var sc in r.SpellChecking)
            {
                testOutputHelper.WriteLine(sc.Query);
                foreach (var s in sc.Suggestions)
                {
                    testOutputHelper.WriteLine(s);
                }
            }
        }

        [Fact]
        public async Task RandomSortingAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                OrderBy = new[] { new RandomSortOrder("random") }
            });
            foreach (var r in results)
                testOutputHelper.WriteLine(r.Manufacturer);
        }

        [Fact]
        public async Task MoreLikeThisAsync()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.AddAsync(new Product
            {
                Id = "apache-cocoon",
                Categories = new[] { "framework", "java" },
                Name = "Apache Cocoon",
                Manufacturer = "Apache",
            });
            await solr.AddAsync(new Product
            {
                Id = "apache-hadoop",
                Categories = new[] { "framework", "java", "mapreduce" },
                Name = "Apache Hadoop",
                Manufacturer = "Apache",
            });
            await solr.CommitAsync();
            var results = await solr.QueryAsync(new SolrQuery("apache"), new QueryOptions
            {
                MoreLikeThis = new MoreLikeThisParameters(new[] { "cat", "manu" })
                {
                    MinDocFreq = 1,
                    MinTermFreq = 1,
                    //Count = 1,
                },
            });
            Assert.True(results.SimilarResults.Count > 0);
            foreach (var r in results.SimilarResults)
            {
                testOutputHelper.WriteLine("Similar documents to {0}", r.Key);
                foreach (var similar in r.Value)
                    testOutputHelper.WriteLine(similar.Id);
            }
        }

        [Fact]
        public async Task StatsAsync()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                Stats = new StatsParameters
                {
                    Facets = new[] { "inStock" },
                    // stats facet currently broken in Solr: https://issues.apache.org/jira/browse/SOLR-2976
                    //FieldsWithFacets = new Dictionary<string, ICollection<string>> {
                    //    {"popularity", new List<string> {"weight"}}
                    //}
                }
            });
            Assert.NotNull(results.Stats);
            foreach (var kv in results.Stats)
            {
                testOutputHelper.WriteLine("Field {0}: ", kv.Key);
                DumpStats(kv.Value, 1);
            }
        }

        public void print(int tabs, string format, params object[] args)
        {
            Console.Write(new string('\t', tabs));
            Console.WriteLine(format, args);
        }

        public void DumpStats(StatsResult s, int tabs)
        {
            print(tabs, "Min: {0}", s.Min);
            print(tabs, "Max: {0}", s.Max);
            print(tabs, "Sum of squares: {0}", s.SumOfSquares);
            foreach (var f in s.FacetResults)
            {
                print(tabs, "Facet: {0}", f.Key);
                foreach (var fv in f.Value)
                {
                    print(tabs + 1, "Facet value: {0}", fv.Key);
                    DumpStats(fv.Value, tabs + 2);
                }
            }
        }

        [Fact]
        public async Task LocalParamsAsync()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            var results = await solr.QueryAsync(new LocalParams { { "q.op", "AND" } } + "solr ipod");
            Assert.Empty(results);
        }

        [Fact]
        public async Task LocalParams2Async()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.QueryAsync(new LocalParams { { "tag", "pp" } } + new SolrQueryByField("cat", "bla"));
        }

        [Fact]
        public async Task LocalParams3Async()
        {
            await Add_then_queryAsync();
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.QueryAsync(new LocalParams { { "tag", "pp" } } + new SolrQuery("cat:bla"));
        }

        [Fact]
        public async Task LooseMappingAsync()
        {
            await Add_then_queryAsync();
            var _ = IntegrationFixture.initDict.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            var results = await solr.QueryAsync(SolrQuery.All);
            Assert.IsType<ArrayList>(results[0]["cat"]);
            Assert.IsType<string>(results[0]["id"]);
            Assert.IsType<bool>(results[0]["inStock"]);
            Assert.IsType<int>(results[0]["popularity"]);
            Assert.IsType<float>(results[0]["price"]);
            Assert.IsType<DateTime>(results[0]["timestamp"]);
            Assert.IsType<string>(((IList)results[0]["cat"])[0]);
            foreach (var r in results)
                foreach (var kv in r)
                {
                    testOutputHelper.WriteLine("{0} ({1}): {2}", kv.Key, TypeOrNull(kv.Value), kv.Value);
                    if (kv.Value is IList)
                    {
                        foreach (var e in (IList)kv.Value)
                            testOutputHelper.WriteLine("\t{0} ({1})", e, TypeOrNull(e));
                    }
                }
        }

        [Fact(Skip = "Registering the connection in the container causes a side effect.")]
        public async Task LooseMappingAddAsync()
        {
            var _ = IntegrationFixture.initDict.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            await solr.AddAsync(new Dictionary<string, object> {
                {"id", "id1234"},
                {"manu", "pepe"},
                {"popularity", 6},
            });
        }

        public Type TypeOrNull(object o)
        {
            if (o == null)
                return null;
            return o.GetType();
        }

        [Fact]
        public async Task FieldCollapsingAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Collapse = new CollapseParameters("manu_exact")
                {
                    Type = CollapseType.Adjacent,
                    MaxDocs = 1,
                }
            });
            testOutputHelper.WriteLine("CollapsedDocuments.Count {0}", results.Collapsing.CollapsedDocuments.Count);
        }


        [Fact]
        public async Task FieldGroupingAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Grouping = new GroupingParameters()
                {
                    Fields = new[] { "manu_exact" },
                    Format = GroupingFormat.Grouped,
                    Limit = 1,
                }
            });

            testOutputHelper.WriteLine("Group.Count {0}", results.Grouping.Count);
            Assert.Equal(1, results.Grouping.Count);
            Assert.True(results.Grouping.ContainsKey("manu_exact"));
            
            // TODO the following assertions fails, maybe the data isn't set up correctly?
            // Assert.True(results.Grouping["manu_exact"].Groups.Count >= 1, 
            // $"Got {results.Grouping["manu_exact"].Groups.Count} groups: " +
            // string.Join(", ", results.Grouping["manu_exact"].Groups.Select(g => g.GroupValue)));
        }

        [Fact(Skip = "Crashes Solr with 'numHits must be &gt; 0; please use TotalHitCountCollector if you just need the total hit count' (?)")]
        public async Task QueryGroupingAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Grouping = new GroupingParameters()
                {
                    Query = new[] { new SolrQuery("manu_exact"), new SolrQuery("name") },
                    Format = GroupingFormat.Grouped,
                    Limit = 1,
                }
            });

            testOutputHelper.WriteLine("Group.Count {0}", results.Grouping.Count);
            Assert.Equal(2, results.Grouping.Count);
            Assert.True(results.Grouping.ContainsKey("manu_exact"));
            Assert.True(results.Grouping.ContainsKey("name"));
            Assert.True(results.Grouping["manu_exact"].Groups.Count >= 1);
            Assert.True(results.Grouping["name"].Groups.Count >= 1);
        }

        [Fact]
        public async Task SemiLooseMappingAsync()
        {
            await Add_then_queryAsync();
            var _ = IntegrationFixture.initLoose.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<ProductLoose>>();
            var products = await solr.QueryAsync(SolrQuery.All, new QueryOptions { Fields = new[] { "*", "score" } });
            Assert.Single(products);
            var product = products[0];
            Assert.Equal("SP2514N", product.Id);
            Assert.True(product.Score.HasValue);
            Assert.False(product.OtherFields.ContainsKey("score"));
            Assert.NotNull(product.Name);
            Assert.NotNull(product.OtherFields);
            testOutputHelper.WriteLine(product.OtherFields.Count.ToString());
            foreach (var field in product.OtherFields)
                testOutputHelper.WriteLine("{0}: {1} ({2})", field.Key, field.Value, TypeOrNull(field.Value));
            Assert.IsType<DateTime>(product.OtherFields["timestamp"]);
            Assert.Equal(new DateTime(1, 1, 1), product.OtherFields["timestamp"]);
            Assert.IsAssignableFrom<ICollection>(product.OtherFields["features"]);
            product.OtherFields["timestamp"] = new DateTime(2010, 1, 1);
            product.OtherFields["features"] = new[] { "a", "b", "c" };
            product.OtherFields.Remove("_version_"); // avoid optimistic locking for now https://issues.apache.org/jira/browse/SOLR-3178
            product.Score = null;
            await solr.AddAsync(product);
        }

        [Fact(Skip = "Getting a solr error")]
        public async Task ExtractRequestHandlerAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            using (var file = File.OpenRead(@"..\..\..\SolrNet.Tests\test.pdf"))
            {
                var response = await solr.ExtractAsync(new ExtractParameters(file, "abcd")
                {
                    ExtractOnly = true,
                    ExtractFormat = ExtractFormat.Text,
                });
                testOutputHelper.WriteLine(response.Content);
                Assert.Equal("Your PDF viewing software works!\n\n\n", response.Content);
            }
        }

        public async Task AddSampleDocsAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            var connection = ServiceLocator.Current.GetInstance<ISolrConnection>();
            var files = Directory.GetFiles("exampledocs", "*.xml");
            foreach (var file in files)
            {
                connection.Post("/update", File.ReadAllText(file, Encoding.UTF8));
            }
            await solr.CommitAsync();
        }

        [Fact]
        public async Task MoreLikeThisHandlerAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.DeleteAsync(SolrQuery.All);
            await solr.CommitAsync();
            await AddSampleDocsAsync();
            var mltParams = new MoreLikeThisHandlerParameters(new[] { "cat", "name" })
            {
                MatchInclude = true,
                MinTermFreq = 1,
                MinDocFreq = 1,
                ShowTerms = InterestingTerms.List,
            };
            var q = SolrMLTQuery.FromQuery(new SolrQuery("id:UTF8TEST"));
            var results = await solr.MoreLikeThisAsync(q, new MoreLikeThisHandlerQueryOptions(mltParams));
            Assert.Equal(2, results.Count);
            Assert.NotNull(results.Match);
            Assert.Equal("UTF8TEST", results.Match.Id);
            Assert.True(results.InterestingTerms.Count > 0);
            foreach (var t in results.InterestingTerms)
            {
                testOutputHelper.WriteLine("Interesting term: {0} ({1})", t.Key, t.Value);
            }

        }
    }
}
