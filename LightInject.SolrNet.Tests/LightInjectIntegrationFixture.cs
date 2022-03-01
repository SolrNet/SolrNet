// 

using System;
using Xunit;
using SolrNet;
using Xunit.Abstractions;

namespace LightInject.SolrNet.Tests
{
    
    [Trait("Category","Integration")]
    public class LightInjectIntegrationFixture 
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly IServiceContainer defaultServiceProvider;

        public LightInjectIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            
            this.defaultServiceProvider = new ServiceContainer();
            this.defaultServiceProvider.AddSolrNet("http://localhost:8983/solr/techproducts");
        }


        [Fact]
        public void Ping_And_Query()
        {
            var solr = defaultServiceProvider.GetInstance<ISolrOperations<LightInjectFixture.Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

        [Fact]
        public void Ping_And_Query_SingleCore()
        {
            var solr = defaultServiceProvider.GetInstance<ISolrOperations<LightInjectFixture.Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

    }
}
