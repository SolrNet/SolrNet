using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryByFieldTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void Basic() {
			ISolrQuery q = new SolrQueryByField<TestDocument>("id", "123456");
			Assert.AreEqual("id:123456", q.Query);
		}

		[Test]
		public void ShouldQuoteSpaces() {
			ISolrQuery q = new SolrQueryByField<TestDocument>("id", "hello world");
			Assert.AreEqual("id:\"hello world\"", q.Query);
		}

		[Test]
		public void ShouldQuoteSpecialChar() {
			ISolrQuery q = new SolrQueryByField<TestDocument>("id", "hello+world-bye&&q||w!e(r)t{y}[u]^i\"o~p:a\\s+d");
			Assert.AreEqual(@"id:hello\+world\-bye\&&q\||w\!e\(r\)t\{y\}\[u\]\^i\""o\~p\:a\\s\+d", q.Query);
		}

		[Test]
		public void ShouldNotQuoteWildcard() {
			ISolrQuery q = new SolrQueryByField<TestDocument>("id", "h?llo*");
			Assert.AreEqual("id:h?llo*", q.Query);
		}
	}
}