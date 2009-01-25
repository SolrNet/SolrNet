using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands.Parameters;

namespace SolrNet.Tests {
    [TestFixture]
    public class FilterQueryTests {
        [Test]
        public void FilterQueries() {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.CreateMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.CreateMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer);
            solr.Query(SolrQuery.All, new QueryOptions {
                FilterQueries = new[] {new SolrQuery("id:0")},
            });
        }

        public class Document {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}