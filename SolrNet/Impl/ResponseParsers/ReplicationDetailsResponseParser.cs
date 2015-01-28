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
            ResponseHeader responseHeader = new ResponseHeader();
            string indexSize = string.Empty;
            string indexPath = string.Empty;
            string isMaster = string.Empty;
            string isSlave = string.Empty;
            long indexVersion = -1;
            long generation = -1;
            string isReplicating = null;
            string totalPercent = null;
            string timeRemaining = null;

            var responseHeaderNode = response.XPathSelectElement("response/lst[@name='responseHeader']");
            if (responseHeaderNode != null)
                responseHeader = ParseHeader(responseHeaderNode);
            else
                return null;

            var responseIndexSizeNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='indexSize']");
            if (responseIndexSizeNode != null)
                indexSize = responseIndexSizeNode.Value;
            else
                indexSize = string.Empty;

            var responseIndexPathNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='indexPath']");
            if (responseIndexPathNode != null)
                indexPath = responseIndexPathNode.Value;
            else
                indexPath = string.Empty;

            var responseIsMasterNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='isMaster']");
            if (responseIsMasterNode != null)
                isMaster = responseIsMasterNode.Value;
            else
                isMaster = string.Empty;

            var responseIsSlaveNode = response.XPathSelectElement("response/lst[@name='details']/str[@name='isSlave']");
            if (responseIsSlaveNode != null)
                isSlave = responseIsSlaveNode.Value;
            else
                isSlave = string.Empty;

            var responseIndexVersionNode = response.XPathSelectElement("response/lst[@name='details']/long[@name='indexVersion']");
            if (responseIndexVersionNode != null)
                indexVersion = long.Parse(responseIndexVersionNode.Value, CultureInfo.InvariantCulture.NumberFormat);

            var responseGenerationNode = response.XPathSelectElement("response/lst[@name='details']/long[@name='generation']");
            if (responseGenerationNode != null)
                generation = long.Parse(responseGenerationNode.Value, CultureInfo.InvariantCulture.NumberFormat);

            //slave node
            var responseSlaveNode = response.XPathSelectElement("response/lst[@name='details']/lst[@name='slave']");
            if (responseSlaveNode != null)
            {
                var isReplicatingTemp = responseSlaveNode.XPathSelectElement("str[@name='isReplicating']");
                isReplicating = isReplicatingTemp == null ? null : isReplicatingTemp.Value;

                if (isReplicating != null && isReplicating.ToLower() == "true")
                {
                    var totalPercentTemp = responseSlaveNode.XPathSelectElement("str[@name='totalPercent']");
                    totalPercent = totalPercentTemp == null ? null : totalPercentTemp.Value;

                    var timeRemainingTemp = responseSlaveNode.XPathSelectElement("str[@name='timeRemaining']");
                    timeRemaining = timeRemainingTemp == null ? null : timeRemainingTemp.Value;
                }
            }

            return new ReplicationDetailsResponse(responseHeader, indexSize, indexPath, isMaster, isSlave, indexVersion, generation, isReplicating, totalPercent, timeRemaining);
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
