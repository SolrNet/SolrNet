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
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses collapse_counts from query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class CollapseResponseParser<T> : ISolrResponseParser<T> {
        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            if (results is SolrQueryResults<T>)
                Parse(xml, (SolrQueryResults<T>) results);
        }

        /// <inheritdoc />
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var mainCollapseNode = xml.Element("response")
                .Elements("lst")
                .FirstOrDefault(X.AttrEq("name", "collapse_counts"));
            if (mainCollapseNode != null) {
                var value = mainCollapseNode.Elements("str").First(X.AttrEq("name", "field")).Value;
                results.Collapsing = new CollapseResults {
                    CollapsedDocuments = ParseCollapsedResults(mainCollapseNode).ToArray(),
                    Field = value,
                };
            }
        }

        /// <summary>
        /// Parses collapsed document.ids and their counts
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<CollapsedDocument> ParseCollapsedResults(XElement node) {
            var results = node.Elements("lst")
                .Where(X.AttrEq("name", "results"))
                .Elements();
            return
                results.Select(docNode => {
                    string fieldValue = docNode.Elements("str")
                        .First(X.AttrEq("name", "fieldValue"))
                        .Value;
                    var collapseCountRaw = docNode.Elements("int")
                        .First(X.AttrEq("name", "collapseCount"))
                        .Value;
                    int collapseCount = int.Parse(collapseCountRaw);
                    return new CollapsedDocument {
                        Id = docNode.Attribute("name").Value,
                        FieldValue = fieldValue,
                        CollapseCount = collapseCount,
                    };
                });
        }
    }
}
