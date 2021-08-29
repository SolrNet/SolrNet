using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using SolrNet;
using SolrNet.Impl;
using Xunit;

namespace Ninject.Integration.SolrNet.Tests
{
    public class NinjectConfigFixture
    {
        private StandardKernel kernel;

        [Fact]
        public void RegistersSolrConnectionWithCoresJsonServerUrl()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent("../../../").FullName)
               .AddJsonFile("cores.json")
               .Build()
               .GetSection("solr:servers");

            kernel = new StandardKernel();
            kernel.Load(new SolrNetModule(configuration.Get<List<SolrServer>>()));

            var instanceKey = "entity";

            var solrConnection = (SolrConnection)kernel.Get<ISolrConnection>((bm) => bm.Get<string>("CoreId") == "entity", new Parameters.IParameter[] { });

            Assert.Equal("http://localhost:8983/solr/techproducts/collection1", solrConnection.ServerURL);
        }


        [Fact]
        public void CheckParseJsonConfiguration()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent("../../../").FullName)
               .AddJsonFile("cores.json")
               .Build()
               .GetSection("solr:servers");

            var servers = configuration.Get<List<SolrServer>>();

            Assert.Equal(3, servers.Count);
            Assert.Equal("entity", servers.First().Id);
            Assert.Equal("http://localhost:8983/solr/techproducts/collection1", servers.First().Url);
            Assert.Equal("Ninject.Integration.SolrNet.Tests.Entity, Ninject.Integration.SolrNet.Tests", servers.First().DocumentType);

            Assert.Equal("entity3", servers.Last().Id);
            Assert.Equal("http://localhost:8983/solr/techproducts/core1", servers.Last().Url);
            Assert.Equal("Ninject.Integration.SolrNet.Tests.Entity2, Ninject.Integration.SolrNet.Tests", servers.Last().DocumentType);

        }
    }
}
