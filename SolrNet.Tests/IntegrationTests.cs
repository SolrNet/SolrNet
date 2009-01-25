using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class IntegrationTests {
        [TearDown]
        public void Teardown() {
            Startup.Container.RemoveAll<ISolrConnection>();
            Startup.Container.RemoveAll<ISolrQueryResultParser<Document>>();
            Startup.Container.RemoveAll<ISolrQueryExecuter<Document>>();
            Startup.Container.RemoveAll<ISolrDocumentSerializer<Document>>();
            Startup.Container.RemoveAll<ISolrBasicOperations<Document>>();
            Startup.Container.RemoveAll<ISolrBasicReadOnlyOperations<Document>>();
            Startup.Container.RemoveAll<ISolrOperations<Document>>();
            Startup.Container.RemoveAll<ISolrReadOnlyOperations<Document>>();
        }

        [Test]
        public void SwappingMappingManager() {
            var mapper = new MappingManager();
            var container = new Container(Startup.Container);
            container.RemoveAll<IReadOnlyMappingManager>();
            container.Register<IReadOnlyMappingManager>(c => mapper);
            Startup.Init<Document>("http://localhost");
            ServiceLocator.SetLocatorProvider(() => container);
            var mapperFromFactory = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        [Test]
        public void SwappingMappingManager2() {
            var mapper = new MappingManager();
            Startup.Container.RemoveAll<IReadOnlyMappingManager>();
            Startup.Container.Register<IReadOnlyMappingManager>(c => mapper);
            Startup.Init<Document>("http://localhost");
            ServiceLocator.SetLocatorProvider(() => Startup.Container);
            var mapperFromFactory = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        public class Document {}
    }
}