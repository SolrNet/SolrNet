using System;
using System.Collections.Generic;
using System.Configuration;
using MbUnit.Framework;
using Microsoft.Practices.Unity;
using SolrNet;
using SolrNet.Exceptions;
using SolrNet.Impl;
using Unity.SolrNetIntegration.Config;

namespace Unity.SolrNetIntegration.Tests {
    [TestFixture]
    public class RegistryTests {
        private static readonly SolrServers testServers = new SolrServers {
            new SolrServerElement {
                Id = "entity",
                DocumentType = typeof (Entity).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/entity",
            },
            new SolrServerElement {
                Id = "entity2Dict",
                DocumentType = typeof (Dictionary<string, object>).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/entity2",
            },
            new SolrServerElement {
                Id = "entity2",
                DocumentType = typeof (Entity2).AssemblyQualifiedName,
                Url = "http://localhost:8983/solr/entity2",
            },
        };

        [Test]
        public void ResolveSolrOperations() {
            using (var container = SetupContainer()) {
                var m = container.Resolve<ISolrOperations<Entity>>();
                Assert.IsNotNull(m);
            }
        }
        [Test]
        public void ResolveAllISolrAbstractResponseParser()
        {
            using (var container = SetupContainer()) {
                var m = container.ResolveAll(typeof(ISolrAbstractResponseParser<RegistryTests>));
                Assert.IsNotEmpty(m);
            }
        }

        [Test]
        public void RegistersSolrConnectionWithAppConfigServerUrl() {
            using (var container = SetupContainer()) {
                var instanceKey = "entity" + typeof (SolrConnection);

                var solrConnection = (SolrConnection) container.Resolve<ISolrConnection>(instanceKey);

                Assert.AreEqual("http://localhost:8983/solr/entity", solrConnection.ServerURL);
            }
        }

        [Test, Category("Integration")]
        public void Ping_And_Query() {
            using (var container = SetupContainer()) {
                var solr = container.Resolve<ISolrOperations<Entity>>();
                solr.Ping();
                Console.WriteLine(solr.Query(SolrQuery.All).Count);
            }
        }

        [Test, ExpectedException(typeof (InvalidURLException))]
        public void Should_throw_exception_for_invalid_protocol_on_url() {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "test",
                    Url = "htp://localhost:8893",
                    DocumentType = typeof (Entity2).AssemblyQualifiedName,
                }
            };
            using (var container = new UnityContainer()) {
                new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container);
                container.Resolve<ISolrConnection>();
            }
        }

        [Test, ExpectedException(typeof (InvalidURLException))]
        public void Should_throw_exception_for_invalid_url() {
            var solrServers = new SolrServers {
                new SolrServerElement {
                    Id = "test",
                    Url = "http:/localhost:8893",
                    DocumentType = typeof (Entity2).AssemblyQualifiedName,
                }
            };
            using (var container = new UnityContainer()) {
                new SolrNetContainerConfiguration().ConfigureContainer(solrServers, container);
                container.Resolve<ISolrConnection>();
            }
        }

        [Test]
        public void Container_has_ISolrFieldParser() {
            using (var container = SetupContainer()) {
                var parser = container.Resolve<ISolrFieldParser>();
                Assert.IsNotNull(parser);
            }
        }

        [Test]
        public void Container_has_ISolrFieldSerializer() {
            using (var container = SetupContainer()) {
                container.Resolve<ISolrFieldSerializer>();
            }
        }

        [Test]
        public void Container_has_ISolrDocumentPropertyVisitor() {
            using (var container = SetupContainer()) {
                container.Resolve<ISolrDocumentPropertyVisitor>();
            }
        }

        [Test, Category("Integration")]
        public void DictionaryDocument() {
            using (var container = new UnityContainer()) {
                new SolrNetContainerConfiguration().ConfigureContainer(testServers, container);
                var solr = container.Resolve<ISolrOperations<Entity2>>();
                var results = solr.Query(SolrQuery.All);
                Assert.GreaterThan(results.Count, 0);
            }
        }

        [Test, Category("Integration")]
        public void DictionaryDocument_add() {
            using (var container = new UnityContainer()) {
                new SolrNetContainerConfiguration().ConfigureContainer(testServers, container);

                var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>();

                solr.Add(new Dictionary<string, object> {
                    {"id", "5"},
                    {"manu", "who knows"},
                    {"popularity", 55},
                    {"timestamp", DateTime.UtcNow},
                });
            }
        }

        [Test]
        public void DictionaryDocument_ResponseParser() {
            using (var container = SetupContainer()) {
                var parser = container.Resolve<ISolrDocumentResponseParser<Dictionary<string, object>>>();
                Assert.IsInstanceOfType<SolrDictionaryDocumentResponseParser>(parser);
            }
        }

        [Test]
        public void DictionaryDocument_Serializer() {
            using (var container = SetupContainer()) {
                var serializer = container.Resolve<ISolrDocumentSerializer<Dictionary<string, object>>>();
                Assert.IsInstanceOfType<SolrDictionarySerializer>(serializer);
            }
        }

        private static IUnityContainer SetupContainer() {
            var solrConfig = (SolrConfigurationSection) ConfigurationManager.GetSection("solr");
            var container = new UnityContainer();
            new SolrNetContainerConfiguration().ConfigureContainer(solrConfig.SolrServers, container);
            return container;
        }
    }
}