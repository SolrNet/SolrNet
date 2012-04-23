using System;
using MbUnit.Framework;
using SolrNet.Commands;
using SolrNet.Tests.Mocks;

namespace SolrNet.Tests {
    [TestFixture]
    public class OptimizeCommandTests {
        [Test]
        public void ExecuteBasic() {
            var conn = new MSolrConnection();
            conn.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<optimize />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new OptimizeCommand();
            cmd.Execute(conn);
            Assert.AreEqual(1, conn.post.Calls);
        }

        [Test]
        public void ExecuteWithBasicOptions() {
            var conn = new MSolrConnection();
            conn.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<optimize waitSearcher=\"true\" waitFlush=\"true\" />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new OptimizeCommand {
                WaitFlush = true,
                WaitSearcher = true
            };
            cmd.Execute(conn);
            Assert.AreEqual(1, conn.post.Calls);
        }

        [Test]
        public void ExecuteWithAllOptions() {
            var conn = new MSolrConnection();
            conn.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<optimize waitSearcher=\"true\" waitFlush=\"true\" expungeDeletes=\"true\" maxSegments=\"2\" />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new OptimizeCommand {
                MaxSegments = 2,
                ExpungeDeletes = true,
                WaitFlush = true,
                WaitSearcher = true
            };
            cmd.Execute(conn);

            Assert.AreEqual(1, conn.post.Calls);
        }
    }
}