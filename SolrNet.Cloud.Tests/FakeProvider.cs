using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SolrNet.Cloud.Tests
{
    public class FakeProvider : ISolrCloudStateProvider, ISolrOperationsProvider
    {
        public string LastOperation { get; set; }

        public string LastUrl { get; set; }

        public string Key {
            get { return "Fake"; }
        }

        public void Dispose() {
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }

        public SolrCloudState GetCloudState() {
            var replica1 = new SolrCloudReplica(true, true, "leader1", "http://localhost:8983/solr1/collection1");
            var replica2 = new SolrCloudReplica(true, false, "replica1", "http://localhost:8983/solr2/collection1");
            var replica3 = new SolrCloudReplica(false, false, "replica2", "http://localhost:8983/solr3/collection1");
            var replica4 = new SolrCloudReplica(true, true, "leader2", "http://localhost:8983/solr2/collection2");
            var replica5 = new SolrCloudReplica(true, false, "replica3", "http://localhost:8983/solr1/collection2");
            var replica6 = new SolrCloudReplica(false, false, "replica4", "http://localhost:8983/solr3/collection2");
            var shard1 = new SolrCloudShard(true, "shard1", null, null,
                new Dictionary<string, SolrCloudReplica>(StringComparer.OrdinalIgnoreCase) {
                    {replica1.Name, replica1},
                    {replica2.Name, replica2},
                    {replica3.Name, replica3}
                });
            var shard2 = new SolrCloudShard(true, "shard2", null, null,
                new Dictionary<string, SolrCloudReplica>(StringComparer.OrdinalIgnoreCase) {
                    {replica4.Name, replica4},
                    {replica5.Name, replica5},
                    {replica6.Name, replica6}
                });
            var collection1 = new SolrCloudCollection("collection1", new SolrCloudRouter("implicit"),
                new Dictionary<string, SolrCloudShard>(StringComparer.OrdinalIgnoreCase) {
                    {shard1.Name, shard1},
                    {shard2.Name, shard2}
                });
            var collection2 = new SolrCloudCollection("collection2", new SolrCloudRouter("implicit"),
                new Dictionary<string, SolrCloudShard>(StringComparer.OrdinalIgnoreCase) {
                    {shard1.Name, shard1},
                    {shard2.Name, shard2}
                });
            return new SolrCloudState(
                new Dictionary<string, SolrCloudCollection>(StringComparer.OrdinalIgnoreCase) {
                    {collection1.Name, collection1},
                    {collection2.Name, collection2},
                });
        }

        public Task InitAsync() {
            return Task.CompletedTask;
        }

        public Task<SolrCloudState> GetFreshCloudStateAsync()
        {
            return Task.FromResult(GetCloudState());
        }

        public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false)
        {
            LastUrl = url;
            return new FakeOperations<T>(this);
        }

        public ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false)
        {
            LastUrl = url;
            return new FakeOperations<T>(this);
        }
    }
}
