using SolrNet;
using System;
using Xunit;
using static SimpleInjector.SolrNet.Tests.SimpleInjectorFixture;

namespace SimpleInjector.SolrNet.Tests
{
    [Trait("Category", "Integration")]
    public class SimpleInjectorIntegrationFixture
    {
        private readonly Container Container;

        public SimpleInjectorIntegrationFixture()
        {
            Container = new Container();

            // collection needs to exist
            Container.AddSolrNet("http://localhost:8983/solr/FilesCollection");
        }
        
        [Fact]
        public void Ping_And_Query()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        [Fact]
        public void Ping_And_Query_SingleCore()
        {
            var solr = Container.GetInstance<ISolrOperations<Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }
    }
}
