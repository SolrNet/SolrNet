using System;
using System.IO;
using System.Xml;
using MbUnit.Framework;
using NHibernate.Event;
using NHibernate.Tool.hbm2ddl;
using Rhino.Mocks;
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class Tests {
        public class SolrNetInsertListener : IPostInsertEventListener {
            private readonly IReadOnlyMappingManager mapper;

            public SolrNetInsertListener(IReadOnlyMappingManager mapper) {
                this.mapper = mapper;
            }

            public void OnPostInsert(PostInsertEvent e) {
                throw new NotImplementedException();
                //var fields = mapper.GetFields(e.Entity.GetType());
                //if (fields == null || fields.Count == 0)
                //    return;
            }
        }

        [Test]
        public void PostInsert() {
            var nhConfig = new Configuration();
            nhConfig.Configure(
                new XmlTextReader(
                    new StringReader(
                        @"<hibernate-configuration xmlns=""urn:nhibernate-configuration-2.2"">
<session-factory name=""LessThanFew"">
<property name=""connection.provider"">NHibernate.Connection.DriverConnectionProvider</property>
<property name=""connection.driver_class"">NHibernate.Driver.SQLite20Driver</property>
<property name=""dialect"">NHibernate.Dialect.SQLiteDialect</property>
<property name=""connection.connection_string"">Data Source=test.db;Version=3;New=True;</property>
</session-factory>
</hibernate-configuration>")));
            nhConfig.Register(typeof (Entity));
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            nhConfig.SetListener(ListenerType.PostInsert, new SolrNetInsertListener(mapper));
            new SchemaExport(nhConfig).Execute(false, true, false, false);
            using (var sessionFactory = nhConfig.BuildSessionFactory())
            using (var session = sessionFactory.OpenSession()) {
                session.Save(new Entity());
            }
        }
    }

    public class Entity {
        public virtual int Id { get; set; }
        public virtual string Description { get; set; }
    }
}