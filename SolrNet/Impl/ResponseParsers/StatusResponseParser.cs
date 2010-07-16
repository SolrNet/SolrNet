namespace SolrNet.Impl.ResponseParsers
{
    using System.Xml;

    public class StatusResponseParser<T> : ISolrResponseParser<T> 
    {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results)
        {
        }
    }
}