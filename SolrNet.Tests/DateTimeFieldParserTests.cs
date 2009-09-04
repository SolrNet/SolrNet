using System;
using System.Collections.Generic;
using MbUnit.Framework;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Tests {
    [TestFixture]
    public class DateTimeFieldParserTests {
        [Test]
        [PairwiseJoin]
        [Factory("DataFactory")]
        public void ParseYears(string d, DateTime dt) {
            var p = new DateTimeFieldParser();
            Assert.AreEqual(dt, p.ParseDate(d));
        }

        public IEnumerable<object[]> DataFactory() {
            yield return new object[] {"1-01-01T00:00:00Z", new DateTime(1, 1, 1)};
            yield return new object[] { "2004-11-01T00:00:00Z", new DateTime(2004, 11, 1) };
        }
    }
}