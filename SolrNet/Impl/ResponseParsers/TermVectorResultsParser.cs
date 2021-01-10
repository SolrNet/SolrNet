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
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using SolrNet.Utils;

namespace SolrNet.Impl.ResponseParsers {
	/// <summary>
	/// Parses TermVector results from a query response
	/// </summary>
	/// <typeparam name="T">Document type</typeparam>
	public class TermVectorResultsParser<T> : ISolrResponseParser<T> {
        /// <inheritdoc />
		public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
			results.Switch(query: r => Parse(xml, r),
						   moreLikeThis: F.DoNothing);
		}

        /// <inheritdoc />
		public void Parse(XDocument xml, SolrQueryResults<T> results) {
			var rootNode = xml.XPathSelectElement("response/lst[@name='termVectors']");
			if (rootNode != null)
				results.TermVectorResults = ParseDocuments(rootNode).ToList();
		}

		/// <summary>
		/// Parses term vector results
		/// </summary>
		/// <param name="rootNode"></param>
		/// <returns></returns>
        public IEnumerable<TermVectorDocumentResult> ParseDocuments(XElement rootNode) {
			var docNodes = rootNode.Elements("lst");

			foreach (var docNode in docNodes) {
				var docNodeName = docNode.Attribute("name").Value;

				if (docNodeName == "warnings") {
					// TODO: warnings
                } else if (docNodeName == "uniqueKeyFieldName") {
                    //TODO: support for unique key field name
                } else {
                    yield return ParseDoc(docNode);
                }
			}
		}

        private static TermVectorDocumentResult ParseDoc(XElement docNode) {
			var fieldNodes = docNode.Elements();
		    var uniqueKey = fieldNodes
		        .Where(X.AttrEq("name", "uniqueKey"))
		        .Select(x => x.Value)
		        .FirstOrDefault();
		    var termVectorResults = fieldNodes
		        .Where(x => !X.AttrEq("name", "uniqueKey")(x))
		        .SelectMany(ParseField)
                .ToList();

            return new TermVectorDocumentResult(uniqueKey, termVectorResults);
		}

        private static IEnumerable<TermVectorResult> ParseField(XElement fieldNode) {
		    return fieldNode.Elements()
		        .Select(termNode => ParseTerm(termNode, fieldNode.Attribute("name").Value));
		}

        private static TermVectorResult ParseTerm(XElement termNode, string fieldName) {
		    var nameValues = termNode.Elements()
                .Select(e => new {name = e.Attribute("name").Value, value = e.Value})
                .ToList();

		    var tf = nameValues
		        .Where(x => x.name == "tf")
		        .Select(x => (int?) int.Parse(x.value))
		        .FirstOrDefault();

            var df = nameValues
		        .Where(x => x.name == "df")
		        .Select(x => (int?) int.Parse(x.value))
		        .FirstOrDefault();

            var tfidf = nameValues
		        .Where(x => x.name == "tf-idf")
		        .Select(x => (double?) double.Parse(x.value, CultureInfo.InvariantCulture.NumberFormat))
		        .FirstOrDefault();

		    var offsets = termNode.Elements().SelectMany(ParseOffsets).ToList();
            var positions = termNode.Elements().SelectMany(ParsePositions).ToList();

            return new TermVectorResult(fieldName, 
                term: termNode.Attribute("name").Value,
                tf: tf, df: df, tfIdf: tfidf, 
                offsets: offsets, positions: positions);
		}

		private static IEnumerable<int> ParsePositions(XElement valueNode) {
		    return from e in new[] {valueNode}
		           where e.Attribute("name").Value == "positions"
                   from p in e.Elements()
		           select int.Parse(p.Value);
		}

        private static IEnumerable<Offset> ParseOffsets(XElement valueNode) {
            return
                from e in new[] { valueNode }
                where e.Attribute("name").Value == "offsets"
                let startEnd = e.Elements()
                let start = startEnd.FirstOrDefault(x => x.Attribute("name").Value == "start")
                let end = startEnd.FirstOrDefault(x => x.Attribute("name").Value == "end")
                where start != null
                where end != null
                select new Offset(start: int.Parse(start.Value), end: int.Parse(end.Value));
		}
	}
}
