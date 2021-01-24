using SolrNet.Tests.Integration;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]
[assembly: TestCollectionOrderer(AlphaTestCollectionOrderer.Type, AlphaTestCollectionOrderer.Assembly)]
