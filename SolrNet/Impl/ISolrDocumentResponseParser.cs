using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl {
    public interface ISolrDocumentResponseParser<T> {
        IList<T> ParseResults(XmlNode parentNode);
    }
}