using System.Linq;
using System.Xml.Linq;

namespace SolrNet.Impl.ResponseParsers {
    public class MoreLikeThisHandlerMatchResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public MoreLikeThisHandlerMatchResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(_ => {}, x => Parse(xml, x));
        }

        public void Parse(XDocument xml, SolrMoreLikeThisHandlerResults<T> results) {
            var resultNode = xml
                .Element("response")
                .Elements("result")
                .FirstOrDefault(e => e.Attribute("name").Value == "match");

            results.Match = resultNode == null ? 
                default(T) : 
                docParser.ParseResults(resultNode).FirstOrDefault();
        }
    }
}