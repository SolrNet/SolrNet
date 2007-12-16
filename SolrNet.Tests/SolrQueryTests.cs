using NUnit.Framework;
using Rhino.Mocks;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void tt() {
			MockRepository mocks = new MockRepository();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			ISolrQueryResultParser<TestDocument> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(connection.Get(null, null)).IgnoreArguments().Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(new SolrQueryResults<TestDocument>());
			}).Verify(delegate {
				SolrQueryExecuter<TestDocument> q = new SolrQueryExecuter<TestDocument>(connection, "id:123456");
				q.ResultParser = parser;
				ISolrQueryResults<TestDocument> r = q.Execute();
			});
		}
	}
}