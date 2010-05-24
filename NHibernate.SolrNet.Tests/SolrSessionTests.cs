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

using System.Linq;
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using Rhino.Mocks;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class SolrSessionTests: BaseNHTests {

        public delegate ISolrQueryResults<Entity> SQuery(string q, QueryOptions options);

        [Test]
        public void QueryAll() {
            var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
            serviceLocator.Expect(x => x.GetService(typeof (ISolrReadOnlyOperations<Entity>))).Return(mockSolr);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            mockSolr.Expect(x => x.Query("*:*", new QueryOptions()))
                .IgnoreArguments()
                .Do(new SQuery((s, o) => {
                    Assert.AreEqual("*:*", s);
                    return new SolrQueryResults<Entity>();
                }))
                ;
            using (var session = new SolrSession(sessionFactory.OpenSession())) {
                var entities = session.CreateSolrQuery("*:*").List<Entity>();
            }
        }

        [Test]
        public void QueryAll_with_pagination() {
            var serviceLocator = MockRepository.GenerateMock<IServiceLocator>();
            serviceLocator.Expect(x => x.GetService(typeof (ISolrReadOnlyOperations<Entity>))).Return(mockSolr);
            var querySerializer = MockRepository.GenerateMock<ISolrQuerySerializer>();
            querySerializer.Expect(x => x.Serialize(null))
                .IgnoreArguments()
                .Return("id:123456");
            serviceLocator.Expect(x => x.GetService(typeof(ISolrQuerySerializer))).Return(querySerializer);
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            mockSolr.Expect(x => x.Query("", new QueryOptions()))
                .IgnoreArguments()
                .Do(new SQuery((s, o) => {
                    Assert.AreEqual("id:123456", s);
                    Assert.AreEqual(5, o.Rows);
                    Assert.AreEqual(3, o.Start);
                    Assert.AreEqual(1, o.OrderBy.Count);
                    Assert.AreEqual("pepe asc", o.OrderBy.First().ToString());
                    return new SolrQueryResults<Entity>();
                }))
                ;
            using (var session = new SolrSession(sessionFactory.OpenSession())) {
                var entities = session.CreateSolrQuery(new SolrQueryByField("id", "123456"))
                    .SetMaxResults(5)
                    .SetFirstResult(3)
                    .SetSort(Criterion.Order.Asc("pepe"))
                    .List<Entity>();
            }
        }


    }
}