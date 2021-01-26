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

namespace SolrNet.Impl.ResponseParsers
{
    /// <summary>
    /// Parses header (status, QTime, etc) from a query response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class HeaderResponseParser<T> : HeaderResponseParser, ISolrAbstractResponseParser<T>
    {
        /// <inheritdoc />
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results)
        {
            var header = Parse(xml);
            if (header != null)
                results.Header = header;
        }
    }

    public class HeaderResponseParser : ISolrHeaderResponseParser
    {
        public void Parse(XDocument xml, AbstractSolrQueryResults<string> results)
        {
            var header = Parse(xml);
            if (header != null)
                results.Header = header;
        }

        /// <summary>
        /// Parses response header
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public ResponseHeader ParseHeader(XElement node)
        {
            var r = new ResponseHeader();
            r.Status = int.Parse(node.XPathSelectElement("int[@name='status']").Value, CultureInfo.InvariantCulture.NumberFormat);
            r.QTime = int.Parse(node.XPathSelectElement("int[@name='QTime']").Value, CultureInfo.InvariantCulture.NumberFormat);
            r.Params = new Dictionary<string, string>();
            var paramNodes = node.XPathSelectElements("lst[@name='params']/str");
            foreach (var n in paramNodes)
            {
                r.Params[n.Attribute("name").Value] = n.Value;
            }
            return r;
        }

        /// <inheritdoc />
        public ResponseHeader Parse(XDocument response)
        {
            var responseHeaderNode = response.XPathSelectElement("response/lst[@name='responseHeader']");
            if (responseHeaderNode != null)
                return ParseHeader(responseHeaderNode);
            return null;
        }
    }
}
