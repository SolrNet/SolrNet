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
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses collapse_counts from query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class CollapseResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var mainCollapseNode = xml.XPathSelectElement("response/lst[@name='collapse_counts']");
            if (mainCollapseNode != null) {
                results.Collapsing = new CollapseResults {
                    CollapsedDocuments = ParseCollapsedResults(mainCollapseNode).ToArray(),
                    Field = mainCollapseNode.XPathSelectElement("str[@name='field']").Value
                };
            }
        }

        /// <summary>
        /// Parses collapsed document.ids and their counts
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IEnumerable<CollapsedDocument> ParseCollapsedResults(XElement node) {
            foreach (var docNode in node.XPathSelectElement("lst[@name='results']").Elements()) {
                yield return new CollapsedDocument {
                    Id = docNode.Attribute("name").Value,
                    FieldValue = docNode.XPathSelectElement("str[@name='fieldValue']").Value,
                    CollapseCount = Convert.ToInt32(docNode.XPathSelectElement("int[@name='collapseCount']").Value)
                };
            }
        }
    }
}