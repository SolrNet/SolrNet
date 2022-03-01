// 

using System;
using System.Collections.Generic;
using Autofac;
using SolrNet;
using Xunit;
using Xunit.Abstractions;

namespace AutofacContrib.SolrNet.Tests {
    [Trait("Category","Integration")]
    public class AutofacIntegrationFixture {
        private readonly ITestOutputHelper testOutputHelper;

        public AutofacIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        [Fact]      
        public void Ping_And_Query()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr/techproducts"));
            var container = builder.Build();
            var solr = container.Resolve<ISolrOperations<AutofacFixture.Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

        [Fact]
        public void DictionaryDocument()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr/techproducts"));
            var container = builder.Build();
            var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>();
            var results = solr.Query(SolrQuery.All);
            Assert.True(results.Count>  0);
            foreach (var d in results)
            {
                Assert.True(d.Count> 0);
                foreach (var kv in d)
                    testOutputHelper.WriteLine("{0}: {1}", kv.Key, kv.Value);
            }
        }

        [Fact]
        public void DictionaryDocument_add()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr/techproducts"));
            var container = builder.Build();
            var solr = container.Resolve<ISolrOperations<Dictionary<string, object>>>();
            solr.Add(new Dictionary<string, object> {
                {"id", "ababa"},
                {"manu", "who knows"},
                {"popularity", 55}
            });
        }
    }
}
