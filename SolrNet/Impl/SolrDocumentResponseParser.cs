using System.Collections.Generic;
using System.Reflection;
using System.Xml;

namespace SolrNet.Impl {
    public class SolrDocumentResponseParser<T> : ISolrDocumentResponseParser<T> where T : new() {
        private readonly IReadOnlyMappingManager mappingManager;
        private readonly ISolrDocumentPropertyVisitor propVisitor;

        public SolrDocumentResponseParser(IReadOnlyMappingManager mappingManager, ISolrDocumentPropertyVisitor propVisitor) {
            this.mappingManager = mappingManager;
            this.propVisitor = propVisitor;
        }

        /// <summary>
        /// Parses documents results
        /// </summary>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        public IList<T> ParseResults(XmlNode parentNode) {
            var results = new List<T>();
            if (parentNode == null)
                return results;
            var allFields = mappingManager.GetFields(typeof (T));
            var nodes = parentNode.SelectNodes("doc");
            if (nodes == null)
                return results;
            foreach (XmlNode docNode in nodes) {
                results.Add(ParseDocument(docNode, allFields));
            }
            return results;
        }

        /// <summary>
        /// Builds a document from the corresponding response xml node
        /// </summary>
        /// <param name="node">response xml node</param>
        /// <param name="fields">document fields</param>
        /// <returns>populated document</returns>
        public T ParseDocument(XmlNode node, ICollection<KeyValuePair<PropertyInfo, string>> fields) {
            var doc = new T();
            foreach (XmlNode field in node.ChildNodes) {
                string fieldName = field.Attributes["name"].InnerText;
                propVisitor.Visit(doc, fieldName, field);
            }
            return doc;
        }
    }
}