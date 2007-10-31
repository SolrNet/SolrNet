using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SortOrderTests {
		[Test]
		[ExpectedException(typeof (InvalidSortOrderException))]
		public void Constructor_ShouldntAcceptSpaces() {
			SortOrder o = new SortOrder("uno dos");
		}

		[Test]
		[ExpectedException(typeof (InvalidSortOrderException))]
		public void ParseNull_ShouldThrow() {
			SortOrder o = SortOrder.Parse(null);
		}

		[Test]
		public void Parse() {
			SortOrder o = SortOrder.Parse("pepe");
			Assert.AreEqual("pepe", o.ToString());
		}

		[Test]
		public void ParseAsc() {
			SortOrder o = SortOrder.Parse("pepe asc");
			StringAssert.IsMatch("pepe asc", o.ToString());
		}

		[Test]
		public void ParseDesc() {
			SortOrder o = SortOrder.Parse("pepe desc");
			StringAssert.IsMatch("pepe desc", o.ToString());
		}

		[Test]
		[ExpectedException(typeof(InvalidSortOrderException))]
		public void InvalidParse_ShouldThrow() {
			SortOrder o = SortOrder.Parse("pepe bla");
		}
	}
}