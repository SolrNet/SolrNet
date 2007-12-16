using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryResultsParserTests {
		[Test]
		public void ParseDocument() {
			SolrQueryResultParser<TestDocument> parser = new SolrQueryResultParser<TestDocument>();
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(responseXml);
			XmlNode docNode = xml.SelectSingleNode("response/result/doc");
			TestDocument doc = parser.ParseDocument(docNode);
			Assert.IsNotNull(doc);
			Assert.AreEqual(123456, doc.Id);
		}

		[Test]
		public void NumFound() {
			SolrQueryResultParser<TestDocument> parser = new SolrQueryResultParser<TestDocument>();
			ISolrQueryResults<TestDocument> r = parser.Parse(responseXml);
			Assert.AreEqual(1, r.NumFound);
		}

		[Test]
		public void Parse() {
			SolrQueryResultParser<TestDocument> parser = new SolrQueryResultParser<TestDocument>();
			ISolrQueryResults<TestDocument> results = parser.Parse(responseXml);
			Assert.AreEqual(1, results.Count);
			TestDocument doc = results[0];
			Assert.AreEqual(123456, doc.Id);
		}

		[Test]
		public void SetPropertyWithArrayOfStrings() {
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			XmlNode fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='cat']");
			SolrQueryResultParser<TestDocumentWithArrays> parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			TestDocumentWithArrays doc = new TestDocumentWithArrays();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays).GetProperty("Cat"), fieldNode);
			Assert.AreEqual(2, doc.Cat.Count);
			IList<string> cats = new List<string>(doc.Cat);
			Assert.AreEqual("electronics", cats[0]);
			Assert.AreEqual("hard drive", cats[1]);
		}

		[Test]
		public void SetPropertyWithIntCollection() {
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			XmlNode fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='numbers']");
			SolrQueryResultParser<TestDocumentWithArrays> parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			TestDocumentWithArrays doc = new TestDocumentWithArrays();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays).GetProperty("Numbers"), fieldNode);
			Assert.AreEqual(2, doc.Numbers.Count);
			IList<int> numbers = new List<int>(doc.Numbers);
			Assert.AreEqual(1, numbers[0]);
			Assert.AreEqual(2, numbers[1]);
		}

		[Test]
		public void SetPropertyWithNonGenericCollection() {
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			XmlNode fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='numbers']");
			SolrQueryResultParser<TestDocumentWithArrays3> parser = new SolrQueryResultParser<TestDocumentWithArrays3>();
			TestDocumentWithArrays3 doc = new TestDocumentWithArrays3();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays3).GetProperty("Numbers"), fieldNode);
			Assert.AreEqual(2, doc.Numbers.Count);
			IList numbers = new ArrayList(doc.Numbers);
			Assert.AreEqual(1, numbers[0]);
			Assert.AreEqual(2, numbers[1]);
		}

		[Test]
		public void SetPropertyWithArrayOfIntsToIntArray() {
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(responseXMLWithArrays);
			XmlNode fieldNode = xml.SelectSingleNode("response/result/doc/arr[@name='numbers']");
			SolrQueryResultParser<TestDocumentWithArrays2> parser = new SolrQueryResultParser<TestDocumentWithArrays2>();
			TestDocumentWithArrays2 doc = new TestDocumentWithArrays2();
			parser.SetProperty(doc, typeof (TestDocumentWithArrays2).GetProperty("Numbers"), fieldNode);
			Assert.AreEqual(2, doc.Numbers.Length);
			IList<int> numbers = new List<int>(doc.Numbers);
			Assert.AreEqual(1, numbers[0]);
			Assert.AreEqual(2, numbers[1]);
		}

		[Test]
		public void ParseResultsWithArrays() {
			SolrQueryResultParser<TestDocumentWithArrays> parser = new SolrQueryResultParser<TestDocumentWithArrays>();
			ISolrQueryResults<TestDocumentWithArrays> results = parser.Parse(responseXMLWithArrays);
			Assert.AreEqual(1, results.Count);
			TestDocumentWithArrays doc = results[0];
			Assert.AreEqual("SP2514N", doc.Id);
		}

		[Test]
		public void SupportsDateTime() {
			SolrQueryResultParser<TestDocumentWithDate> parser = new SolrQueryResultParser<TestDocumentWithDate>();
			ISolrQueryResults<TestDocumentWithDate> results = parser.Parse(responseXMLWithDate);
			Assert.AreEqual(1, results.Count);
			TestDocumentWithDate doc = results[0];
			Assert.AreEqual(new DateTime(2001, 1, 2, 3, 3, 3), doc.Fecha);
		}

		public class TestDocument : ISolrDocument {
			private string advancedView;
			private string basicView;
			private int id;

			[SolrField("advancedview")]
			public string AdvancedView {
				get { return advancedView; }
				set { advancedView = value; }
			}

			[SolrField("basicview")]
			public string BasicView {
				get { return basicView; }
				set { basicView = value; }
			}

			[SolrField("id")]
			public int Id {
				get { return id; }
				set { id = value; }
			}
		}

		private static readonly string responseXml =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc><str name=""advancedview""/><str name=""basicview""/><int name=""id"">123456</int></doc></result>
</response>
";

		private static readonly string responseXMLWithArrays =
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

		private static readonly string responseXMLWithDate =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<result numFound=""1"" start=""0"">
	<doc>
	<date name=""Fecha"">2001-01-02T03:03:03Z</date>
	</doc>
</result>
</response>
";

		public class TestDocumentWithArrays : ISolrDocument {
			private ICollection<string> cat;
			private ICollection<string> features;
			private string id;
			private bool inStock;
			private string manu;
			private string name;
			private int popularity;
			private double price;
			private string sku;
			private ICollection<int> numbers;

			[SolrField("cat")]
			public ICollection<string> Cat {
				get { return cat; }
				set { cat = value; }
			}

			[SolrField("features")]
			public ICollection<string> Features {
				get { return features; }
				set { features = value; }
			}

			[SolrField("id")]
			public string Id {
				get { return id; }
				set { id = value; }
			}

			[SolrField("inStock")]
			public bool InStock {
				get { return inStock; }
				set { inStock = value; }
			}

			[SolrField("manu")]
			public string Manu {
				get { return manu; }
				set { manu = value; }
			}

			[SolrField("name")]
			public string Name {
				get { return name; }
				set { name = value; }
			}

			[SolrField("popularity")]
			public int Popularity {
				get { return popularity; }
				set { popularity = value; }
			}

			[SolrField("price")]
			public double Price {
				get { return price; }
				set { price = value; }
			}

			[SolrField("sku")]
			public string Sku {
				get { return sku; }
				set { sku = value; }
			}

			[SolrField("numbers")]
			public ICollection<int> Numbers {
				get { return numbers; }
				set { numbers = value; }
			}
		}

		public class TestDocumentWithArrays2 : ISolrDocument {
			private int[] numbers;

			[SolrField("numbers")]
			public int[] Numbers {
				get { return numbers; }
				set { numbers = value; }
			}
		}

		public class TestDocumentWithArrays3 : ISolrDocument {
			private ICollection numbers;

			[SolrField("numbers")]
			public ICollection Numbers {
				get { return numbers; }
				set { numbers = value; }
			}
		}

		public class TestDocumentWithDate: ISolrDocument {
			private DateTime fecha;

			[SolrField]
			public DateTime Fecha {
				get { return fecha; }
				set { fecha = value; }
			}
		}
	}
}