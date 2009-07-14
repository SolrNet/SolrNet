using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    public class MoreLikeThisResponseParser<T> : ISolrResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;
        private readonly ISolrDocumentIndexer<T> docIndexer;

        public MoreLikeThisResponseParser(ISolrDocumentResponseParser<T> docParser, ISolrDocumentIndexer<T> docIndexer) {
            this.docParser = docParser;
            this.docIndexer = docIndexer;
        }

        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var moreLikeThis = xml.SelectSingleNode("response/lst[@name='moreLikeThis']");
            if (moreLikeThis != null)
                results.SimilarResults = ParseMoreLikeThis(results, moreLikeThis);
        }

        /// <summary>
        /// Parses more-like-this results
        /// </summary>
        /// <param name="results"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<T, IList<T>> ParseMoreLikeThis(IEnumerable<T> results, XmlNode node) {
            var r = new Dictionary<T, IList<T>>();
            var docRefs = node.SelectNodes("result");
            if (docRefs == null)
                return r;
            var resultsByKey = docIndexer.IndexResultsByKey(results);
            foreach (XmlNode docRef in docRefs) {
                var docRefKey = docRef.Attributes["name"].InnerText;
                var doc = resultsByKey[docRefKey];
                r[doc] = docParser.ParseResults(docRef);
            }
            return r;
        }
    }
}