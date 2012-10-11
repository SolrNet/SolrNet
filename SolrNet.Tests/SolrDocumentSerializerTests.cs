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
using System.Xml;
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Mapping;

namespace SolrNet.Tests {
	[TestFixture]
	public partial class SolrDocumentSerializerTests {
		[Test]
		public void Serializes() {
		    var mapper = new AttributesMappingManager();
			var ser = new SolrDocumentSerializer<SampleDoc>(mapper, new DefaultFieldSerializer());
			var doc = new SampleDoc {Id = "id", Dd = 23.5m};
			string fs = ser.Serialize(doc, null).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"Id\">id</field><field name=\"Flower\">23.5</field></doc>", fs);
		}

		[Test]
		public void SupportsCollections() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithCollections>(mapper, new DefaultFieldSerializer());
			var doc = new TestDocWithCollections();
			string fs = ser.Serialize(doc, null).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"coll\">one</field><field name=\"coll\">two</field></doc>", fs);
		}

		[Test]
		public void EscapesStrings() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<SampleDoc>(mapper, new DefaultFieldSerializer());
			var doc = new SampleDoc {Id = "<quote\""};
            string fs = ser.Serialize(doc, null).OuterXml;
			Console.WriteLine(fs);
			var xml = new XmlDocument();
			xml.LoadXml(fs);
            Assert.AreEqual("<doc><field name=\"Id\">&lt;quote\"</field><field name=\"Flower\">0</field></doc>", fs);
		}

		[Test]
		public void AcceptsNullObjects() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<SampleDoc>(mapper, new DefaultFieldSerializer());
			var doc = new SampleDoc {Id = null};
            string fs = ser.Serialize(doc, null).OuterXml;
			Console.WriteLine(fs);
			var xml = new XmlDocument();
			xml.LoadXml(fs);
		}

		/// <summary>
		/// Support according to http://lucene.apache.org/solr/api/org/apache/solr/schema/DateField.html
		/// </summary>
		[Test]
		public void SupportsDateTime() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithDate>(mapper, new DefaultFieldSerializer());
			var doc = new TestDocWithDate {Date = new DateTime(2001, 1, 2, 3, 4, 5)};
            string fs = ser.Serialize(doc, null).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"Date\">2001-01-02T03:04:05Z</field></doc>", fs);
		}

		[Test]
		public void SupportsBoolTrue() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithBool>(mapper, new DefaultFieldSerializer());
			var doc = new TestDocWithBool {B = true};
            string fs = ser.Serialize(doc, null).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"B\">true</field></doc>", fs);
		}

		[Test]
		public void SupportsBoolFalse() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithBool>(mapper, new DefaultFieldSerializer());
			var doc = new TestDocWithBool {B = false};
            string fs = ser.Serialize(doc, null).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"B\">false</field></doc>", fs);
		}

        [Test]
        public void SupportsGuid() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithGuid>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithGuid {Key = Guid.NewGuid()};
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual("<doc><field name=\"Key\">"+doc.Key+"</field></doc>", fs);
        }

        [Test]
        public void SupportsGenericDictionary_empty() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithGenDict>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithGenDict {
                Id = 5,
                Dict = new Dictionary<string, string>(),
            };
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual("<doc><field name=\"Id\">" + doc.Id + "</field></doc>", fs);
        }

        [Test]
        public void SupportsGenericDictionary_string_string() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithGenDict>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithGenDict {
                Id = 5,
                Dict = new Dictionary<string, string> {
                    {"one", "1"},
                    {"two", "2"},
                },
            };
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual("<doc><field name=\"Id\">" + doc.Id + "</field><field name=\"Dictone\">1</field><field name=\"Dicttwo\">2</field></doc>", fs);
        }

        [Test]
        public void SupportsGenericDictionary_string_int() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithGenDict2>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithGenDict2 {
                Id = 5,
                Dict = new Dictionary<string, int> {
                    {"one", 1},
                    {"two", 2},
                },
            };
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual("<doc><field name=\"Id\">" + doc.Id + "</field><field name=\"Dictone\">1</field><field name=\"Dicttwo\">2</field></doc>", fs);
        }

        [Test]
        public void SupportsGenericDictionary_rest() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithGenDict3>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithGenDict3 {
                Id = 5,
                Dict = new Dictionary<string, object> {
                    {"one", 1},
                    {"two", 2},
                    {"fecha", new DateTime(2010, 1, 1)},
                    {"SomeCollection", new[] {"a", "b", "c"}},
                },
            };
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual("<doc><field name=\"Id\">5</field><field name=\"one\">1</field><field name=\"two\">2</field><field name=\"fecha\">2010-01-01T00:00:00Z</field><field name=\"SomeCollection\">a</field><field name=\"SomeCollection\">b</field><field name=\"SomeCollection\">c</field></doc>", fs);
        }

        [Test]
        public void SupportsNullableDateTime() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithNullableDate>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithNullableDate();
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual("<doc />", fs);
        }

        [Test]
        public void UTF_XML() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithString>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithString {
                Desc = @"ÚóÁ⌠╒"""
            };
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual(@"<doc><field name=""Desc"">ÚóÁ⌠╒""</field></doc>", fs);
        }

        [Test]
        public void DocumentBoost() {
            var mapper = new AttributesMappingManager();
            ISolrDocumentSerializer<TestDocWithString> ser = new SolrDocumentSerializer<TestDocWithString>(mapper, new DefaultFieldSerializer());
            var doc = new TestDocWithString {
                Desc = "hello"
            };
            string fs = ser.Serialize(doc, 2.1).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual(@"<doc boost=""2.1""><field name=""Desc"">hello</field></doc>", fs);            
        }

        [Test]
        public void Inheritance() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithString>(mapper, new DefaultFieldSerializer());
            var doc = new InheritedDoc {
                Desc = "Description",
                Desc1 = "Description1"
            };
            string fs = ser.Serialize(doc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual(@"<doc><field name=""Desc1"">Description1</field><field name=""Desc"">Description</field></doc>", fs);
        }

        [Test]
        public void PropertyWithoutGetter() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithoutGetter>(mapper, new DefaultFieldSerializer());
            string fs = ser.Serialize(new TestDocWithoutGetter(), null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
        }

        [Test]
        public void Location() {
            var mapper = new AttributesMappingManager();
            var ser = new SolrDocumentSerializer<TestDocWithLocation>(mapper, new DefaultFieldSerializer());
            var testDoc = new TestDocWithLocation { Loc = new Location(12.2, -12.3) };
            string fs = ser.Serialize(testDoc, null).OuterXml;
            var xml = new XmlDocument();
            xml.LoadXml(fs);
            Console.WriteLine(fs);
            Assert.AreEqual(@"<doc><field name=""location"">12.2,-12.3</field></doc>", fs);
        }

	}
}