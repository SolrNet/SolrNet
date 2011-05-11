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
using SolrNet.Commands.Parameters;
using SolrNet.Schema;

namespace SolrNet {
    /// <summary>
    /// Read-only operations without convenience overloads
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISolrBasicReadOnlyOperations<T> {
        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options);

        /// <summary>
        /// Pings the Solr server.
        /// It can be used by a load balancer in front of a set of Solr servers to check response time of all the Solr servers in order to do response time based load balancing.
        /// See http://wiki.apache.org/solr/SolrConfigXml for more information.
        /// </summary>
        ResponseHeader Ping();

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <returns>Solr schema</returns>
        SolrSchema GetSchema();

        /// <summary>
        /// Gets the current status of the DataImportHandler.
        /// </summary>
        /// <returns>DIH status</returns>
        SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options);
    }
}