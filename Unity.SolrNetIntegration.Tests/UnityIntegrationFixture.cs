// 

using System;
using System.Collections.Generic;
using System.Configuration;
using Xunit;
using SolrNet;
using Unity.SolrNetIntegration.Config;
using Xunit.Abstractions;

namespace Unity.SolrNetIntegration.Tests {
    [Trait("Category","Integration")]
    public class UnityIntegrationFixture {
        private readonly ITestOutputHelper testOutputHelper;

        internal static readonly SolrServers TestServers = new SolrServers {
            new SolrServerElement {
                Id = "entity",
                DocumentType = typeof (Entity).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/core0",
            },
            new SolrServerElement {
                Id = "entity2Dict",
                DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/core1",
            },
            new SolrServerElement {
                Id = "entity2",
                DocumentType = typeof (Entity2).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/entity2",
            },
        };

        public UnityIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void Ping_And_Query()
        {
            using (var container = new UnityContainer())
            {
                new SolrNetContainerConfiguration().ConfigureContainer(TestServers, container);
                var solr = container.Resolve<ISolrOperations<Entity>>();
                solr.Ping();
                testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
            }
        }

        [Fact]
        public void DictionaryDocument()
        {
            using (var container = new UnityContainer())
            {
                new SolrNetContainerConfiguration().ConfigureContainer(TestServers, container);
                var solr = container.Resolve<ISolrOperations<Entity2>>();
                var results = solr.Query(SolrQuery.All);
                Assert.True(results.Count> 0);
            }
        }

        [Fact]
        public void DictionaryDocument_add()
        {
            using (var container = new UnityContainer())
            {
                new SolrNetContainerConfiguration().ConfigureContainer(TestServers, container);

                var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>();

                solr.Add(new Dictionary<string, object> {
                    {"id", "5"},
                    {"manu", "who knows"},
                    {"popularity", 55}
                });
            }
        }
    }
}
