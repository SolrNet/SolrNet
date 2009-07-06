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

using System.Collections.Generic;
using MbUnit.Framework;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using Rhino.Mocks;
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class ListenersTests {
        private ISessionFactory sessionFactory;
        private ISolrOperations<Entity> mockSolr;

        [Test]
        public void PostInsert_manual_flush() {
            var entity = new Entity {Description = "pepe"};
            mockSolr.Expect(x => x.Add(entity))
                .Repeat.Once()
                .Return(mockSolr);
            mockSolr.Replay();
            using (var session = sessionFactory.OpenSession()) {
                session.FlushMode = FlushMode.Never;
                session.Save(entity);
                session.Flush();
            }
            mockSolr.VerifyAllExpectations();
        }

        [Test]
        public void PostInsert_manual_flush_without_flush() {
            var entity = new Entity {Description = "pepe"};
            mockSolr.Expect(x => x.Add(entity))
                .Repeat.Never()
                .Return(mockSolr);
            mockSolr.Replay();
            using (var session = sessionFactory.OpenSession()) {
                session.FlushMode = FlushMode.Never;
                session.Save(entity);
            }
            mockSolr.VerifyAllExpectations();
        }

        [Test]
        public void PostInsert_autoflush_without_flush() {
            var entity = new Entity {Description = "pepe"};
            mockSolr.Expect(x => x.Add(entity))
                .Repeat.Once()
                .Return(mockSolr);
            mockSolr.Replay();
            using (var session = sessionFactory.OpenSession()) {
                session.Save(entity);
            }
            mockSolr.VerifyAllExpectations();
        }

        [Test]
        public void PostInsert_without_commit() {
            var entity = new Entity();
            mockSolr.Expect(x => x.Add(entity))
                .Repeat.Never()
                .Return(mockSolr);
            mockSolr.Replay();
            using (var session = sessionFactory.OpenSession()) {
                using (var tx = session.BeginTransaction()) {
                    session.Save(entity);
                    tx.Rollback();
                }
            }
            mockSolr.VerifyAllExpectations();
        }

        [Test]
        public void tt() {
            using (var session = new SolrSession(sessionFactory.OpenSession())) {}
        }

        [SetUp]
        public void FixtureSetup() {
            var nhConfig = new Configuration {
                Properties = new Dictionary<string, string> {
                    {Environment.ConnectionProvider, "NHibernate.Connection.DriverConnectionProvider"},
                    {Environment.ConnectionDriver, "NHibernate.Driver.SQLite20Driver"},
                    {Environment.Dialect, "NHibernate.Dialect.SQLiteDialect"},
                    {Environment.ConnectionString, "Data Source=test.db;Version=3;New=True;"},
                }
            };
            nhConfig.Register(typeof (Entity));
            mockSolr = MockRepository.GenerateMock<ISolrOperations<Entity>>();
            nhConfig.SetListener(new SolrNetListener<Entity>(mockSolr));
            new SchemaExport(nhConfig).Execute(false, true, false, false);
            sessionFactory = nhConfig.BuildSessionFactory();
        }

        [TearDown]
        public void FixtureTeardown() {
            sessionFactory.Dispose();
        }
    }
}