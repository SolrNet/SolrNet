#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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

namespace NHibernate.SolrNet {
    public class NHSolrQueryImpl : AbstractQueryImpl, INHSolrQuery {
        private readonly QueryOptions options = new QueryOptions();
        private readonly IServiceProvider provider;

        public NHSolrQueryImpl(IServiceProvider provider, string queryString, FlushMode flushMode, ISessionImplementor session, ParameterMetadata parameterMetadata) : 
            base(queryString, flushMode, session, parameterMetadata) {
            this.provider = provider;
        }

        public override IQuery SetLockMode(string alias, LockMode lockMode) {
            return this;
        }

        public override int ExecuteUpdate() {
            throw new HibernateException("Not supported operation");
        }

        public override IEnumerable Enumerable() {
            throw new HibernateException("Not supported operation");
        }

        public override IEnumerable<T> Enumerable<T>() {
            return Execute<T>();
        }

        public override IList List() {
            throw new HibernateException("Not supported operation");
        }

        public override void List(IList results) {
            throw new HibernateException("Not supported operation");
        }

        public override IList<T> List<T>() {
            return new List<T>(Execute<T>());
        }

        private ICollection<T> Execute<T>() {
            var solrType = typeof(ISolrReadOnlyOperations<>).MakeGenericType(typeof(T));
            var solr = (ISolrReadOnlyOperations<T>) provider.GetService(solrType);
            return solr.Query(QueryString, options);
        }

        public new INHSolrQuery SetMaxResults(int maxResults) {
            options.Rows = maxResults;
            return this;
        }

        public new INHSolrQuery SetFirstResult(int firstResult) {
            options.Start = firstResult;
            return this;
        }

        private SortOrder ConvertOrder(Order o) {
            return SortOrder.Parse(o.ToString());
        }

        public INHSolrQuery SetSort(Order o) {
            options.OrderBy = new[] {ConvertOrder(o)};
            return this;
        }

        protected override IDictionary LockModes {
            get { return null; }
        }
    }
}