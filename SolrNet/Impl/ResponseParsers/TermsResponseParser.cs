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
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses spell-checking results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class TermsResponseParser<T> : ISolrResponseParser<T> {
        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        /// <inheritdoc />
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var termsNode = xml.XPathSelectElement("response/lst[@name='terms']");
            if (termsNode != null)
                results.Terms = ParseTerms(termsNode);
        }

        /// <summary>
        /// Parses spell-checking results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public TermsResults ParseTerms(XElement node)
        {
            var r = new TermsResults();
            var terms = node.Elements("lst");
            foreach (var c in terms) {
                var result = new TermsResult();
                result.Field = c.Attribute("name").Value;
                var termList = new List<KeyValuePair<string, int>>();
                var termNodes = c.XPathSelectElements("int");
                foreach (var termNode in termNodes) {
                    termList.Add(new KeyValuePair<string, int>(termNode.Attribute("name").Value, int.Parse(termNode.Value)));
                }
                result.Terms = termList;
                r.Add(result);
            }
            return r;
        }
    }
}
