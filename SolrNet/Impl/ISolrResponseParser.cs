using System.Xml;

namespace SolrNet.Impl {
    public interface ISolrResponseParser<T> {
        void Parse(XmlDocument xml, SolrQueryResults<T> results);
    }
}