using SolrNet.Impl;
using Xunit;

namespace SolrNet.Tests
{
    public class SolrRequestOptionsTests
    {
        /// <summary>
        /// Check if the SolrConnectionTimeout func can be set and read
        /// </summary>
        [Fact]
        public void CanGetAndSetSolrConnectionTimeout()
        {
            //ARRANGE
            var options = new SolrRequestOptions();
            var expectation = 10;
            
            //ACT
            options.SolrConnectionTimeout = () => 10;

            //ASSERT
            Assert.NotNull(options.SolrConnectionTimeout);
            Assert.Equal(expectation, options.SolrConnectionTimeout());
        }
        
        
        /// <summary>
        /// Check if the SolrConnectionTimeout func can be set and read
        /// </summary>
        [Fact]
        public void SolrConnectionTimeout_CanBeNull()
        {
            //ARRANGE
            var options = new SolrRequestOptions();

            //ASSERT
            Assert.Null(options.SolrConnectionTimeout);
        }
    }
}
