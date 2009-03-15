using System.Xml;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    public class DefaultDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly AggregateDocumentVisitor visitor;

        public DefaultDocumentVisitor(IReadOnlyMappingManager mapper, ISolrFieldParser parser) {
            visitor = new AggregateDocumentVisitor(new ISolrDocumentPropertyVisitor[] {
                new GenericDictionaryDocumentVisitor(mapper, parser),
                new RegularDocumentVisitor(parser, mapper),
            });
        }

        public void Visit(object doc, string fieldName, XmlNode field) {
            visitor.Visit(doc, fieldName, field);
        }
    }
}