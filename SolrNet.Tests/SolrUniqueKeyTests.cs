using NUnit.Framework;
using Rhino.Mocks;

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