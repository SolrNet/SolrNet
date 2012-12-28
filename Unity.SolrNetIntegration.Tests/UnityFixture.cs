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
    public class UnityFixture {
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
                var m = container.ResolveAll(typeof(ISolrAbstractResponseParser<UnityFixture>));
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

        internal static IUnityContainer SetupContainer() {
            var solrConfig = (SolrConfigurationSection) ConfigurationManager.GetSection("solr");
            var container = new UnityContainer();
            new SolrNetContainerConfiguration().ConfigureContainer(solrConfig.SolrServers, container);
            return container;
        }
    }
}