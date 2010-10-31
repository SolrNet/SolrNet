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

using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrQueryExecuterTests {
        public class TestDocument  {
            [SolrUniqueKey]
            public int Id { get; set; }

            public string OtherField { get; set; }
        }

        [Test]
        public void Execute() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            var conn = new MockConnection(q);
            var mocks = new MockRepository();
            var parser = mocks.StrictMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            var serializer = mocks.StrictMock<ISolrQuerySerializer>();
            With.Mocks(mocks).Expecting(() => {
                Expect.On(parser)
                    .Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(mockR);
                Expect.On(serializer)
                    .Call(serializer.Serialize(null))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(queryString);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, null);
                var r = queryExecuter.Execute(new SolrQuery(queryString), null);
            });
        }

        [Test]
        public void Sort() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["sort"] = "id asc";
            var conn = new MockConnection(q);
            var mocks = new MockRepository();
            var parser = mocks.StrictMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            var querySerializer = new SolrQuerySerializerStub(queryString);
            With.Mocks(mocks).Expecting(() => {
                Expect.Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(mockR);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null);                
                var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                    OrderBy = new[] {new SortOrder("id")}
                });
            });
        }

        [Test]
        public void SortMultipleWithOrders() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["sort"] = "id asc,name desc";
            var conn = new MockConnection(q);
            var mocks = new MockRepository();
            var parser = mocks.StrictMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            var querySerializer = new SolrQuerySerializerStub(queryString);
            With.Mocks(mocks).Expecting(() => {
                Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null);
                var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                    OrderBy = new[] {
                        new SortOrder("id", Order.ASC),
                        new SortOrder("name", Order.DESC)
                    }
                });
            });
        }

        [Test]
        public void ResultFields() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["fl"] = "id,name";
            var conn = new MockConnection(q);
            var mocks = new MockRepository();
            var parser = mocks.StrictMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            var querySerializer = new SolrQuerySerializerStub(queryString);
            With.Mocks(mocks).Expecting(delegate {
                Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null);
                var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                    Fields = new[] {"id", "name"},
                });
            });
        }

        [Test]
        public void Facets() {
            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["facet"] = "true";
            q["facet.field"] = "Id";
            q["facet.query"] = "id:[1 TO 5]";
            var conn = new MockConnection(q);
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, facetQuerySerializer);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetQuery(new SolrQuery("id:[1 TO 5]")),
                    }
                }
            });
        }

        public KeyValuePair<T1, T2> KVP<T1, T2>(T1 a, T2 b) {
            return new KeyValuePair<T1, T2>(a, b);
        }

        [Test]
        public void MultipleFacetFields() {
            var conn = new MockConnection(new[] {
                KVP("q", ""),
                KVP("rows", SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString()),
                KVP("facet", "true"),
                KVP("facet.field", "Id"),
                KVP("facet.field", "OtherField"),
            });
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var facetQuerySerializer = new DefaultFacetQuerySerializer(serializer, new DefaultFieldSerializer());
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, facetQuerySerializer);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetFieldQuery("OtherField"),
                    }
                }
            });
        }

        [Test]
        public void Highlighting() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            const string highlightedField = "field1";
            const string afterTerm = "after";
            const string beforeTerm = "before";
            const int snippets = 3;
            const string alt = "alt";
            const int fragsize = 7;
            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["hl"] = "true";
            q["hl.fl"] = highlightedField;
            q["hl.snippets"] = snippets.ToString();
            q["hl.fragsize"] = fragsize.ToString();
            q["hl.requireFieldMatch"] = "true";
            q["hl.alternateField"] = alt;
            q["hl.simple.pre"] = beforeTerm;
            q["hl.simple.post"] = afterTerm;
            q["hl.regex.slop"] = "4.12";
            q["hl.regex.pattern"] = "\\.";
            q["hl.regex.maxAnalyzedChars"] = "8000";
            q["hl.usePhraseHighlighter"] = "true";
            q["hl.useFastVectorHighlighter"] = "true";
            q["hl.highlightMultiTerm"] = "true";
            q["hl.mergeContiguous"] = "true";
            q["hl.maxAnalyzedChars"] = "12";
            q["hl.maxAlternateFieldLength"] = "22";
            q["hl.fragmenter"] = "regex";

            var conn = new MockConnection(q);
            var querySerializer = new SolrQuerySerializerStub("");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Highlight = new HighlightingParameters {
                    Fields = new[] { highlightedField },
                    AfterTerm = afterTerm,
                    BeforeTerm = beforeTerm,
                    Snippets = snippets,
                    AlternateField = alt,
                    Fragsize = fragsize,
                    RequireFieldMatch = true,
                    RegexSlop = 4.12,
                    RegexPattern = "\\.",
                    RegexMaxAnalyzedChars = 8000,
                    UsePhraseHighlighter = true,
                    UseFastVectorHighlighter = true,
                    MergeContiguous = true,
                    MaxAnalyzedChars = 12,
                    HighlightMultiTerm = true,
                    MaxAlternateFieldLength = 22,
                    Fragmenter = SolrHighlightFragmenter.Regex
                }
            });
        }

        [Test]
        public void FilterQuery() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var conn = new MockConnection(new[] {
                KVP("q", "*:*"),
                KVP("rows", "10"),
                KVP("fq", "id:0"),
                KVP("fq", "id:2"),
            });
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null) {
                DefaultRows = 10,
            };
            queryExecuter.Execute(SolrQuery.All, new QueryOptions {
                FilterQueries = new[] {
                    new SolrQuery("id:0"),
                    new SolrQuery("id:2"),
                },
            });
        }

        [Test]
        public void SpellChecking() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, null, null);
            var p = queryExecuter.GetSpellCheckingParameters(new QueryOptions {
                SpellCheck = new SpellCheckingParameters {
                    Query = "hell",
                    Build = true,
                    Collate = true,
                    Count = 4,
                    Dictionary = "spanish",
                    OnlyMorePopular = true,
                    Reload = true,
                },
            }).ToList();
            Assert.Contains(p, KVP("spellcheck", "true"));
            Assert.Contains(p, KVP("spellcheck.q", "hell"));
            Assert.Contains(p, KVP("spellcheck.build", "true"));
            Assert.Contains(p, KVP("spellcheck.collate", "true"));
            Assert.Contains(p, KVP("spellcheck.count", "4"));
            Assert.Contains(p, KVP("spellcheck.dictionary", "spanish"));
            Assert.Contains(p, KVP("spellcheck.onlyMorePopular", "true"));
            Assert.Contains(p, KVP("spellcheck.reload", "true"));
        }

        [Test]
        public void GetAllParameters_with_spelling() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var querySerializer = new SolrQuerySerializerStub("*:*");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null);
            var p = queryExecuter.GetAllParameters(SolrQuery.All, new QueryOptions {
                SpellCheck = new SpellCheckingParameters {
                    Query = "hell",
                    Build = true,
                    Collate = true,
                    Count = 4,
                    Dictionary = "spanish",
                    OnlyMorePopular = true,
                    Reload = true,
                },
            }).ToList();
            Assert.Contains(p, KVP("spellcheck", "true"));
            Assert.Contains(p, KVP("spellcheck.q", "hell"));
            Assert.Contains(p, KVP("spellcheck.build", "true"));
            Assert.Contains(p, KVP("spellcheck.collate", "true"));
            Assert.Contains(p, KVP("spellcheck.count", "4"));
            Assert.Contains(p, KVP("spellcheck.dictionary", "spanish"));
            Assert.Contains(p, KVP("spellcheck.onlyMorePopular", "true"));
            Assert.Contains(p, KVP("spellcheck.reload", "true"));
        }

        [Test]
        public void MoreLikeThis() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var querySerializer = new SolrQuerySerializerStub("apache");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null);
            var p = queryExecuter.GetAllParameters(new SolrQuery("apache"), new QueryOptions {
                MoreLikeThis = new MoreLikeThisParameters(new[] { "manu", "cat" }) {
                    MinDocFreq = 1,
                    MinTermFreq = 1,
                },
            }).ToList();
            Assert.Contains(p, KVP("mlt", "true"));
            Assert.Contains(p, KVP("mlt.mindf", "1"));
            Assert.Contains(p, KVP("mlt.fl", "manu,cat"));
            Assert.Contains(p, KVP("mlt.mintf", "1"));
            Assert.Contains(p, KVP("q", "apache"));
        }

        [Test]
        public void GetMoreLikeThisParameters() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, null, null);
            var p = queryExecuter.GetMoreLikeThisParameters(new QueryOptions {
                MoreLikeThis = new MoreLikeThisParameters(new[] {"field1", "field2"}) {
                    Boost = true,
                    Count = 10,
                    QueryFields = new[] {"qf1", "qf2"},
                    MaxQueryTerms = 2,
                    MaxTokens = 3,
                    MaxWordLength = 4,
                    MinDocFreq = 5,
                    MinTermFreq = 6,
                    MinWordLength = 7,
                }
            }).ToList();
            Assert.Contains(p, KVP("mlt", "true"));
            Assert.Contains(p, KVP("mlt.boost", "true"));
            Assert.Contains(p, KVP("mlt.count", "10"));
            Assert.Contains(p, KVP("mlt.maxqt", "2"));
            Assert.Contains(p, KVP("mlt.maxntp", "3"));
            Assert.Contains(p, KVP("mlt.maxwl", "4"));
            Assert.Contains(p, KVP("mlt.mindf", "5"));
            Assert.Contains(p, KVP("mlt.mintf", "6"));
            Assert.Contains(p, KVP("mlt.minwl", "7"));
            Assert.Contains(p, KVP("mlt.fl", "field1,field2"));
            Assert.Contains(p, KVP("mlt.qf", "qf1,qf2"));
        }

        [Test]
        public void FacetFieldOptions() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var querySerializer = new SolrQuerySerializerStub("q");
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, null);
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, facetQuerySerializer);
            var facetOptions = queryExecuter.GetFacetFieldOptions(new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new List<ISolrFacetQuery> {
                        new SolrFacetQuery(new SolrQuery("q")),
                    },
                    Prefix = "pref",
                    EnumCacheMinDf = 123,
                    Limit = 100,
                    MinCount = 5,
                    Missing = true,
                    Offset = 55,
                    Sort = true,
                }
            }).ToDictionary(x => x.Key, x => x.Value);
            Assert.AreEqual("pref", facetOptions["facet.prefix"]);
            Assert.AreEqual("123", facetOptions["facet.enum.cache.minDf"]);
            Assert.AreEqual("100", facetOptions["facet.limit"]);
            Assert.AreEqual("5", facetOptions["facet.mincount"]);
            Assert.AreEqual("true", facetOptions["facet.missing"]);
            Assert.AreEqual("55", facetOptions["facet.offset"]);
            Assert.AreEqual("true", facetOptions["facet.sort"]);
        }

        [Test]
        public void StatsOptions() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, null, null);
            var statsOptions = queryExecuter.GetStatsQueryOptions(new QueryOptions {
                Stats = new StatsParameters()
                .AddField("popularity")
                .AddFieldWithFacet("price", "inStock")
                .AddFieldWithFacets("afield", "facet1", "facet2")
                .AddFacet("globalfacet")
            }).ToList();
            Assert.AreEqual(8, statsOptions.Count);
            AssertContains(statsOptions, "stats", "true");
            AssertContains(statsOptions, "stats.field", "popularity");
            AssertContains(statsOptions, "stats.field", "price");
            AssertContains(statsOptions, "f.price.stats.facet", "inStock");
            AssertContains(statsOptions, "stats.field", "afield");
            AssertContains(statsOptions, "f.afield.stats.facet", "facet1");
            AssertContains(statsOptions, "f.afield.stats.facet", "facet2");
            AssertContains(statsOptions, "stats.facet", "globalfacet");
        }

        public void AssertContains<K, V>(IEnumerable<KeyValuePair<K, V>> d, K key, V value) {
            foreach (var kv in d) {
                if (Equals(kv.Key, key) && Equals(kv.Value, value))
                    return;
            }
            Assert.Fail("KeyValue ('{0}','{1}') not found", key, value);
        }

        [Test]
        public void ExtraParams() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var serializer = mocks.StrictMock<ISolrQuerySerializer>();
            With.Mocks(mocks)
                .Expecting(() => Expect.On(serializer)
                    .Call(serializer.Serialize(null))
                    .IgnoreArguments()
                    .Return("123123"))
                .Verify(() => {
                    var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, null);
                    var p = queryExecuter.GetAllParameters(new SolrQuery("123123"), new QueryOptions {
                        ExtraParams = new Dictionary<string, string> {
                            {"qt", "geo"},
                            {"lat", "40.75141843299745"},
                            {"long", "-74.0093994140625"},
                            {"radius", "1"},
                        }
                    }).ToDictionary(x => x.Key, x => x.Value);
                    Assert.AreEqual("123123", p["q"]);
                    Assert.AreEqual("geo", p["qt"]);
                    Assert.AreEqual("1", p["radius"]);
                });
        }
    }
}