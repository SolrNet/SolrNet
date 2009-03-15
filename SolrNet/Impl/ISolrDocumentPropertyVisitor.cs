using System;
using System.Xml;

namespace SolrNet.Impl {
    public interface ISolrDocumentPropertyVisitor {
        void Visit(object doc, string fieldName, XmlNode field);
    }
}