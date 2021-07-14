using System.Collections.Generic;
using Xunit;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Configuration;
using StructureMap.SolrNetIntegration.Config;
using System.Linq;

namespace StructureMap.SolrNetIntegration.Tests
{

    public class StructureMapFixture
    {
        IContainer Container;
        public StructureMapFixture()
        {
            var servers = new List<SolrServer>
            {
                new SolrServer ("entity","http://localhost:8983/solr/techproducts/collection1", "StructureMap.SolrNetIntegration.Tests.Entity, StructureMap.SolrNetIntegration.Tests"),
                new SolrServer ("entity2","http://localhost:8983/solr/techproducts/core0", "StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests"),
                new SolrServer ("entity3","http://localhost:8983/solr/techproducts/core1", "StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests")
            };
            Container = new Container(c => c.IncludeRegistry(SolrNetRegistry.Create(servers)));
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
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var solrConfig = (SolrConfigurationSection)config.GetSection("solr");
            var container = new Container(c => c.IncludeRegistry(SolrNetRegistry.Create(solrConfig.SolrServers)));

            var instanceKey = "entity" + typeof(SolrConnection);

            var solrConnection = (AutoSolrConnection)container.GetInstance<ISolrConnection>(instanceKey);

            Assert.Equal("http://localhost:8983/solr/techproducts/collection1", solrConnection.ServerURL);
        }


        [Fact]
        public void CheckParseAppConfig()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var solrConfig = (SolrConfigurationSection)config.GetSection("solr");
            var servers = solrConfig.SolrServers;

            Assert.Equal("entity", servers.First().Id);
            Assert.Equal("http://localhost:8983/solr/techproducts/collection1", servers.First().Url);
            Assert.Equal("StructureMap.SolrNetIntegration.Tests.Entity, StructureMap.SolrNetIntegration.Tests", servers.First().DocumentType);

            Assert.Equal(3, servers.Count);
            Assert.Equal("entity3", servers.Last().Id);
            Assert.Equal("http://localhost:8983/solr/techproducts/core1", servers.Last().Url);
            Assert.Equal("StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests", servers.Last().DocumentType);

        }

        [Fact]
        public void RegistersSolrConnectionWithCoresJsonServerUrl()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent("../../../").FullName)
               .AddJsonFile("cores.json")
               .Build()
               .GetSection("solr:servers");

            var container = new Container(c => c.IncludeRegistry(SolrNetRegistry.Create(configuration.Get<List<SolrServer>>())));


            var instanceKey = "entity" + typeof(SolrConnection);

            var solrConnection = (AutoSolrConnection)container.GetInstance<ISolrConnection>(instanceKey);

            Assert.Equal("http://localhost:8983/solr/techproducts/collection1", solrConnection.ServerURL);
        }


        [Fact]
        public void CheckParseJsonConfiguration()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent("../../../").FullName)
               .AddJsonFile("cores.json")
               .Build()
               .GetSection("solr:servers");

            var servers = configuration.Get<List<SolrServer>>();

            Assert.Equal(3, servers.Count);
            Assert.Equal("entity", servers.First().Id);
            Assert.Equal("http://localhost:8983/solr/techproducts/collection1", servers.First().Url);
            Assert.Equal("StructureMap.SolrNetIntegration.Tests.Entity, StructureMap.SolrNetIntegration.Tests", servers.First().DocumentType);

            Assert.Equal("entity3", servers.Last().Id);
            Assert.Equal("http://localhost:8983/solr/techproducts/core1", servers.Last().Url);
            Assert.Equal("StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests", servers.Last().DocumentType);

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
            Assert.Throws<InvalidURLException>(() => new Container(c => c.IncludeRegistry(SolrNetRegistry.Create(solrServers))));
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

            Assert.Throws<InvalidURLException>(() => new Container(c => c.IncludeRegistry(SolrNetRegistry.Create(solrServers))));
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
                    Url = "http://localhost:8983/solr/techproducts/entity1",
                },
                new SolrServerElement {
                    Id = "entity1dict",
                    DocumentType = typeof(Dictionary<string, object>).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/techproducts/entity1",
                },
                new SolrServerElement {
                    Id = "another",
                    DocumentType = typeof(Entity2).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/techproducts/entity2",
                },
            };
            var container = new Container(c => c.IncludeRegistry(SolrNetRegistry.Create(cores)));
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
