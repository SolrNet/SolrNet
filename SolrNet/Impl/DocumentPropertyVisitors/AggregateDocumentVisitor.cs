using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using SolrNet.Utils;

namespace SolrNet.Impl.DocumentPropertyVisitors {
    public class AggregateDocumentVisitor : ISolrDocumentPropertyVisitor {
        private readonly IEnumerable<ISolrDocumentPropertyVisitor> visitors;

        public AggregateDocumentVisitor(IEnumerable<ISolrDocumentPropertyVisitor> visitors) {
            this.visitors = visitors;
        }

        public bool CanHandleType(Type t) {
            return Func.Any(visitors, v => v.CanHandleType(t));
        }

        public void Visit(object doc, string fieldName, XmlNode field) {
            foreach (var v in visitors) {
                v.Visit(doc, fieldName, field);
            }
        }
    }
}