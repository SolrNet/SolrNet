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
using System.Globalization;
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses stats results from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class StatsResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            results.Switch(query: r => Parse(xml, r),
                           moreLikeThis: F.DoNothing);
        }

        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var statsNode = xml.XPathSelectElement("response/lst[@name='stats']");
            if (statsNode != null)
                results.Stats = ParseStats(statsNode, "stats_fields");
        }

        /// <summary>
        /// Parses the stats results and uses recursion to get any facet results
        /// </summary>
        /// <param name="node"></param>
        /// <param name="selector">Start with 'stats_fields'</param>
        /// <returns></returns>
        public Dictionary<string, StatsResult> ParseStats(XElement node, string selector) {
            var d = new Dictionary<string, StatsResult>();
            var mainNode = node.XPathSelectElement(string.Format("lst[@name='{0}']", selector));
            foreach (var n in mainNode.Elements()) {
                var name = n.Attribute("name").Value;
                d[name] = ParseStatsNode(n);
            }

            return d;
        }

        public IDictionary<string, Dictionary<string, StatsResult>> ParseFacetNode(XElement node) {
            var r = new Dictionary<string, Dictionary<string, StatsResult>>();
            foreach (var n in node.Elements()) {
                var facetName = n.Attribute("name").Value;
                r[facetName] = ParseStats(n.Parent, facetName);
            }
            return r;
        }

        public StatsResult ParseStatsNode(XElement node) {
            var r = new StatsResult();
            foreach (var statNode in node.Elements()) {
                var name = statNode.Attribute("name").Value;
                switch (name) {
                    case "min":
                        r.Min = GetDoubleValue(statNode);
                        break;
                    case "max":
                        r.Max = GetDoubleValue(statNode);
                        break;
                    case "sum":
                        r.Sum = GetDoubleValue(statNode);
                        break;
                    case "sumOfSquares":
                        r.SumOfSquares = GetDoubleValue(statNode);
                        break;
                    case "mean":
                        r.Mean = GetDoubleValue(statNode);
                        break;
                    case "stddev":
                        r.StdDev = GetDoubleValue(statNode);
                        break;
                    case "count":
						r.Count = Convert.ToInt64( statNode.Value, CultureInfo.InvariantCulture );
                        break;
                    case "missing":
						r.Missing = Convert.ToInt64( statNode.Value, CultureInfo.InvariantCulture );
                        break;
                    default:
                        r.FacetResults = ParseFacetNode(statNode);
                        break;
                }
            }
            return r;
        }

        private static double GetDoubleValue(XElement statNode) {
            double parsedValue;
            if (!double.TryParse(statNode.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsedValue))
                parsedValue = double.NaN;
            return parsedValue;
        }
    }
}