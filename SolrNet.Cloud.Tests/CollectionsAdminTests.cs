using System;
using System.Linq;
using Xunit;
using SolrNet.Cloud.CollectionsAdmin;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Cloud.ZooKeeperClient;
using System.Threading;

namespace SolrNet.Cloud.Tests
{
    [Trait("Category","Integration")]
    public class CollectionsAdminTests
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

        private const string ZOOKEEPER_CONNECTION = "127.0.0.1:9983";
        private const int ZOOKEEPER_REFRESH_PERIOD_MSEC = 2000;
        private ISolrCloudStateProvider solrCloudStateProvider = null;

        public  CollectionsAdminTests()
        {
            solrconnection = new SolrConnection(SOLR_CONNECTION_URL);
            collections = new SolrCollectionsAdmin(solrconnection, new HeaderResponseParser<string>());

            var solrCloud = new SolrCloudStateProvider(ZOOKEEPER_CONNECTION);
            Startup.Init<string>(solrCloud, COLLECTION_NAME, true);
            solrCloudStateProvider = Startup.Container.GetInstance<ISolrCloudStateProvider>(solrCloud.Key);
        }

        [Fact]
        public void ReloadColection()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            
            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            AssertCollectionPresenceByCloudState(COLLECTION_NAME);

            var res = collections.ReloadCollection(COLLECTION_NAME);
            Assert.True(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            AssertCollectionPresenceByCloudState(COLLECTION_NAME);
        }

        [Fact]
        public void CreateRemoveCollectionExcplicitRouter()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var state = GetFreshCloudState();
            Assert.NotNull(state);
            Assert.True(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));

            var res = collections.CreateCollection(COLLECTION_NAME, numShards: NUM_SHARDS);
            Assert.True(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);            
            var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
            Assert.NotNull(collectionState.Shards);
            Assert.True(collectionState.Shards.Count == NUM_SHARDS);

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.True(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            state = GetFreshCloudState();
            Assert.NotNull(state);
            Assert.True(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));
        }

        [Fact]
        public void CreateRemoveCollectionImplicitRouter()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shardNamesString = string.Join(",", SHARD_NAMES);
            var res = collections.CreateCollection(COLLECTION_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
            Assert.True(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);            
            Assert.NotNull(collectionState.Router);
            Assert.True(collectionState.Router.Name == ROUTER_NAME);
            Assert.NotNull(collectionState.Shards);
            Assert.True(collectionState.Shards.Count == SHARD_NAMES.Length);
            foreach (var shardName in SHARD_NAMES)
            {
                Assert.True(collectionState.Shards.ContainsKey(shardName));
            }

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.True(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var state = GetFreshCloudState();
            Assert.NotNull(state);
            Assert.True(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));
        }

        [Fact]
        public void AddRemoveShard()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            try {
                var shardNamesString = string.Join(",", SHARD_NAMES);
                var res = collections.CreateCollection(COLLECTION_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
                Assert.True(res.Status == 0);

                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
                Assert.NotNull(collectionState.Router);
                Assert.True(collectionState.Router.Name == ROUTER_NAME);
                Assert.NotNull(collectionState.Shards);
                Assert.True(collectionState.Shards.Count == SHARD_NAMES.Length);
                foreach (var shardName in SHARD_NAMES)
                {
                    Assert.True(collectionState.Shards.ContainsKey(shardName));
                }

                collections.CreateShard(COLLECTION_NAME, "shard3");
                // Assert shard is created, check via cluster state
                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
                Assert.NotNull(collectionState.Shards);
                Assert.True(collectionState.Shards.ContainsKey("shard3"));

                collections.DeleteShard(COLLECTION_NAME, "shard3");

                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
                Assert.NotNull(collectionState.Shards);
                Assert.False(collectionState.Shards.ContainsKey("shard3"));
            } catch (Exception e) {
                Assert.True(false, e.ToString());
            } finally {
                var res = collections.DeleteCollection(COLLECTION_NAME);
                Assert.True(res.Status == 0);
            }
        }

        [Fact]
        public void ModifyCollection()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            AssertAddCollectionAndGetFirstShard(COLLECTION_NAME);
            var response = collections.ModifyCollection(COLLECTION_NAME, maxShardsPerNode: 3, replicationFactor: 2, autoAddReplicas: true);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        //[Fact]
        // causes internal server error (one shard, two shards - doesn't matter)
        public void SplitShard()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shard_names = new[] { "shard1" };
            var shard = AssertAddCollectionAndGetFirstShard(COLLECTION_NAME, shard_names);
            var response = collections.SplitShard(COLLECTION_NAME, shard.Name);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        [Fact]
        public void CreateAlias()
        {
            AssertCreateAlias();
        }

        [Fact]
        public void DeleteAlias()
        {
            AssertCreateAlias();
            var response = collections.DeleteAlias(COLLECTION_ALIAS);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        [Fact]
        public void AddReplica()
        {
            AssertAddReplica();
        }

        [Fact]
        public void DeleteReplica()
        {
            var shard = AssertAddReplica();
            var replica = shard.Replicas.Values.Last();
            var response = collections.DeleteReplica(COLLECTION_NAME, shard: shard.Name, replica: replica.Name);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        [Fact]
        public void ClusterPropertySetDelete()
        {
            var response = collections.ClusterPropertySetDelete(CLUSTER_PROPERTY_NAME, "true");
            Assert.NotNull(response);
            Assert.True(response.Status == 0);

            response = collections.ClusterPropertySetDelete(CLUSTER_PROPERTY_NAME);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
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
            Assert.True(response.Status == 0);
        }

        [Fact]
        public void AddRole()
        {
            AssertAddRole();
        }

        [Fact]
        public void RemoveRole()
        {
            var node = AssertAddRole();
            var response = collections.RemoveRole(NODE_ROLE_NAME, node);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        [Fact]
        public void AddReplicaProperty()
        {
            AssertAddReplicaProperty();
        }

        [Fact]
        public void DeleteReplicaProperty()
        {
            var shardReplicaNames = AssertAddReplicaProperty();
            var response = collections.DeleteReplicaProperty(COLLECTION_NAME, shard: shardReplicaNames.Item1, replica: shardReplicaNames.Item2, property: PROPERTY_NAME);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        [Fact]
        public void BalanceShardUnique()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.BalanceShardUnique(COLLECTION_NAME, PROPERTY_NAME);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        [Fact]
        public void RebalanceLeaders()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.RebalanceLeaders(COLLECTION_NAME);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }

        [Fact]
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

        private SolrCloudState GetFreshCloudState()
        {
            return (solrCloudStateProvider as SolrCloudStateProvider).GetFreshCloudState();
        }

        private SolrCloudCollection AssertCollectionPresenceByCloudState(string collectionName)
        {
            //var state = solrCloudStateProvider.GetCloudState();
            var state = GetFreshCloudState();

            Assert.NotNull(state);
            Assert.NotNull(state.Collections);
            Assert.True(state.Collections.ContainsKey(collectionName));
            var collectionState = state.Collections[collectionName];
            Assert.NotNull(collectionState);
            Assert.True(collectionState.Name == collectionName);
            return collectionState;
        }

        private SolrCloudShard AssertAddCollectionAndGetFirstShard(string collectionName = COLLECTION_NAME, string[] shard_names = null)
        {
            var finalShardNames = shard_names ?? SHARD_NAMES;
            Assert.NotNull(finalShardNames);
            Assert.True(finalShardNames.Length > 0);
            CreateCollectionIfNotExists(collections, collectionName, shard_names: finalShardNames);
            var collectionState = AssertCollectionPresenceByCloudState(collectionName);
            Assert.NotNull(collectionState.Shards);
            Assert.True(collectionState.Shards.ContainsKey(finalShardNames[0]));
            var shard = collectionState.Shards[finalShardNames[0]];
            Assert.NotNull(shard);
            Assert.NotNull(shard.Replicas);
            Assert.True(shard.Replicas.Count > 0);
            return shard;
        }

        private Tuple<string, string> AssertAddReplicaProperty()
        {            
            var shard = AssertAddCollectionAndGetFirstShard();            
            var replica = shard.Replicas.Values.First();
            Assert.NotNull(replica);
            var response = collections.AddReplicaProperty(COLLECTION_NAME, shard: shard.Name, replica: replica.Name, property: PROPERTY_NAME, propertyValue: "true");
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
            return new Tuple<string, string>(shard.Name, replica.Name);
        }

        private string AssertAddRole()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
            var node = collectionState.Shards.Values.First().Replicas.Values.First().Url;
            var response = collections.AddRole(NODE_ROLE_NAME, node);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
            return node;
        }

        private SolrCloudShard AssertAddReplica()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shard = AssertAddCollectionAndGetFirstShard();
            var response = collections.AddReplica(COLLECTION_NAME, shard: shard.Name);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
            return shard;
        }

        private void AssertCreateAlias()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.CreateAlias(COLLECTION_NAME, COLLECTION_ALIAS);
            Assert.NotNull(response);
            Assert.True(response.Status == 0);
        }
    }
}
