using System.Xml;
using System.Xml.Linq;

namespace SolrNet.Impl {
    public interface ISolrHeaderResponseParser {
        ResponseHeader Parse(XDocument response);
    }
}