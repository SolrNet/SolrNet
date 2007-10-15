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
			Expect.Call(connection.Get(null, null)).IgnoreArguments().Repeat.Once().Return("");
			ISolrQueryResultParser<TestDocument> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(new SolrQueryResults<TestDocument>());
			mocks.ReplayAll();
			SolrExecutableQuery<TestDocument> q = new SolrExecutableQuery<TestDocument>(connection, "id:123456");
			q.ResultParser = parser;
			ISolrQueryResults<TestDocument> r = q.Execute();
			mocks.VerifyAll();
		}
	}
}