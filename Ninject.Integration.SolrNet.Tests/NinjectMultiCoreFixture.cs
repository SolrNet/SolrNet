﻿#region license
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

using Moroco;
using Xunit;
using Ninject.Integration.SolrNet.Config;
using SolrNet;
using System.Configuration;
using SolrNet.Exceptions;
using SolrNet.Tests.Mocks;

namespace Ninject.Integration.SolrNet.Tests
{

    public class NinjectMultiCoreFixture {
        private StandardKernel kernel;
        public NinjectMultiCoreFixture()
        {
            kernel = new StandardKernel();
        }

        [Fact]
        public void ResolveSolrOperations() {
            //var kernel = new StandardKernel();

            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "default",
                    Url = "http://localhost:8983/solr/techproducts",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr = kernel.Get<ISolrOperations<Entity>>();
            Assert.NotNull(solr);
        }

        [Fact]
        public void Resolve_MultiCore_FromConfig()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            
            var solrConfig = (SolrConfigurationSection)config.GetSection("solr");
            kernel.Load(new SolrNetModule(solrConfig.SolrServers));

            var solrOperations = kernel.Get<ISolrOperations<Entity>>();
            Assert.NotNull(solrOperations);

            var solrOperations2 = kernel.Get<ISolrOperations<Entity2>>();
            Assert.NotNull(solrOperations2);
        }

        [Fact]
        public void MultiCore_SameClassBinding()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "core-0",
                    Url = "http://localhost:8983/solr/techproducts/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                },
                new SolrServerElement {
                    Id = "core-1",
                    Url = "http://localhost:8983/solr/techproducts/core1",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr1 = kernel.Get<ISolrOperations<Entity>>("core-0");
            Assert.NotNull(solr1);
            var solr2 = kernel.Get<ISolrOperations<Entity>>("core-1");
            Assert.NotNull(solr2);
        }

        [Fact]
        public void MultiCore_Rebind_IConnection()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "core-0",
                    Url = "http://localhost:8983/solr/techproducts/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                },
                new SolrServerElement {
                    Id = "core-1",
                    Url = "http://localhost:8983/solr/techproducts/core1",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));

            const string Response = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
";

            var solr1 = kernel.Get<ISolrOperations<Entity>>("core-0");
            Assert.NotNull(solr1);

            Assert.Throws<SolrConnectionException>(() => solr1.Query("SomeField:Value"));

            MSolrConnection conn = new MSolrConnection();
            conn.get &= x => x.Return(Response);
            kernel.Rebind<ISolrConnection>().ToConstant(conn).WithMetadata("CoreId", "core-0");
            kernel.Rebind<ISolrConnection>().ToConstant(conn).WithMetadata("CoreId", "core-1");

            var solr2 = kernel.Get<ISolrOperations<Entity>>("core-1");
            Assert.NotNull(solr2);

            var r = solr2.Query("SomeField:Value");
            Assert.Equal(1, r.NumFound);
        }
    }
}
