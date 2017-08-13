using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using SolrNet.Commands.Cores;
using SolrNet.Utils;

namespace SolrNet.Tests {
    
    public class MergeCommandTests {
        [Fact]
        public void Index() {
            var m = new MergeCommand("core0", new MergeCommand.IndexDir("/path/to/index1"), new MergeCommand.IndexDir("/path/to/index2"));
            var parameters = m.GetParameters().ToList();
            Assert.Contains( KV.Create("action", "mergeindexes"),parameters);
            Assert.Contains( KV.Create("indexDir", "/path/to/index1"),parameters);
            Assert.Contains( KV.Create("indexDir", "/path/to/index2"),parameters);
            Assert.Contains( KV.Create("core", "core0"),parameters);
        }

        [Fact]
        public void Core() {
            var m = new MergeCommand("core0", new MergeCommand.SrcCore("core1"), new MergeCommand.SrcCore("core2"));
            var parameters = m.GetParameters().ToList();
            Assert.Contains( KV.Create("action", "mergeindexes"),parameters);
            Assert.Contains( KV.Create("srcCore", "core1"),parameters);
            Assert.Contains( KV.Create("srcCore", "core2"),parameters);
            Assert.Contains( KV.Create("core", "core0"),parameters);
        }
    }
}
