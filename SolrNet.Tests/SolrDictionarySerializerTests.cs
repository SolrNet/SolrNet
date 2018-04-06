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
using System.Xml.Linq;
using Xunit;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;

namespace SolrNet.Tests
{

    public class SolrDictionarySerializerTests
    {
        [Fact]
        public void Serialize_null()
        {
            var serializer = GetSerializer();
            Assert.Throws<ArgumentNullException>(() => serializer.Serialize(null, null));
        }

        [Fact]
        public void Serialize_empty()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object>(), null);
            Assert.Equal(0, xml.Nodes().Count());
        }

        [Fact]
        public void Serialize_string()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", "uno"}
            }, null);
            AssertSerializedField(xml, "uno");
        }

        private static void AssertSerializedField(XElement docNode, string value)
        {
            Assert.Single(docNode.Nodes());
            var fieldNode = docNode.Element("field");
            Assert.NotNull(fieldNode);
            Assert.Equal("one", fieldNode.Attribute("name").Value);
            Assert.Equal(value, fieldNode.Value);
        }

        [Fact]
        public void Serialize_int()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1}
            }, null);
            AssertSerializedField(xml, "1");
        }

        [Fact]
        public void Serialize_float()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1.23f}
            }, null);
            AssertSerializedField(xml, "1.23");
        }

        [Fact]
        public void Serialize_double()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1.23d}
            }, null);
            AssertSerializedField(xml, "1.23");
        }

        [Fact]
        public void Serialize_decimal()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1.23m}
            }, null);
            AssertSerializedField(xml, "1.23");
        }

        [Fact]
        public void Serialize_DateTime()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", new DateTime(2000, 1, 2, 12, 23, 34)}
            }, null);
            AssertSerializedField(xml, "2000-01-02T12:23:34Z");
        }

        [Fact]
        public void Serialize_Array()
        {
            var serializer = GetSerializer();
            var docNode = serializer.Serialize(new Dictionary<string, object> {
                {"one", new[] {1,2,3}}
            }, null);
            Assert.Equal(3, docNode.Nodes().Count());
            var fieldNodes = docNode.Elements("field").ToList();
            Assert.Equal("1", fieldNodes[0].Value);
            Assert.Equal("2", fieldNodes[1].Value);
            Assert.Equal("3", fieldNodes[2].Value);
        }

        [Fact]
        public void Serialize_List()
        {
            var serializer = GetSerializer();
            var docNode = serializer.Serialize(new Dictionary<string, object> {
                {"one", new List<string> {"a", "b", "c"}}
            }, null);
            Assert.Equal(3, docNode.Nodes().Count());
            var fieldNodes = docNode.Elements("field").ToList();
            Assert.Equal("a", fieldNodes[0].Value);
            Assert.Equal("b", fieldNodes[1].Value);
            Assert.Equal("c", fieldNodes[2].Value);
        }

        [Fact]
        public void Serialize_SparseList()
        {
            var serializer = GetSerializer();
            var docNode = serializer.Serialize(new Dictionary<string, object> {
                {"one", new List<string> {"a", null, "b", "c"}}
            }, null);
            Assert.Equal(3, docNode.Nodes().Count());
            var fieldNodes = docNode.Elements("field").ToList();
            Assert.Equal("a", fieldNodes[0].Value);
            Assert.Equal("b", fieldNodes[1].Value);
            Assert.Equal("c", fieldNodes[2].Value);
        }

        [Fact]
        public void Serialize_EmptyList()
        {
            var serializer = GetSerializer();
            var docNode = serializer.Serialize(new Dictionary<string, object> {
                {"one", new List<string> {null, null}}
            }, null);
            Assert.Equal(0, docNode.Nodes().Count());
        }

        [Fact]
        public void Serialize_KeyValuePair()
        {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", new KeyValuePair<string, string>("a", "b")}
            }, null);
            AssertSerializedField(xml, "[a, b]");
        }

        private SolrDictionarySerializer GetSerializer()
        {
            return new SolrDictionarySerializer(new DefaultFieldSerializer());
        }
    }
}