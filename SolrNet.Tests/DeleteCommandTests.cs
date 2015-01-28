#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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

using MbUnit.Framework;
using Moroco;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;

namespace SolrNet.Tests {
	[TestFixture]
	public class DeleteCommandTests {
		[Test]
		public void DeleteById() {
			const string id = "123123";
		    var conn = new Mocks.MSolrConnection();
		    conn.post = conn.post
		        .Args("/update", string.Format("<delete><id>{0}</id></delete>", id));
            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, null), null);
            cmd.Execute(conn);
            Assert.AreEqual(1, conn.post.Calls);
        }

        [Test]
        public void DeleteByMultipleId() {
            var ids = new[] {"123", "456"};
            var conn = new Mocks.MSolrConnection();
            var xml = string.Format("<delete><id>{0}</id><id>{1}</id></delete>", ids[0], ids[1]);
            conn.post = conn.post.Args("/update", xml);
            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, null, null), null);
            cmd.Execute(conn);
            Assert.AreEqual(1, conn.post.Calls);
        }

		[Test]
		public void DeleteByQuery() {
            const string queryString = "someQuery";
		    var q = new SolrQuery(queryString);
            var xml = string.Format("<delete><query>{0}</query></delete>", queryString);

		    var conn = new Mocks.MSolrConnection();
		    conn.post = conn.post.Args("/update", xml);

		    var querySerializer = new Mocks.MSolrQuerySerializer();
		    querySerializer.serialize += _ => queryString;

            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(null, q, querySerializer), null);
            cmd.Execute(conn);

            Assert.AreEqual(1, conn.post.Calls);
		}

        [Test]
        public void DeleteByIdAndQuery() {
            var ids = new[] { "123", "456" };
            const string queryString = "someQuery";
            var xml = string.Format("<delete><id>{0}</id><id>{1}</id><query>{2}</query></delete>", ids[0], ids[1], queryString);
            var conn = new Mocks.MSolrConnection();
            conn.post = conn.post.Args("/update", xml);

            var q = new SolrQuery(queryString);
            var querySerializer = new Mocks.MSolrQuerySerializer();
            querySerializer.serialize += _ => queryString;

            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, q, querySerializer), null);
            cmd.Execute(conn);

            Assert.AreEqual(1, conn.post.Calls);

        }

		[Test]
		public void DeleteFromCommitted() {
			const string id = "123123";
            var xml = string.Format("<delete fromCommitted=\"true\"><id>{0}</id></delete>", id);
		    var conn = new Mocks.MSolrConnection();
		    conn.post = conn.post.Args("/update", xml);

            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, null), null)
            {
                FromCommitted = true
            };
            cmd.Execute(conn);

            Assert.AreEqual(1, conn.post.Calls);
		}

		[Test]
		public void DeleteFromCommittedAndFromPending() {
			const string id = "123123";

		    var conn = new Mocks.MSolrConnection();
            var xml = string.Format("<delete fromPending=\"false\" fromCommitted=\"false\"><id>{0}</id></delete>", id);
		    conn.post = conn.post.Args("/update", xml);

            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, null), null)
            {
                FromCommitted = false,
                FromPending = false
            };
            cmd.Execute(conn);

            Assert.AreEqual(1, conn.post.Calls);
		}

		[Test]
		public void DeleteFromPending() {
			const string id = "123123";

		    var conn = new Mocks.MSolrConnection();
            var xml = string.Format("<delete fromPending=\"true\"><id>{0}</id></delete>", id);
		    conn.post = conn.post.Args("/update", xml);

            var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, null), null)
            {
                FromPending = true
            };
            cmd.Execute(conn);

            Assert.AreEqual(1, conn.post.Calls);
		}
	}
}