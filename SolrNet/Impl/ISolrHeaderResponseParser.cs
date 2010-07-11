using System.Xml;

namespace SolrNet.Impl {
    public interface ISolrHeaderResponseParser {
        ResponseHeader Parse(XmlDocument response);
    }
}