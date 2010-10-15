using System;
using System.Collections.Generic;
using System.Configuration;
using MbUnit.Framework;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using System.Reflection;
using StructureMap.SolrNetIntegration.Config;

namespace StructureMap.SolrNetIntegration.Tests
{
    [TestFixture]
    public class RegistryTests
    {                
        [Test]
        public void ResolveSolrOperations()
        {
            SetupContainer();
           var m = ObjectFactory.GetInstance<ISolrOperations<Entity>>();
            Assert.IsNotNull(m);            
        }

        [Test]
        public void RegistersSolrConnectionWithAppConfigServerUrl() {
            SetupContainer();
            var instanceKey = "entity" + typeof(SolrConnection);

            var solrConnection = (SolrConnection)ObjectFactory.Container.GetInstance<ISolrConnection>(instanceKey);

            Assert.AreEqual("http://localhost:8983/solr/entity", solrConnection.ServerURL);
        }

        [Test, Category("Integration")]
        public void Ping_And_Query()
        {
            SetupContainer();
            var solr = ObjectFactory.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        [Test, ExpectedException(typeof(InvalidURLException))]
        public void Should_throw_exception_for_invalid_protocol_on_url()
        {
            var solrServers = new TestSolrServers();
            var solrServerElement = new SolrServerElement 
            {
                Id = "test",
                Url = "htp://localhost:8893",
                DocumentType = typeof(Entity2).AssemblyQualifiedName,
            };
            solrServers.Add(solrServerElement);
            ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(solrServers)));
            ObjectFactory.GetInstance<SolrConnection>();
        }

        [Test, ExpectedException(typeof(InvalidURLException))]
        public void Should_throw_exception_for_invalid_url()
        {
            var solrServers = new TestSolrServers();
            var solrServerElement = new SolrServerElement 
            {
                Id = "test",
                Url = "http:/localhost:8893",
                DocumentType = typeof(Entity2).AssemblyQualifiedName,
            };
            solrServers.Add(solrServerElement);
            ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(solrServers)));
            ObjectFactory.GetInstance<SolrConnection>();
        }

        [Test]
        public void Container_has_ISolrFieldParser()
        {
            SetupContainer();
            var parser = ObjectFactory.GetInstance<ISolrFieldParser>();
            Assert.IsNotNull(parser);
        }

        [Test]
        public void Container_has_ISolrFieldSerializer()
        {
           SetupContainer();
            ObjectFactory.GetInstance<ISolrFieldSerializer>();
        }

        [Test]
        public void Container_has_ISolrDocumentPropertyVisitor()
        {
            SetupContainer();
            ObjectFactory.GetInstance<ISolrDocumentPropertyVisitor>();
        }

        [Test]
        public void ResponseParsers()
        {
            SetupContainer();

            var parser = ObjectFactory.GetInstance<ISolrQueryResultParser<Entity>>() as SolrQueryResultParser<Entity>;

            var field = parser.GetType().GetField("parsers", BindingFlags.NonPublic | BindingFlags.Instance);
            var parsers = (ISolrResponseParser<Entity>[])field.GetValue(parser);
            Assert.AreEqual(8, parsers.Length);
            foreach (var t in parsers)
                Console.WriteLine(t);
        }

        [Test, Category("Integration")]
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

        [Test, Category("Integration")]
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

        [Test]
        public void DictionaryDocument_ResponseParser()
        {
            SetupContainer();

            var parser = ObjectFactory.GetInstance<ISolrDocumentResponseParser<Dictionary<string, object>>>();
            Assert.IsInstanceOfType<SolrDictionaryDocumentResponseParser>(parser);
        }

        [Test]
        public void DictionaryDocument_Serializer()
        {
            SetupContainer();
            var serializer = ObjectFactory.GetInstance<ISolrDocumentSerializer<Dictionary<string, object>>>();
            Assert.IsInstanceOfType<SolrDictionarySerializer>(serializer);
        }

        private static void SetupContainer()
        {
            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(solrConfig.SolrServers)));
        }

        private class TestSolrServers : SolrServers
        {
            public void Add(ConfigurationElement configurationElement) 
            {
                base.BaseAdd(configurationElement);
            }
        }
    }
}
