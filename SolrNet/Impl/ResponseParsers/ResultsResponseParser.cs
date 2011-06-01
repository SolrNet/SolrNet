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
using System.Xml.Linq;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses documents from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class ResultsResponseParser<T> : ISolrResponseParser<T> {
        private readonly ISolrDocumentResponseParser<T> docParser;

        public ResultsResponseParser(ISolrDocumentResponseParser<T> docParser) {
            this.docParser = docParser;
        }

        public void Parse(XDocument xml, SolrQueryResults<T> results) {
            var resultNode = xml.Element("response").Element("result");

			//FIX BY klaas 
			//If resultNode == null exit func
			//		This can occur when grouped results are returned
			if (resultNode == null)
			{
				return;
			}

            results.NumFound = Convert.ToInt32(resultNode.Attribute("numFound").Value);
            var maxScore = resultNode.Attribute("maxScore");
            if (maxScore != null) {
                results.MaxScore = double.Parse(maxScore.Value, CultureInfo.InvariantCulture.NumberFormat);
            }

            foreach (var result in docParser.ParseResults(resultNode))
                results.Add(result);
        }
    }
}