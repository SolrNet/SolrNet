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

using System;
using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.Tests {
    [TestFixture]
    public class ExtractCommandTests {
        [Test]
        public void Execute() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var parameters = new ExtractParameters(null, "1", "text.doc");

            With.Mocks(mocks).Expecting(() => {
                Expect.Call(conn.PostStream("/update/extract", null, null, new List<KeyValuePair<string, string>> {
                    KV("literal.id", parameters.Id),
                    KV("resource.name", parameters.ResourceName),
                }))
                .Repeat.Once()
                .Return("");
            })
            .Verify(() => {
                var cmd = new ExtractCommand(new ExtractParameters(null, "1", "text.doc"));
                cmd.Execute(conn);
            });
        }

        [Test]
        public void ExecuteWithAllParameters() {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();
            var parameters = new ExtractParameters(null, "1", "text.doc");

            With.Mocks(mocks).Expecting(() =>
            {
                Expect.Call(conn.PostStream("/update/extract", "application/word-document", null, new List<KeyValuePair<string, string>> {
                    KV("literal.id", parameters.Id),
                    KV("resource.name", parameters.ResourceName),
                    KV("literal.field1", "value1"),
                    KV("literal.field2", "value2"),
                    KV("stream.type", "application/word-document"),
                    KV("commit", "true"),
                    KV("uprefix", "pref"),
                    KV("defaultField", "field1"),
                    KV("extractOnly", "true"),
                    KV("extractFormat", "text"),
                    KV("capture", "html"),
                    KV("captureAttr", "true"),
                    KV("xpath", "body"),
                    KV("lowernames", "true")
                }))
                .Repeat.Once()
                .Return("");
            })
            .Verify(() =>
            {
                var cmd = new ExtractCommand(new ExtractParameters(null, "1", "text.doc")
                {
                    AutoCommit = true,
                    Capture = "html",
                    CaptureAttributes = true,
                    DefaultField = "field1",
                    ExtractOnly = true,
                    ExtractFormat = ExtractFormat.Text,
                    Fields = new[] { new ExtractField("field1", "value1"), new ExtractField("field2", "value2") },
                    LowerNames = true,
                    XPath = "body",
                    Prefix = "pref",
                    StreamType = "application/word-document"
                });
                cmd.Execute(conn);
            });
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ExecuteWithDuplicateIdField()
        {
            var mocks = new MockRepository();
            var conn = mocks.StrictMock<ISolrConnection>();

            const string DuplicateId = "duplicateId";
            var cmd = new ExtractCommand(new ExtractParameters(null, DuplicateId, "text.doc")
            {
                Fields = new[] { new ExtractField("id", DuplicateId), new ExtractField("field2", "value2") }
            });
            cmd.Execute(conn);
        }

        private static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }
    }
}
