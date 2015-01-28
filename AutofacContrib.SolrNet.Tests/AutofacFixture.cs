﻿#region license
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
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Autofac;
using HttpWebAdapters;
using MbUnit.Framework;
using SolrNet;
using SolrNet.Impl;
using SolrNet.Tests.Mocks;
using Mocks = SolrNet.Tests.Mocks;

namespace AutofacContrib.SolrNet.Tests {
    [TestFixture]
    public class AutofacFixture {
        [Test]
        public void ReplaceMapper() {
            var builder = new ContainerBuilder();
            var mapper = new MReadOnlyMappingManager();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr") {Mapper = mapper});
            var container = builder.Build();
            var m = container.Resolve<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, m);
        }

        [Test]
        public void ReplaceHttpWebRequestFactory()
        {
            var builder = new ContainerBuilder();
            var getResponseCalls = 0;
            var response = new Mocks.HttpWebResponse
            {
                dispose = () => { },
                headers = () => new WebHeaderCollection {
                    {HttpResponseHeader.ETag, "123"},
                },
                getResponseStream = () => 
                    // If we don't give back at least the basic XML, we get an XmlParseException
                    new MemoryStream(Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?><response />")),
            };
            IHttpWebRequestFactory factory = new Mocks.HttpWebRequestFactory {
                create = _ => new Mocks.HttpWebRequest {
                    getResponse = () => {
                        getResponseCalls++;
                        return response;
                    },
                    Headers = new WebHeaderCollection(),
                },
            };
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr") { HttpWebRequestFactory = factory });
            var container = builder.Build();
            var operations = container.Resolve<ISolrOperations<Dictionary<string, object>>>();
            var results = operations.Query(new SolrQuery("q:*"));
            Assert.IsNotNull(results);
            Assert.AreEqual(1, getResponseCalls);
        }

        [Test]
        public void ResolveSolrOperations() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var m = container.Resolve<ISolrOperations<Entity>>();
        }

        /// <summary>
        /// Tests that resolving <see cref="ISolrBasicOperations{T}"/> and <see cref="ISolrBasicReadOnlyOperations{T}"/>
        /// resolve to the same instance.
        /// </summary>
        [Test]
        public void ResolveSolrBasicOperationsAndSolrBasicReadOnlyOperationsUseSameEntity() {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var basic = container.Resolve<ISolrBasicOperations<Entity>>();
            var basicReadonly = container.Resolve<ISolrBasicReadOnlyOperations<Entity>>();
            Assert.AreSame(basic, basicReadonly);
        }
        
        [Test]
        public void DictionaryDocument_Operations()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var m = container.Resolve<ISolrOperations<Dictionary<string, object>>>();
        }

        [Test]
        public void DictionaryDocument_ResponseParser()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var parser = container.Resolve<ISolrDocumentResponseParser<Dictionary<string, object>>>();
            Assert.IsInstanceOfType<SolrDictionaryDocumentResponseParser>(parser);
        }

        [Test]
        public void DictionaryDocument_Serializer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new SolrNetModule("http://localhost:8983/solr"));
            var container = builder.Build();
            var serializer = container.Resolve<ISolrDocumentSerializer<Dictionary<string, object>>>();
            Assert.IsInstanceOfType<SolrDictionarySerializer>(serializer);
        }

        public class Entity {}
    }
}