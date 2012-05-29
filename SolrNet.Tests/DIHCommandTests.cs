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
        public void ConstructorDefaultValueTest() {
            var cmd = new DIHCommand();
            Assert.IsFalse(cmd.Clean.HasValue);
            Assert.IsFalse(cmd.Command.HasValue);
            Assert.AreEqual("dataimport", cmd.HandlerName);
        }

        [Test]
        public void BasicDataImportTest() {
            var conn = AssertGet(new DIHCommand(), "/dataimport", emptyParameters);
            Assert.AreEqual(1, conn.get.Calls);
        }

        [Test]
        public void NamedHandlerTest() {
            var cmd = new DIHCommand {HandlerName = "custom-name"};
            AssertGet(cmd, "/custom-name", emptyParameters);
        }

        [Test]
        public void FullImportTest() {
            var cmd = new DIHCommand {Command = DIHCommands.FullImport};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("command", "full-import")
            });
        }

        [Test]
        public void DeltaImportTest() {
            var cmd = new DIHCommand {Command = DIHCommands.DeltaImport};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("command", "delta-import")
            });
        }

        [Test]
        public void ReloadConfigTest() {
            var cmd = new DIHCommand {Command = DIHCommands.ReloadConfig};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("command", "reload-config")
            });
        }

        [Test]
        public void AbortTest() {
            var cmd = new DIHCommand {Command = DIHCommands.Abort};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("command", "abort")
            });
        }

        [Test]
        public void CleanTrueTest() {
            var cmd = new DIHCommand {Clean = true};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("clean", "true")
            });
        }

        [Test]
        public void CleanFalseTest() {
            var cmd = new DIHCommand {Clean = false};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("clean", "false")
            });
        }

        [Test]
        public void CommitTest() {
            var cmd = new DIHCommand {Commit = false};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("commit", "false")
            });
        }

        [Test]
        public void OptimizeTest() {
            var cmd = new DIHCommand {Optimize = false};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("optimize", "false")
            });
        }

        [Test]
        public void DebugTest() {
            var cmd = new DIHCommand {Debug = true};
            AssertGet(cmd, "/dataimport", new[] {
                KV.Create("debug", "true")
            });
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