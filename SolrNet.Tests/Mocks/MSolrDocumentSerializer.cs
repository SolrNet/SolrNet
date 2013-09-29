using System;
using System.Xml.Linq;

namespace SolrNet.Tests.Mocks {
    public class MSolrDocumentSerializer<T> : ISolrDocumentSerializer<T> {
        public XElement Serialize(T doc, double? boost) {
            throw new NotImplementedException();
        }
    }
}