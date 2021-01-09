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
using System.Threading.Tasks;
using System.Threading;

namespace SolrNet
{
    /// <summary>
    /// Read-only operations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ISolrReadOnlyOperations<T> : ISolrBasicReadOnlyOperations<T>
    {
        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q">query to execute</param>
        /// <returns>query results</returns>
        SolrQueryResults<T> Query(string q);

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        SolrQueryResults<T> Query(string q, ICollection<SortOrder> orders);

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        SolrQueryResults<T> Query(string q, QueryOptions options);

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        SolrQueryResults<T> Query(ISolrQuery q);

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orders"></param>
        /// <returns></returns>
        SolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders);

        /// <summary>
        /// Executes a single facet field query
        /// </summary>
        /// <param name="facets"></param>
        /// <returns></returns>
        ICollection<KeyValuePair<string, int>> FacetFieldQuery(SolrFacetFieldQuery facets);

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q">query to execute</param>
        /// <param name="cancellationToken"></param>
        /// <returns>query results</returns>
        Task<SolrQueryResults<T>> QueryAsync(string q, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="orders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SolrQueryResults<T>> QueryAsync(string q, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="options"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SolrQueryResults<T>> QueryAsync(string q, QueryOptions options, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="q"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SolrQueryResults<T>> QueryAsync(ISolrQuery q, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes a query
        /// </summary>
        /// <param name="query"></param>
        /// <param name="orders"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<SolrQueryResults<T>> QueryAsync(ISolrQuery query, ICollection<SortOrder> orders, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Executes a single facet field query
        /// </summary>
        /// <param name="facets"></param>
        /// <returns></returns>
        Task<ICollection<KeyValuePair<string, int>>> FacetFieldQueryAsync(SolrFacetFieldQuery facets);

    }
}
