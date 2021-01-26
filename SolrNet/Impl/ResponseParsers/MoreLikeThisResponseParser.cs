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
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses more-like-this results from a query response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoreLikeThisResponseParser<T> : ISolrResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: r => Parse(xml, r), 
                           moreLikeThis: F.DoNothing);
        }

        public MoreLikeThisResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        /// <inheritdoc />
        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var moreLikeThis = xml.XPathSelectElement("response/lst[@name='moreLikeThis']");
            if (moreLikeThis != null)
                results.SimilarResults = ParseMoreLikeThis(results, moreLikeThis);
        }

        /// <summary>
        /// Parses more-like-this results
        /// </summary>
        /// <param name="results"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, IList<T>> ParseMoreLikeThis(IEnumerable<T> results, XElement node) {
            var r = new Dictionary<string, IList<T>>();
            var docRefs = node.Elements("result");
            foreach (var docRef in docRefs) {
                var docRefKey = docRef.Attribute("name").Value;
                r[docRefKey] = docParser.ParseResults(docRef);
            }
            return r;
        }
    }
}
