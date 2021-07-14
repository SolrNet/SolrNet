using Xunit;
using SolrNet;
using SolrNet.Impl;
using System.Collections.Generic;
#if NETCOREAPP2_0 || NETSTANDARD2_0
using Microsoft.Extensions.Configuration;
using System.IO;
#else
using System.Configuration;
#endif
using StructureMap.SolrNetIntegration.Config;

namespace StructureMap.SolrNetIntegration.Tests
{

    public class StructureMapMultiCoreFixture
    {

        private readonly IContainer Container;
        public StructureMapMultiCoreFixture()
        {

            var servers = new List<SolrServer>
            {
                new SolrServer ("entity","http://localhost:8983/solr/techproducts/collection1", "StructureMap.SolrNetIntegration.Tests.Entity, StructureMap.SolrNetIntegration.Tests"),
                new SolrServer ("entity2","http://localhost:8983/solr/techproducts/core0", "StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests"),
                new SolrServer ("entity3","http://localhost:8983/solr/techproducts/core1", "StructureMap.SolrNetIntegration.Tests.Entity2, StructureMap.SolrNetIntegration.Tests")
            };
         
            Container = new Container(c =>
            {
                c.Scan(s =>
                {
                    s.Assembly(typeof(SolrNetRegistry).Assembly);
                    s.Assembly(typeof(SolrConnection).Assembly);
                    s.WithDefaultConventions();
                });
                c.AddRegistry(SolrNetRegistry.Create(servers));
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
