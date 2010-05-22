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
using Microsoft.Practices.ServiceLocation;
using NHibernate.SolrNet.Impl;
using SolrNet;
using SolrNet.Impl;

namespace NHibernate.SolrNet {
    /// <summary>
    /// NHibernate <see cref="ISession"/> with SolrNet extensions for querying
    /// </summary>
    public class SolrSession : DelegatingSession, ISolrSession {
        private readonly ISession session;
        private readonly IServiceProvider provider;

        /// <summary>
        /// Creates a session using the current <see cref="ServiceLocator"/>
        /// </summary>
        /// <param name="session">NHibernate session to wrap</param>
        /// <remarks>The wrapped session is owned by this session. It will be disposed when this session is disposed</remarks>
        public SolrSession(ISession session) : this(session, ServiceLocator.Current) {}

        /// <summary>
        /// Creates a session using a defined <see cref="IServiceProvider"/> to fetch SolrNet components
        /// </summary>
        /// <param name="session">NHibernate session to wrap</param>
        /// <param name="provider">Used to fetch SolrNet components</param>
        /// <remarks>The wrapped session is owned by this session. It will be disposed when this session is disposed</remarks>
        public SolrSession(ISession session, IServiceProvider provider) : base(session) {
            if (session == null)
                throw new ArgumentNullException("session");
            if (provider == null)
                throw new ArgumentNullException("provider");
            this.session = session;
            this.provider = provider;
        }

        /// <summary>
        /// Creates a Solr query
        /// </summary>
        /// <param name="query">Solr query</param>
        /// <returns>query object</returns>
        public INHSolrQuery CreateSolrQuery(string query) {
            return new NHSolrQueryImpl(provider, query, session.FlushMode, session.GetSessionImplementation(), null);
        }

        /// <summary>
        /// Creates a Solr query
        /// </summary>
        /// <param name="query">Solr query</param>
        /// <returns>query object</returns>
        public INHSolrQuery CreateSolrQuery(ISolrQuery query) {
            var serializer = (ISolrQuerySerializer) provider.GetService(typeof (ISolrQuerySerializer));
            return CreateSolrQuery(serializer.Serialize(query));
        }
    }
}