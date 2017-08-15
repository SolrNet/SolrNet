using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests
{
    public class SolrCloudOperationsTests
    {
        [Test]
        public void ShouldRedirectToProvidedOperations() {
            var provider = new FakeProvider();
            var operations = new SolrCloudOperations<FakeEntity>(provider, provider);
            operations.Commit();
            Debug.Assert(provider.LastOperation == "Commit", "Should call the provided Commit method");
        }

        [Test]
        public void ShouldEventuallyUseAllActiveReplicasForRead() {
            var provider = new FakeProvider();
            var state = provider.GetCloudState();
            var count = state.Collections.Values
                .SelectMany(collection => collection.Shards.Values)
                .SelectMany(shard => shard.Replicas.Values)
                .Where(replica => replica.IsActive)
                .Select(replica => replica.Url)
                .Distinct()
                .Count();
            var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var collection in state.Collections.Values) {
                var operations = new SolrCloudOperations<FakeEntity>(provider, provider, collection.Name);
                for (var i = 0; i < 10*count; i++) {
                    operations.Ping();
                    urls.Add(provider.LastUrl);
                }
            }
            Debug.Assert(urls.Count == count, "Should eventually use all active replicas for read");
        }

        [Test]
        public void ShouldEventuallyUseAllActiveLeadersForWrite() {
            var provider = new FakeProvider();
            var state = provider.GetCloudState();
            var count = state.Collections.Values
                .SelectMany(collection => collection.Shards.Values)
                .SelectMany(shard => shard.Replicas.Values)
                .Where(replica => replica.IsActive && replica.IsLeader)
                .Select(replica => replica.Url)
                .Distinct()
                .Count();
            var urls = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var collection in state.Collections.Values)
            {
                var operations = new SolrCloudOperations<FakeEntity>(provider, provider, collection.Name);
                for (var i = 0; i < 10 * count; i++)
                {
                    operations.Commit();
                    urls.Add(provider.LastUrl);
                }
            }
            Debug.Assert(urls.Count == count, "Should eventually use all active leaders for write");
        }
    }
}