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
using Xunit;
using SolrNet;
using Microsoft.Extensions.DependencyInjection;
using SolrNet.Impl;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SolrNet.Microsoft.DependencyInjection;

namespace Microsoft.DependencyInjection.SolrNet.Tests
{

    public class MicrosoftDependencyFixture
    {


        private readonly IServiceProvider DefaultServiceProvider;

        public MicrosoftDependencyFixture()
        {

            var sc = new ServiceCollection();

            sc.AddSolrNet("http://localhost:8983/solr/techproducts");
            DefaultServiceProvider = sc.BuildServiceProvider();
        }

        [Fact]
        public void ResolveIReadOnlyMappingManager()
        {
            var mapper = DefaultServiceProvider.GetService<IReadOnlyMappingManager>();
            Assert.NotNull(mapper);
        }

        [Fact]
        public void ResolveIReadOnlyMappingManagers()
        {
            var mapper = DefaultServiceProvider.GetService<IEnumerable<IReadOnlyMappingManager>>();
            Assert.NotNull(mapper);
        }


        [Fact]
        public void ResolveSolrDocumentActivator()
        {
            var solr = DefaultServiceProvider.GetService<ISolrDocumentActivator<Entity>>();
            Assert.NotNull(solr);
        }

        [Fact]
        public void ResolveSolrAbstractResponseParser()
        {
            var solr = DefaultServiceProvider.GetService<ISolrAbstractResponseParser<Entity>>();
            Assert.NotNull(solr);
        }

        [Fact]
        public void CannotResolveSolrAbstractResponseParsersViaArray()
        {
            //MS Dependency injection doesn't support 
            Assert.Throws<InvalidOperationException>(() =>
                DefaultServiceProvider.GetRequiredService<ISolrAbstractResponseParser<Entity>[]>()
            );

        }


        [Fact]
        public void ResolveSolrAbstractResponseParsersViaEnumerable()
        {
            //MS Dependency injection doesn't support 
            var result = DefaultServiceProvider.GetRequiredService<IEnumerable<ISolrAbstractResponseParser<Entity>>>();
            Assert.NotNull(result);
            Assert.Single(result);

        }

        [Fact]
        public void ResolveSolrMoreLikeThisHandlerQueryResultsParser()
        {
            var solr = DefaultServiceProvider.GetService<ISolrMoreLikeThisHandlerQueryResultsParser<Entity>>();
            Assert.NotNull(solr);
        }


        [Fact]
        public void ResolveSolrOperations()
        {
            var solr = DefaultServiceProvider.GetService<ISolrOperations<Entity>>();
            Assert.NotNull(solr);
        }

        [Fact]
        public void ServiceCollectionIsRequired()
        {
            var expectedMessage = new Regex("Value cannot be null.*services");

            var exception1 = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddSolrNet(null, "http://bar.com"));
            var exception2 = Assert.Throws<ArgumentNullException>(() => ServiceCollectionExtensions.AddSolrNet<Entity>(null, "http://bar.com"));

            Assert.Matches(expectedMessage, exception1.Message);
            Assert.Matches(expectedMessage, exception2.Message);
        }

        [Fact]
        public void UrlIsRequired()
        {
            var sc = new ServiceCollection();
            var expectedMessage = new Regex("Value cannot be null.*url");
            
            var exception1 = Assert.Throws<ArgumentNullException>(() => sc.AddSolrNet((string) null));
            var exception2 = Assert.Throws<ArgumentNullException>(() => sc.AddSolrNet<Entity>((string) null));

            Assert.Matches(expectedMessage, exception1.Message);
            Assert.Matches(expectedMessage, exception2.Message);
        }

        [Fact]
        public void UrlRetrieverIsRequired()
        {
            var sc = new ServiceCollection();
            var expectedMessage = new Regex("Value cannot be null.*urlRetriever");

            var exception1 = Assert.Throws<ArgumentNullException>(() => sc.AddSolrNet((Func<IServiceProvider, string>) null));
            var exception2 = Assert.Throws<ArgumentNullException>(() => sc.AddSolrNet<Entity>((Func<IServiceProvider, string>) null));

            Assert.Matches(expectedMessage, exception1.Message);
            Assert.Matches(expectedMessage, exception2.Message);
        }

        [Fact]
        public void UrlRetrieverMustReturnValidUrl()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSolrNet(sp => null);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            var expectedMessage = new Regex("Value cannot be null.*solrUrl");

            var exception = Assert.Throws<ArgumentNullException>(() => serviceProvider.GetService<ISolrOperations<Entity>>());
       
            Assert.Matches(expectedMessage, exception.Message);
        }

        [Fact]
        public void OnlyOneNonTypedAllowed()
        {
            var sc = new ServiceCollection();
            sc.AddSolrNet("http://foo.com");
            var exception = Assert.Throws<InvalidOperationException>(() => sc.AddSolrNet("http://bar.com"));

            Assert.Contains("Only one non-typed Solr Core", exception.Message);
        }

        [Fact]
        public void NoDuplicateTypesAllowed()
        {
            var sc = new ServiceCollection();
            sc.AddSolrNet<Entity>("http://foo.com");
            sc.AddSolrNet<Object>("http://whatever.com");
            var exception = Assert.Throws<InvalidOperationException>(() => sc.AddSolrNet<Entity>("http://bar.com"));

            Assert.Equal($"SolrNet was already added for model of type {nameof(Entity)}", exception.Message);
        }

        [Fact]
        public void NonTypedBeforeTyped()
        {
            var sc = new ServiceCollection();
            sc.AddSolrNet<Entity>("http://foo.com");
            var exception = Assert.Throws<InvalidOperationException>(() => sc.AddSolrNet("http://bar.com"));

            Assert.Contains("Only one non-typed Solr Core", exception.Message);
        }



        [Fact]
        public void SetBasicAuthenticationHeader()
        {
            var sc = new ServiceCollection();

            //my credentials
            var credentials = System.Text.Encoding.ASCII.GetBytes("myUsername:myPassword");
            //in base64
            var credentialsBase64 = Convert.ToBase64String(credentials);
            //use the options to set the Authorization header.
            sc.AddSolrNet("http://localhost:8983/solr/techproducts", options => { options.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentialsBase64); });

            //test
            var provider = sc.BuildServiceProvider();
            var connection = provider.GetRequiredService<ISolrConnection>() as AutoSolrConnection;

            Assert.NotNull(connection.HttpClient.DefaultRequestHeaders.Authorization);
            Assert.Equal(credentialsBase64, connection.HttpClient.DefaultRequestHeaders.Authorization.Parameter);
        }

        [Fact]
        public void SetBasicAuthenticationHeaderForTypedInstance()
        {
            var sc = new ServiceCollection();

            //my credentials
            var credentials = System.Text.Encoding.ASCII.GetBytes("myUsername:myPassword");
            //in base64
            var credentialsBase64 = Convert.ToBase64String(credentials);
            //use the options to set the Authorization header.
            sc.AddSolrNet<string>("http://localhost:8983/solr/techproducts", options => { options.HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentialsBase64); });

            //test
            var provider = sc.BuildServiceProvider();
            var injectionConnection = provider.GetRequiredService<ISolrInjectedConnection<string>>() as BasicInjectionConnection<string>;
            var connection = injectionConnection.Connection as AutoSolrConnection;
            Assert.NotNull(connection.HttpClient.DefaultRequestHeaders.Authorization);
            Assert.Equal(credentialsBase64, connection.HttpClient.DefaultRequestHeaders.Authorization.Parameter);
        }

        public class Entity { }
    }
}
