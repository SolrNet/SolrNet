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

namespace SolrNet.Impl.ResponseParsers
{
    /// <summary>
    /// Parses spell-checking results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SpellCheckResponseParser<T> : ISolrResponseParser<T>
    {
        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
        {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        /// <inheritdoc />
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

            IEnumerable<XElement> collationNodes;
            var collationsNode = node.XPathSelectElement("lst[@name='collations']");
            if (collationsNode != null)
            {
                // Solr 5.0+
                collationNodes = collationsNode.XPathSelectElements("lst[@name='collation']")
                    .Union(collationsNode.XPathSelectElements("str[@name='collation']"));
            }
            else
            {
                // Solr 4.x and lower
                collationNodes = suggestionsNode.XPathSelectElements("lst[@name='collation']")
                    .Union(suggestionsNode.XPathSelectElements("str[@name='collation']"));              
            }
            
            CollationResult tempCollation;
            foreach (var cn in collationNodes)
            {
                //If it does not contain collationQuery element, it is a suggestion
                if (cn.XPathSelectElement("str[@name='collationQuery']") != null)
                {
                    tempCollation = new CollationResult();
                    tempCollation.CollationQuery = cn.XPathSelectElement("str[@name='collationQuery']").Value;

                    if (cn.XPathSelectElement("long[@name='hits']") != null)
                    {
                        tempCollation.Hits = Convert.ToInt64(cn.XPathSelectElement("long[@name='hits']").Value);
                    }
                    else if (cn.XPathSelectElement("int[@name='hits']") != null)
                    {
                        tempCollation.Hits = Convert.ToInt32(cn.XPathSelectElement("int[@name='hits']").Value);
                    }

                    //Selects the mispellings and corrections
                    var correctionNodes = cn.XPathSelectElements("lst[@name='misspellingsAndCorrections']");
                    foreach (var mc in correctionNodes.Elements())
                    {
                        tempCollation.MisspellingsAndCorrections.Add(new KeyValuePair<string, string>(mc.Attribute("name").Value, mc.Value));
                    }
                    r.Collations.Add(tempCollation);
                }
                else if(cn.Name.LocalName.Equals("str"))
                {
                    tempCollation = new CollationResult();
                    tempCollation.CollationQuery = cn.Value;
                    tempCollation.Hits = -1;                    
                    r.Collations.Add(tempCollation);
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
                    var suggestionArray = c.XPathSelectElements("arr[@name='suggestion']");
                    var suggestionNodes = suggestionArray.Descendants("str");
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
