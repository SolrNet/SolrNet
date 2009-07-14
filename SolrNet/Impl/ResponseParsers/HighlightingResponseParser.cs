using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    public class HighlightingResponseParser<T> : ISolrResponseParser<T> {
        private readonly ISolrDocumentIndexer<T> docIndexer;

        public HighlightingResponseParser(ISolrDocumentIndexer<T> docIndexer) {
            this.docIndexer = docIndexer;
        }

        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var highlightingNode = xml.SelectSingleNode("response/lst[@name='highlighting']");
            if (highlightingNode != null)
                results.Highlights = ParseHighlighting(results, highlightingNode);
        }

        /// <summary>
        /// Parses highlighting results
        /// </summary>
        /// <param name="results"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<T, IDictionary<string, string>> ParseHighlighting(IEnumerable<T> results, XmlNode node) {
            var r = new Dictionary<T, IDictionary<string, string>>();
            var docRefs = node.SelectNodes("lst");
            if (docRefs == null)
                return r;
            var resultsByKey = docIndexer.IndexResultsByKey(results);
            foreach (XmlNode docRef in docRefs) {
                var docRefKey = docRef.Attributes["name"].InnerText;
                var doc = resultsByKey[docRefKey];
                r[doc] = ParseHighlightingFields(docRef.ChildNodes);
            }
            return r;
        }

        /// <summary>
        /// Parses highlighting results
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public IDictionary<string, string> ParseHighlightingFields(XmlNodeList nodes) {
            var fields = new Dictionary<string, string>();
            foreach (XmlNode field in nodes) {
                var fieldName = field.Attributes["name"].InnerText;
                fields[fieldName] = field.InnerText;
            }
            return fields;
        }
    }
}