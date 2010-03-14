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
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Engine.Query;
using NHibernate.Impl;
using SolrNet;
using SolrNet.Commands.Parameters;
using Order=NHibernate.Criterion.Order;

namespace NHibernate.SolrNet.Impl {
    /// <summary>
    /// NHibernate <see cref="IQuery"/> for SolrNet queries
    /// </summary>
    public class NHSolrQueryImpl : AbstractQueryImpl, INHSolrQuery {
        private readonly QueryOptions options = new QueryOptions();
        private readonly IServiceProvider provider;

        public NHSolrQueryImpl(IServiceProvider provider, string queryString, FlushMode flushMode, ISessionImplementor session, ParameterMetadata parameterMetadata) : 
            base(queryString, flushMode, session, parameterMetadata) {
            this.provider = provider;
        }

        /// <summary>
        /// Ignored
        /// </summary>
        /// <param name="alias"></param>
        /// <param name="lockMode"></param>
        /// <returns>this</returns>
        public override IQuery SetLockMode(string alias, LockMode lockMode) {
            return this;
        }

        /// <summary>
        /// Operation not supported. For Solr updates use the SolrNet interfaces
        /// </summary>
        /// <returns></returns>
        public override int ExecuteUpdate() {
            throw new HibernateException("Operation not supported. For Solr updates use the SolrNet interfaces");
        }

        /// <summary>
        /// Operation not supported. Please use <see cref="Enumerable{T}"/> instead.
        /// </summary>
        /// <returns></returns>
        public override IEnumerable Enumerable() {
            throw new HibernateException("Operation not supported. Please use Enumerable<T>() instead.");
        }

        /// <summary>
        /// Return the query results as an <see cref="System.Collections.Generic.IEnumerable{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>This is not a lazy IEnumerable</remarks>
        public override IEnumerable<T> Enumerable<T>() {
            return Execute<T>();
        }

        /// <summary>
        /// Operation not supported. Please use <see cref="List{T}"/> instead.
        /// </summary>
        /// <returns></returns>
        public override IList List() {
            throw new HibernateException("Operation not supported. Please use List<T>() instead.");
        }

        /// <summary>
        /// Operation not supported. Please use <see cref="List{T}"/> instead.
        /// </summary>
        /// <param name="results"></param>
        public override void List(IList results) {
            throw new HibernateException("Operation not supported. Please use List<T>() instead.");
        }

        /// <summary>
        /// Return the query results as an <see cref="System.Collections.Generic.List{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public override IList<T> List<T>() {
            return new List<T>(Execute<T>());
        }

        /// <summary>
        /// Null
        /// </summary>
        protected override IDictionary<string, LockMode> LockModes {
            get { return null; }
        }

        private ICollection<T> Execute<T>() {
            var solrType = typeof(ISolrReadOnlyOperations<>).MakeGenericType(typeof(T));
            var solr = (ISolrReadOnlyOperations<T>) provider.GetService(solrType);
            return solr.Query(QueryString, options);
        }

        /// <summary>
        /// Set the maximum number of rows to retrieve.
        /// </summary>
        /// <param name="maxResults">The maximum number of rows to retreive</param>
        /// <returns>this</returns>
        public new INHSolrQuery SetMaxResults(int maxResults) {
            options.Rows = maxResults;
            return this;
        }

        /// <summary>
        /// Sets the first row to retrieve.
        /// </summary>
        /// <param name="firstResult">The first row to retreive.</param>
        /// <returns>this</returns>
        public new INHSolrQuery SetFirstResult(int firstResult) {
            options.Start = firstResult;
            return this;
        }

        private SortOrder ConvertOrder(Order o) {
            return SortOrder.Parse(o.ToString());
        }

        /// <summary>
        /// Sets sort order
        /// </summary>
        /// <param name="o">Sort order</param>
        /// <returns>this</returns>
        public INHSolrQuery SetSort(Order o) {
            options.OrderBy = new[] {ConvertOrder(o)};
            return this;
        }
    }
}