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
using Xunit;
using SolrNet.Impl;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    
    public class CollapseResponseParserTests {
        [Fact]
        public void Parse() {
            var parser = new CollapseResponseParser<TestDoc>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.collapseResponse.xml");
            var results = new SolrQueryResults<TestDoc>();
            parser.Parse(xml, results);
            Assert.NotNull(results.Collapsing);
            Assert.Equal("manu_exact", results.Collapsing.Field);
            Assert.Equal(5, results.Collapsing.CollapsedDocuments.Count);
            var firstCollapse = results.Collapsing.CollapsedDocuments.ElementAt(0);
            Assert.Equal("F8V7067-APL-KIT", firstCollapse.Id);
            Assert.Equal(1, firstCollapse.CollapseCount);
            Assert.Equal("Belkin", firstCollapse.FieldValue);
        }

        [Fact]
        public void Parse2() {
            var parser = new CollapseResponseParser<TestDoc>();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.collapseResponse2.xml");
            var results = new SolrQueryResults<TestDoc>();
            parser.Parse(xml, results);
            Assert.NotNull(results.Collapsing);
            Assert.Equal("manu_exact", results.Collapsing.Field);
            Assert.Equal(3, results.Collapsing.CollapsedDocuments.Count);
            var firstCollapse = results.Collapsing.CollapsedDocuments.ElementAt(0);
            Assert.Equal("F8V7067-APL-KIT", firstCollapse.Id);
            Assert.Equal(1, firstCollapse.CollapseCount);
            Assert.Equal("Belkin", firstCollapse.FieldValue);
        }

        public class TestDoc {}
    }
}