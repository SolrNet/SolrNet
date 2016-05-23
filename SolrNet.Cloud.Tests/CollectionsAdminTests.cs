using System;
using System.Linq;
using NUnit.Framework;
using SolrNet.Cloud.CollectionsAdmin;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Cloud.ZooKeeperClient;
using System.Threading;

namespace SolrNet.Cloud.Tests
{
    [TestFixture]
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

        [TestFixtureSetUp]
        public void Setup()
        {
            solrconnection = new SolrConnection(SOLR_CONNECTION_URL);
            collections = new SolrCollectionsAdmin(solrconnection, new HeaderResponseParser<string>());

            var solrCloud = new SolrCloudStateProvider(ZOOKEEPER_CONNECTION);
            Startup.Init<string>(solrCloud, COLLECTION_NAME, true);
            solrCloudStateProvider = Startup.Container.GetInstance<ISolrCloudStateProvider>(solrCloud.Key);
        }

        //[TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void ReloadColection()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            
            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            AssertCollectionPresenceByCloudState(COLLECTION_NAME);

            var res = collections.ReloadCollection(COLLECTION_NAME);
            Assert.That(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            AssertCollectionPresenceByCloudState(COLLECTION_NAME);
        }

        [Test]
        public void CreateRemoveCollectionExcplicitRouter()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var state = GetFreshCloudState();
            Assert.IsNotNull(state);
            Assert.IsTrue(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));

            var res = collections.CreateCollection(COLLECTION_NAME, numShards: NUM_SHARDS);
            Assert.That(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);            
            var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
            Assert.IsNotNull(collectionState.Shards);
            Assert.IsTrue(collectionState.Shards.Count == NUM_SHARDS);

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.That(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            state = GetFreshCloudState();
            Assert.IsNotNull(state);
            Assert.IsTrue(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));
        }

        [Test]
        public void CreateRemoveCollectionImplicitRouter()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shardNamesString = string.Join(",", SHARD_NAMES);
            var res = collections.CreateCollection(COLLECTION_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
            Assert.That(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);            
            Assert.IsNotNull(collectionState.Router);
            Assert.IsTrue(collectionState.Router.Name == ROUTER_NAME);
            Assert.IsNotNull(collectionState.Shards);
            Assert.IsTrue(collectionState.Shards.Count == SHARD_NAMES.Length);
            foreach (var shardName in SHARD_NAMES)
            {
                Assert.IsTrue(collectionState.Shards.ContainsKey(shardName));
            }

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.That(res.Status == 0);

            Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
            var state = GetFreshCloudState();
            Assert.IsNotNull(state);
            Assert.IsTrue(state.Collections == null || !state.Collections.ContainsKey(COLLECTION_NAME));
        }

        [Test]
        public void AddRemoveShard()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            try {
                var shardNamesString = string.Join(",", SHARD_NAMES);
                var res = collections.CreateCollection(COLLECTION_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
                Assert.That(res.Status == 0);

                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
                Assert.IsNotNull(collectionState.Router);
                Assert.IsTrue(collectionState.Router.Name == ROUTER_NAME);
                Assert.IsNotNull(collectionState.Shards);
                Assert.IsTrue(collectionState.Shards.Count == SHARD_NAMES.Length);
                foreach (var shardName in SHARD_NAMES)
                {
                    Assert.IsTrue(collectionState.Shards.ContainsKey(shardName));
                }

                collections.CreateShard(COLLECTION_NAME, "shard3");
                // Assert shard is created, check via cluster state
                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
                Assert.IsNotNull(collectionState.Shards);
                Assert.IsTrue(collectionState.Shards.ContainsKey("shard3"));

                collections.DeleteShard(COLLECTION_NAME, "shard3");

                Thread.Sleep(ZOOKEEPER_REFRESH_PERIOD_MSEC);
                collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
                Assert.IsNotNull(collectionState.Shards);
                Assert.IsFalse(collectionState.Shards.ContainsKey("shard3"));
            } catch (Exception e) {
                Assert.Fail(e.ToString());
            } finally {
                var res = collections.DeleteCollection(COLLECTION_NAME);
                Assert.That(res.Status == 0);
            }
        }

        [Test]
        public void ModifyCollection()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            AssertAddCollectionAndGetFirstShard(COLLECTION_NAME);
            var response = collections.ModifyCollection(COLLECTION_NAME, maxShardsPerNode: 3, replicationFactor: 2, autoAddReplicas: true);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        //[Test]
        // causes internal server error (one shard, two shards - doesn't matter)
        public void SplitShard()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shard_names = new[] { "shard1" };
            var shard = AssertAddCollectionAndGetFirstShard(COLLECTION_NAME, shard_names);
            var response = collections.SplitShard(COLLECTION_NAME, shard.Name);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void CreateAlias()
        {
            AssertCreateAlias();
        }

        [Test]
        public void DeleteAlias()
        {
            AssertCreateAlias();
            var response = collections.DeleteAlias(COLLECTION_ALIAS);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void AddReplica()
        {
            AssertAddReplica();
        }

        [Test]
        public void DeleteReplica()
        {
            var shard = AssertAddReplica();
            var replica = shard.Replicas.Values.Last();
            var response = collections.DeleteReplica(COLLECTION_NAME, shard: shard.Name, replica: replica.Name);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void ClusterPropertySetDelete()
        {
            var response = collections.ClusterPropertySetDelete(CLUSTER_PROPERTY_NAME, "true");
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);

            response = collections.ClusterPropertySetDelete(CLUSTER_PROPERTY_NAME);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void Migrate()
        {
            RemoveCollectionIfExists(collections, "test1");
            RemoveCollectionIfExists(collections, "test2");
            var response_collection1 = collections.CreateCollection("test1", numShards: 1);
            var response_collection2 = collections.CreateCollection("test2", numShards: 1);
            var response = collections.Migrate("test1", "test2", "a!");
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void AddRole()
        {
            AssertAddRole();
        }

        [Test]
        public void RemoveRole()
        {
            var node = AssertAddRole();
            var response = collections.RemoveRole(NODE_ROLE_NAME, node);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void AddReplicaProperty()
        {
            AssertAddReplicaProperty();
        }

        [Test]
        public void DeleteReplicaProperty()
        {
            var shardReplicaNames = AssertAddReplicaProperty();
            var response = collections.DeleteReplicaProperty(COLLECTION_NAME, shard: shardReplicaNames.Item1, replica: shardReplicaNames.Item2, property: PROPERTY_NAME);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void BalanceShardUnique()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.BalanceShardUnique(COLLECTION_NAME, PROPERTY_NAME);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void RebalanceLeaders()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.RebalanceLeaders(COLLECTION_NAME);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }

        [Test]
        public void OverseerStatus()
        {
            var overseerStatus = collections.OverseerStatus();
            Assert.IsNotNull(overseerStatus);
            Assert.IsNotNull(overseerStatus.ResponseHeader);
            Assert.IsTrue(overseerStatus.ResponseHeader.QTime > 0);
            Assert.IsNotNullOrEmpty(overseerStatus.Leader);
            Assert.IsNotNull(overseerStatus.CollectionOperations);
            Assert.IsNotNull(overseerStatus.CollectionQueue);
            Assert.IsNotNull(overseerStatus.OverseerInternalQueue);
            Assert.IsNotNull(overseerStatus.OverseerOperations);
            Assert.IsNotNull(overseerStatus.OverseerQueue);

            var operation = overseerStatus.OverseerQueue.Values.FirstOrDefault();
            Assert.IsNotNull(operation);
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

            Assert.IsNotNull(state);
            Assert.IsNotNull(state.Collections);
            Assert.IsTrue(state.Collections.ContainsKey(collectionName));
            var collectionState = state.Collections[collectionName];
            Assert.IsNotNull(collectionState);
            Assert.IsTrue(collectionState.Name == collectionName);
            return collectionState;
        }

        private SolrCloudShard AssertAddCollectionAndGetFirstShard(string collectionName = COLLECTION_NAME, string[] shard_names = null)
        {
            var finalShardNames = shard_names ?? SHARD_NAMES;
            Assert.IsNotNull(finalShardNames);
            Assert.IsTrue(finalShardNames.Length > 0);
            CreateCollectionIfNotExists(collections, collectionName, shard_names: finalShardNames);
            var collectionState = AssertCollectionPresenceByCloudState(collectionName);
            Assert.IsNotNull(collectionState.Shards);
            Assert.IsTrue(collectionState.Shards.ContainsKey(finalShardNames[0]));
            var shard = collectionState.Shards[finalShardNames[0]];
            Assert.IsNotNull(shard);
            Assert.IsNotNull(shard.Replicas);
            Assert.IsTrue(shard.Replicas.Count > 0);
            return shard;
        }

        private Tuple<string, string> AssertAddReplicaProperty()
        {            
            var shard = AssertAddCollectionAndGetFirstShard();            
            var replica = shard.Replicas.Values.First();
            Assert.IsNotNull(replica);
            var response = collections.AddReplicaProperty(COLLECTION_NAME, shard: shard.Name, replica: replica.Name, property: PROPERTY_NAME, propertyValue: "true");
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
            return new Tuple<string, string>(shard.Name, replica.Name);
        }

        private string AssertAddRole()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var collectionState = AssertCollectionPresenceByCloudState(COLLECTION_NAME);
            var node = collectionState.Shards.Values.First().Replicas.Values.First().Url;
            var response = collections.AddRole(NODE_ROLE_NAME, node);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
            return node;
        }

        private SolrCloudShard AssertAddReplica()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            var shard = AssertAddCollectionAndGetFirstShard();
            var response = collections.AddReplica(COLLECTION_NAME, shard: shard.Name);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
            return shard;
        }

        private void AssertCreateAlias()
        {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var response = collections.CreateAlias(COLLECTION_NAME, COLLECTION_ALIAS);
            Assert.IsNotNull(response);
            Assert.IsTrue(response.Status == 0);
        }
    }
}
