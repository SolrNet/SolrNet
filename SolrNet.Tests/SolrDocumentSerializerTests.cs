using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrDocumentSerializerTests {
		public class SampleDoc : ISolrDocument {
			private string id;
			private decimal dd;

			[SolrField]
			public string Id {
				get { return id; }
				set { id = value; }
			}

			[SolrField("Flower")]
			public decimal Dd {
				get { return dd; }
				set { dd = value; }
			}
		}

		public class TestDocWithCollections : ISolrDocument {
			[SolrField]
			public ICollection<string> coll {
				get { return new string[] {"one", "two"}; }
			}
		}

		public class TestDocWithDate : ISolrDocument {
			private DateTime date;

			[SolrField]
			public DateTime Date {
				get { return date; }
				set { date = value; }
			}
		}

		[Test]
		public void Serializes() {
			SolrDocumentSerializer<SampleDoc> ser = new SolrDocumentSerializer<SampleDoc>();
			SampleDoc doc = new SampleDoc();
			doc.Id = "id";
			doc.Dd = 23.5m;
			string fs = ser.Serialize(doc).OuterXml;
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"Id\">id</field><field name=\"Flower\">23,5</field></doc>", fs);
		}

		[Test]
		public void SupportsCollections() {
			SolrDocumentSerializer<TestDocWithCollections> ser = new SolrDocumentSerializer<TestDocWithCollections>();
			TestDocWithCollections doc = new TestDocWithCollections();
			string fs = ser.Serialize(doc).OuterXml;
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"coll\"><arr><str>one</str><str>two</str></arr></field></doc>", fs);
		}

		[Test]
		public void EscapesStrings() {
			SolrDocumentSerializer<SampleDoc> ser = new SolrDocumentSerializer<SampleDoc>();
			SampleDoc doc = new SampleDoc();
			doc.Id = "<quote\"";
			string fs = ser.Serialize(doc).OuterXml;
			Console.WriteLine(fs);
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fs);
		}

		[Test]
		public void AcceptsNullObjects() {
			SolrDocumentSerializer<SampleDoc> ser = new SolrDocumentSerializer<SampleDoc>();
			SampleDoc doc = new SampleDoc();
			doc.Id = null;
			string fs = ser.Serialize(doc).OuterXml;
			Console.WriteLine(fs);
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fs);
		}

		/// <summary>
		/// Support according to http://lucene.apache.org/solr/api/org/apache/solr/schema/DateField.html
		/// </summary>
		[Test]
		public void SupportsDateTime() {
			SolrDocumentSerializer<TestDocWithDate> ser = new SolrDocumentSerializer<TestDocWithDate>();
			TestDocWithDate doc = new TestDocWithDate();
			doc.Date = new DateTime(2001, 1, 2, 3, 4, 5);
			string fs = ser.Serialize(doc).OuterXml;
			XmlDocument xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"Fecha\">2001-01-02T03:04:05Z</field></doc>", fs);
		}
	}
}