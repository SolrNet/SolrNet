using System;
using Xunit;
using SolrNet.Commands;
using SolrNet.Tests.Mocks;
using Xunit.Abstractions;

namespace SolrNet.Tests {
    
    public class OptimizeCommandTests {
        private readonly ITestOutputHelper testOutputHelper;

        public OptimizeCommandTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void ExecuteBasic() {
            var conn = new MSolrConnection();
            conn.post += (url, content) => {
                Assert.Equal("/update", url);
                Assert.Equal("<optimize />", content);
                testOutputHelper.WriteLine(content);
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
                testOutputHelper.WriteLine(content);
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
                testOutputHelper.WriteLine(content);
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
