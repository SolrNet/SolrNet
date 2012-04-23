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
using Moroco;
using NHibernate.SolrNet.Impl;
using NHibernate.Tool.hbm2ddl;
using SolrNet;
using SolrNet.Tests.Mocks;

namespace NHibernate.SolrNet.Tests {
    public class BaseNHTests {

        protected ISessionFactory sessionFactory;
        protected MSolrOperations<Entity> mockSolr;

        [SetUp]
        public void FixtureSetup() {
            var nhConfig = ConfigurationExtensions.GetNhConfig();
            mockSolr = new MSolrOperations<Entity>();
            var mapper = new MReadOnlyMappingManager();
            var provider = new MServiceProvider();
            provider.getService += t => {
                if (t == typeof(IReadOnlyMappingManager))
                    return mapper;
                throw new ArgumentException("Unexpected");
            };
            NHHelper.SetListener(nhConfig, GetSolrNetListener(mockSolr));
            new SchemaExport(nhConfig).Execute(false, true, false);
            sessionFactory = nhConfig.BuildSessionFactory();
        }

        protected virtual SolrNetListener<Entity> GetSolrNetListener(ISolrOperations<Entity> solr) {
            return new SolrNetListener<Entity>(solr);
        }

        [TearDown]
        public void FixtureTeardown() {
            mockSolr.VerifyAll();
            sessionFactory.Dispose();
        }
    }
}