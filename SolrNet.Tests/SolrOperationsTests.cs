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
using System.Reflection;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
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
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<add><doc /></add>"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Add(new[] {new TestDocumentWithoutUniqueKey()});
                });
        }

        [Test]
        public void AddWithBoost() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager(), new DefaultFieldSerializer());
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<add><doc boost=\"2.1\" /></add>"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.AddWithBoost(new[] {new KeyValuePair<TestDocumentWithoutUniqueKey, double?>(new TestDocumentWithoutUniqueKey(), 2.1),});
                });            
        }

        [Test]
        public void Commit() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<commit waitSearcher=\"true\" waitFlush=\"true\" />"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Commit(null);
                });
        }

        [Test]
        public void CommitWithOptions() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<commit waitSearcher=\"true\" waitFlush=\"true\" />"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Commit(new WaitOptions());
                });
        }

        [Test]
        public void CommitWithOptions2_WaitSearcher_WaitFlush() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<commit waitSearcher=\"true\" waitFlush=\"true\" />"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Commit(new WaitOptions {WaitSearcher = true, WaitFlush = true});
                });
        }

        [Test]
        public void CommitWithOptions2_WaitSearcher() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks).Expecting(() => Expect.Call(connection.Post("/update", "<commit waitSearcher=\"false\" waitFlush=\"true\" />"))
                                                  .Repeat.Once()
                                                  .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Commit(new WaitOptions {WaitSearcher = false});
                });
        }

        [Test]
        public void CommitWithOptions2_WaitFlush() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<commit waitSearcher=\"true\" waitFlush=\"true\" />"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Commit(new WaitOptions {WaitFlush = true});
                });
        }

        [Test]
        public void DeleteByQuery() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<delete><query>id:123</query></delete>"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(delegate {
                    var ops = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                    ops.Delete(null, new SolrQuery("id:123"));
                });
        }

        [Test]
        public void DeleteByMultipleId() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            var validationManager = mocks.CreateMock<IMappingValidationManager>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<delete><id>0</id><id>0</id></delete>"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(delegate {
                    var basic = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                    var ops = new SolrServer<TestDocumentWithUniqueKey>(basic, new AttributesMappingManager(), validationManager, null);
                    ops.Delete(new[] {
                        new TestDocumentWithUniqueKey(),
                        new TestDocumentWithUniqueKey(),
                    });
                });
        }

        [Test]
        [ExpectedException(typeof (NoUniqueKeyException))]
        public void DeleteDocumentWithoutUniqueKey_ShouldThrow() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocumentWithoutUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var validationManager = mocks.CreateMock<IMappingValidationManager>();
            With.Mocks(mocks)
                .Expecting(() => {
                    Expect.Call(mapper.GetUniqueKey(typeof (TestDocumentWithoutUniqueKey)))
                        .Throw(new NoUniqueKeyException(typeof(TestDocumentWithoutUniqueKey)));
                }).Verify(() => {
                    var ops = new SolrServer<TestDocumentWithoutUniqueKey>(basicServer, mapper, validationManager, null);
                    ops.Delete(new TestDocumentWithoutUniqueKey());
                });
        }

        [Test]
        public void DeleteDocumentWithUniqueKey() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var validationManager = mocks.CreateMock<IMappingValidationManager>();
            With.Mocks(mocks)
                .Expecting(() => {
                    Expect.Call(basicServer.Send(null))
                        .IgnoreArguments()
                        .Repeat.Once()
                        .Return("");
                    Expect.Call(mapper.GetUniqueKey(typeof (TestDocumentWithUniqueKey)))
                        .Return(new SolrFieldModel { Property = typeof(TestDocumentWithUniqueKey).GetProperty("id"), FieldName = "id" });
                })
                .Verify(delegate {
                    var ops = new SolrServer<TestDocumentWithUniqueKey>(basicServer, mapper, validationManager, null);
                    ops.Delete(new TestDocumentWithUniqueKey());
                });
        }

        [Test]
        public void Optimize() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<optimize waitSearcher=\"true\" waitFlush=\"true\" />"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Optimize(null);
                });
        }

        [Test]
        public void OptimizeWithOptions() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<optimize waitSearcher=\"true\" waitFlush=\"true\" />"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Optimize(new WaitOptions {WaitFlush = true, WaitSearcher = true});
                });
        }

        [Test]
        public void OptimizeWithWaitOptions() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithoutUniqueKey>>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(connection.Post("/update", "<optimize waitSearcher=\"true\" waitFlush=\"true\" />"))
                                     .Repeat.Once()
                                     .Return(null))
                .Verify(() => {
                    var ops = new SolrBasicServer<TestDocumentWithoutUniqueKey>(connection, executer, docSerializer);
                    ops.Optimize(new WaitOptions {WaitFlush = true, WaitSearcher = true});
                });
        }

        [Test]
        public void QueryWithPagination() {
            const string qstring = "id:123";
            const int start = 10;
            const int rows = 20;

            var mocks = new MockRepository();
            //var connection = mocks.CreateMock<ISolrConnection>();
            var query = new Dictionary<string, string>();
            query["q"] = qstring;
            query["start"] = start.ToString();
            query["rows"] = rows.ToString();
            var connection = new MockConnection(query);
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                Expect.On(parser).Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(qstring), new QueryOptions {Start = start, Rows = rows});
            });
        }

        [Test]
        public void QueryWithSort() {
            const string qstring = "id:123";

            var mocks = new MockRepository();
            var query = new Dictionary<string, string>();
            query["q"] = qstring;
            query["rows"] = SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString();
            query["sort"] = "id asc,name desc";
            var connection = new MockConnection(query);
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                Expect.On(parser).Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(qstring),
                                   new QueryOptions {
                                       OrderBy = new[] {
                                           new SortOrder("id", Order.ASC),
                                           new SortOrder("name", Order.DESC)
                                       }
                                   });
            });
        }

        [Test]
        public void QueryWithSortAndPagination() {
            const string qstring = "id:123";
            const int start = 10;
            const int rows = 20;

            var mocks = new MockRepository();
            var query = new Dictionary<string, string>();
            query["q"] = qstring;
            query["start"] = start.ToString();
            query["rows"] = rows.ToString();
            query["sort"] = "id asc,name desc";
            var connection = new MockConnection(query);
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                Expect.On(parser).Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(qstring), new QueryOptions {
                    Start = start,
                    Rows = rows,
                    OrderBy = new[] {
                        new SortOrder("id", Order.ASC), 
                        new SortOrder("name", Order.DESC)
                    }
                });
            });
        }

        [Test]
        public void FacetQuery() {
            var mocks = new MockRepository();
            var query = new Dictionary<string, string>();
            query["q"] = "";
            query["rows"] = SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString();
            query["facet"] = "true";
            query["facet.query"] = "id:1";
            var connection = new MockConnection(query);
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                Expect.On(parser).Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(""), new QueryOptions {
                    Facet = new FacetParameters {
                        Queries = new ISolrFacetQuery[] {
                            new SolrFacetQuery(new SolrQuery("id:1")),
                        },
                    }
                });
            });
        }

        [Test]
        public void FacetField() {
            var mocks = new MockRepository();
            var query = new Dictionary<string, string>();
            query["q"] = "";
            query["rows"] = SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString();
            query["facet"] = "true";
            query["facet.field"] = "id";
            query["f.id.facet.limit"] = "3";
            var connection = new MockConnection(query);
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                Expect.On(parser).Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(""), new QueryOptions {
                    Facet = new FacetParameters {
                        Queries = new ISolrFacetQuery[] {
                            new SolrFacetFieldQuery("id") {Limit = 3},
                        },
                    }
                });
            });
        }

        [Test]
        public void FacetFieldQuery() {
            var mocks = new MockRepository();
            var query = new Dictionary<string, string>();
            query["q"] = "*:*";
            query["facet"] = "true";
            query["facet.field"] = "cat";
            query["rows"] = "0";
            var connection = new MockConnection(query);
            var resultParser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var validationManager = mocks.CreateMock<IMappingValidationManager>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                Expect.Call(resultParser.Parse(""))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey> {
                        FacetFields = new Dictionary<string, ICollection<KeyValuePair<string, int>>> {
                            {
                                "cat", new List<KeyValuePair<string, int>> {
                                    new KeyValuePair<string, int>("electronics", 5),
                                    new KeyValuePair<string, int>("hard drive", 3),
                                }
                                }
                        }
                    });
            }).Verify(() => {
                var queryExecuter = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, resultParser);
                var basicSolr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, queryExecuter, docSerializer);
                var solr = new SolrServer<TestDocumentWithUniqueKey>(basicSolr, mapper, validationManager, null);
                var r = solr.FacetFieldQuery(new SolrFacetFieldQuery("cat"));
                Assert.AreEqual(2, r.Count);
                Assert.AreEqual("electronics", Func.First(r).Key);
            });
        }

        [Test]
        public void SearchResults_ShouldBeIterable() {
            var mocks = new MockRepository();
            var results = mocks.CreateMock<ISolrQueryResults<string>>();
            Assert.IsInstanceOfType(typeof(IEnumerable<string>), results);
        }

        public delegate string Writer(string ignored, string s);

        [Test]
        public void NullableDateTime() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var resultParser = mocks.CreateMock<ISolrQueryResultParser<TestDocWithNullable>>();
            var queryExecuter = new SolrQueryExecuter<TestDocWithNullable>(connection, resultParser);
            var mapper = new AttributesMappingManager();
            var docSerializer = new SolrDocumentSerializer<TestDocWithNullable>(mapper, new DefaultFieldSerializer());
            var validationManager = mocks.CreateMock<IMappingValidationManager>();
            var basicSolr = new SolrBasicServer<TestDocWithNullable>(connection, queryExecuter, docSerializer);
            var solr = new SolrServer<TestDocWithNullable>(basicSolr, mapper, validationManager, null);
            string xml = null;
            With.Mocks(mocks)
                .Expecting(() => {
                    Expect.Call(connection.Post(null, null))
                        .IgnoreArguments()
                        .Do(new Writer(delegate(string u, string s) {
                            Console.WriteLine(s);
                            xml = s;
                            return null;
                        }));
                })
                .Verify(() => {
                    solr.Add(new TestDocWithNullable());
                    Assert.AreEqual("<add><doc /></add>", xml);
                });

        }
    }
}