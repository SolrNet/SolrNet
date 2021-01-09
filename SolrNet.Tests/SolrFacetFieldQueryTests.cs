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
using Xunit.Abstractions;

namespace SolrNet.Tests {
    
    public class SolrFacetFieldQueryTests {
        private readonly ITestOutputHelper testOutputHelper;

        public SolrFacetFieldQueryTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public IList<KeyValuePair<string, string>> Serialize(object o) {
            var fieldSerializer = new DefaultFieldSerializer();
            var serializer = new DefaultFacetQuerySerializer(new DefaultQuerySerializer(fieldSerializer), fieldSerializer);
            return serializer.Serialize(o).ToList();
        }
            
        [Fact]
        public void FieldOnly() {
            var fq = new SolrFacetFieldQuery("pepe");
            var q = Serialize(fq);
            Assert.Equal(1, q.Count);
            testOutputHelper.WriteLine(q[0].ToString());
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
        }

        [Fact]
        public void Prefix() {
            var fq = new SolrFacetFieldQuery("pepe") {Prefix = "pre"};
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.prefix", q[1].Key);
            Assert.Equal("pre", q[1].Value);
        }

        [Fact]
        public void Sort() {
            var fq = new SolrFacetFieldQuery("pepe") {Sort = true};
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.sort", q[1].Key);
            Assert.Equal("true", q[1].Value);
        }

        [Fact]
        public void Limit() {
            var fq = new SolrFacetFieldQuery("pepe") {Limit = 5};
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.limit", q[1].Key);
            Assert.Equal("5", q[1].Value);
        }

        [Fact]
        public void Offset() {
            var fq = new SolrFacetFieldQuery("pepe") {Offset = 5};
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.offset", q[1].Key);
            Assert.Equal("5", q[1].Value);
        }

        [Fact]
        public void Mincount() {
            var fq = new SolrFacetFieldQuery("pepe") {MinCount = 5};
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.mincount", q[1].Key);
            Assert.Equal("5", q[1].Value);
        }

        [Fact]
        public void MincountWithLocalParams_IgnoresLocalParams() {
            var fq = new SolrFacetFieldQuery(new LocalParams { { "ex", "cat" } } + "pepe") { MinCount = 5 };
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("{!ex=cat}pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.mincount", q[1].Key);
            Assert.Equal("5", q[1].Value);
        }

        [Fact]
        public void Missing() {
            var fq = new SolrFacetFieldQuery("pepe") {Missing = true};
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.missing", q[1].Key);
            Assert.Equal("true", q[1].Value);
        }

        [Fact]
        public void EnumCacheDF() {
            var fq = new SolrFacetFieldQuery("pepe") {EnumCacheMinDf = 5};
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.enum.cache.minDf", q[1].Key);
            Assert.Equal("5", q[1].Value);
        }

        [Fact]
        public void Contains()
        {
            var fq = new SolrFacetFieldQuery("pepe") { Contains = "cont" };
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.contains", q[1].Key);
            Assert.Equal("cont", q[1].Value);
        }

        [Fact]
        public void ContainsIgnoreCase()
        {
            var fq = new SolrFacetFieldQuery("pepe") { ContainsIgnoreCase = true };
            var q = Serialize(fq);
            Assert.Equal(1, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);

            fq = new SolrFacetFieldQuery("pepe") { ContainsIgnoreCase = true, Contains = "cont" };
            q = Serialize(fq);
            Assert.Equal(3, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);

            Assert.Equal("f.pepe.facet.contains", q[1].Key);
            Assert.Equal("cont", q[1].Value);

            Assert.Equal("f.pepe.facet.contains.ignoreCase", q[2].Key);
            Assert.Equal("true", q[2].Value);
        }

        [Fact]
        public void Exists()
        {
            var fq = new SolrFacetFieldQuery("pepe") { Exists = true };
            var q = Serialize(fq);
            Assert.Equal(2, q.Count);
            Assert.Equal("facet.field", q[0].Key);
            Assert.Equal("pepe", q[0].Value);
            Assert.Equal("f.pepe.facet.exists", q[1].Key);
            Assert.Equal("true", q[1].Value);
        }
    }
}
