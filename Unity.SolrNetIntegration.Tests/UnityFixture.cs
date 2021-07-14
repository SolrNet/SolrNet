using System;
using System.Collections.Generic;
using System.Configuration;
using Xunit;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using Unity.SolrNetIntegration.Config;

namespace Unity.SolrNetIntegration.Tests {
    public class UnityFixture {
        [Fact]
        public void ResolveSolrOperations() {
            using (var container = SetupContainer()) {
                var m = container.Resolve<ISolrOperations<Entity>>();
                Assert.NotNull(m);
            }
        }
        [Fact]
        public void ResolveAllISolrAbstractResponseParser()
        {
            using (var container = SetupContainer()) {
                var m = container.ResolveAll(typeof(ISolrAbstractResponseParser<UnityFixture>));
                Assert.NotEmpty(m);
            }
        }

        [Fact]
        public void RegistersSolrConnectionWithAppConfigServerUrl() {
            using (var container = SetupContainer()) {
                var instanceKey = "entity" + typeof (SolrConnection);

                var solrConnection = (SolrConnection) container.Resolve<ISolrConnection>(instanceKey);

                Assert.Equal("http://localhost:8983/solr/techproducts/collection1", solrConnection.ServerURL);
            }
        }

        [Fact]
        public void Should_throw_exception_for_invalid_protocol_on_url() {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "test",
                    Url = "htp://localhost:8893",
                    DocumentType = typeof (Entity2).AssemblyQualifiedName,
                }
            };
            using (var container = new UnityContainer()) {
                Assert.Throws<InvalidURLException>(() => new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container));
            }
        }

       [Fact]
        public void Should_throw_exception_for_invalid_url() {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "test",
                    Url = "http:/localhost:8893",
                    DocumentType = typeof (Entity2).AssemblyQualifiedName,
                }
            };
            using (var container = new UnityContainer()) {
                Assert.Throws<InvalidURLException>(() => new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container));
            }
        }

        [Fact]
        public void Container_has_ISolrFieldParser() {
            using (var container = SetupContainer()) {
                var parser = container.Resolve<ISolrFieldParser>();
                Assert.NotNull(parser);
            }
        }

        [Fact]
        public void Container_has_ISolrFieldSerializer() {
            using (var container = SetupContainer()) {
                container.Resolve<ISolrFieldSerializer>();
            }
        }

        [Fact]
        public void Container_has_ISolrDocumentPropertyVisitor() {
            using (var container = SetupContainer()) {
                container.Resolve<ISolrDocumentPropertyVisitor>();
            }
        }

        [Fact]
        public void DictionaryDocument_ResponseParser() {
            using (var container = SetupContainer()) {
                var parser = container.Resolve<ISolrDocumentResponseParser<Dictionary<string, object>>>();
                Assert.IsType<SolrDictionaryDocumentResponseParser>(parser);
            }
        }

        [Fact]
        public void DictionaryDocument_Serializer() {
            using (var container = SetupContainer()) {
                var serializer = container.Resolve<ISolrDocumentSerializer<Dictionary<string, object>>>();
                Assert.IsType<SolrDictionarySerializer>(serializer);
            }
        }

        internal static IUnityContainer SetupContainer() {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var solrConfig = (SolrConfigurationSection) config.GetSection("solr");
            var container = new UnityContainer();
            new SolrNetContainerConfiguration().ConfigureContainer(solrConfig.SolrServers, container);
            return container;
        }
    }
}
