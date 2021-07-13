using Xunit;
using SolrNet.Cloud.CollectionsAdmin;
using SolrNet.Cloud.ZooKeeperClient;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using System.Threading.Tasks;

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
                    solrCollectionsAdmin.CreateCollection(collectionName, numShards: 1);
                }
            }            
        }

        private bool containerPrepared = false;
        async Task PrepareContainerAsync()
        {
            if (containerPrepared)
                return;
            
            var collectionNames = new[] { "data", "hosts" };
            PrepareCollections(collectionNames);            

            await Startup.InitAsync<FakeEntity>(new SolrCloudStateProvider("localhost:9983"), collectionNames[0], true);
            await Startup.InitAsync<FakeEntity1>(new SolrCloudStateProvider("localhost:9983"), collectionNames[1]);
            
            containerPrepared = true;
        }

        [Fact(Skip = "Fails, need to look into it")]
        public async Task ShouldResolveBasicOperationsFromStartupContainer()
        {
            await PrepareContainerAsync();
            Assert.NotNull(Startup.Container.GetInstance<ISolrBasicOperations<FakeEntity>>());

            var a = Startup.Container.GetInstance<ISolrOperations<FakeEntity>>();
            var b = Startup.Container.GetInstance<ISolrOperations<FakeEntity1>>();

            a.Query("test", new QueryOptions() { Rows = 10 });
            b.Query("test1", new QueryOptions() { Rows = 10 });
        }

        [Fact]
        public async Task ShouldResolveBasicReadOnlyOperationsFromStartupContainer()
        {
            await PrepareContainerAsync();

            Assert.NotNull(Startup.Container.GetInstance<ISolrBasicReadOnlyOperations<FakeEntity>>());
        }


        [Fact]
        public async Task ShouldResolveOperationsFromStartupContainer() {

            await PrepareContainerAsync();
            Assert.NotNull(Startup.Container.GetInstance<ISolrOperations<FakeEntity>>());
        }

        [Fact]
        public async Task ShouldResolveOperationsProviderFromStartupContainer()
        {
            await PrepareContainerAsync();
            Assert.NotNull(Startup.Container.GetInstance<ISolrOperationsProvider>());
        }

        [Fact]
        public async Task ShouldResolveReadOnlyOperationsFromStartupContainer()
        {
            await PrepareContainerAsync();
            Assert.NotNull(Startup.Container.GetInstance<ISolrReadOnlyOperations<FakeEntity>>());
        }
    }
}
