﻿#region license
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
	[TestFixture]
	public partial class SolrQueryResultsParserTests {
		[Test]
		public void ParseDocument() {
		    var mapper = new AttributesMappingManager();
            var parser = new SolrDocumentResponseParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocument>());
		    var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
			var docNode = xml.SelectSingleNode("response/result/doc");
			var doc = parser.ParseDocument(docNode, mapper.GetFields(typeof(TestDocument)));
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
		}

        [Test]
	    public void ParseDocumentWithMappingManager() {
            var mapper = new MappingManager();
            mapper.Add(typeof(TestDocumentWithoutAttributes).GetProperty("Id"), "id");
            var parser = new SolrDocumentResponseParser<TestDocumentWithoutAttributes>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocumentWithoutAttributes>());
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
			var docNode = xml.SelectSingleNode("response/result/doc");
            var doc = parser.ParseDocument(docNode, mapper.GetFields(typeof(TestDocumentWithoutAttributes)));
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
	    }

		[Test]
		public void NumFound() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocument>(new SolrDocumentResponseParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocument>()));
            var parser = new SolrQueryResultParser<TestDocument>(new[] {innerParser});
            var r = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml"));
			Assert.AreEqual(1, r.NumFound);
		}

		[Test]
		public void Parse() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocument>(new SolrDocumentResponseParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocument>()));
            var parser = new SolrQueryResultParser<TestDocument>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(123456, doc.Id);
		}

		[Test]
		public void SetPropertyWithArrayOfStrings() {
		    var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
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
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
			var fieldNode = xml.SelectSingleNode("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
            visitor.Visit(doc, "price", fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyNullableDouble() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
			var fieldNode = xml.SelectSingleNode("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithNullableDouble();
            visitor.Visit(doc, "price", fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyWithIntCollection() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
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
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
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
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
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
            var innerParser = new ResultsResponseParser<TestDocumentWithArrays>(new SolrDocumentResponseParser<TestDocumentWithArrays>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocumentWithArrays>()));
            var parser = new SolrQueryResultParser<TestDocumentWithArrays>(new[] { innerParser });
			var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArrays.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual("SP2514N", doc.Id);
		}

		[Test]
		public void SupportsDateTime() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocumentWithDate>(new SolrDocumentResponseParser<TestDocumentWithDate>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocumentWithDate>()));
            var parser = new SolrQueryResultParser<TestDocumentWithDate>(new[] { innerParser });
			var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDate.xml"));
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
            var innerParser = new ResultsResponseParser<TestDocumentWithNullableDate>(new SolrDocumentResponseParser<TestDocumentWithNullableDate>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocumentWithNullableDate>()));
            var parser = new SolrQueryResultParser<TestDocumentWithNullableDate>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDate.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
		}

		[Test]
		public void SupportsIEnumerable() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocumentWithArrays4>(new SolrDocumentResponseParser<TestDocumentWithArrays4>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocumentWithArrays4>()));
            var parser = new SolrQueryResultParser<TestDocumentWithArrays4>(new[] { innerParser });
			var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArraysSimple.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(2, new List<string>(doc.Features).Count);
		}

        [Test]
        public void SupportsGuid() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocWithGuid>(new SolrDocumentResponseParser<TestDocWithGuid>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithGuid>()));
            var parser = new SolrQueryResultParser<TestDocWithGuid>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithGuid.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.Key);
        }

        [Test]
        public void SupportsEnumAsInteger() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocWithEnum>(new SolrDocumentResponseParser<TestDocWithEnum>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithEnum>()));
            var parser = new SolrQueryResultParser<TestDocWithEnum>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithEnumAsInt.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.En);
        }

        [Test]
        public void SupportsEnumAsString() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocWithEnum>(new SolrDocumentResponseParser<TestDocWithEnum>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithEnum>()));
            var parser = new SolrQueryResultParser<TestDocWithEnum>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithEnumAsString.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.En);
        }

        [Test]
        public void GenericDictionary_string_string() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict>(new SolrDocumentResponseParser<TestDocWithGenDict>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithGenDict>()));
            var parser = new SolrQueryResultParser<TestDocWithGenDict>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDict.xml"));
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
            var innerParser = new ResultsResponseParser<TestDocWithGenDict2>(new SolrDocumentResponseParser<TestDocWithGenDict2>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithGenDict2>()));
            var parser = new SolrQueryResultParser<TestDocWithGenDict2>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDict.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Assert.IsNotNull(doc.Dict);
            Assert.AreEqual(2, doc.Dict.Count);
            Assert.AreEqual(1, doc.Dict["One"]);
            Assert.AreEqual(2, doc.Dict["Two"]);
        }

        [Test]
        public void GenericDictionary_string_float() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict3>(new SolrDocumentResponseParser<TestDocWithGenDict3>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithGenDict3>()));
            var parser = new SolrQueryResultParser<TestDocWithGenDict3>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDictFloat.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Assert.IsNotNull(doc.Dict);
            Assert.AreEqual(2, doc.Dict.Count);
            Assert.AreEqual(1.45f, doc.Dict["One"]);
            Assert.AreEqual(2.234f, doc.Dict["Two"]);
        }

        [Test]
        public void GenericDictionary_string_decimal() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict4>(new SolrDocumentResponseParser<TestDocWithGenDict4>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithGenDict4>()));
            var parser = new SolrQueryResultParser<TestDocWithGenDict4>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDictFloat.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Assert.IsNotNull(doc.Dict);
            Assert.AreEqual(2, doc.Dict.Count);
            Assert.AreEqual(1.45m, doc.Dict["One"]);
            Assert.AreEqual(2.234m, doc.Dict["Two"]);
        }

        [Test]
        public void GenericDictionary_rest_of_fields() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict5>(new SolrDocumentResponseParser<TestDocWithGenDict5>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithGenDict5>()));
            var parser = new SolrQueryResultParser<TestDocWithGenDict5>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDictFloat.xml"));
            Assert.AreEqual("1.45", results[0].DictOne);
            Assert.IsNotNull(results[0].Dict);
            Assert.AreEqual(4, results[0].Dict.Count);
            Assert.AreEqual("2.234", results[0].Dict["DictTwo"]);
            Assert.AreEqual(new DateTime(1, 1, 1), results[0].Dict["timestamp"]);
            Assert.AreEqual(92.0f, results[0].Dict["price"]);
            Assert.IsInstanceOfType(typeof(ICollection), results[0].Dict["features"]);
        }

		[Test]
		public void WrongFieldDoesntThrow() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocumentWithDate>(new SolrDocumentResponseParser<TestDocumentWithDate>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocumentWithDate>()));
            var parser = new SolrQueryResultParser<TestDocumentWithDate>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArraysSimple.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
		}

		[Test]
		public void ReadsMaxScoreAttribute() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocumentWithArrays4>(new SolrDocumentResponseParser<TestDocumentWithArrays4>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocumentWithArrays4>()));
            var parser = new SolrQueryResultParser<TestDocumentWithArrays4>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArraysSimple.xml"));
			Assert.AreEqual(1.6578954, results.MaxScore);
		}

		[Test]
		public void ReadMaxScore_doesnt_crash_if_not_present() {
            var mapper = new AttributesMappingManager();
            var innerParser = new ResultsResponseParser<TestDocument>(new SolrDocumentResponseParser<TestDocument>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocument>()));
            var parser = new SolrQueryResultParser<TestDocument>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml"));
			Assert.IsNull(results.MaxScore);
		}

        private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

        public void ProfileTest(ProfilingContainer container) {
            var parser = container.Resolve<ISolrQueryResultParser<TestDocumentWithArrays>>();
            var xml = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArraysSimple.xml");

            for (var i = 0; i < 1000; i++) {
                parser.Parse(xml);
            }

            var profile = Flatten(container.GetProfile());
            var q = from n in profile
                    group n.Value by n.Key into x
                    let kv = new { method = x.Key, count = x.Count(), total = x.Sum(t => t.TotalMilliseconds)}
                    orderby kv.total descending
                    select kv;

            foreach (var i in q)
                Console.WriteLine("{0} {1}: {2} executions, {3}ms", i.method.DeclaringType, i.method, i.count, i.total);

        }

        public IEnumerable<KeyValuePair<MethodInfo, TimeSpan>> Flatten(Node<KeyValuePair<MethodInfo, TimeSpan>> n) {
            if (n.Value.Key != null)
                yield return n.Value;
            foreach (var i in n.Children.SelectMany(c => Flatten(c)))
                yield return i;
        }


        [Test]
        [Ignore("Performance test, potentially slow")]
        public void Performance_MemoizeMapping() {
            var container = new ProfilingContainer();
            container.AddComponent<ISolrDocumentResponseParser<TestDocumentWithArrays>, SolrDocumentResponseParser<TestDocumentWithArrays>>();
            container.AddComponent<ISolrResponseParser<TestDocumentWithArrays>, ResultsResponseParser<TestDocumentWithArrays>>("resultsParser");
            container.Register(Component.For<IReadOnlyMappingManager>().ImplementedBy<MemoizingMappingManager>()
                .ServiceOverrides(ServiceOverride.ForKey("mapper").Eq("att")));
            container.AddComponent<IReadOnlyMappingManager, AttributesMappingManager>("att");
            container.Register(Component.For<ISolrQueryResultParser<TestDocumentWithArrays>>().ImplementedBy<SolrQueryResultParser<TestDocumentWithArrays>>()
                .ServiceOverrides(ServiceOverride.ForKey("parsers").Eq(new[] { "resultsParser" })));
            container.AddComponent<ISolrFieldParser, DefaultFieldParser>();
            container.AddComponent<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            ProfileTest(container);
            
        }

		[Test]
		[Ignore("Performance test, potentially slow")]
		public void Performance() {
		    var container = new ProfilingContainer();
            container.AddComponent<ISolrDocumentResponseParser<TestDocumentWithArrays>, SolrDocumentResponseParser<TestDocumentWithArrays>>();
            container.AddComponent<ISolrResponseParser<TestDocumentWithArrays>, ResultsResponseParser<TestDocumentWithArrays>>("resultsParser");
            container.AddComponent<IReadOnlyMappingManager, AttributesMappingManager>();
            container.Register(Component.For<ISolrQueryResultParser<TestDocumentWithArrays>>().ImplementedBy<SolrQueryResultParser<TestDocumentWithArrays>>()
                .ServiceOverrides(ServiceOverride.ForKey("parsers").Eq(new[] { "resultsParser" })));
            container.AddComponent<ISolrFieldParser, DefaultFieldParser>();
            container.AddComponent<ISolrDocumentPropertyVisitor, DefaultDocumentVisitor>();
            ProfileTest(container);

		}

		[Test]
		public void ParseFacetResults() {
		    var innerParser = new FacetsResponseParser<TestDocumentWithArrays>();
            var parser = new SolrQueryResultParser<TestDocumentWithArrays>(new[] { innerParser });
			var r = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithFacet.xml"));
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
		    var parser = new HeaderResponseParser<TestDocument>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
			var docNode = xml.SelectSingleNode("response/lst[@name='responseHeader']");
			var header = parser.ParseHeader(docNode);
			Assert.AreEqual(1, header.Status);
			Assert.AreEqual(15, header.QTime);
			Assert.AreEqual(2, header.Params.Count);
			Assert.AreEqual("id:123456", header.Params["q"]);
			Assert.AreEqual("2.2", header.Params["version"]);
		}

        private IDictionary<string, IDictionary<string, ICollection<string>>> ParseHighlightingResults(string rawXml) {
            var mapper = new AttributesMappingManager();
            var parser = new HighlightingResponseParser<Product>();
            var xml = new XmlDocument();
            xml.LoadXml(rawXml);
            var docNode = xml.SelectSingleNode("response/lst[@name='highlighting']");
            var item = new Product { Id = "SP2514N" };
            return parser.ParseHighlighting(new SolrQueryResults<Product> { item }, docNode);
        }

		[Test]
		public void ParseHighlighting() {
		    var highlights = ParseHighlightingResults(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithHighlighting.xml"));
			Assert.AreEqual(1, highlights.Count);
			var kv = highlights.First().Value;
			Assert.AreEqual(1, kv.Count);
			Assert.AreEqual("features", kv.First().Key);
            Assert.AreEqual(1, kv.First().Value.Count);
            //Console.WriteLine(kv.First().Value.First());
            Assert.Like(kv.First().Value.First(), "Noise");
		}

        [Test]
        public void ParseHighlighting2() {
            var highlights = ParseHighlightingResults(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithHighlighting2.xml"));
            var first = highlights.First();
            first.Value.Keys.ToList().ForEach(Console.WriteLine);
            first.Value["source_en"].ToList().ForEach(Console.WriteLine);
            Assert.AreEqual(3, first.Value["source_en"].Count);
        }

        [Test]
        public void ParseSpellChecking() {
            var parser = new SpellCheckResponseParser<Product>();
            var xml = new XmlDocument();
            xml.LoadXml(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithSpellChecking.xml"));
            var docNode = xml.SelectSingleNode("response/lst[@name='spellcheck']");
            var spellChecking = parser.ParseSpellChecking(docNode);
            Assert.IsNotNull(spellChecking);
            Assert.AreEqual("dell ultrasharp", spellChecking.Collation);
            Assert.AreEqual(2, spellChecking.Count);
        }

        [Test]
        public void ParseMoreLikeThis() {
            var mapper = new AttributesMappingManager();
            var parser = new MoreLikeThisResponseParser<Product>(new SolrDocumentResponseParser<Product>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Product>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithMoreLikeThis.xml");
            var docNode = xml.SelectSingleNode("response/lst[@name='moreLikeThis']");
            var product1 = new Product { Id = "UTF8TEST" };
            var product2 = new Product { Id = "SOLR1000" };
            var mlt = parser.ParseMoreLikeThis(new[] {
                product1,
                product2,
            }, docNode);
            Assert.IsNotNull(mlt);
            Assert.AreEqual(2, mlt.Count);
            Assert.IsTrue(mlt.ContainsKey(product1.Id));
            Assert.IsTrue(mlt.ContainsKey(product2.Id));
            Assert.AreEqual(1, mlt[product1.Id].Count);
            Assert.AreEqual(1, mlt[product2.Id].Count);
            Console.WriteLine(mlt[product1.Id][0].Id);
        }

        [Test]
        public void ParseStatsResults() {
            var parser = new StatsResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithStats.xml");
            var docNode = xml.SelectSingleNode("response/lst[@name='stats']");
            var stats = parser.ParseStats(docNode, "stats_fields");
            Assert.AreEqual(1, stats.Count);
            Assert.IsTrue(stats.ContainsKey("price"));
            var priceStats = stats["price"];
            Assert.AreEqual(0.0, priceStats.Min);
            Assert.AreEqual(2199.0, priceStats.Max);
            Assert.AreEqual(5251.2699999999995, priceStats.Sum);
            Assert.AreEqual(15, priceStats.Count);
            Assert.AreEqual(11, priceStats.Missing);
            Assert.AreEqual(6038619.160300001, priceStats.SumOfSquares);
            Assert.AreEqual(350.08466666666664, priceStats.Mean);
            Assert.AreEqual(547.737557906113, priceStats.StdDev);
            Assert.AreEqual(1, priceStats.FacetResults.Count);
            Assert.IsTrue(priceStats.FacetResults.ContainsKey("inStock"));
            var priceInStockStats = priceStats.FacetResults["inStock"];
            Assert.AreEqual(2, priceInStockStats.Count);
            Assert.IsTrue(priceInStockStats.ContainsKey("true"));
            Assert.IsTrue(priceInStockStats.ContainsKey("false"));
            var priceInStockFalseStats = priceInStockStats["false"];
            Assert.AreEqual(11.5, priceInStockFalseStats.Min);
            Assert.AreEqual(649.99, priceInStockFalseStats.Max);
            Assert.AreEqual(1161.39, priceInStockFalseStats.Sum);
            Assert.AreEqual(4, priceInStockFalseStats.Count);
            Assert.AreEqual(0, priceInStockFalseStats.Missing);
            Assert.AreEqual(653369.2551, priceInStockFalseStats.SumOfSquares);
            Assert.AreEqual(290.3475, priceInStockFalseStats.Mean);
            Assert.AreEqual(324.63444676281654, priceInStockFalseStats.StdDev);
            var priceInStockTrueStats = priceInStockStats["true"];
            Assert.AreEqual(0.0, priceInStockTrueStats.Min);
            Assert.AreEqual(2199.0, priceInStockTrueStats.Max);
            Assert.AreEqual(4089.879999999999, priceInStockTrueStats.Sum);
            Assert.AreEqual(11, priceInStockTrueStats.Count);
            Assert.AreEqual(0, priceInStockTrueStats.Missing);
            Assert.AreEqual(5385249.905200001, priceInStockTrueStats.SumOfSquares);
            Assert.AreEqual(371.8072727272727, priceInStockTrueStats.Mean);
            Assert.AreEqual(621.6592938755265, priceInStockTrueStats.StdDev);
        }

        [Test]
        public void ParseFacetDateResults() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithDateFacet.xml");
            var p = new FacetsResponseParser<Product>();
            var results = p.ParseFacetDates(xml);
            Assert.AreEqual(1, results.Count);
            foreach (var kv in results) {
                Console.WriteLine("date facets for field '{0}'", kv.Key);
                Console.WriteLine("gap: {0}", kv.Value.Gap);
                Console.WriteLine("end: {0}", kv.Value.End);
                foreach (var vv in kv.Value.DateResults) {
                    Console.WriteLine("Facet count for '{0}': {1}", vv.Key, vv.Value);
                }
            }
        }

        [Test]
        public void ParseFacetDateResultsWithOther() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithDateFacetAndOther.xml");
            var p = new FacetsResponseParser<Product>();
            var results = p.ParseFacetDates(xml);
            Assert.AreEqual(1, results.Count);
            foreach (var kv in results) {
                Console.WriteLine("date facets for field '{0}'", kv.Key);
                Console.WriteLine("gap: {0}", kv.Value.Gap);
                Console.WriteLine("end: {0}", kv.Value.End);
                foreach (var vv in kv.Value.DateResults) {
                    Console.WriteLine("Facet count for '{0}': {1}", vv.Key, vv.Value);
                }
                foreach (var vv in kv.Value.OtherResults) {
                    Console.WriteLine("Facet count for '{0}': {1}", vv.Key, vv.Value);
                }
            }
            Assert.AreEqual(1, results["timestamp"].OtherResults[FacetDateOther.Before]);
            Assert.AreEqual(0, results["timestamp"].OtherResults[FacetDateOther.After]);
            Assert.AreEqual(0, results["timestamp"].OtherResults[FacetDateOther.Between]);
        }

        [Test]
        public void PropertyWithoutSetter() {
            var mapper = new AttributesMappingManager();
            var parser = new SolrDocumentResponseParser<TestDocWithoutSetter>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<TestDocWithoutSetter>());
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.SelectSingleNode("response/result/doc");
            var doc = parser.ParseDocument(docNode, mapper.GetFields(typeof(TestDocument)));
            Assert.IsNotNull(doc);
            Assert.AreEqual(0, doc.Id);
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