using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrOperationsTests {
		[Test]
		public void Commit() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			ISolrOperations<ISolrDocument> ops = new SolrServer<ISolrDocument>(connection);
			ops.Commit();
		}

		[Test]
		public void CommitWithOptions() {
			MockRepository mocks = new MockRepository();
			ISolrDocument doc = mocks.CreateMock<ISolrDocument>();
			ISolrConnection connection = mocks.CreateMock<ISolrConnection>();
			Expect.Call(connection.Post("<commit waitFlush=\"true\" waitSearcher=\"true\"/>")).Repeat.Once().Return(null);
			mocks.ReplayAll();
			ISolrOperations<ISolrDocument> ops = new SolrServer<ISolrDocument>(connection);
			ops.Commit(true, true);
			mocks.VerifyAll();
		}

		[Test]
		public void SearchResults_ShouldBeIterable() {
			MockRepository mocks = new MockRepository();
			ISolrQueryResults<ISolrDocument> results = mocks.CreateMock<ISolrQueryResults<ISolrDocument>>();
			Assert.IsInstanceOfType(typeof(IEnumerable<ISolrDocument>), results);
		}

		[Test]
		[Category("Integration")]
		public void DeleteAll() {
			
		}
	}
}