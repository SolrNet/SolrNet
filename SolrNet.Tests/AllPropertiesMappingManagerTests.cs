using NUnit.Framework;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	[TestFixture]
	public class AllPropertiesMappingManagerTests {
		[Test]
		public void GetFields() {
			var m = new AllPropertiesMappingManager();
			var fields = m.GetFields(typeof(Entity));
			Assert.AreEqual(2, fields.Count);
			foreach (var f in fields) {
				if (f.Value == "Id")
					Assert.AreEqual("Id", f.Key.Name);
				else if (f.Value == "Description")
					Assert.AreEqual("Description", f.Key.Name);
				else
					Assert.Fail("Invalid field '{0}'", f.Value);
			}
		}

		[Test]
		public void Get_and_set_unique_key() {
			var m = new AllPropertiesMappingManager();
			m.SetUniqueKey(typeof(Entity).GetProperty("Id"));
			var pk = m.GetUniqueKey(typeof (Entity));
			Assert.IsNotNull(pk);
			Assert.IsNotNull(pk.Key);
			Assert.IsNotNull(pk.Value);
			Assert.AreEqual("Id", pk.Key.Name);
			Assert.AreEqual("Id", pk.Value);
		}

        [Test]
        [ExpectedException(typeof(NoUniqueKeyException))]
        public void NoUniqueKey_ShouldThrow() {
            var m = new AllPropertiesMappingManager();
            var pk = m.GetUniqueKey(typeof(Entity));
        }

		public class Entity {
			public int Id { get; set; }

			public string Description { get; set; }
		}
	}
}