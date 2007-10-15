using NUnit.Framework;
using Rhino.Mocks;

namespace SolrNet.Tests {
	[TestFixture]
	public class DeleteCommandTests {
		[Test]
		public void DeleteById() {
			const string id = "123123";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			Expect.Call(conn.Post(string.Format("<delete><id>{0}</id></delete>", id))).Repeat.Once().Return("");
			mocks.ReplayAll();
			DeleteCommand cmd = new DeleteCommand();
			cmd.DeleteParam = new DeleteByIdParam(id);
			cmd.Execute(conn);
			mocks.VerifyAll();
		}

		[Test]
		public void DeleteByQuery() {
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			ISolrQuery<ISolrDocument> q = mocks.CreateMock<ISolrQuery<ISolrDocument>>();
			const string queryString = "someQuery";
			Expect.Call(q.Query).Repeat.Once().Return(queryString);
			Expect.Call(conn.Post(string.Format("<delete><query>{0}</query></delete>", queryString))).Repeat.Once().Return("");
			mocks.ReplayAll();
			DeleteCommand cmd = new DeleteCommand();
			cmd.DeleteParam = new DeleteByQueryParam<ISolrDocument>(q);
			cmd.Execute(conn);
			mocks.VerifyAll();
		}
	}
}