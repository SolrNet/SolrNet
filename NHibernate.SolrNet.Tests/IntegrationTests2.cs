using System.Collections.Generic;
using log4net.Config;
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Tool.hbm2ddl;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Mapping;

namespace NHibernate.SolrNet.Tests {
    /// <summary>
    /// This test does not reflect a reference usage of Solr, SolrNet or the SolrNet-NHibernate integration.
    /// Storing more than one type in a single Solr index should almost never be done.
    /// Instead, the object model should be flattened.
    /// Do not use as reference.
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    public class IntegrationTests2 {
        private const string _httpSolrTest = "http://localhost:8983/solr";

        [Test]
        public void InsertAGraph() {
            using (var session = cfgHelper.OpenSession(sessionFactory)) {
                session.FlushMode = FlushMode.Never;
                var parent = new Parent {Id = "1", ParentProp1 = "Test"};

                var child1 = parent.AddChild(new Child {ChildProp1 = "Child1", Id = "2"});
                var child2 = parent.AddChild(new Child {ChildProp1 = "Child2", Id = "3"});
                var child3 = parent.AddChild(new Child {ChildProp1 = "Child3", Id = "4"});

                session.Save(parent);
                session.Save(child1);
                session.Save(child2);
                session.Save(child3);
                session.Flush();
            }

            var solr = ServiceLocator.Current.GetInstance<ISolrReadOnlyOperations<Dictionary<string,object>>>();
            var all = solr.Query(SolrQuery.All);
            Assert.AreEqual(4, all.Count);
        }

        private static Configuration SetupNHibernate() {
            var cfg = ConfigurationExtensions.GetEmptyNHConfig();
            cfg.AddXmlString(
                @"<?xml version='1.0' encoding='utf-8' ?>
<hibernate-mapping xmlns='urn:nhibernate-mapping-2.2' default-lazy='false'>

  <class name='NHibernate.SolrNet.Tests.Parent, NHibernate.SolrNet.Tests'>
    <id name='Id'>
      <generator class='assigned'/>
    </id>
    
    <property name='ParentProp1'/>
    <bag name='Children' lazy='true' cascade='none' >
            <key column='parent_id'/>
            <one-to-many class='NHibernate.SolrNet.Tests.Child, NHibernate.SolrNet.Tests'/>
        </bag>
  </class>

  <class name='NHibernate.SolrNet.Tests.Child, NHibernate.SolrNet.Tests'>
    <id name='Id'>
      <generator class='assigned'/>
    </id>
    
    <property name='ChildProp1'/>
    
  </class>


</hibernate-mapping>");
            new SchemaExport(cfg).Execute(false, true, false);
            return cfg;
        }


        private static void SetupSolr() {
            var connection = new SolrConnection(_httpSolrTest);
            Startup.InitContainer();

            Startup.Container.Remove<IReadOnlyMappingManager>();

            var mapper = new MappingManager();
            Mappings(mapper);
            Startup.Container.Register<IReadOnlyMappingManager>(c => mapper);

            Startup.Container.Remove<ISolrDocumentPropertyVisitor>();
            var propertyVisitor = new DefaultDocumentVisitor(mapper, Startup.Container.GetInstance<ISolrFieldParser>());
            Startup.Container.Register<ISolrDocumentPropertyVisitor>(c => propertyVisitor);

            Startup.Init<Child>(connection);
            Startup.Init<Parent>(connection);
            Startup.Init<Dictionary<string,object>>(connection);
            Startup.Container.RemoveAll<ISolrDocumentResponseParser<Dictionary<string, object>>>();
            Startup.Container.Register<ISolrDocumentResponseParser<Dictionary<string, object>>>(c => new SolrDictionaryDocumentResponseParser(c.GetInstance<ISolrFieldParser>()));

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<Child>>();

            solr.Delete(SolrQuery.All);
            solr.Commit();
        }

        private static IMappingManager Mappings(IMappingManager mapper) {
            if (mapper == null)
                mapper = new MappingManager();
            mapper.Add(typeof (Child).GetProperty("Id"), "id");
            mapper.Add(typeof (Child).GetProperty("ChildProp1"), "name_s");
            mapper.Add(typeof (Parent).GetProperty("Id"), "id");
            mapper.Add(typeof(Parent).GetProperty("ParentProp1"), "name_s");
            return mapper;
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

    public class Parent {
        public virtual string Id { get; set; }
        public virtual string ParentProp1 { get; set; }
        public virtual IList<Child> Children { get; set; }

        public Child AddChild(Child child) {
            if (Children == null)
                Children = new List<Child>();

            if (child == null)
                child = new Child();

            child.Parent = this;
            Children.Add(child);
            return child;
        }
    }

    public class Child {
        public virtual string Id { get; set; }
        public virtual string ChildProp1 { get; set; }
        public virtual Parent Parent { get; set; }
    }
}