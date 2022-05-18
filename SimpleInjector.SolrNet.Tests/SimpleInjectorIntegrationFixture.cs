using SolrNet;
using System;
using Xunit;
using Xunit.Abstractions;
using static SimpleInjector.SolrNet.Tests.SimpleInjectorFixture;

namespace SimpleInjector.SolrNet.Tests
{
    [Trait("Category", "Integration")]
    public class SimpleInjectorIntegrationFixture
    {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly Container Container;

        public SimpleInjectorIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            Container = new Container();

            // collection needs to exist
            Container.AddSolrNet("http://localhost:8983/solr/entity1");
        }
        
        [Fact]
        public void Ping_And_Query()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

        [Fact]
        public void Ping_And_Query_SingleCore()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }
    }
}
