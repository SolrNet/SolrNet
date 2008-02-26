using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrMultipleCriteriaQueryTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void Concat() {
			var q1 = new SolrQuery("1");
			var q2 = new SolrQuery("2");
			var qm = new SolrMultipleCriteriaQuery(new[] {q1, q2});
			Assert.AreEqual("1 2", qm.Query);
		}

		[Test]
		public void AcceptsNulls() {
			var q1 = new SolrQuery("1");
			ISolrQuery q2 = null;
			var qm = new SolrMultipleCriteriaQuery(new[] {q1, q2});
			Assert.AreEqual("1", qm.Query);
		}
	}
}