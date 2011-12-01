using System.Xml.Linq;

namespace SolrNet.Impl {
    public class SolrMoreLikeThisHandlerQueryResultsParser<T> : ISolrMoreLikeThisHandlerQueryResultsParser<T> {
        private readonly ISolrAbstractResponseParser<T>[] parsers;

        public SolrMoreLikeThisHandlerQueryResultsParser(ISolrAbstractResponseParser<T>[] parsers) {
            this.parsers = parsers;
        }

        public SolrMoreLikeThisHandlerResults<T> Parse(string r) {
            var results = new SolrMoreLikeThisHandlerResults<T>();
            var xml = XDocument.Parse(r);
            foreach (var p in parsers) {
                p.Parse(xml, results);
            }

            return results;
        }
    }
}