using NUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class OperatorOverloadingTests {
        [Test]
        public void OneAnd() {
            var q = new SolrQuery("solr") && new SolrQuery("name:desc");
            Assert.AreEqual("(solr AND name:desc)", q.Query);
        }

        [Test]
        public void OneOr() {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc");
            Assert.AreEqual("(solr OR name:desc)", q.Query);
        }

        [Test]
        public void MultipleAnd() {
            var q = new SolrQuery("solr") && new SolrQuery("name:desc") && new SolrQueryByField("id", "123456");
            Assert.AreEqual("((solr AND name:desc) AND id:123456)", q.Query);
        }

        [Test]
        public void MultipleOr() {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc") || new SolrQueryByField("id", "123456");
            Assert.AreEqual("((solr OR name:desc) OR id:123456)", q.Query);
        }

        [Test]
        public void MixedAndOrs_obeys_operator_precedence() {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc") && new SolrQueryByField("id", "123456");
            Assert.AreEqual("(solr OR (name:desc AND id:123456))", q.Query);
        }

        [Test]
        public void MixedAndOrs_with_parentheses_obeys_precedence() {
            var q = (new SolrQuery("solr") || new SolrQuery("name:desc")) && new SolrQueryByField("id", "123456");
            Assert.AreEqual("((solr OR name:desc) AND id:123456)", q.Query);
        }

        [Test]
        public void Add() {
            var q = new SolrQuery("solr") + new SolrQuery("name:desc");
            Assert.AreEqual("(solr  name:desc)", q.Query);
        }

        [Test]
        public void Not() {
            var q = !new SolrQuery("solr");
            Assert.AreEqual("-solr", q.Query);
        }
    }
}