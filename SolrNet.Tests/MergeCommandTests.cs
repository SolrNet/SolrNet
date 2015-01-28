using System;
using System.Collections.Generic;
using System.Linq;
using MbUnit.Framework;
using SolrNet.Commands.Cores;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class MergeCommandTests {
        [Test]
        public void Index() {
            var m = new MergeCommand("core0", new MergeCommand.IndexDir("/path/to/index1"), new MergeCommand.IndexDir("/path/to/index2"));
            var parameters = m.GetParameters().ToList();
            Assert.Contains(parameters, KV.Create("action", "mergeindexes"));
            Assert.Contains(parameters, KV.Create("indexDir", "/path/to/index1"));
            Assert.Contains(parameters, KV.Create("indexDir", "/path/to/index2"));
            Assert.Contains(parameters, KV.Create("core", "core0"));
        }

        [Test]
        public void Core() {
            var m = new MergeCommand("core0", new MergeCommand.SrcCore("core1"), new MergeCommand.SrcCore("core2"));
            var parameters = m.GetParameters().ToList();
            Assert.Contains(parameters, KV.Create("action", "mergeindexes"));
            Assert.Contains(parameters, KV.Create("srcCore", "core1"));
            Assert.Contains(parameters, KV.Create("srcCore", "core2"));
            Assert.Contains(parameters, KV.Create("core", "core0"));
        }
    }
}
