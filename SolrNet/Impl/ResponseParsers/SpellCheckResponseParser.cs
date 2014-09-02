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

using SolrNet.Utils;
using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses spell-checking results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SpellCheckResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var spellCheckingNode = xml.XPathSelectElement("response/lst[@name='spellcheck']");
            if (spellCheckingNode != null)
                results.SpellChecking = ParseSpellChecking(spellCheckingNode);
        }

        /// <summary>
        /// Parses spell-checking results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SpellCheckResults ParseSpellChecking(XElement node) {
            var r = new SpellCheckResults();
            var suggestionsNode = node.XPathSelectElement("lst[@name='suggestions']");
            var collationNode = suggestionsNode.XPathSelectElement("str[@name='collation']");
            if (collationNode != null)
                r.Collation = collationNode.Value;
            var spellChecks = suggestionsNode.Elements("lst");
            foreach (var spellCheck in spellChecks)
            {
                var query = spellCheck.Attribute("name").Value;
                if (query.Equals("collation", StringComparison.InvariantCultureIgnoreCase))
                {
                    ParseCollationNode(spellCheck,r);
                }
                else
                    ParseSpellCheckingNode(spellCheck,r);
            }
            if (r.Collations == null ||
                !r.Collations.Any(c => c.MisspellingsAndCorrections != null && c.MisspellingsAndCorrections.Any()))
                return r;
            var collationResult = r.Collations.FirstOrDefault();
            if (collationResult != null)
                r.Collations.Query = collationResult.MisspellingsAndCorrections.FirstOrDefault();
            return r;
        }

        /// <summary>
        /// Parses spell-checking node and adds to SpellCheckResults entity
        /// </summary>
        /// <param name="node"></param>
        /// <param name="spellCheckResults"></param>
        /// <returns></returns>
        private void ParseSpellCheckingNode(XElement node, SpellCheckResults spellCheckResults)
        {
            var result = new SpellCheckResult
            {
                Query = node.Attribute("name").Value,
                NumFound = Convert.ToInt32(node.XPathSelectElement("int[@name='numFound']").Value),
                EndOffset = Convert.ToInt32(node.XPathSelectElement("int[@name='endOffset']").Value),
                StartOffset = Convert.ToInt32(node.XPathSelectElement("int[@name='startOffset']").Value)
            };
            var suggestionNodes = node.XPathSelectElements("arr[@name='suggestion']/str");
            var suggestions = suggestionNodes.Select(suggestionNode => suggestionNode.Value).ToList();
            result.Suggestions = suggestions;
            spellCheckResults.Add(result);
        }

        /// <summary>
        /// Parses collation node in spell-checking node to CollationResults entity
        /// </summary>
        /// <param name="node"></param>
        /// <param name="spellCheckResults"></param>
        /// <returns></returns>
        private void ParseCollationNode(XElement node, SpellCheckResults spellCheckResults)
        {
            if(spellCheckResults.Collations==null)
                spellCheckResults.Collations=new CollationResults();
            var result = new CollationResult
            {
                Query = node.XPathSelectElement("str[@name='collationQuery']").Value,
                Hits = Convert.ToInt32(node.XPathSelectElement("int[@name='hits']").Value)
            };
            var misspellingsAndCorrectionNodes = node.XPathSelectElements("arr[@name='misspellingsAndCorrections']/str").ToList();
            if (!misspellingsAndCorrectionNodes.Any())
                misspellingsAndCorrectionNodes = node.XPathSelectElements("lst[@name='misspellingsAndCorrections']/str").ToList();
            var misspellingsAndCorrections = misspellingsAndCorrectionNodes.Select(misspellingsAndCorrectionNode => misspellingsAndCorrectionNode.Value).ToList();
            result.MisspellingsAndCorrections = misspellingsAndCorrections;
            spellCheckResults.Collations.Add(result);
        }
    }
}
