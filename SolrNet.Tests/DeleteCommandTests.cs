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
using Rhino.Mocks;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.Tests {
	[TestFixture]
	public class DeleteCommandTests {
		[Test]
		public void DeleteById() {
			const string id = "123123";
			var mocks = new MockRepository();
			var conn = mocks.StrictMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(() => {
				Expect.Call(conn.Post("/update", string.Format("<delete><id>{0}</id></delete>", id)))
                    .Repeat.Once()
                    .Return("");
            }).Verify(() => {
                var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] {id}, null, null));
				cmd.Execute(conn);
			});
		}

        [Test]
        public void DeleteByMultipleId() {
            var ids = new[] {"123", "456"};
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks).Expecting(delegate {
                Expect.Call(conn.Post("/update", string.Format("<delete><id>{0}</id><id>{1}</id></delete>", ids[0], ids[1]))).Repeat.Once().Return("");
            }).Verify(delegate {
                var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, null, null));
                cmd.Execute(conn);
            });            
        }

		[Test]
		public void DeleteByQuery() {
			var mocks = new MockRepository();
			var conn = mocks.StrictMock<ISolrConnection>();
			var q = mocks.StrictMock<ISolrQuery>();
		    var querySerializer = mocks.StrictMock<ISolrQuerySerializer>();
			With.Mocks(mocks).Expecting(delegate {
				const string queryString = "someQuery";
				Expect.On(conn)
                    .Call(conn.Post("/update", string.Format("<delete><query>{0}</query></delete>", queryString)))
                    .Repeat.Once()
                    .Return("");
			    Expect.On(querySerializer)
			        .Call(querySerializer.Serialize(null))
			        .IgnoreArguments()
                    .Return(queryString);
			}).Verify(delegate {
                var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(null, q, querySerializer));
				cmd.Execute(conn);
			});
		}

        [Test]
        public void DeleteByIdAndQuery() {
            var ids = new[] { "123", "456" };
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var q = mocks.StrictMock<ISolrQuery>();
            var querySerializer = mocks.StrictMock<ISolrQuerySerializer>();
            With.Mocks(mocks).Expecting(delegate {
                const string queryString = "someQuery";
                Expect.On(querySerializer)
                    .Call(querySerializer.Serialize(null))
                    .IgnoreArguments()
                    .Return(queryString);
                Expect.On(conn)
                    .Call(conn.Post("/update", string.Format("<delete><id>{0}</id><id>{1}</id><query>{2}</query></delete>", ids[0], ids[1], queryString)))
                    .Repeat.Once()
                    .Return("");
            }).Verify(delegate {
                var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(ids, q, querySerializer));
                cmd.Execute(conn);
            });
        }

		[Test]
		public void DeleteFromCommitted() {
			const string id = "123123";
			var mocks = new MockRepository();
			var conn = mocks.StrictMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.On(conn).Call(conn.Post("/update", string.Format("<delete fromCommitted=\"true\"><id>{0}</id></delete>", id)))
                    .Repeat.Once()
                    .Return("");
			}).Verify(delegate {
                var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, null)) {
                    FromCommitted = true
                };
				cmd.Execute(conn);
			});
		}

		[Test]
		public void DeleteFromCommittedAndFromPending() {
			const string id = "123123";
			var mocks = new MockRepository();
			var conn = mocks.StrictMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.On(conn).Call(conn.Post("/update", string.Format("<delete fromPending=\"false\" fromCommitted=\"false\"><id>{0}</id></delete>", id)))
                    .Repeat.Once()
                    .Return("");
			}).Verify(delegate {
                var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, null)) {
                    FromCommitted = false, 
                    FromPending = false
                };
				cmd.Execute(conn);
			});
		}

		[Test]
		public void DeleteFromPending() {
			const string id = "123123";
			var mocks = new MockRepository();
			var conn = mocks.StrictMock<ISolrConnection>();
			With.Mocks(mocks).Expecting(delegate {
				Expect.On(conn).Call(conn.Post("/update", string.Format("<delete fromPending=\"true\"><id>{0}</id></delete>", id)))
                    .Repeat.Once()
                    .Return("");
			}).Verify(delegate {
                var cmd = new DeleteCommand(new DeleteByIdAndOrQueryParam(new[] { id }, null, null)) {
                    FromPending = true
                };
				cmd.Execute(conn);
			});
		}
	}
}