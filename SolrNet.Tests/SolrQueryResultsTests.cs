using NUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrQueryResultsTests {
        [Test]
        public void FacetQueries_NotNullByDefault() {
            var r = new SolrQueryResults<Entity>();
            Assert.IsNotNull(r.FacetQueries);
        }

        [Test]
        public void FacetFields_NotNullByDefault() {
            var r = new SolrQueryResults<Entity>();
            Assert.IsNotNull(r.FacetFields);
        }

        public class Entity {}
    }
}