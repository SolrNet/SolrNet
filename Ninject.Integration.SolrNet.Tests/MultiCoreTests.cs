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
using Ninject.Integration.SolrNet.Config;
using SolrNet;
using System.Configuration;

namespace Ninject.Integration.SolrNet.Tests {
    [TestFixture]
    public class MultiCoreTests {
        private StandardKernel kernel;
        [SetUp]
        public void SetUp()
        {
            kernel = new StandardKernel();
        }

        [Test]
        public void ResolveSolrOperations() {
            //var kernel = new StandardKernel();

            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "default",
                    Url = "http://localhost:8983/solr",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr = kernel.Get<ISolrOperations<Entity>>();
            Assert.IsNotNull(solr);
        }

        [Test, Category("Integration")]
        public void Ping_And_Query_SingleCore()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "default",
                    Url = "http://localhost:8983/solr/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr = kernel.Get<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        [Test]
        public void Resolve_MultiCore_FromConfig()
        {
            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            kernel.Load(new SolrNetModule(solrConfig.SolrServers));

            var solrOperations = kernel.Get<ISolrOperations<Entity>>();
            Assert.IsNotNull(solrOperations);

            var solrOperations2 = kernel.Get<ISolrOperations<Entity2>>();
            Assert.IsNotNull(solrOperations2);
        }

        [Test, Category("Integration")]
        public void Ping_And_Query_MultiCore()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "main",
                    Url = "http://localhost:8983/solr/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                },
                new SolrServerElement {
                    Id = "alt",
                    Url = "http://localhost:8983/solr/core1",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr1 = kernel.Get<ISolrOperations<Entity>>();
            solr1.Ping();
            Console.WriteLine("Query core 1: {0}",solr1.Query(SolrQuery.All).Count);
            var solr2 = kernel.Get<ISolrOperations<Entity2>>();
            solr2.Ping();
            Console.WriteLine("Query core 2: {0}", solr2.Query(SolrQuery.All).Count);
        }

        [Test, Category("Integration")]
        public void MultiCore_GetByName()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "core-0",
                    Url = "http://localhost:8983/solr/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                },
                new SolrServerElement {
                    Id = "core-1",
                    Url = "http://localhost:8983/solr/core1",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr1 = kernel.Get<ISolrOperations<Entity>>("core-0");
            Assert.IsNotNull(solr1);
            Console.WriteLine("Query core 1: {0}", solr1.Query(SolrQuery.All).Count);
            var solr2 = kernel.Get<ISolrOperations<Entity2>>("core-1");
            Assert.IsNotNull(solr2);
            solr2.Ping();
            Console.WriteLine("Query core 2: {0}", solr2.Query(SolrQuery.All).Count);
        }

        [Test]
        public void MultiCore_SameClassBinding()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "core-0",
                    Url = "http://localhost:8983/solr/core0",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                },
                new SolrServerElement {
                    Id = "core-1",
                    Url = "http://localhost:8983/solr/core1",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                }
            };
            kernel.Load(new SolrNetModule(solrServers));
            var solr1 = kernel.Get<ISolrOperations<Entity>>("core-0");
            Assert.IsNotNull(solr1);
            var solr2 = kernel.Get<ISolrOperations<Entity>>("core-1");
            Assert.IsNotNull(solr2);
        }
    }
}