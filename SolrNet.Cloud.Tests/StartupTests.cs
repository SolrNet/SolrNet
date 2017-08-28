using Xunit;
using SolrNet.Cloud.CollectionsAdmin;
using SolrNet.Cloud.ZooKeeperClient;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;

namespace SolrNet.Cloud.Tests
{
    [Trait("Category", "Integration")]
    public class StartupTests
    {
        private void PrepareCollections(string[] collectionNames)
        {
            const string solrUrl = "http://localhost:8983/solr";
            
            //var headerParser = ServiceLocator.Current.GetInstance<ISolrHeaderResponseParser>();
            var headerParser = new HeaderResponseParser();
            ISolrCollectionsAdmin solrCollectionsAdmin = new SolrCollectionsAdmin(new SolrConnection(solrUrl), headerParser);
            var collections = solrCollectionsAdmin.ListCollections();
            foreach (var collectionName in collectionNames)
            {
                if (!collections.Contains(collectionName))
                {
                    solrCollectionsAdmin.CreateCollection(collectionName, numShards: 2);
                }
            }            
        }

        public StartupTests()
        {
            var collectionNames = new[] { "data", "hosts" };
            PrepareCollections(collectionNames);            

            Startup.Init<FakeEntity>(new SolrCloudStateProvider("127.0.0.1:9983"), collectionNames[0], true);
            Startup.Init<FakeEntity1>(new SolrCloudStateProvider("127.0.0.1:9983"), collectionNames[1]);
        }

        [Fact]
        public void ShouldResolveBasicOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrBasicOperations<FakeEntity>>());

            var a = Startup.Container.GetInstance<ISolrOperations<FakeEntity>>();
            var b = Startup.Container.GetInstance<ISolrOperations<FakeEntity1>>();

            a.Query("test", new QueryOptions() { Rows = 10 });
            b.Query("test1", new QueryOptions() { Rows = 10 });
        }

        [Fact]
        public void ShouldResolveBasicReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrBasicReadOnlyOperations<FakeEntity>>());
        }


        [Fact]
        public void ShouldResolveOperationsFromStartupContainer() {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrOperations<FakeEntity>>());
        }

        [Fact]
        public void ShouldResolveOperationsProviderFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrOperationsProvider>());
        }

        [Fact]
        public void ShouldResolveReadOnlyOperationsFromStartupContainer()
        {
            Assert.NotNull(
                Startup.Container.GetInstance<ISolrReadOnlyOperations<FakeEntity>>());
        }
    }
}