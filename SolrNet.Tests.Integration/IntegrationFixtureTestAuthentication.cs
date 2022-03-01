#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Text;
using Xunit;
using System.Threading.Tasks;
using HttpWebAdapters;
using SolrNet.Exceptions;
using Xunit.Abstractions;

namespace SolrNet.Tests.Integration
{
    [Trait("Category", "Integration")]
    [TestCaseOrderer(MethodDefTestCaseOrderer.Type, MethodDefTestCaseOrderer.Assembly)]
    public class IntegrationFixtureTestAuthentication
    {
        //private readonly ITestOutputHelper testOutputHelper;
        //private readonly IServiceProvider defaultServiceProviderAuth_WithSyncAuth;
        //private readonly IServiceProvider defaultServiceProviderAuth_WithSyncAndAsyncAuth;
        //private readonly IServiceProvider defaultServiceProviderAuth_WithoutAuth;     

        //public IntegrationFixtureTestAuthentication(ITestOutputHelper testOutputHelper)
        //{
        //    var sc = new ServiceCollection();
        //    this.defaultServiceProviderAuth_WithSyncAuth.AddSolrNet("http://localhost:8984/solr/techproducts",
        //        null,
        //        () => new BasicAuthHttpWebRequestFactory("solr", "SolrRocks"));

        //    this.defaultServiceProviderAuth_WithSyncAndAsyncAuth = new ServiceContainer();
        //    this.defaultServiceProviderAuth_WithSyncAndAsyncAuth.AddSolrNet("http://localhost:8984/solr/techproducts",
        //        (options) => options.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"solr:SolrRocks"))),
        //        () => new BasicAuthHttpWebRequestFactory("solr", "SolrRocks"));


        //    this.defaultServiceProviderAuth_WithoutAuth = new ServiceContainer();
        //    this.defaultServiceProviderAuth_WithoutAuth.AddSolrNet("http://localhost:8984/solr/techproducts");     
        //}

        //[Fact]
        //public void Test_SolrWithAuth_ServiceContainerSyncAuth_Ping()
        //{
        //    var solr = defaultServiceProviderAuth_WithSyncAuth.GetInstance<ISolrOperations<LightInjectFixture.Entity>>();
        //    solr.Ping();
        //    testOutputHelper.WriteLine(solr.Query(SolrQuery.All).Count.ToString());
        //}

        //[Fact]
        //public void Test_SolrWithAuth_ServiceContainerASyncAuth_Ping()
        //{
        //    var solr = defaultServiceProviderAuth_WithSyncAndAsyncAuth.GetInstance<ISolrOperations<LightInjectFixture.Entity>>();
        //    Task.Run(async () => await solr.PingAsync());
        //    testOutputHelper.WriteLine(Task.Run(async () => await solr.QueryAsync(SolrQuery.All)).Result.Count.ToString());
        //}

        //[Fact]
        //public void Test_SolrWithAuth_ServiceContainerSyncAuth_AsyncPingFails()
        //{
        //    var solr = defaultServiceProviderAuth_WithSyncAuth.GetInstance<ISolrOperations<LightInjectFixture.Entity>>();
        //    var ex = Assert.ThrowsAsync<SolrConnectionException>(async () => {
        //        await solr.PingAsync();
        //    });
        //    Assert.Contains("401", ex.Result.Message.ToLower());
        //}

        //[Fact]
        //public void Test_SolrWithAuth_ServiceContainerWithoutAuth_PingFails()
        //{
        //    var solr = defaultServiceProviderAuth_WithoutAuth.GetInstance<ISolrOperations<LightInjectFixture.Entity>>();
        //    var ex = Assert.Throws<SolrConnectionException>(() => {
        //        solr.Ping();
        //    });
        //    Assert.Contains("401", ex.Message.ToLower());
        //}
    }
}
