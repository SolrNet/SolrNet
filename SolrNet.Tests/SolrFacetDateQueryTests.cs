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
using MbUnit.Framework;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrFacetDateQueryTests {
        [Test]
        public void Serialize() {
            var q = new SolrFacetDateQuery("timestamp", new DateTime(2009,1,1), new DateTime(2009,2,2), "+1DAY") {
                HardEnd = true,
                Other = new[] {FacetDateOther.After},
            };
            var r = Serialize(q);
            Assert.Contains(r, KV("facet.date", "timestamp"));
            Assert.Contains(r, KV("f.timestamp.facet.date.start", "2009-01-01T00:00:00Z"));
            Assert.Contains(r, KV("f.timestamp.facet.date.end", "2009-02-02T00:00:00Z"));
            Assert.Contains(r, KV("f.timestamp.facet.date.gap", "+1DAY"));
            Assert.Contains(r, KV("f.timestamp.facet.date.hardend", "true"));
            Assert.Contains(r, KV("f.timestamp.facet.date.other", "after"));
        }
        
        [Test]
        public void IgnoresLocalParams() {
            var q = new SolrFacetDateQuery(new LocalParams { { "ex", "cat" } } + "timestamp", new DateTime(2009, 1, 1), new DateTime(2009, 2, 2), "+1DAY") {
                HardEnd = true,
                Other = new[] { FacetDateOther.After },
            };
            var r = Serialize(q);
            Assert.Contains(r, KV("facet.date", "{!ex=cat}timestamp"));
            Assert.Contains(r, KV("f.timestamp.facet.date.start", "2009-01-01T00:00:00Z"));
            Assert.Contains(r, KV("f.timestamp.facet.date.end", "2009-02-02T00:00:00Z"));
            Assert.Contains(r, KV("f.timestamp.facet.date.gap", "+1DAY"));
            Assert.Contains(r, KV("f.timestamp.facet.date.hardend", "true"));
            Assert.Contains(r, KV("f.timestamp.facet.date.other", "after"));
        }

        public KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

        private static IList<KeyValuePair<string, string>> Serialize(object o) {
            var fieldSerializer = new DefaultFieldSerializer();
            var serializer = new DefaultFacetQuerySerializer(new DefaultQuerySerializer(fieldSerializer), fieldSerializer);
            return serializer.Serialize(o).ToList();
        }
    }
}