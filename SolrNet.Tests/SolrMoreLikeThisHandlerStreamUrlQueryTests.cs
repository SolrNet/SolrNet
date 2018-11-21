using System;
using Xunit;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
    
    public class SolrMoreLikeThisHandlerStreamUrlQueryTests {
        [Fact]
        public void Invalid_url_as_string_throws() {
            Assert.Throws<InvalidURLException>(() => new SolrMoreLikeThisHandlerStreamUrlQuery("asdasd"));
        }
    }
}
