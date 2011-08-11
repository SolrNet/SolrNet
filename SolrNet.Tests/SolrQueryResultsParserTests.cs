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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
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
using Castle.Facilities.SolrNetIntegration;

namespace SolrNet.Tests {
	[TestFixture]
	public partial class SolrQueryResultsParserTests {

        private SolrDocumentResponseParser<T> GetDocumentParser<T>() {
            var mapper = new AttributesMappingManager();
            return GetDocumentParser<T>(mapper);
        }

        private SolrDocumentResponseParser<T> GetDocumentParser<T>(IReadOnlyMappingManager mapper) {
            return new SolrDocumentResponseParser<T>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<T>());
        }

		[Test]
		public void ParseDocument() {
            var parser = GetDocumentParser<TestDocument>();
		    var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/result/doc");
			var doc = parser.ParseDocument(docNode);
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
		}

        [Test]
	    public void ParseDocumentWithMappingManager() {
            var mapper = new MappingManager();
            mapper.Add(typeof(TestDocumentWithoutAttributes).GetProperty("Id"), "id");
            var parser = GetDocumentParser<TestDocumentWithoutAttributes>(mapper);
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/result/doc");
            var doc = parser.ParseDocument(docNode);
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
	    }

		[Test]
		public void NumFound() {
            var docParser = GetDocumentParser<TestDocument>();
            var innerParser = new ResultsResponseParser<TestDocument>(docParser);
            var parser = new SolrQueryResultParser<TestDocument>(new[] {innerParser});
            var r = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml"));
			Assert.AreEqual(1, r.NumFound);
		}

		[Test]
		public void Parse() {
            var docParser = GetDocumentParser<TestDocument>();
            var innerParser = new ResultsResponseParser<TestDocument>(docParser);
            var parser = new SolrQueryResultParser<TestDocument>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(123456, doc.Id);
		}

		[Test]
		public void SetPropertyWithArrayOfStrings() {
		    var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='cat']");
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
            var fieldNode = xml.XPathSelectElement("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
            visitor.Visit(doc, "price", fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyNullableDouble() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithNullableDouble();
            visitor.Visit(doc, "price", fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyWithIntCollection() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='numbers']");
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
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='numbers']");
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
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='numbers']");
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
            var docParser = GetDocumentParser<TestDocumentWithArrays>();
            var innerParser = new ResultsResponseParser<TestDocumentWithArrays>(docParser);
            var parser = new SolrQueryResultParser<TestDocumentWithArrays>(new[] { innerParser });
			var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArrays.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual("SP2514N", doc.Id);
		}

		[Test]
		public void SupportsDateTime() {
            var docParser = GetDocumentParser<TestDocumentWithDate>();
            var innerParser = new ResultsResponseParser<TestDocumentWithDate>(docParser);
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
            var docParser = GetDocumentParser<TestDocumentWithNullableDate>();
            var innerParser = new ResultsResponseParser<TestDocumentWithNullableDate>(docParser);
            var parser = new SolrQueryResultParser<TestDocumentWithNullableDate>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithDate.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
		}

		[Test]
		public void SupportsIEnumerable() {
            var docParser = GetDocumentParser<TestDocumentWithArrays4>();
            var innerParser = new ResultsResponseParser<TestDocumentWithArrays4>(docParser);
            var parser = new SolrQueryResultParser<TestDocumentWithArrays4>(new[] { innerParser });
			var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArraysSimple.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(2, new List<string>(doc.Features).Count);
		}

        [Test]
        public void SupportsGuid() {
            var docParser = GetDocumentParser<TestDocWithGuid>();
            var innerParser = new ResultsResponseParser<TestDocWithGuid>(docParser);
            var parser = new SolrQueryResultParser<TestDocWithGuid>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithGuid.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.Key);
        }

        [Test]
        public void SupportsEnumAsInteger() {
            var docParser = GetDocumentParser<TestDocWithEnum>();
            var innerParser = new ResultsResponseParser<TestDocWithEnum>(docParser);
            var parser = new SolrQueryResultParser<TestDocWithEnum>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithEnumAsInt.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.En);
        }

        [Test]
        public void SupportsEnumAsString() {
            var docParser = GetDocumentParser<TestDocWithEnum>();
            var innerParser = new ResultsResponseParser<TestDocWithEnum>(docParser);
            var parser = new SolrQueryResultParser<TestDocWithEnum>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithEnumAsString.xml"));
            Assert.AreEqual(1, results.Count);
            var doc = results[0];
            Console.WriteLine(doc.En);
        }

        [Test]
        [ExpectedException(typeof(Exception))]
        public void EmptyEnumThrows() {
            var mapper = new MappingManager();
            mapper.Add(typeof(TestDocWithEnum).GetProperty("En"), "basicview");
            var docParser = GetDocumentParser<TestDocWithEnum>(mapper);
            var innerParser = new ResultsResponseParser<TestDocWithEnum>(docParser);
            var parser = new SolrQueryResultParser<TestDocWithEnum>(new[] { innerParser });
            var xml = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            parser.Parse(xml);
        }

        [Test]
        public void SupportsNullableGuidWithEmptyField() {
            var docParser = GetDocumentParser<TestDocWithNullableEnum>();
            var innerParser = new ResultsResponseParser<TestDocWithNullableEnum>(docParser);
            var parser = new SolrQueryResultParser<TestDocWithNullableEnum>(new[] { innerParser });
            var xml = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.response.xml");
            var results = parser.Parse(xml);
            Assert.AreEqual(1, results.Count);
            Assert.IsFalse(results[0].BasicView.HasValue);
        }

        [Test]
        public void GenericDictionary_string_string() {
            var docParser = GetDocumentParser<TestDocWithGenDict>();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict>(docParser);
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
            var docParser = GetDocumentParser<TestDocWithGenDict2>();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict2>(docParser);
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
            var docParser = GetDocumentParser<TestDocWithGenDict3>();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict3>(docParser);
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
            var docParser = GetDocumentParser<TestDocWithGenDict4>();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict4>(docParser);
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
            var docParser = GetDocumentParser<TestDocWithGenDict5>();
            var innerParser = new ResultsResponseParser<TestDocWithGenDict5>(docParser);
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
            var docParser = GetDocumentParser<TestDocumentWithDate>();
            var innerParser = new ResultsResponseParser<TestDocumentWithDate>(docParser);
            var parser = new SolrQueryResultParser<TestDocumentWithDate>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArraysSimple.xml"));
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
		}

		[Test]
		public void ReadsMaxScoreAttribute() {
            var docParser = GetDocumentParser<TestDocumentWithArrays4>();
            var innerParser = new ResultsResponseParser<TestDocumentWithArrays4>(docParser);
            var parser = new SolrQueryResultParser<TestDocumentWithArrays4>(new[] { innerParser });
            var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithArraysSimple.xml"));
			Assert.AreEqual(1.6578954, results.MaxScore);
		}

		[Test]
		public void ReadMaxScore_doesnt_crash_if_not_present() {
            var docParser = GetDocumentParser<TestDocument>();
            var innerParser = new ResultsResponseParser<TestDocument>(docParser);
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
		public void Performance() {
		    var container = new ProfilingContainer();
            container.AddFacility("solr", new SolrNetFacility("http://localhost"));
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
			Assert.AreEqual(2, r.FacetFields["cat"].First(q => q.Key == "connector").Value);
            Assert.AreEqual(2, r.FacetFields["cat"].First(q => q.Key == "").Value); // facet.missing as empty string

			Assert.IsNotNull(r.FacetQueries);
			Console.WriteLine(r.FacetQueries.Count);
			Assert.AreEqual(1, r.FacetQueries.Count);
		}

		[Test]
		public void ParseResponseHeader() {
		    var parser = new HeaderResponseParser<TestDocument>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='responseHeader']");
			var header = parser.ParseHeader(docNode);
			Assert.AreEqual(1, header.Status);
			Assert.AreEqual(15, header.QTime);
			Assert.AreEqual(2, header.Params.Count);
			Assert.AreEqual("id:123456", header.Params["q"]);
			Assert.AreEqual("2.2", header.Params["version"]);
		}

        [Test]
        public void ExtractResponse() {
            var parser = new ExtractResponseParser(new HeaderResponseParser<TestDocument>());
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithExtractContent.xml");
            var extractResponse = parser.Parse(xml);
            Assert.AreEqual("Hello world!", extractResponse.Content);
        }

        private IDictionary<string, IDictionary<string, ICollection<string>>> ParseHighlightingResults(string rawXml) {
            var mapper = new AttributesMappingManager();
            var parser = new HighlightingResponseParser<Product>();
            var xml = XDocument.Parse(rawXml);
            var docNode = xml.XPathSelectElement("response/lst[@name='highlighting']");
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
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSpellChecking.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='spellcheck']");
            var spellChecking = parser.ParseSpellChecking(docNode);
            Assert.IsNotNull(spellChecking);
            Assert.AreEqual("dell ultrasharp", spellChecking.Collation);
            Assert.AreEqual(2, spellChecking.Count);
        }

        [Test]
        public void ParseClustering() {
            var parser = new ClusterResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithClustering.xml");
            var docNode = xml.XPathSelectElement("response/arr[@name='clusters']");
            var clustering = parser.ParseClusterNode(docNode);
            Assert.IsNotNull(clustering);
            Assert.AreEqual(89, clustering.Clusters.Count());
            Assert.AreEqual("International", clustering.Clusters.First().Label);
            Assert.AreEqual(33.729704170097, clustering.Clusters.First().Score);
            Assert.AreEqual(8, clustering.Clusters.First().Documents.Count());
            Assert.AreEqual("19622040", clustering.Clusters.First().Documents.First());
        }

        [Test]
        public void ParseTerms()
        {
            var parser = new TermsResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithTerms.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='terms']");
            var terms = parser.ParseTerms(docNode);
            Assert.IsNotNull(terms);
            Assert.AreEqual(2, terms.Count);
            Assert.AreEqual("text", terms.First().Field);
            Assert.AreEqual("textgen", terms.ElementAt(1).Field);
            Assert.AreEqual("boot", terms.First().Terms.First().Key);
            Assert.AreEqual(479, terms.First().Terms.First().Value);
            Assert.AreEqual("boots", terms.ElementAt(1).Terms.First().Key);
            Assert.AreEqual(463, terms.ElementAt(1).Terms.First().Value);
        }

        [Test]
        public void ParseMoreLikeThis() {
            var mapper = new AttributesMappingManager();
            var parser = new MoreLikeThisResponseParser<Product>(new SolrDocumentResponseParser<Product>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Product>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithMoreLikeThis.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='moreLikeThis']");
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
            var docNode = xml.XPathSelectElement("response/lst[@name='stats']");
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
            var results = p.ParseFacetDates(xml.Root);
            Assert.AreEqual(1, results.Count);
            var result = results.First();
            Assert.AreEqual("timestamp", result.Key);
            Assert.AreEqual("+1DAY", result.Value.Gap);
            Assert.AreEqual(new DateTime(2009, 8, 10, 0, 33, 46, 578), result.Value.End);
            var dateResults = result.Value.DateResults;
            Assert.AreEqual(1, dateResults.Count);
            Assert.AreEqual(16, dateResults[0].Value);
            Assert.AreEqual(new DateTime(2009, 8, 9, 0, 33, 46, 578), dateResults[0].Key);
        }

        [Test]
        public void ParseFacetDateResultsWithOther() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithDateFacetAndOther.xml");
            var p = new FacetsResponseParser<Product>();
            var results = p.ParseFacetDates(xml.Root);
            Assert.AreEqual(1, results.Count);
            var result = results.First();
            Assert.AreEqual("timestamp", result.Key);
            Assert.AreEqual("+1DAY", result.Value.Gap);
            Assert.AreEqual(new DateTime(2009, 8, 10, 0, 46, 29), result.Value.End);
            Assert.AreEqual(new DateTime(2009, 8, 9, 22, 46, 29), result.Value.DateResults[0].Key);
            var other = result.Value.OtherResults;
            Assert.AreEqual(1, other[FacetDateOther.Before]);
            Assert.AreEqual(0, other[FacetDateOther.After]);
            Assert.AreEqual(0, other[FacetDateOther.Between]);
        }

		[Test]
		public void ParseResultsWithGroups()
		{
			var mapper = new AttributesMappingManager();
			var innerParser = new GroupingResponseParser<Product>(new SolrDocumentResponseParser<Product>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Product>()));
			var parser = new SolrQueryResultParser<Product>(new[] { innerParser });
			var results = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithGroupingOnInstock.xml"));
			Assert.AreEqual(1, results.Grouping.Count);
			Assert.AreEqual(2, results.Grouping["inStock"].Groups.Count());
			Assert.AreEqual(13, results.Grouping["inStock"].Groups.First().NumFound);
		}

		[Test]
		public void ParseResultsWithFacetPivot()
		{
			var innerParser = new FacetsResponseParser<Product>();
			var parser = new SolrQueryResultParser<Product>(new[] { innerParser });
			var r = parser.Parse(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithFacetPivoting.xml"));
			Assert.IsNotNull(r.FacetPivots);
			Console.WriteLine(r.FacetPivots.Count);
			Assert.IsTrue(r.FacetPivots.ContainsKey("inStock,manu"));

			Assert.AreEqual(2, r.FacetPivots["inStock,manu"].Count);
			Assert.AreEqual("inStock", r.FacetPivots["inStock,manu"][0].Field);
			Assert.AreEqual(10, r.FacetPivots["inStock,manu"][0].ChildPivots.Count); 

		}

        [Test]
        public void PropertyWithoutSetter() {
            var parser = GetDocumentParser<TestDocWithoutSetter>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/result/doc");
            var doc = parser.ParseDocument(docNode);
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

        public class TestDocWithNullableEnum {
            [SolrField("basicview")]
            public AEnum? BasicView { get; set; }
        }
	}
}