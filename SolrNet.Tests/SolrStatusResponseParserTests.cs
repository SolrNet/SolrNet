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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;
using Castle.Core;
using MbUnit.Framework;
using SolrNet.Attributes;
using SolrNet.Impl;
using SolrNet.Impl.DocumentPropertyVisitors;
using SolrNet.Impl.FieldParsers;
using SolrNet.Impl.ResponseParsers;
using SolrNet.Mapping;
using SolrNet.Tests.Utils;
using Castle.Facilities.SolrNetIntegration;

namespace SolrNet.Tests
{
    [TestFixture]
    public partial class SolrStatusResponseParserTests
    {
        [Test]
        public void ParseDocument() {
            var parser = new SolrStatusResponseParser();
            var xml = EmbeddedResource.GetEmbeddedXml( GetType(), "Resources.responseWithStatus.xml" );
            var r = parser.Parse( xml );
            Assert.IsNotNull( r );
            Assert.IsTrue( r.Count > 0 );
        }

        private List<CoreResult> ParseFromResults( string xmlResource ) {
            var parser = new SolrStatusResponseParser();
            var xml = EmbeddedResource.GetEmbeddedXml( GetType(), xmlResource );
            var r = parser.Parse( xml );
            return r;
        }

        [Test]
        public void MoreThan1CoreExists() {
            var r = ParseFromResults( "Resources.responseWithStatus.xml" );
            Assert.AreNotEqual( 0, r.Count );
            Assert.AreNotEqual( 1, r.Count );
            Assert.AreNotEqual( 3, r.Count );
            Assert.AreNotEqual( 4, r.Count );
            Assert.AreNotEqual( 5, r.Count );
        }

        [Test]
        public void CoresExists() {
            var r = ParseFromResults( "Resources.responseWithStatus.xml" );
            Assert.AreEqual( 2, r.Count );
        }

        [Test]
        public void Core1Exists() {
            var r = ParseFromResults( "Resources.responseWithStatus.xml" );
            var core = r[0];
            Assert.AreEqual( "core1", core.Name );
            Assert.AreEqual( @"C:\Projects\FDO\Main\Common\solr\core1\", core.InstanceDir );
            Assert.AreEqual( @"C:\Projects\FDO\Main\Common\solr\core1\data\", core.DataDir );
            Assert.AreEqual( DateTime.Parse( "2010-07-16T10:01:05.854Z" ), core.StartTime );
            Assert.AreEqual( ( long )25133036, core.Uptime );
        }

        [Test]
        public void Core1IndexExists() {
            var r = ParseFromResults( "Resources.responseWithStatus.xml" );
            var coreIndex = r[0].Index;
            Assert.AreEqual( 3604, coreIndex.SearchableDocumentCount );
            Assert.AreEqual( 3604, coreIndex.TotalDocumentCount );
            Assert.AreEqual( ( long )1276170296452, coreIndex.Version );
            Assert.IsTrue( coreIndex.IsOptimized );
            Assert.IsTrue( coreIndex.IsCurrent );
            Assert.IsFalse( coreIndex.HasDeletions );
            Assert.AreEqual( @"org.apache.lucene.store.SimpleFSDirectory:org.apache.lucene.store.SimpleFSDirectory@C:\Projects\FDO\Main\Common\solr\core1\data\index", coreIndex.Directory );
            Assert.AreEqual( DateTime.Parse( "2010-07-09T08:42:11.593Z" ), coreIndex.LastModified );
        }

        [Test]
        public void Core0Exists() {
            var r = ParseFromResults( "Resources.responseWithStatus.xml" );
            var core = r[1];
            Assert.AreEqual( "core0", core.Name );
            Assert.AreEqual( @"C:\Projects\FDO\Main\Common\solr\core0\", core.InstanceDir );
            Assert.AreEqual( @"C:\Projects\FDO\Main\Common\solr\core0\data\", core.DataDir );
            Assert.AreEqual( DateTime.Parse( "2010-07-16T16:48:15.674Z" ), core.StartTime );
            Assert.AreEqual( ( long )703217, core.Uptime );
        }

        [Test]
        public void Core0IndexExists() {
            var r = ParseFromResults( "Resources.responseWithStatus.xml" );
            var coreIndex = r[1].Index;
            Assert.AreEqual( 3610, coreIndex.SearchableDocumentCount );
            Assert.AreEqual( 3613, coreIndex.TotalDocumentCount );
            Assert.AreEqual( ( long )1276170296081, coreIndex.Version );
            Assert.IsFalse( coreIndex.IsOptimized );
            Assert.IsTrue( coreIndex.IsCurrent );
            Assert.IsTrue( coreIndex.HasDeletions );
            Assert.AreEqual( @"org.apache.lucene.store.SimpleFSDirectory:org.apache.lucene.store.SimpleFSDirectory@C:\Projects\FDO\Main\Common\solr\core0\data\index", coreIndex.Directory );
            Assert.AreEqual( DateTime.Parse( "2010-06-10T13:30:30Z" ), coreIndex.LastModified );
        }
    }
}
