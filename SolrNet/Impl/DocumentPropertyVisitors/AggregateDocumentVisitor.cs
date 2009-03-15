using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    public class AggregateDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly IEnumerable<ISolrDocumentPropertyVisitor> visitors;

        public AggregateDocumentVisitor(IEnumerable<ISolrDocumentPropertyVisitor> visitors) {
            this.visitors = visitors;
        }

        public void Visit(object doc, string fieldName, XmlNode field) {
            foreach (var v in visitors) {
                v.Visit(doc, fieldName, field);
            }
        }
    }
}