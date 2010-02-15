#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
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

using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses highlighting results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
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
        public IDictionary<T, IDictionary<string, ICollection<string>>> ParseHighlighting(IEnumerable<T> results, XmlNode node) {
            var r = new Dictionary<T, IDictionary<string, ICollection<string>>>();
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
        public IDictionary<string, ICollection<string>> ParseHighlightingFields(XmlNodeList nodes) {
            var fields = new Dictionary<string, ICollection<string>>();
            foreach (XmlNode field in nodes) {
                var fieldName = field.Attributes["name"].InnerText;
                var snippets = new List<string>();
                foreach (XmlNode str in field.SelectNodes("str")) {
                    snippets.Add(str.InnerText);
                }
                fields[fieldName] = snippets;
            }
            return fields;
        }
    }
}