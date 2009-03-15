using System;
using System.Xml;

namespace SolrNet.Impl {
    public interface ISolrDocumentPropertyVisitor {
        bool CanHandleType(Type t);
        void Visit(object doc, string fieldName, XmlNode field);
    }
}