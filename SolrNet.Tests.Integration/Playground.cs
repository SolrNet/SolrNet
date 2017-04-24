using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using MbUnit.Framework;
using SolrNet.Attributes;
using SolrNet.Cloud.ZooKeeperClient;
using CloudStartup = SolrNet.Cloud.Startup;

namespace SolrNet.Unity.Tests
{
    public class Test
    {
        [Test]
        public void TestSorlCloud() {
            using (var provider = new SolrCloudStateProvider("10.26.11.30:9983")) {
                CloudStartup.Init<TestEntity>(provider);
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
            Assert.AreEqual(num, results.Count, "results count");
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

        [Test]
        public void AddRemoveTest()
        {
            const int DocumentCount = 1000;
            const string ZooKeeperConnection = "10.26.11.30:9983";

            CloudStartup.Init<TestEntity>(new SolrCloudStateProvider(ZooKeeperConnection));
            var operations = CloudStartup.Container.GetInstance<ISolrOperations<TestEntity>>();

            operations.Delete(SolrQuery.All);
            operations.Commit();

            //// send one by one to test sharding distribution and sending to leaders only
            foreach (var ent in GenerateTestData(DocumentCount))
                operations.Add(ent);
            operations.Commit();

            var results = operations.Query(SolrQuery.All);
            Assert.AreEqual(DocumentCount, results.Count, "results count");
        }
    }

    public class TestEntity {
        [SolrField("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }
    }
}
