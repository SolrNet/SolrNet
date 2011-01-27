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
    public class SolrFacetFieldQueryTests {
        public IList<KeyValuePair<string, string>> Serialize(object o) {
            var fieldSerializer = new DefaultFieldSerializer();
            var serializer = new DefaultFacetQuerySerializer(new DefaultQuerySerializer(fieldSerializer), fieldSerializer);
            return serializer.Serialize(o).ToList();
        }
            
        [Test]
        public void FieldOnly() {
            var fq = new SolrFacetFieldQuery("pepe");
            var q = Serialize(fq);
            Assert.AreEqual(1, q.Count);
            Console.WriteLine(q[0]);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
        }

        [Test]
        public void Prefix() {
            var fq = new SolrFacetFieldQuery("pepe") {Prefix = "pre"};
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.prefix", q[1].Key);
            Assert.AreEqual("pre", q[1].Value);
        }

        [Test]
        public void Sort() {
            var fq = new SolrFacetFieldQuery("pepe") {Sort = true};
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.sort", q[1].Key);
            Assert.AreEqual("true", q[1].Value);
        }

        [Test]
        public void Limit() {
            var fq = new SolrFacetFieldQuery("pepe") {Limit = 5};
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.limit", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }

        [Test]
        public void Offset() {
            var fq = new SolrFacetFieldQuery("pepe") {Offset = 5};
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.offset", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }

        [Test]
        public void Mincount() {
            var fq = new SolrFacetFieldQuery("pepe") {MinCount = 5};
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.mincount", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }

        [Test]
        public void MincountWithLocalParams_IgnoresLocalParams() {
            var fq = new SolrFacetFieldQuery(new LocalParams { { "ex", "cat" } } + "pepe") { MinCount = 5 };
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("{!ex=cat}pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.mincount", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }

        [Test]
        public void Missing() {
            var fq = new SolrFacetFieldQuery("pepe") {Missing = true};
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.missing", q[1].Key);
            Assert.AreEqual("true", q[1].Value);
        }

        [Test]
        public void EnumCacheDF() {
            var fq = new SolrFacetFieldQuery("pepe") {EnumCacheMinDf = 5};
            var q = Serialize(fq);
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual("facet.field", q[0].Key);
            Assert.AreEqual("pepe", q[0].Value);
            Assert.AreEqual("f.pepe.facet.enum.cache.minDf", q[1].Key);
            Assert.AreEqual("5", q[1].Value);
        }
    }
}