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
using Xunit;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Impl.QuerySerializers;

namespace SolrNet.DSL.Tests {
    public class QueryBuildingTests {

        public string Serialize(object q) {
            var serializer = new DefaultQuerySerializer(new DefaultFieldSerializer());
            return serializer.Serialize(q);
        }


        [Fact]
        public void Simple() {
            var q = Query.Simple("name:solr");
            Assert.Equal("name:solr", q.Query);
        }

        [Fact]
        public void FieldValue() {
            var q = Query.Field("name").Is("solr");
            Assert.Equal("name:(solr)", Serialize(q));
        }

        [Fact]
        public void FieldValueDecimal() {
            var q = Query.Field("price").Is(400);
            Assert.Equal("price:(400)", Serialize(q));
        }

        [Fact]
        public void FieldValueEmpty() {
            var q = Query.Field("price").Is("");
            Assert.Equal("price:(\"\")", Serialize(q));
        }

        [Fact]
        public void FieldValueNot() {
            var q = Query.Field("name").Is("solr").Not();
            Assert.Equal("-name:(solr)", Serialize(q));
        }

        [Fact]
        public void FieldValueRequired() {
            var q = Query.Field("name").Is("solr").Required();
            Assert.Equal("+name:(solr)", Serialize(q));
        }

        [Fact]
        public void Range() {
            var q = Query.Field("price").From(10).To(20);
            Assert.Equal("price:[10 TO 20]", Serialize(q));
        }

        [Fact]
        public void InList() {
            var q = Query.Field("price").In(10, 20, 30);
            Assert.Equal("(price:((10) OR (20) OR (30)))", Serialize(q));
        }

        [Fact]
        public void InList_empty_is_ignored() {
            var q = Query.Field("price").In(new string[0]) && Query.Field("id").Is(123);
            var query = Serialize(q);
            Console.WriteLine(query);
            Assert.Equal("(id:(123))", query);
        }

        [Fact]
        public void HasValue() {
            var q = Query.Field("name").HasAnyValue();
            Assert.Equal("name:[* TO *]", Serialize(q));
        }
    }
}