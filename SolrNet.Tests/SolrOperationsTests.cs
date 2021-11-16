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
using Xunit;
using Moroco;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Mapping;
using SolrNet.Tests.Mocks;
using SolrNet.Tests.Utils;
using SolrNet.Utils;
using System.Linq;
using System.IO;
using System.Text;

namespace SolrNet.Tests {
    
    public class SolrOperationsTests {
        public class TestDocumentWithoutUniqueKey  {}

        public class TestDocumentWithUniqueKey  {
            [SolrUniqueKey]
            public int id {
                get { return 0; }
            }
        }

        public class TestDocWithNullable {
            [SolrField]
            public DateTime? Dt { get; set; }
        }

        [Fact]
        public void Add() {
            var xml = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            var connection = new MSolrConnection();
            connection.post = connection.post
                .Expect(1)
                .Args("/update", "<add><doc /></add>")
                .Return(xml);

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse = headerParser.parse.Return(null);

            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, docSerializer, null, headerParser, null, null, null);
            ops.AddWithBoost(new[] {
                new KeyValuePair<TestDocumentWithoutUniqueKey, double?>(new TestDocumentWithoutUniqueKey(), null),
            }, null);
            connection.post.Verify();
        }

        [Fact]
        public void AddWithParameters() {
            var xml = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<add commitWithin=\"4343\" overwrite=\"false\"><doc /></add>", content);
                return xml;
            };
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse = headerParser.parse.Return(null);
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, docSerializer, null, headerParser, null, null, null);
            var parameters = new AddParameters { CommitWithin = 4343, Overwrite = false };
            ops.AddWithBoost(new[] { new KeyValuePair<TestDocumentWithoutUniqueKey, double?>(new TestDocumentWithoutUniqueKey(), null), }, parameters);
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void AddWithBoost() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<add><doc boost=\"2.1\" /></add>", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse = headerParser.parse.Return(null);
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, docSerializer, null, headerParser, null, null, null);
            ops.AddWithBoost(new[] { new KeyValuePair<TestDocumentWithoutUniqueKey, double?>(new TestDocumentWithoutUniqueKey(), 2.1), }, null);

            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void Extract() {
            var parameters = new ExtractParameters(null, "1", "test.doc");
            var connection = new MSolrConnection();
            connection.postStream +=new MFunc<string, string, System.IO.Stream, IEnumerable<KeyValuePair<string, string>>, string> ( (url, contentType, content, param) => {
                Assert.Equal("/update/extract", url);
                Assert.Equal(parameters.Content, content);
                var expectedParams = new[] {
                    KV.Create("literal.id", parameters.Id),
                    KV.Create("resource.name", parameters.ResourceName),
                };
                Assert.Equal(expectedParams, param); //should ignore order
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithExtractContent.xml");
            });
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var extractResponseParser = new MSolrExtractResponseParser {
                parse = _ => new ExtractResponse(null)
            };
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, docSerializer, null, null, null, null, extractResponseParser);
            ops.Extract(parameters);
            Assert.Equal(1, connection.postStream.Calls);
        }

        [Fact]
        public void Commit() {

            var connection = new MSolrConnection();
            connection.post &= x => x.Args("/update", "<commit />")
                                     .Return(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml"))
                                     .Expect(1);

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse = headerParser.parse.Return(null);

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(null);
            connection.post.Verify();
        }

        [Fact]
        public void CommitWithOptions() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<commit />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            connection.post &= x => x.Expect(1);

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions());
            connection.post.Verify();
            
        }

        [Fact]
        public void CommitWithOptions2_WaitSearcher_WaitFlush() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<commit waitSearcher=\"true\" waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse &= x => x.Stub();
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions { WaitSearcher = true, WaitFlush = true });
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void CommitWithOptions2_WaitSearcher() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<commit waitSearcher=\"false\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse &= x => x.Stub();
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions { WaitSearcher = false });
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void CommitWithOptions2_WaitFlush() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<commit waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse &= x => x.Stub();
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions { WaitFlush = true });
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void DeleteByQuery() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<delete><query>id:123</query></delete>", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;
            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => "id:123";
            var ops = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, null, null, null, headerParser, querySerializer, null, null);
            ops.Delete(null, new SolrQuery("id:123"));
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void DeleteByMultipleId() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<delete><id>0</id><id>0</id></delete>", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;
            var basic = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            var ops = new SolrServer<TestDocumentWithUniqueKey>(basic, new AttributesMappingManager(), null);
            ops.Delete(new[] {
                new TestDocumentWithUniqueKey(),
                new TestDocumentWithUniqueKey(),
            });
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        //[ExpectedException(typeof (SolrNetException))]
        public void DeleteDocumentWithoutUniqueKey_ShouldThrow() {
            var mapper = new MReadOnlyMappingManager();
            mapper.getUniqueKey += t => {
                Assert.Equal(typeof(TestDocumentWithoutUniqueKey), t);
                return null;
            };
            var ops = new SolrServer<TestDocumentWithoutUniqueKey>(null, mapper, null);
          Assert.Throws<SolrNetException>( ()=> ops.Delete(new TestDocumentWithoutUniqueKey()));
            Assert.Equal(1, mapper.getUniqueKey.Calls);
        }

        [Fact]
        public void DeleteDocumentWithUniqueKey() {
            var mapper = new MReadOnlyMappingManager();
            mapper.getUniqueKey += t => {
                Assert.Equal(typeof(TestDocumentWithUniqueKey), t);
                return new SolrFieldModel (
                    property : typeof (TestDocumentWithUniqueKey).GetProperty("id"),
                    fieldName : "id");
            };
            var basicServer = new MSolrBasicOperations<TestDocumentWithUniqueKey>();
            basicServer.delete &= x => x.Stub();
            var ops = new SolrServer<TestDocumentWithUniqueKey>(basicServer, mapper, null);
            ops.Delete(new TestDocumentWithUniqueKey());
            Assert.Equal(1, mapper.getUniqueKey.Calls);
        }

        [Fact]
        public void Optimize() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<optimize />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Optimize(null);
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void OptimizeWithOptions() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<optimize waitSearcher=\"true\" waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Optimize(new CommitOptions { WaitFlush = true, WaitSearcher = true });
            Assert.Equal(1, connection.post.Calls);
        }

        [Fact]
        public void OptimizeWithWaitOptions() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<optimize waitSearcher=\"true\" waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Optimize(new CommitOptions { WaitFlush = true, WaitSearcher = true });
        }

        [Fact]
        public void MoreLikeThisHandlerQuery() {
            const string qstring = "id:123";

            var connection = new MSolrConnection();
            connection.get += new MFunc<string, IEnumerable<KeyValuePair<string, string>>, string>((url, param) => {
                Assert.Equal("/mlt", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString() },
                    {"mlt", "true"},
                    {"mlt.fl", "id"},
                    {"mlt.match.include", "true"},
                };
                Assert.Equal(expectedParams.OrderBy(p=>p.Key), param.OrderBy(p=>p.Key)); //should ignore order
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithInterestingTermsDetails.xml");
            });

            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize &= x => x.Return(qstring);

            var mlthParser = new MSolrMoreLikeThisHandlerQueryResultsParser<TestDocumentWithUniqueKey>();
            mlthParser.parse += _ => new SolrMoreLikeThisHandlerResults<TestDocumentWithUniqueKey>();

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(null, connection, querySerializer, null, mlthParser);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.MoreLikeThis(new SolrMoreLikeThisHandlerQuery(new SolrQuery(qstring)), new MoreLikeThisHandlerQueryOptions(new MoreLikeThisHandlerParameters(new[] { "id" }) { MatchInclude = true }));
            Assert.Equal(1, connection.get.Calls);
        }

        [Fact]
        public void QueryWithPagination() {
            const string qstring = "id:123";
            const int start = 10;
            const int rows = 20;
            var connection = new MSolrConnection();
            connection.get += new MFunc<string, IEnumerable<KeyValuePair<string, string>>, string>((url, param) => {
                Assert.Equal("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"start", start.ToString()},
                    {"rows", rows.ToString()},
                };
                Assert.Equal(expectedParams, param); //should ignore order
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            });

            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => qstring;

            var resultParser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            resultParser.parse &= x => x.Stub();

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(resultParser, connection, querySerializer, null, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery(qstring), new QueryOptions { Start = start, Rows = rows });

            Assert.Equal(1, connection.get.Calls);
        }

        [Fact]
        public void QueryWithSort() {
            const string qstring = "id:123";

            var connection = new MSolrConnection();
            connection.get += new MFunc<string, IEnumerable<KeyValuePair<string, string>>, string>((url, param) => {
                Assert.Equal("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString()},
                    {"sort", "id asc,name desc"},
                };
                Assert.Equal(expectedParams, param); //should ignore order
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            });

            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => qstring;

            var resultParser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            resultParser.parse &= x => x.Stub();

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(resultParser, connection, querySerializer, null, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery(qstring),
                               new QueryOptions {
                                   OrderBy = new[] {
                                        new SortOrder("id", Order.ASC),
                                        new SortOrder("name", Order.DESC)
                                    }
                               });

            Assert.Equal(1, connection.get.Calls);
        }

        [Fact]
        public void QueryWithSortAndPagination() {
            const string qstring = "id:123";
            const int start = 10;
            const int rows = 20;

            var connection = new MSolrConnection();
            connection.get += new MFunc<string, IEnumerable<KeyValuePair<string, string>>, string>((url, param) => {
                Assert.Equal("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"start", start.ToString()},
                    {"rows", rows.ToString()},
                    {"sort", "id asc,name desc"},
                };
                Assert.Equal(expectedParams, param); //should ignore order
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            });

            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => qstring;

            var resultParser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            resultParser.parse &= x => x.Stub();

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(resultParser, connection, querySerializer, null, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery(qstring), new QueryOptions {
                Start = start,
                Rows = rows,
                OrderBy = new[] {
                    new SortOrder("id", Order.ASC), 
                    new SortOrder("name", Order.DESC)
                }
            });

            Assert.Equal(1, connection.get.Calls);
        }

        [Fact]
        public void FacetQuery() {

            var connection = new MSolrConnection();
            connection.get += new MFunc<string, IEnumerable<KeyValuePair<string, string>>, string>( (url, param) => {
                Assert.Equal("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", "*:*"},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString()},
                    {"facet", "true"},
                    {"facet.query", "id:1"},
                };
                Assert.Equal (expectedParams, param); //should ignore order
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            });

            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());
            var parser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            parser.parse &= x => x.Stub();
            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(parser, connection, querySerializer, facetQuerySerializer, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery("*:*"), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetQuery(new SolrQuery("id:1")),
                    },
                }
            });

            Assert.Equal(1, connection.get.Calls);
        }

        [Fact]
        public void FacetField() {
            var connection = new MSolrConnection();
            connection.get += new MFunc<string, IEnumerable<KeyValuePair<string, string>>, string>((url, param) => {
                Assert.Equal("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", "*:*"},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString()},
                    {"facet", "true"},
                    {"facet.field", "id"},
                    {"f.id.facet.limit", "3"},
                };
                Assert.Equal(expectedParams, param); //should ignore order
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            });

            var parser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            parser.parse &= x => x.Stub();
            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => "*:*";
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(parser, connection, querySerializer, facetQuerySerializer, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery("*:*"), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                            new SolrFacetFieldQuery("id") {Limit = 3},
                        },
                }
            });

            Assert.Equal(1, connection.get.Calls);
        }

        [Fact]
        public void SearchResults_ShouldBeIterable() {
            var results = new SolrQueryResults<string>();
            Assert.IsAssignableFrom<IEnumerable<string>>(results);
        }

        [Fact]
        public void AtomicUpdate()
        {
            var xml = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            var connection = new MSolrConnection();
            connection.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("commitWithin", ((KeyValuePair<string, string>[])param)[0].Key);
                Assert.Equal("4343", ((KeyValuePair<string, string>[])param)[0].Value);
                Assert.Equal("[{\"id\":\"0\",\"animal\":{\"set\":\"squirrel\"},\"food\":{\"add\":[\"nuts\",\"seeds\"]},\"count\":{\"inc\":3},\"colour\":{\"remove\":\"pink\"},\"habitat\":{\"removeregex\":[\"tree.*\",\"river.+\"]}}]", text);
                return xml;
            });
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;
            var basic = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            var ops = new SolrServer<TestDocumentWithUniqueKey>(basic, new AttributesMappingManager(), null);
            var parameters = new AtomicUpdateParameters { CommitWithin = 4343 };
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("animal", AtomicUpdateType.Set, "squirrel"),
                new AtomicUpdateSpec("food", AtomicUpdateType.Add, new string[] {"nuts", "seeds" }), new AtomicUpdateSpec("count", AtomicUpdateType.Inc, 3),
                new AtomicUpdateSpec("colour", AtomicUpdateType.Remove, "pink"), new AtomicUpdateSpec("habitat", AtomicUpdateType.RemoveRegex, new string[]{ "tree.*", "river.+" }) };
            ops.AtomicUpdate(new TestDocumentWithUniqueKey(), updateSpecs, parameters);
            Assert.Equal(1, connection.postStream.Calls);
        }
    }
}
