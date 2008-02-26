using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryByFieldTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void Basic() {
			var q = new SolrQueryByField("id", "123456");
			Assert.AreEqual("id:123456", q.Query);
		}

		[Test]
		public void ShouldQuoteSpaces() {
			var q = new SolrQueryByField("id", "hello world");
			Assert.AreEqual("id:\"hello world\"", q.Query);
		}

		[Test]
		public void ShouldQuoteSpecialChar() {
			var q = new SolrQueryByField("id", "hello+world-bye&&q||w!e(r)t{y}[u]^i\"o~p:a\\s+d");
			Assert.AreEqual(@"id:hello\+world\-bye\&&q\||w\!e\(r\)t\{y\}\[u\]\^i\""o\~p\:a\\s\+d", q.Query);
		}

		[Test]
		public void ShouldNotQuoteWildcard() {
			var q = new SolrQueryByField("id", "h?llo*");
			Assert.AreEqual("id:h?llo*", q.Query);
		}
	}
}