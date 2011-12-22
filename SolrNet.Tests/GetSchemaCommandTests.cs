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
using SolrNet.Commands;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class GetSchemaCommandTests {
        [Test]
        public void GetSchemaCommand() {
            var conn = new Mocks.MSolrConnection();
            conn.get += (url, kvs) => {
                Assert.AreEqual("/admin/file", url);
                var expectedKVs = new[] {KV.Create("file", "schema.xml")};
                Assert.AreElementsEqualIgnoringOrder(expectedKVs, kvs);
                return "";
            };
            var cmd = new GetSchemaCommand();
            cmd.Execute(conn);
            Assert.AreEqual(1, conn.get.Calls);
        }
    }
}