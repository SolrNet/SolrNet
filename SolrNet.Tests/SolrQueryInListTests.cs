using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryInListTests {
		[Test]
		public void tt() {
			var q = new SolrQueryInList("id", new[] {1, 2, 3, 4});
			Assert.AreEqual("(id:1 OR id:2 OR id:3 OR id:4)", q.Query);
		}
	}
}