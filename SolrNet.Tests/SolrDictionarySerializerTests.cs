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
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrDictionarySerializerTests {
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Serialize_null() {
            var serializer = GetSerializer();
            serializer.Serialize(null, null);
        }

        [Test]
        public void Serialize_empty() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object>(), null);
            Assert.AreEqual(xml.Nodes().Count(), 0);
        }

        [Test]
        public void Serialize_string() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", "uno"}
            }, null);
            AssertSerializedField(xml, "uno");
        }

        private static void AssertSerializedField(XElement docNode, string value) {
            Assert.AreEqual(docNode.Nodes().Count(), 1);
            var fieldNode = docNode.Element("field");
            Assert.IsNotNull(fieldNode);
            Assert.AreEqual("one", fieldNode.Attribute("name").Value);
            Assert.AreEqual(value, fieldNode.Value);
        }

        [Test]
        public void Serialize_int() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1}
            }, null);
            AssertSerializedField(xml, "1");
        }

        [Test]
        public void Serialize_float() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1.23f}
            }, null);
            AssertSerializedField(xml, "1.23");
        }

        [Test]
        public void Serialize_double() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1.23d}
            }, null);
            AssertSerializedField(xml, "1.23");
        }

        [Test]
        public void Serialize_decimal() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", 1.23m}
            }, null);
            AssertSerializedField(xml, "1.23");
        }

        [Test]
        public void Serialize_DateTime() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", new DateTime(2000, 1, 2, 12, 23, 34)}
            }, null);
            AssertSerializedField(xml, "2000-01-02T12:23:34Z");
        }

        [Test]
        public void Serialize_Array() {
            var serializer = GetSerializer();
            var docNode = serializer.Serialize(new Dictionary<string, object> {
                {"one", new[] {1,2,3}}
            }, null);
            Assert.AreEqual(docNode.Nodes().Count(), 3);
            var fieldNodes = docNode.Elements("field").ToList();
            Assert.AreEqual("1", fieldNodes[0].Value);
            Assert.AreEqual("2", fieldNodes[1].Value);
            Assert.AreEqual("3", fieldNodes[2].Value);
        }

        [Test]
        public void Serialize_List() {
            var serializer = GetSerializer();
            var docNode = serializer.Serialize(new Dictionary<string, object> {
                {"one", new List<string> {"a", "b", "c"}}
            }, null);
            Assert.AreEqual(docNode.Nodes().Count(), 3);
            var fieldNodes = docNode.Elements("field").ToList();
            Assert.AreEqual("a", fieldNodes[0].Value);
            Assert.AreEqual("b", fieldNodes[1].Value);
            Assert.AreEqual("c", fieldNodes[2].Value);
        }

        [Test]
        public void Serialize_KeyValuePair() {
            var serializer = GetSerializer();
            var xml = serializer.Serialize(new Dictionary<string, object> {
                {"one", new KeyValuePair<string, string>("a", "b")}
            }, null);
            AssertSerializedField(xml, "[a, b]");
        }

        private SolrDictionarySerializer GetSerializer() {
            return new SolrDictionarySerializer(new DefaultFieldSerializer());
        }
    }
}