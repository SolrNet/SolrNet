using System.Collections.Generic;
using Xunit;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
#if NETCOREAPP2_0 || NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using System.IO;
#else
using System.Configuration;
#endif
using StructureMap.SolrNetIntegration.Config;

namespace StructureMap.SolrNetIntegration.Tests
{

    public class StructureMapFixture
    {
        private readonly IContainer Container;

        public StructureMapFixture()
        {
#if NETCOREAPP2_0 || NETSTANDARD2_0
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent("../../../").FullName)
                .AddJsonFile("cores.json")
                .Build()
                .GetSection("solr");

            Container = new Container(c => c.IncludeRegistry(new SolrNetRegistry(configuration.Get<SolrServers>())));
#else
            var solrConfig = (SolrConfigurationSection)ConfigurationManager.GetSection("solr");
            Container = new Container(c => c.IncludeRegistry(new SolrNetRegistry(solrConfig.SolrServers)));
#endif
        }

        [Fact]
        public void ResolveSolrOperations()
        {
            var m = Container.GetInstance<ISolrOperations<Entity>>();
            Assert.NotNull(m);
        }

        [Fact]
        public void RegistersSolrConnectionWithAppConfigServerUrl()
        {
            var instanceKey = "entity" + typeof(SolrConnection);

            var solrConnection = (SolrConnection)Container.GetInstance<ISolrConnection>(instanceKey);

            Assert.Equal("http://localhost:8983/solr/collection1", solrConnection.ServerURL);
        }

        [Fact]
        public void Should_throw_exception_for_invalid_protocol_on_url()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "test",
                    Url = "htp://localhost:8893",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                }
            };
            Assert.Throws<InvalidURLException>(() => new Container(c => c.IncludeRegistry(new SolrNetRegistry(solrServers))));
        }

        [Fact]
        public void Should_throw_exception_for_invalid_url()
        {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "test",
                    Url = "http:/localhost:8893",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                }
            };

            Assert.Throws<InvalidURLException>(() => new Container(c => c.IncludeRegistry(new SolrNetRegistry(solrServers))));
        }

        [Fact]
        public void Container_has_ISolrFieldParser()
        {

            var parser = Container.GetInstance<ISolrFieldParser>();
            Assert.NotNull(parser);
        }

        [Fact]
        public void Container_has_ISolrFieldSerializer()
        {
            Container.GetInstance<ISolrFieldSerializer>();
        }

        [Fact]
        public void Container_has_ISolrDocumentPropertyVisitor()
        {
            Container.GetInstance<ISolrDocumentPropertyVisitor>();
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

        [Fact]
        public void DictionaryDocument_ResponseParser()
        {
            var parser = Container.GetInstance<ISolrDocumentResponseParser<Dictionary<string, object>>>();
            Assert.IsType<SolrDictionaryDocumentResponseParser>(parser);
        }

        [Fact]
        public void DictionaryDocument_Serializer()
        {
            var serializer = Container.GetInstance<ISolrDocumentSerializer<Dictionary<string, object>>>();
            Assert.IsType<SolrDictionarySerializer>(serializer);
        }

      


    }
}
