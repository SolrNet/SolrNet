﻿#region license
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

    public class SolrFacetDateQueryTests
    {

        [Fact]
#pragma warning disable xUnit1024 // Test methods cannot have overloads
        public void Serialize()
        {
            var q = new SolrFacetDateQuery("timestamp", new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2009, 2, 2, 0, 0, 0, DateTimeKind.Utc), "+1DAY")
            {
                HardEnd = true,
                MinCount = 2,
                Other = new[] {FacetDateOther.After},
                Include = new[] { FacetDateInclude.Lower },
            };
            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.date", "timestamp"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.start", "2009-01-01T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.end", "2009-02-02T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.gap", "+1DAY"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.hardend", "true"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.mincount", "2"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.other", "after"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.include", "lower"), r);
        }

        [Fact]
        public void IgnoresLocalParams()
        {
            var q = new SolrFacetDateQuery(new LocalParams { { "ex", "cat" } } + "timestamp", new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(2009, 2, 2, 0, 0, 0, DateTimeKind.Utc), "+1DAY")
            {
                HardEnd = true,
                MinCount = 2,
                Other = new[] { FacetDateOther.After },
                Include = new[] { FacetDateInclude.Lower },
            };
            var r = Serialize(q);
            Assert.Contains(KV.Create("facet.date", "{!ex=cat}timestamp"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.start", "2009-01-01T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.end", "2009-02-02T00:00:00Z"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.gap", "+1DAY"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.hardend", "true"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.mincount", "2"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.other", "after"), r);
            Assert.Contains(KV.Create("f.timestamp.facet.date.include", "lower"), r);
        }

        private static IList<KeyValuePair<string, string>> Serialize(object o)
        {
            var fieldSerializer = new DefaultFieldSerializer();
            var serializer = new DefaultFacetQuerySerializer(new DefaultQuerySerializer(fieldSerializer), fieldSerializer);
            return serializer.Serialize(o).ToList();
        }
    }
}
