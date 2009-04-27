#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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

using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Mapping;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class IntegrationTests {
        [TearDown]
        public void Teardown() {
            Startup.Container.RemoveAll<ISolrConnection>();
            Startup.Container.RemoveAll<ISolrQueryResultParser<Document>>();
            Startup.Container.RemoveAll<ISolrQueryExecuter<Document>>();
            Startup.Container.RemoveAll<ISolrDocumentSerializer<Document>>();
            Startup.Container.RemoveAll<ISolrBasicOperations<Document>>();
            Startup.Container.RemoveAll<ISolrBasicReadOnlyOperations<Document>>();
            Startup.Container.RemoveAll<ISolrOperations<Document>>();
            Startup.Container.RemoveAll<ISolrReadOnlyOperations<Document>>();
        }

        [Test]
        public void SwappingMappingManager() {
            var mapper = new MappingManager();
            var container = new Container(Startup.Container);
            container.RemoveAll<IReadOnlyMappingManager>();
            container.Register<IReadOnlyMappingManager>(c => mapper);
            Startup.Init<Document>("http://localhost");
            ServiceLocator.SetLocatorProvider(() => container);
            var mapperFromFactory = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        [Test]
        public void SwappingMappingManager2() {
            var mapper = new MappingManager();
            Startup.Container.RemoveAll<IReadOnlyMappingManager>();
            Startup.Container.Register<IReadOnlyMappingManager>(c => mapper);
            Startup.Init<Document>("http://localhost");
            ServiceLocator.SetLocatorProvider(() => Startup.Container);
            var mapperFromFactory = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        public class Document {}
    }
}