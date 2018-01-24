using System.Configuration;
using Microsoft.Extensions.Configuration;
using Xunit;
using SolrNet;
using SolrNet.Impl;
using StructureMap.Configuration.DSL;
using StructureMap.SolrNetIntegration.Config;

namespace StructureMap.SolrNetIntegration.Tests
{

    public class StructureMapMultiCoreFixture
    {

        private readonly IContainer Container;
        public StructureMapMultiCoreFixture()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath("D:/repos/SolrNet/StructureMap.SolrNetIntegration.Tests/")
                .AddJsonFile("cores.json")
                .Build()
                .GetSection("solr");
            Container = new Container(c =>
            {
                c.Scan(s =>
                {
                    s.Assembly(typeof(SolrNetRegistry).Assembly);
                    s.Assembly(typeof(SolrConnection).Assembly);
                    s.WithDefaultConventions();
                });
                c.AddRegistry(new SolrNetRegistry(configuration.Get<SolrServers>()));
            });
        }

        [Fact]
        public void Get_SolrOperations_for_Entity()
        {
            var solrOperations = Container.GetInstance<ISolrOperations<Entity>>();
            Assert.NotNull(solrOperations);
        }

        [Fact]
        public void Get_SolrOperations_for_Entity2()
        {
            var solrOperations2 = Container.GetInstance<ISolrOperations<Entity2>>("entity2");
            Assert.NotNull(solrOperations2);
        }

        [Fact]
        public void Same_document_type_different_core_url()
        {
            var core1 = Container.GetInstance<ISolrOperations<Entity2>>("entity2");
            var core2 = Container.GetInstance<ISolrOperations<Entity2>>("entity3");
        }


    }
}
