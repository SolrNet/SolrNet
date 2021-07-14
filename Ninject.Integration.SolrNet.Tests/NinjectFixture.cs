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

namespace Ninject.Integration.SolrNet.Tests {
    
    public class NinjectFixture {

        [Fact]
        public void ReplaceMapper() {
            var c = new StandardKernel();
            var mapper = new global::SolrNet.Tests.Mocks.MReadOnlyMappingManager();
            c.Load(new SolrNetModule("http://localhost:8983/solr/techproducts") {Mapper = mapper});
            var m = c.Get<IReadOnlyMappingManager>();
            Assert.Same(mapper, m);
        }

        [Fact]
        public void ResolveSolrOperations() {
            var c = new StandardKernel();
            c.Load(new SolrNetModule("http://localhost:8983/solr/techproducts"));
            var m = c.Get<ISolrOperations<Entity>>();
        }

        public class Entity {}
    }
}
