using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrOperationsTests {
        public class TestDocumentWithoutUniqueKey : ISolrDocument {}

        public class TestDocumentWithUniqueKey : ISolrDocument {
            [SolrUniqueKey]
            public int id {
                get { return 0; }
            }
        }

        [Test]
        public void Add() {
            var mocks = new MockRepository();
            var connection = mocks.CreateMock<ISolrConnection>();
            var executer = mocks.CreateMock<ISolrQueryExecuter<TestDocumentWithoutUniqueKey>>();
            var docSerializer = new SolrDocumentSerializer<TestDocumentWithoutUniqueKey>(new AttributesMappingManager());
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
                    ops.Delete(new SolrQuery("id:123"));
                });
        }

        [Test]
        [Ignore("obsolete")]
        public void DeleteByQueryWithParams() {
            //var mocks = new MockRepository();
            ////ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
            //var connection = mocks.CreateMock<ISolrConnection>();
            //With.Mocks(mocks).Expecting(delegate {
            //    Expect.Call(connection.Post("/update", "<delete fromPending=\"true\" fromCommitted=\"true\"><query>id:123</query></delete>"))
            //        .Repeat.Once()
            //        .Return(null);
            //}).Verify(delegate {
            //    ISolrOperations<TestDocumentWithUniqueKey> ops = new SolrServer<TestDocumentWithUniqueKey>(connection);
            //    ops.Delete(new SolrQuery("id:123"), true, true);
            //});
        }

        [Test]
        [ExpectedException(typeof (NoUniqueKeyException))]
        public void DeleteDocumentWithoutUniqueKey_ShouldThrow() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocumentWithoutUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            With.Mocks(mocks)
                .Expecting(() => {
                    Expect.Call(mapper.GetUniqueKey(typeof (TestDocumentWithoutUniqueKey)))
                        .Throw(new NoUniqueKeyException(typeof(TestDocumentWithoutUniqueKey)));
                }).Verify(() => {
                    var ops = new SolrServer<TestDocumentWithoutUniqueKey>(basicServer, mapper);
                    ops.Delete(new TestDocumentWithoutUniqueKey());
                });
        }

        [Test]
        public void DeleteDocumentWithUniqueKey() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            With.Mocks(mocks)
                .Expecting(() => {
                    Expect.Call(basicServer.Send(null))
                        .IgnoreArguments()
                        .Repeat.Once()
                        .Return("");
                    Expect.Call(mapper.GetUniqueKey(typeof (TestDocumentWithUniqueKey)))
                        .Return(new KeyValuePair<PropertyInfo, string>(typeof (TestDocumentWithUniqueKey).GetProperty("id"), "id"));
                })
                .Verify(delegate {
                    var ops = new SolrServer<TestDocumentWithUniqueKey>(basicServer, mapper);
                    ops.Delete(new TestDocumentWithUniqueKey());
                });
        }

        [Test]
        [Ignore("obsolete")]
        public void DeleteDocumentWithUniqueKeyWithParams() {
            //var mocks = new MockRepository();
            ////ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
            //var connection = mocks.CreateMock<ISolrConnection>();
            //With.Mocks(mocks).Expecting(delegate {
            //    Expect.Call(connection.Post("/update", "<delete fromPending=\"true\" fromCommitted=\"false\"><id>0</id></delete>")).Repeat.Once().Return(null);
            //}).Verify(delegate {
            //    var ops = new SolrBasicServer<TestDocumentWithUniqueKey>(connection);
            //    ops.Delete(new TestDocumentWithUniqueKey(), true, false);
            //});
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
            var container = mocks.CreateMock<IServiceProvider>();
            Factory.Init(container);
            var connection = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                var query = new Dictionary<string, string>();
                query["q"] = qstring;
                query["start"] = start.ToString();
                query["rows"] = rows.ToString();
                Expect.Call(connection.Get("/select", query))
                    .Repeat.Once()
                    .Return("");
                Expect.Call(container.GetService(typeof (IListRandomizer)))
                    .Repeat.Any()
                    .Return(null);

                SetupResult.For(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser, mapper);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(qstring), new QueryOptions {Start = start, Rows = rows});
            });
        }

        [Test]
        public void QueryWithSort() {
            const string qstring = "id:123";

            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceProvider>();
            Factory.Init(container);
            var connection = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                IDictionary<string, string> query = new Dictionary<string, string>();
                query["q"] = qstring;
                query["rows"] = SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString();
                query["sort"] = "id asc,name desc";
                Expect.Call(connection.Get("/select", query))
                    .Repeat.Once()
                    .Return("");
                Expect.Call(container.GetService(typeof(IListRandomizer)))
                    .Repeat.Any()
                    .Return(null);
                SetupResult.For(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser, mapper);
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
            var container = mocks.CreateMock<IServiceProvider>();
            Factory.Init(container);
            var connection = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                var query = new Dictionary<string, string>();
                query["q"] = qstring;
                query["start"] = start.ToString();
                query["rows"] = rows.ToString();
                query["sort"] = "id asc,name desc";
                Expect.Call(connection.Get("/select", query)).Repeat.Once().Return("");
                Expect.Call(container.GetService(typeof(IListRandomizer)))
                    .Repeat.Any()
                    .Return(null);

                SetupResult.For(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser, mapper);
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
            var container = mocks.CreateMock<IServiceProvider>();
            Factory.Init(container);
            var connection = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                var query = new Dictionary<string, string>();
                query["q"] = "";
                query["rows"] = SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString();
                query["facet"] = "true";
                query["facet.query"] = "id:1";
                Expect.Call(connection.Get("/select", query))
                    .Repeat.Once()
                    .Return("");
                Expect.Call(container.GetService(typeof(IListRandomizer)))
                    .Repeat.Any()
                    .Return(null);
                SetupResult.For(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser, mapper);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(""), new QueryOptions {
                    FacetQueries = new ISolrFacetQuery[] {
                        new SolrFacetQuery(new SolrQuery("id:1")),
                    },
                });
            });
        }

        [Test]
        public void FacetField() {
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceProvider>();
            Factory.Init(container);
            var connection = mocks.CreateMock<ISolrConnection>();
            var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                var query = new Dictionary<string, string>();
                query["q"] = "";
                query["rows"] = SolrQueryExecuter<TestDocumentWithUniqueKey>.ConstDefaultRows.ToString();
                query["facet"] = "true";
                query["facet.field"] = "id";
                query["facet.limit"] = "3";
                Expect.Call(connection.Get("/select", query))
                    .Repeat.Once()
                    .Return("");
                Expect.Call(container.GetService(typeof(IListRandomizer)))
                    .Repeat.Any()
                    .Return(null);
                SetupResult.For(parser.Parse(null))
                    .IgnoreArguments()
                    .Return(new SolrQueryResults<TestDocumentWithUniqueKey>());
            }).Verify(() => {
                var executer = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, parser, mapper);
                var solr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, executer, docSerializer);
                var r = solr.Query(new SolrQuery(""), new QueryOptions {
                    FacetQueries = new ISolrFacetQuery[] {
                        new SolrFacetFieldQuery("id") {Limit = 3},
                    },
                });
            });
        }

        [Test]
        public void FacetFieldQuery() {
            var mocks = new MockRepository();
            var container = mocks.CreateMock<IServiceProvider>();
            Factory.Init(container);
            var query = new Dictionary<string, string>();
            query["q"] = "*:*";
            query["facet"] = "true";
            query["facet.field"] = "cat";
            query["rows"] = "0";
            var connection = new MockConnection(query);
            var resultParser = mocks.CreateMock<ISolrQueryResultParser<TestDocumentWithUniqueKey>>();
            var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<TestDocumentWithUniqueKey>>();
            With.Mocks(mocks).Expecting(() => {
                Expect.Call(container.GetService(typeof(IListRandomizer)))
                    .Repeat.Any()
                    .Return(null);
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
                var queryExecuter = new SolrQueryExecuter<TestDocumentWithUniqueKey>(connection, resultParser, mapper);
                var basicSolr = new SolrBasicServer<TestDocumentWithUniqueKey>(connection, queryExecuter, docSerializer);
                var solr = new SolrServer<TestDocumentWithUniqueKey>(basicSolr, mapper);
                var r = solr.FacetFieldQuery(new SolrFacetFieldQuery("cat"));
                Assert.AreEqual(2, r.Count);
                Assert.AreEqual("electronics", Func.First(r).Key);
            });
        }

        [Test]
        public void SearchResults_ShouldBeIterable() {
            var mocks = new MockRepository();
            var results = mocks.CreateMock<ISolrQueryResults<ISolrDocument>>();
            Assert.IsInstanceOfType(typeof (IEnumerable<ISolrDocument>), results);
        }
    }
}