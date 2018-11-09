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
using System.Collections.Generic;
using Xunit;
using SolrNet.Impl;
using SolrNet.Tests.Utils;

namespace SolrNet.Tests {
    
    public class SolrStatusResponseParserTests {
        [Fact]
        public void ParseDocument() {
            var parser = new SolrStatusResponseParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), "Resources.responseWithStatus.xml");
            var r = parser.Parse(xml);
            Assert.NotNull(r);
            Assert.True(r.Count > 0);
        }

        private List<CoreResult> ParseFromResults(string xmlResource) {
            var parser = new SolrStatusResponseParser();
            var xml = EmbeddedResource.GetEmbeddedXml(GetType(), xmlResource);
            var r = parser.Parse(xml);
            return r;
        }

        [Fact]
        public void MoreThan1CoreExists() {
            var r = ParseFromResults("Resources.responseWithStatus.xml");
            Assert.NotEqual(0, r.Count);
            Assert.NotEqual(1, r.Count);
            Assert.NotEqual(3, r.Count);
            Assert.NotEqual(4, r.Count);
            Assert.NotEqual(5, r.Count);
        }

        [Fact]
        public void CoresExists() {
            var r = ParseFromResults("Resources.responseWithStatus.xml");
            Assert.Equal(2, r.Count);
        }

        [Fact]
        public void Core1Exists() {
            var r = ParseFromResults("Resources.responseWithStatus.xml");
            var core = r[0];
            Assert.Equal("core1", core.Name);
            Assert.Equal(@"C:\Projects\FDO\Main\Common\solr\core1\", core.InstanceDir);
            Assert.Equal(@"C:\Projects\FDO\Main\Common\solr\core1\data\", core.DataDir);
            Assert.Equal(DateTime.Parse("2010-07-16T10:01:05.854Z"), core.StartTime);
            Assert.Equal((long) 25133036, core.Uptime);
        }

        [Fact]
        public void Core1IndexExists() {
            var r = ParseFromResults("Resources.responseWithStatus.xml");
            var coreIndex = r[0].Index;
            Assert.Equal(3604, coreIndex.SearchableDocumentCount);
            Assert.Equal(3604, coreIndex.TotalDocumentCount);
            Assert.Equal((long) 1276170296452, coreIndex.Version);
            Assert.True(coreIndex.IsOptimized);
            Assert.True(coreIndex.IsCurrent);
            Assert.False(coreIndex.HasDeletions);
            Assert.Equal(@"org.apache.lucene.store.SimpleFSDirectory:org.apache.lucene.store.SimpleFSDirectory@C:\Projects\FDO\Main\Common\solr\core1\data\index", coreIndex.Directory);
            Assert.Equal(DateTime.Parse("2010-07-09T08:42:11.593Z"), coreIndex.LastModified);
        }

        [Fact]
        public void Core0Exists() {
            var r = ParseFromResults("Resources.responseWithStatus.xml");
            var core = r[1];
            Assert.Equal("core0", core.Name);
            Assert.Equal(@"C:\Projects\FDO\Main\Common\solr\core0\", core.InstanceDir);
            Assert.Equal(@"C:\Projects\FDO\Main\Common\solr\core0\data\", core.DataDir);
            Assert.Equal(DateTime.Parse("2010-07-16T16:48:15.674Z"), core.StartTime);
            Assert.Equal((long) 703217, core.Uptime);
        }

        [Fact]
        public void Core0IndexExists() {
            var r = ParseFromResults("Resources.responseWithStatus.xml");
            var coreIndex = r[1].Index;
            Assert.Equal(3610, coreIndex.SearchableDocumentCount);
            Assert.Equal(3613, coreIndex.TotalDocumentCount);
            Assert.Equal((long) 1276170296081, coreIndex.Version);
            Assert.False(coreIndex.IsOptimized);
            Assert.True(coreIndex.IsCurrent);
            Assert.True(coreIndex.HasDeletions);
            Assert.Equal(@"org.apache.lucene.store.SimpleFSDirectory:org.apache.lucene.store.SimpleFSDirectory@C:\Projects\FDO\Main\Common\solr\core0\data\index", coreIndex.Directory);
            Assert.Equal(DateTime.Parse("2010-06-10T13:30:30Z"), coreIndex.LastModified);
        }

        [Fact]
        public void IsDefaultCore() {
            var r = ParseFromResults("Resources.responseWithStatus2.xml");
            Assert.Single(r);
            var core = r[0];
            Assert.True(core.IsDefaultCore);
        }
    }
}