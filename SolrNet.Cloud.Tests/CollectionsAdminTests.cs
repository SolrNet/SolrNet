using System;
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

            var res = collections.CreateCollection(COLLECTION_NAME, configName: CONFIG_NAME, numShards: NUM_SHARDS);
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
            var res = collections.CreateCollection(COLLECTION_NAME, configName: CONFIG_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
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
                var res = collections.CreateCollection(COLLECTION_NAME, configName: CONFIG_NAME, routerName: ROUTER_NAME, shards: shardNamesString, maxShardsPerNode: 10);
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

        private void CreateCollectionIfNotExists(ISolrCollectionsAdmin solr, string collectionName)
        {
            var list = solr.ListCollections();
            if (!list.Contains(collectionName))
            {
                var shardNamesString = string.Join(",", SHARD_NAMES);
                solr.CreateCollection(collectionName, routerName: ROUTER_NAME, shards: shardNamesString);
            }
        }

        private void RemoveCollectionIfExists(ISolrCollectionsAdmin solr, string colName) {
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
    }
}
