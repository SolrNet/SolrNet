using System.Linq;
using System.Xml.Linq;

namespace SolrNet.Impl.ResponseParsers {
    public class MlthMatchResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public MlthMatchResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            if (results is SolrMoreLikeThisHandlerResults<T>) {
                Parse(xml, (SolrMoreLikeThisHandlerResults<T>) results);
            }
        }

        public void Parse(XDocument xml, SolrMoreLikeThisHandlerResults<T> results) {
            var resultNode = xml
                .Element("response")
                .Elements("result")
                .FirstOrDefault(e => (string) e.Attribute("name") == "match");

            if (resultNode == null) {
                results.Match = default(T);
                return;
            }

            results.Match = docParser.ParseResults(resultNode).FirstOrDefault();
        }
    }
}