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
using System.Linq;
using System.Xml.Linq;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses documents from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class ResultsResponseParser<T> : ISolrAbstractResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public ResultsResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        private static XElement GetMainResultNode(XDocument xml) {
            return xml.Element("response").Elements("result")
                .FirstOrDefault(e => {
                    var nameAttr = e.Attribute("name");
                    if (nameAttr == null)
                        return true;
                    return string.IsNullOrEmpty(nameAttr.Value) || nameAttr.Value == "response";
                });
        }

        private static XElement GetGroupResultNode(XDocument xml) {
            var groupElement = xml.Element("response").Elements("lst")
                .FirstOrDefault(e => e.Attribute("name").Value == "grouped");
            if (groupElement == null)
                return null;
            return groupElement.Descendants().FirstOrDefault(e => e.Name == "result");
        }

        private static StartOrCursor.Cursor GetNextCursorMark(XDocument xml)
        {
            var nextCursorMarkElement = xml.Element("response").Elements("str")
                .FirstOrDefault(e => e.Attribute("name").Value == "nextCursorMark");
            if (nextCursorMarkElement == null)
                return null;
            return new StartOrCursor.Cursor(nextCursorMarkElement.Value);
        }

        /// <inheritdoc/>
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) {
            var resultNode = GetMainResultNode(xml) ?? GetGroupResultNode(xml);
            if (resultNode == null)
                return;

            results.NumFound = Convert.ToInt64(resultNode.Attribute("numFound").Value);
            results.Start = Convert.ToInt64(resultNode.Attribute("start").Value);
            var maxScore = resultNode.Attribute("maxScore");
            if (maxScore != null)
                results.MaxScore = double.Parse(maxScore.Value, CultureInfo.InvariantCulture.NumberFormat);

            results.AddRange(docParser.ParseResults(resultNode));
            results.NextCursorMark = GetNextCursorMark(xml);
        }
    }
}
