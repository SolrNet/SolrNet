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

using System;
using MbUnit.Framework;

namespace SolrNet.Tests {
    [TestFixture]
    public class SolrQueryBoostTests {
        [Test]
        public void Boost() {
            var q = new SolrQueryBoost(new SolrQuery("solr"), 34.2);
            Assert.AreEqual("(solr)^34.2", q.Query);
        }

        [Test]
        public void Boost_with_culture() {
            using (ThreadSettings.Culture("fr-FR")) {
                var q = new SolrQueryBoost(new SolrQuery("solr"), 34.2);
                Assert.AreEqual("(solr)^34.2", q.Query);                
            }
        }

        [Test]
        public void Boost_with_high_value() {
            var q = new SolrQueryBoost(new SolrQuery("solr"), 34.2E10);
            Assert.AreEqual("(solr)^342000000000", q.Query);
        }

        [Test]
        public void SolrQuery_Boost() {
            var q = new SolrQuery("solr").Boost(12.2);
            Assert.AreEqual("(solr)^12.2", q.Query);
        }
    }
}