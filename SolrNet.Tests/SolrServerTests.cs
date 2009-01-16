using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrServerTests {
        [Test]
        public void Ping() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocument>>();
            With.Mocks(mocks)
                .Expecting(basicServer.Ping)
                .Verify(() => {
                    var s = new SolrServer<TestDocument>(basicServer);
                    s.Ping();
                });
        }

        [Test]
        public void Commit() {
            var mocks = new MockRepository();
            var basicServer = mocks.CreateMock<ISolrBasicOperations<TestDocument>>();
            With.Mocks(mocks)
                .Expecting(() => basicServer.Commit(null))
                .Verify(() => {
                    var s = new SolrServer<TestDocument>(basicServer);
                    s.Commit();
                });            
        }

        public class TestDocument {
            [SolrUniqueKey]
            public int id {
                get { return 0; }
            }
        }
    }
}