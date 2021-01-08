﻿// 

using System;
using Xunit;
using SolrNet;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.DependencyInjection.SolrNet.Tests
{
    [Trait("Category","Integration")]
    public class MicrosoftDependencyIntegrationFixture {
        private readonly IServiceProvider DefaultServiceProvider;

        public MicrosoftDependencyIntegrationFixture()
        {
            var sc = new ServiceCollection();
            sc.AddSolrNet("http://localhost:8983/solr");
            DefaultServiceProvider = sc.BuildServiceProvider();
        }

        [Fact]
        public void Ping_And_Query()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<MicrosoftDependencyFixture.Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

        [Fact]
        public void Ping_And_Query_SingleCore()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<MicrosoftDependencyFixture.Entity>>();
            solr.Ping();
            Console.WriteLine(solr.Query(SolrQuery.All).Count);
        }

    }
}
