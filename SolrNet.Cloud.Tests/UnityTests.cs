using Microsoft.Practices.Unity;
using NUnit.Framework;
using Unity.SolrNetCloudIntegration;

namespace SolrNet.Cloud.Tests
{
    public class UnityTests
    {
        private IUnityContainer Setup() {
            return new SolrNetContainerConfiguration().ConfigureContainer(
                new FakeProvider(),
                new UnityContainer());
        }

        [Test]
        public void ShouldResolveBasicOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Setup().Resolve<ISolrBasicOperations<FakeEntity>>(),
                "Should resolve basic operations from unity container");
        }

        [Test]
        public void ShouldResolveBasicReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Setup().Resolve<ISolrBasicReadOnlyOperations<FakeEntity>>(),
                "Should resolve basic read only operations from unity container");
        }

        [Test]
        public void ShouldResolveOperationsFromStartupContainer() {
            Assert.NotNull(
                Setup().Resolve<ISolrOperations<FakeEntity>>(),
                "Should resolve operations from unity container");
        }

        [Test]
        public void ShouldResolveReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Setup().Resolve<ISolrReadOnlyOperations<FakeEntity>>(),
                "Should resolve read only operations from unity container");
        }
    }
}