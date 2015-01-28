using System;
using System.Collections.Generic;
using System.Configuration;
using MbUnit.Framework;
using SolrNet;
using StructureMap.SolrNetIntegration.Config;

namespace StructureMap.SolrNetIntegration.Tests {
    [TestFixture]
    [Category("Integration")]
    public class StructureMapIntegrationFixture {

        [Test]
        public void Ping_And_Query()
        {
            SetupContainer();
            var solr = ObjectFactory.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        [Test]
        public void DictionaryDocument()
        {
            SetupContainer();

            var solr = ObjectFactory.Container.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            var results = solr.Query(SolrQuery.All);
            Assert.GreaterThan(results.Count, 0);
            foreach (var d in results)
            {
                Assert.GreaterThan(d.Count, 0);
                foreach (var kv in d)
                    Console.WriteLine("{0}: {1}", kv.Key, kv.Value);
            }
        }

        [Test]
        public void DictionaryDocument_add()
        {
            SetupContainer();

            var solr = ObjectFactory.Container.GetInstance<ISolrOperations<Dictionary<string, object>>>();

            solr.Add(new Dictionary<string, object> 
            {
                {"id", "ababa"},
                {"manu", "who knows"},
                {"popularity", 55},
                {"timestamp", DateTime.UtcNow},
            });
        }

        private static void SetupContainer()
        {
            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(solrConfig.SolrServers)));
        }

    }
}