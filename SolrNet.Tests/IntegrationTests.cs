using NUnit.Framework;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class IntegrationTests {
        [Test]
        public void SwappingMappingManager() {
            var mapper = new MappingManager();
            var container = new Container(Startup.Container);
            container.Remove<IReadOnlyMappingManager>();
            container.Register<IReadOnlyMappingManager>(c => mapper);
            Factory.Init(container);
            Startup.Init<Document>("http://localhost");
            var mapperFromFactory = Factory.Get<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        [Test]
        public void SwappingMappingManager2() {
            var mapper = new MappingManager();
            Startup.Container.Remove<IReadOnlyMappingManager>();
            Startup.Container.Register<IReadOnlyMappingManager>(c => mapper);
            Startup.Init<Document>("http://localhost");
            var mapperFromFactory = Factory.Get<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        public class Document {}
    }
}