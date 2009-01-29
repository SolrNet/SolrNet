using System;
using Castle.Core.Configuration;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using CommonServiceLocator.WindsorAdapter;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SolrNet;

namespace Castle.Facilities.SolrNetIntegration.Tests {
    [TestFixture]
    public class Tests {
        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void NoConfig_throws() {
            var container = new WindsorContainer();
            container.AddFacility<SolrNetFacility>();
        }

        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void InvalidUrl_throws() {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("solr");
            configuration.CreateChild("solrURL", "123");
            configStore.AddFacilityConfiguration("solr", configuration);
            var container = new WindsorContainer(configStore);
            container.AddFacility<SolrNetFacility>("solr");
        }

        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void InvalidProtocol_throws() {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("solr");
            configuration.CreateChild("solrURL", "ftp://localhost");
            configStore.AddFacilityConfiguration("solr", configuration);
            var container = new WindsorContainer(configStore);
            container.AddFacility<SolrNetFacility>("solr");
        }

        [Test]
        public void Ping_Query() {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("solr");
            configuration.CreateChild("solrURL", "http://localhost:8983/solr");
            configStore.AddFacilityConfiguration("solr", configuration);
            var container = new WindsorContainer(configStore);
            container.AddFacility<SolrNetFacility>("solr");

            var locator = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);

            var solr = container.Resolve<ISolrOperations<Document>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        public class Document {}
    }
}