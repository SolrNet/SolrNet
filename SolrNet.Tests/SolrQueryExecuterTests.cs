using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands.Parameters;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryExecuterTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void Execute() {
			const string queryString = "id:123456";
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				var q = new Dictionary<string, string>();
				q["q"] = queryString;
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString) {ResultParser = parser};
				var r = queryExecuter.Execute();
			});
		}

		[Test]
		public void Sort() {
			const string queryString = "id:123456";
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				var q = new Dictionary<string, string>();
				q["q"] = queryString;
				q["sort"] = "id asc";
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString) {
					ResultParser = parser,
					Options = new QueryOptions {OrderBy = new[] {new SortOrder("id")}}
				};
				var r = queryExecuter.Execute();
			});
		}

		[Test]
		public void SortMultipleWithOrders() {
			const string queryString = "id:123456";
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				var q = new Dictionary<string, string>();
				q["q"] = queryString;
				q["sort"] = "id asc,name desc";
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString) {
					ResultParser = parser,
					Options = new QueryOptions {
						OrderBy = new[] {
							new SortOrder("id", Order.ASC),
							new SortOrder("name", Order.DESC)
						}
					}
				};
				var r = queryExecuter.Execute();
			});
		}

		[Test, Ignore("incomplete")]
		public void UndefinedFieldError() {
			const string queryString = "id:123456";
			var mocks = new MockRepository();
			var conn = mocks.CreateMock<ISolrConnection>();
			var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
			var mockR = mocks.DynamicMock<ISolrQueryResults<TestDocument>>();
			With.Mocks(mocks).Expecting(delegate {
				var q = new Dictionary<string, string>();
				q["q"] = queryString;
				Expect.Call(conn.Get("/select", q)).Repeat.Once().Return("");
				Expect.Call(parser.Parse(null)).IgnoreArguments().Repeat.Once().Return(mockR);
			}).Verify(delegate {
				var queryExecuter = new SolrQueryExecuter<TestDocument>(conn, queryString) {
					ResultParser = parser
				};
				var r = queryExecuter.Execute();
			});
		}
	}
}