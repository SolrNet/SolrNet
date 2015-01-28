using System.Configuration;
using MbUnit.Framework;
using Microsoft.Practices.Unity;
using SolrNet;
using Unity.SolrNetIntegration.Config;

namespace Unity.SolrNetIntegration.Tests {
    [TestFixture]
    public class UnityMultiCoreFixture {
        private IUnityContainer container;

        [SetUp]
        public void SetUp() {
            var solrConfig = (SolrConfigurationSection) ConfigurationManager.GetSection("solr");

            container = new UnityContainer();
            new SolrNetContainerConfiguration().ConfigureContainer(solrConfig.SolrServers, container);
        }

        [TearDown]
        public void Teardown() {
            container.Dispose();
        }

        [Test]
        public void Get_SolrOperations_for_Entity() {
            var solrOperations = container.Resolve<ISolrOperations<Entity>>();
            Assert.IsNotNull(solrOperations);
        }

        [Test]
        public void Get_SolrOperations_for_Entity2() {
            var solrOperations2 = container.Resolve<ISolrOperations<Entity2>>();
            Assert.IsNotNull(solrOperations2);
        }

        [Test]
        public void Get_named_SolrOperations_for_Entity() {
            var solrOperations = container.Resolve<ISolrOperations<Entity>>("entity");
            Assert.IsNotNull(solrOperations);
        }

        [Test]
        public void Get_named_SolrOperations_for_Entity2() {
            var solrOperations2 = container.Resolve<ISolrOperations<Entity2>>("entity2");
            Assert.IsNotNull(solrOperations2);
        }

        [Test]
        public void Same_document_type_different_core_url() {
            var cores = new SolrServers {
                new SolrServerElement {
                    Id = "core1",
                    DocumentType = typeof (Entity).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/entity1",
                },
                new SolrServerElement {
                    Id = "core2",
                    DocumentType = typeof (Entity).AssemblyQualifiedName,
                    Url = "http://localhost:8983/solr/entity2",
                }
            };

            container = new UnityContainer();
            new SolrNetContainerConfiguration().ConfigureContainer(cores, container);
            var core1 = container.Resolve<ISolrOperations<Entity>>("core1");
            var core2 = container.Resolve<ISolrOperations<Entity>>("core2");
        }
    }
}