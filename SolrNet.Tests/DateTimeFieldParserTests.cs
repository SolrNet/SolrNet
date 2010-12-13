#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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

        [Test]
        [Factory("DateTimes")]
        public void NullableRoundTrips(DateTime? dt) {
            var parser = new NullableFieldParser(new DateTimeFieldParser());
            var serializer = new NullableFieldSerializer(new DateTimeFieldSerializer());
            var s = serializer.Serialize(dt).First().FieldValue;
            Console.WriteLine(s);
            var xml = new XDocument();
            xml.Add(new XElement("date", s));
            var value = (DateTime?) parser.Parse(xml.Root, typeof(DateTime?));
            Console.WriteLine(value.Value.ToString("r"));
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