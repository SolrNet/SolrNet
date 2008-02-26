using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryByRangeTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void IntInclusive() {
			ISolrQuery q = new SolrQueryByRange<TestDocument, int>("id", 123, 456);
			Assert.AreEqual("id:[123 TO 456]", q.Query);
		}

		[Test]
		public void StringInclusive() {
			ISolrQuery q = new SolrQueryByRange<TestDocument, string>("id", "Arroz", "Zimbabwe");
			Assert.AreEqual("id:[Arroz TO Zimbabwe]", q.Query);
		}

		[Test]
		public void StringExclusive() {
			ISolrQuery q = new SolrQueryByRange<TestDocument, string>("id", "Arroz", "Zimbabwe", false);
			Assert.AreEqual("id:{Arroz TO Zimbabwe}", q.Query);
		}
	}
}