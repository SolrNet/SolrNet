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
using Xunit;
using SolrNet.Commands;
using Moroco;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SolrNet.Tests
{
    public class AtomicUpdateCommandTests {
		[Fact]
		public void AtomicUpdateSet() {
		    var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
		        Assert.Equal("[{\"id\":\"0\",\"animal\":{\"set\":\"squirrel\"}}]", text);
		        Console.WriteLine(text);
                return null;
		    });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("animal", AtomicUpdateType.Set, "squirrel") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        public void AtomicUpdateSetArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"animal\":{\"set\":[\"squirrel\",\"fox\"]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("animal", AtomicUpdateType.Set, new string[] {"squirrel", "fox"}) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        public void AtomicUpdateSetEmptyArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"animal\":{\"set\":[]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("animal", AtomicUpdateType.Set, new string[] {}) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateAdd()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"add\":\"nuts\"}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.Add, "nuts") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateAddArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"add\":[\"nuts\",\"seeds\",\"berries\"]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.Add, new string[] { "nuts", "seeds", "berries" }) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateAddEmptyArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"add\":[]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.Add, new string[] { }) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateRemove()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"remove\":\"nuts\"}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.Remove, "nuts") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateRemoveArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"remove\":[\"nuts\",\"seeds\",\"berries\"]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.Remove, new string[] { "nuts", "seeds", "berries" }) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateRemoveEmptyArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"remove\":[]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.Remove, new string[] { }) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateRemoveRegex()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"removeregex\":\"nu.*\"}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.RemoveRegex, "nu.*") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateRemoveRegexArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"removeregex\":[\"nu.*\",\"seeds\",\"berr.+\"]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.RemoveRegex, new string[] { "nu.*", "seeds", "berr.+" }) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateRemoveRegexEmptyArray()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"food\":{\"removeregex\":[]}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.RemoveRegex, new string[] { }) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateInc()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{\"id\":\"0\",\"count\":{\"inc\":3}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, 3) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateWithParameter()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("commitwithin", ((KeyValuePair<string, string>[])param)[0].Key);
                Assert.Equal("4343", ((KeyValuePair<string, string>[]) param)[0].Value);
                Assert.Equal("[{\"id\":\"0\",\"count\":{\"inc\":3}}]", text);
                Console.WriteLine(text);
                return null;
            });
            var parameters = new AtomicUpdateParameters { CommitWithin = 4343 };
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, 3) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, parameters);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }

        [Fact]
        public void AtomicUpdateNullUniqueKeyHandledGracefully()
        {
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, "3") };
            Assert.Throws<ArgumentNullException>(() => new AtomicUpdateCommand(null, "0", updateSpecs, null));
        }

        [Fact]
        public void AtomicUpdateNullIdHandledGracefully()
        {
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, "3") };
            Assert.Throws<ArgumentNullException>(() => new AtomicUpdateCommand("id", null, updateSpecs, null));
        }

        [Fact]
        public void AtomicUpdateNullSpecHandledGracefully()
        {
            Assert.Throws<ArgumentNullException>(() => new AtomicUpdateCommand("id", "0", null, null));
        }

        [Fact]
        public void AtomicUpdateSerialization()
        {
            var conn = new Mocks.MSolrConnection();
            conn.postStream += new MFunc<string, string, Stream, IEnumerable<KeyValuePair<string, string>>, string>((url, contentType, content, param) => {
                string text = new StreamReader(content, Encoding.UTF8).ReadToEnd();
                Assert.Equal("/update", url);
                Assert.Equal("[{"
                    + "\"id\":\"0\","
                    + "\"quote\":{\"set\":\"\\\"quoted\\\"\"},"
                    + "\"backslashInText\":{\"set\":\"y\\\\n\"},"
                    + "\"newLineInText\":{\"set\":\"line1\\nline2\"}"
                    + "}]",
                    text);
                Console.WriteLine(text);
                return null;
            });
            /* This document is equivalent to:
            * {
            *   id = 0;
            *   quote = "quoted";
            *   backslashInText = y\n;
            *   newLineInText = line1
            *   line2;
            * }
            */
            var updateSpecs = new AtomicUpdateSpec[] {
                new AtomicUpdateSpec("quote", AtomicUpdateType.Set, "\"quoted\""),
                new AtomicUpdateSpec("backslashInText", AtomicUpdateType.Set, "y\\n"),
                new AtomicUpdateSpec("newLineInText", AtomicUpdateType.Set, "line1\nline2")
            };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.postStream.Calls);
        }
    }
}
