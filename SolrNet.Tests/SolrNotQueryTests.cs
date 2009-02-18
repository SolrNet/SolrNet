using System;
using NUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrNotQueryTests {
        [Test]
        public void SimpleQuery() {
            var q = new SolrQuery("desc:samsung");
            var notq = new SolrNotQuery(q);
            Assert.AreEqual("-desc:samsung", notq.Query);
        }

        [Test]
        public void QueryByField() {
            var q = new SolrQueryByField("desc", "samsung");
            var notq = new SolrNotQuery(q);
            Assert.AreEqual("-desc:samsung", notq.Query);
        }

        [Test]
        public void RangeQuery() {
            var q = new SolrQueryByRange<decimal>("price", 100, 200);
            var notq = new SolrNotQuery(q);
            Assert.AreEqual("-price:[100 TO 200]", notq.Query);
        }

        [Test]
        public void QueryInList() {
            var q = new SolrQueryInList("desc", "samsung", "hitachi", "fujitsu");
            var notq = new SolrNotQuery(q);
            Assert.AreEqual("-(desc:samsung OR desc:hitachi OR desc:fujitsu)", notq.Query);
        }

        [Test]
        public void MultipleCriteria() {
            var q = SolrMultipleCriteriaQuery.Create(new SolrQueryByField("desc", "samsung"), new SolrQueryByRange<decimal>("price", 100, 200));
            var notq = new SolrNotQuery(q);
            Assert.AreEqual("-(desc:samsung  price:[100 TO 200])", notq.Query);
        }
    }
}