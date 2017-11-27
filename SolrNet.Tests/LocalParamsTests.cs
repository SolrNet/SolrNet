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

using System.Collections.Generic;
using System.Linq;
using Xunit;
using SolrNet.Exceptions;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Utils;

namespace SolrNet.Tests
{

    public class LocalParamsTests
    {
        public static IEnumerable<object[]> getTestParams()
        {
            yield return new object[] { KV.Create(new Dictionary<string, string> {
                    {"type", "spatial"},
                }, "{!type=spatial}") };
            yield return new object[] {KV.Create(new Dictionary<string, string> {
                    {"type", "spatial"},
                    {"a", "b"},
                }, "{!type=spatial a=b}")};
            yield return new object[]{ KV.Create(new Dictionary<string, string> {
                    {"type", "spatial"},
                    {"a", "1 2 3"},
                }, "{!type=spatial a='1 2 3'}")};
            yield return new object[] { KV.Create(new Dictionary<string, string> {
                    {"type", "spatial"},
                    {"a", "1 2 '3"},
                }, "{!type=spatial a='1 2 \\'3'}") };

        }
        [Theory]
        [MemberData(nameof(getTestParams))]
        public static void TestsParams(KeyValuePair<Dictionary<string, string>, string> dv)
        {

            var expectedResult = dv.Value;
            var localParams = dv.Key;
            Assert.Equal(expectedResult, new LocalParams(localParams).ToString());

        }

        public string SerializeQuery(object q)
        {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }

        [Fact]
        public void NullValueThrows()
        {
            var p = new LocalParams {
                {"a", null}
            };
            Assert.Throws<SolrNetException>(() => p.ToString());
        }

        [Fact]
        public void OperatorPlus()
        {
            var p = new LocalParams {
                {"type", "spatial"},
            };
            var q = new SolrQueryByField("field", "value");
            var qq = p + q;
            Assert.Equal("{!type=spatial}field:(value)", SerializeQuery(qq));
        }

        [Fact]
        public void OperatorPlus_multiple_queries()
        {
            var p = new LocalParams {
                {"type", "spatial"},
            };
            var q = new SolrQueryByField("field", "value");
            var q2 = new SolrQueryByRange<decimal>("price", 100m, 200m);
            var qq = p + (q + q2);
            Assert.Equal("{!type=spatial}(field:(value)  price:[100 TO 200])", SerializeQuery(qq));
        }
    }
}