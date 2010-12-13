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
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses highlighting results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class HighlightingResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var highlightingNode = xml.XPathSelectElement("response/lst[@name='highlighting']");
            if (highlightingNode != null)
                results.Highlights = ParseHighlighting(results, highlightingNode);
        }

        /// <summary>
        /// Parses highlighting results
        /// </summary>
        /// <param name="results"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, IDictionary<string, ICollection<string>>> ParseHighlighting(IEnumerable<T> results, XElement node) {
            var r = new Dictionary<string, IDictionary<string, ICollection<string>>>();
            var docRefs = node.Elements("lst");
            foreach (var docRef in docRefs) {
                var docRefKey = docRef.Attribute("name").Value;
                r[docRefKey] = ParseHighlightingFields(docRef.Elements());
            }
            return r;
        }

        /// <summary>
        /// Parses highlighting results
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public IDictionary<string, ICollection<string>> ParseHighlightingFields(IEnumerable<XElement> nodes) {
            var fields = new Dictionary<string, ICollection<string>>();
            foreach (var field in nodes) {
                var fieldName = field.Attribute("name").Value;
                var snippets = new List<string>();
                foreach (var str in field.Elements("str")) {
                    snippets.Add(str.Value);
                }
                fields[fieldName] = snippets;
            }
            return fields;
        }
    }
}