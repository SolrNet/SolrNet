using System;
using System.Collections.Generic;
using System.Configuration;
using Xunit;
using SolrNet;
using StructureMap.SolrNetIntegration.Config;

namespace StructureMap.SolrNetIntegration.Tests {
    
    [Trait("Category","Integration")]
    public class StructureMapIntegrationFixture {

        private readonly IContainer Container;

        public StructureMapIntegrationFixture()
        {
            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            Container = new Container (c => c.IncludeRegistry(new SolrNetRegistry(solrConfig.SolrServers)));
        }


        [Fact]
        public void Ping_And_Query()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        [Fact]
        public void DictionaryDocument()
        {

            var solr = Container.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            var results = solr.Query(SolrQuery.All);
            Assert.True (results.Count > 0);
            foreach (var d in results)
            {
                Assert.True(d.Count > 0);
                foreach (var kv in d)
                    Console.WriteLine("{0}: {1}", kv.Key, kv.Value);
            }
        }

        [Fact]
        public void DictionaryDocument_add()
        {

            var solr = Container.GetInstance<ISolrOperations<Dictionary<string, object>>>();

            solr.Add(new Dictionary<string, object> 
            {
                {"id", "ababa"},
                {"manu", "who knows"},
                {"popularity", 55},
                {"timestamp", DateTime.UtcNow},
            });
        }

       
    }
}