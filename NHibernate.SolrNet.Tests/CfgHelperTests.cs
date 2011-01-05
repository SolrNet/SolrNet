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
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Event;
using NHibernate.SolrNet.Impl;
using Rhino.Mocks;
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class CfgHelperTests {
        [Test]
        public void Configure_from_servicelocator() {
            var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            mapper.Expect(x => x.GetRegisteredTypes()).Return(new[] { typeof(Entity) });
            serviceLocator.Expect(x => x.GetInstance<IReadOnlyMappingManager>()).Return(mapper);
            var solr = MockRepository.GenerateMock<ISolrOperations<Entity>>();
            serviceLocator.Expect(x => x.GetService(typeof(ISolrOperations<Entity>))).Return(solr);
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
            var provider = MockRepository.GenerateMock<IServiceProvider>();
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            mapper.Expect(x => x.GetRegisteredTypes()).Return(new[] {typeof (Entity)});
            var solr = MockRepository.GenerateMock<ISolrOperations<Entity>>();
            provider.Expect(x => x.GetService(typeof (IReadOnlyMappingManager))).Return(mapper);
            provider.Expect(x => x.GetService(typeof (ISolrOperations<Entity>))).Return(solr);
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
            var provider = MockRepository.GenerateMock<IServiceProvider>();
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            mapper.Expect(x => x.GetRegisteredTypes()).Return(new[] {typeof (Entity)});
            var solr = MockRepository.GenerateMock<ISolrOperations<Entity>>();
            provider.Expect(x => x.GetService(typeof (IReadOnlyMappingManager))).Return(mapper);
            provider.Expect(x => x.GetService(typeof (ISolrOperations<Entity>))).Return(solr);
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
            var mockListener = MockRepository.GenerateMock<IPostInsertEventListener>();
            nhConfig.SetListener(ListenerType.PostInsert, mockListener);

            var provider = MockRepository.GenerateMock<IServiceProvider>();
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            mapper.Expect(x => x.GetRegisteredTypes()).Return(new[] { typeof(Entity) });
            provider.Expect(x => x.GetService(typeof(IReadOnlyMappingManager))).Return(mapper);
            var solr = MockRepository.GenerateMock<ISolrOperations<Entity>>();
            provider.Expect(x => x.GetService(typeof(ISolrOperations<Entity>))).Return(solr);

            var helper = new CfgHelper(provider);
            helper.Configure(nhConfig, true);
            Assert.AreEqual(2, nhConfig.EventListeners.PostInsertEventListeners.Length);
        }

    }
}