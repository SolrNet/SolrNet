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
        
        private static readonly Lazy<Configuration> config = new(() => ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None));
        private static readonly Lazy<string> serverURL = new (() => config.Value.AppSettings.Settings["solr"].Value);
        public static readonly System.Lazy<object> init = new System.Lazy<object>(() => {
            Startup.Init<Product>(new LoggingConnection(new SolrConnection(serverURL.Value)));
            return null;
        });
        public static readonly System.Lazy<object> initDict = new System.Lazy<object>(() => {
            Startup.Init<Dictionary<string, object>>(new LoggingConnection(new SolrConnection(serverURL.Value)));
            return null;
        });
        
        public static readonly Lazy<object> initLoose = new Lazy<object>(() => {
            Startup.Init<ProductLoose>(new LoggingConnection(new SolrConnection(serverURL.Value)));
            return null;
        });
        
        public IntegrationFixtureAsync(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            var x = init.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            solr.Commit();
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
                PriceMoney = new Money(123.44m, "GBP"),
                Popularity = 6,
                InStock = false,
            }
        };
        
        [Fact]
        public async Task AddAndQueryUnicode() {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            var id = Guid.NewGuid().ToString();
            const string name = "aşı ★✴カ";
            await solr.AddAsync(new Product
            {
                Id = id,
                Name = name,
            });
            await solr.CommitAsync();
            try
            {
                var results = await solr.QueryAsync(new SolrQueryByField("id", id));
                Assert.Single(results);
                Assert.Equal(id, results[0].Id);
                Assert.Equal(name, results[0].Name);
            }
            finally
            {
                await solr.DeleteAsync(id);
            }
        }

        [Fact]
        public async Task QueryByRangeMoneyAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            //await solr.AddRangeAsync(products);
            //await solr.CommitAsync();

            var results = await solr.QueryAsync(new SolrQueryByRange<Money>("price_c", new Money(123, "USD"), new Money(3000, "USD")));
            Assert.Equal(9, results.Count);
        }

        [Fact]
        public async Task DeleteByIdAndOrQueryAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();

            //await solr.AddRangeAsync(products);
            //await solr.CommitAsync();

            // await solr.DeleteAsync(new[] { "DEL12345", "DEL12346" }, new SolrQueryByField("features", "feature 3"));
            // await solr.CommitAsync();
            // var productsAfterDelete = await solr.QueryAsync(SolrQuery.All);
            //
            // Assert.Empty(productsAfterDelete);
        }


        [Fact]
        public async Task HighlightingAsync()
        {
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
            if (SolrVersion > new Version(6, 6))
            {
                testOutputHelper.WriteLine($"Date facet not available in Solr {SolrVersion}, skipping test");
                return;
            }
            var solr = ServiceLocator.Current.GetInstance<ISolrBasicOperations<Product>>();
            var results = await solr.QueryAsync(SolrQuery.All, new QueryOptions
            {
                Rows = 0,
                Facet = new FacetParameters
                {
                    Queries = new[] {
                        new SolrFacetDateQuery("manufacturedate_dt", DateTime.Now.AddHours(-1), DateTime.Now.AddHours(1), "+1DAY") {
                            HardEnd = true,
                            Other = new[] {FacetDateOther.After, FacetDateOther.Before}
                        },
                    }
                }
            });
            var dateFacetResult = results.FacetDates["manufacturedate_dt"];
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
                testOutputHelper.WriteLine(r.Id);
        }

        [Fact]
        public async Task MoreLikeThisAsync()
        {
            
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
            
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            var results = await solr.QueryAsync(new LocalParams { { "q.op", "AND" } } + "solr ipod");
            Assert.Empty(results);
        }

        [Fact]
        public async Task LocalParams2Async()
        {
            
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.QueryAsync(new LocalParams { { "tag", "pp" } } + new SolrQueryByField("cat", "bla"));
        }

        [Fact]
        public async Task LocalParams3Async()
        {
            
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
            await solr.QueryAsync(new LocalParams { { "tag", "pp" } } + new SolrQuery("cat:bla"));
        }

        [Fact]
        public async Task LooseMappingAsync()
        {
            
            var _ = initDict.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            var results = await solr.QueryAsync(new SolrQueryByField("id", "TWINX2048-3200PRO"));
            Assert.IsType<ArrayList>(results[0]["cat"]);
            Assert.IsType<string>(results[0]["id"]);
            Assert.IsType<bool>(results[0]["inStock"]);
            Assert.IsType<int>(results[0]["popularity"]);
            Assert.IsType<float>(results[0]["price"]);
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
            var _ = initDict.Value;
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
            
            var _ = initLoose.Value;
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<ProductLoose>>();
            var products = await solr.QueryAsync(new SolrQueryByField("id", "SP2514N"), new QueryOptions { Fields = new[] { "*", "score" } });
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
            var manufacturedate = Assert.IsType<DateTime>(product.OtherFields["manufacturedate_dt"]);
            Assert.Equal(new DateTime(2006, 02, 13), manufacturedate.Date);
            Assert.IsAssignableFrom<ICollection>(product.OtherFields["features"]);
            
            // product.OtherFields["timestamp"] = new DateTime(2010, 1, 1);
            // product.OtherFields["features"] = new[] { "a", "b", "c" };
            // product.OtherFields.Remove("_version_"); // avoid optimistic locking for now https://issues.apache.org/jira/browse/SOLR-3178
            // product.Score = null;
            // await solr.AddAsync(product);
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
        
        [Fact(Skip = "MoreLikeThisHandler not available for the sample Solr schema")]
        public async Task MoreLikeThisHandlerAsync()
        {
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
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

        private static readonly Version SolrVersion = Version.Parse(Environment.GetEnvironmentVariable("SOLR_VERSION") ?? "0.0");
    }
}
