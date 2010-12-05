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
using NHibernate.Cfg;
using NHibernate.SolrNet.Impl;
using SolrNet;

namespace NHibernate.SolrNet {
    /// <summary>
    /// Helper class to configure NHibernate-SolrNet integration.
    /// </summary>
    public class CfgHelper {
        private readonly IReadOnlyMappingManager mapper;
        private readonly IServiceProvider provider;

        /// <summary>
        /// Gets SolrNet components from a <see cref="IServiceProvider"/>, except for the <see cref="IReadOnlyMappingManager"/>
        /// </summary>
        /// <param name="mapper">Use this mapper for NHibernate-SolrNet integration</param>
        /// <param name="provider">Used to fetch SolrNet components</param>
        public CfgHelper(IReadOnlyMappingManager mapper, IServiceProvider provider) {
            this.mapper = mapper;
            this.provider = provider;
        }

        /// <summary>
        /// Gets SolrNet components from a <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="provider">Used to fetch SolrNet components</param>
        public CfgHelper(IServiceProvider provider) {
            this.provider = provider;
            mapper = (IReadOnlyMappingManager) provider.GetService(typeof (IReadOnlyMappingManager));
        }

        /// <summary>
        /// Gets SolrNet components from the current <see cref="ServiceLocator"/>
        /// </summary>
        public CfgHelper() {
            provider = ServiceLocator.Current;
            mapper = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
        }

        /// <summary>
        /// Registers SolrNet's NHibernate listeners
        /// </summary>
        /// <param name="config">NHibernate configuration</param>
        /// <param name="autoCommit"></param>
        /// <returns></returns>
        public Configuration Configure(Configuration config, bool autoCommit) {
            foreach (var t in mapper.GetRegisteredTypes()) {
                var listenerType = typeof (SolrNetListener<>).MakeGenericType(t);
                var solrType = typeof (ISolrOperations<>).MakeGenericType(t);
                var solr = provider.GetService(solrType);
                var listener = (ICommitSetting) Activator.CreateInstance(listenerType, solr);
                listener.Commit = autoCommit;
                NHHelper.SetListener(config, listener);
            }
            return config;
        }

        /// <summary>
        /// Wraps a NHibernate <see cref="ISession"/> and adds Solr operations
        /// </summary>
        /// <param name="session"><see cref="ISession"/> to wrap</param>
        /// <returns></returns>
        public ISolrSession OpenSession(ISession session) {
            return new SolrSession(session, provider);
        }

        /// <summary>
        /// Opens a new NHibernate <see cref="ISession"/> and wraps it to add Solr operations
        /// </summary>
        /// <returns></returns>
        public ISolrSession OpenSession(ISessionFactory sessionFactory) {
            return OpenSession(sessionFactory.OpenSession());
        }

    }
}