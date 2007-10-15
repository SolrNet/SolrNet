using System.Xml;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryResultsParserTests {
		private static readonly string responseXml =
			@"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc><str name=""advancedview""/><str name=""basicview""/><int name=""id"">123456</int></doc></result>
</response>
";

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
			XmlDocument xml = new XmlDocument();
			ISolrQueryResults<TestDocument> results = parser.Parse(responseXml);
			Assert.AreEqual(1, results.Count);
			TestDocument doc = results[0];
			Assert.AreEqual(123456, doc.Id);
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
	}
}