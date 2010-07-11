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

using MbUnit.Framework;
using Microsoft.Practices.ServiceLocation;
using SolrNet.Mapping;
using SolrNet.Mapping.Validation;
using SolrNet.Utils;

namespace SolrNet.Tests {
    [TestFixture]
    public class IntegrationTests {

        [Test]
        public void SwappingMappingManager() {
            var mapper = new MappingManager();
            Startup.Container.Clear();
            Startup.InitContainer();
            var container = new Container(Startup.Container);
            container.RemoveAll<IReadOnlyMappingManager>();
            container.Register<IReadOnlyMappingManager>(c => mapper);
            ServiceLocator.SetLocatorProvider(() => container);
            Startup.Init<Document>("http://localhost");
            var mapperFromFactory = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        [Test]
        public void SwappingMappingManager2() {
            var mapper = new MappingManager();
            Startup.Container.Clear();
            Startup.InitContainer();
            Startup.Container.RemoveAll<IReadOnlyMappingManager>();
            Startup.Container.Register<IReadOnlyMappingManager>(c => mapper);
            ServiceLocator.SetLocatorProvider(() => Startup.Container);
            Startup.Init<Document>("http://localhost");
            var mapperFromFactory = ServiceLocator.Current.GetInstance<IReadOnlyMappingManager>();
            Assert.AreSame(mapper, mapperFromFactory);
        }

        [Test]
        public void MappingValidationManager() {
            Startup.Container.Clear();
            Startup.InitContainer();
            var manager = Startup.Container.GetInstance<IMappingValidator>();
        }

        public class Document {}
    }
}