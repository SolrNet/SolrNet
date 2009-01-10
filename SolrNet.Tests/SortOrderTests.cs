using NUnit.Framework;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	[TestFixture]
	public class SortOrderTests {
		[Test]
		[ExpectedException(typeof (InvalidSortOrderException))]
		public void Constructor_ShouldntAcceptSpaces() {
			var o = new SortOrder("uno dos");
		}

		[Test]
		public void DefaultOrder() {
			var o = new SortOrder("uno");
			Assert.AreEqual("uno asc", o.ToString());
		}

		[Test]
		[ExpectedException(typeof (InvalidSortOrderException))]
		public void ParseNull_ShouldThrow() {
			var o = SortOrder.Parse(null);
		}

        [Test]
        [ExpectedException(typeof(InvalidSortOrderException))]
        public void ParseEmpty_ShouldThrow() {
            var o = SortOrder.Parse("");
        }

		[Test]
		public void Parse() {
			var o = SortOrder.Parse("pepe");
			Assert.AreEqual("pepe asc", o.ToString());
		}

		[Test]
		public void ParseAsc() {
			var o = SortOrder.Parse("pepe asc");
			StringAssert.IsMatch("pepe asc", o.ToString());
		}

		[Test]
		public void ParseDesc() {
			var o = SortOrder.Parse("pepe desc");
			StringAssert.IsMatch("pepe desc", o.ToString());
		}

		[Test]
		[ExpectedException(typeof (InvalidSortOrderException))]
		public void InvalidParse_ShouldThrow() {
			var o = SortOrder.Parse("pepe bla");
		}
	}
}