using System;
using System.Collections.Generic;
using Xunit;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    
    public class GenericDictionaryDocumentVisitorTests {

        [Fact]
        public void ParseDictionaryOfCollection() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.docWithDynamicFields.xml");
            var mapper = new AttributesMappingManager();
            var parser = new SolrDocumentResponseParser<Entity>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Entity>());
            var entity = parser.ParseDocument(xml.Root);
            Assert.NotNull(entity);
            Assert.NotNull(entity.Attributes);
            Assert.Equal(16, entity.Attributes.Count);

            var attr2 = entity.Attributes["2"];
            Assert.Equal(5, attr2.Count);
            Assert.Contains( 63,attr2);
            Assert.Contains( 64,attr2);
            Assert.Contains( 65,attr2);
            Assert.Contains( 66,attr2);
            Assert.Contains( 102,attr2);
        }

        class Entity {
            [SolrField("attr_")]
            public IDictionary<string, ICollection<int>> Attributes { get; set; }
        }
    }
}
