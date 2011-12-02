using System;
using MbUnit.Framework;
using SolrNet.Exceptions;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrMoreLikeThisHandlerStreamUrlQueryTests {
        [Test]
        [ExpectedException(typeof(InvalidURLException))]
        public void Invalid_url_as_string_throws() {
            new SolrMoreLikeThisHandlerStreamUrlQuery("asdasd");
        }
    }
}
