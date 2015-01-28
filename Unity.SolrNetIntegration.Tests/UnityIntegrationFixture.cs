// 

using System;
using System.Collections.Generic;
using System.Configuration;
using MbUnit.Framework;
using Microsoft.Practices.Unity;
using SolrNet;
using Unity.SolrNetIntegration.Config;

namespace Unity.SolrNetIntegration.Tests {
    [TestFixture]
    [Category("Integration")]
    public class UnityIntegrationFixture {
        internal static readonly SolrServers TestServers = new SolrServers {
            new SolrServerElement {
                Id = "entity",
                DocumentType = typeof (Entity).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/entity",
            },
            new SolrServerElement {
                Id = "entity2Dict",
                DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/entity2",
            },
            new SolrServerElement {
                Id = "entity2",
                DocumentType = typeof (Entity2).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/entity2",
            },
        };

        [Test]
        public void Ping_And_Query()
        {
            using (var container = UnityFixture.SetupContainer())
            {
                var solr = container.Resolve<ISolrOperations<Entity>>();
                solr.Ping();
                Console.WriteLine(solr.Query(SolrQuery.All).Count);
            }
        }

        [Test]
        public void DictionaryDocument()
        {
            using (var container = new UnityContainer())
            {
                new SolrNetContainerConfiguration().ConfigureContainer(TestServers, container);
                var solr = container.Resolve<ISolrOperations<Entity2>>();
                var results = solr.Query(SolrQuery.All);
                Assert.GreaterThan(results.Count, 0);
            }
        }

        [Test]
        public void DictionaryDocument_add()
        {
            using (var container = new UnityContainer())
            {
                new SolrNetContainerConfiguration().ConfigureContainer(TestServers, container);

                var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>();

                solr.Add(new Dictionary<string, object> {
                    {"id", "5"},
                    {"manu", "who knows"},
                    {"popularity", 55},
                    {"timestamp", DateTime.UtcNow},
                });
            }
        }
    }
}