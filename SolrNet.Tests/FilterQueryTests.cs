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

using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.Mocks;
using SolrNet.Commands.Parameters;
using SolrNet.Impl;

namespace SolrNet.Tests {
    [TestFixture]
    public class FilterQueryTests {
        [Test]
        public void FilterQueries() {
            var conn = new MockConnection(new Dictionary<string, string> {
                {"fq", "id:0"},
                {"q", "*:*"},
                {"rows", "100000000"},
            });
            var mocks = new MockRepository();
            var queryExec = mocks.StrictMock<ISolrQueryExecuter<Document>>();
            var docSerializer = mocks.StrictMock<ISolrDocumentSerializer<Document>>();
            ISolrBasicReadOnlyOperations<Document> solr = new SolrBasicServer<Document>(conn, queryExec, docSerializer, null, null, null, null, null);
            solr.Query(SolrQuery.All, new QueryOptions {
                FilterQueries = new[] {new SolrQuery("id:0")},
            });
        }

        public class Document {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}