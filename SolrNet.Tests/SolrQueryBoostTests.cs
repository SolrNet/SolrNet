using System;
using MbUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrQueryBoostTests {
        [Test]
        public void Boost() {
            var q = new SolrQueryBoost(new SolrQuery("solr"), 34.2);
            Assert.AreEqual("(solr)^34.2", q.Query);
        }

        [Test]
        public void Boost_with_culture() {
            using (ThreadSettings.Culture("fr-FR")) {
                var q = new SolrQueryBoost(new SolrQuery("solr"), 34.2);
                Assert.AreEqual("(solr)^34.2", q.Query);                
            }
        }

        [Test]
        public void Boost_with_high_value() {
            var q = new SolrQueryBoost(new SolrQuery("solr"), 34.2E10);
            Assert.AreEqual("(solr)^342000000000", q.Query);
        }

        [Test]
        public void SolrQuery_Boost() {
            var q = new SolrQuery("solr").Boost(12.2);
            Assert.AreEqual("(solr)^12.2", q.Query);
        }
    }
}