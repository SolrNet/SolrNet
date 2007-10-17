using System.Reflection;
using NUnit.Framework;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	[TestFixture]
	public class UniquePropertyFinderTests {
		public class DocWithoutUniqueKey : ISolrDocument {
			public int id {
				get { return 0; }
			}
		}

		public class DocWithTwoUniqueKeys : ISolrDocument {
			[SolrUniqueKey]
			public string id {
				get { return ""; }
			}

			[SolrUniqueKey]
			public string id2 {
				get { return ""; }
			}
		}

		public class DocWithOneUniqueKey : ISolrDocument {
			[SolrUniqueKey]
			public string id {
				get { return ""; }
			}

			public string id2 {
				get { return ""; }
			}
		}

		[Test]
		public void FindOneUniqueKey() {
			IUniqueKeyFinder<DocWithOneUniqueKey> f = new UniqueKeyFinder<DocWithOneUniqueKey>();
			Assert.AreEqual("id", f.UniqueKeyProperty.Name);
		}

		[Test]
		public void NoUniqueKey() {
			IUniqueKeyFinder<DocWithoutUniqueKey> f = new UniqueKeyFinder<DocWithoutUniqueKey>();
			Assert.IsNull(f.UniqueKeyProperty);
		}

		[Test]
		[ExpectedException(typeof (BadMappingException))]
		public void ClassWithTwoUniqueKeys_ShouldFailAdd() {
			IUniqueKeyFinder<DocWithTwoUniqueKeys> f = new UniqueKeyFinder<DocWithTwoUniqueKeys>();
			PropertyInfo info = f.UniqueKeyProperty;
		}
	}
}