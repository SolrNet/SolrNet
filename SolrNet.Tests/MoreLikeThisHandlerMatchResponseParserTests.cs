using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Integration.Sample;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class MoreLikeThisHandlerMatchResponseParserTests {
        [Test]
        public void Parse() {
            var mapper = new AttributesMappingManager();
            var fieldParser = new DefaultFieldParser();
            var docVisitor = new DefaultDocumentVisitor(mapper, fieldParser);
            var docParser = new SolrDocumentResponseParser<Product>(mapper, docVisitor, new SolrDocumentActivator<Product>());
            var p = new MoreLikeThisHandlerMatchResponseParser<Product>(docParser);
            var mltResults = new SolrMoreLikeThisHandlerResults<Product>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithMLTHandlerMatch.xml");
            p.Parse(xml, mltResults);
            Assert.IsNotNull(mltResults.Match);
        }
    }
}