using System;
using NUnit.Framework;
using SolrNet.Utils;

#region license

// Copyright (c) 2007-2009 Mauricio Scheffer
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

namespace SolrNet.DSL.Tests {
    [TestFixture]
    public class QueryBuildingTests {
        [Test]
        public void Simple() {
            var q = Query.Simple("name:solr");
            Assert.AreEqual("name:solr", q.Query);
        }

        [Test]
        public void FieldValue() {
            var q = Query.Field("name").Is("solr");
            Assert.AreEqual("name:solr", q.Query);
        }

        [Test]
        public void FieldValueDecimal() {
            var q = Query.Field("price").Is(400);
            Assert.AreEqual("price:400", q.Query);
        }

        [Test]
        public void FieldValueNot() {
            var q = Query.Field("name").Is("solr").Not();
            Assert.AreEqual("-name:solr", q.Query);
        }

        [Test]
        public void Range() {
            var q = Query.Field("price").From(10).To(20);
            Assert.AreEqual("price:[10 TO 20]", q.Query);
        }

        [Test]
        public void InList() {
            var q = Query.Field("price").In(10, 20, 30);
            Assert.AreEqual("(price:10 OR price:20 OR price:30)", q.Query);
        }

        [Test]
        [Ignore("not implemented")]
        public void And() {}
    }

    public static class Query {
        public static ISolrQuery Simple(string s) {
            return new SolrQuery(s);
        }

        public static FieldDefinition Field(string field) {
            return new FieldDefinition(field);
        }
    }

    public class RangeDefinition<T> {
        private readonly string fieldName;
        private readonly T from;

        public RangeDefinition(string fieldName, T from) {
            this.fieldName = fieldName;
            this.from = from;
        }

        public ISolrQuery To(T to) {
            return new SolrQueryByRange<T>(fieldName, from, to);
        }
    }


    public class FieldDefinition {
        private readonly string fieldName;

        public FieldDefinition(string fieldName) {
            this.fieldName = fieldName;
        }

        public RangeDefinition<T> From<T>(T from) {
            return new RangeDefinition<T>(fieldName, from);
        }

        public ISolrQuery In<T>(params T[] values) {
            return new SolrQueryInList(fieldName, Func.Select(values, v => Convert.ToString(v)));
        }

        public ISolrQuery Is<T>(T value) {
            return new SolrQueryByField(fieldName, Convert.ToString(value));
        }
    }
}