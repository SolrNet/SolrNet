using System.IO;
using System.Linq;
using Xunit;

namespace SolrNet.Cloud.Tests {
    public class SolrClusterStateParserTests {
        private string EmptyJson {
            get { return File.ReadAllText($"resources{Path.DirectorySeparatorChar}empty.json"); }
        }

        private string NotEmptyJson {
            get { return File.ReadAllText($"resources{Path.DirectorySeparatorChar}not-empty.json"); }
        }

        private string InternalCollectionsState {
            get { return File.ReadAllText($"resources{Path.DirectorySeparatorChar}clusterstate1.json"); }
        }

        private string FirstExternalCollectionState {
            get { return File.ReadAllText($"resources{Path.DirectorySeparatorChar}externalstate1.json"); }
        }

        private string SecondExternalCollectionState {
            get { return File.ReadAllText($"resources{Path.DirectorySeparatorChar}externalstate2.json"); }
        }

        [Fact]
        public void ShouldProduceEmptyStateFromEmptyJson() {
            var state = SolrCloudStateParser.Parse(EmptyJson);
            Assert.False(state.Collections.Any());
        }

        [Fact]
        public void ShouldProduceNotEmptyStateFromNotEmptyJson() {
            var collections = SolrCloudStateParser.Parse(NotEmptyJson).Collections.Values;
            Assert.True(collections.Any());
            var shards = collections.SelectMany(collection => collection.Shards.Values).ToList();
            Assert.True(shards.Any());
            Assert.Contains(shards, shard => shard.IsActive);
            var replicas = shards.SelectMany(shard => shard.Replicas.Values).ToList();
            Assert.True(replicas.Any());
            Assert.Contains(replicas, replica => replica.IsActive);
            Assert.Contains(replicas, replica => replica.IsLeader);
        }

        [Fact]
        public void ShouldProduceMergedStateFromSeveralJson() {
            var internalCollectionsState = SolrCloudStateParser.Parse(InternalCollectionsState);
            var external1State = SolrCloudStateParser.Parse(FirstExternalCollectionState);
            var external2State = SolrCloudStateParser.Parse(SecondExternalCollectionState);

            var totalCollectionsCount = internalCollectionsState.Collections.Count + external1State.Collections.Count + external2State.Collections.Count;
            var zkState = internalCollectionsState.Merge(external1State).Merge(external2State);

            Assert.Equal(zkState.Collections.Count, totalCollectionsCount);
        }
    }
}
