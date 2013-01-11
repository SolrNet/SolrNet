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

using System.Configuration;
using Autofac;
using AutofacContrib.SolrNet.Config;
using MbUnit.Framework;
using SolrNet;
using SolrNet.Impl;
using System.Collections.Generic;

namespace AutofacContrib.SolrNet.Tests
{
    [TestFixture]
    public class AutofacMulticoreFixture
    {
        [Test]
        public void ResolveSolrOperations()
        {
            // Arrange 
            var builder = new ContainerBuilder();
            var cores = new SolrServers {
                                new SolrServerElement {
                                        Id = "entity1",
                                        DocumentType = typeof (Entity1).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreEntity1",
                                    },
                            };

            builder.RegisterModule(new SolrNetModule(cores));
            var container = builder.Build();

            // Act
            var m = container.Resolve<ISolrOperations<Entity1>>();

            // Assert
            Assert.IsTrue(m is SolrServer<Entity1>);
        }

        [Test]
        public void ResolveSolrReadOnlyOperations()
        {
            // Arrange 
            var builder = new ContainerBuilder();
            var cores = new SolrServers {
                                new SolrServerElement {
                                        Id = "entity1",
                                        DocumentType = typeof (Entity1).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreEntity1",
                                    },
                            };

            builder.RegisterModule(new SolrNetModule(cores));
            var container = builder.Build();

            // Act
            var solrReadOnlyOperations = container.Resolve<ISolrReadOnlyOperations<Entity1>>();

            // Assert
            Assert.IsTrue(solrReadOnlyOperations is SolrServer<Entity1>);
        }

        [Test]
        public void ResolveSolrOperations_withMultiCore()
        {
            // Arrange 
            var builder = new ContainerBuilder();
            var cores = new SolrServers {
                                new SolrServerElement {
                                        Id = "entity1",
                                        DocumentType = typeof (Entity1).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreEntity1",
                                    },
                               new SolrServerElement {
                                        Id = "entity2",
                                        DocumentType = typeof (Entity2).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreEntity2",
                                },
                            };

            builder.RegisterModule(new SolrNetModule(cores));
            var container = builder.Build();

            // Act
            var solrOperations1 = container.Resolve<ISolrOperations<Entity1>>();
            var solrOperations2 = container.Resolve<ISolrOperations<Entity2>>();

            // Assert
            Assert.IsTrue(solrOperations1 is SolrServer<Entity1>);
            Assert.IsTrue(solrOperations2 is SolrServer<Entity2>);
        }

        [Test]
        public void ResolveSolrOperations_viaNamedWithMultiCore()
        {
            // Arrange 
            var builder = new ContainerBuilder();
            var cores = new SolrServers {
                                new SolrServerElement {
                                        Id = "entity1",
                                        DocumentType = typeof (Entity1).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreEntity1",
                                    },
                               new SolrServerElement {
                                        Id = "entity2",
                                        DocumentType = typeof (Entity2).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreEntity2",
                                },
                            };

            builder.RegisterModule(new SolrNetModule(cores));
            var container = builder.Build();

            // Act
            var solrOperations1 = container.ResolveNamed<ISolrOperations<Entity1>>("entity1");
            var solrOperations2 = container.ResolveNamed<ISolrOperations<Entity2>>("entity2");

            // Assert
            Assert.IsTrue(solrOperations1 is SolrServer<Entity1>);
            Assert.IsTrue(solrOperations2 is SolrServer<Entity2>);
        }

        [Test]
        public void ResolveSolrOperations_viaNamedWithMultiCoreForDictionary()
        {
            // Arrange 
            var builder = new ContainerBuilder();
            var cores = new SolrServers {
                                new SolrServerElement {
                                        Id = "dictionary1",
                                        DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreDictionaryEntity1",
                                    },
                               new SolrServerElement {
                                        Id = "dictionary2",
                                        DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreDictionaryEntity2",
                                },
                            };

            builder.RegisterModule(new SolrNetModule(cores));
            var container = builder.Build();

            // Act
            var solrOperations1 = container.ResolveNamed<ISolrOperations<Dictionary<string, object>>>("dictionary1");
            var solrOperations2 = container.ResolveNamed<ISolrOperations<Dictionary<string, object>>>("dictionary2");

            // Assert
            Assert.IsTrue(solrOperations1 is SolrServer<Dictionary<string, object>>);
            Assert.IsTrue(solrOperations2 is SolrServer<Dictionary<string, object>>);
        }

        [Test]
        public void ResolveSolrReadOnlyOperations_viaNamedWithMultiCoreForDictionary()
        {
            // Arrange 
            var builder = new ContainerBuilder();
            var cores = new SolrServers {
                                new SolrServerElement {
                                        Id = "dictionary1",
                                        DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreDictionaryEntity1",
                                    },
                               new SolrServerElement {
                                        Id = "dictionary2",
                                        DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
                                        Url = "http://localhost:8983/solr/coreDictionaryEntity2",
                                },
                            };

            builder.RegisterModule(new SolrNetModule(cores));
            var container = builder.Build();

            // Act
            var solrReadOnlyOperations1 = container.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>("dictionary1");
            var solrReadOnlyOperations2 = container.ResolveNamed<ISolrReadOnlyOperations<Dictionary<string, object>>>("dictionary2");

            // Assert
            Assert.IsTrue(solrReadOnlyOperations1 is SolrServer<Dictionary<string, object>>);
            Assert.IsTrue(solrReadOnlyOperations2 is SolrServer<Dictionary<string, object>>);
        }

        [Test]
        public void ResolveSolrOperations_fromConfigSection()
        {
            // Arrange 
            var builder = new ContainerBuilder();

            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            builder.RegisterModule(new SolrNetModule(solrConfig.SolrServers));

            var container = builder.Build();

            // Act
            var m = container.Resolve<ISolrOperations<Entity1>>();

            // Assert
            Assert.IsTrue(m is SolrServer<Entity1>);
        }
    }

    public class Entity1 { }
    public class Entity2 { }
}