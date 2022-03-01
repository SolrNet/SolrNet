using System;
using System.Collections.Generic;
using SolrNet;
using Xunit;
#if NETCOREAPP2_0 || NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using System.IO;
#else
using System.Configuration;
#endif

using StructureMap.SolrNetIntegration.Config;
using Xunit.Abstractions;

namespace StructureMap.SolrNetIntegration.Tests {
    
    [Trait("Category","Integration")]
    public class StructureMapIntegrationFixture {
        private readonly ITestOutputHelper testOutputHelper;

        private readonly IContainer Container;

        public StructureMapIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            var servers = new List<SolrServer>
            {
                new SolrServer ("entity","http://localhost:8983/solr/entity1", "StructureMap.SolrNetIntegration.Tests.Entity, StructureMap.SolrNetIntegration.Tests"),
                new SolrServer ("entity2","http://localhost:8983/solr/core0", "StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests"),
                new SolrServer ("entity3","http://localhost:8983/solr/core1", "StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests")
            };
            Container = new Container(c => c.IncludeRegistry(SolrNetRegistry.Create(servers)));
        }

        [Fact]
        public void Ping_And_Query()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

        [Fact]
        public void DictionaryDocument()
        {
            var cores = new SolrServers {
                new SolrServerElement {
                    Id = "entity1dict",
                    DocumentType = typeof(Dictionary<string, object>).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/core1",
                }
            };

            Container.Configure(c => c.AddRegistry(SolrNetRegistry.Create(cores)));

            var solr = Container.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            var results = solr.Query(SolrQuery.All);
            Assert.True (results.Count > 0);
            foreach (var d in results)
            {
                Assert.True(d.Count > 0);
                foreach (var kv in d)
                    testOutputHelper.WriteLine("{0}: {1}", kv.Key, kv.Value);
            }
        }

        [Fact]
        public void DictionaryDocument_add()
        {
            var cores = new SolrServers {
                new SolrServerElement {
                    Id = "entity1dict",
                    DocumentType = typeof(Dictionary<string, object>).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/core1",
                }
            };

            Container.Configure(c=>c.AddRegistry(SolrNetRegistry.Create(cores)));

            var solr = Container.GetInstance<ISolrOperations<Dictionary<string, object>>>();

            solr.Add(new Dictionary<string, object> 
            {
                {"id", "ababa"},
                {"manu", "who knows"},
                {"popularity", 55}
            });
        }
    }
}
