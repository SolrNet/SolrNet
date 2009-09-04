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
using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses collapse_counts from query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class CollapseResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var mainCollapseNode = xml.SelectSingleNode("response/lst[@name='collapse_counts']");
            if (mainCollapseNode != null) {
                results.Collapsing = new CollapseResults();
                results.Collapsing.CountResults = ParseCountResults(mainCollapseNode);
                results.Collapsing.DocResults = ParseDocResults(mainCollapseNode);
                results.Collapsing.Field = mainCollapseNode.SelectSingleNode("str[@name='field']").InnerText;
            }
        }

        /// <summary>
        /// Parses collapsed document.ids and their counts
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, int> ParseDocResults(XmlNode node) {
            var d = new Dictionary<string, int>();
            foreach (XmlNode countNode in node.SelectSingleNode("lst[@name='doc']").ChildNodes) {
                var key = countNode.Attributes["name"].Value;
                var value = Convert.ToInt32(countNode.InnerText);
                d[key] = value;
            }
            return d;
        }

        /// <summary>
        /// Parses collapsed field values and their counts
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, int> ParseCountResults(XmlNode node) {
            var d = new Dictionary<string, int>();
            foreach (XmlNode countNode in node.SelectSingleNode("lst[@name='count']").ChildNodes) {
                var key = countNode.Attributes["name"].Value;
                var value = Convert.ToInt32(countNode.InnerText);
                d[key] = value;
            }
            return d;
        }
    }
}