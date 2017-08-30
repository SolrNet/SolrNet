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
using SolrNet.Impl.FieldParsers;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses facets from query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class FacetsResponseParser<T> : ISolrAbstractResponseParser<T> {
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            var mainFacetNode = xml.Element("response")
                .Elements("lst")
                .FirstOrDefault(X.AttrEq("name", "facet_counts"));
            if (mainFacetNode != null) {
                results.FacetQueries = ParseFacetQueries(mainFacetNode);
                results.FacetFields = ParseFacetFields(mainFacetNode);
                results.FacetDates = ParseFacetDates(mainFacetNode);
				results.FacetPivots = ParseFacetPivots(mainFacetNode);
                results.FacetRanges = ParseFacetRanges(mainFacetNode);
                results.FacetIntervals = ParseFacetIntervals(mainFacetNode);
            }
        }

        /// <summary>
        /// Parses facet queries results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, int> ParseFacetQueries(XElement node) {
            var d = new Dictionary<string, int>();
            var facetQueries = node.Elements("lst")
                .Where(X.AttrEq("name", "facet_queries"))
                .Elements();
            foreach (var fieldNode in facetQueries) {
                var key = fieldNode.Attribute("name").Value;
                var value = Convert.ToInt32(fieldNode.Value);
                d[key] = value;
            }
            return d;
        }

    

        /// <summary>
        /// Parses facet fields results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, ICollection<KeyValuePair<string, int>>> ParseFacetFields(XElement node) {
            var d = new Dictionary<string, ICollection<KeyValuePair<string, int>>>();
            var facetFields = node.Elements("lst")
                .Where(X.AttrEq("name", "facet_fields"))
                .SelectMany(x => x.Elements());
            foreach (var fieldNode in facetFields) {
                var field = fieldNode.Attribute("name").Value;
                var c = new List<KeyValuePair<string, int>>();
                foreach (var facetNode in fieldNode.Elements()) {
                    var nameAttr = facetNode.Attribute("name");
                    var key = nameAttr == null ? "" : nameAttr.Value;
                    var value = Convert.ToInt32(facetNode.Value);
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
        [Obsolete("As of Solr 3.1 has been deprecated, as of Solr 6.6 unsupported.")]
        public IDictionary<string, DateFacetingResult> ParseFacetDates(XElement node) {
            var d = new Dictionary<string, DateFacetingResult>();
            var facetDateNode = node.Elements("lst")
                .Where(X.AttrEq("name", "facet_dates"));
            if (facetDateNode != null) {
                foreach (var fieldNode in facetDateNode.Elements()) {
                    var name = fieldNode.Attribute("name").Value;
                    d[name] = ParseDateFacetingNode(fieldNode);
                }
            }
            return d;
        }

        /// <summary>
        /// Parses facet range results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, RangeFacetingResult> ParseFacetRanges(XElement node)
        {
            var d = new Dictionary<string, RangeFacetingResult>();
            var facetRangeNode = node.Elements("lst")
                .Where(X.AttrEq("name", "facet_ranges"));
            if (facetRangeNode != null)
            {
                foreach (var fieldNode in facetRangeNode.Elements())
                {
                    var name = fieldNode.Attribute("name").Value;
                    d[name] = ParseRangeFacetingNode(fieldNode);
                }
            }
            return d;
        }

        [Obsolete("As of Solr 3.1 has been deprecated, as of Solr 6.6 unsupported.")]
        public DateFacetingResult ParseDateFacetingNode(XElement node) {
            var r = new DateFacetingResult();
            var intParser = new IntFieldParser();
            foreach (var dateFacetingNode in node.Elements()) {
                var name = dateFacetingNode.Attribute("name").Value;
                switch (name) {
                    case "gap":
                        r.Gap = dateFacetingNode.Value;
                        break;
                    case "end":
                        r.End = DateTimeFieldParser.ParseDate(dateFacetingNode.Value);
                        break;
                    default:
                        // Temp fix to support Solr 3.1, which has added a new element <date name="start">...</date>
                        // not seen in Solr 1.4 to the facet date response – just ignore this element.
                        if (dateFacetingNode.Name != "int")
                            break;
                            
                        var count = (int) intParser.Parse(dateFacetingNode, typeof (int));
                        if (name == FacetDateOther.After.ToString())
                            r.OtherResults[FacetDateOther.After] = count;
                        else if (name == FacetDateOther.Before.ToString())
                            r.OtherResults[FacetDateOther.Before] = count;
                        else if (name == FacetDateOther.Between.ToString())
                            r.OtherResults[FacetDateOther.Between] = count;
                        else {
                            var d = DateTimeFieldParser.ParseDate(name);
                            r.DateResults.Add(KV.Create(d, count));
                        }
                        break;
                }
            }
            return r;
        }

        public RangeFacetingResult ParseRangeFacetingNode(XElement node)
        {
            var r = new RangeFacetingResult();
            var intParser = new IntFieldParser();
            foreach (var rangeFacetingNode in node.Elements())
            {
                var name = rangeFacetingNode.Attribute("name").Value;
                switch (name)
                {
                    case "gap":
                        r.Gap = rangeFacetingNode.Value;
                        break;
                    case "start":
                        r.Start = rangeFacetingNode.Value;
                        break;
                    case "end":
                        r.End = rangeFacetingNode.Value;
                        break;
                    case "counts":
                        foreach (var item in rangeFacetingNode.Elements()) {
                            r.RangeResults.Add(KV.Create(item.Attribute("name").Value, (int)intParser.Parse(item, typeof(int))));
                        }
                        break;
                    default:
                        //collect FacetRangeOther items
                        if (rangeFacetingNode.Name != "int")
                            break;

                        var count = (int)intParser.Parse(rangeFacetingNode, typeof(int));
                        if (name == FacetDateOther.After.ToString())
                            r.OtherResults[FacetRangeOther.After] = count;
                        else if (name == FacetDateOther.Before.ToString())
                            r.OtherResults[FacetRangeOther.Before] = count;
                        else if (name == FacetDateOther.Between.ToString())
                            r.OtherResults[FacetRangeOther.Between] = count;
                        break;
                }
            }
            return r;
        }


        /// <summary>
        /// Parses facet interval results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, ICollection<KeyValuePair<string,int>>> ParseFacetIntervals(XElement node)
        {
            var d = new Dictionary<string, ICollection<KeyValuePair<string,int>>>();
            var facetIntervals = node.Elements("lst")
                .Where(X.AttrEq("name", "facet_intervals"))
                .SelectMany(x=>x.Elements());
            foreach (var fieldNode in facetIntervals)
            {
                var field = fieldNode.Attribute("name").Value;
                var c = new List<KeyValuePair<string, int>>();
                foreach (var facetNode in fieldNode.Elements())
                {
                    var nameAttr = facetNode.Attribute("name");
                    var key = nameAttr?.Value ?? "";
                    var value = Convert.ToInt32(facetNode.Value);
                    c.Add(new KeyValuePair<string, int>(key, value));
                }
                d[field] = c;
            }


            return d;
        }


        /// <summary>
        /// Parses facet pivot results
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public IDictionary<string, IList<Pivot>> ParseFacetPivots(XElement node)
		{
			var d = new Dictionary<string, IList<Pivot>>();
            var facetPivotNode = node.Elements("lst")
                .Where(X.AttrEq("name", "facet_pivot"));
            foreach (var fieldNode in facetPivotNode.Elements()) {
                var name = fieldNode.Attribute("name").Value;
                d[name] = fieldNode.Elements("lst").Select(ParsePivotNode).ToArray();
            }
            return d;
		}

		public Pivot ParsePivotNode(XElement node)
		{
			Pivot pivot = new Pivot();

            pivot.Field = node.Elements("str").First(X.AttrEq("name", "field")).Value;
            pivot.Value = node.Elements().First(X.AttrEq("name", "value")).Value;
            pivot.Count = int.Parse(node.Elements("int").First(X.AttrEq("name", "count")).Value);

            var childPivotNodes = node.Elements("arr").Where(X.AttrEq("name", "pivot")).ToList();
			if (childPivotNodes.Count > 0)
			{
				pivot.HasChildPivots = true;
				pivot.ChildPivots = new List<Pivot>();

				foreach (var childNode in childPivotNodes.Elements())
				{
					pivot.ChildPivots.Add(ParsePivotNode(childNode));
				}
			}


			return pivot;
		}

    }
}