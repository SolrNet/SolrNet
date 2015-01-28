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
using MbUnit.Framework;
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

namespace SolrNet.Tests {
    [TestFixture]
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

        [Test]
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

        [Test]
        public void AddWithParameters() {
            var xml = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<add commitWithin=\"4343\" overwrite=\"false\"><doc /></add>", content);
                return xml;
            };
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse = headerParser.parse.Return(null);
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, docSerializer, null, headerParser, null, null, null);
            var parameters = new AddParameters { CommitWithin = 4343, Overwrite = false };
            ops.AddWithBoost(new[] { new KeyValuePair<TestDocumentWithoutUniqueKey, double?>(new TestDocumentWithoutUniqueKey(), null), }, parameters);
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void AddWithBoost() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<add><doc boost=\"2.1\" /></add>", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse = headerParser.parse.Return(null);
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, docSerializer, null, headerParser, null, null, null);
            ops.AddWithBoost(new[] { new KeyValuePair<TestDocumentWithoutUniqueKey, double?>(new TestDocumentWithoutUniqueKey(), 2.1), }, null);

            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void Extract() {
            var parameters = new ExtractParameters(null, "1", "test.doc");
            var connection = new MSolrConnection();
            connection.postStream += (url, contentType, content, param) => {
                Assert.AreEqual("/update/extract", url);
                Assert.AreEqual(parameters.Content, content);
                var expectedParams = new[] {
                    KV.Create("literal.id", parameters.Id),
                    KV.Create("resource.name", parameters.ResourceName),
                };
                Assert.AreElementsEqualIgnoringOrder(expectedParams, param);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithExtractContent.xml");
            };
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            var extractResponseParser = new MSolrExtractResponseParser {
                parse = _ => new ExtractResponse(null)
            };
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, docSerializer, null, null, null, null, extractResponseParser);
            ops.Extract(parameters);
            Assert.AreEqual(1, connection.postStream.Calls);
        }

        [Test]
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

        [Test]
        public void CommitWithOptions() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<commit />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            connection.post &= x => x.Expect(1);

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions());
            connection.post.Verify();
            
        }

        [Test]
        public void CommitWithOptions2_WaitSearcher_WaitFlush() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<commit waitSearcher=\"true\" waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse &= x => x.Stub();
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions { WaitSearcher = true, WaitFlush = true });
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void CommitWithOptions2_WaitSearcher() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<commit waitSearcher=\"false\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse &= x => x.Stub();
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions { WaitSearcher = false });
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void CommitWithOptions2_WaitFlush() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<commit waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse &= x => x.Stub();
            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Commit(new CommitOptions { WaitFlush = true });
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void DeleteByQuery() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<delete><query>id:123</query></delete>", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };
            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;
            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => "id:123";
            var ops = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, null, null, null, headerParser, querySerializer, null, null);
            ops.Delete(null, new SolrQuery("id:123"));
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void DeleteByMultipleId() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<delete><id>0</id><id>0</id></delete>", content);
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
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        [ExpectedException(typeof (SolrNetException))]
        public void DeleteDocumentWithoutUniqueKey_ShouldThrow() {
            var mapper = new MReadOnlyMappingManager();
            mapper.getUniqueKey += t => {
                Assert.AreEqual(typeof(TestDocumentWithoutUniqueKey), t);
                return null;
            };
            var ops = new SolrServer<TestDocumentWithoutUniqueKey>(null, mapper, null);
            ops.Delete(new TestDocumentWithoutUniqueKey());
            Assert.AreEqual(1, mapper.getUniqueKey.Calls);
        }

        [Test]
        public void DeleteDocumentWithUniqueKey() {
            var mapper = new MReadOnlyMappingManager();
            mapper.getUniqueKey += t => {
                Assert.AreEqual(typeof(TestDocumentWithUniqueKey), t);
                return new SolrFieldModel (
                    property : typeof (TestDocumentWithUniqueKey).GetProperty("id"),
                    fieldName : "id");
            };
            var basicServer = new MSolrBasicOperations<TestDocumentWithUniqueKey>();
            basicServer.delete &= x => x.Stub();
            var ops = new SolrServer<TestDocumentWithUniqueKey>(basicServer, mapper, null);
            ops.Delete(new TestDocumentWithUniqueKey());
            Assert.AreEqual(1, mapper.getUniqueKey.Calls);
        }

        [Test]
        public void Optimize() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<optimize />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Optimize(null);
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void OptimizeWithOptions() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<optimize waitSearcher=\"true\" waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Optimize(new CommitOptions { WaitFlush = true, WaitSearcher = true });
            Assert.AreEqual(1, connection.post.Calls);
        }

        [Test]
        public void OptimizeWithWaitOptions() {
            var connection = new MSolrConnection();
            connection.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<optimize waitSearcher=\"true\" waitFlush=\"true\" />", content);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var headerParser = new MSolrHeaderResponseParser();
            headerParser.parse += _ => null;

            var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, null, null, null, headerParser, null, null, null);
            ops.Optimize(new CommitOptions { WaitFlush = true, WaitSearcher = true });
        }

        [Test]
        public void MoreLikeThisHandlerQuery() {
            const string qstring = "id:123";

            var connection = new MSolrConnection();
            connection.get += (url, param) => {
                Assert.AreEqual("/mlt", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString() },
                    {"mlt", "true"},
                    {"mlt.fl", "id"},
                    {"mlt.match.include", "true"},
                };
                Assert.AreElementsEqualIgnoringOrder(expectedParams, param);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithInterestingTermsDetails.xml");
            };

            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize &= x => x.Return(qstring);

            var mlthParser = new MSolrMoreLikeThisHandlerQueryResultsParser<TestDocumentWithUniqueKey>();
            mlthParser.parse += _ => new SolrMoreLikeThisHandlerResults<TestDocumentWithUniqueKey>();

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(null, connection, querySerializer, null, mlthParser);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.MoreLikeThis(new SolrMoreLikeThisHandlerQuery(new SolrQuery(qstring)), new MoreLikeThisHandlerQueryOptions(new MoreLikeThisHandlerParameters(new[] { "id" }) { MatchInclude = true }));
            Assert.AreEqual(1, connection.get.Calls);
        }

        [Test]
        public void QueryWithPagination() {
            const string qstring = "id:123";
            const int start = 10;
            const int rows = 20;
            var connection = new MSolrConnection();
            connection.get += (url, param) => {
                Assert.AreEqual("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"start", start.ToString()},
                    {"rows", rows.ToString()},
                };
                Assert.AreElementsEqualIgnoringOrder(expectedParams, param);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => qstring;

            var resultParser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            resultParser.parse &= x => x.Stub();

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(resultParser, connection, querySerializer, null, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery(qstring), new QueryOptions { Start = start, Rows = rows });

            Assert.AreEqual(1, connection.get.Calls);
        }

        [Test]
        public void QueryWithSort() {
            const string qstring = "id:123";

            var connection = new MSolrConnection();
            connection.get += (url, param) => {
                Assert.AreEqual("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString()},
                    {"sort", "id asc,name desc"},
                };
                Assert.AreElementsEqualIgnoringOrder(expectedParams, param);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

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

            Assert.AreEqual(1, connection.get.Calls);
        }

        [Test]
        public void QueryWithSortAndPagination() {
            const string qstring = "id:123";
            const int start = 10;
            const int rows = 20;

            var connection = new MSolrConnection();
            connection.get += (url, param) => {
                Assert.AreEqual("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", qstring},
                    {"start", start.ToString()},
                    {"rows", rows.ToString()},
                    {"sort", "id asc,name desc"},
                };
                Assert.AreElementsEqualIgnoringOrder(expectedParams, param);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

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

            Assert.AreEqual(1, connection.get.Calls);
        }

        [Test]
        public void FacetQuery() {

            var connection = new MSolrConnection();
            connection.get += (url, param) => {
                Assert.AreEqual("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", ""},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString()},
                    {"facet", "true"},
                    {"facet.query", "id:1"},
                };
                Assert.AreElementsEqualIgnoringOrder(expectedParams, param);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var querySerializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());
            var parser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            parser.parse &= x => x.Stub();
            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(parser, connection, querySerializer, facetQuerySerializer, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                        new SolrFacetQuery(new SolrQuery("id:1")),
                    },
                }
            });

            Assert.AreEqual(1, connection.get.Calls);
        }

        [Test]
        public void FacetField() {
            var connection = new MSolrConnection();
            connection.get += (url, param) => {
                Assert.AreEqual("/select", url);
                var expectedParams = new Dictionary<string, string> {
                    {"q", ""},
                    {"rows", SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString()},
                    {"facet", "true"},
                    {"facet.field", "id"},
                    {"f.id.facet.limit", "3"},
                };
                Assert.AreElementsEqualIgnoringOrder(expectedParams, param);
                return EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            };

            var parser = new MSolrAbstractResponseParser<TestDocumentWithUniqueKey>();
            parser.parse &= x => x.Stub();
            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => "";
            var facetQuerySerializer = new DefaultFacetQuerySerializer(querySerializer, new DefaultFieldSerializer());

            var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(parser, connection, querySerializer, facetQuerySerializer, null);
            var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, null, null, null, null, null, null);
            var r = solr.Query(new SolrQuery(""), new QueryOptions {
                Facet = new FacetParameters {
                    Queries = new ISolrFacetQuery[] {
                            new SolrFacetFieldQuery("id") {Limit = 3},
                        },
                }
            });

            Assert.AreEqual(1, connection.get.Calls);
        }

        [Test]
        public void SearchResults_ShouldBeIterable() {
            var results = new SolrQueryResults<string>();
            Assert.IsInstanceOfType(typeof(IEnumerable<string>), results);
        }
    }
}