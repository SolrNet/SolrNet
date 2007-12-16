using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryExecuterTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void Execute() {
			const string queryString = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			ISolrQueryResultParser<TestDocument> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			ISolrQueryResults<TestDocument> mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				IDictionary<string, string> q = new Dictionary<string, string>();
				q["q"] = queryString;
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				SolrQueryExecuter<TestDocument> queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString);
				queryExecuter.ResultParser = parser;
				ISolrQueryResults<TestDocument> r = queryExecuter.Execute();
			});
		}

		[Test]
		public void Sort() {
			const string queryString = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			ISolrQueryResultParser<TestDocument> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			ISolrQueryResults<TestDocument> mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				IDictionary<string, string> q = new Dictionary<string, string>();
				q["q"] = queryString;
				q["sort"] = "id asc";
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				SolrQueryExecuter<TestDocument> queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString);
				queryExecuter.ResultParser = parser;
				queryExecuter.OrderBy = new SortOrder[] {new SortOrder("id")};
				ISolrQueryResults<TestDocument> r = queryExecuter.Execute();
			});
		}

		[Test]
		public void SortMultipleWithOrders() {
			const string queryString = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			ISolrQueryResultParser<TestDocument> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			ISolrQueryResults<TestDocument> mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				IDictionary<string, string> q = new Dictionary<string, string>();
				q["q"] = queryString;
				q["sort"] = "id asc,name desc";
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				SolrQueryExecuter<TestDocument> queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString);
				queryExecuter.ResultParser = parser;
				queryExecuter.OrderBy = new SortOrder[] {new SortOrder("id", Order.ASC), new SortOrder("name", Order.DESC)};
				ISolrQueryResults<TestDocument> r = queryExecuter.Execute();
			});
		}

		[Test, Ignore("incomplete")]
		public void UndefinedFieldError() {
			const string queryString = "id:123456";
			MockRepository mocks = new MockRepository();
			ISolrConnection conn = mocks.CreateMock<ISolrConnection>();
			ISolrQueryResultParser<TestDocument> parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			ISolrQueryResults<TestDocument> mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				IDictionary<string, string> q = new Dictionary<string, string>();
				q["q"] = queryString;
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				SolrQueryExecuter<TestDocument> queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString);
				queryExecuter.ResultParser = parser;
				ISolrQueryResults<TestDocument> r = queryExecuter.Execute();
			});
		}
	}
}