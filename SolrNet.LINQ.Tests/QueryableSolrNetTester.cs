using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using SolrNet.Mapping;
using SolrNet.Tests;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Commands.Parameters;
using Rhino.Mocks;
using SolrNet.LINQ;
using SolrNet.Attributes;
using MbUnit.Framework;

namespace SolrNet.LINQ.Tests
{
    [TestFixture]
    public class QueryableSolrNetTester
    {
        [FixtureSetUp]
        public void FixtureSetup() {
            SolrNetLinqExt.Init(new AttributesMappingManager());
        }

        [Test]
        public void WhereEqual()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);

            var linqQuery = from doc in solr.AsQueryable()
                            where doc.Name == "aa"
                            
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("(name:aa)", resDocs.Query);
        }


        [Test]
        public void GreaterEquals_SmallerEqual()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);

            var linqQuery = from doc in solr.AsQueryable()
                            where doc.Price >= 1 && doc.Price <= 10

                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("((price:[1 TO *]) AND (price:[* TO  10]))", resDocs.Query);
        }

        [Test]
        public void NotOperator()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);

            var linqQuery = from doc in solr.AsQueryable()
                            where !(doc.Name == "bb")
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("-(name:bb)", resDocs.Query);
        }


        [Test]
        public void DefaultField()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);

            var linqQuery = from doc in solr.AsQueryable()
                            where doc.DefaultFieldEquals("aa")
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("aa", resDocs.Query);
        }

        [Test]
        public void AndsOrs()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);

            var linqQuery = from doc in solr.AsQueryable()
                            where doc.DefaultFieldEquals("aa") && (doc.Price >= 1 || doc.Price <= 11)
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("(aa AND ((price:[1 TO *]) OR (price:[* TO  11])))", resDocs.Query);
        }


        [Test]
        public void PartialEval()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);
            int priceVal = 1;
            DateTime dt = new DateTime(2011, 1, 1);
            var linqQuery = from doc in solr.AsQueryable()
                            where (doc.Price >= priceVal || doc.Timestamp >= dt)
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("((price:[1 TO *]) OR (timestamp:[2011-01-01T12:00:00.000Z TO *]))", resDocs.Query);
        }


        [Test]
        public void Collection()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);
         
          
            var linqQuery = from doc in solr.AsQueryable()
                            where doc.Categories.AnyItem() == "cat1"
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("(cat:cat1)", resDocs.Query);
        }

        [Test]
        public void DynamicField()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);


            var linqQuery = from doc in solr.AsQueryable()
                            where doc.DynamicField("field_1") == "val"
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("(field_1:val)", resDocs.Query);
        }


        [Test]
        public void Boosting()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);


            var linqQuery = from doc in solr.AsQueryable()
                            where (doc.Name == "john").Boost(10)
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual("(name:john)^10", resDocs.Query);
        }



        [Test]
        public void Sort()
        {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);
            int priceVal = 1;
            DateTime dt = new DateTime(2011, 1, 1);
            var linqQuery = from doc in solr.AsQueryable()
                            where (doc.Price >= priceVal || doc.Timestamp >= dt)
                            orderby doc.Id, doc.Price descending
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);

            Assert.AreEqual(qo.OrderBy.Count, 2);
            var so = new SortOrder("id", Order.ASC);
            Assert.IsTrue(qo.OrderBy.Contains(so));
            so = new SortOrder("price", Order.DESC);
            Assert.IsTrue(qo.OrderBy.Contains(so));
        }

        public class Document
        {
            [SolrField("id")]
            public int Id { get; set; }
            [SolrField("name")]
            public string Name { get; set; }

            [SolrField("price")]
            public int Price { get; set; }

            [SolrField("cat")]
            public ICollection<string> Categories { get; set; }

            [SolrField("timestamp")]
            public DateTime Timestamp { get; set; }

            [SolrField("*")]
            public IDictionary<string, object> OtherFields { get; set; }
        }
    }
}
