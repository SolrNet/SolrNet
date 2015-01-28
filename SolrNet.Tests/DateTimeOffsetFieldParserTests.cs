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
using SolrNet.Utils;

namespace SolrNet.Tests {
    public class DateTimeOffsetFieldParserTests {
        [StaticTestFactory]
        public static IEnumerable<Test> ParseYears() {
            return parsedDates.Select(pd => {
                var name = "ParseYears " + pd.Key;
                Test t = new TestCase(name, () => Assert.AreEqual(pd.Value, DateTimeOffsetFieldParser.Parse(pd.Key)));
                return t;
            });
        }

        private static readonly IEnumerable<KeyValuePair<string, DateTimeOffset>> parsedDates =
            new[] {
                KV.Create("1-01-01T00:00:00Z", new DateTimeOffset(new DateTime(1, 1, 1), TimeSpan.Zero)),
                KV.Create("2004-11-01T00:00:00Z", new DateTimeOffset(new DateTime(2004, 11, 1), TimeSpan.Zero)),
                KV.Create("2012-05-10T14:17:23.684Z", new DateTimeOffset(new DateTime(2012, 5, 10, 14, 17, 23, 684), TimeSpan.Zero)),
                KV.Create("2012-05-10T14:17:23.68Z", new DateTimeOffset(new DateTime(2012, 5, 10, 14, 17, 23, 680), TimeSpan.Zero)),
                KV.Create("2012-05-10T14:17:23.6Z", new DateTimeOffset(new DateTime(2012, 5, 10, 14, 17, 23, 600), TimeSpan.Zero)),
            };

        [StaticTestFactory]
        public static IEnumerable<Test> RoundTrip() {
            return dateTimes.Select(dt => {
                Test t = new TestCase("RoundTrip " + dt, () => {
                    var value = DateTimeOffsetFieldParser.Parse(DateTimeOffsetFieldSerializer.Serialize(dt));
                    Assert.AreEqual(dt, value);
                });
                return t;
            });
        }

        [StaticTestFactory]
        public static IEnumerable<Test> NullableRoundTrips() {
            var parser = new NullableFieldParser(new DateTimeOffsetFieldParser());
            var serializer = new NullableFieldSerializer(new DateTimeOffsetFieldSerializer());
            return dateTimes.Select(dt => {
                Test t = new TestCase("NullableRoundTrips " + dt, () => {
                    var s = serializer.Serialize(dt).First().FieldValue;
                    var xml = new XDocument();
                    xml.Add(new XElement("date", s));
                    var value = (DateTimeOffset?)parser.Parse(xml.Root, typeof(DateTimeOffset?));
                    Assert.AreEqual(dt, value);
                });
                return t;
            });
        }

        private static readonly IEnumerable<DateTimeOffset> dateTimes =
            new[] {
                new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc)),
                new DateTimeOffset(new DateTime(2004, 11, 1)),
                new DateTimeOffset(new DateTime(2004, 11, 1, 15, 41, 23)),
                new DateTimeOffset(new DateTime(2012, 5, 10, 14, 17, 23, 684)),
                new DateTimeOffset(new DateTime(2008, 5, 6, 14, 21, 23, 0, DateTimeKind.Local)),
            };
    }
}