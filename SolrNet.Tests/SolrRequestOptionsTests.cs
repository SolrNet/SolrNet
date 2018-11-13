using System.Collections.Generic;
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
        public void SolrConnectionTimeout_CanSetAndGet()
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
        
        /// <summary>
        /// Check if the SolrConnectionTimeout func can be set and read
        /// </summary>
        [Fact]
        public void SolrRequestOptions_AreApplied()
        {
            //ARRANGE
            var options = new SolrRequestOptions();
            var expectation = 10;
            options.SolrConnectionTimeout = () => 10;
            var relativeUrl = "igonowhere/foo";
            var parameters = new List<KeyValuePair<string, string>>();
            var mockConn = new MockConnection();
            var sut = new PostSolrConnection(mockConn, "FooBarConnString");
            
            //ACT
            var (request, queryString)= sut.PrepareGet(relativeUrl, parameters, options);

            //ASSERT
            Assert.NotNull(request);
            Assert.Equal(expectation, request.Timeout);
        }
        
        /// <summary>
        /// Check if the SolrConnectionTimeout func can be set and read
        /// </summary>
        [Fact]
        public void SolrRequestOptions_IsNull_ShouldNotThrow()
        {
            //ARRANGE
            var relativeUrl = "igonowhere/foo";
            var parameters = new List<KeyValuePair<string, string>>();
            var mockConn = new MockConnection();
            var sut = new PostSolrConnection(mockConn, "FooBarConnString");
            //ACT
            //Should not throw
            var ex = Record.Exception(() => sut.PrepareGet(relativeUrl, parameters, null));

            //ASSERT
            Assert.Null(ex);

        }
        
        /// <summary>
        /// Check if the SolrConnectionTimeout func can be set and read
        /// </summary>
        [Fact]
        public void SolrRequestOptions_TimeoutNotSet_ShouldNotSetRequestTimeout()
        {
            //ARRANGE
            var options = new SolrRequestOptions();
            var defaultValue = 100000; //Default value of HttpWebRequest timeout in ms
            var relativeUrl = "igonowhere/foo";
            var parameters = new List<KeyValuePair<string, string>>();
            var mockConn = new MockConnection();
            var sut = new PostSolrConnection(mockConn, "FooBarConnString");
            
            //ACT
            var (request, queryString)= sut.PrepareGet(relativeUrl, parameters, options);

            //ASSERT
            Assert.NotNull(request);
            Assert.Equal(defaultValue, request.Timeout);

        }
    }
}
