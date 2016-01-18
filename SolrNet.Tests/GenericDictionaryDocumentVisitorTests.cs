using System;
using System.Collections.Generic;
using MbUnit.Framework;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class GenericDictionaryDocumentVisitorTests {

        [Test]
        public void ParseDictionaryOfCollection() {
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.docWithDynamicFields.xml");
            var mapper = new AttributesMappingManager();
            var parser = new SolrDocumentResponseParser<Entity>(mapper, new DefaultDocumentVisitor(mapper, new DefaultFieldParser()), new SolrDocumentActivator<Entity>());
            var entity = parser.ParseDocument(xml.Root);
            Assert.IsNotNull(entity, "entity was null");
            Assert.IsNotNull(entity.Attributes, "attributes was null");
            Assert.AreEqual(16, entity.Attributes.Count);

            var attr2 = entity.Attributes["2"];
            Assert.AreEqual(5, attr2.Count);
            Assert.Contains(attr2, 63);
            Assert.Contains(attr2, 64);
            Assert.Contains(attr2, 65);
            Assert.Contains(attr2, 66);
            Assert.Contains(attr2, 102);
        }

        class Entity {
            [SolrField("attr_")]
            public IDictionary<string, ICollection<int>> Attributes { get; set; }
        }
    }
}
