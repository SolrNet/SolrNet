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
using Microsoft.Practices.ServiceLocation;
using NHibernate.Event;
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    public class CfgHelper {

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

        public Cfg.Configuration Configure(Cfg.Configuration config) {
            foreach (var t in mapper.GetRegisteredTypes()) {
                var listenerType = typeof (SolrNetListener<>).MakeGenericType(t);
                var solrType = typeof (ISolrOperations<>).MakeGenericType(t);
                var solr = provider.GetService(solrType);
                var listener = Activator.CreateInstance(listenerType, solr);
                config.SetListener(ListenerType.PostInsert, listener);
                config.SetListener(ListenerType.PostDelete, listener);
                config.SetListener(ListenerType.PostUpdate, listener);
            }
            return config;
        }
    }
}