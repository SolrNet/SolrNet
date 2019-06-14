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

using System.Threading.Tasks;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet
{
    /// <summary>
    /// Executable query
    /// </summary>
    /// <typeparam name="T">Document type</typeparam>
    public interface ISolrQueryExecuter<T>
    {
        /// <summary>
        /// Executes the query and returns results
        /// </summary>
        /// <returns>query results</returns>
        SolrQueryResults<T> Execute(ISolrQuery q, QueryOptions options);

        SolrMoreLikeThisHandlerResults<T> Execute(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options);

        /// <summary>
        /// Executes the query and returns results
        /// </summary>
        /// <returns>query results</returns>
        Task<SolrQueryResults<T>> ExecuteAsync(ISolrQuery q, QueryOptions options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Executes the query with a body and returns results
        /// </summary>
        /// <returns>query results</returns>
        Task<SolrQueryResults<T>> ExecuteAsync(ISolrQuery q, ISolrQueryBody body, QueryOptions options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        Task<SolrMoreLikeThisHandlerResults<T>> ExecuteAsync(SolrMLTQuery query, MoreLikeThisHandlerQueryOptions options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));

        /// <summary>
        /// Executes a MoreLikeThis query, taking a body to compare, and returns results.
        /// </summary>
        /// <param name="query">The query text</param>
        /// <param name="body">The body to compare against.</param>
        /// <param name="options">Additional query options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns></returns>
        Task<SolrMoreLikeThisHandlerResults<T>> ExecuteAsync(SolrMLTQuery query, ISolrQueryBody body, MoreLikeThisHandlerQueryOptions options, System.Threading.CancellationToken cancellationToken = default(System.Threading.CancellationToken));
    }
}
