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
using SolrNet.Cloud;
using Microsoft.Extensions.DependencyInjection;
using SolrNet.Impl;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using SolrNet.Attributes;

namespace Microsoft.DependencyInjection.SolrNet.Cloud.Tests
{
    /// <summary>
    /// Model class for tests.
    /// </summary>
    public class Entity {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("name")]
        public string Name { get; set; }
    }

    public class MicrosoftDependencyFixture
    {
        private readonly IServiceProvider DefaultServiceProvider;
        private readonly string zooKeeperUrl;
        private readonly string collection;

        public MicrosoftDependencyFixture()
        {
            var sc = new ServiceCollection();
            zooKeeperUrl = "192.168.1.200:2181";
            collection = "test";
            sc.AddSolrNetCloud<Entity>(zooKeeperUrl, collection);
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
            var exception = Assert.Throws<ArgumentNullException>(() =>
                ServiceCollectionExtensionsSolrCloud.AddSolrNetCloud<Entity>(null, zooKeeperUrl, collection));
            Assert.Matches(expectedMessage, exception.Message);
        }

        [Fact]
        public void ZooKeeperUrlIsRequired()
        {
            var sc = new ServiceCollection();
            var expectedMessage = new Regex("Value cannot be null.*zooKeeperUrl");            
            var exception = Assert.Throws<ArgumentNullException>(() => sc.AddSolrNetCloud<Entity>((string)null, collection));
            Assert.Matches(expectedMessage, exception.Message);
        }

        [Fact]
        public void CollectionNameIsRequired()
        {
            var sc = new ServiceCollection();
            var expectedMessage = new Regex("Value cannot be null.*collection");
            var exception = Assert.Throws<ArgumentNullException>(() => sc.AddSolrNetCloud<Entity>(zooKeeperUrl, (string)null));
            Assert.Matches(expectedMessage, exception.Message);
        }

        [Fact]
        public void NoDuplicateTypesAllowed()
        {
            var sc = new ServiceCollection();
            sc.AddSolrNetCloud<Entity>(zooKeeperUrl, collection);           
            var exception = Assert.Throws<InvalidOperationException>(() => sc.AddSolrNetCloud<Entity>(zooKeeperUrl, collection));
            Assert.Equal($"SolrNet was already added for model of type {nameof(Entity)}", exception.Message);
        }
    }
}
