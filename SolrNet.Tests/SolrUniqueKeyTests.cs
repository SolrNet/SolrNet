using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrUniqueKeyTests {
		public class TestDocumentWithTwoUniqueFields : ISolrDocument {
			[SolrUniqueKey]
			public string id {
				get { return ""; }
			}

			[SolrUniqueKey]
			public string id2 {
				get { return ""; }
			}
		}

		[Test]
		[ExpectedException(typeof(BadMappingException))]
		public void ClassWithTwoUniqueKeys_ShouldFailAdd() {
			TestDocumentWithTwoUniqueFields doc = new TestDocumentWithTwoUniqueFields();
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.DynamicMock<ISolrConnection>();
			SetupResult.For(conn.ServerURL).Return("");
			mocks.ReplayAll();
			AddCommand<TestDocumentWithTwoUniqueFields> cmd =
				new AddCommand<TestDocumentWithTwoUniqueFields>(new TestDocumentWithTwoUniqueFields[] {doc});
			cmd.Execute(conn);
			mocks.VerifyAll();
		}
	}
}