using System.Linq;
using NUnit.Framework;
using SolrNet.Attributes;

namespace SolrNet.Tests {
	[TestFixture]
	public class AttributesMappingManagerTests {
		[Test]
		public void GetFields() {
			var m = new AttributesMappingManager();
			var fields = m.GetFields(typeof (Entity));
			Assert.AreEqual(2, fields.Count);
			foreach (var f in fields) {
				if (f.Value == "Id")
					Assert.AreEqual("Id", f.Key.Name);
				else if (f.Value == "desc")
					Assert.AreEqual("Description", f.Key.Name);
				else
					Assert.Fail("Invalid field '{0}'", f.Value);
			}
		}

		[Test]
		public void GetUniqueKey() {
			var m = new AttributesMappingManager();
			var key = m.GetUniqueKey(typeof (Entity));
			Assert.IsNotNull(key);
			Assert.IsNotNull(key.Key);
			Assert.AreEqual("Id", key.Key.Name);
			Assert.AreEqual("Id", key.Value);
		}

		public class Entity {
			[SolrUniqueKey]
			public int Id { get; set; }

			[SolrField("desc")]
			public string Description { get; set; }
		}
	}
}