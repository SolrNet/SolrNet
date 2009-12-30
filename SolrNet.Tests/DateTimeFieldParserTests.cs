using System;
using System.Collections.Generic;
using MbUnit.Framework;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;

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
            yield return new object[] { "1-01-01T00:00:00Z", new DateTime(1, 1, 1) };
            yield return new object[] { "2004-11-01T00:00:00Z", new DateTime(2004, 11, 1) };
        }

        [Test]
        [Factory("DateTimes")]
        public void RoundTrip(DateTime dt) {
            var parser = new DateTimeFieldParser();
            var serializer = new DateTimeFieldSerializer();
            var s = serializer.SerializeDate(dt);
            Console.WriteLine(s);
            var value = parser.ParseDate(s);
            Console.WriteLine(value.ToString("r"));
            Assert.AreEqual(dt, value);
        }

        public IEnumerable<object> DateTimes() {
            yield return new DateTime(1, 1, 1);
            yield return new DateTime(2004, 11, 1);
            yield return new DateTime(2004, 11, 1, 15, 41, 23);
            yield return new DateTime(2008, 5, 6, 14, 21, 23, 0, DateTimeKind.Local);
        }

    }
}