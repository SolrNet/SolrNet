using NUnit.Framework;
using SolrNet.Cloud.ZooKeeperClient;
using SolrNet.Commands.Parameters;

namespace SolrNet.Cloud.Tests
{
    public class StartupTests
    {
        [SetUp]
        public void Setup() {
            Startup.Init<FakeEntity>(new SolrCloudStateProvider("127.0.0.1:9983"), "data", true);
            Startup.Init<FakeEntity1>(new SolrCloudStateProvider("127.0.0.1:9983"), "hosts");
        }

        [Test]
        public void ShouldResolveBasicOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrBasicOperations<FakeEntity>>(),
                "Should resolve basic operations from startup container");

            var a = Startup.Container.GetInstance<ISolrOperations<FakeEntity>>();
            var b = Startup.Container.GetInstance<ISolrOperations<FakeEntity1>>();

            var restult = a.Query("name:test", new QueryOptions(){Rows = 10});
            b.Query("name:test1", new QueryOptions(){Rows = 10});
        }

        [Test]
        public void ShouldResolveBasicReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrBasicReadOnlyOperations<FakeEntity>>(),
                "Should resolve basic read only operations from startup container");
        }

        [Test]
        public void ShouldResolveOperationsFromStartupContainer() {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrOperations<FakeEntity>>(),
                "Should resolve operations from startup container");
        }

        [Test]
        public void ShouldResolveOperationsProviderFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrOperationsProvider>(),
                "Should resolve operations provider from startup container");
        }

        [Test]
        public void ShouldResolveReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrReadOnlyOperations<FakeEntity>>(),
                "Should resolve read only operations from startup container");
        }
    }
}