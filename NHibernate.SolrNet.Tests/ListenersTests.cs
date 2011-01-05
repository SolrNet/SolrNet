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

using MbUnit.Framework;
using Rhino.Mocks;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class ListenersTests: BaseNHTests {

        [Test]
        public void PostInsert_manual_flush_adds_to_solr() {
            var entity = new Entity {Description = "pepe"};
            using (var session = sessionFactory.OpenSession()) {
                session.FlushMode = FlushMode.Never;
                session.Save(entity);
                session.Flush();
            }
            mockSolr.AssertWasCalled(x => x.Add(entity, null), o => o.Repeat.Once().Return(null));
        }

        [Test]
        public void PostInsert_manual_flush_without_flush_doesnt_add_to_solr() {
            var entity = new Entity {Description = "pepe"};
            using (var session = sessionFactory.OpenSession()) {
                session.FlushMode = FlushMode.Never;
                session.Save(entity);
            }
            mockSolr.AssertWasNotCalled(x => x.Add(entity, null));
        }

        [Test]
        public void PostInsert_autoflush_without_flush_adds_to_solr() {
            var entity = new Entity {Description = "pepe"};
            using (var session = sessionFactory.OpenSession()) {
                session.Save(entity);
            }
            mockSolr.AssertWasCalled(x => x.Add(entity, null), o => o.Repeat.Once().Return(null));
        }

        [Test]
        public void PostInsert_without_commit_doesnt_add_to_solr() {
            var entity = new Entity();
            using (var session = sessionFactory.OpenSession()) {
                using (var tx = session.BeginTransaction()) {
                    session.Save(entity);
                    tx.Rollback();
                }
            }
            mockSolr.AssertWasNotCalled(x => x.Add(entity, null));
        }

        [Test]
        public void PostInsert_with_commit_adds_to_solr() {
            var entity = new Entity();
            using (var session = sessionFactory.OpenSession()) {
                using (var tx = session.BeginTransaction()) {
                    session.Save(entity);
                    tx.Commit();
                }
            }
            mockSolr.AssertWasCalled(x => x.Add(entity, null), o => o.Repeat.Once().Return(null));
        } 

        [Test]
        [Ignore("Session dispose should follow transaction rollback. See http://www.nhforge.org/doc/nh/en/index.html#manipulatingdata-endingsession-commit")]
        public void Insert_with_multiple_transactions() {
            var entity = new Entity();
            using (var session = sessionFactory.OpenSession()) {
                session.FlushMode = FlushMode.Commit;
                using (var tx = session.BeginTransaction()) {
                    session.Save(entity);
                    tx.Rollback();
                }
                using (var tx = session.BeginTransaction()) {
                    session.Save(entity);
                    tx.Commit();
                }
            }
            mockSolr.AssertWasCalled(x => x.Add(entity, null), o => o.Repeat.Once().Return(null));
        }

        [Test]
        public void Insert_with_multiple_transactions2() {
            var entity = new Entity();
            using (var session = sessionFactory.OpenSession()) {
                session.FlushMode = FlushMode.Commit;
                using (var tx = session.BeginTransaction()) {
                    session.Save(entity);
                    tx.Commit();
                }
                using (var tx = session.BeginTransaction()) {
                    session.Save(entity);
                    tx.Rollback();
                }
            }
            mockSolr.AssertWasCalled(x => x.Add(entity, null), o => o.Repeat.Once().Return(null));
        }

    }
}