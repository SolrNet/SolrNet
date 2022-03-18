using System;
using System.Linq;
using System.Reflection;
using Xunit;
using SolrNet.Cloud.CollectionsAdmin;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Cloud.ZooKeeperClient;
using System.Threading;
using System.Threading.Tasks;
using SolrNet.Tests.Integration;
using Xunit.Abstractions;

namespace SolrNet.Cloud.Tests
{
    [Trait("Category", "Integration")]
    [TestCaseOrderer(MethodDefTestCaseOrderer.Type, MethodDefTestCaseOrderer.Assembly)]
    public class CollectionsAdminTests : IAsyncLifetime
    {
        private const string COLLECTION_NAME = "test";
        private const string CONFIG_NAME = "data";
        private readonly string[] SHARD_NAMES = new string[] { "shard1", "shard2" };
        private const string ROUTER_NAME = "implicit";
        private const int NUM_SHARDS = 1;
        private const string PROPERTY_NAME = "preferredLeader";
        private const string NODE_ROLE_NAME = "overseer";
        private const string CLUSTER_PROPERTY_NAME = "autoAddReplicas";
        private const string COLLECTION_ALIAS = "test_alias";

        private string SOLR_CONNECTION_URL = "http://localhost:8983/solr";
        private SolrConnection solrconnection;

        private SolrCollectionsAdmin collections;

        private const string ZOOKEEPER_CONNECTION = "localhost:9983";
        private const int ZOOKEEPER_REFRESH_PERIOD_MSEC = 2000;
        private ISolrCloudStateProvider solrCloudStateProvider = null;

        public CollectionsAdminTests(ITestOutputHelper output)
        {
            // https://github.com/xunit/xunit/issues/416#issuecomment-378512739
            var type = output.GetType();
            var testMember = type.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
            var test = (ITest)testMember.GetValue(output);
            Console.WriteLine("Starting " + test.DisplayName);
        }
    
        [Fact]
        public async Task ReloadCollection()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);

            var res = collections.ReloadCollection(COLLECTION_NAME);
            Assert.Equal(expected: 0, actual: res.Status);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);
        }

        [Fact]
        public async Task CreateRemoveCollectionExplicitRouter()
        {

            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var state = await GetFreshCloudStateAsync();
            Assert.NotNull(state);
            Assert.True(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));

            var res = collections.CreateCollection(COLLECTION_NAME, numShards: NUM_SHARDS);
            Assert.Equal(expected: 0, actual: res.Status);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var collectionState = await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);
            Assert.NotNull(collectionState.Shards);
            Assert.Equal(expected: NUM_SHARDS, actual: collectionState.Shards.Count);

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.Equal(expected: 0, actual: res.Status);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            state = await GetFreshCloudStateAsync();
            Assert.NotNull(state);
            Assert.True(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));
        }

        [Fact]
        public async Task CreateRemoveCollectionImplicitRouter()
        {

            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shardNamesString = string.Join(",", SHARD_NAMES);
            var res = collections.CreateCollection(COLLECTION_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
            Assert.Equal(expected: 0, actual: res.Status);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var collectionState = await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);
            Assert.NotNull(collectionState.Router);
            Assert.Equal(expected: ROUTER_NAME, actual: collectionState.Router.Name);
            Assert.NotNull(collectionState.Shards);
            Assert.Equal(expected: SHARD_NAMES.Length, actual: collectionState.Shards.Count);
            foreach (var shardName in SHARD_NAMES)
            {
                Assert.Contains(collectionState.Shards.Select(x => x.Key), x => x == shardName);
            }

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.Equal(expected: 0, actual: res.Status);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var state = await GetFreshCloudStateAsync();
            Assert.NotNull(state);
            Assert.True(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));
        }

        [Fact]
        public async Task AddRemoveShard()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            try
            {
                var shardNamesString = string.Join(",", SHARD_NAMES);
                var res = collections.CreateCollection(COLLECTION_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
                Assert.Equal(expected: 0, actual: res.Status);

                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                var collectionState = await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);
                Assert.NotNull(collectionState.Router);
                Assert.Equal(expected: ROUTER_NAME, actual: collectionState.Router.Name);
                Assert.NotNull(collectionState.Shards);
                Assert.Equal(expected: SHARD_NAMES.Length, actual: collectionState.Shards.Count);
                foreach (var shardName in SHARD_NAMES)
                {
                    Assert.Contains(collectionState.Shards.Select(x => x.Key), x => x == shardName);
                }

                collections.CreateShard(COLLECTION_NAME, "shard3");
                // Assert shard is created, check via cluster state
                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                collectionState = await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);
                Assert.NotNull(collectionState.Shards);
                Assert.Contains(collectionState.Shards.Select(x => x.Key), x => x == "shard3");

                collections.DeleteShard(COLLECTION_NAME, "shard3");

                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                collectionState = await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);
                Assert.NotNull(collectionState.Shards);
                Assert.False(collectionState.Shards.ContainsKey("shard3"));
            }
            finally
            {
                var res = collections.DeleteCollection(COLLECTION_NAME);
                Assert.Equal(expected: 0, actual: res.Status);
            }
        }

        [Fact]
        public async Task ModifyCollection()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            await AssertAddCollectionAndGetFirstShardAsync(COLLECTION_NAME);
            var response = collections.ModifyCollection(COLLECTION_NAME, maxShardsPerNode: 3, replicationFactor: 2, autoAddReplicas: true);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact(Skip = "causes internal server error (one shard, two shards - doesn't matter)")]
        public async Task SplitShard()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shard_names = new[] { "shard1" };
            var shard = await AssertAddCollectionAndGetFirstShardAsync(COLLECTION_NAME, shard_names);
            var response = collections.SplitShard(COLLECTION_NAME, shard.Name);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact(Skip = "Fails, need to look into it")]
        public void CreateAlias()
        {
            AssertCreateAlias();
        }

        [Fact(Skip = "Fails, need to look into it")]
        public void DeleteAlias()
        {
            AssertCreateAlias();
            var response = collections.DeleteAlias(COLLECTION_ALIAS);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact]
        public async Task AddReplica()
        {
            await AssertAddReplicaAsync();
        }

        [Fact]
        public async Task DeleteReplica()
        {
            var shard = await AssertAddReplicaAsync();
            var replica = shard.Replicas.Values.Last();
            var response = collections.DeleteReplica(COLLECTION_NAME, shard: shard.Name, replica: replica.Name);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact]
        public void ClusterPropertySetDelete()
        {
            var response = collections.ClusterPropertySetDelete(CLUSTER_PROPERTY_NAME, "true");
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);

            response = collections.ClusterPropertySetDelete(CLUSTER_PROPERTY_NAME);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact]
        public void Migrate()
        {
            RemoveCollectionIfExists(collections, "test1");
            RemoveCollectionIfExists(collections, "test2");
            var response_collection1 = collections.CreateCollection("test1", numShards: 1);
            var response_collection2 = collections.CreateCollection("test2", numShards: 1);
            var response = collections.Migrate("test1", "test2", "a!");
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact]
        public async Task AddRole()
        {
            await AssertAddRoleAsync();
        }

        [Fact]
        public async Task RemoveRole()
        {
            var node = await AssertAddRoleAsync();
            var response = collections.RemoveRole(NODE_ROLE_NAME, node);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact]
        public async Task AddReplicaProperty()
        {
            await AssertAddReplicaPropertyAsync();
        }

        [Fact]
        public async Task DeleteReplicaProperty()
        {
            var shardReplicaNames = await AssertAddReplicaPropertyAsync();
            var response = collections.DeleteReplicaProperty(COLLECTION_NAME, shard: shardReplicaNames.Item1, replica: shardReplicaNames.Item2, property: PROPERTY_NAME);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact]
        public void BalanceShardUnique()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.BalanceShardUnique(COLLECTION_NAME, PROPERTY_NAME);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact]
        public void RebalanceLeaders()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.RebalanceLeaders(COLLECTION_NAME);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        [Fact(Skip = "Fails, need to look into it")]
        public void OverseerStatus()
        {
            var overseerStatus = collections.OverseerStatus();
            Assert.NotNull(overseerStatus);
            Assert.NotNull(overseerStatus.ResponseHeader);
            Assert.True(overseerStatus.ResponseHeader.QTime > 0);
            Assert.False(string.IsNullOrWhiteSpace(overseerStatus.Leader));
            Assert.NotNull(overseerStatus.CollectionOperations);
            Assert.NotNull(overseerStatus.CollectionQueue);
            Assert.NotNull(overseerStatus.OverseerInternalQueue);
            Assert.NotNull(overseerStatus.OverseerOperations);
            Assert.NotNull(overseerStatus.OverseerQueue);

            var operation = overseerStatus.OverseerQueue.Values.FirstOrDefault();
            Assert.NotNull(operation);
        }

        private void CreateCollectionIfNotExists(ISolrCollectionsAdmin solr, string collectionName, string routerName = ROUTER_NAME, string[] shard_names = null, int replicationFactor = 1)
        {
            var list = solr.ListCollections();
            if (!list.Contains(collectionName))
            {
                var finalShardNames = shard_names ?? SHARD_NAMES;
                var shardNamesString = string.Join(",", finalShardNames);
                solr.CreateCollection(collectionName, routerName: routerName, shards: shardNamesString, replicationFactor: replicationFactor, maxShardsPerNode: finalShardNames.Length + 1);
            }
        }

        private void RemoveCollectionIfExists(ISolrCollectionsAdmin solr, string colName)
        {
            var list = solr.ListCollections();
            if (list.Contains(colName))
            {
                solr.DeleteCollection(colName);
            }
        }

        private Task<SolrCloudState> GetFreshCloudStateAsync()
        {
            return solrCloudStateProvider.GetFreshCloudStateAsync();
        }

        private async Task<SolrCloudCollection> AssertCollectionPresenceByCloudStateAsync(string collectionName)
        {
            //var state = solrCloudStateProvider.GetCloudState();
            var state = await GetFreshCloudStateAsync();

            Assert.NotNull(state);
            Assert.NotNull(state.Collections);
            Assert.Contains(state.Collections.Select(x => x.Key).ToArray(), x => x == collectionName);
            var collectionState = state.Collections[collectionName];
            Assert.NotNull(collectionState);
            Assert.Equal(expected: collectionName, actual: collectionState.Name);
            return collectionState;
        }

        private async Task<SolrCloudShard> AssertAddCollectionAndGetFirstShardAsync(string collectionName = COLLECTION_NAME, string[] shard_names = null)
        {
            var finalShardNames = shard_names ?? SHARD_NAMES;
            Assert.NotNull(finalShardNames);
            Assert.InRange(finalShardNames.Length, low: 1, high: int.MaxValue);
            CreateCollectionIfNotExists(collections, collectionName, shard_names: finalShardNames);
            var collectionState = await AssertCollectionPresenceByCloudStateAsync(collectionName);
            Assert.NotNull(collectionState.Shards);
            Assert.Contains(collectionState.Shards.Select(x => x.Key), x => x == finalShardNames[0]);
            var shard = collectionState.Shards[finalShardNames[0]];
            Assert.NotNull(shard);
            Assert.NotNull(shard.Replicas);
            Assert.InRange(shard.Replicas.Count, low: 1, high: int.MaxValue);
            return shard;
        }

        private async Task<Tuple<string, string>> AssertAddReplicaPropertyAsync()
        {
            var shard = await AssertAddCollectionAndGetFirstShardAsync();
            var replica = shard.Replicas.Values.First();
            Assert.NotNull(replica);
            var response = collections.AddReplicaProperty(COLLECTION_NAME, shard: shard.Name, replica: replica.Name, property: PROPERTY_NAME, propertyValue: "true");
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
            return new Tuple<string, string>(shard.Name, replica.Name);
        }

        private async Task<string> AssertAddRoleAsync()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var collectionState = await AssertCollectionPresenceByCloudStateAsync(COLLECTION_NAME);
            var node = collectionState.Shards.Values.First().Replicas.Values.First().Url;
            var response = collections.AddRole(NODE_ROLE_NAME, node);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
            return node;
        }

        private async Task<SolrCloudShard> AssertAddReplicaAsync()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shard = await AssertAddCollectionAndGetFirstShardAsync();
            var response = collections.AddReplica(COLLECTION_NAME, shard: shard.Name);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
            return shard;
        }

        private void AssertCreateAlias()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.CreateAlias(COLLECTION_NAME, COLLECTION_ALIAS);
            Assert.NotNull(response);
            Assert.Equal(expected: 0, actual: response.Status);
        }

        public async  Task InitializeAsync()
        {
            solrconnection = new SolrConnection(SOLR_CONNECTION_URL);
            collections = new SolrCollectionsAdmin(solrconnection, new HeaderResponseParser<string>());

            var solrCloud = new SolrCloudStateProvider(ZOOKEEPER_CONNECTION);
            await Startup.InitAsync<string>(solrCloud, COLLECTION_NAME, true);
            solrCloudStateProvider = Startup.Container.GetInstance<ISolrCloudStateProvider>(solrCloud.Key);
        }

        public async Task DisposeAsync()
        {
            await solrCloudStateProvider.DisposeAsync();
        }
    }
}
