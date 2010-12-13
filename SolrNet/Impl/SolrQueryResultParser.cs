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
using System.Xml;
using System.Xml.Linq;

namespace SolrNet.Impl {
    /// <summary>
    /// Default query results parser.
    /// Parses xml query results
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class SolrQueryResultParser<T> : ISolrQueryResultParser<T> {
        private readonly ISolrResponseParser<T>[] parsers;

        public SolrQueryResultParser(ISolrResponseParser<T>[] parsers) {
            this.parsers = parsers;
        }

        /// <summary>
        /// Parses solr's xml response
        /// </summary>
        /// <param name="r">solr xml response</param>
        /// <returns>query results</returns>
        public ISolrQueryResults<T> Parse(string r) {
            var results = new SolrQueryResults<T>();
            var xml = XDocument.Parse(r);
            foreach (var p in parsers)
                p.Parse(xml, results);

            return results;
        }
    }
}