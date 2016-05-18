using System;
using NUnit.Framework;
using SolrNet.Cloud.CollectionsAdmin;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;


namespace SolrNet.Cloud.Tests
{
    [TestFixture]
    public class CollectionsAdminTests
    {        
        private const string COLLECTION_NAME = "test";
        private const string CONFIG_NAME = "data";
        private const string SHARD_NAMES = "shard1,shard2";
        private const string ROUTER_NAME = "implicit";

        private SolrConnection solrconnection;
        private SolrCollectionsAdmin collections;

        [TestFixtureSetUp]
        public void Setup() {
            solrconnection = new SolrConnection("http://localhost:8983/solr");
            collections = new SolrCollectionsAdmin(solrconnection, new HeaderResponseParser<string>());
        }

        //[TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void ReloadColection() {
            CreateCollectionIfNotExists(collections, COLLECTION_NAME);
            var res = collections.ReloadCollection(COLLECTION_NAME);
            Assert.That(res.Status == 0);
        }

        [Test]
        public void CreateRemoveCollectionExcplicitRouter()
        {
            var res = collections.CreateCollection(COLLECTION_NAME, configName: CONFIG_NAME, numShards: 1);
            Assert.That(res.Status == 0);

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.That(res.Status == 0);
        }

        [Test]
        public void CreateRemoveCollectionImplicitRouter()
        {
            var res = collections.CreateCollection(COLLECTION_NAME, configName: CONFIG_NAME, routerName: ROUTER_NAME, shards: SHARD_NAMES, maxShardsPerNode:10);
            Assert.That(res.Status == 0);

            res = collections.DeleteCollection(COLLECTION_NAME);
            Assert.That(res.Status == 0);
        }

        [Test]
        public void AddRemoveShard()
        {
            RemoveCollectionIfExists(collections, COLLECTION_NAME);
            try {
                var res = collections.CreateCollection(COLLECTION_NAME, configName: CONFIG_NAME, routerName: ROUTER_NAME, shards: SHARD_NAMES, maxShardsPerNode: 10);
                Assert.That(res.Status == 0);

                collections.CreateShard(COLLECTION_NAME, "shard3");
                // Assert shard is created, check via cluster state
                collections.DeleteShard(COLLECTION_NAME, "shard3");
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
                solr.CreateCollection(collectionName, routerName: ROUTER_NAME, shards: SHARD_NAMES);
            }
        }

        private void RemoveCollectionIfExists(ISolrCollectionsAdmin solr, string colName) {
            var list = solr.ListCollections();
            if (list.Contains(colName))
            {
                solr.DeleteCollection(colName);
            }
        }
    }
}
