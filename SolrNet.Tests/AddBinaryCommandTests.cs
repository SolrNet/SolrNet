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

using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.Tests {
    [TestFixture]
    public class AddBinaryCommandTests {
        [Test]
        public void Execute() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            With.Mocks(mocks).Expecting(() =>
            {
                Expect.Call(conn.PostBinary("/update/extract", null, new List<KeyValuePair<string, string>>()))
                    .Repeat.Once()
                    .Return("");
            }).Verify(() =>
            {
                var cmd = new AddBinaryCommand(new AddBinaryParameters(null, "1", "text.doc"));
                cmd.Execute(conn);
            });
        }
    }
}
