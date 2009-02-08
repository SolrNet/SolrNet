using System;
using System.Linq;
using NUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrFacetFieldQueryTests {
        [Test]
        public void FieldOnly() {
            var fq = new SolrFacetFieldQuery("pepe");
            var q = fq.Query.ToList();
            Assert.AreEqual(1, q.Count);
            Console.WriteLine(q[0]);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
        }

        [Test]
        public void Prefix() {
            var fq = new SolrFacetFieldQuery("pepe") {Prefix = "pre"};
            var q = fq.Query.ToList();
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("facet.prefix", q[1].Key);
            Assert.AreEqual("pre", q[1].Value);
        }

        [Test]
        public void Sort() {
            var fq = new SolrFacetFieldQuery("pepe") {Sort = true};
            var q = fq.Query.ToList();
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("facet.sort", q[1].Key);
            Assert.AreEqual("true", q[1].Value);
        }

        [Test]
        public void Limit() {
            var fq = new SolrFacetFieldQuery("pepe") {Limit = 5};
            var q = fq.Query.ToList();
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("facet.limit", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }

        [Test]
        public void Offset() {
            var fq = new SolrFacetFieldQuery("pepe") {Offset = 5};
            var q = fq.Query.ToList();
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("facet.offset", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }

        [Test]
        public void Mincount() {
            var fq = new SolrFacetFieldQuery("pepe") {MinCount = 5};
            var q = fq.Query.ToList();
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("facet.mincount", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }

        [Test]
        public void Missing() {
            var fq = new SolrFacetFieldQuery("pepe") {Missing = true};
            var q = fq.Query.ToList();
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("facet.missing", q[1].Key);
            Assert.AreEqual("true", q[1].Value);
        }

        [Test]
        public void EnumCacheDF() {
            var fq = new SolrFacetFieldQuery("pepe") {EnumCacheMinDf = 5};
            var q = fq.Query.ToList();
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("facet.enum.cache.minDf", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }
    }
}