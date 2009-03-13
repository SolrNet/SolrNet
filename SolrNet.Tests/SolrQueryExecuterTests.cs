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

using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrQueryExecuterTests {
        public class TestDocument : ISolrDocument {
            [SolrUniqueKey]
            public int Id { get; set; }

            public string OtherField { get; set; }
        }

        [Test]
        public void Execute() {
            const string queryString = "id:123456";
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var conn = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            With.Mocks(mocks).Expecting(() => {
                var q = new Dictionary<string, string>();
                q["q"] = queryString;
                Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
                Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
                var r = queryExecuter.Execute(new SolrQuery(queryString), null);
            });
        }

        [Test]
        public void Sort() {
            const string queryString = "id:123456";
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var conn = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            With.Mocks(mocks).Expecting(() => {
                var q = new Dictionary<string, string>();
                q["q"] = queryString;
                q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
                q["sort"] = "id asc";
                Expect.Call(conn.Get("/select", q))
                    .Repeat.Once()
                    .Return("");
                Expect.Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(mockR);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
                var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                    OrderBy = new[] {new SortOrder("id")}
                });
            });
        }

        [Test]
        public void SortMultipleWithOrders() {
            const string queryString = "id:123456";
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var conn = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            With.Mocks(mocks).Expecting(() => {
                var q = new Dictionary<string, string>();
                q["q"] = queryString;
                q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
                q["sort"] = "id asc,name desc";
                Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
                Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
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
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var conn = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
            var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
            With.Mocks(mocks).Expecting(delegate {
                var q = new Dictionary<string, string>();
                q["q"] = queryString;
                q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
                q["fl"] = "id,name";
                Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
                Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
                var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                    Fields = new[] {"id", "name"},
                });
            });
        }

        [Test]
        public void Facets() {
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var conn = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            With.Mocks(mocks).Expecting(() => {
                var q = new Dictionary<string, string>();
                q["q"] = "";
                q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
                q["facet"] = "true";
                q["facet.field"] = "Id";
                q["facet.query"] = "id:[1 TO 5]";
                Expect.Call(conn.Get("/select", q))
                    .Repeat.Once().Return("");
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
                queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                    FacetQueries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetQuery(new SolrQuery("id:[1 TO 5]")),
                    },
                });
            });
        }

        public KeyValuePair<T1, T2> KVP<T1, T2>(T1 a, T2 b) {
            return new KeyValuePair<T1, T2>(a, b);
        }

        [Test]
        public void MultipleFacetFields() {
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var conn = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            With.Mocks(mocks).Expecting(() => {
                var q = new List<KeyValuePair<string, string>> {
                    KVP("q", ""),
                    KVP("rows", SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString()),
                    KVP("facet", "true"),
                    KVP("facet.field", "Id"),
                    KVP("facet.field", "OtherField"),
                };
                Expect.Call(conn.Get("/select", q))
                    .Repeat.Once().Return("");
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
                queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                    FacetQueries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetFieldQuery("OtherField"),
                    },
                });
            });
        }

        [Test]
        public void Highlighting() {
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var conn = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            const string highlightedField = "field1";
            const string afterTerm = "after";
            const string beforeTerm = "before";
            const int snippets = 3;
            const string alt = "alt";
            const int fragsize = 7;
            With.Mocks(mocks).Expecting(() => {
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
                Expect.Call(conn.Get("/select", q))
                    .Repeat.Once().Return("");
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
                queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                    Highlight = new HighlightingParameters {
                        Fields = new[] {highlightedField},
                        AfterTerm = afterTerm,
                        BeforeTerm = beforeTerm,
                        Snippets = snippets,
                        AlternateField = alt,
                        Fragsize = fragsize,
                        RequireFieldMatch = true,
                        RegexSlop = 4.12,
                        RegexPattern = "\\.",
                        RegexMaxAnalyzedChars = 8000,
                    }
                });
            });
        }

        [Test]
        public void FilterQuery() {
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceLocator>();
            ServiceLocator.SetLocatorProvider(() => container);
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = new MockConnection(new[] {
                KVP("q", "*:*"),
                KVP("rows", "10"),
                KVP("fq", "id:0"),
                KVP("fq", "id:2"),
            });
            var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser) {
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
            var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
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
            Assert.Contains(KVP("spellcheck", "true"), p);
            Assert.Contains(KVP("spellcheck.q", "hell"), p);
            Assert.Contains(KVP("spellcheck.build", "true"), p);
            Assert.Contains(KVP("spellcheck.collate", "true"), p);
            Assert.Contains(KVP("spellcheck.count", "4"), p);
            Assert.Contains(KVP("spellcheck.dictionary", "spanish"), p);
            Assert.Contains(KVP("spellcheck.onlyMorePopular", "true"), p);
            Assert.Contains(KVP("spellcheck.reload", "true"), p);
        }

        [Test]
        public void GetAllParameters_with_spelling() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
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
            Assert.Contains(KVP("spellcheck", "true"), p);
            Assert.Contains(KVP("spellcheck.q", "hell"), p);
            Assert.Contains(KVP("spellcheck.build", "true"), p);
            Assert.Contains(KVP("spellcheck.collate", "true"), p);
            Assert.Contains(KVP("spellcheck.count", "4"), p);
            Assert.Contains(KVP("spellcheck.dictionary", "spanish"), p);
            Assert.Contains(KVP("spellcheck.onlyMorePopular", "true"), p);
            Assert.Contains(KVP("spellcheck.reload", "true"), p);
        }

        [Test]
        public void MoreLikeThis() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
            var p = queryExecuter.GetAllParameters(new SolrQuery("apache"), new QueryOptions {
                MoreLikeThis = new MoreLikeThisParameters(new[] { "manu", "cat" }) {
                    MinDocFreq = 1,
                    MinTermFreq = 1,
                },
            }).ToList();
            Assert.Contains(KVP("mlt", "true"), p);
            Assert.Contains(KVP("mlt.mindf", "1"), p);
            Assert.Contains(KVP("mlt.fl", "manu,cat"), p);
            Assert.Contains(KVP("mlt.mintf", "1"), p);
            Assert.Contains(KVP("q", "apache"), p);
        }

        [Test]
        public void GetMoreLikeThisParameters() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
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
            Assert.Contains(KVP("mlt", "true"), p);
            Assert.Contains(KVP("mlt.boost", "true"), p);
            Assert.Contains(KVP("mlt.count", "10"), p);
            Assert.Contains(KVP("mlt.maxqt", "2"), p);
            Assert.Contains(KVP("mlt.maxntp", "3"), p);
            Assert.Contains(KVP("mlt.maxwl", "4"), p);
            Assert.Contains(KVP("mlt.mindf", "5"), p);
            Assert.Contains(KVP("mlt.mintf", "6"), p);
            Assert.Contains(KVP("mlt.minwl", "7"), p);
            Assert.Contains(KVP("mlt.fl", "field1,field2"), p);
            Assert.Contains(KVP("mlt.qf", "qf1,qf2"), p);
        }

        [Test]
        public void ExtraParams() {
            var mocks = new MockRepository();
            var parser = mocks.DynamicMock<ISolrQueryResultParser<TestDocument>>();
            var conn = mocks.DynamicMock<ISolrConnection>();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, parser);
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
        }
    }
}