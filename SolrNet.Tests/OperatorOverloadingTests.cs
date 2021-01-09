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

namespace SolrNet.Tests
{

    public class OperatorOverloadingTests
    {
        private readonly ITestOutputHelper testOutputHelper;

        public OperatorOverloadingTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public string Serialize(object q)
        {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }

        [Fact]
        public void OneAnd()
        {
            var q = new SolrQuery("solr") && new SolrQuery("name:desc");
            Assert.Equal("(solr AND name:desc)", Serialize(q));
        }

        [Fact]
        public void OneOr()
        {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc");
            Assert.Equal("(solr OR name:desc)", Serialize(q));
        }

        [Fact]
        public void MultipleAnd()
        {
            var q = new SolrQuery("solr") && new SolrQuery("name:desc") && new SolrQueryByField("id", "123456");
            Assert.Equal("((solr AND name:desc) AND id:(123456))", Serialize(q));
        }

        [Fact]
        public void MultipleOr()
        {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc") || new SolrQueryByField("id", "123456");
            Assert.Equal("((solr OR name:desc) OR id:(123456))", Serialize(q));
        }

        [Fact]
        public void MixedAndOrs_obeys_operator_precedence()
        {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc") && new SolrQueryByField("id", "123456");
            Assert.Equal("(solr OR (name:desc AND id:(123456)))", Serialize(q));
        }

        [Fact]
        public void MixedAndOrs_with_parentheses_obeys_precedence()
        {
            var q = (new SolrQuery("solr") || new SolrQuery("name:desc")) && new SolrQueryByField("id", "123456");
            Assert.Equal("((solr OR name:desc) AND id:(123456))", Serialize(q));
        }

        [Fact]
        public void Add()
        {
            var q = new SolrQuery("solr") + new SolrQuery("name:desc");
            Assert.Equal("(solr  name:desc)", Serialize(q));
        }

        [Fact]
        public void PlusEqualMany()
        {
            AbstractSolrQuery q = new SolrQuery("first");
            foreach (var _ in Enumerable.Range(0, 10))
            {
                q += new SolrQuery("others");
            }
            Assert.Equal("((((((((((first  others)  others)  others)  others)  others)  others)  others)  others)  others)  others)", Serialize(q));
        }

        [Fact]
        public void Not()
        {
            var q = !new SolrQuery("solr");
            Assert.Equal("-solr", Serialize(q));
        }

        [Fact]
        public void AndNot()
        {
            var q = new SolrQuery("a") && !new SolrQuery("b");
            testOutputHelper.WriteLine(Serialize(q));
            Assert.Equal("(a AND -b)", Serialize(q));
        }

        [Fact]
        public void Minus()
        {
            var q = new SolrQuery("solr") - new SolrQuery("name:desc");
            Assert.Equal("(solr  -name:desc)", Serialize(q));
        }

        [Fact]
        public void AllMinus()
        {
            var q = SolrQuery.All - new SolrQuery("product");
            Assert.Equal("(*:*  -product)", Serialize(q));
        }

        [Fact]

        public void NullAnd_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => SolrQuery.All && null);
        }

        [Fact]

        public void NullOr_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => SolrQuery.All || null);
        }

        [Fact]

        public void NullPlus_throws()
        {
            Assert.Throws<ArgumentNullException>(() => SolrQuery.All + null);
        }

        [Fact]
        public void NullMinus_throws()
        {
            Assert.Throws<ArgumentNullException>(() => SolrQuery.All - null);
        }

        [Fact]
        public void NullNot_argumentnull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                AbstractSolrQuery a = null;
                var b = !a;
            });
        }
    }
}
