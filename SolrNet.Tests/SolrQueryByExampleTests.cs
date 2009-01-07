using NUnit.Framework;
using SolrNet.Attributes;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryByExampleTests {
		[Test]
		public void Basic() {
			var q = new SolrQueryByExample<TestDocument>(new TestDocument(0, "a"));
			Assert.AreEqual("(Id:0  Ss:a)", q.Query);
		}

		[Test]
		public void NullShouldNotGenerateQuery() {
			var q = new SolrQueryByExample<TestDocument>(new TestDocument(0, null));
			Assert.AreEqual("(Id:0)", q.Query);
		}

		[Test]
		public void NullShouldNotGenerateQuery2() {
			var q = new SolrQueryByExample<TestDocument>(new TestDocument(null, "a"));
			Assert.AreEqual("(Ss:a)", q.Query);
		}

		public class TestDocument : ISolrDocument {
			public TestDocument(int? id, string ss) {
				Id = id;
				Ss = ss;
			}

			[SolrField]
			public int? Id { get; set; }

			[SolrField]
			public string Ss { get; set; }
		}
	}
}