#if NET462
using System;
using System.Collections.Generic;
using System.Configuration;
using SolrNet;
using StructureMap.SolrNetIntegration.Config;
using Xunit;

namespace StructureMap.SolrNetIntegration.Tests
{
    [Trait("Category", "Integration")]
    public class System_Configuration_Cores
    {
        private readonly IContainer Container;

        public System_Configuration_Cores()
        {
            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            Container = new Container(c => c.IncludeRegistry(new SolrNetRegistry(solrConfig.SolrServers)));
        }

        [Fact]
        public void Ping_And_Query()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }
        [Fact]
        public void DictionaryDocument_and_multi_core()
        {
            var cores = new SolrServers {
                new SolrServerElement {
                    Id = "default",
                    DocumentType = typeof(Entity).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/entity1",
                },
                new SolrServerElement {
                    Id = "entity1dict",
                    DocumentType = typeof(Dictionary<string, object>).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/entity1",
                },
                new SolrServerElement {
                    Id = "another",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/entity2",
                },
            };
            var container = new Container(c => c.IncludeRegistry(new SolrNetRegistry(cores)));
            var solr1 = container.GetInstance<ISolrOperations<Entity>>();
            var solr2 = container.GetInstance<ISolrOperations<Entity2>>();
            var solrDict = container.GetInstance<ISolrOperations<Dictionary<string, object>>>();
        }
    }
}
#endif

