using System;
using NUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrHasValueQueryTests {
        [Test]
        public void Query() {
            var q = new SolrHasValueQuery("name");
            Assert.AreEqual("name:[* TO *]", q.Query);
        }
    }
}