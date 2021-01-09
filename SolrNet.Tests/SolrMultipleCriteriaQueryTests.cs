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
using System.Linq;
using Xunit;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;
using Xunit.Abstractions;

namespace SolrNet.Tests {
    
    public class SolrMultipleCriteriaQueryTests {
        private readonly ITestOutputHelper testOutputHelper;

        public SolrMultipleCriteriaQueryTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public class TestDocument  {}

        public string Serialize(object q) {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }


        [Fact]
        public void Concat() {
            var q1 = new SolrQuery("1");
            var q2 = new SolrQuery("2");
            var qm = new SolrMultipleCriteriaQuery(new[] {q1, q2});
            Assert.Equal("(1  2)", Serialize(qm));
        }

        [Fact]
        public void Concat_different_types() {
            var q1 = new SolrQuery("1");
            var q2 = new SolrQueryByField("f", "v");
            var qm = new SolrMultipleCriteriaQuery(new ISolrQuery[] {q1, q2});
            testOutputHelper.WriteLine(Serialize(qm));
            Assert.Equal("(1  f:(v))", Serialize(qm));
        }


        [Fact]
        public void AcceptsNulls() {
            var q1 = new SolrQuery("1");
            ISolrQuery q2 = null;
            var qm = new SolrMultipleCriteriaQuery(new[] {q1, q2});
            Assert.Equal("(1)", Serialize(qm));
        }

        [Fact]
        public void Empty() {
            var qm = new SolrMultipleCriteriaQuery(new ISolrQuery[] { });
            Assert.Empty(Serialize(qm));
        }

        [Fact]
        public void EmptyQueries_are_ignored() {
            var qm = new SolrMultipleCriteriaQuery(new ISolrQuery[] { new SolrQuery(""), });
            Assert.Empty(Serialize(qm));
        }

        [Fact]
        public void NullQueries_are_ignored() {
            var qm = new SolrMultipleCriteriaQuery(new ISolrQuery[] { new SolrQuery(null), });
            Assert.Empty(Serialize(qm));
        }

        [Fact]
        public void StaticConstructor() {
            var q = SolrMultipleCriteriaQuery.Create(new SolrQueryByField("id", "123"), new SolrQuery("solr"));
            Assert.Equal(2, q.Queries.Count());
            Assert.Equal("(id:(123)  solr)", Serialize(q));
            Assert.Empty(q.Oper);
        }
    }
}
