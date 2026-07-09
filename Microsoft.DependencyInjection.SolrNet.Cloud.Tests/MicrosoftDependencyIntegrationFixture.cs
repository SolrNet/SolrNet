// 

using System;
using Xunit;
using SolrNet;
using SolrNet.Cloud;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;
using SolrNet.Attributes;
using System.Threading.Tasks;

namespace Microsoft.DependencyInjection.SolrNet.Cloud.Tests
{
    
    [Trait("Category","Integration")]
    public class MicrosoftDependencyIntegrationFixture {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly IServiceProvider DefaultServiceProvider;

        public MicrosoftDependencyIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            var sc = new ServiceCollection();
            var zooKeeperUrl = "192.168.1.200:2181";
            var collection = "test";
            sc.AddSolrNetCloud<Entity>(zooKeeperUrl, collection);
            DefaultServiceProvider = sc.BuildServiceProvider();
        }


        [Fact]
        public async Task Ping()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<Entity>>();
            var result = await solr.PingAsync();
            Assert.NotNull(result);
            Assert.Equal(0, result.Status);
            testOutputHelper.WriteLine(result.ToString());
        }

        [Fact]
        public async Task QueryAll()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<Entity>>();
            var result = await solr.QueryAsync(SolrQuery.All);
            Assert.NotNull(result);
            testOutputHelper.WriteLine(result.Count.ToString());
        }

        [Fact]
        public async Task AddEntity()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<Entity>>();
            var entity = new Entity
            {
                Id = "999999999",                
                Name = "Test"
            };
            var result = await solr.AddAsync(entity);
            Assert.NotNull(result);
            Assert.Equal(0, result.Status);
            testOutputHelper.WriteLine(result.ToString());
        }

        [Fact]
        public async Task GetEntity()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<Entity>>();
            string id = "999999999";
            var result = await solr.QueryAsync(new SolrQueryByField("id", id));
            Assert.NotNull(result);
            Assert.NotNull(result[0]);
            Assert.Equal(id, result[0].Id);
            testOutputHelper.WriteLine(result.ToString());
        }

        [Fact]
        public async Task DeleteEntity()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<Entity>>();
            string id = "999999999";
            var result = await solr.DeleteAsync(id);
            Assert.NotNull(result);
            Assert.Equal(0, result.Status);
            testOutputHelper.WriteLine(result.ToString());
        }
    }
}
