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
using MbUnit.Framework;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.DSL.Tests {
    [TestFixture]
    public class QueryBuildingTests {

        public string Serialize(object q) {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }


        [Test]
        public void Simple() {
            var q = Query.Simple("name:solr");
            Assert.AreEqual("name:solr", q.Query);
        }

        [Test]
        public void FieldValue() {
            var q = Query.Field("name").Is("solr");
            Assert.AreEqual("name:(solr)", Serialize(q));
        }

        [Test]
        public void FieldValueDecimal() {
            var q = Query.Field("price").Is(400);
            Assert.AreEqual("price:(400)", Serialize(q));
        }

        [Test]
        public void FieldValueEmpty() {
            var q = Query.Field("price").Is("");
            Assert.AreEqual("price:(\"\")", Serialize(q));
        }

        [Test]
        public void FieldValueNot() {
            var q = Query.Field("name").Is("solr").Not();
            Assert.AreEqual("-name:(solr)", Serialize(q));
        }

        [Test]
        public void FieldValueRequired() {
            var q = Query.Field("name").Is("solr").Required();
            Assert.AreEqual("+name:(solr)", Serialize(q));
        }

        [Test]
        public void Range() {
            var q = Query.Field("price").From(10).To(20);
            Assert.AreEqual("price:[10 TO 20]", Serialize(q));
        }

        [Test]
        public void InList() {
            var q = Query.Field("price").In(10, 20, 30);
            Assert.AreEqual("(price:(10) OR price:(20) OR price:(30))", Serialize(q));
        }

        [Test]
        public void InList_empty_is_ignored() {
            var q = Query.Field("price").In(new string[0]) && Query.Field("id").Is(123);
            var query = Serialize(q);
            Console.WriteLine(query);
            Assert.AreEqual("(id:(123))", query);
        }

        [Test]
        public void HasValue() {
            var q = Query.Field("name").HasAnyValue();
            Assert.AreEqual("name:[* TO *]", Serialize(q));
        }
    }
}