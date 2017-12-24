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
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moroco;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Tests.Mocks;
using SolrNet.Utils;

namespace SolrNet.Tests {
    
    public class SolrQueryExecuterTests {
        public class TestDocument {
            [SolrUniqueKey]
            public int Id { get; set; }

            public string OtherField { get; set; }
        }

        [Fact]
        public void Execute()
        {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            var conn = new MockConnection(q);
            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => queryString;
            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();

            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), null);
            Assert.Equal(1, serializer.serialize.Calls);
        }

        [Fact]
        public void RequestHandler()
        {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["handler"] = "/update";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            var queryOptions = new QueryOptions {
                RequestHandler = new RequestHandlerParameters("/update")
            };
            var conn = new MockConnection(q);
            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => queryString;
            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();

            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), queryOptions);
        }

        [Fact]
        public void Sort() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["sort"] = "id asc";
            var conn = new MockConnection(q);
            var querySerializer = new SolrQuerySerializerStub(queryString);
            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Expect(1);
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                OrderBy = new[] { new SortOrder("id") }
            });
            parser.parse.Verify();
        }

        [Fact]
        public void SortMultipleWithOrders() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["sort"] = "id asc,name desc";
            var conn = new MockConnection(q);

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var querySerializer = new SolrQuerySerializerStub(queryString);
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                OrderBy = new[] {
                        new SortOrder("id", Order.ASC),
                        new SortOrder("name", Order.DESC)
                    }
            });
        }

        [Fact]
        public void ResultFields() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["fl"] = "id,name";
            var conn = new MockConnection(q);
            var querySerializer = new SolrQuerySerializerStub(queryString);

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                Fields = new[] { "id", "name" },
            });
        }

        [Fact]
        public void Facets() {
            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["facet"] = "true";
            q["facet.field"] = "Id";
            q["facet.query"] = "id:[1 TO 5]";
            var conn = new MockConnection(q);
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, facetQuerySerializer, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetQuery(new SolrQuery("id:[1 TO 5]")),
                    }
                }
            });
        }

        [Fact]
        public void MultipleFacetFields() {
            var conn = new MockConnection(new[] {
                KV.Create("q", ""),
                KV.Create("rows", SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString()),
                KV.Create("facet", "true"),
                KV.Create("facet.field", "Id"),
                KV.Create("facet.field", "OtherField"),
            });
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var facetQuerySerializer = new DefaultFacetQuerySerializer(serializer, new DefaultFieldSerializer());

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, serializer, facetQuerySerializer, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("Id"),
                        new SolrFacetFieldQuery("OtherField"),
                    }
                }
            });
        }

        [Fact]
        public void Highlighting() {
            const string highlightedField = "field1";
            const string afterTerm = "after";
            const string beforeTerm = "before";
            const int snippets = 3;
            const string alt = "alt";
            const int fragsize = 7;
            const string query = "mausch";
            var highlightQuery = new SolrQuery(query);
            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["hl"] = "true";
            q["hl.q"] = query;
            q["hl.fl"] = highlightedField;
            q["hl.snippets"] = snippets.ToString();
            q["hl.fragsize"] = fragsize.ToString();
            q["hl.requireFieldMatch"] = "true";
            q["hl.alternateField"] = alt;
            q["hl.tag.pre"] = beforeTerm;
            q["hl.tag.post"] = afterTerm;
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
            var querySerializer = new DefaultQuerySerializer(new MSolrFieldSerializer());

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Highlight = new HighlightingParameters {
                    Fields = new[] { highlightedField },
                    AfterTerm = afterTerm,
                    BeforeTerm = beforeTerm,
                    Query = highlightQuery,
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

        [Fact]
        public void HighlightingWithoutFieldsOutputsPrePost() {
            const string afterTerm = "after";
            const string beforeTerm = "before";

            var q = new Dictionary<string, string>();
            q["q"] = "";
            q["rows"] = SolrQueryExecuter<TestDocument>.ConstDefaultRows.ToString();
            q["hl"] = "true";
            q["hl.tag.pre"] = beforeTerm;
            q["hl.tag.post"] = afterTerm;
            q["hl.useFastVectorHighlighter"] = "true";

            var conn = new MockConnection(q);
            var querySerializer = new SolrQuerySerializerStub("");

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null);
            queryExecuter.Execute(new SolrQuery(""), new QueryOptions {
                Highlight = new HighlightingParameters {
                    AfterTerm = afterTerm,
                    BeforeTerm = beforeTerm,
                    UseFastVectorHighlighter = true,
                }
            });
        }


        [Fact]
        public void HighlightingWithFastVectorHighlighter() {
            var e = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = e.GetHighlightingParameters(new QueryOptions {
                Highlight = new HighlightingParameters {
                    Fields = new[] {"a"},
                    AfterTerm = "after",
                    BeforeTerm = "before",
                    UseFastVectorHighlighter = true,
                }
            });
            Assert.Equal("true", p["hl.useFastVectorHighlighter"]);
            Assert.Equal("before", p["hl.tag.pre"]);
            Assert.Equal("after", p["hl.tag.post"]);
            Assert.False(p.ContainsKey("hl.simple.pre"));
            Assert.False(p.ContainsKey("hl.simple.post"));
        }

        [Fact]
        public void FilterQuery() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var conn = new MockConnection(new[] {
                KV.Create("q", "*:*"),
                KV.Create("rows", "10"),
                KV.Create("fq", "id:0"),
                KV.Create("fq", "id:2"),
            });

            var parser = new MSolrAbstractResponseParser<TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrQueryExecuter<TestDocument>(parser, conn, querySerializer, null, null) {
                DefaultRows = 10,
            };
            queryExecuter.Execute(SolrQuery.All, new QueryOptions {
                FilterQueries = new[] {
                    new SolrQuery("id:0"),
                    new SolrQuery("id:2"),
                },
            });
        }

        [Fact]
        public void SpellChecking() {
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
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
            Assert.Contains( KV.Create("spellcheck", "true"),p);
            Assert.Contains( KV.Create("spellcheck.q", "hell"),p);
            Assert.Contains( KV.Create("spellcheck.build", "true"),p);
            Assert.Contains( KV.Create("spellcheck.collate", "true"),p);
            Assert.Contains( KV.Create("spellcheck.count", "4"),p);
            Assert.Contains( KV.Create("spellcheck.dictionary", "spanish"),p);
            Assert.Contains( KV.Create("spellcheck.onlyMorePopular", "true"),p);
            Assert.Contains( KV.Create("spellcheck.reload", "true"),p);
        }

        [Fact]
        public void TermsSingleField() {
            var p = SolrQueryExecuter<TestDocument>.GetTermsParameters(new QueryOptions {
                Terms = new TermsParameters("text") {
                    Limit = 10,
                    Lower = "lower",
                    LowerInclude = true,
                    MaxCount = 10,
                    MinCount = 0,
                    Prefix = "pre",
                    Raw = true,
                    Regex = "regex",
                    RegexFlag = new[] { RegexFlag.CanonEq, RegexFlag.CaseInsensitive },
                    Sort = TermsSort.Count,
                    Upper = "upper",
                    UpperInclude = true
                },
            }).ToList();
            Assert.Contains( KV.Create("terms", "true"),p);
            Assert.Contains( KV.Create("terms.fl", "text"),p);
            Assert.Contains( KV.Create("terms.lower", "lower"),p);
            Assert.Contains( KV.Create("terms.lower.incl", "true"),p);
            Assert.Contains( KV.Create("terms.maxcount", "10"),p);
            Assert.Contains( KV.Create("terms.mincount", "0"),p);
            Assert.Contains( KV.Create("terms.prefix", "pre"),p);
            Assert.Contains( KV.Create("terms.raw", "true"),p);
            Assert.Contains( KV.Create("terms.regex", "regex"),p);
            Assert.Contains( KV.Create("terms.regex.flag", RegexFlag.CanonEq.ToString()),p);
            Assert.Contains( KV.Create("terms.regex.flag", RegexFlag.CaseInsensitive.ToString()),p);
            Assert.Contains( KV.Create("terms.sort", "count"),p);
            Assert.Contains( KV.Create("terms.upper", "upper"),p);
            Assert.Contains( KV.Create("terms.upper.incl", "true"),p);
        }

        [Fact]
        public void TermsMultipleFields() {
            var p = SolrQueryExecuter<TestDocument>.GetTermsParameters(new QueryOptions {
                Terms = new TermsParameters(new List<string> { "text", "text2", "text3" }) {
                    Limit = 10,
                    Lower = "lower",
                    LowerInclude = true,
                    MaxCount = 10,
                    MinCount = 0,
                    Prefix = "pre",
                    Raw = true,
                    Regex = "regex",
                    RegexFlag = new[] { RegexFlag.CanonEq, RegexFlag.CaseInsensitive },
                    Sort = TermsSort.Count,
                    Upper = "upper",
                    UpperInclude = true
                },
            }).ToList();
            Assert.Contains( KV.Create("terms", "true"),p);
            Assert.Contains( KV.Create("terms.fl", "text"),p);
            Assert.Contains( KV.Create("terms.fl", "text2"),p);
            Assert.Contains( KV.Create("terms.fl", "text3"),p);
            Assert.Contains( KV.Create("terms.lower", "lower"),p);
            Assert.Contains( KV.Create("terms.lower.incl", "true"),p);
            Assert.Contains( KV.Create("terms.maxcount", "10"),p);
            Assert.Contains( KV.Create("terms.mincount", "0"),p);
            Assert.Contains( KV.Create("terms.prefix", "pre"),p);
            Assert.Contains( KV.Create("terms.raw", "true"),p);
            Assert.Contains( KV.Create("terms.regex", "regex"),p);
            Assert.Contains( KV.Create("terms.regex.flag", RegexFlag.CanonEq.ToString()),p);
            Assert.Contains( KV.Create("terms.regex.flag", RegexFlag.CaseInsensitive.ToString()),p);
            Assert.Contains( KV.Create("terms.sort", "count"),p);
            Assert.Contains( KV.Create("terms.upper", "upper"),p);
            Assert.Contains( KV.Create("terms.upper.incl", "true"),p);
        }

        [Fact]
        public void GetTermVectorParameterOptions_All() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.All).ToList();
            Assert.Equal(1, r.Count);
            Assert.Equal("tv.all", r[0]);
        }

        [Fact]
        public void GetTermVectorParameterOptions_All_indirect() {
            const TermVectorParameterOptions o = 
                TermVectorParameterOptions.DocumentFrequency 
                | TermVectorParameterOptions.TermFrequency 
                | TermVectorParameterOptions.Positions 
                | TermVectorParameterOptions.Offsets 
                | TermVectorParameterOptions.TermFrequency_InverseDocumentFrequency;
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(o).ToList();
            Assert.Equal(1, r.Count);
            Assert.Equal("tv.all", r[0]);
        }

        [Fact]
        public void GetTermVectorParameterOptions_Tf() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.TermFrequency).ToList();
            Assert.Equal(1, r.Count);
            Assert.Equal("tv.tf", r[0]);
        }

        [Fact]
        public void GetTermVectorParameterOptions_Df() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.DocumentFrequency).ToList();
            Assert.Equal(1, r.Count);
            Assert.Equal("tv.df", r[0]);
        }

        [Fact]
        public void GetTermVectorParameterOptions_default() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.Default).ToList();
            Assert.Equal(0, r.Count);
        }

        [Fact]
        public void GetTermVectorParameterOptions_TfDf() {
            const TermVectorParameterOptions o =
                TermVectorParameterOptions.DocumentFrequency
                | TermVectorParameterOptions.TermFrequency;
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(o).ToList();
            Assert.Equal(2, r.Count);
            Assert.Contains( "tv.df",r);
            Assert.Contains( "tv.tf",r);
        }

        [Fact]
        public void GetTermVectorParameterOptions_offsets() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.Offsets).ToList();
            Assert.Equal(1, r.Count);
            Assert.Equal("tv.offsets", r[0]);
        }

        [Fact]
        public void GetTermVectorParameterOptions_tfidf() {
            var r = SolrQueryExecuter<object>.GetTermVectorParameterOptions(TermVectorParameterOptions.TermFrequency_InverseDocumentFrequency).ToList();
            Assert.Equal(3, r.Count);
            Assert.Contains( "tv.df",r);
            Assert.Contains( "tv.tf",r);
            Assert.Contains( "tv.tf_idf",r);
        }

		[Fact]
		public void TermVector() {
            var p = SolrQueryExecuter<TestDocument>.GetTermVectorQueryOptions(new QueryOptions {
				TermVector = new TermVectorParameters {
                    Fields = new[] {"text"},
                    Options = TermVectorParameterOptions.All,
				},
			}).ToList();
			Assert.Contains( KV.Create("tv", "true"),p);
			Assert.Contains( KV.Create("tv.all", "true"),p);
			Assert.Contains( KV.Create("tv.fl", "text"),p);
		}

        [Fact]
        public void GetAllParameters_with_spelling() {
            var querySerializer = new SolrQuerySerializerStub("*:*");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
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
            Assert.Contains( KV.Create("spellcheck", "true"),p);
            Assert.Contains( KV.Create("spellcheck.q", "hell"),p);
            Assert.Contains( KV.Create("spellcheck.build", "true"),p);
            Assert.Contains( KV.Create("spellcheck.collate", "true"),p);
            Assert.Contains( KV.Create("spellcheck.count", "4"),p);
            Assert.Contains( KV.Create("spellcheck.dictionary", "spanish"),p);
            Assert.Contains( KV.Create("spellcheck.onlyMorePopular", "true"),p);
            Assert.Contains( KV.Create("spellcheck.reload", "true"),p);
        }

        [Fact]
        public void MoreLikeThis() {
            var querySerializer = new SolrQuerySerializerStub("apache");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = queryExecuter.GetAllParameters(new SolrQuery("apache"), new QueryOptions {
                MoreLikeThis = new MoreLikeThisParameters(new[] { "manu", "cat" }) {
                    MinDocFreq = 1,
                    MinTermFreq = 1,
                },
            }).ToList();
            Assert.Contains( KV.Create("mlt", "true"),p);
            Assert.Contains( KV.Create("mlt.mindf", "1"),p);
            Assert.Contains( KV.Create("mlt.fl", "manu,cat"),p);
            Assert.Contains( KV.Create("mlt.mintf", "1"),p);
            Assert.Contains( KV.Create("q", "apache"),p);
        }

        [Fact]
        public void GetMoreLikeThisParameters() {
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = queryExecuter.GetMoreLikeThisParameters(
                new MoreLikeThisParameters(new[] { "field1", "field2" }) {
                    Boost = true,
                    Count = 10,
                    QueryFields = new[] { "qf1", "qf2" },
                    MaxQueryTerms = 2,
                    MaxTokens = 3,
                    MaxWordLength = 4,
                    MinDocFreq = 5,
                    MinTermFreq = 6,
                    MinWordLength = 7,
                }).ToList();
            Assert.Contains( KV.Create("mlt", "true"),p);
            Assert.Contains( KV.Create("mlt.boost", "true"),p);
            Assert.Contains( KV.Create("mlt.count", "10"),p);
            Assert.Contains( KV.Create("mlt.maxqt", "2"),p);
            Assert.Contains( KV.Create("mlt.maxntp", "3"),p);
            Assert.Contains( KV.Create("mlt.maxwl", "4"),p);
            Assert.Contains( KV.Create("mlt.mindf", "5"),p);
            Assert.Contains( KV.Create("mlt.mintf", "6"),p);
            Assert.Contains( KV.Create("mlt.minwl", "7"),p);
            Assert.Contains( KV.Create("mlt.fl", "field1,field2"),p);
            Assert.Contains( KV.Create("mlt.qf", "qf1,qf2"),p);
        }

        [Fact]
        public void GetAllParameters_mlt_with_field_query() {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var qe = new SolrQueryExecuter<TestDocument>(null, null, serializer, null, null);
            var p = qe.GetAllMoreLikeThisHandlerParameters(
                new SolrMoreLikeThisHandlerQuery(new SolrQueryByField("id", "1234")),
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] { "one", "three" }) {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    }) {
                        Start = 0,
                        Rows = 5,
                        Fields = new[] { "one", "two", "three" },
                    }).ToList();
            Assert.Contains( KV.Create("q", "id:(1234)"),p);
            Assert.Contains( KV.Create("start", "0"),p);
            Assert.Contains( KV.Create("rows", "5"),p);
            Assert.Contains( KV.Create("fl", "one,two,three"),p);
            Assert.Contains( KV.Create("mlt.fl", "one,three"),p);
            Assert.Contains( KV.Create("mlt.match.include", "false"),p);
            Assert.Contains( KV.Create("mlt.match.offset", "5"),p);
            Assert.Contains( KV.Create("mlt.interestingTerms", "none"),p);
        }

        [Fact]
        public void GetAllParameters_mlt_with_stream_body_query() {
            var qe = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = qe.GetAllMoreLikeThisHandlerParameters(
                new SolrMoreLikeThisHandlerStreamBodyQuery("one two three"),
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] { "one", "three" })
                    {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    })
                    {
                        Start = 0,
                        Rows = 5,
                        Fields = new[] { "one", "two", "three" },
                    }).ToList();
            Assert.Contains( KV.Create("stream.body", "one two three"),p);
        }

        [Fact]
        public void GetAllParameters_mlt_with_stream_url_query() {
            var qe = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = qe.GetAllMoreLikeThisHandlerParameters(
                new SolrMoreLikeThisHandlerStreamUrlQuery("http://wiki.apache.org/solr/MoreLikeThisHandler"),
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] { "one", "three" })
                    {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    })
                    {
                        Start = 0,
                        Rows = 5,
                        Fields = new[] { "one", "two", "three" },
                    }).ToList();
            Assert.Contains( KV.Create("stream.url", "http://wiki.apache.org/solr/MoreLikeThisHandler"),p);
        }

        [Fact]
        public void FacetFieldOptions() {
            var querySerializer = new SolrQuerySerializerStub("q");
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, null);
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, facetQuerySerializer, null);
            var facetOptions = queryExecuter.GetFacetFieldOptions(
                new FacetParameters {
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
                    Threads = 5
                }).ToDictionary(x => x.Key, x => x.Value);
            Assert.Equal("pref", facetOptions["facet.prefix"]);
            Assert.Equal("123", facetOptions["facet.enum.cache.minDf"]);
            Assert.Equal("100", facetOptions["facet.limit"]);
            Assert.Equal("5", facetOptions["facet.mincount"]);
            Assert.Equal("true", facetOptions["facet.missing"]);
            Assert.Equal("55", facetOptions["facet.offset"]);
            Assert.Equal("true", facetOptions["facet.sort"]);
            Assert.Equal("5", facetOptions["facet.threads"]);
        }

        [Fact]
        public void StatsOptions() {
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var statsOptions = queryExecuter.GetStatsQueryOptions(new QueryOptions {
                Stats = new StatsParameters()
                    .AddField("popularity")
                    .AddFieldWithFacet("price", "inStock")
                    .AddFieldWithFacets("afield", "facet1", "facet2")
                    .AddFacet("globalfacet")
            }).ToList();
            Assert.Equal(8, statsOptions.Count);
            Assert.Contains( KV.Create("stats", "true"),statsOptions);
            Assert.Contains( KV.Create("stats.field", "popularity"),statsOptions);
            Assert.Contains( KV.Create("stats.field", "price"),statsOptions);
            Assert.Contains( KV.Create("f.price.stats.facet", "inStock"),statsOptions);
            Assert.Contains( KV.Create("stats.field", "afield"),statsOptions);
            Assert.Contains( KV.Create("f.afield.stats.facet", "facet1"),statsOptions);
            Assert.Contains( KV.Create("f.afield.stats.facet", "facet2"),statsOptions);
            Assert.Contains( KV.Create("stats.facet", "globalfacet"),statsOptions);
        }

        [Fact]
        public void ExtraParams() {
            var querySerializer = new SolrQuerySerializerStub("123123");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = queryExecuter.GetAllParameters(new SolrQuery("123123"), new QueryOptions {
                ExtraParams = new Dictionary<string, string> {
                            {"qt", "geo"},
                            {"lat", "40.75141843299745"},
                            {"long", "-74.0093994140625"},
                            {"radius", "1"},
                        }
            }).ToDictionary(x => x.Key, x => x.Value);
            Assert.Equal("123123", p["q"]);
            Assert.Equal("geo", p["qt"]);
            Assert.Equal("1", p["radius"]);
        }

        [Fact]
        public void GetClusteringParameters() {
            var querySerializer = new SolrQuerySerializerStub("apache");
            var queryExecuter = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = queryExecuter.GetAllParameters(new SolrQuery("apache"), new QueryOptions {
                Clustering = new ClusteringParameters {
                    Title = "headline",
                    FragSize = 10,
                    LexicalResources = "fakedir",
                    ProduceSummary = true,
                    Algorithm = "org.carrot2.clustering.lingo.LingoClusteringAlgorithm",
                    Url = "none",
                    Collection = false,
                    Engine = "default",
                    SubClusters = false,
                    Snippet = "synopsis",
                    NumDescriptions = 20
                },
            }).ToList();
            Assert.Contains( KV.Create("carrot.title", "headline"),p);
            Assert.Contains( KV.Create("clustering.engine", "default"),p);
            Assert.Contains( KV.Create("clustering.collection", "false"),p);
            Assert.Contains( KV.Create("carrot.algorithm", "org.carrot2.clustering.lingo.LingoClusteringAlgorithm"),p);
            Assert.Contains( KV.Create("carrot.url", "none"),p);
            Assert.Contains( KV.Create("carrot.snippet", "synopsis"),p);
            Assert.Contains( KV.Create("carrot.produceSummary", "true"),p);
            Assert.Contains( KV.Create("carrot.fragSize", "10"),p);
            Assert.Contains( KV.Create("carrot.numDescriptions", "20"),p);
            Assert.Contains( KV.Create("carrot.outputSubClusters", "false"),p);
            Assert.Contains( KV.Create("carrot.lexicalResourcesDir", "fakedir"),p);
        }

        [Fact]
        public void GetCursormarkWithDefaultSetup()
        {
            var e = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = e.GetCommonParameters(new CommonQueryOptions {
                StartOrCursor = StartOrCursor.Cursor.Start
            });

            Assert.Equal("cursorMark", p.First().Key);
            Assert.Equal("*", p.First().Value);
        }

        [Fact]
        public void GetCursormarkWithMarkSet()
        {
            var e = new SolrQueryExecuter<TestDocument>(null, null, null, null, null);
            var p = e.GetCommonParameters(new CommonQueryOptions {
                StartOrCursor = new StartOrCursor.Cursor("AoEoZTQ3YmY0NDM=")
            });

            Assert.Equal("cursorMark", p.First().Key);
            Assert.Equal("AoEoZTQ3YmY0NDM=", p.First().Value);
        }

        [Fact]
        public void GetCollapseExpandParameters() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var e = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var p = e.GetAllParameters(SolrQuery.All, new QueryOptions {
                Rows = 1,
                CollapseExpand = new CollapseExpandParameters("somefield", null, null, null),
            }).ToList();
            Assert.Contains( KV.Create("fq", "{!collapse field=somefield}"),p);
        }

        [Fact]
        public void GetCollapseExpandParameters_min_policy() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var e = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var max = new CollapseExpandParameters.MinOrMax.Max("maxfield");
            var policy = CollapseExpandParameters.NullPolicyType.Collapse;
            var p = e.GetAllParameters(SolrQuery.All, new QueryOptions {
                Rows = 1,
                CollapseExpand = new CollapseExpandParameters("somefield", null, max, policy),
            }).ToList();
            Assert.Contains( KV.Create("fq", "{!collapse field=somefield nullPolicy=collapse max=maxfield}"),p);
        }

        [Fact]
        public void GetCollapseExpandParameters_Expand() {
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var e = new SolrQueryExecuter<TestDocument>(null, null, querySerializer, null, null);
            var expand = new ExpandParameters(
                sort: new SortOrder("sortField", Order.ASC),
                rows: 100,
                query: new SolrQuery("aquery"),
                filterQuery: null);

            var p = e.GetAllParameters(SolrQuery.All, new QueryOptions {
                Rows = 1,
                CollapseExpand = new CollapseExpandParameters("somefield", expand, null, null),
            }).ToList();
            Assert.Contains( KV.Create("fq", "{!collapse field=somefield}"),p);
            Assert.Contains( KV.Create("expand.sort", "sortField asc"),p);
            Assert.Contains( KV.Create("expand.rows", "100"),p);
            Assert.Contains( KV.Create("expand.q", "aquery"),p);
        }
    }
}