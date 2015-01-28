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

using System.Collections.Generic;
using System.Reflection;
using log4net.Config;
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using NHibernate.SolrNet.Impl;
using NHibernate.Tool.hbm2ddl;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Mapping;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests {
        [Test]
        public void Insert() {
            using (var session = sessionFactory.OpenSession()) {
                session.Save(new Entity {
                    Id = "abcd",
                    Description = "Testing NH-Solr integration",
                    Tags = new[] {"cat1", "aoe"},
                });
                session.Flush();
            }
            using (var session = cfgHelper.OpenSession(sessionFactory)) {
                var entities = session.CreateSolrQuery("solr").List<Entity>();
                Assert.AreEqual(1, entities.Count);
                Assert.AreEqual(2, entities[0].Tags.Count);
            }
        }

        [Test]
        public void DoesntLeakMem() {
            using (var session = cfgHelper.OpenSession(sessionFactory)) {
                session.FlushMode = FlushMode.Never;
                session.Save(new Entity {
                    Id = "abcd",
                    Description = "Testing NH-Solr integration",
                    Tags = new[] { "cat1", "aoe" },
                });
            }
            var listener = cfg.EventListeners.PostInsertEventListeners[0];
            var addField = typeof (SolrNetListener<Entity>).GetField("entitiesToAdd", BindingFlags.NonPublic | BindingFlags.Instance);
            var addDict = (IDictionary<ITransaction, List<Entity>>)addField.GetValue(listener);
            Assert.AreEqual(0, addDict.Count);
        }

        private Configuration SetupNHibernate() {
            var cfg = ConfigurationExtensions.GetEmptyNHConfig();
            cfg.AddXmlString(@"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2' default-lazy='false'>
  <class name='NHibernate.SolrNet.Tests.Entity, NHibernate.SolrNet.Tests'>
    <id name='Id'>
      <generator class='assigned'/>
    </id>
    <property name='Description'/>
  </class>
</hibernate-mapping>");
            new SchemaExport(cfg).Execute(false, true, false);
            return cfg;
        }

        private void SetupSolr() {
            Startup.InitContainer();

            Startup.Container.Remove<IReadOnlyMappingManager>();
            var mapper = new MappingManager();
            mapper.Add(typeof (Entity).GetProperty("Description"), "name");
            mapper.Add(typeof (Entity).GetProperty("Id"), "id");
            mapper.Add(typeof (Entity).GetProperty("Tags"), "cat");
            Startup.Container.Register<IReadOnlyMappingManager>(c => mapper);

            Startup.Container.Remove<ISolrDocumentPropertyVisitor>();
            var propertyVisitor = new DefaultDocumentVisitor(mapper, Startup.Container.GetInstance<ISolrFieldParser>());
            Startup.Container.Register<ISolrDocumentPropertyVisitor>(c => propertyVisitor);

            Startup.Init<Entity>("http://localhost:8983/solr");
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Entity>>();
            solr.Delete(SolrQuery.All);
            solr.Commit();
        }

        [FixtureSetUp]
        public void FixtureSetup() {
            BasicConfigurator.Configure();
            SetupSolr();

            cfg = SetupNHibernate();

            cfgHelper = new CfgHelper();
            cfgHelper.Configure(cfg, true);
            sessionFactory = cfg.BuildSessionFactory();
        }

        [FixtureTearDown]
        public void FixtureTearDown() {
            sessionFactory.Dispose();
        }

        private Configuration cfg;
        private CfgHelper cfgHelper;
        private ISessionFactory sessionFactory;
    }
}