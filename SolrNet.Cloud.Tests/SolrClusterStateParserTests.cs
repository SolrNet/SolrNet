using System.IO;
using System.Linq;
using NUnit.Framework;

namespace SolrNet.Cloud.Tests {
    public class SolrClusterStateParserTests {
        private string EmptyJson {
            get { return File.ReadAllText(@"resources\empty.json"); }
        }

        private string NotEmptyJson {
            get { return File.ReadAllText(@"resources\not-empty.json"); }
        }

        private string InternalCollectionsState {
            get { return File.ReadAllText(@"resources\clusterstate1.json"); }
        }

        private string FirstExternalCollectionState {
            get { return File.ReadAllText(@"resources\externalstate1.json"); }
        }

        private string SecondExternalCollectionState {
            get { return File.ReadAllText(@"resources\externalstate2.json"); }
        }

        [Test]
        public void ShouldProduceEmptyStateFromEmptyJson() {
            var state = SolrCloudStateParser.Parse(EmptyJson);
            Assert.False(state.Collections.Any());
        }

        [Test]
        public void ShouldProduceNotEmptyStateFromNotEmptyJson() {
            var collections = SolrCloudStateParser.Parse(NotEmptyJson).Collections.Values;
            Assert.True(collections.Any());
            var shards = collections.SelectMany(collection => collection.Shards.Values).ToList();
            Assert.True(shards.Any());
            Assert.True(shards.Any(shard => shard.IsActive));
            var replicas = shards.SelectMany(shard => shard.Replicas.Values).ToList();
            Assert.True(replicas.Any());
            Assert.True(replicas.Any(replica => replica.IsActive));
            Assert.True(replicas.Any(replica => replica.IsLeader));
        }

        [Test]
        public void ShouldProduceMergedStateFromSeveralJson() {
            var internalCollectionsState = SolrCloudStateParser.Parse(InternalCollectionsState);
            var external1State = SolrCloudStateParser.Parse(FirstExternalCollectionState);
            var external2State = SolrCloudStateParser.Parse(SecondExternalCollectionState);

            var totalCollectionsCount = internalCollectionsState.Collections.Count + external1State.Collections.Count + external2State.Collections.Count;
            var zkState = internalCollectionsState.Merge(external1State).Merge(external2State);

            Assert.AreEqual(zkState.Collections.Count, totalCollectionsCount);
        }
    }
}