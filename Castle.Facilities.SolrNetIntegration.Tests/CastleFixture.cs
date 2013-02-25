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

using Castle.Core.Configuration;
using Castle.Core.Resource;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using MbUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Castle.Facilities.SolrNetIntegration.Tests
{
    using SolrNet;
    using SolrNet.Impl;
    using SolrNet.Mapping.Validation;
    using SolrNet.Tests.Mocks;

    [TestFixture]
    public class CastleFixture
    {
        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void NoConfigThrows()
        {
            var container = new WindsorContainer();
            container.AddFacility<SolrNetFacility>();
        }

        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void InvalidUrlThrows()
        {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("facility");

            configuration.Attributes.Add("type", typeof(SolrNetFacility).FullName);
            configuration.CreateChild("solrURL", "123");
            configStore.AddFacilityConfiguration(typeof(SolrNetFacility).FullName, configuration);

            var container = new WindsorContainer(configStore);
        }

        [Test]
        [ExpectedException(typeof(FacilityException))]
        public void InvalidProtocolThrows()
        {
            var configStore = new DefaultConfigurationStore();
            var configuration = new MutableConfiguration("facility");

            configuration.Attribute("type", typeof(SolrNetFacility).AssemblyQualifiedName);
            configuration.CreateChild("solrURL", "ftp://localhost");
            configStore.AddFacilityConfiguration(typeof(SolrNetFacility).FullName, configuration);

            var container = new WindsorContainer(configStore);
        }

        [Test]
        public void ReplacingMapper()
        {
            var mapper = new MReadOnlyMappingManager();
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr") { Mapper = mapper };

            var container = new WindsorContainer();
            container.AddFacility(solrFacility);

            var m = container.Resolve<IReadOnlyMappingManager>();

            Assert.AreSame(m, mapper);
        }

        [Test]
        public void ContainerHasISolrFieldParser()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);
            container.Resolve<ISolrFieldParser>();
        }

        [Test]
        public void ContainerHasISolrFieldSerializer()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);
            container.Resolve<ISolrFieldSerializer>();
        }

        [Test]
        public void ContainerHasISolrDocumentPropertyVisitor()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);
            container.Resolve<ISolrDocumentPropertyVisitor>();
        }

        [Test]
        public void ResolveISolrOperations()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);
            container.Resolve<ISolrOperations<Document>>();
        }


        [Test]
        public void MultiCore()
        {
            const string Core0URL = "http://localhost:8983/solr/core0";
            const string Core1URL = "http://localhost:8983/solr/core1";

            var solrFacility = new SolrNetFacility(Core0URL);
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);

            // override core1 components
            const string Core1Connection = "core1.connection";

            container.Register(Component.For<ISolrConnection>().ImplementedBy<SolrConnection>().Named(Core1Connection).DependsOn(new Dependency[] { Parameter.ForKey("serverURL").Eq(Core1URL) }));
            container.Register(Component.For(typeof(ISolrBasicOperations<Core1Entity>), typeof(ISolrBasicReadOnlyOperations<Core1Entity>)).ImplementedBy<SolrBasicServer<Core1Entity>>().DependsOn(new Dependency[] { ServiceOverride.ForKey("connection").Eq(Core1Connection) }));
            container.Register(Component.For<ISolrQueryExecuter<Core1Entity>>().ImplementedBy<SolrQueryExecuter<Core1Entity>>().DependsOn(new Dependency[] { ServiceOverride.ForKey("connection").Eq(Core1Connection) }));

            // assert that everything is correctly wired
            container.Kernel.DependencyResolving += (client, model, dep) =>
            {
                if (model.TargetType != typeof(ISolrConnection))
                    return;

                if (client.Services.Contains(typeof(ISolrBasicOperations<Core1Entity>)) || client.Services.Contains(typeof(ISolrQueryExecuter<Core1Entity>)))
                    Assert.AreEqual(Core1URL, ((SolrConnection)dep).ServerURL);

                if (client.Services.Contains(typeof(ISolrBasicOperations<Document>)) || client.Services.Contains(typeof(ISolrQueryExecuter<Document>)))
                    Assert.AreEqual(Core0URL, ((SolrConnection)dep).ServerURL);
            };

            container.Resolve<ISolrOperations<Core1Entity>>();
            container.Resolve<ISolrOperations<Document>>();
        }

        [Test]
        public void AddCore()
        {
            const string Core0URL = "http://localhost:8983/solr/core0";
            const string Core1URL = "http://localhost:8983/solr/core1";

            var solrFacility = new SolrNetFacility("http://localhost:8983/solr/defaultCore");

            solrFacility.AddCore("core0-id", typeof(Document), Core0URL);
            solrFacility.AddCore("core1-id", typeof(Document), Core1URL);
            solrFacility.AddCore("core2-id", typeof(Core1Entity), Core1URL);

            var container = new WindsorContainer();
            container.AddFacility(solrFacility);

            TestCores(container);
        }

        [Test]
        public void AddCoreFromXml()
        {
            var container = new WindsorContainer(new XmlInterpreter(new StaticContentResource(@"<castle>
<facilities>
    <facility id='solr' type='Castle.Facilities.SolrNetIntegration.SolrNetFacility, Castle.Facilities.SolrNetIntegration'>
        <solrURL>http://localhost:8983/solr/defaultCore</solrURL>
        <cores>
            <core id='core0-id'>
                <documentType>Castle.Facilities.SolrNetIntegration.Tests.CastleFixture+Document, Castle.Facilities.SolrNetIntegration.Tests</documentType>
                <url>http://localhost:8983/solr/core0</url>
            </core>
            <core id='core1-id'>
                <documentType>Castle.Facilities.SolrNetIntegration.Tests.CastleFixture+Document, Castle.Facilities.SolrNetIntegration.Tests</documentType>
                <url>http://localhost:8983/solr/core1</url>
            </core>
            <core id='core2-id'>
                <documentType>Castle.Facilities.SolrNetIntegration.Tests.CastleFixture+Core1Entity, Castle.Facilities.SolrNetIntegration.Tests</documentType>
                <url>http://localhost:8983/solr/core1</url>
            </core>
        </cores>
    </facility>
</facilities>
</castle>")));

            TestCores(container);
        }

        public void TestCores(IWindsorContainer container)
        {
            // assert that everything is correctly wired
            container.Kernel.DependencyResolving += (client, model, dep) =>
            {
                if (model.TargetType != typeof(ISolrConnection))
                    return;

                if (client.Name.StartsWith("core0-id"))
                    Assert.AreEqual("http://localhost:8983/solr/core0", ((SolrConnection)dep).ServerURL);

                if (client.Name.StartsWith("core1-id"))
                    Assert.AreEqual("http://localhost:8983/solr/core1", ((SolrConnection)dep).ServerURL);

                if (client.Name.StartsWith("core2-id"))
                    Assert.AreEqual("http://localhost:8983/solr/core1", ((SolrConnection)dep).ServerURL);
            };

            Assert.IsInstanceOfType<ISolrOperations<Document>>(container.Resolve<ISolrOperations<Document>>("core0-id"));
            Assert.IsInstanceOfType<ISolrOperations<Document>>(container.Resolve<ISolrOperations<Document>>("core1-id"));
            Assert.IsInstanceOfType<ISolrOperations<Core1Entity>>(container.Resolve<ISolrOperations<Core1Entity>>("core2-id"));
        }



        [Test]
        public void DictionaryDocumentOperations()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);
            container.Resolve<ISolrOperations<Dictionary<string, object>>>();
        }

        [Test]
        public void DictionaryDocumentResponseParser()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);

            var parser = container.Resolve<ISolrDocumentResponseParser<Dictionary<string, object>>>();

            Assert.IsInstanceOfType<SolrDictionaryDocumentResponseParser>(parser);
        }

        [Test]
        public void DictionaryDocumentSerializer()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);

            var serializer = container.Resolve<ISolrDocumentSerializer<Dictionary<string, object>>>();

            Assert.IsInstanceOfType<SolrDictionarySerializer>(serializer);
        }

        [Test]
        public void MappingValidationManager()
        {
            var solrFacility = new SolrNetFacility("http://localhost:8983/solr");
            var container = new WindsorContainer();

            container.AddFacility(solrFacility);

            var validator = container.Resolve<IMappingValidator>();
        }

        [Test]
        public void SetConnectionTimeoutInMulticore()
        {
            const string Core0URL = "http://localhost:8983/solr/core0";
            const string Core1URL = "http://localhost:8983/solr/core1";

            var solrFacility = new SolrNetFacility("http://localhost:8983/solr/defaultCore");

            solrFacility.AddCore("core0-id", typeof(Document), Core0URL);
            solrFacility.AddCore("core1-id", typeof(Document), Core1URL);
            solrFacility.AddCore("core2-id", typeof(Core1Entity), Core1URL);

            var container = new WindsorContainer();

            container.Kernel.ComponentModelCreated += model =>
            {
                if (model.Implementation == typeof(SolrConnection))
                    model.Parameters.Add("Timeout", "2000");
            };

            container.AddFacility(solrFacility);

            var allTimeouts = container.ResolveAll<ISolrConnection>().Cast<SolrConnection>().Select(x => x.Timeout);

            foreach (var t in allTimeouts)
                Assert.AreEqual(2000, t);
        }

        public class Document { }

        public class Core1Entity { }
    }
}