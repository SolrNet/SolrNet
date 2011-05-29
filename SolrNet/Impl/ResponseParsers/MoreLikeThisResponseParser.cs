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
    /// Parses more-like-this results from a query response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoreLikeThisResponseParser<T> : ISolrResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public MoreLikeThisResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var moreLikeThis = xml.XPathSelectElement("response/lst[@name='moreLikeThis']");
            if (moreLikeThis != null)
                results.MoreLikeThis.Results = ParseMoreLikeThis(results, moreLikeThis);

            var termList = xml.XPathSelectElement("response/arr[@name='interestingTerms']");
            if (termList != null)
                results.MoreLikeThis.InterestingTerms = ParseInterestingTerms(termList);

            var termDetails = xml.XPathSelectElement("response/lst[@name='interestingTerms']");
            if (termDetails != null)
                results.MoreLikeThis.InterestingTerms = ParseInterestingTerms(termDetails);
        }

        /// <summary>
        /// Parses the interesting-params
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, float> ParseInterestingTerms(XElement node)
        {
            var r = new Dictionary<string, float>();
            var strings = node.Elements("str");
            if (strings != null)
            {
                foreach (var str in strings)
                {
                    r[str.Value] = 0;
                }
            }
            var floats = node.Elements("float");
            if (floats != null)
            {
                foreach (var flt in floats)
                {
                    r[flt.Attribute("name").Value] = float.Parse(flt.Value);
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