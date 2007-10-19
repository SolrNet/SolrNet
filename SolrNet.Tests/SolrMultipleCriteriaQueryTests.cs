using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrMultipleCriteriaQueryTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void Concat() {
			ISolrQuery<TestDocument> q1 = new SolrQuery<TestDocument>("1");
			ISolrQuery<TestDocument> q2 = new SolrQuery<TestDocument>("2");
			ISolrQuery<TestDocument> qm = new SolrMultipleCriteriaQuery<TestDocument>(new ISolrQuery<TestDocument>[] {q1, q2});
			Assert.AreEqual("1 2", qm.Query);
		}

		[Test]
		public void AcceptsNulls() {
			ISolrQuery<TestDocument> q1 = new SolrQuery<TestDocument>("1");
			ISolrQuery<TestDocument> q2 = null;
			ISolrQuery<TestDocument> qm = new SolrMultipleCriteriaQuery<TestDocument>(new ISolrQuery<TestDocument>[] {q1, q2});
			Assert.AreEqual("1", qm.Query);
		}
	}
}