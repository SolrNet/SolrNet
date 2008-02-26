using NUnit.Framework;
using Rhino.Mocks;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void tt() {
			var mocks = new MockRepository();
			var connection = mocks.CreateMock<ISolrConnection>();
			var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(connection.Get(null, null)).IgnoreArguments().Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(new SolrQueryResults<TestDocument>());
			}).Verify(delegate {
				var q = new SolrQueryExecuter<TestDocument>(connection, "id:123456") {ResultParser = parser};
				var r = q.Execute();
			});
		}
	}
}