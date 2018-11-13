using System.Linq;
using Xunit;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests
{
    
    public class CollapseExpandResponseParserTests
    {
        [Fact]
        public void Parse()
        {
            var mapper = new AttributesMappingManager();
            var parser = new CollapseExpandResponseParser<Doc>(new SolrDocumentResponseParser<Doc>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Doc>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.collapseWithoutExpandResponse.xml");
            var results = new SolrQueryResults<Doc>();
            parser.Parse(xml, results);
            Assert.Null(results.CollapseExpand);
        }

        [Fact]
        public void Parse2()
        {
            var mapper = new AttributesMappingManager();
            var parser = new CollapseExpandResponseParser<Doc>(new SolrDocumentResponseParser<Doc>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Doc>()));
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.collapseWithExpandResponse.xml");
            var results = new SolrQueryResults<Doc>();
            parser.Parse(xml, results);
            Assert.NotNull(results.CollapseExpand);
            Assert.Equal(4, results.CollapseExpand.Groups.Count);

            var group = results.CollapseExpand.Groups.ElementAt(0);
            Assert.Equal(2, group.Documents.Count);
            Assert.Equal("First", group.GroupValue);
            Assert.Equal(2, group.NumFound);
        }

        class Doc {}
    }
}
