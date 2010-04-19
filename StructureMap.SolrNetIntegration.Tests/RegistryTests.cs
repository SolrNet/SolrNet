using System;
using System.Collections.Generic;
using System.Configuration;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using Structuremap.SolrNetIntegration;
using System.Reflection;

namespace StructureMap.SolrNetIntegration.Tests
{
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
        public void RegistersSolrConnectionWithAppConfigServerUrl()
        {
            SetupContainer();

            var expectedServerUrl = ConfigurationManager.AppSettings["SolrUrl"];

            var connection = ObjectFactory.GetInstance<ISolrConnection>();
            Assert.AreSame(connection.ServerURL, expectedServerUrl);
        }

        [Test]
        [Category("Integration")]
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
            const string solrURL = "htp://localhost:8893";
            ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(solrURL)));
            ObjectFactory.GetInstance<SolrConnection>();
        }

        [Test, ExpectedException(typeof(InvalidURLException))]
        public void Should_throw_exception_for_invalid_url()
        {
            const string solrURL = "http:/localhost:8893";
            ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(solrURL)));
            ObjectFactory.GetInstance<SolrConnection>();
        }

        [Test]
        public void ReplacingMapper()
        {
            var mapper = MockRepository.GenerateMock<IReadOnlyMappingManager>();
            var solrFacility = new SolrNetRegistry("http://localhost:8983/solr", mapper);
            ObjectFactory.Initialize(c => c.AddRegistry(solrFacility));
            var m = ObjectFactory.GetInstance<IReadOnlyMappingManager>();
            Assert.AreSame(m, mapper);
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
        public void Resolve_ISolrOperations()
        {
            SetupContainer();
            var operations = ObjectFactory.GetInstance<ISolrOperations<Entity>>();
            Assert.IsNotNull(operations);

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


        [Test]
        [Category("Integration")]
        public void DictionaryDocument()
        {
            SetupContainer();

            var solr = ObjectFactory.GetInstance<ISolrOperations<Dictionary<string, object>>>();
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
        [Category("Integration")]
        public void DictionaryDocument_add()
        {
            SetupContainer();

            var solr = ObjectFactory.GetInstance<ISolrOperations<Dictionary<string, object>>>();
            solr.Add(new Dictionary<string, object> {
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
            var solrURL = ConfigurationManager.AppSettings["solrUrl"];          
            ObjectFactory.Initialize(c => c.IncludeRegistry(new SolrNetRegistry(solrURL)));
        }
       

        public class  Entity{}

    }
    
}
