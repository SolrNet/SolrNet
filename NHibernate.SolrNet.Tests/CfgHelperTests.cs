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
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Event;
using NHibernate.SolrNet.Impl;
using SolrNet;
using SolrNet.Tests.Mocks;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class CfgHelperTests {
        [Test]
        public void Configure_from_servicelocator() {
            var mapper = new MReadOnlyMappingManager();
            mapper.getRegisteredTypes += () => new[] {typeof (Entity)};

            var serviceLocator = new MServiceLocator();
            serviceLocator.getInstance += MServiceLocator.One<IReadOnlyMappingManager>(mapper);

            var solr = new MSolrOperations<Entity>();
            serviceLocator.getService += MServiceLocator.One<ISolrOperations<Entity>>(solr);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            var nhConfig = ConfigurationExtensions.GetNhConfig();
            var helper = new CfgHelper();
            helper.Configure(nhConfig, true);
            Assert.GreaterThan(nhConfig.EventListeners.PostInsertEventListeners.Length, 0);
            Assert.GreaterThan(nhConfig.EventListeners.PostUpdateEventListeners.Length, 0);
            Assert.GreaterThan(nhConfig.EventListeners.PostDeleteEventListeners.Length, 0);
            var listener = nhConfig.EventListeners.PostInsertEventListeners[0];
            Assert.IsInstanceOfType<SolrNetListener<Entity>>(listener);
        }

        [Test]
        public void Configure_from_serviceprovider() {
            var nhConfig = ConfigurationExtensions.GetNhConfig();

            var provider = new MServiceProvider();
            var mapper = new MReadOnlyMappingManager();
            mapper.getRegisteredTypes += () => new[] {typeof (Entity)};

            var solr = new MSolrOperations<Entity>();
            provider.getService += MServiceLocator.Table(new Dictionary<System.Type, object> {
                {typeof (IReadOnlyMappingManager), mapper},
                {typeof (ISolrOperations<Entity>), solr},
            });
            var helper = new CfgHelper(provider);
            helper.Configure(nhConfig, true);
            Assert.GreaterThan(nhConfig.EventListeners.PostInsertEventListeners.Length, 0);
            Assert.GreaterThan(nhConfig.EventListeners.PostUpdateEventListeners.Length, 0);
            Assert.GreaterThan(nhConfig.EventListeners.PostDeleteEventListeners.Length, 0);
            var listener = nhConfig.EventListeners.PostInsertEventListeners[0];
            Assert.IsInstanceOfType<SolrNetListener<Entity>>(listener);
        }

        [Test]
        public void Configure_with_addparameters() {
            var nhConfig = ConfigurationExtensions.GetNhConfig();
            var mapper = new MReadOnlyMappingManager();
            mapper.getRegisteredTypes += () => new[] {typeof (Entity)};
            var solr = new MSolrOperations<Entity>();

            var provider = new MServiceProvider();
            provider.getService += MServiceLocator.Table(new Dictionary<System.Type, object> {
                {typeof(IReadOnlyMappingManager), mapper},
                {typeof(ISolrOperations<Entity>), solr},
            });

            var addParameters = new AddParameters {CommitWithin = 4343};
            var helper = new CfgHelper(provider);
            helper.Configure(nhConfig, true, addParameters);
            var listener = nhConfig.EventListeners.PostInsertEventListeners[0];
            Assert.IsInstanceOfType<SolrNetListener<Entity>>(listener);
            Assert.AreEqual(addParameters, ((IListenerSettings)listener).AddParameters);
        }

        [Test]
        public void Does_not_override_existing_listeners() {
            var nhConfig = ConfigurationExtensions.GetNhConfig();

            var mockListener = new MPostInsertEventListener();
            nhConfig.SetListener(ListenerType.PostInsert, mockListener);

            var mapper = new MReadOnlyMappingManager();
            mapper.getRegisteredTypes += () => new[] {typeof (Entity)};

            var solr = new MSolrOperations<Entity>();

            var provider = new MServiceProvider();
            provider.getService += MServiceLocator.Table(new Dictionary<System.Type, object> {
                {typeof(IReadOnlyMappingManager), mapper},
                {typeof(ISolrOperations<Entity>), solr},
            });

            var helper = new CfgHelper(provider);
            helper.Configure(nhConfig, true);
            Assert.AreEqual(2, nhConfig.EventListeners.PostInsertEventListeners.Length);
        }

    }
}