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

using System.Linq;
using MbUnit.Framework;
using SolrNet.Commands.Parameters;

namespace SolrNet.Tests {
    [TestFixture]
    public class QueryOptionsTests {
        [Test]
        public void AddFields() {
            QueryOptions o = new QueryOptions().AddFields("f1", "f2");
            Assert.AreEqual(2, o.Fields.Count);
            Assert.AreEqual("f1", o.Fields.First());
            Assert.AreEqual("f2", o.Fields.ElementAt(1));
        }

        [Test]
        public void AddOrder() {
            QueryOptions o = new QueryOptions().AddOrder(new SortOrder("f1"), new SortOrder("f2", Order.ASC));
            Assert.AreEqual(2, o.OrderBy.Count);
        }

        [Test]
        public void AddFilterQueries() {
            QueryOptions o = new QueryOptions().AddFilterQueries(new SolrQuery("a"), new SolrQueryByField("f1", "v"));
            Assert.AreEqual(2, o.FilterQueries.Count);  
        }

        [Test]
        public void AddFacets() {
            QueryOptions o = new QueryOptions().AddFacets(new SolrFacetFieldQuery("f1"), new SolrFacetQuery(new SolrQuery("q")));
            Assert.AreEqual(2, o.Facet.Queries.Count);
        }
    }
}