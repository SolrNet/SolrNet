﻿#region license
// Copyright (c) 2007-2011 Mauricio Scheffer
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
using Moroco;
using NHibernate.SolrNet.Impl;
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    [TestFixture]
    public class ListenersWithAddParametersTests : BaseNHTests {
        readonly AddParameters addParameters = new AddParameters { CommitWithin = 4343 };

        protected override SolrNetListener<Entity> GetSolrNetListener(ISolrOperations<Entity> solr) {
            return new SolrNetListener<Entity>(solr) {AddParameters = addParameters};
        }

        [Test]
        public void Add_includes_parameters_when_configured() {
            var entity = new Entity { Description = "pepe" };
            mockSolr.addDocParams += (doc, param) => {
                Assert.AreSame(entity, doc);
                Assert.AreSame(addParameters, param);
                return null;
            };
            mockSolr.addDocParams &= x => x.Expect(1);
            using (var session = sessionFactory.OpenSession()) {
                session.FlushMode = FlushMode.Never;
                session.Save(entity);
                session.Flush();
            }
        }
    }
}
