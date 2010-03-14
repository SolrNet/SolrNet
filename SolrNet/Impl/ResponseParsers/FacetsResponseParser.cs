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
using System.Xml;
using SolrNet.Impl.FieldParsers;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses facets from query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class FacetsResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var mainFacetNode = xml.SelectSingleNode("response/lst[@name='facet_counts']");
            if (mainFacetNode != null) {
                results.FacetQueries = ParseFacetQueries(mainFacetNode);
                results.FacetFields = ParseFacetFields(mainFacetNode);
                results.FacetDates = ParseFacetDates(mainFacetNode);
            }
        }

        /// <summary>
        /// Parses facet queries results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, int> ParseFacetQueries(XmlNode node) {
            var d = new Dictionary<string, int>();
            foreach (XmlNode fieldNode in node.SelectSingleNode("lst[@name='facet_queries']").ChildNodes) {
                var key = fieldNode.Attributes["name"].Value;
                var value = Convert.ToInt32(fieldNode.InnerText);
                d[key] = value;
            }
            return d;
        }

        /// <summary>
        /// Parses facet fields results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> ParseFacetFields(XmlNode node) {
            var d = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            foreach (XmlNode fieldNode in node.SelectSingleNode("lst[@name='facet_fields']").ChildNodes) {
                var field = fieldNode.Attributes["name"].Value;
                var c = new List<KeyValuePair<string, int>>();
                foreach (XmlNode facetNode in fieldNode.ChildNodes) {
                    var key = facetNode.Attributes["name"].Value;
                    var value = Convert.ToInt32(facetNode.InnerText);
                    c.Add(new KeyValuePair<string, int>(key, value));
                }
                d[field] = c;
            }
            return d;
        }

        /// <summary>
        /// Parses facet dates results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, DateFacetingResult> ParseFacetDates(XmlNode node) {
            var d = new Dictionary<string, DateFacetingResult>();
            var facetDateNode = node.SelectSingleNode("lst[@name='facet_dates']");
            if (facetDateNode != null) {
                foreach (XmlNode fieldNode in facetDateNode.ChildNodes) {
                    var name = fieldNode.Attributes["name"].Value;
                    d[name] = ParseDateFacetingNode(fieldNode);
                }
            }
            return d;
        }

        public KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

        public DateFacetingResult ParseDateFacetingNode(XmlNode node) {
            var r = new DateFacetingResult();
            var dateParser = new DateTimeFieldParser();
            var intParser = new IntFieldParser();
            foreach (XmlNode dateFacetingNode in node.ChildNodes) {
                var name = dateFacetingNode.Attributes["name"].Value;
                switch (name) {
                    case "gap":
                        r.Gap = dateFacetingNode.InnerText;
                        break;
                    case "end":
                        r.End = (DateTime) dateParser.Parse(dateFacetingNode, typeof (DateTime));
                        break;
                    default:
                        var count = (int) intParser.Parse(dateFacetingNode, typeof (int));
                        if (name == FacetDateOther.After.ToString())
                            r.OtherResults[FacetDateOther.After] = count;
                        else if (name == FacetDateOther.Before.ToString())
                            r.OtherResults[FacetDateOther.Before] = count;
                        else if (name == FacetDateOther.Between.ToString())
                            r.OtherResults[FacetDateOther.Between] = count;
                        else {
                            var d = dateParser.ParseDate(name);
                            r.DateResults.Add(KV(d, count));
                        }
                        break;
                }
            }
            return r;
        }
    }
}