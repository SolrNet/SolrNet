// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Xunit;
using SolrNet;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SolrNet.Impl;
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

        [Fact]
        public void BasicOpenTelemetryTest()
        {
            // Arrange
            Activity oTelActivity = null;
            var listener = new ActivityListener
            {
                ActivityStarted = _ => { },
                ActivityStopped = activity => oTelActivity = activity,
                ShouldListenTo = activitySource => activitySource.Name == DiagnosticHeaders.DefaultSourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData
            };
            ActivitySource.AddActivityListener(listener);

            var solr = DefaultServiceProvider.GetService<ISolrOperations<MicrosoftDependencyFixture.Entity>>();
            
            // Act
            solr.Query(SolrQuery.All);
            
            // Assert
            Assert.NotNull(oTelActivity);
            Assert.Equal(ActivityKind.Client, oTelActivity.Kind);
            Assert.Equal("query", oTelActivity.OperationName);
            Assert.Equal("query", oTelActivity.DisplayName);
            Assert.Contains(oTelActivity.TagObjects, _ => _ is { Key: "db.system", Value: "solr" });
            Assert.Contains(oTelActivity.TagObjects, _ => _ is { Key: "db.query.text", Value: """[{"q":"*:*"},{"rows":"100000000"}]""" });
            Assert.Contains(oTelActivity.TagObjects, _ => _ is { Key: "db.collection.name", Value: "techproducts" });
            Assert.Contains(oTelActivity.TagObjects, _ => _ is { Key: "server.address", Value: "127.0.0.1" });
            Assert.Contains(oTelActivity.TagObjects, _ => _ is { Key: "server.port" });
            Assert.Contains(oTelActivity.TagObjects, _ => _ is { Key: "solr.status" });
            Assert.Contains(oTelActivity.TagObjects, _ => _ is { Key: "solr.qtime" });
            Assert.Contains(oTelActivity.TagObjects,
                _ => _.Key == "url.full" && Regex.IsMatch(_.Value as string ?? "",
                    @"http:\/\/127\.0\.0\.1:\d+\/solr\/techproducts\/select"));
        }
    }
}
