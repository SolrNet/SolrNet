using System;
using System.Xml;

namespace SolrNet.Tests.Mocks {
    public class MSolrDocumentSerializer<T> : ISolrDocumentSerializer<T> {
        public XmlDocument Serialize(T doc, double? boost) {
            throw new NotImplementedException();
        }
    }
}