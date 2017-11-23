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
using Xunit;
using Moroco;
using SolrNet.Attributes;
using SolrNet.Commands;
using SolrNet.Impl;
using SolrNet.Impl.FieldSerializers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
	public class AtomicUpdateCommandTests {
		[Fact]
		public void AtomicUpdateSet() {
		    var conn = new Mocks.MSolrConnection();
		    conn.post += (url, content) => {
		        Assert.Equal("/update", url);
		        Assert.Equal("<add overwrite=\"true\"><doc><field name=\"id\">0</field><field name=\"animal\" update=\"set\">squirrel</field></doc></add>", content);
		        Console.WriteLine(content);
		        return null;
		    };
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("animal", AtomicUpdateType.Set, "squirrel") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void AtomicUpdateAdd()
        {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<add overwrite=\"true\"><doc><field name=\"id\">0</field><field name=\"food\" update=\"add\">nuts</field></doc></add>", content);
                Console.WriteLine(content);
                return null;
            };
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("food", AtomicUpdateType.Add, "nuts") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void AtomicUpdateInc()
        {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<add overwrite=\"true\"><doc><field name=\"id\">0</field><field name=\"count\" update=\"inc\">3</field></doc></add>", content);
                Console.WriteLine(content);
                return null;
            };
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, "3") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void AtomicUpdateNull()
        {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<add overwrite=\"true\"><doc><field name=\"id\">0</field><field name=\"animal\" null=\"true\" /></doc></add>", content);
                Console.WriteLine(content);
                return null;
            };
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("animal", AtomicUpdateType.Inc, null) };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, null);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void AtomicUpdateWithParameter()
        {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<add commitWithin=\"4343\" overwrite=\"true\"><doc><field name=\"id\">0</field><field name=\"count\" update=\"inc\">3</field></doc></add>", content);
                Console.WriteLine(content);
                return null;
            };
            var parameters = new AtomicUpdateParameters { CommitWithin = 4343 };
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, "3") };
            var cmd = new AtomicUpdateCommand("id", "0", updateSpecs, parameters);
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void AtomicUpdateNullUniqueKeyHandledGracefully()
        {
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, "3") };
            var cmd = new AtomicUpdateCommand(null, "0", updateSpecs, null);
            Assert.Throws<ArgumentNullException>(() => cmd.Execute(new Mocks.MSolrConnection()));
        }

        [Fact]
        public void AtomicUpdateNullIdHandledGracefully()
        {
            var updateSpecs = new AtomicUpdateSpec[] { new AtomicUpdateSpec("count", AtomicUpdateType.Inc, "3") };
            var cmd = new AtomicUpdateCommand("id", null, updateSpecs, null);
            Assert.Throws<ArgumentNullException>(() => cmd.Execute(new Mocks.MSolrConnection()));
        }

        [Fact]
        public void AtomicUpdateNullSpecHandledGracefully()
        {
            var cmd = new AtomicUpdateCommand("id", "0", null, null);
            Assert.Throws<NullReferenceException>(() => cmd.Execute(new Mocks.MSolrConnection()));
        }
    }
}