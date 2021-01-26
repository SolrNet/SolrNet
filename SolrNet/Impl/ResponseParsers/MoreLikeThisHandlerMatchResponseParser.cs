using System.Linq;
using System.Xml.Linq;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    public class MoreLikeThisHandlerMatchResponseParser<T> : ISolrMoreLikeThisHandlerResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public MoreLikeThisHandlerMatchResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: F.DoNothing,
                           moreLikeThis: r => Parse(xml, r));
        }

        /// <inheritdoc />
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
