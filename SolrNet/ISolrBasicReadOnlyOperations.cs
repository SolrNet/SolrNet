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
using SolrNet.Impl;
using SolrNet.Schema;
using System.Threading.Tasks;
using System.Threading;

namespace SolrNet
{
    /// <summary>
    /// Read-only operations without convenience overloads
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISolrBasicReadOnlyOperations<T>
    {
        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        SolrQueryResults<T> Query(ISolrQuery query, QueryOptions options);

        /// <summary>
        /// Executes a query asynchronously
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes a MoreLikeThisHandler query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        SolrMoreLikeThisHandlerResults<T> MoreLikeThis(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options);

        /// <summary>
        /// Executes a MoreLikeThisHandler query asynchronously
        /// </summary>
        /// <param name="query"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SolrMoreLikeThisHandlerResults<T>> MoreLikeThisAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Pings the Solr server.
        /// It can be used by a load balancer in front of a set of Solr servers to check response time of all the Solr servers in order to do response time based load balancing.
        /// See http://wiki.apache.org/solr/SolrConfigXml for more information.
        /// </summary>
        ResponseHeader Ping();


        /// <summary>
        /// Pings the Solr server asynchronously.
        /// It can be used by a load balancer in front of a set of Solr servers to check response time of all the Solr servers in order to do response time based load balancing.
        /// See http://wiki.apache.org/solr/SolrConfigXml for more information.
        /// </summary>
        Task<ResponseHeader> PingAsync();

        /// <summary>
        /// Gets the schema.
        /// </summary>
        /// <param name="schemaFileName">Name of the schema file.</param>
        /// <returns> Solr schema </returns>
        SolrSchema GetSchema(string schemaFileName);

        /// <summary>
        /// Gets the schema asynchronously.
        /// </summary>
        /// <param name="schemaFileName">Name of the schema file.</param>
        /// <returns> Solr schema </returns>
        Task<SolrSchema> GetSchemaAsync(string schemaFileName);

        /// <summary>
        /// Gets the current status of the DataImportHandler.
        /// </summary>
        /// <returns>DIH status</returns>
        SolrDIHStatus GetDIHStatus(KeyValuePair<string, string> options);

        /// <summary>
        /// Gets the current status of the DataImportHandler asynchronously.
        /// </summary>
        /// <returns>DIH status</returns>
        Task<SolrDIHStatus> GetDIHStatusAsync(KeyValuePair<string, string> options);
    }
}
