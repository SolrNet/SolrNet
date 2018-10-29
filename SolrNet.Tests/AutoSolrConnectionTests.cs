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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HttpWebAdapters;
using Xunit;
using Moroco;
using SolrNet.Exceptions;
using SolrNet.Impl;
using System.Xml.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace SolrNet.Tests
{

    public class AutoSolrConnectionTests
    {
        private const string solrURL = "http://localhost:8983/solr";

        [Trait("Category", "Integration")]
        [Fact()]
        public void SimpleGet()
        {
            var p = new Dictionary<string, string>();
            p["q"] = "*";
            p["rows"] = "1";
            var conn = new AutoSolrConnection(solrURL);
            var response = conn.Get("/select/", p);
            var xdoc = XDocument.Parse(response);
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
            Assert.True(int.Parse((string)xdoc.Root.Element("result").Attribute("numFound")) > 1);
            Assert.Single(xdoc.Root.Element("result").Elements("doc"));
        }

        [Trait("Category", "Integration")]
        [Fact()]
        public async Task SimpleGetAsync()
        {
            var p = new Dictionary<string, string>();
            p["q"] = "*";
            p["rows"] = "1";
            var conn = new AutoSolrConnection(solrURL);
            var response = await conn.GetAsync("/select/", p);
            var xdoc = XDocument.Parse(response);
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
            Assert.True(int.Parse((string)xdoc.Root.Element("result").Attribute("numFound")) > 1);
            Assert.Single(xdoc.Root.Element("result").Elements("doc"));
        }

        [Trait("Category", "Integration")]
        [Fact()]
        public async Task GetAsyncAutoPost()
        {
            var p = new Dictionary<string, string>();
            p["q"] = "*";
            p["rows"] = "1";
            p["test"] = string.Join("", Enumerable.Range(0, 9000).Select(a => "a"));
            var conn = new AutoSolrConnection(solrURL);
            var response = await conn.GetAsync("/select/", p);
            var xdoc = XDocument.Parse(response);
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
            Assert.True(int.Parse((string)xdoc.Root.Element("result").Attribute("numFound")) > 1);
            Assert.Single(xdoc.Root.Element("result").Elements("doc"));
        }


        [Trait("Category", "Integration")]
        [Fact()]
        public async Task GetStreamAsyncAutoPost()
        {
            var p = new Dictionary<string, string>();
            p["q"] = "*";
            p["rows"] = "1";
            p["test"] = string.Join("", Enumerable.Range(0, 9000).Select(a => "a"));
            var conn = new AutoSolrConnection(solrURL);
            XDocument xdoc;
            using (var response = await conn.GetAsStreamAsync("/select/", p, default(CancellationToken)))
                xdoc = XDocument.Load(response);

            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
            Assert.True(int.Parse((string)xdoc.Root.Element("result").Attribute("numFound")) > 1);
            Assert.Single(xdoc.Root.Element("result").Elements("doc"));
        }

        [Trait("Category", "Integration")]
        [Fact()]
        public async Task GetAsyncAutoPostWithStream()
        {
            //No mocking yet of HTTPClient, so checked with Fiddler if indeed POST-ed
            var p = new Dictionary<string, string>();
            p["q"] = "*";
            p["rows"] = "1";
            p["test"] = string.Join("", Enumerable.Range(0, 9000).Select(a => "a"));
            var conn = new AutoSolrConnection(solrURL);
            XDocument xdoc;
            using (var response = await conn.GetAsStreamAsync("/select/", p, CancellationToken.None))
            {
                xdoc = XDocument.Load(response);
            }
            Assert.Equal("0", xdoc.Root.Element("lst").Elements("int").First(el => (string)el.Attribute("name") == "status").Value);
            Assert.True(int.Parse((string)xdoc.Root.Element("result").Attribute("numFound")) > 1);
            Assert.Single(xdoc.Root.Element("result").Elements("doc"));
        }

        [Trait("Category", "Integration")]
        [Fact()]
        public async Task GetAsyncWithCancelledToken()
        {
            var p = new Dictionary<string, string>();
            p["q"] = "*";
            p["rows"] = "1";
            var conn = new AutoSolrConnection(solrURL);
            var tokenSource = new CancellationTokenSource(1);

            await Assert.ThrowsAsync<TaskCanceledException>(() => conn.GetAsStreamAsync("/select/", p, tokenSource.Token));
            
        }
    }
}
