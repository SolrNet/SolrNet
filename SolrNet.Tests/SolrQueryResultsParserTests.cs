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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Mapping;

namespace SolrNet.Tests {
	[TestFixture]
	public partial class SolrQueryResultsParserTests {
		[Test]
		public void ParseDocument() {
		    var mapper = new AttributesMappingManager();
		    var parser = new SolrQueryResultParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var xml = new XmlDocument();
			xml.LoadXml(responseXml);
			var docNode = xml.SelectSingleNode("response/result/doc");
			var doc = parser.ParseDocument(docNode, mapper.GetFields(typeof(TestDocument)));
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
		}

        [Test]
	    public void ParseDocumentWithMappingManager() {
            var mapper = new MappingManager();
            mapper.Add(typeof(TestDocumentWithoutAttributes).GetProperty("Id"), "id");
            var parser = new SolrQueryResultParser<TestDocumentWithoutAttributes>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var xml = new XmlDocument();
			xml.LoadXml(responseXml);
			var docNode = xml.SelectSingleNode("response/result/doc");
            var doc = parser.ParseDocument(docNode, mapper.GetFields(typeof(TestDocumentWithoutAttributes)));
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
	    }

		[Test]
		public void NumFound() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var r = parser.Parse(responseXml);
			Assert.AreEqual(1, r.NumFound);
		}

		[Test]
		public void Parse() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var results = parser.Parse(responseXml);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(123456, doc.Id);
		}

		[Test]
		public void SetPropertyWithArrayOfStrings() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='cat']");
            var mapper = new AttributesMappingManager();
		    var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
			visitor.Visit(doc, "cat", fieldNode);
			Assert.AreEqual(2, doc.Cat.Count);
			var cats = new List<string>(doc.Cat);
			Assert.AreEqual("electronics", cats[0]);
			Assert.AreEqual("hard drive", cats[1]);
		}

		[Test]
		public void SetPropertyDouble() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
            visitor.Visit(doc, "price", fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyNullableDouble() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithNullableDouble();
            visitor.Visit(doc, "price", fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyWithIntCollection() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='numbers']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
            visitor.Visit(doc, "numbers", fieldNode);
			Assert.AreEqual(2, doc.Numbers.Count);
			var numbers = new List<int>(doc.Numbers);
			Assert.AreEqual(1, numbers[0]);
			Assert.AreEqual(2, numbers[1]);
		}

		[Test]
		public void SetPropertyWithNonGenericCollection() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='numbers']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays3();
            visitor.Visit(doc, "numbers", fieldNode);
			Assert.AreEqual(2, doc.Numbers.Count);
			var numbers = new ArrayList(doc.Numbers);
			Assert.AreEqual(1, numbers[0]);
			Assert.AreEqual(2, numbers[1]);
		}

		[Test]
		public void SetPropertyWithArrayOfIntsToIntArray() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='numbers']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays2();
            visitor.Visit(doc, "numbers", fieldNode);
			Assert.AreEqual(2, doc.Numbers.Length);
			var numbers = new List<int>(doc.Numbers);
			Assert.AreEqual(1, numbers[0]);
			Assert.AreEqual(2, numbers[1]);
		}

		[Test]
		public void ParseResultsWithArrays() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithArrays>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var results = parser.Parse(responseXMLWithArrays);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual("SP2514N", doc.Id);
		}

		[Test]
		public void SupportsDateTime() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithDate>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var results = parser.Parse(responseXMLWithDate);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
		}

		[Test]
		public void ParseDate_without_milliseconds() {
		    var parser = new DateTimeFieldParser();
			var dt = parser.ParseDate("2001-01-02T03:04:05Z");
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), dt);
		}

		[Test]
		public void ParseDate_with_milliseconds() {
            var parser = new DateTimeFieldParser();
            var dt = parser.ParseDate("2001-01-02T03:04:05.245Z");
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5, 245), dt);
		}

		[Test]
		public void SupportsNullableDateTime() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithNullableDate>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var results = parser.Parse(responseXMLWithDate);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
		}

		[Test]
		public void SupportsIEnumerable() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithArrays4>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var results = parser.Parse(responseXMLWithArraysSimple);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(2, new List<string>(doc.Features).Count);
		}

        [Test]
        public void SupportsGuid() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocWithGuid>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var results = parser.Parse(responseXMLWithGuid);
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.Key);
        }

        [Test]
        public void SupportsEnumAsInteger() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocWithEnum>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var results = parser.Parse(responseXMLWithEnumAsInt);
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.En);
        }

        [Test]
        public void SupportsEnumAsString() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocWithEnum>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var results = parser.Parse(responseXMLWithEnumAsString);
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.En);
        }

        [Test]
        public void GenericDictionary_string_string() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocWithGenDict>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var results = parser.Parse(responseXMLWithDict);
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Assert.IsNotNull(doc.Dict);
            Assert.AreEqual(2, doc.Dict.Count);
            Assert.AreEqual("1", doc.Dict["One"]);
            Assert.AreEqual("2", doc.Dict["Two"]);
        }

        [Test]
        public void GenericDictionary_string_int() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocWithGenDict2>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var results = parser.Parse(responseXMLWithDict);
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Assert.IsNotNull(doc.Dict);
            Assert.AreEqual(2, doc.Dict.Count);
            Assert.AreEqual(1, doc.Dict["One"]);
            Assert.AreEqual(2, doc.Dict["Two"]);
        }


		[Test]
		public void WrongFieldDoesntThrow() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithDate>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var results = parser.Parse(responseXMLWithArraysSimple);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
		}

		[Test]
		public void ReadsMaxScoreAttribute() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithArrays4>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var results = parser.Parse(responseXMLWithArraysSimple);
			Assert.AreEqual(1.6578954, results.MaxScore);
		}

		[Test]
		public void ReadMaxScore_doesnt_crash_if_not_present() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var results = parser.Parse(responseXml);
			Assert.IsNull(results.MaxScore);
		}

		[Test]
		[Ignore("Performance test, potentially slow")]
		public void Performance() {
			//BasicConfigurator.Configure();
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithArrays>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			for (var i = 0; i < 10000; i++) {
				parser.Parse(responseXMLWithArrays);
			}
		}

		[Test]
		public void ParseFacetResults() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocumentWithArrays>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var r = parser.Parse(responseXMLWithFacet);
			Assert.IsNotNull(r.FacetFields);
			Console.WriteLine(r.FacetFields.Count);
			Assert.IsTrue(r.FacetFields.ContainsKey("cat"));
			Assert.IsTrue(r.FacetFields.ContainsKey("inStock"));
			Assert.AreEqual(2, r.FacetFields["cat"].Where(q => q.Key == "connector").First().Value);

			Assert.IsNotNull(r.FacetQueries);
			Console.WriteLine(r.FacetQueries.Count);
			Assert.AreEqual(1, r.FacetQueries.Count);
		}

		[Test]
		public void ParseResponseHeader() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var xml = new XmlDocument();
			xml.LoadXml(responseXml);
			var docNode = xml.SelectSingleNode("response/lst[@name='responseHeader']");
			var header = parser.ParseHeader(docNode);
			Assert.AreEqual(1, header.Status);
			Assert.AreEqual(15, header.QTime);
			Assert.AreEqual(2, header.Params.Count);
			Assert.AreEqual("id:123456", header.Params["q"]);
			Assert.AreEqual("2.2", header.Params["version"]);
		}

		[Test]
		public void ParseHighlighting() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<Product>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
			var xml = new XmlDocument();
			xml.LoadXml(responseXmlWithHighlighting);
			var docNode = xml.SelectSingleNode("response/lst[@name='highlighting']");
			var item = new Product { Id = "SP2514N" };
			var highlights = parser.ParseHighlighting(new SolrQueryResults<Product> { item }, docNode);
			Assert.AreEqual(1, highlights.Count);
			Assert.AreSame(item, highlights.First().Key);
			var kv = highlights.First().Value;
			Assert.AreEqual(1, kv.Count);
			Assert.AreEqual("features", kv.First().Key);
			Assert.That(kv.First().Value, Text.Contains("Noise"));
		}

        [Test]
        public void ParseSpellChecking() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<Product>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var xml = new XmlDocument();
            xml.LoadXml(responseXmlWithSpellChecking);
            var docNode = xml.SelectSingleNode("response/lst[@name='spellcheck']");
            var spellChecking = parser.ParseSpellChecking(docNode);
            Assert.IsNotNull(spellChecking);
            Assert.AreEqual("dell ultrasharp", spellChecking.Collation);
            Assert.AreEqual(2, spellChecking.Count);
        }

        [Test]
        public void ParseMoreLikeThis() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrQueryResultParser<Product>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()));
            var xml = new XmlDocument();
            xml.LoadXml(responseXmlWithMoreLikeThis);
            var docNode = xml.SelectSingleNode("response/lst[@name='moreLikeThis']");
            var product1 = new Product { Id = "UTF8TEST" };
            var product2 = new Product { Id = "SOLR1000" };
            var mlt = parser.ParseMoreLikeThis(new[] {
                product1,
                product2,
            }, docNode);
            Assert.IsNotNull(mlt);
            Assert.AreEqual(2, mlt.Count);
            Assert.IsTrue(mlt.ContainsKey(product1));
            Assert.IsTrue(mlt.ContainsKey(product2));
            Assert.AreEqual(1, mlt[product1].Count);
            Assert.AreEqual(1, mlt[product2].Count);
            Console.WriteLine(mlt[product1][0].Id);
        }

        public enum AEnum {
            One,
            Two,
            Three
        }

        public class TestDocWithEnum {
            [SolrField]
            public AEnum En { get; set; }
        }
	}
}