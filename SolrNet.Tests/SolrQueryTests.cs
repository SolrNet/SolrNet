using System;
using System.Collections;
using System.Collections.Generic;
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
			SolrExecutableQuery<TestDocument> q = new SolrExecutableQuery<TestDocument>(connection, "id:123456");
			ISolrQueryResults<TestDocument> r = q.Execute();
		}
	}
}