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
using MbUnit.Framework;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.Tests {
    [TestFixture]
    public class OperatorOverloadingTests {
        public string Serialize(object q) {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }

        [Test]
        public void OneAnd() {
            var q = new SolrQuery("solr") && new SolrQuery("name:desc");
            Assert.AreEqual("(solr AND name:desc)", Serialize(q));
        }

        [Test]
        public void OneOr() {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc");
            Assert.AreEqual("(solr OR name:desc)", Serialize(q));
        }

        [Test]
        public void MultipleAnd() {
            var q = new SolrQuery("solr") && new SolrQuery("name:desc") && new SolrQueryByField("id", "123456");
            Assert.AreEqual("((solr AND name:desc) AND id:(123456))", Serialize(q));
        }

        [Test]
        public void MultipleOr() {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc") || new SolrQueryByField("id", "123456");
            Assert.AreEqual("((solr OR name:desc) OR id:(123456))", Serialize(q));
        }

        [Test]
        public void MixedAndOrs_obeys_operator_precedence() {
            var q = new SolrQuery("solr") || new SolrQuery("name:desc") && new SolrQueryByField("id", "123456");
            Assert.AreEqual("(solr OR (name:desc AND id:(123456)))", Serialize(q));
        }

        [Test]
        public void MixedAndOrs_with_parentheses_obeys_precedence() {
            var q = (new SolrQuery("solr") || new SolrQuery("name:desc")) && new SolrQueryByField("id", "123456");
            Assert.AreEqual("((solr OR name:desc) AND id:(123456))", Serialize(q));
        }

        [Test]
        public void Add() {
            var q = new SolrQuery("solr") + new SolrQuery("name:desc");
            Assert.AreEqual("(solr  name:desc)", Serialize(q));
        }

        [Test]
        public void PlusEqualMany() {
            AbstractSolrQuery q = new SolrQuery("first");
            foreach (var _ in Enumerable.Range(0, 10)) {
                q += new SolrQuery("others");
            }
            Assert.AreEqual("((((((((((first  others)  others)  others)  others)  others)  others)  others)  others)  others)  others)", Serialize(q));
        }

        [Test]
        public void Not() {
            var q = !new SolrQuery("solr");
            Assert.AreEqual("-solr", Serialize(q));
        }

        [Test]
        public void AndNot() {
            var q = new SolrQuery("a") && !new SolrQuery("b");
            Console.WriteLine(Serialize(q));
            Assert.AreEqual("(a AND -b)", Serialize(q));
        }

        [Test]
        public void Minus() {
            var q = new SolrQuery("solr") - new SolrQuery("name:desc");
            Assert.AreEqual("(solr  -name:desc)", Serialize(q));
        }

        [Test]
        public void AllMinus() {
            var q = SolrQuery.All - new SolrQuery("product");
            Assert.AreEqual("(*:*  -product)", Serialize(q));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAnd_Throws() {
            var a = SolrQuery.All && null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullOr_Throws() {
            var a = SolrQuery.All || null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullPlus_throws() {
            var a = SolrQuery.All + null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullMinus_throws() {
            var a = SolrQuery.All - null;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullNot_argumentnull() {
            AbstractSolrQuery a = null;
            var b = !a;
        }
    }
}