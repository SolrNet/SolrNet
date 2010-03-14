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

using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace SolrNet.Impl.ResponseParsers {
    /// <summary>
    /// Parses header (status, QTime, etc) from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class HeaderResponseParser<T> : ISolrResponseParser<T> {
        public void Parse(XmlDocument xml, SolrQueryResults<T> results) {
            var responseHeaderNode = xml.SelectSingleNode("response/lst[@name='responseHeader']");
            if (responseHeaderNode != null) {
                results.Header = ParseHeader(responseHeaderNode);
            }
        }

        /// <summary>
        /// Parses response header
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ResponseHeader ParseHeader(XmlNode node) {
            var r = new ResponseHeader();
            r.Status = int.Parse(node.SelectSingleNode("int[@name='status']").InnerText, CultureInfo.InvariantCulture.NumberFormat);
            r.QTime = int.Parse(node.SelectSingleNode("int[@name='QTime']").InnerText, CultureInfo.InvariantCulture.NumberFormat);
            r.Params = new Dictionary<string, string>();
            var paramNodes = node.SelectNodes("lst[@name='params']/str");
            if (paramNodes != null) {
                foreach (XmlNode n in paramNodes) {
                    r.Params[n.Attributes["name"].InnerText] = n.InnerText;
                }
            }
            return r;
        }
    }
}