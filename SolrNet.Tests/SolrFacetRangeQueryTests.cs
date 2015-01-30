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
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrFacetRangeQueryTests {
        [Test]
        public void Serialize() {
            var q = new SolrFacetRangeQuery("wordcount", 40, 1000, 10)
            {
                HardEnd = true,
                MinCount = 1,
                Other = new[] {FacetRangeOther.After},
                Include = new[] { FacetRangeInclude.Lower },
            };
            var r = Serialize(q);
            Assert.Contains(r, KV.Create("facet.range", "wordcount"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.start", "40"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.end", "1000"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.gap", "10"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.hardend", "true"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.mincount", "1"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.other", "after"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.include", "lower"));
        }
        
        [Test]
        public void IgnoresLocalParams() {
            var q = new SolrFacetRangeQuery(new LocalParams { { "ex", "cat" } } + "wordcount", 40, 1000, 10)
            {
                HardEnd = true,
                MinCount = 1,
                Other = new[] { FacetRangeOther.After },
                Include = new[] { FacetRangeInclude.Lower },
            };
            var r = Serialize(q);
            Assert.Contains(r, KV.Create("facet.range", "{!ex=cat}wordcount"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.start", "40"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.end", "1000"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.gap", "10"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.mincount", "1"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.hardend", "true"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.other", "after"));
            Assert.Contains(r, KV.Create("f.wordcount.facet.range.include", "lower"));
        }

        private static IList<KeyValuePair<string, string>> Serialize(object o) {
            var fieldSerializer = new DefaultFieldSerializer();
            var serializer = new DefaultFacetQuerySerializer(new DefaultQuerySerializer(fieldSerializer), fieldSerializer);
            return serializer.Serialize(o).ToList();
        }
    }
}
