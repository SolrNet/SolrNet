using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryByExampleTests {
		[Test]
		public void Basic() {
			ISolrQuery q = new SolrQueryByExample<TestDocument>(new TestDocument(0, "a"));
			Assert.AreEqual("Id:0 Ss:a", q.Query);
		}

		[Test]
		public void NullShouldNotGenerateQuery() {
			ISolrQuery q = new SolrQueryByExample<TestDocument>(new TestDocument(0, null));
			Assert.AreEqual("Id:0", q.Query);
		}

		[Test]
		public void NullShouldNotGenerateQuery2() {
			ISolrQuery q = new SolrQueryByExample<TestDocument>(new TestDocument(null, "a"));
			Assert.AreEqual("Ss:a", q.Query);
		}

		public class TestDocument : ISolrDocument {
			private int? id;
			private string ss;

			public TestDocument(int? id, string ss) {
				this.id = id;
				this.ss = ss;
			}

			[SolrField]
			public int? Id {
				get { return id; }
				set { id = value; }
			}

			[SolrField]
			public string Ss {
				get { return ss; }
				set { ss = value; }
			}
		}
	}
}