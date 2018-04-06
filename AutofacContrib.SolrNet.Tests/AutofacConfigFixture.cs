using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;
using SolrNet;
using SolrNet.Impl;
using Xunit;

namespace AutofacContrib.SolrNet.Tests
{
    public class AutofacConfigFixture
    {

        [Fact]
        public void RegistersSolrConnectionWithCoresJsonServerUrl()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetParent("../../../").FullName)
               .AddJsonFile("cores.json")
               .Build()
               .GetSection("solr:servers");

            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule(configuration.Get<List<SolrServer>>()));
            var container = builder.Build();
            var solrConnection = (SolrConnection)container.ResolveNamed<ISolrConnection>("entitySolrNet.Impl.SolrConnection");

            Assert.Equal("http://localhost:8983/solr/collection1", solrConnection.ServerURL);
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
            Assert.Equal("http://localhost:8983/solr/collection1", servers.First().Url);
            Assert.Equal("AutofacContrib.SolrNet.Tests.Entity, AutofacContrib.SolrNet.Tests", servers.First().DocumentType);

            Assert.Equal("entity3", servers.Last().Id);
            Assert.Equal("http://localhost:8983/solr/core1", servers.Last().Url);
            Assert.Equal("AutofacContrib.SolrNet.Tests.Entity2, AutofacContrib.SolrNet.Tests", servers.Last().DocumentType);

        }
    }
}
