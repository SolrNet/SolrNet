using System;
using System.Linq;
using NUnit.Framework;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrFacetFieldQueryTests {
		[Test]
		public void FieldOnly() {
			var fq = new SolrFacetFieldQuery("pepe");
			var q = fq.Query.ToList();
			Assert.AreEqual(1, q.Count);
			Console.WriteLine(q[0]);
			Assert.AreEqual("facet.field", q[0].Key);
			Assert.AreEqual("pepe", q[0].Value);
		}
	}
}