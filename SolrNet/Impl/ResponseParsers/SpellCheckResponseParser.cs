using System;
using System.Collections.Generic;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    public class SpellCheckResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var spellCheckingNode = xml.SelectSingleNode("response/lst[@name='spellcheck']");
            if (spellCheckingNode != null)
                results.SpellChecking = ParseSpellChecking(spellCheckingNode);
        }

        /// <summary>
        /// Parses spell-checking results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SpellCheckResults ParseSpellChecking(XmlNode node) {
            var r = new SpellCheckResults();
            var suggestionsNode = node.SelectSingleNode("lst[@name='suggestions']");
            var collationNode = suggestionsNode.SelectSingleNode("str[@name='collation']");
            if (collationNode != null)
                r.Collation = collationNode.InnerText;
            var spellChecks = suggestionsNode.SelectNodes("lst");
            if (spellChecks != null) {
                foreach (XmlNode c in spellChecks) {
                    var result = new SpellCheckResult();
                    result.Query = c.Attributes["name"].InnerText;
                    result.NumFound = Convert.ToInt32(c.SelectSingleNode("int[@name='numFound']").InnerText);
                    result.EndOffset = Convert.ToInt32(c.SelectSingleNode("int[@name='endOffset']").InnerText);
                    result.StartOffset = Convert.ToInt32(c.SelectSingleNode("int[@name='startOffset']").InnerText);
                    var suggestions = new List<string>();
                    var suggestionNodes = c.SelectNodes("arr[@name='suggestion']/str");
                    if (suggestionNodes != null) {
                        foreach (XmlNode suggestionNode in suggestionNodes) {
                            suggestions.Add(suggestionNode.InnerText);
                        }
                    }
                    result.Suggestions = suggestions;
                    r.Add(result);
                }
            }
            return r;
        }
    }
}