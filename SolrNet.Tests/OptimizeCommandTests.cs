using System;
using Xunit;
using SolrNet.Commands;
using SolrNet.Tests.Mocks;

namespace SolrNet.Tests {
    
    public class OptimizeCommandTests {
        [Fact]
        public void ExecuteBasic() {
            var conn = new MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<optimize />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new OptimizeCommand();
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void ExecuteWithBasicOptions() {
            var conn = new MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<optimize waitSearcher=\"true\" waitFlush=\"true\" />", content);
                Console.WriteLine(content);
                return null;
            };
            var cmd = new OptimizeCommand {
                WaitFlush = true,
                WaitSearcher = true
            };
            cmd.Execute(conn);
            Assert.Equal(1, conn.post.Calls);
        }

        [Fact]
        public void ExecuteWithAllOptions() {
            var conn = new MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<optimize waitSearcher=\"true\" waitFlush=\"true\" expungeDeletes=\"true\" maxSegments=\"2\" />", content);
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

            Assert.Equal(1, conn.post.Calls);
        }
    }
}