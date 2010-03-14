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
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    public class SolrSession : DelegatingSession, ISolrSession {
        private readonly ISession session;
        private readonly IServiceProvider provider;

        public SolrSession(ISession session) : this(session, ServiceLocator.Current) {}

        public SolrSession(ISession session, IServiceProvider provider) : base(session) {
            if (session == null)
                throw new ArgumentNullException("session");
            if (provider == null)
                throw new ArgumentNullException("provider");
            this.session = session;
            this.provider = provider;
        }

        public INHSolrQuery CreateSolrQuery(string query) {
            return new NHSolrQueryImpl(provider, query, session.FlushMode, session.GetSessionImplementation(), null);
        }

        public INHSolrQuery CreateSolrQuery(ISolrQuery query) {
            return CreateSolrQuery(query.Query);
        }
    }
}