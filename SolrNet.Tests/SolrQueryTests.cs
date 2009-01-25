using System;
using NUnit.Framework;
using Rhino.Mocks;
using SolrNet.Utils;

namespace SolrNet.Tests {
	[TestFixture]
	public class SolrQueryTests {
		public class TestDocument : ISolrDocument {}

		[Test]
		public void tt() {
			var mocks = new MockRepository();
		    var container = mocks.CreateMock<IServiceProvider>();
            Factory.Init(container);
			var connection = mocks.CreateMock<ISolrConnection>();
		    var listRnd = mocks.CreateMock<IListRandomizer>();
			var parser = mocks.CreateMock<ISolrQueryResultParser<TestDocument>>();
		    var mapper = mocks.CreateMock<IReadOnlyMappingManager>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.Call(connection.Get(null, null))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return("");
				Expect.Call(parser.Parse(null))
                    .IgnoreArguments()
                    .Repeat.Once()
                    .Return(new SolrQueryResults<TestDocument>());
			    Expect.Call(container.GetService(typeof (IListRandomizer)))
			        .Return(listRnd);
			}).Verify(delegate {
			    var q = new SolrQueryExecuter<TestDocument>(connection, parser, mapper);
				var r = q.Execute(new SolrQuery("id:123456"), null);
			});
		}
	}
}