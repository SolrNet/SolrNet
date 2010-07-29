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
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.SolrNet.Impl;
using SolrNet;
using SolrNet.Utils;

namespace NHibernate.SolrNet {
    /// <summary>
    /// Helper class to configure NHibernate-SolrNet integration.
    /// </summary>
    public class CfgHelper {

        private struct NHListenerInfo {
            public System.Type Intf;
            public ListenerType ListenerType;
            public Converter<EventListeners, Array> ListenerCollection;
        }

        private static readonly ICollection<NHListenerInfo> ListenerInfo = new[] {
            new NHListenerInfo {
                Intf = typeof(IEvictEventListener),
                ListenerType = ListenerType.Evict,
                ListenerCollection = l => l.EvictEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPostInsertEventListener),
                ListenerType = ListenerType.PostInsert,
                ListenerCollection = l => l.PostInsertEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPostInsertEventListener),
                ListenerType = ListenerType.PostCommitInsert,
                ListenerCollection = l => l.PostCommitInsertEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPostDeleteEventListener),
                ListenerType = ListenerType.PostDelete,
                ListenerCollection = l => l.PostDeleteEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPostDeleteEventListener),
                ListenerType = ListenerType.PostCommitDelete,
                ListenerCollection = l => l.PostCommitDeleteEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPostUpdateEventListener),
                ListenerType = ListenerType.PostUpdate,
                ListenerCollection = l => l.PostUpdateEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPostUpdateEventListener),
                ListenerType = ListenerType.PostCommitUpdate,
                ListenerCollection = l => l.PostCommitUpdateEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPreInsertEventListener),
                ListenerType = ListenerType.PreInsert,
                ListenerCollection = l => l.PreInsertEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPreDeleteEventListener),
                ListenerType = ListenerType.PreDelete,
                ListenerCollection = l => l.PreDeleteEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IPreUpdateEventListener),
                ListenerType = ListenerType.PreUpdate,
                ListenerCollection = l => l.PreUpdateEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(ILoadEventListener),
                ListenerType = ListenerType.Load, // preload, postload?
                ListenerCollection = l => l.LoadEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(ILockEventListener),
                ListenerType = ListenerType.Lock,
                ListenerCollection = l => l.LockEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IRefreshEventListener),
                ListenerType = ListenerType.Refresh,
                ListenerCollection = l => l.RefreshEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IMergeEventListener),
                ListenerType = ListenerType.Merge,
                ListenerCollection = l => l.MergeEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IFlushEventListener),
                ListenerType = ListenerType.Flush,
                ListenerCollection = l => l.FlushEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IFlushEntityEventListener),
                ListenerType = ListenerType.FlushEntity,
                ListenerCollection = l => l.FlushEntityEventListeners,
            },
            new NHListenerInfo {
                Intf = typeof(IAutoFlushEventListener),
                ListenerType = ListenerType.Autoflush,
                ListenerCollection = l => l.AutoFlushEventListeners,
            },
        };

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
                SetListener(config, listener);
            }
            return config;
        }

        internal void SetListener(Configuration config, object listener) {
            if (listener == null)
                throw new ArgumentNullException("listener");
            foreach (var intf in listener.GetType().GetInterfaces()) {
                var intf1 = intf;
                var listenerInfo = Func.Filter(ListenerInfo, i => i.Intf == intf1);
                foreach (var i in listenerInfo) {
                    var currentListeners = i.ListenerCollection(config.EventListeners);
                    config.SetListeners(i.ListenerType, AppendToArray(currentListeners, listener));
                }
            }
        }

        private object[] AppendToArray(Array currentListeners, object listener) {
            var elemType = currentListeners.GetType().GetElementType();
            var newListeners = Array.CreateInstance(elemType, currentListeners.Length + 1);
            currentListeners.CopyTo(newListeners, 0);
            newListeners.SetValue(listener, currentListeners.Length);
            return (object[]) newListeners;
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