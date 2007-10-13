using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrDocumentSerializerTests {
		public class SampleDoc : ISolrDocument {
			[SolrField]
			public string Id {
				get { return "id"; }
			}

			[SolrField("Flower")]
			public decimal caca {
				get { return 23.5m; }
			}
		}

		[Test]
		public void tt() {
			SolrDocumentSerializer<SampleDoc> ser = new SolrDocumentSerializer<SampleDoc>();
			string fs = ser.Serialize(new SampleDoc());
			Assert.AreEqual("<doc><field name=\"Id\">id</field><field name=\"Flower\">23,5</field></doc>", fs);
		}
	}
}