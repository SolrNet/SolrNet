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
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.SolrNet.Impl;
using SolrNet;

namespace NHibernate.SolrNet {
    public class CfgHelper {

        public static readonly Dictionary<System.Type, ListenerType[]> ListenerDict = new Dictionary<System.Type, ListenerType[]> {
            {typeof (IEvictEventListener), new[] {ListenerType.Evict}},
            {typeof (IPostInsertEventListener), new[] {ListenerType.PostInsert, ListenerType.PostCommitInsert}},
            {typeof (IPostDeleteEventListener), new[] {ListenerType.PostDelete, ListenerType.PostCommitDelete}},
            {typeof (IPostUpdateEventListener), new[] {ListenerType.PostUpdate, ListenerType.PostCommitUpdate}},
            {typeof (IPreInsertEventListener), new[] {ListenerType.PreInsert}},
            {typeof (IPreDeleteEventListener), new[] {ListenerType.PreDelete}},
            {typeof (IPreUpdateEventListener), new[] {ListenerType.PreUpdate}},
            {typeof (ILoadEventListener), new[] {ListenerType.Load}},
            {typeof (ILockEventListener), new[] {ListenerType.Lock}},
            {typeof (IRefreshEventListener), new[] {ListenerType.Refresh}},
            {typeof (IMergeEventListener), new[] {ListenerType.Merge}},
            {typeof (IFlushEventListener), new[] {ListenerType.Flush}},
            {typeof (IFlushEntityEventListener), new[] {ListenerType.FlushEntity}},
            {typeof (IAutoFlushEventListener), new[] {ListenerType.Autoflush}},
        };



        private readonly IReadOnlyMappingManager mapper;
        private readonly IServiceProvider provider;

        public CfgHelper(IReadOnlyMappingManager mapper, IServiceProvider provider) {
            this.mapper = mapper;
            this.provider = provider;
        }

        public CfgHelper(IServiceProvider provider) {
            this.provider = provider;
            mapper = (IReadOnlyMappingManager) provider.GetService(typeof (IReadOnlyMappingManager));
        }

        /// <summary>
        /// Gets required services from the current <see cref="ServiceLocator"/>
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
                SetListener(config, listener);
            }
            return config;
        }

        private void SetListener(Configuration config, object listener) {
            if (listener == null)
                throw new ArgumentNullException("listener");
            foreach (var intf in listener.GetType().GetInterfaces())
                if (ListenerDict.ContainsKey(intf))
                    foreach (var t in ListenerDict[intf])
                        config.SetListener(t, listener);
        }

        public ISolrSession OpenSession(ISession session) {
            return new SolrSession(session, provider);
        }

        public ISolrSession OpenSession(ISessionFactory sessionFactory) {
            return OpenSession(sessionFactory.OpenSession());
        }

    }
}