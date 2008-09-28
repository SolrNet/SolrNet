using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using log4net.Config;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryResultsParserTests {
		[Test]
		public void ParseDocument() {
			var parser = new SolrQueryResultParser<TestDocument>();
			var xml = new XmlDocument();
			xml.LoadXml(responseXml);
			var docNode = xml.SelectSingleNode("response/result/doc");
			var doc = parser.ParseDocument(docNode);
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
		}

		[Test]
		public void NumFound() {
			var parser = new SolrQueryResultParser<TestDocument>();
			var r = parser.Parse(responseXml);
			Assert.AreEqual(1, r.NumFound);
		}

		[Test]
		public void Parse() {
			var parser = new SolrQueryResultParser<TestDocument>();
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
			var parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			var doc = new TestDocumentWithArrays();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays).GetProperty("Cat"), fieldNode);
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
			var parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			var doc = new TestDocumentWithArrays();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays).GetProperty("Price"), fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyNullableDouble() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/float[@name='price']");
			var parser = new SolrQueryResultParser<TestDocumentWithNullableDouble>();
			var doc = new TestDocumentWithNullableDouble();
			parser.SetProperty(doc, typeof (TestDocumentWithNullableDouble).GetProperty("Price"), fieldNode);
			Assert.AreEqual(92d, doc.Price);
		}

		[Test]
		public void SetPropertyWithIntCollection() {
			var xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			var fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='numbers']");
			var parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			var doc = new TestDocumentWithArrays();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays).GetProperty("Numbers"), fieldNode);
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
			var parser = new SolrQueryResultParser<TestDocumentWithArrays3>();
			var doc = new TestDocumentWithArrays3();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays3).GetProperty("Numbers"), fieldNode);
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
			var parser = new SolrQueryResultParser<TestDocumentWithArrays2>();
			var doc = new TestDocumentWithArrays2();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays2).GetProperty("Numbers"), fieldNode);
			Assert.AreEqual(2, doc.Numbers.Length);
			var numbers = new List<int>(doc.Numbers);
			Assert.AreEqual(1, numbers[0]);
			Assert.AreEqual(2, numbers[1]);
		}

		[Test]
		public void ParseResultsWithArrays() {
			var parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			var results = parser.Parse(responseXMLWithArrays);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual("SP2514N", doc.Id);
		}

		[Test]
		public void SupportsDateTime() {
			var parser = new SolrQueryResultParser<TestDocumentWithDate>();
			var results = parser.Parse(responseXMLWithDate);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
		}

		[Test]
		public void ParseDate_without_milliseconds() {
			var parser = new SolrQueryResultParser<TestDocumentWithDate>();
			var dt = parser.ParseDate("2001-01-02T03:04:05Z");
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), dt);
		}

		[Test]
		public void ParseDate_with_milliseconds() {
			var parser = new SolrQueryResultParser<TestDocumentWithDate>();
			var dt = parser.ParseDate("2001-01-02T03:04:05.245Z");
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5, 245), dt);
		}

		[Test]
		public void SupportsNullableDateTime() {
			var parser = new SolrQueryResultParser<TestDocumentWithNullableDate>();
			var results = parser.Parse(responseXMLWithDate);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 4, 5), doc.Fecha);
		}

		[Test]
		public void SupportsIEnumerable() {
			var parser = new SolrQueryResultParser<TestDocumentWithArrays4>();
			var results = parser.Parse(responseXMLWithArraysSimple);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
			Assert.AreEqual(2, new List<string>(doc.Features).Count);
		}

		[Test]
		public void WrongFieldDoesntThrow() {
			var parser = new SolrQueryResultParser<TestDocumentWithDate>();
			var results = parser.Parse(responseXMLWithArraysSimple);
			Assert.AreEqual(1, results.Count);
			var doc = results[0];
		}

		[Test]
		public void ReadsMaxScoreAttribute() {
			var parser = new SolrQueryResultParser<TestDocumentWithArrays4>();
			var results = parser.Parse(responseXMLWithArraysSimple);
			Assert.AreEqual(1.6578954, results.MaxScore);
		}

		[Test]
		public void ReadMaxScore_doesnt_crash_if_not_present() {
			var parser = new SolrQueryResultParser<TestDocument>();
			var results = parser.Parse(responseXml);
			Assert.IsNull(results.MaxScore);
		}

		[Test]
		[Ignore("Performance test, potentially slow")]
		public void Performance() {
			BasicConfigurator.Configure();
			var parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			for (var i = 0; i < 1000; i++) {
				parser.Parse(responseXMLWithArrays);
			}
		}

		[Test]
		public void ParseFacetResults() {
			var parser = new SolrQueryResultParser<TestDocumentWithArrays>();
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
			var parser = new SolrQueryResultParser<TestDocument>();
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

		public class TestDocument : ISolrDocument {
			[SolrField("advancedview")]
			public string AdvancedView { get; set; }

			[SolrField("basicview")]
			public string BasicView { get; set; }

			[SolrField("id")]
			public int Id { get; set; }
		}

		private const string responseXml =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader"">
	<int name=""status"">1</int>
	<int name=""QTime"">15</int>
	<lst name=""params"">
		<str name=""q"">id:123456</str>
		<str name=""version"">2.2</str>
	</lst>
</lst>
<result name=""response"" numFound=""1"" start=""0""><doc><str name=""advancedview""/><str name=""basicview""/><int name=""id"">123456</int></doc></result>
</response>
";

		private const string responseXMLWithArraysSimple =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<responseHeader><status>0</status><QTime>1</QTime></responseHeader>
<result numFound=""1"" start=""0"" maxScore=""1.6578954"">
	<doc>
		<arr name=""features""><str>7200RPM, 8MB cache, IDE Ultra ATA-133</str><str>NoiseGuard, SilentSeek technology, Fluid Dynamic Bearing (FDB) motor</str></arr>
	</doc>
</result>
</response>
";

		private const string responseXMLWithArrays =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<responseHeader><status>0</status><QTime>1</QTime></responseHeader>

<result numFound=""1"" start=""0"">
 <doc>
  <arr name=""cat""><str>electronics</str><str>hard drive</str></arr>
  <arr name=""features""><str>7200RPM, 8MB cache, IDE Ultra ATA-133</str><str>NoiseGuard, SilentSeek technology, Fluid Dynamic Bearing (FDB) motor</str></arr>
  <str name=""id"">SP2514N</str>
  <bool name=""inStock"">true</bool>
  <str name=""manu"">Samsung Electronics Co. Ltd.</str>
  <str name=""name"">Samsung SpinPoint P120 SP2514N - hard drive - 250 GB - ATA-133</str>
  <int name=""popularity"">6</int>
  <float name=""price"">92.0</float>
  <str name=""sku"">SP2514N</str>
	<arr name=""numbers""><int>1</int><int>2</int></arr>
 </doc>
</result>
</response>
";

		private const string responseXMLWithDate = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<result numFound=""1"" start=""0"">
	<doc>
	<date name=""Fecha"">2001-01-02T03:04:05Z</date>
	</doc>
</result>
</response>
";

		private const string responseXMLWithFacet =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<responseHeader><status>0</status><QTime>2</QTime></responseHeader>
<result numFound=""4"" start=""0""/>
<lst name=""facet_counts"">
 <lst name=""facet_queries"">
	<int name=""payment:[0 TO 1000]"">259</int>
 </lst>
 <lst name=""facet_fields"">
  <lst name=""cat"">
        <int name=""search"">0</int>
        <int name=""memory"">0</int>
        <int name=""graphics"">0</int>
        <int name=""card"">0</int>
        <int name=""music"">1</int>
        <int name=""software"">0</int>
        <int name=""electronics"">3</int>
        <int name=""copier"">0</int>
        <int name=""multifunction"">0</int>
        <int name=""camera"">0</int>
        <int name=""connector"">2</int>
        <int name=""hard"">0</int>
        <int name=""scanner"">0</int>
        <int name=""monitor"">0</int>
        <int name=""drive"">0</int>
        <int name=""printer"">0</int>
  </lst>
  <lst name=""inStock"">
        <int name=""false"">3</int>
        <int name=""true"">1</int>
  </lst>
 </lst>
</lst>
</response>";

		public class TestDocumentWithNullableDouble : ISolrDocument {
			[SolrField("price")]
			public double? Price { get; set; }
		}

		public class TestDocumentWithArrays : ISolrDocument {
			[SolrField("cat")]
			public ICollection<string> Cat { get; set; }

			[SolrField("features")]
			public ICollection<string> Features { get; set; }

			[SolrField("id")]
			public string Id { get; set; }

			[SolrField("inStock")]
			public bool InStock { get; set; }

			[SolrField("manu")]
			public string Manu { get; set; }

			[SolrField("name")]
			public string Name { get; set; }

			[SolrField("popularity")]
			public int Popularity { get; set; }

			[SolrField("price")]
			public double Price { get; set; }

			[SolrField("sku")]
			public string Sku { get; set; }

			[SolrField("numbers")]
			public ICollection<int> Numbers { get; set; }
		}

		public class TestDocumentWithArrays2 : ISolrDocument {
			[SolrField("numbers")]
			public int[] Numbers { get; set; }
		}

		public class TestDocumentWithArrays3 : ISolrDocument {
			[SolrField("numbers")]
			public ICollection Numbers { get; set; }
		}

		public class TestDocumentWithArrays4 : ISolrDocument {
			[SolrField("features")]
			public IEnumerable<string> Features { get; set; }
		}

		public class TestDocumentWithDate : ISolrDocument {
			[SolrField]
			public DateTime Fecha { get; set; }
		}

		public class TestDocumentWithNullableDate : ISolrDocument {
			[SolrField]
			public DateTime? Fecha { get; set; }
		}
	}
}