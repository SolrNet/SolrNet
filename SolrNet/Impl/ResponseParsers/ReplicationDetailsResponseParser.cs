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
    /// Parses header (status, QTime, etc) and details from a response
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public class ReplicationDetailsResponseParser<T> : ISolrAbstractResponseParser<T>, ISolrReplicationDetailsResponseParser
    {
        /// <summary>
        /// Header parser
        /// </summary>
        /// <param name="xml">XML</param>
        /// <param name="results">results</param>
        public void Parse(XDocument xml, AbstractSolrQueryResults<T> results) 
        {
            var header = Parse(xml);
            if (header != null)
                results.Header = header.responseHeader;
        }

        /// <summary>
        /// Parses XML response to response class
        /// </summary>
        /// <param name="response">XML</param>
        /// <returns>ReplicationDetailsResponse class</returns>
        public ReplicationDetailsResponse Parse(XDocument response)
        {
            ReplicationDetailsResponse rivr = new ReplicationDetailsResponse();
            var responseHeaderNode = response.XPathSelectElement("response/lst[@name='responseHeader']");
            if (responseHeaderNode != null)
                rivr.responseHeader = ParseHeader(responseHeaderNode);
            else
                return null;

            var responseIndexSizeNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='indexSize']");
            if (responseIndexSizeNode != null)
                rivr.indexSize = responseIndexSizeNode.Value;
            else
                return null;

            var responseIndexPathNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='indexPath']");
            if (responseIndexPathNode != null)
                rivr.indexPath = responseIndexPathNode.Value;
            else
                return null;

            var responseIsMasterNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='isMaster']");
            if (responseIsMasterNode != null)
                rivr.isMaster = responseIsMasterNode.Value;
            else
                return null;

            var responseIsSlaveNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='isSlave']");
            if (responseIsSlaveNode != null)
                rivr.isSlave = responseIsSlaveNode.Value;
            else
                return null;

            var responseIndexVersionNode = response.XPathSelectElement("response/lst[@name='details']/long[@name='indexVersion']");
            if (responseIndexVersionNode != null)
                rivr.indexversion = long.Parse(responseIndexVersionNode.Value, CultureInfo.InvariantCulture.NumberFormat);
            else
                return null;

            var responseGenerationNode = response.XPathSelectElement("response/lst[@name='details']/long[@name='generation']");
            if (responseGenerationNode != null)
                rivr.generation = long.Parse(responseGenerationNode.Value, CultureInfo.InvariantCulture.NumberFormat);
            else
                return null;

            //slave node
            var responseSlaveNode = response.XPathSelectElement("response/lst[@name='details']/lst[@name='slave']");
            if (responseSlaveNode != null)
            {
                var isReplicating = responseSlaveNode.XPathSelectElement("str[@name='isReplicating']");
                rivr.isReplicating = isReplicating == null ? null : isReplicating.Value;

                if (rivr.isReplicating != null && rivr.isReplicating.ToLower() == "true")
                {
                     var totalPercent = responseSlaveNode.XPathSelectElement("str[@name='totalPercent']");
                    rivr.totalPercent = totalPercent == null ? null : totalPercent.Value;

                    var timeRemaining = responseSlaveNode.XPathSelectElement("str[@name='timeRemaining']");
                    rivr.timeRemaining = timeRemaining == null ? null : timeRemaining.Value;
                }
            }

            return rivr;
        }

        /// <summary>
        /// Parses response header
        /// </summary>
        /// <param name="node">XML</param>
        /// <returns>ResponseHeader</returns>
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
    }
}
