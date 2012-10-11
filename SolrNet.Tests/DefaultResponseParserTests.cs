using System;
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;
using Doc = SolrNet.Tests.SolrDocumentSerializerTests.TestDocWithLocation;

namespace SolrNet.Tests {
    [TestFixture]
    public class DefaultResponseParserTests {
        [Test]
        public void ParseResponseWithLocation() {
            var mapper = new AttributesMappingManager();
            var parser = new DefaultResponseParser<Doc>(new SolrDocumentResponseParser<Doc>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Doc>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.response.xml");
            var results = new SolrQueryResults<Doc>();
            parser.Parse(xml, results);
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual(new Location(51.5171, -0.1062), results[0].Loc);
        }
    }
}
