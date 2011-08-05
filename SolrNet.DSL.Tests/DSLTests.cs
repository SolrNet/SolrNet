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
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Tests;

namespace SolrNet.DSL.Tests {
    /// <summary>
    /// These tests are more to define DSL syntax than anything else.
    /// </summary>
    [TestFixture]
    public class DSLTests {
        public class TestDocument  {}

        public class TestDocumentWithId  {
            [SolrField]
            public int Id { get; set; }
        }

        private const string response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
";

        public delegate string Writer(string s, IDictionary<string, string> q);

        [FixtureSetUp]
        public void FixtureSetup() {
            Startup.Init<TestDocument>("http://localhost");
            Startup.Init<TestDocumentWithId>("http://localhost");
        }

        [Test]
        public void Add() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Post("/update", "<add><doc /></add>")).Return(""))
                .Verify(() => {
                    Solr.Connection = conn;
                    Solr.Add(new TestDocument());
                });
        }

        [Test]
        public void Commit() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            Solr.Connection = conn;
            Solr.Commit();
        }

        [Test]
        public void CommitWithParams() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            Solr.Connection = conn;
            Solr.Commit(true, true);
        }

        [Test]
        public void DeleteById() {
            const string id = "123456";
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Post("/update", string.Format("<delete><id>{0}</id></delete>", id)))
                                     .Return(""))
                .Verify(() => {
                    Solr.Connection = conn;
                    Solr.Delete.ById(id);
                });
        }

        [Test]
        public void DeleteByIds() {
            var ids = new[] {"123", "456"};
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Post("/update", string.Format("<delete><id>{0}</id><id>{1}</id></delete>", ids[0], ids[1])))
                                     .Return(""))
                .Verify(() => {
                    Solr.Connection = conn;
                    Solr.Delete.ByIds(ids);
                });
        }

        [Test]
        public void DeleteByQuery() {
            const string q = "id:123456";
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks).Expecting(() => {
                if (conn != null)
                    Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q))).Return("");
            }).Verify(() => {
                Solr.Connection = conn;
                Solr.Delete.ByQuery(new SolrQuery(q));
            });
        }

        [Test]
        public void DeleteByQueryString() {
            const string q = "id:123456";
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", q)))
                                     .Return(""))
                .Verify(() => {
                    Solr.Connection = conn;
                    Solr.Delete.ByQuery(q);
                });
        }

        [Test]
        public void Optimize() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            Solr.Connection = conn;
            Solr.Optimize();
        }

        [Test]
        public void OptimizeWithParams() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            Solr.Connection = conn;
            Solr.Optimize(true, true);
        }

        public string DefaultRows() {
            return SolrQueryExecuter<TestDocumentWithId>.ConstDefaultRows.ToString();
        }

        [Test]
        public void OrderBy() {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"q", "((Id:123456))"},
                {"sort", "id asc"},
                {"rows", DefaultRows()},
            });
            Solr.Connection = conn;
            Solr.Query<TestDocumentWithId>()
                .By("Id").Is("123456")
                .OrderBy("id")
                .Run();
        }

        [Test]
        public void OrderBy2() {
            const string queryString = "id:123";
            var query = new Dictionary<string, string> {
                {"q", queryString},
                {"rows", DefaultRows()},
                {"sort", "id asc"},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>(new SolrQuery(queryString), new SortOrder("id", Order.ASC));
        }

        [Test]
        public void OrderBy2Multiple() {
            const string queryString = "id:123";
            var query = new Dictionary<string, string> {
                {"q", queryString},
                {"rows", DefaultRows()},
                {"sort", "id asc,name desc"},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>(new SolrQuery(queryString), new[] {
                new SortOrder("id", Order.ASC),
                new SortOrder("name", Order.DESC)
            });
        }

        [Test]
        public void OrderByAscDesc() {
            var query = new Dictionary<string, string> {
                {"q", "((Id:123456))"},
                {"rows", DefaultRows()},
                {"sort", "id asc"},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            var doc = new TestDocumentWithId {Id = 123456};
            Solr.Query<TestDocumentWithId>()
                .By("Id").Is("123456")
                .OrderBy("id", Order.ASC)
                .Run();
        }

        [Test]
        public void OrderByAscDescMultiple() {
            var query = new Dictionary<string, string> {
                {"q", "((Id:123456))"},
                {"rows", DefaultRows()},
                {"sort", "id asc,name desc"},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            var doc = new TestDocumentWithId {Id = 123456};
            Solr.Query<TestDocumentWithId>()
                .By("Id").Is("123456")
                .OrderBy("id", Order.ASC)
                .OrderBy("name", Order.DESC)
                .Run();
        }

        [Test]
        public void Query() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Get(null, null))
                                     .IgnoreArguments()
                                     .Repeat.Once()
                                     .Return(response))
                .Verify(() => {
                    Solr.Connection = conn;
                    var r = Solr.Query<TestDocument>("");
                    Assert.AreEqual(1, r.NumFound);
                });
        }

        [Test]
        public void Query_InvalidField_ShouldNOTThrow() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(
                                     @"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc><str name=""advancedview""/><str name=""basicview""/><int name=""id"">123456</int></doc></result>
</response>
"))
                .Verify(() => {
                    Solr.Connection = conn;
                    Solr.Query<TestDocument>("");
                });
        }

        [Test]
        public void QueryByAnyField() {
            var query = new Dictionary<string, string> {
                {"q", "((id:123456))"},
                {"rows", DefaultRows()},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>().By("id").Is("123456").Run();
        }

        [Test]
        public void QueryByRange() {
            var query = new Dictionary<string, string> {
                {"q", "(id:[123 TO 456])"},
                {"rows", DefaultRows()},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>().ByRange("id", 123, 456).Run();
        }

        [Test]
        public void QueryByRange_AnotherSyntax() {
            var query = new Dictionary<string, string> {
                {"q", "(id:[123 TO 456])"},
                {"rows", DefaultRows()},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>().By("id").Between(123).And(456).Run();
        }

        [Test]
        public void QueryByRangeConcatenable() {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"q", "((id:[123 TO 456])  p:[a TO z])"},
                {"rows", DefaultRows()},
            });
            Solr.Connection = conn;
            Solr.Query<TestDocument>().ByRange("id", 123, 456).ByRange("p", "a", "z").Run();
        }

        [Test]
        public void QueryByRangeExclusive() {
            var query = new Dictionary<string, string> {
                {"q", "(id:{123 TO 456})"},
                {"rows", DefaultRows()},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>().ByRange("id", 123, 456).Exclusive().Run();
        }

        [Test]
        public void QueryByRangeInclusive() {
            var query = new Dictionary<string, string> {
                {"q", "(id:[123 TO 456])"},
                {"rows", DefaultRows()},
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>().ByRange("id", 123, 456).Inclusive().Run();
        }

        [Test]
        public void QueryISolrQuery() {
            const string queryString = "id:123";
            var conn = new MockConnection(new Dictionary<string, string> {
                {"q", queryString},
                //{"rows", DefaultRows()},
            });
            Solr.Connection = conn;
            Solr.Query<TestDocument>(new SolrQuery(queryString));
        }

        [Test]
        public void QueryISolrQueryWithPagination() {
            const string queryString = "id:123";
            var query = new Dictionary<string, string> {
                {"q", queryString},
                {"start", 10.ToString()},
                {"rows", 20.ToString()}
            };
            var conn = new MockConnection(query);
            Solr.Connection = conn;
            Solr.Query<TestDocument>(new SolrQuery(queryString), 10, 20);
        }

        [Test]
        public void QueryWithPagination() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Get(null, null))
                                     .IgnoreArguments()
                                     .Repeat.Once()
                                     .Return(response))
                .Verify(() => {
                    Solr.Connection = conn;
                    var r = Solr.Query<TestDocument>("", 10, 20);
                    Assert.AreEqual(1, r.NumFound);
                });
        }

        [Test]
        public void FacetField() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Get(null, null))
                                     .IgnoreArguments()
                                     .Repeat.Once()
                                     .Return(response))
                .Verify(() => {
                    Solr.Connection = conn;
                    var r = Solr.Query<TestDocument>().By("makedesc").Is("bmw").WithFacetField("modeldesc").Run();
                });
        }

        [Test]
        public void FacetField_options() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Get(null, null))
                                     .IgnoreArguments()
                                     .Repeat.Once()
                                     .Return(response))
                .Verify(() => {
                    Solr.Connection = conn;
                    var r = Solr.Query<TestDocument>().By("makedesc").Is("bmw")
                        .WithFacetField("modeldesc")
                        .LimitTo(100)
                        .DontSortByCount()
                        .WithPrefix("xx")
                        .WithMinCount(10)
                        .StartingAt(20)
                        .IncludeMissing()
                        .Run();
                });
        }

        [Test]
        public void FacetQuery_string() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Get(null, null))
                                     .IgnoreArguments()
                                     .Repeat.Once()
                                     .Return(response))
                .Verify(() => {
                    Solr.Connection = conn;
                    var r = Solr.Query<TestDocument>().By("makedesc").Is("bmw").WithFacetQuery("").Run();
                });
        }

        [Test]
        public void FacetQuery_ISolrQuery() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks)
                .Expecting(() => Expect.Call(conn.Get(null, null))
                                     .IgnoreArguments()
                                     .Repeat.Once()
                                     .Return(response))
                .Verify(() => {
                    Solr.Connection = conn;
                    var r = Solr.Query<TestDocument>().By("makedesc").Is("bmw").WithFacetQuery(new SolrQuery("")).Run();
                });
        }

        [Test]
        [Ignore("Not implemented")]
        public void FacetQuery_Fluent() {
            throw new NotImplementedException();
            //var mocks = new MockRepository();
            //var conn = mocks.StrictMock<ISolrConnection>();
            //With.Mocks(mocks).Expecting(delegate {
            //  Expect.Call(conn.Get(null, null)).IgnoreArguments().Repeat.Once().Return(response);
            //}).Verify(delegate {
            //  Solr.Connection = conn;
            //  var r = Solr.Query<TestDocument>().By("makedesc").Is("bmw")
            // .WithFacetQuery().By("cat").Is("something")
            // .Run();
            //});
        }

        [Test]
        public void Highlighting() {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"q", "((makedesc:bmw))"},
                {"hl", "true"},
                {"hl.fl", "make"},
                {"rows", DefaultRows()},
            });
            Solr.Connection = conn;
            var r = Solr.Query<TestDocument>().By("makedesc").Is("bmw").WithHighlighting(new HighlightingParameters {
                Fields = new[] {"make"},
            }).Run();
        }

        [Test]
        public void HighlightingFields() {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"q", "((makedesc:bmw))"},
                {"hl", "true"},
                {"hl.fl", "make,category"},
                {"rows", DefaultRows()},
            });
            Solr.Connection = conn;
            var r = Solr.Query<TestDocument>().By("makedesc").Is("bmw").WithHighlightingFields("make", "category").Run();
        }
    }
}