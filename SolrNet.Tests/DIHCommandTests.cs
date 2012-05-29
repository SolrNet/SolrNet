using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using SolrNet.Commands;
using SolrNet.Tests.Mocks;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class DIHCommandTests {
        private readonly IEnumerable<KeyValuePair<string, string>> emptyParameters = new KeyValuePair<string, string>[0]; 

        [Test]
        public void BasicDataImportTest() {
            var conn = AssertGet(new DIHCommand(), "/dataimport", emptyParameters);
            Assert.AreEqual(1, conn.get.Calls);
        }

        [Test]
        public void NamedDataImportTest() {
            var cmd = new DIHCommand {HandlerName = "custom-name"};
            var conn = AssertGet(cmd, "/custom-name", emptyParameters);
            Assert.AreEqual(1, conn.get.Calls);
        }

        [Test]
        public void CommandTypeFullImportTest() {
            var cmd = new DIHCommand {CommandType = DIHCommand.DIHCommandType.FullImport};
            var conn = AssertGet(cmd, "/dataimport", new[] {
                KV.Create("command", "full-import")
            });
            Assert.AreEqual(1, conn.get.Calls);
        }

        [Test]
        public void CommandTypeDelatImportTest() {
            var cmd = new DIHCommand {CommandType = DIHCommand.DIHCommandType.DeltaImport};
            var expectedParameters = new[] {
                KV.Create("command", "delta-import")
            };
            var conn = AssertGet(cmd, "/dataimport", expectedParameters);
            Assert.AreEqual(1, conn.get.Calls);
        }

        private static MSolrConnection AssertGet(DIHCommand cmd, string expectedUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            return AssertGet(cmd, (s, pairs) => {
                Assert.AreEqual(expectedUrl, s);
                Assert.AreElementsEqualIgnoringOrder(parameters, pairs);
                Console.WriteLine(parameters);
                return null;
            });
        }

        private static MSolrConnection AssertGet(DIHCommand cmd, Func<string, IEnumerable<KeyValuePair<string, string>>, string> get = null, Func<string, IEnumerable<KeyValuePair<string, string>>, string> post = null) {
            var conn = new MSolrConnection();
            conn.get += get;
            cmd.Execute(conn);
            return conn;
        }
    }
}