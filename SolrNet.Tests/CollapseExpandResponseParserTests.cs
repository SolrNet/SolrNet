using System.Linq;
using MbUnit.Framework;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests
{
    [TestFixture]
    public class CollapseExpandResponseParserTests
    {
        [Test]
        public void Parse()
        {
            var mapper = new AttributesMappingManager();
            var parser = new CollapseExpandResponseParser<Doc>(new SolrDocumentResponseParser<Doc>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Doc>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.collapseWithoutExpandResponse.xml");
            var results = new SolrQueryResults<Doc>();
            parser.Parse(xml, results);
            Assert.IsNull(results.CollapseExpand);
        }

        [Test]
        public void Parse2()
        {
            var mapper = new AttributesMappingManager();
            var parser = new CollapseExpandResponseParser<Doc>(new SolrDocumentResponseParser<Doc>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Doc>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.collapseWithExpandResponse.xml");
            var results = new SolrQueryResults<Doc>();
            parser.Parse(xml, results);
            Assert.IsNotNull(results.CollapseExpand);
            Assert.AreEqual(4, results.CollapseExpand.Groups.Count);

            var group = results.CollapseExpand.Groups.ElementAt(0);
            Assert.AreEqual(group.Documents.Count, 2);
            Assert.AreEqual(group.GroupValue, "First");
            Assert.AreEqual(group.NumFound, 2);
        }

        class Doc {}
    }
}
