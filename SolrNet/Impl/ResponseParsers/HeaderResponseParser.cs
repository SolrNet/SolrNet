using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    public class HeaderResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var responseHeaderNode = xml.SelectSingleNode("response/lst[@name='responseHeader']");
            if (responseHeaderNode != null) {
                results.Header = ParseHeader(responseHeaderNode);
            }
        }

        /// <summary>
        /// Parses response header
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ResponseHeader ParseHeader(XmlNode node) {
            var r = new ResponseHeader();
            r.Status = int.Parse(node.SelectSingleNode("int[@name='status']").InnerText, CultureInfo.InvariantCulture.NumberFormat);
            r.QTime = int.Parse(node.SelectSingleNode("int[@name='QTime']").InnerText, CultureInfo.InvariantCulture.NumberFormat);
            r.Params = new Dictionary<string, string>();
            var paramNodes = node.SelectNodes("lst[@name='params']/str");
            if (paramNodes != null) {
                foreach (XmlNode n in paramNodes) {
                    r.Params[n.Attributes["name"].InnerText] = n.InnerText;
                }
            }
            return r;
        }
    }
}