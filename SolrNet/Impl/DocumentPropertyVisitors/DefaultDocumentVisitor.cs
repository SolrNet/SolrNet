using System;
using System.Reflection;
using System.Xml;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    public class DefaultDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly AggregateDocumentVisitor visitor;
        private readonly IReadOnlyMappingManager mapper;
        private readonly ISolrFieldParser parser;

        public DefaultDocumentVisitor(IReadOnlyMappingManager mapper, ISolrFieldParser parser) {
            this.mapper = mapper;
            this.parser = parser;
            visitor = new AggregateDocumentVisitor(new ISolrDocumentPropertyVisitor[] {
                new GenericDictionaryDocumentVisitor(mapper, parser),
                new RegularDocumentVisitor(parser, mapper),
            });
        }

        public bool CanHandleType(Type t) {
            return visitor.CanHandleType(t);
        }

        public void Visit(object doc, string fieldName, XmlNode field) {
            visitor.Visit(doc, fieldName, field);
        }
    }
}