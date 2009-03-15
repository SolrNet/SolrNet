#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Xml;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet.Impl {
    /// <summary>
    /// Default query results parser.
    /// Parses xml query results
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrQueryResultParser<T> : ISolrQueryResultParser<T> where T : new() {

        private readonly IReadOnlyMappingManager mappingManager;
        private readonly ISolrDocumentPropertyVisitor propVisitor;

        public SolrQueryResultParser(IReadOnlyMappingManager mappingManager, ISolrDocumentPropertyVisitor propVisitor) {
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
            var allFields = mappingManager.GetFields(typeof(T));
            var nodes = parentNode.SelectNodes("doc");
            if (nodes == null)
                return results;
            foreach (XmlNode docNode in nodes) {
                results.Add(ParseDocument(docNode, allFields));
            }
            return results;
        }

        /// <summary>
        /// Parses solr's xml response
        /// </summary>
        /// <param name="r">solr xml response</param>
        /// <returns>query results</returns>
        public ISolrQueryResults<T> Parse(string r) {
            var results = new SolrQueryResults<T>();
            var xml = new XmlDocument();
            xml.LoadXml(r);
            var resultNode = xml.SelectSingleNode("response/result");
            results.NumFound = Convert.ToInt32(resultNode.Attributes["numFound"].InnerText);
            var maxScore = resultNode.Attributes["maxScore"];
            if (maxScore != null) {
                results.MaxScore = double.Parse(maxScore.InnerText, CultureInfo.InvariantCulture.NumberFormat);
            }

            foreach (var result in ParseResults(resultNode))
                results.Add(result);
            
            var mainFacetNode = xml.SelectSingleNode("response/lst[@name='facet_counts']");
            if (mainFacetNode != null) {
                results.FacetQueries = ParseFacetQueries(mainFacetNode);
                results.FacetFields = ParseFacetFields(mainFacetNode);
            }

            var responseHeaderNode = xml.SelectSingleNode("response/lst[@name='responseHeader']");
            if (responseHeaderNode != null) {
                results.Header = ParseHeader(responseHeaderNode);
            }

            var highlightingNode = xml.SelectSingleNode("response/lst[@name='highlighting']");
            if (highlightingNode != null)
                results.Highlights = ParseHighlighting(results, highlightingNode);

            var spellCheckingNode = xml.SelectSingleNode("response/lst[@name='spellcheck']");
            if (spellCheckingNode != null)
                results.SpellChecking = ParseSpellChecking(spellCheckingNode);

            var moreLikeThis = xml.SelectSingleNode("response/lst[@name='moreLikeThis']");
            if (moreLikeThis != null)
                results.SimilarResults = ParseMoreLikeThis(results, moreLikeThis);

            return results;
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

        /// <summary>
        /// Parses response header
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ResponseHeader ParseHeader(XmlNode node) {
            var r = new ResponseHeader();
            r.Status = int.Parse(node.SelectSingleNode("int[@name='status']").InnerText);
            r.QTime = int.Parse(node.SelectSingleNode("int[@name='QTime']").InnerText);
            r.Params = new Dictionary<string, string>();
            var paramNodes = node.SelectNodes("lst[@name='params']/str");
            if (paramNodes != null) {
                foreach (XmlNode n in paramNodes) {
                    r.Params[n.Attributes["name"].InnerText] = n.InnerText;
                }				
            }
            return r;
        }

        /// <summary>
        /// Creates an index of documents by unique key
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public IDictionary<string, T> IndexResultsByKey(IEnumerable<T> results) {
            var r = new Dictionary<string, T>();
            var prop = mappingManager.GetUniqueKey(typeof (T)).Key;
            foreach (var d in results) {
                var key = prop.GetValue(d, null).ToString();
                r[key] = d;
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
            var resultsByKey = IndexResultsByKey(results);
            foreach (XmlNode docRef in docRefs) {
                var docRefKey = docRef.Attributes["name"].InnerText;
                var doc = resultsByKey[docRefKey];
                r[doc] = ParseHighlightingFields(docRef.ChildNodes);
            }
            return r;
        }

        /// <summary>
        /// Parses spell-checking results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SpellCheckResults ParseSpellChecking(XmlNode node) {
            var r = new SpellCheckResults();
            var suggestionsNode = node.SelectSingleNode("lst[@name='suggestions']");
            var collationNode = suggestionsNode.SelectSingleNode("str[@name='collation']");
            if (collationNode != null)
                r.Collation = collationNode.InnerText;
            var spellChecks = suggestionsNode.SelectNodes("lst");
            if (spellChecks != null) {
                foreach (XmlNode c in spellChecks) {
                    var result = new SpellCheckResult();
                    result.Query = c.Attributes["name"].InnerText;
                    result.NumFound = Convert.ToInt32(c.SelectSingleNode("int[@name='numFound']").InnerText);
                    result.EndOffset = Convert.ToInt32(c.SelectSingleNode("int[@name='endOffset']").InnerText);
                    result.StartOffset = Convert.ToInt32(c.SelectSingleNode("int[@name='startOffset']").InnerText);
                    var suggestions = new List<string>();
                    var suggestionNodes = c.SelectNodes("arr[@name='suggestion']/str");
                    if (suggestionNodes != null) {
                        foreach (XmlNode suggestionNode in suggestionNodes) {
                            suggestions.Add(suggestionNode.InnerText);
                        }                        
                    }
                    result.Suggestions = suggestions;
                    r.Add(result);
                }
            }
            return r;
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
            var resultsByKey = IndexResultsByKey(results);
            foreach (XmlNode docRef in docRefs) {
                var docRefKey = docRef.Attributes["name"].InnerText;
                var doc = resultsByKey[docRefKey];
                r[doc] = ParseResults(docRef);
            }
            return r;
        }
    }
}