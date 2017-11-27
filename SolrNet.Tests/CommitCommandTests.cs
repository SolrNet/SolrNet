using System;
using Xunit;
using SolrNet.Commands;

namespace SolrNet.Tests {
    
    public class CommitCommandTests {
        [Fact]
        public void ExecuteBasic() {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<commit />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new CommitCommand();
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void ExecuteWithBasicOptions() {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<commit waitSearcher=\"true\" waitFlush=\"true\" />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new CommitCommand { WaitFlush = true, WaitSearcher = true };
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void ExecuteWithAllOptions() {
            var conn = new Mocks.MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<commit waitSearcher=\"true\" waitFlush=\"true\" expungeDeletes=\"true\" maxSegments=\"2\" />", content);
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
            Assert.Equal(1, conn.post.Calls);
        }
    }
}