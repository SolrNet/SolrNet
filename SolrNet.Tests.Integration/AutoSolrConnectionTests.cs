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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using SolrNet.Impl;
using Xunit;

namespace SolrNet.Tests.Integration
{

    [Trait("Category","Integration")]
    [TestCaseOrderer(MethodDefTestCaseOrderer.Type, MethodDefTestCaseOrderer.Assembly)]
    public class AutoSolrConnectionTests
    {
        private const string solrURL = "http://localhost:8983/solr/techproducts";

        [Fact]
        public void SimpleGet()
        {
            var p = new Dictionary<string, string>();
            p["wt"] = "xml";
            p["q"] = "*";
            p["rows"] = "1";
            var conn = new AutoSolrConnection(solrURL);
            var response = conn.Get("/select/", p);
            var xdoc = XDocument.Parse(response);
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
        }

        [Fact]
        public async Task SimpleGetAsync()
        {
            var p = new Dictionary<string, string>();
            p["wt"] = "xml";
            p["q"] = "*";
            p["rows"] = "1";
            var conn = new AutoSolrConnection(solrURL);
            var response = await conn.GetAsync("/select/", p);
            var xdoc = XDocument.Parse(response);
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
        }

        [Fact]
        public async Task GetAsyncAutoPost()
        {
            var p = new Dictionary<string, string>();
            p["wt"] = "xml";
            p["q"] = "*";
            p["rows"] = "1";
            p["test"] = string.Join("", Enumerable.Range(0, 9000).Select(a => Guid.NewGuid().ToString()));
            var conn = new AutoSolrConnection(solrURL);
            var response = await conn.GetAsync("/select/", p);
            var xdoc = XDocument.Parse(response);
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
        }
        
        [Fact]
        public async Task GetStreamAsyncAutoPost()
        {
            var p = new Dictionary<string, string>();
            p["wt"] = "xml";
            p["q"] = "*";
            p["rows"] = "1";
            p["test"] = string.Join("", Enumerable.Range(0, 9000).Select(a => Guid.NewGuid().ToString()));
            var conn = new AutoSolrConnection(solrURL);
            XDocument xdoc;
            using (var response = await conn.GetAsStreamAsync("/select/", p, default(CancellationToken)))
                xdoc = XDocument.Load(response);

            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
        }

        [Fact]
        public async Task GetAsyncAutoPostWithStream()
        {
            //No mocking yet of HTTPClient, so checked with Fiddler if indeed POST-ed
            var p = new Dictionary<string, string>();
            p["wt"] = "xml";
            p["q"] = "*";
            p["rows"] = "1";
            p["test"] = string.Join("", Enumerable.Range(0, 9000).Select(a => Guid.NewGuid().ToString()));
            var conn = new AutoSolrConnection(solrURL);
            XDocument xdoc;
            using (var response = await conn.GetAsStreamAsync("/select/", p, CancellationToken.None))
            {
                xdoc = XDocument.Load(response);
            }
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
        }

        [Fact]
        public async Task GetAsyncWithCancelledToken()
        {
            var p = new Dictionary<string, string>();
            p["q"] = "*";
            p["rows"] = "1";
            var conn = new AutoSolrConnection(solrURL);
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();

            await Assert.ThrowsAsync<TaskCanceledException>(() => conn.GetAsStreamAsync("/select/", p, tokenSource.Token));
            
        }
    }
}
