#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using Microsoft.Practices.ServiceLocation;
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
			var connection = mocks.CreateMock<ISolrConnection>();
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
			}).Verify(delegate {
			    var q = new SolrQueryExecuter<TestDocument>(connection, parser);
				var r = q.Execute(new SolrQuery("id:123456"), null);
			});
		}
	}
}