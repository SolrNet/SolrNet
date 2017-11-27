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
using Xunit;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Utils;

namespace SolrNet.Tests
{
    public class DateTimeFieldParserTests
    {
        [Theory]
        [MemberData(nameof(parsedDates))]
        public static void ParseYears(KeyValuePair<string, DateTime> pd)
        {
            var name = "ParseYears " + pd.Key;
            var value = DateTimeFieldParser.ParseDate(pd.Key);
            Assert.Equal(pd.Value, value);
            Assert.Equal(pd.Value.Kind, value.Kind);
        }

        public static readonly IEnumerable<object[]> parsedDates =
            new[] {
              new object[] {  KV.Create("1-01-01T00:00:00Z", new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc)) },
                new object[] {KV.Create("2004-11-01T00:00:00Z", new DateTime(2004, 11, 1, 0, 0, 0, DateTimeKind.Utc)) },
                new object[] {KV.Create("2012-05-10T14:17:23.684Z", new DateTime(2012, 5, 10, 14, 17, 23, 684, DateTimeKind.Utc)) },
                new object[] {KV.Create("2012-05-10T14:17:23.68Z", new DateTime(2012, 5, 10, 14, 17, 23, 680, DateTimeKind.Utc)) },
                new object[] {KV.Create("2012-05-10T14:17:23.6Z", new DateTime(2012, 5, 10, 14, 17, 23, 600, DateTimeKind.Utc)) },
            };

        [Theory]
        [MemberData(nameof(dateTimes))]
        public static void RoundTrip(DateTime dt)
        {
            var value = DateTimeFieldParser.ParseDate(DateTimeFieldSerializer.SerializeDate(dt));
            Assert.Equal(dt, value);
            Assert.Equal(dt.Kind, value.Kind);
        }

        [Theory]
        [MemberData(nameof(dateTimes))]
        public static void NullableRoundTrips(DateTime dt)
        {
            var parser = new NullableFieldParser(new DateTimeFieldParser());
            var serializer = new NullableFieldSerializer(new DateTimeFieldSerializer());

            var s = serializer.Serialize(dt).First().FieldValue;
            var xml = new XDocument();
            xml.Add(new XElement("date", s));
            var value = (DateTime?)parser.Parse(xml.Root, typeof(DateTime?));
            Assert.Equal(dt, value);
            Assert.Equal(dt.Kind, value.Value.Kind);

        }

        public static readonly IEnumerable<object[]> dateTimes =
            new[] {
               new object[] { new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
               new object[] { new DateTime(2004, 11, 1, 0, 0, 0, DateTimeKind.Utc) },
                new object[] {new DateTime(2004, 11, 1, 15, 41, 23, DateTimeKind.Utc) },
                new object[] {new DateTime(2012, 5, 10, 14, 17, 23, 684, DateTimeKind.Utc) },
                new object[] {new DateTime(2008, 5, 6, 14, 21, 23, 0, DateTimeKind.Utc) },
            };
    }
}