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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses highlighting results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class HighlightingResponseParser<T> : ISolrResponseParser<T> {
        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        /// <inheritdoc />
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
        public static IDictionary<string, HighlightedSnippets> ParseHighlighting(IEnumerable<T> results, XElement node) {
            var highlights = new Dictionary<string, HighlightedSnippets>();
            var docRefs = node.Elements("lst");
            foreach (var docRef in docRefs) {
                var docRefKey = docRef.Attribute("name").Value;
                highlights.Add(docRefKey, ParseHighlightingFields(docRef.Elements()));                    
            }
            return highlights;
        }

        /// <summary>
        /// Parse highlighting snippets for each field.
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public static HighlightedSnippets ParseHighlightingFields(IEnumerable<XElement> nodes) {
            var fields = new HighlightedSnippets();
            foreach (var field in nodes) {
                var fieldName = field.Attribute("name").Value;
                ICollection<string> snippets = field.Elements("str")
                    .Select(str => str.Value)
                    .ToList();
                if (snippets.Count == 0 && !string.IsNullOrEmpty(field.Value))
                    snippets = new[] { field.Value };
                fields.Add(fieldName, snippets);
            }
            return fields;
        }
    }
}
