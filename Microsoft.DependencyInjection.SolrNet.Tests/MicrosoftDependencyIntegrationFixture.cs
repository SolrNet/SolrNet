// 

using System;
using Xunit;
using SolrNet;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SolrNet.Tests.Common;
using Xunit.Abstractions;

namespace Microsoft.DependencyInjection.SolrNet.Tests
{
    
    [Trait("Category","Integration")]
    public class MicrosoftDependencyIntegrationFixture {
        private readonly ITestOutputHelper testOutputHelper;
        private readonly IServiceProvider DefaultServiceProvider;

        public MicrosoftDependencyIntegrationFixture(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            var sc = new ServiceCollection();
            
            sc.AddSolrNet($"{TestContainers.BaseUrl}solr/techproducts");

            DefaultServiceProvider = sc.BuildServiceProvider();
        }


        [Fact]
        public void Ping_And_Query()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<MicrosoftDependencyFixture.Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

        [Fact]
        public void Ping_And_Query_SingleCore()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<MicrosoftDependencyFixture.Entity>>();
            solr.Ping();
            testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        }

    }
}
