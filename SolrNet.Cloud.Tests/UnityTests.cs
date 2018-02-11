using Xunit;
using Unity.SolrNetCloudIntegration;
using Unity;

namespace SolrNet.Cloud.Tests
{
    public class UnityTests
    {
        private IUnityContainer Setup() {
            return new SolrNetContainerConfiguration().ConfigureContainer(
                new FakeProvider(),
                new UnityContainer());
        }

        [Fact]
        public void ShouldResolveBasicOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Setup().Resolve<ISolrBasicOperations<FakeEntity>>());
        }

        [Fact]
        public void ShouldResolveBasicReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Setup().Resolve<ISolrBasicReadOnlyOperations<FakeEntity>>());
        }

        [Fact]
        public void ShouldResolveOperationsFromStartupContainer() {
            Assert.NotNull(
                Setup().Resolve<ISolrOperations<FakeEntity>>());
        }

        [Fact]
        public void ShouldResolveReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Setup().Resolve<ISolrReadOnlyOperations<FakeEntity>>());
        }

        [Fact]
        public void ShouldResolveIOperations ()
        {
            using (var container = new UnityContainer())
            {
                var cont = new Unity.SolrNetCloudIntegration.SolrNetContainerConfiguration().ConfigureContainer(new FakeProvider(), container);
                var obj = cont.Resolve<ISolrOperations<Camera>>();
            
            }
        }

        public class Camera
        {
            [Attributes.SolrField("Name")]
            public string Name { get; set; }

            [Attributes.SolrField("UID")]
            public int UID { get; set; }

            [Attributes.SolrField("id")]
            public string Id { get; set; }
        }
    }
}
