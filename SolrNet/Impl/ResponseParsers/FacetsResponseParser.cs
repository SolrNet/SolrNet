using System;
using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    public class FacetsResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var mainFacetNode = xml.SelectSingleNode("response/lst[@name='facet_counts']");
            if (mainFacetNode != null) {
                results.FacetQueries = ParseFacetQueries(mainFacetNode);
                results.FacetFields = ParseFacetFields(mainFacetNode);
            }
        }

        /// <summary>
        /// Parses facet queries results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, int> ParseFacetQueries(XmlNode node) {
            var d = new Dictionary<string, int>();
            foreach (XmlNode fieldNode in node.SelectSingleNode("lst[@name='facet_queries']").ChildNodes) {
                var key = fieldNode.Attributes["name"].Value;
                var value = Convert.ToInt32(fieldNode.InnerText);
                d[key] = value;
            }
            return d;
        }

        /// <summary>
        /// Parses facet fields results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> ParseFacetFields(XmlNode node) {
            var d = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            foreach (XmlNode fieldNode in node.SelectSingleNode("lst[@name='facet_fields']").ChildNodes) {
                var field = fieldNode.Attributes["name"].Value;
                var c = new List<KeyValuePair<string, int>>();
                foreach (XmlNode facetNode in fieldNode.ChildNodes) {
                    var key = facetNode.Attributes["name"].Value;
                    var value = Convert.ToInt32(facetNode.InnerText);
                    c.Add(new KeyValuePair<string, int>(key, value));
                }
                d[field] = c;
            }
            return d;
        }
    }
}