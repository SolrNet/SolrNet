using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using Moroco;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Tests.Mocks;
using SolrNet.Utils;
using Xunit;
using Unit = Moroco.Unit;

namespace SolrNet.Tests
{
    public class SolrPostQueryExecuterTests
    {
        [Fact]
        public void ExecuteWithoutBodyContent()
        {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => queryString;
            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();

            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, serializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), null);
            Assert.Equal(1, serializer.serialize.Calls);
        }

        [Fact]
        public void ExecuteWithBodyContent()
        {
            const string queryJson = "{ \"query\": \"id:123456\" }";
            var q = new Dictionary<string, string>();
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, queryJson);
            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => string.Empty;
            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();

            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, serializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(null), new QueryOptions()
            {
                QueryBody = new SimpleJsonQueryBody(queryJson)
            });
            Assert.Equal(1, serializer.serialize.Calls);
        }

        [Fact]
        public void RequestHandler()
        {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["handler"] = "/update";
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
            var queryOptions = new QueryOptions {
                RequestHandler = new RequestHandlerParameters("/update")
            };
            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => queryString;
            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();

            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, serializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), queryOptions);
        }

        [Fact]
        public void Sort() {
            const string queryString = "id:123456";
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
            q["sort"] = "id asc";
            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var querySerializer = new SolrQuerySerializerStub(queryString);
            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Expect(1);
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, querySerializer, null, null);
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
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
            q["sort"] = "id asc,name desc";
            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);

            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();
            var querySerializer = new SolrQuerySerializerStub(queryString);
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, querySerializer, null, null);
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
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
            q["fl"] = "id,name";
            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var querySerializer = new SolrQuerySerializerStub(queryString);

            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, querySerializer, null, null);
            var r = queryExecuter.Execute(new SolrQuery(queryString), new QueryOptions {
                Fields = new[] { "id", "name" },
            });
        }

        [Fact]
        public void Facets() {
            var q = new Dictionary<string, string>();
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
            q["facet"] = "true";
            q["facet.field"] = "Id";
            q["facet.query"] = "id:[1 TO 5]";
            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());

            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, querySerializer, facetQuerySerializer, null);
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
                KV.Create("rows", AbstractSolrQueryExecuter.ConstDefaultRows.ToString()),
                KV.Create("facet", "true"),
                KV.Create("facet.field", "Id"),
                KV.Create("facet.field", "OtherField"),
            }, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var facetQuerySerializer = new DefaultFacetQuerySerializer(serializer, new DefaultFieldSerializer());

            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, serializer, facetQuerySerializer, null);
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
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
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

            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var querySerializer = new DefaultQuerySerializer(new MSolrFieldSerializer());

            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, querySerializer, null, null);
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
            q["rows"] = AbstractSolrQueryExecuter.ConstDefaultRows.ToString();
            q["hl"] = "true";
            q["hl.tag.pre"] = beforeTerm;
            q["hl.tag.post"] = afterTerm;
            q["hl.useFastVectorHighlighter"] = "true";

            var conn = new MockConnection(q, SimpleJsonQueryBody.ApplicationJson, string.Empty);
            var querySerializer = new SolrQuerySerializerStub("");

            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, querySerializer, null, null);
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
            var e = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, null, null, null, null);
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
            }, SimpleJsonQueryBody.ApplicationJson, string.Empty);

            var parser = new MSolrAbstractResponseParser<SolrQueryExecuterTests.TestDocument>();
            parser.parse &= x => x.Stub();
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(parser, conn, querySerializer, null, null) {
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
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, null, null, null, null);
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
        public void MoreLikeThis() {
            var querySerializer = new SolrQuerySerializerStub("apache");
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, null, querySerializer, null, null);
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
            var queryExecuter = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, null, null, null, null);
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
            var qe = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, null, serializer, null, null);
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
            var qe = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, null, null, null, null);
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
            Assert.DoesNotContain( KV.Create("stream.body", "one two three"),p);
        }

        [Fact]
        public void ExecuteMLT_with_stream_body_query() {
            var parser = new MSolrMoreLikeThisHandlerQueryResultsParser<SolrQueryExecuterTests.TestDocument>();
            var q = new Dictionary<string, string>();
            q["mlt"] = "true";
            q["mlt.fl"] = "one,three";
            q["mlt.match.include"] = "false";
            q["mlt.match.offset"] = "5";
            q["mlt.interestingTerms"] = InterestingTerms.None.ToString().ToLowerInvariant();
            q["start"] = "0";
            q["rows"] = "5";
            q["fl"] = "one,two,three";
            var conn = new MockConnection(q, MediaTypeNames.Text.Plain, "one two three");

            var qe = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, conn, null, null, parser);
            qe.Execute(new SolrMoreLikeThisHandlerStreamBodyQuery("one two three"),
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] {"one", "three"})
                    {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    })
                {
                    Start = 0,
                    Rows = 5,
                    Fields = new[] {"one", "two", "three"},
                });
        }

        [Fact]
        public void ExecuteMLT_with_stream_body_option()
        {
            const string queryString = "my query";

            var parser = new MSolrMoreLikeThisHandlerQueryResultsParser<SolrQueryExecuterTests.TestDocument>();
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["mlt"] = "true";
            q["mlt.fl"] = "one,three";
            q["mlt.match.include"] = "false";
            q["mlt.match.offset"] = "5";
            q["mlt.interestingTerms"] = InterestingTerms.None.ToString().ToLowerInvariant();
            q["start"] = "0";
            q["rows"] = "5";
            q["fl"] = "one,two,three";
            var conn = new MockConnection(q, MediaTypeNames.Text.Plain, "one two three");

            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => queryString;
            var qe = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, conn, serializer, null, parser);
            qe.Execute(new SolrMoreLikeThisHandlerQuery(new SolrQuery(queryString)), 
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] {"one", "three"})
                    {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    })
                {
                    Start = 0,
                    Rows = 5,
                    Fields = new[] {"one", "two", "three"},
                    QueryBody = new PlainTextQueryBody("one two three")
                });
        }

        [Fact]
        public void ExecuteMLT_with_no_body()
        {
            const string queryString = "my query";

            var parser = new MSolrMoreLikeThisHandlerQueryResultsParser<SolrQueryExecuterTests.TestDocument>();
            var q = new Dictionary<string, string>();
            q["q"] = queryString;
            q["mlt"] = "true";
            q["mlt.fl"] = "one,three";
            q["mlt.match.include"] = "false";
            q["mlt.match.offset"] = "5";
            q["mlt.interestingTerms"] = InterestingTerms.None.ToString().ToLowerInvariant();
            q["start"] = "0";
            q["rows"] = "5";
            q["fl"] = "one,two,three";
            var conn = new MockConnection(q, MediaTypeNames.Text.Plain, string.Empty);

            var serializer = new MSolrQuerySerializer();
            serializer.serialize += _ => queryString;
            var qe = new SolrPostQueryExecuter<SolrQueryExecuterTests.TestDocument>(null, conn, serializer, null, parser);
            qe.Execute(new SolrMoreLikeThisHandlerQuery(new SolrQuery(queryString)), 
                new MoreLikeThisHandlerQueryOptions(
                    new MoreLikeThisHandlerParameters(new[] {"one", "three"})
                    {
                        MatchInclude = false,
                        MatchOffset = 5,
                        ShowTerms = InterestingTerms.None,
                    })
                {
                    Start = 0,
                    Rows = 5,
                    Fields = new[] {"one", "two", "three"},
                });
        }

        private class MSolrMoreLikeThisHandlerQueryResultsParser<T> : ISolrMoreLikeThisHandlerQueryResultsParser<T>
        {
            public SolrMoreLikeThisHandlerResults<T> Parse(string r)
            {
                return new SolrMoreLikeThisHandlerResults<T>();
            }
        }
    }

}
