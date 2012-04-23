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
using System.Linq;
using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using Moroco;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Tests.Mocks;
using Order = NHibernate.Criterion.Order;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class SolrSessionTests : BaseNHTests {
        public delegate SolrQueryResults<Entity> SQuery(string q, QueryOptions options);

        [Test]
        public void QueryAll() {
            var serviceLocator = new MServiceLocator();
            serviceLocator.getService += t => {
                if (t == typeof (ISolrReadOnlyOperations<Entity>))
                    return mockSolr;
                throw new Exception("unexpected type");
            };
            ServiceLocator.SetLocatorProvider(() => serviceLocator);
            mockSolr.queryStringOptions += (q, opt) => {
                Assert.AreEqual("*:*", q);
                return new SolrQueryResults<Entity>();
            };
            mockSolr.queryStringOptions &= x => x.Expect(1);
            using (var session = new SolrSession(sessionFactory.OpenSession())) {
                var entities = session.CreateSolrQuery("*:*").List<Entity>();
            }
        }

        [Test]
        public void QueryAll_with_pagination() {

            var querySerializer = new MSolrQuerySerializer();
            querySerializer.serialize += _ => "id:123456";

            var serviceLocator = new MServiceLocator();
            serviceLocator.getService = MServiceLocator.Table(new Dictionary<System.Type, object> {
                {typeof (ISolrReadOnlyOperations<Entity>), mockSolr},
                {typeof (ISolrQuerySerializer), querySerializer},
            });

            ServiceLocator.SetLocatorProvider(() => serviceLocator);

            mockSolr.queryStringOptions += (s, o) => {
                Assert.AreEqual("id:123456", s);
                Assert.AreEqual(5, o.Rows);
                Assert.AreEqual(3, o.Start);
                Assert.AreEqual(1, o.OrderBy.Count);
                Assert.AreEqual("pepe asc", o.OrderBy.First().ToString());
                return new SolrQueryResults<Entity>();
            };
            mockSolr.queryStringOptions &= x => x.Expect(1);

            using (var session = new SolrSession(sessionFactory.OpenSession())) {
                var entities = session.CreateSolrQuery(new SolrQueryByField("id", "123456"))
                    .SetMaxResults(5)
                    .SetFirstResult(3)
                    .SetSort(Order.Asc("pepe"))
                    .List<Entity>();
            }
        }
    }
}