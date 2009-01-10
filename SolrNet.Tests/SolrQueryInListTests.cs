using NUnit.Framework;
using System.Linq;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryInListTests {
		[Test]
		public void ListOfInt() {
			var q = new SolrQueryInList("id", new[] {1, 2, 3, 4}.Select(i => i.ToString()));
			Assert.AreEqual("(id:1 OR id:2 OR id:3 OR id:4)", q.Query);
		}

        [Test]
        public void ShouldQuoteValues() {
            var q = new SolrQueryInList("id", new[] {"one", "two thousand"});
            Assert.AreEqual("(id:one OR id:\"two thousand\")", q.Query);
        }
	}
}