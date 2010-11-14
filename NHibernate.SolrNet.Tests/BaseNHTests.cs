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
using System.ComponentModel.Design;
using MbUnit.Framework;
using NHibernate.SolrNet.Impl;
using NHibernate.Tool.hbm2ddl;
using Rhino.Mocks;
using SolrNet;

namespace NHibernate.SolrNet.Tests {
    public class BaseNHTests {

        protected ISessionFactory sessionFactory;
        protected ISolrOperations<Entity> mockSolr;

        [SetUp]
        public void FixtureSetup() {
            var nhConfig = ConfigurationExtensions.GetNhConfig();
            mockSolr = MockRepository.GenerateMock<ISolrOperations<Entity>>();
            var provider = MockRepository.GenerateMock<IServiceProvider>();
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            provider.Expect(x => x.GetService(typeof (IReadOnlyMappingManager))).Return(mapper);
            NHHelper.SetListener(nhConfig, new SolrNetListener<Entity>(mockSolr));
            new SchemaExport(nhConfig).Execute(false, true, false);
            sessionFactory = nhConfig.BuildSessionFactory();
        }

        [TearDown]
        public void FixtureTeardown() {
            sessionFactory.Dispose();
        }
    }
}