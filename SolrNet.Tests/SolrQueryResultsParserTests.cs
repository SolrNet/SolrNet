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
using System.Xml.Linq;
using System.Xml.XPath;
using Xunit;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;
using Castle.Facilities.SolrNetIntegration;

namespace SolrNet.Tests
{

    public partial class SolrQueryResultsParserTests
    {

        private static SolrDocumentResponseParser<T> GetDocumentParser<T>()
        {
            var mapper = new AttributesMappingManager();
            return GetDocumentParser<T>(mapper);
        }

        private static SolrDocumentResponseParser<T> GetDocumentParser<T>(IReadOnlyMappingManager mapper)
        {
            return new SolrDocumentResponseParser<T>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<T>());
        }

        [Fact]
        public void ParseDocument()
        {
            var parser = GetDocumentParser<TestDocument>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/result/doc");
            var doc = parser.ParseDocument(docNode);
            Assert.NotNull(doc);
            Assert.Equal(123456, doc.Id);
        }

        [Fact]
        public void ParseDocumentWithMappingManager()
        {
            var mapper = new MappingManager();
            mapper.Add(typeof(TestDocumentWithoutAttributes).GetProperty("Id"), "id");
            var parser = GetDocumentParser<TestDocumentWithoutAttributes>(mapper);
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/result/doc");
            var doc = parser.ParseDocument(docNode);
            Assert.NotNull(doc);
            Assert.Equal(123456, doc.Id);
        }

        private static SolrQueryResults<T> ParseFromResource<T>(string xmlResource)
        {
            var docParser = GetDocumentParser<T>();
            var parser = new ResultsResponseParser<T>(docParser);
            var r = new SolrQueryResults<T>();
            var xml = EmbeddedResource.GetEmbeddedXml(typeof(SolrQueryResultsParserTests), xmlResource);
            parser.Parse(xml, r);
            return r;
        }

        [Fact]
        public void NumFound()
        {
            var r = ParseFromResource<TestDocument>("Resources.response.xml");
            Assert.Equal(1, r.NumFound);
        }

        [Fact]
        public void CanParseNextCursormark()
        {
            var r = ParseFromResource<TestDocument>("Resources.response.xml");
            Assert.Equal(new StartOrCursor.Cursor("AoEoZTQ3YmY0NDM="), r.NextCursorMark);
        }

        [Fact]
        public void Parse()
        {
            var results = ParseFromResource<TestDocument>("Resources.response.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.Equal(123456, doc.Id);
        }

        [Fact]
        public void SetPropertyWithArrayOfStrings()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='cat']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
            visitor.Visit(doc, "cat", fieldNode);
            Assert.Equal(2, doc.Cat.Count);
            var cats = new List<string>(doc.Cat);
            Assert.Equal("electronics", cats[0]);
            Assert.Equal("hard drive", cats[1]);
        }

        [Fact]
        public void SetPropertyDouble()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
            visitor.Visit(doc, "price", fieldNode);
            Assert.Equal(92d, doc.Price);
        }

        [Fact]
        public void SetPropertyNullableDouble()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/float[@name='price']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithNullableDouble();
            visitor.Visit(doc, "price", fieldNode);
            Assert.Equal(92d, doc.Price);
        }

        [Fact]
        public void SetPropertyWithIntCollection()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='numbers']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays();
            visitor.Visit(doc, "numbers", fieldNode);
            Assert.Equal(2, doc.Numbers.Count);
            var numbers = new List<int>(doc.Numbers);
            Assert.Equal(1, numbers[0]);
            Assert.Equal(2, numbers[1]);
        }

        [Fact]
        public void SetPropertyWithNonGenericCollection()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='numbers']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays3();
            visitor.Visit(doc, "numbers", fieldNode);
            Assert.Equal(2, doc.Numbers.Count);
            var numbers = new ArrayList(doc.Numbers);
            Assert.Equal(1, numbers[0]);
            Assert.Equal(2, numbers[1]);
        }

        [Fact]
        public void SetPropertyWithArrayOfIntsToIntArray()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArrays.xml");
            var fieldNode = xml.XPathSelectElement("response/result/doc/arr[@name='numbers']");
            var mapper = new AttributesMappingManager();
            var visitor = new DefaultDocumentVisitor(mapper, new DefaultFieldParser());
            var doc = new TestDocumentWithArrays2();
            visitor.Visit(doc, "numbers", fieldNode);
            Assert.Equal(2, doc.Numbers.Length);
            var numbers = new List<int>(doc.Numbers);
            Assert.Equal(1, numbers[0]);
            Assert.Equal(2, numbers[1]);
        }

        [Fact]
        public void ParseResultsWithArrays()
        {
            var results = ParseFromResource<TestDocumentWithArrays>("Resources.responseWithArrays.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.Equal("SP2514N", doc.Id);
        }

        [Fact]
        public void SupportsDateTime()
        {
            var results = ParseFromResource<TestDocumentWithDate>("Resources.responseWithDate.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.Equal(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
        }

        [Fact]
        public void ParseDate_without_milliseconds()
        {
            var dt = DateTimeFieldParser.ParseDate("2001-01-02T03:04:05Z");
            Assert.Equal(new DateTime(2001, 1, 2, 3, 4, 5), dt);
        }

        [Fact]
        public void ParseDate_with_milliseconds()
        {
            var dt = DateTimeFieldParser.ParseDate("2001-01-02T03:04:05.245Z");
            Assert.Equal(new DateTime(2001, 1, 2, 3, 4, 5, 245), dt);
        }

        [Fact]
        public void SupportsNullableDateTime()
        {
            var results = ParseFromResource<TestDocumentWithNullableDate>("Resources.responseWithDate.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.Equal(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
        }

        [Fact]
        public void SupportsIEnumerable()
        {
            var results = ParseFromResource<TestDocumentWithArrays4>("Resources.responseWithArraysSimple.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.Equal(2, new List<string>(doc.Features).Count);
        }

        [Fact]
        public void SupportsGuid()
        {
            var results = ParseFromResource<TestDocWithGuid>("Resources.responseWithGuid.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            //Console.WriteLine(doc.Key);
        }

        [Fact]
        public void SupportsEnumAsInteger()
        {
            var results = ParseFromResource<TestDocWithEnum>("Resources.responseWithEnumAsInt.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            //Console.WriteLine(doc.En);
        }

        [Fact]
        public void SupportsEnumAsString()
        {
            var results = ParseFromResource<TestDocWithEnum>("Resources.responseWithEnumAsString.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            //Console.WriteLine(doc.En);
        }

        [Fact]
        public void EmptyEnumThrows()
        {
            var mapper = new MappingManager();
            mapper.Add(typeof(TestDocWithEnum).GetProperty("En"), "basicview");
            var docParser = GetDocumentParser<TestDocWithEnum>(mapper);
            var parser = new ResultsResponseParser<TestDocWithEnum>(docParser);
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var results = new SolrQueryResults<TestDocWithEnum>();
            Assert.Throws<Exception>(() => parser.Parse(xml, results));
        }

        [Fact]
        public void SupportsNullableGuidWithEmptyField()
        {
            var results = ParseFromResource<TestDocWithNullableEnum>("Resources.response.xml");
            Assert.Equal(1, results.Count);
            Assert.False(results[0].BasicView.HasValue);
        }

        [Fact]
        public void GenericDictionary_string_string()
        {
            var results = ParseFromResource<TestDocWithGenDict>("Resources.responseWithDict.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.NotNull(doc.Dict);
            Assert.Equal(2, doc.Dict.Count);
            Assert.Equal("1", doc.Dict["One"]);
            Assert.Equal("2", doc.Dict["Two"]);
        }

        [Fact]
        public void GenericDictionary_string_int()
        {
            var results = ParseFromResource<TestDocWithGenDict2>("Resources.responseWithDict.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.NotNull(doc.Dict);
            Assert.Equal(2, doc.Dict.Count);
            Assert.Equal(1, doc.Dict["One"]);
            Assert.Equal(2, doc.Dict["Two"]);
        }

        [Fact]
        public void GenericDictionary_string_float()
        {
            var results = ParseFromResource<TestDocWithGenDict3>("Resources.responseWithDictFloat.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.NotNull(doc.Dict);
            Assert.Equal(2, doc.Dict.Count);
            Assert.Equal(1.45f, doc.Dict["One"]);
            Assert.Equal(2.234f, doc.Dict["Two"]);
        }

        [Fact]
        public void GenericDictionary_string_decimal()
        {
            var results = ParseFromResource<TestDocWithGenDict4>("Resources.responseWithDictFloat.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
            Assert.NotNull(doc.Dict);
            Assert.Equal(2, doc.Dict.Count);
            Assert.Equal(1.45m, doc.Dict["One"]);
            Assert.Equal(2.234m, doc.Dict["Two"]);
        }

        [Fact]
        public void GenericDictionary_rest_of_fields()
        {
            var results = ParseFromResource<TestDocWithGenDict5>("Resources.responseWithDictFloat.xml");
            Assert.Equal("1.45", results[0].DictOne);
            Assert.NotNull(results[0].Dict);
            Assert.Equal(4, results[0].Dict.Count);
            Assert.Equal("2.234", results[0].Dict["DictTwo"]);
            Assert.Equal(new DateTime(1, 1, 1), results[0].Dict["timestamp"]);
            Assert.Equal(92.0f, results[0].Dict["price"]);
            Assert.IsAssignableFrom(typeof(ICollection), results[0].Dict["features"]);
        }

        [Fact]
        public void WrongFieldDoesntThrow()
        {
            var results = ParseFromResource<TestDocumentWithDate>("Resources.responseWithArraysSimple.xml");
            Assert.Equal(1, results.Count);
            var doc = results[0];
        }

        [Fact]
        public void ReadsMaxScoreAttribute()
        {
            var results = ParseFromResource<TestDocumentWithArrays4>("Resources.responseWithArraysSimple.xml");
            Assert.Equal(1.6578954, results.MaxScore);
        }

        [Fact]
        public void ReadMaxScore_doesnt_crash_if_not_present()
        {
            var results = ParseFromResource<TestDocument>("Resources.response.xml");
            Assert.Null(results.MaxScore);
        }

        public void ProfileTest(ProfilingContainer container)
        {
            var parser = container.Resolve<ISolrAbstractResponseParser<TestDocumentWithArrays>>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithArraysSimple.xml");
            var results = new SolrQueryResults<TestDocumentWithArrays>();

            for (var i = 0; i < 1000; i++)
            {
                parser.Parse(xml, results);
            }

            var profile = Flatten(container.GetProfile());
            var q = from n in profile
                    group n.Value by n.Key into x
                    let kv = new { method = x.Key, count = x.Count(), total = x.Sum(t => t.TotalMilliseconds) }
                    orderby kv.total descending
                    select kv;

            //foreach (var i in q)
            //    Console.WriteLine("{0} {1}: {2} executions, {3}ms", i.method.DeclaringType, i.method, i.count, i.total);

        }

        public IEnumerable<KeyValuePair<MethodInfo, TimeSpan>> Flatten(Node<KeyValuePair<MethodInfo, TimeSpan>> n)
        {
            if (n.Value.Key != null)
                yield return n.Value;
            foreach (var i in n.Children.SelectMany(Flatten))
                yield return i;
        }


        [Fact(Skip = "Performance test, potentially slow")]
        public void Performance()
        {
            var container = new ProfilingContainer();
            container.AddFacility("solr", new SolrNetFacility("http://localhost"));
            ProfileTest(container);
        }

        [Fact]
        public void ParseFacetResults()
        {
            var parser = new FacetsResponseParser<TestDocumentWithArrays>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithFacet.xml");
            var r = new SolrQueryResults<TestDocumentWithArrays>();
            parser.Parse(xml, r);
            Assert.NotNull(r.FacetFields);
            //Console.WriteLine(r.FacetFields.Count);
            Assert.True(r.FacetFields.ContainsKey("cat"));
            Assert.True(r.FacetFields.ContainsKey("inStock"));
            Assert.Equal(2, r.FacetFields["cat"].First(q => q.Key == "connector").Value);
            Assert.Equal(2, r.FacetFields["cat"].First(q => q.Key == "").Value); // facet.missing as empty string


            //Facet Ranges
            Assert.NotNull(r.FacetRanges);
            Assert.Equal(r.FacetRanges.Count, 2);
            Assert.Equal(r.FacetRanges.First().Key , "date-timestamp");
            Assert.Equal(r.FacetRanges.First().Value.Start, "2017-07-30T00:00:00Z");
            Assert.Equal(r.FacetRanges.First().Value.End, "2017-08-30T00:00:00Z");
            Assert.Equal(r.FacetRanges.First().Value.Gap, "+1DAY");
            Assert.Equal(r.FacetRanges.First().Value.OtherResults[FacetRangeOther.Before], 41622120);
            Assert.Equal(r.FacetRanges.First().Value.OtherResults[FacetRangeOther.After], 47336);
            Assert.Equal(r.FacetRanges.First().Value.OtherResults[FacetRangeOther.Between], 75812);
            Assert.Equal(r.FacetRanges.First().Value.RangeResults.Count,31);
            Assert.Equal(r.FacetRanges.First().Value.RangeResults.First().Key, "2017-07-30T00:00:00Z");
            Assert.Equal(r.FacetRanges.First().Value.RangeResults.First().Value, 222);
            Assert.Equal(r.FacetRanges.First().Value.RangeResults.Last().Key, "2017-08-29T00:00:00Z");
            Assert.Equal(r.FacetRanges.First().Value.RangeResults.Last().Value, 20);

            Assert.Equal(r.FacetRanges.Last().Key, "version");
            Assert.Equal(r.FacetRanges.Last().Value.Gap, "1000");
            Assert.Equal(r.FacetRanges.Last().Value.RangeResults.First().Key, "1531035549990449850");
            Assert.Equal(r.FacetRanges.Last().Value.RangeResults.First().Value, 20);
            Assert.Equal(r.FacetRanges.Last().Value.RangeResults.Last().Key, "1531035549990659850");
            Assert.Equal(r.FacetRanges.Last().Value.RangeResults.Last().Value, 0);


            //Facet Intervals 
            Assert.NotNull(r.FacetIntervals);
            Assert.Equal(r.FacetIntervals.Count, 2);
            Assert.Equal(r.FacetIntervals.First().Key, "letters");
            Assert.Equal(r.FacetIntervals.First().Value.Count, 3);
            Assert.Equal(r.FacetIntervals.First().Value.First().Key , "[*,b]");
            Assert.Equal(r.FacetIntervals.First().Value.First().Value , 5);
            Assert.Equal(r.FacetIntervals.First().Value.Last().Key, "bar");
            Assert.Equal(r.FacetIntervals.First().Value.Last().Value, 4544341);


            Assert.Equal(r.FacetIntervals.Last().Key, "number");
            Assert.Equal(r.FacetIntervals.Last().Value.Count, 2);
            Assert.Equal(r.FacetIntervals.Last().Value.First().Key, "[0,500]");
            Assert.Equal(r.FacetIntervals.Last().Value.First().Value, 9);
            Assert.Equal(r.FacetIntervals.Last().Value.Last().Key, "[500,1000]");
            Assert.Equal(r.FacetIntervals.Last().Value.Last().Value, 123);




            //Facet Queries
            Assert.NotNull(r.FacetQueries);
            //Console.WriteLine(r.FacetQueries.Count);
            Assert.Equal(1, r.FacetQueries.Count);
        }

        [Fact]
        public void ParseResponseHeader()
        {
            var parser = new HeaderResponseParser<TestDocument>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='responseHeader']");
            var header = parser.ParseHeader(docNode);
            Assert.Equal(1, header.Status);
            Assert.Equal(15, header.QTime);
            Assert.Equal(2, header.Params.Count);
            Assert.Equal("id:123456", header.Params["q"]);
            Assert.Equal("2.2", header.Params["version"]);
        }

        [Fact]
        public void ExtractResponse()
        {
            var parser = new ExtractResponseParser(new HeaderResponseParser<TestDocument>());
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithExtractContent.xml");
            var extractResponse = parser.Parse(xml);
            Assert.Equal("Hello world!", extractResponse.Content);
        }

        private static IDictionary<string, HighlightedSnippets> ParseHighlightingResults(string rawXml)
        {
            var xml = XDocument.Parse(rawXml);
            var docNode = xml.XPathSelectElement("response/lst[@name='highlighting']");
            var item = new Product { Id = "SP2514N" };
            return HighlightingResponseParser<Product>.ParseHighlighting(new SolrQueryResults<Product> { item }, docNode);
        }

        [Fact]
        public void ParseHighlighting()
        {
            var highlights = ParseHighlightingResults(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithHighlighting.xml"));
            Assert.Equal(1, highlights.Count);
            var kv = highlights.First().Value;
            Assert.Equal(1, kv.Count);
            Assert.Equal("features", kv.First().Key);
            Assert.Equal(1, kv.First().Value.Count);
            //Console.WriteLine(kv.First().Value.First());
            Assert.StartsWith("<em>Noise</em>", kv.First().Value.First(), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ParseHighlightingWrappedWithClass()
        {
            var highlights = ParseHighlightingResults(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithHighlighting.xml"));
            Assert.Equal(1, highlights.Count);
            var first = highlights.First();
            Assert.Equal("SP2514N", first.Key);
            var fieldsWithSnippets = highlights["SP2514N"].Snippets;
            Assert.Equal(1, fieldsWithSnippets.Count);
            Assert.Equal("features", fieldsWithSnippets.First().Key);
            var snippets = highlights["SP2514N"].Snippets["features"];
            Assert.Equal(1, snippets.Count);
            Assert.StartsWith("<em>Noise</em>", snippets.First(),StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ParseHighlighting2()
        {
            var highlights = ParseHighlightingResults(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithHighlighting2.xml"));
            var first = highlights.First();
            //first.Value.Keys.ToList().ForEach(Console.WriteLine);
            //first.Value["source_en"].ToList().ForEach(Console.WriteLine);
            Assert.Equal(3, first.Value["source_en"].Count);
        }

        [Fact]
        public void ParseHighlighting2WrappedWithClass()
        {
            var highlights = ParseHighlightingResults(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithHighlighting2.xml"));
            var first = highlights.First();
            //foreach (var i in first.Value.Snippets.Keys)
            //    Console.WriteLine(i);
            //foreach (var i in first.Value.Snippets["source_en"])
            //    Console.WriteLine(i);
            Assert.Equal(3, first.Value.Snippets["source_en"].Count);
        }

        [Fact]
        public void ParseHighlighting3()
        {
            var highlights = ParseHighlightingResults(EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithHighlighting3.xml"));
            Assert.Equal(0, highlights["e4420cc2"].Count);
            Assert.Equal(1, highlights["e442c4cd"].Count);
            Assert.Equal(1, highlights["e442c4cd"]["bodytext"].Count);
            Assert.Contains("Garia lancerer", highlights["e442c4cd"]["bodytext"].First());
        }

        [Fact]
        public void ParseSpellChecking()
        {
            var parser = new SpellCheckResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSpellChecking.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='spellcheck']");
            var spellChecking = parser.ParseSpellChecking(docNode);
            Assert.NotNull(spellChecking);
            Assert.Equal("dell ultrasharp", spellChecking.Collation);
            Assert.Equal(2, spellChecking.Count);
        }

        [Fact]
        public void ParseSpellCheckingCollateTrueInSuggestions()
        {
            // Multiple collation nodes are included in suggestions in Solr 4.x, included for backward compatibility
            var parser = new SpellCheckResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSpellCheckingCollationInSuggestions.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='spellcheck']");
            var spellChecking = parser.ParseSpellChecking(docNode);
            Assert.NotNull(spellChecking);
            Assert.Equal("audit", spellChecking.Collation);
            Assert.Equal(2, spellChecking.Count);
            Assert.Equal(2, spellChecking.Collations.Count);
        }

        [Fact]
        public void ParseSpellCheckingCollateTrueInExpandedCollations()
        {
            //Collations node now separates from collation nodes from suggestions
            //Result when spellcheck.extendedResults=true and spellcheck.collateExtendedResults=true.
            var parser = new SpellCheckResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSpellCheckingExpandedCollationInCollations.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='spellcheck']");
            var spellChecking = parser.ParseSpellChecking(docNode);
            Assert.NotNull(spellChecking);
            Assert.Equal("dell ultrasharp", spellChecking.Collation);
            Assert.Equal(2, spellChecking.Count);
            Assert.Equal(1, spellChecking.First().Suggestions.Count);
            Assert.Equal("dell", spellChecking.First().Suggestions.First());
            Assert.Equal(1, spellChecking.Last().Suggestions.Count);
            Assert.Equal("ultrasharp", spellChecking.Last().Suggestions.First());
            Assert.Equal(1, spellChecking.Collations.Count);
        }

        [Fact]
        public void ParseSpellCheckingCollateTrueInCollations()
        {
            //Collations node now separates from collation nodes from suggestions
            //Result when spellcheck.extendedResults=false and spellcheck.collateExtendedResults=false.
            var parser = new SpellCheckResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithSpellCheckingCollationInCollations.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='spellcheck']");
            var spellChecking = parser.ParseSpellChecking(docNode);
            Assert.NotNull(spellChecking);
            Assert.Equal("dell maxtor", spellChecking.Collation);
            Assert.Equal(2, spellChecking.Count);
            Assert.Equal(1, spellChecking.First().Suggestions.Count);
            Assert.Equal("dell", spellChecking.First().Suggestions.First());
            Assert.Equal(2, spellChecking.Last().Suggestions.Count);
            Assert.Equal("maxtor", spellChecking.Last().Suggestions.First());
            Assert.Equal("motor", spellChecking.Last().Suggestions.Last());
            Assert.Equal(2, spellChecking.Collations.Count);
        }

        [Fact]
        public void ParseClustering()
        {
            var parser = new ClusterResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithClustering.xml");
            var docNode = xml.XPathSelectElement("response/arr[@name='clusters']");
            var clustering = parser.ParseClusterNode(docNode);
            Assert.NotNull(clustering);
            Assert.Equal(89, clustering.Clusters.Count());
            Assert.Equal("International", clustering.Clusters.First().Label);
            Assert.Equal(33.729704170097, clustering.Clusters.First().Score);
            Assert.Equal(8, clustering.Clusters.First().Documents.Count());
            Assert.Equal("19622040", clustering.Clusters.First().Documents.First());
        }

        [Fact]
        public void ParseTerms()
        {
            var parser = new TermsResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithTerms.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='terms']");
            var terms = parser.ParseTerms(docNode);
            Assert.NotNull(terms);
            Assert.Equal(2, terms.Count);
            Assert.Equal("text", terms.First().Field);
            Assert.Equal("textgen", terms.ElementAt(1).Field);
            Assert.Equal("boot", terms.First().Terms.First().Key);
            Assert.Equal(479, terms.First().Terms.First().Value);
            Assert.Equal("boots", terms.ElementAt(1).Terms.First().Key);
            Assert.Equal(463, terms.ElementAt(1).Terms.First().Value);
        }

        [Fact]
        public void ParseTermVector()
        {
            var parser = new TermVectorResultsParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithTermVector.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='termVectors']");
            var docs = parser.ParseDocuments(docNode).ToList();

            Assert.NotNull(docs);
            Assert.Equal(2, docs.Count);
            var cable = docs
                .First(d => d.UniqueKey == "3007WFP")
                .TermVector
                .First(f => f.Field == "includes");

            Assert.Equal("cable", cable.Term);
            Assert.Equal(1, cable.Tf);
            Assert.Equal(1, cable.Df);
            Assert.Equal(1.0, cable.Tf_Idf);

            var positions = cable.Positions.ToList();
            Assert.Equal(2, cable.Positions.Count);
            Assert.Equal(1, positions[0]);
            Assert.Equal(10, positions[1]);

            var offsets = cable.Offsets.ToList();
            Assert.Equal(1, cable.Offsets.Count);
            Assert.Equal(4, offsets[0].Start);
            Assert.Equal(9, offsets[0].End);
        }

        [Fact]
        public void ParseTermVector2()
        {
            var parser = new TermVectorResultsParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithTermVector2.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='termVectors']");
            var docs = parser.ParseDocuments(docNode).ToList();
            Assert.NotNull(docs);
            Assert.Equal(1, docs.Count);
            Assert.Equal("20", docs[0].UniqueKey);
            var vectors = docs[0].TermVector.ToList();
            Assert.Equal(15, vectors.Count);
        }

        [Fact]
        public void ParseMoreLikeThis()
        {
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
            Assert.NotNull(mlt);
            Assert.Equal(2, mlt.Count);
            Assert.True(mlt.ContainsKey(product1.Id));
            Assert.True(mlt.ContainsKey(product2.Id));
            Assert.Equal(1, mlt[product1.Id].Count);
            Assert.Equal(1, mlt[product2.Id].Count);
            //Console.WriteLine(mlt[product1.Id][0].Id);
        }

        [Fact]
        public void ParseStatsResults()
        {
            var parser = new StatsResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithStats.xml");
            var docNode = xml.XPathSelectElement("response/lst[@name='stats']");
            var stats = parser.ParseStats(docNode, "stats_fields");
            Assert.Equal(2, stats.Count);
            Assert.True(stats.ContainsKey("price"));
            var priceStats = stats["price"];
            Assert.Equal(0.0, priceStats.Min);
            Assert.Equal(2199.0, priceStats.Max);
            Assert.Equal(5251.2699999999995, priceStats.Sum);
            Assert.Equal(15, priceStats.Count);
            Assert.Equal(11, priceStats.Missing);
            Assert.Equal(6038619.160300001, priceStats.SumOfSquares);
            Assert.Equal(350.08466666666664, priceStats.Mean);
            Assert.Equal(547.737557906113, priceStats.StdDev);
            Assert.Equal(1, priceStats.FacetResults.Count);
            Assert.True(priceStats.FacetResults.ContainsKey("inStock"));
            var priceInStockStats = priceStats.FacetResults["inStock"];
            Assert.Equal(2, priceInStockStats.Count);
            Assert.True(priceInStockStats.ContainsKey("true"));
            Assert.True(priceInStockStats.ContainsKey("false"));
            var priceInStockFalseStats = priceInStockStats["false"];
            Assert.Equal(11.5, priceInStockFalseStats.Min);
            Assert.Equal(649.99, priceInStockFalseStats.Max);
            Assert.Equal(1161.39, priceInStockFalseStats.Sum);
            Assert.Equal(4, priceInStockFalseStats.Count);
            Assert.Equal(0, priceInStockFalseStats.Missing);
            Assert.Equal(653369.2551, priceInStockFalseStats.SumOfSquares);
            Assert.Equal(290.3475, priceInStockFalseStats.Mean);
            Assert.Equal(324.63444676281654, priceInStockFalseStats.StdDev);
            var priceInStockTrueStats = priceInStockStats["true"];
            Assert.Equal(0.0, priceInStockTrueStats.Min);
            Assert.Equal(2199.0, priceInStockTrueStats.Max);
            Assert.Equal(4089.879999999999, priceInStockTrueStats.Sum);
            Assert.Equal(11, priceInStockTrueStats.Count);
            Assert.Equal(0, priceInStockTrueStats.Missing);
            Assert.Equal(5385249.905200001, priceInStockTrueStats.SumOfSquares);
            Assert.Equal(371.8072727272727, priceInStockTrueStats.Mean);
            Assert.Equal(621.6592938755265, priceInStockTrueStats.StdDev);

            var zeroResultsStats = stats["zeroResults"];
            Assert.Equal(double.NaN, zeroResultsStats.Min);
            Assert.Equal(double.NaN, zeroResultsStats.Max);
            Assert.Equal(0, zeroResultsStats.Count);
            Assert.Equal(0, zeroResultsStats.Missing);
            Assert.Equal(0.0, zeroResultsStats.Sum);
            Assert.Equal(0.0, zeroResultsStats.SumOfSquares);
            Assert.Equal(double.NaN, zeroResultsStats.Mean);
            Assert.Equal(0.0, zeroResultsStats.StdDev);
        }

        [Fact]
        public void ParseStatsResults2()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithStats.xml");
            var parser = new StatsResponseParser<Product>();
            var stats = parser.ParseStats(xml.Root, "stats_fields");

            Assert.NotNull(stats);
            Assert.Contains("instock_prices", stats.Keys);
            Assert.Contains("all_prices", stats.Keys);

            var instock = stats["instock_prices"];
            Assert.Equal(0, instock.Min);
            Assert.Equal(2199, instock.Max);
            Assert.Equal(16, instock.Count);
            Assert.Equal(16, instock.Missing);
            Assert.Equal(5251.270030975342, instock.Sum);

            var all = stats["all_prices"];
            Assert.Equal(4089.880027770996, all.Sum);
            Assert.Equal(2199, all.Max);
        }

        [Fact]
        public void ParseFacetDateResults()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithDateFacet.xml");
            var p = new FacetsResponseParser<Product>();
            var results = p.ParseFacetDates(xml.Root);
            Assert.Equal(1, results.Count);
            var result = results.First();
            Assert.Equal("timestamp", result.Key);
            Assert.Equal("+1DAY", result.Value.Gap);
            Assert.Equal(new DateTime(2009, 8, 10, 0, 33, 46, 578), result.Value.End);
            var dateResults = result.Value.DateResults;
            Assert.Equal(1, dateResults.Count);
            Assert.Equal(16, dateResults[0].Value);
            Assert.Equal(new DateTime(2009, 8, 9, 0, 33, 46, 578), dateResults[0].Key);
        }

        [Fact]
        public void ParseFacetDateResultsWithOther()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithDateFacetAndOther.xml");
            var p = new FacetsResponseParser<Product>();
            var results = p.ParseFacetDates(xml.Root);
            Assert.Equal(1, results.Count);
            var result = results.First();
            Assert.Equal("timestamp", result.Key);
            Assert.Equal("+1DAY", result.Value.Gap);
            Assert.Equal(new DateTime(2009, 8, 10, 0, 46, 29), result.Value.End);
            Assert.Equal(new DateTime(2009, 8, 9, 22, 46, 29), result.Value.DateResults[0].Key);
            var other = result.Value.OtherResults;
            Assert.Equal(1, other[FacetDateOther.Before]);
            Assert.Equal(0, other[FacetDateOther.After]);
            Assert.Equal(0, other[FacetDateOther.Between]);
        }

        [Fact]
        public void ParseFacetRangeResults()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithRangeFacet.xml");
            var p = new FacetsResponseParser<Product>();
            var results = p.ParseFacetRanges(xml.Root);
            Assert.Equal(1, results.Count);
            var result = results.First();
            Assert.Equal("timestamp", result.Key);
            Assert.Equal("+1DAY", result.Value.Gap);
            Assert.Equal("2017-08-29T00:00:00Z", result.Value.Start);
            Assert.Equal("2017-08-31T00:00:00Z", result.Value.End);
            var RangeResults = result.Value.RangeResults;
            Assert.Equal(2, RangeResults.Count);
            Assert.Equal(27, RangeResults[0].Value);
            Assert.Equal("2017-08-29T00:00:00Z", RangeResults[0].Key);
            Assert.Equal(124, RangeResults[1].Value);
            Assert.Equal("2017-08-30T00:00:00Z", RangeResults[1].Key);
        }

        [Fact]
        public void ParseFacetRangeResultsWithOther()
        {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.partialResponseWithRangeFacetAndOther.xml");
            var p = new FacetsResponseParser<Product>();
            var results = p.ParseFacetRanges(xml.Root);
            Assert.Equal(1, results.Count);
            var result = results.First();
            Assert.Equal("timestamp", result.Key);
            Assert.Equal("+1DAY", result.Value.Gap);
            Assert.Equal("2017-08-29T00:00:00Z", result.Value.Start);
            Assert.Equal("2017-08-31T00:00:00Z", result.Value.End);
            var RangeResults = result.Value.RangeResults;
            Assert.Equal(2, RangeResults.Count);
            Assert.Equal(27, RangeResults[0].Value);
            Assert.Equal("2017-08-29T00:00:00Z", RangeResults[0].Key);
            Assert.Equal(124, RangeResults[1].Value);
            Assert.Equal("2017-08-30T00:00:00Z", RangeResults[1].Key);

            var other = result.Value.OtherResults;
            Assert.Equal(41739753, other[FacetRangeOther.Before]);
            Assert.Equal(47567, other[FacetRangeOther.After]);
            Assert.Equal(151, other[FacetRangeOther.Between]);
        }

        [Fact]
        public void ParseResultsWithGroups()
        {
            var mapper = new AttributesMappingManager();
            var parser = new GroupingResponseParser<Product>(new SolrDocumentResponseParser<Product>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Product>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithGroupingOnInstock.xml");
            var results = new SolrQueryResults<Product>();
            parser.Parse(xml, results);
            Assert.Equal(1, results.Grouping.Count);
            Assert.Equal(2, results.Grouping["inStock"].Groups.Count());
            Assert.Equal(13, results.Grouping["inStock"].Groups.First().NumFound);
        }

        [Fact]
        public void ParseResultsWithFacetPivot()
        {
            var parser = new FacetsResponseParser<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithFacetPivoting.xml");
            var r = new SolrQueryResults<Product>();
            parser.Parse(xml, r);
            Assert.NotNull(r.FacetPivots);
            //Console.WriteLine(r.FacetPivots.Count);
            Assert.True(r.FacetPivots.ContainsKey("inStock,manu"));

            Assert.Equal(2, r.FacetPivots["inStock,manu"].Count);
            Assert.Equal("inStock", r.FacetPivots["inStock,manu"][0].Field);
            Assert.Equal(10, r.FacetPivots["inStock,manu"][0].ChildPivots.Count);

        }

        [Fact]
        public void PropertyWithoutSetter()
        {
            var parser = GetDocumentParser<TestDocWithoutSetter>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var docNode = xml.XPathSelectElement("response/result/doc");
            var doc = parser.ParseDocument(docNode);
            Assert.NotNull(doc);
            Assert.Equal(0, doc.Id);
        }

        [Fact]
        public void ParseInterestingTermsList()
        {
            var innerParser = new InterestingTermsResponseParser<Product>();
            var parser = new SolrMoreLikeThisHandlerQueryResultsParser<Product>(new[] { innerParser });
            var response = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithInterestingTermsList.xml");
            var results = parser.Parse(response);

            Assert.NotNull(results);
            Assert.NotNull(results.InterestingTerms);
            Assert.Equal(4, results.InterestingTerms.Count);
            Assert.Equal("three", results.InterestingTerms[2].Key);
            Assert.True(results.InterestingTerms.All(t => t.Value == 0.0f));
        }

        [Fact]
        public void ParseInterestingTermsDetails()
        {
            var innerParser = new InterestingTermsResponseParser<Product>();
            var parser = new SolrMoreLikeThisHandlerQueryResultsParser<Product>(new[] { innerParser });
            var response = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithInterestingTermsDetails.xml");
            var results = parser.Parse(response);

            Assert.NotNull(results);
            Assert.NotNull(results.InterestingTerms);
            Assert.Equal(4, results.InterestingTerms.Count);
            Assert.Equal("content:three", results.InterestingTerms[2].Key);
            Assert.Equal(3.3f, results.InterestingTerms[2].Value);
        }

        [Fact]
        public void ParseMlthMatch()
        {
            var innerParser = new MoreLikeThisHandlerMatchResponseParser<TestDocWithGuid>(GetDocumentParser<TestDocWithGuid>());
            var parser = new SolrMoreLikeThisHandlerQueryResultsParser<TestDocWithGuid>(new[] { innerParser });
            var response = EmbeddedResource.GetEmbeddedString(GetType(), "Resources.responseWithInterestingTermsDetails.xml");
            var results = parser.Parse(response);

            Assert.NotNull(results);
            Assert.NotNull(results.Match);
            Assert.Equal(new Guid("224fbdc1-12df-4520-9fbe-dd91f916eba1"), results.Match.Key);
        }

        public enum AEnum
        {
            One,
            Two,
            Three
        }



        public class TestDocWithEnum
        {
            [SolrField]
            public AEnum En { get; set; }
        }

        public class TestDocWithNullableEnum
        {
            [SolrField("basicview")]
            public AEnum? BasicView { get; set; }
        }
    }
}