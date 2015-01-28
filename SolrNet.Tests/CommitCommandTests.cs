using System;
using MbUnit.Framework;
using SolrNet.Commands;

namespace SolrNet.Tests {
    [TestFixture]
    public class CommitCommandTests {
        [Test]
        public void ExecuteBasic() {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<commit />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new CommitCommand();
            cmd.Execute(conn);
            Assert.AreEqual(1, conn.post.Calls);
        }

        [Test]
        public void ExecuteWithBasicOptions() {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<commit waitSearcher=\"true\" waitFlush=\"true\" />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new CommitCommand { WaitFlush = true, WaitSearcher = true };
            cmd.Execute(conn);
            Assert.AreEqual(1, conn.post.Calls);
        }

        [Test]
        public void ExecuteWithAllOptions() {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.AreEqual("/update", url);
                Assert.AreEqual("<commit waitSearcher=\"true\" waitFlush=\"true\" expungeDeletes=\"true\" maxSegments=\"2\" />", content);
                Console.WriteLine(content);
                return null;
            };

            var cmd = new CommitCommand {
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