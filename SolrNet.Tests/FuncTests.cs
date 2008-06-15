using System.Collections.Generic;
using NUnit.Framework;
using SolrNet.Utils;

namespace SolrNet.Tests {
	[TestFixture]
	public class FuncTests {
		[Test]
		public void Take() {
			var l = new[] {1, 2, 3, 4, 5};
			var r = new List<int>(Func.Take(l, 2));
			Assert.AreEqual(2, r.Count);
			Assert.AreEqual(1, r[0]);
			Assert.AreEqual(2, r[1]);
		}

		[Test]
		public void TakeMore() {
			var l = new[] { 1, 2, 3, 4, 5 };
			var r = new List<int>(Func.Take(l, 200));
			Assert.AreEqual(5, r.Count);
			Assert.AreEqual(1, r[0]);
			Assert.AreEqual(2, r[1]);
		}

	}
}