using System;
using System.Collections.Generic;
using MbUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrFacetDateQueryTests {
        [Test]
        public void tt() {
            var q = new SolrFacetDateQuery("timestamp", new DateTime(2009,1,1), new DateTime(2009,2,2), "+1DAY") {
                HardEnd = true,
                Other = new[] {FacetDateOther.After},
            };
            var r = q.Query;
            Assert.Contains(r, KV("facet.date", "timestamp"));
            Assert.Contains(r, KV("facet.timestamp.start", "2009-01-01T00:00:00Z"));
            Assert.Contains(r, KV("facet.timestamp.end", "2009-02-02T00:00:00Z"));
            Assert.Contains(r, KV("facet.timestamp.gap", "+1DAY"));
            Assert.Contains(r, KV("facet.timestamp.hardend", "true"));
            Assert.Contains(r, KV("facet.timestamp.other", "after"));
        }

        public KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}