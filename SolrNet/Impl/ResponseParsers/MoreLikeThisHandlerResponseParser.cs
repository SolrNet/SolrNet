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
using System.Globalization;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses more-like-this-handler results from a query response
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MoreLikeThisHandlerResponseParser<T> : ISolrResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public MoreLikeThisHandlerResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            results.MoreLikeThis = ParseMoreLikeThisHandler(xml);
        }

        /// <summary>
        /// Parses the results from the more-like-this-handler
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public MoreLikeThisResults<T> ParseMoreLikeThisHandler(XDocument xml)
        {
            var r = new MoreLikeThisResults<T>();
            var matchNode = xml.XPathSelectElement("response/result[@name='match']");
            if (matchNode != null)
            {
                r.NumFound = Convert.ToInt32(matchNode.Attribute("numFound").Value);
                var maxScore = matchNode.Attribute("maxScore");
                if (maxScore != null)
                {
                    r.MaxScore = double.Parse(maxScore.Value, CultureInfo.InvariantCulture.NumberFormat);
                }

                foreach (var result in docParser.ParseResults(matchNode))
                    r.Match = result;
            }

            var termList = xml.XPathSelectElement("response/arr[@name='interestingTerms']");
            if (termList != null)
                r.InterestingTermList = ParseInterestingTermList(termList);

            var termDetails = xml.XPathSelectElement("response/lst[@name='interestingTerms']");
            if (termDetails != null)
                r.InterestingTermDetails = ParseInterestingTermDetails(termDetails);

            return r;
        }

        /// <summary>
        /// Parses the interesting-params
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, float> ParseInterestingTermDetails(XElement node)
        {
            var r = new Dictionary<string, float>();
            var floats = node.Elements("float");
            if (floats != null)
            {
                foreach (var flt in floats)
                {
                    r[flt.Attribute("name").Value] = float.Parse(flt.Value, CultureInfo.InvariantCulture.NumberFormat);
                }
            }
            return r;
        }

        /// <summary>
        /// Parses the interesting-params
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IList<string> ParseInterestingTermList(XElement node)
        {
            var r = new List<string>();
            var strings = node.Elements("str");
            if (strings != null)
            {
                foreach (var str in strings)
                {
                    r.Add(str.Value);
                }
            }
            return r;
        }
    }
}