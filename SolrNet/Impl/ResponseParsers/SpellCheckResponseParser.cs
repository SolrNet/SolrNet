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
using System.Linq;

namespace SolrNet.Impl.ResponseParsers
{
    /// <summary>
    /// Parses spell-checking results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SpellCheckResponseParser<T> : ISolrResponseParser<T>
    {
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
        {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        public void Parse(XDocument xml, SolrQueryResults<T> results)
        {
            var spellCheckingNode = xml.XPathSelectElement("response/lst[@name='spellcheck']");
            if (spellCheckingNode != null)
                results.SpellChecking = ParseSpellChecking(spellCheckingNode);
        }

        /// <summary>
        /// Parses spell-checking results
        /// </summary>
        /// <param name="node">SpellCheck node</param>
        /// <returns>List of suggestions and collations</returns>
        public SpellCheckResults ParseSpellChecking(XElement node)
        {
            var r = new SpellCheckResults();
            var suggestionsNode = node.XPathSelectElement("lst[@name='suggestions']");

            var collationNode = suggestionsNode.XPathSelectElement("str[@name='collation']");
            if (collationNode != null)
            {
                r.Collation = collationNode.Value;
            }

            IEnumerable<XElement> collationNodes;
            var collationsNode = node.XPathSelectElement("lst[@name='collations']");
            if (collationsNode != null)
            {
                // Solr 5.0+
                collationNodes = collationsNode.XPathSelectElements("lst[@name='collation']");
                if (collationNodes.Count() == 0)
                    collationNodes = collationsNode.XPathSelectElements("str[@name='collation']");
            }
            else
            {
                // Solr 4.x and lower
                collationNodes = suggestionsNode.XPathSelectElements("lst[@name='collation']");
            }

            foreach (var cn in collationNodes)
            {
                string collationValue = string.Empty;
                if (cn.XPathSelectElement("str[@name='collationQuery']") != null)
                    collationValue = cn.XPathSelectElement("str[@name='collationQuery']").Value;
                else if (cn.Name.LocalName == "str")
                    collationValue = cn.Value;

                if (collationValue != string.Empty)
                {
                    r.Collations.Add(collationValue);
                    if (string.IsNullOrEmpty(r.Collation))
                        r.Collation = collationValue;
                }
            }

            var spellChecks = suggestionsNode.Elements("lst");
            foreach (var c in spellChecks)
            {
                if (c.Attribute("name").Value != "collation" || c.XPathSelectElement("int[@name='numFound']") != null)
                {
                    //Spelling suggestions are added, required to check if 'collation' is a search term or indicates collation node
                    var result = new SpellCheckResult();
                    result.Query = c.Attribute("name").Value;
                    result.NumFound = Convert.ToInt32(c.XPathSelectElement("int[@name='numFound']").Value);
                    result.EndOffset = Convert.ToInt32(c.XPathSelectElement("int[@name='endOffset']").Value);
                    result.StartOffset = Convert.ToInt32(c.XPathSelectElement("int[@name='startOffset']").Value);
                    var suggestions = new List<string>();
                    var suggestionNodes = c.XPathSelectElements("arr[@name='suggestion']/lst/str");
                    if (suggestionNodes.Count() == 0)
                        suggestionNodes = c.XPathSelectElements("arr[@name='suggestion']/str");
                    foreach (var suggestionNode in suggestionNodes)
                    {
                        suggestions.Add(suggestionNode.Value);
                    }
                    result.Suggestions = suggestions;
                    r.Add(result);
                }
            }

            return r;
        }
    }
}