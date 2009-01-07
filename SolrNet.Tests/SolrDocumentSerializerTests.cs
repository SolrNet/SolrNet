using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using SolrNet.Attributes;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrDocumentSerializerTests {
		public class SampleDoc : ISolrDocument {
			[SolrField]
			public string Id { get; set; }

			[SolrField("Flower")]
			public decimal Dd { get; set; }
		}

		public class TestDocWithCollections : ISolrDocument {
			[SolrField]
			public ICollection<string> coll {
				get { return new[] {"one", "two"}; }
			}
		}

		public class TestDocWithDate : ISolrDocument {
			[SolrField]
			public DateTime Date { get; set; }
		}

		public class TestDocWithBool : ISolrDocument {
			[SolrField]
			public bool B { get; set; }
		}

		[Test]
		public void Serializes() {
			var ser = new SolrDocumentSerializer<SampleDoc>();
			var doc = new SampleDoc {Id = "id", Dd = 23.5m};
			string fs = ser.Serialize(doc).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"Id\">id</field><field name=\"Flower\">23.5</field></doc>", fs);
		}

		[Test]
		public void SupportsCollections() {
			var ser = new SolrDocumentSerializer<TestDocWithCollections>();
			var doc = new TestDocWithCollections();
			string fs = ser.Serialize(doc).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"coll\">one</field><field name=\"coll\">two</field></doc>", fs);
		}

		[Test]
		public void EscapesStrings() {
			var ser = new SolrDocumentSerializer<SampleDoc>();
			var doc = new SampleDoc {Id = "<quote\""};
			string fs = ser.Serialize(doc).OuterXml;
			Console.WriteLine(fs);
			var xml = new XmlDocument();
			xml.LoadXml(fs);
		}

		[Test]
		public void AcceptsNullObjects() {
			var ser = new SolrDocumentSerializer<SampleDoc>();
			var doc = new SampleDoc {Id = null};
			string fs = ser.Serialize(doc).OuterXml;
			Console.WriteLine(fs);
			var xml = new XmlDocument();
			xml.LoadXml(fs);
		}

		/// <summary>
		/// Support according to http://lucene.apache.org/solr/api/org/apache/solr/schema/DateField.html
		/// </summary>
		[Test]
		public void SupportsDateTime() {
			var ser = new SolrDocumentSerializer<TestDocWithDate>();
			var doc = new TestDocWithDate {Date = new DateTime(2001, 1, 2, 3, 4, 5)};
			string fs = ser.Serialize(doc).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"Date\">2001-01-02T03:04:05Z</field></doc>", fs);
		}

		[Test]
		public void SupportsBoolTrue() {
			var ser = new SolrDocumentSerializer<TestDocWithBool>();
			var doc = new TestDocWithBool {B = true};
			string fs = ser.Serialize(doc).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"B\">true</field></doc>", fs);
		}

		[Test]
		public void SupportsBoolFalse() {
			var ser = new SolrDocumentSerializer<TestDocWithBool>();
			var doc = new TestDocWithBool {B = false};
			string fs = ser.Serialize(doc).OuterXml;
			var xml = new XmlDocument();
			xml.LoadXml(fs);
			Assert.AreEqual("<doc><field name=\"B\">false</field></doc>", fs);
		}
	}
}