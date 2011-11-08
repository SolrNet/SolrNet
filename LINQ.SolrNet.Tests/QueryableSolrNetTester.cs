using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SolrNet.Tests;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Commands.Parameters;
using Rhino.Mocks;
using LINQ.SolrNet.Helpers;
using SolrNet.Attributes;

namespace LINQ.SolrNet.Tests
{
    [TestClass]
    public class QueryableSolrNetTester
    {
        [TestMethod]
        public void FilterQueries()
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
                            where doc.Name == "aa" && doc.Id >= 10
                            orderby doc.Name descending,doc.Id
                            select doc;

            QueryOptions qo;
            var resDocs = ((IQueryableSolrNet<Document>)linqQuery).GetSolrQuery(out qo);
        }

        public class Document
        {
            [SolrField("id")]
            public int Id { get; set; }
            [SolrField("name")]
            public string Name { get; set; }
        }
    }
}
