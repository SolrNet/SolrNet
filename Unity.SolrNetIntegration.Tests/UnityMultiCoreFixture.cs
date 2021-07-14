using System.Configuration;
using Xunit;
using SolrNet;
using Unity.SolrNetIntegration.Config;
using System;

namespace Unity.SolrNetIntegration.Tests {
    public class UnityMultiCoreFixture : IDisposable {
        private IUnityContainer container;

        
        public  UnityMultiCoreFixture() {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var solrConfig = (SolrConfigurationSection) config.GetSection("solr");

            container = new UnityContainer();
            new SolrNetContainerConfiguration().ConfigureContainer(solrConfig.SolrServers, container);
        }

        [Fact]
        public void Get_SolrOperations_for_Entity() {
            var solrOperations = container.Resolve<ISolrOperations<Entity>>();
            Assert.NotNull(solrOperations);
        }

        [Fact]
        public void Get_SolrOperations_for_Entity2() {
            var solrOperations2 = container.Resolve<ISolrOperations<Entity2>>();
            Assert.NotNull(solrOperations2);
        }

        [Fact]
        public void Get_named_SolrOperations_for_Entity() {
            var solrOperations = container.Resolve<ISolrOperations<Entity>>("entity");
            Assert.NotNull(solrOperations);
        }

        [Fact]
        public void Get_named_SolrOperations_for_Entity2() {
            var solrOperations2 = container.Resolve<ISolrOperations<Entity2>>("entity2");
            Assert.NotNull(solrOperations2);
        }

        [Fact]
        public void Same_document_type_different_core_url() {
            var cores = new SolrServers {
                new SolrServerElement {
                    Id = "core1",
                    DocumentType = typeof (Entity).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/techproducts/entity1",
                },
                new SolrServerElement {
                    Id = "core2",
                    DocumentType = typeof (Entity).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/techproducts/entity2",
                }
            };

            container = new UnityContainer();
            new SolrNetContainerConfiguration().ConfigureContainer(cores, container);
            var core1 = container.Resolve<ISolrOperations<Entity>>("core1");
            var core2 = container.Resolve<ISolrOperations<Entity>>("core2");
        }

        public void Dispose()
        {
            container.Dispose();
        }
    }
}
