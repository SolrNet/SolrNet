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
using Rhino.Mocks;
using SampleSolrApp.Controllers;
using SampleSolrApp.Models;
using SolrNet;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SampleSolrApp.Tests.Controllers {
    [TestFixture]
    public class HomeControllerTest {
        [Test]
        public void Index_Without_parameters() {
            var solr = MockRepository.GenerateMock<ISolrReadOnlyOperations<Product>>();
            solr.Expect(o => o.Query(SolrQuery.All, new QueryOptions()))
                .IgnoreArguments()
                .Return(new SolrQueryResults<Product>());
            var c = new HomeController(solr);
            var result = c.Index(new SearchParameters());   
        }

        [Test]
        public void Facets() {
            
        }
    }
}