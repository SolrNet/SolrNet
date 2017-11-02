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
using Xunit;
using SolrNet.Impl.FacetQuerySerializers;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using SolrNet.Utils;

namespace SolrNet.Tests
{

    public class SolrFacetRangeQueryTests
    {
        [Fact]
        public void SerializeWithDate()
        {
            var q = new SolrFacetRangeQuery("timestamp", new DateTime(2009, 1, 1), new DateTime(2009, 2, 2), "+1DAY")
            {
                HardEnd = true,
                Other = new[] { FacetRangeOther.After },
                Include = new[] { FacetRangeInclude.Lower },
                Method = FacetRangeMethod.DV
            };
            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.range", "timestamp"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.start", "2009-01-01T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.end", "2009-02-02T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.gap", "+1DAY"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.hardend", "true"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.other", "after"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.include", "lower"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.method", "dv"), r);
        }

        [Fact]
        public void SerializeWithInt()
        {
            var q = new SolrFacetRangeQuery("version",5, 100, 2)
            {
                HardEnd = true,
                Other = new[] { FacetRangeOther.After },
                Include = new[] { FacetRangeInclude.Lower }
            };
            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.range", "version"), r);
            Assert.Contains(KV.Create("f.version.facet.range.start", "5"), r);
            Assert.Contains(KV.Create("f.version.facet.range.end", "100"), r);
            Assert.Contains(KV.Create("f.version.facet.range.gap", "2"), r);
            Assert.Contains(KV.Create("f.version.facet.range.hardend", "true"), r);
            Assert.Contains(KV.Create("f.version.facet.range.other", "after"), r);
            Assert.Contains(KV.Create("f.version.facet.range.include", "lower"), r);
            Assert.False(r.Any(kv => kv.Key == "f.version.facet.range.method"));
        }

        [Fact]
        public void IgnoresLocalParams()
        {
            var q = new SolrFacetRangeQuery(new LocalParams { { "ex", "cat" } } + "timestamp", new DateTime(2009, 1, 1), new DateTime(2009, 2, 2), "+1DAY")
            {
                HardEnd = true,
                Other = new[] { FacetRangeOther.After },
                Include = new[] { FacetRangeInclude.Lower },
            };
            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.range", "{!ex=cat}timestamp"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.start", "2009-01-01T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.end", "2009-02-02T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.gap", "+1DAY"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.hardend", "true"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.other", "after"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.range.include", "lower"), r);
        }

        private static IList<KeyValuePair<string, string>> Serialize(object o)
        {
            var fieldSerializer = new DefaultFieldSerializer();
            var serializer = new DefaultFacetQuerySerializer(new DefaultQuerySerializer(fieldSerializer), fieldSerializer);
            return serializer.Serialize(o).ToList();
        }
    }
}
