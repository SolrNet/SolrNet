using System.Collections.Generic;
using System.Threading.Tasks;
using CommonServiceLocator;
using SolrNet.Attributes;
using SolrNet.Cloud.ZooKeeperClient;
using Xunit;
using CloudStartup = SolrNet.Cloud.Startup;

namespace SolrNet.Tests.Integration
{
    [Trait("Category", "Integration")]
    public class Test
    {
        [Fact(Skip = "TODO move to SolrNet.Cloud.Tests?")]
        public async Task TestSolrCloud()
        {
            using (var provider = new SolrCloudStateProvider("10.26.11.30:9983"))
            {
                await CloudStartup.InitAsync<TestEntity>(provider);
                TestRoutine();
            }
        }

        public void TestRoutine()
        {
            var num = 1000;

            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<TestEntity>>();

            solr.Delete(SolrQuery.All);
            solr.Commit();

            // send one by one to test sharding distribution and sending to leaders only
            foreach (var ent in GenerateTestData(num))
                solr.Add(ent);

            solr.Commit();

            var results = solr.Query(SolrQuery.All);
            Assert.Equal(num, results.Count);
        }


        IEnumerable<TestEntity> GenerateTestData(int num)
        {
            for (int i = 0; i < num; i++)
            {
                yield return new TestEntity()
                {
                    Id = i.ToString(),
                    Name = "test" + i
                };
            }
        }



        [Fact(Skip = "TODO move to SolrNet.Cloud.Tests?")]
        public async Task AddRemoveTest()
        {
            const int DocumentCount = 1000;
            const string ZooKeeperConnection = "10.26.11.30:9983";
            await CloudStartup.InitAsync<TestEntity>(new SolrCloudStateProvider(ZooKeeperConnection));
            var operations = CloudStartup.Container.GetInstance<ISolrOperations<TestEntity>>();

            operations.Delete(SolrQuery.All);
            operations.Commit();

            //// send one by one to test sharding distribution and sending to leaders only
            foreach (var ent in GenerateTestData(DocumentCount))
                operations.Add(ent);
            operations.Commit();

            var results = operations.Query(SolrQuery.All);
            Assert.Equal(DocumentCount, results.Count);
        }
    }

    public class TestEntity
    {
        [SolrField("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }
    }
}
