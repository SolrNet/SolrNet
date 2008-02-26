using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrMultipleCriteriaQueryTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void Concat() {
			ISolrQuery q1 = new SolrQuery("1");
			ISolrQuery q2 = new SolrQuery("2");
			ISolrQuery qm = new SolrMultipleCriteriaQuery<TestDocument>(new ISolrQuery[] {q1, q2});
			Assert.AreEqual("1 2", qm.Query);
		}

		[Test]
		public void AcceptsNulls() {
			ISolrQuery q1 = new SolrQuery("1");
			ISolrQuery q2 = null;
			ISolrQuery qm = new SolrMultipleCriteriaQuery<TestDocument>(new ISolrQuery[] {q1, q2});
			Assert.AreEqual("1", qm.Query);
		}
	}
}