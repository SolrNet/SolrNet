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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SolrNet.Tests
{
    public class MockConnection : ISolrConnection
    {
        private readonly ICollection<KeyValuePair<string, string>> expectations;
        private const string response =
            @"<?xml version=""1.0"" encoding=""UTF-8""?>
<response>
<lst name=""responseHeader""><int name=""status"">0</int><int name=""QTime"">0</int><lst name=""params""><str name=""q"">id:123456</str><str name=""?""/><str name=""version"">2.2</str></lst></lst><result name=""response"" numFound=""1"" start=""0""><doc></doc></result>
</response>
";

        public MockConnection() { }

        public MockConnection(ICollection<KeyValuePair<string, string>> expectations)
        {
            this.expectations = expectations;
        }

        public virtual string ServerURL { get; set; }

        public virtual string Version { get; set; }

        public virtual Encoding XmlEncoding { get; set; }

        public virtual string Post(string relativeUrl, string s)
        {
            return string.Empty;
        }

        public virtual Task<string> PostAsync (string relativeUrl, string s)
        {
            return Task.FromResult(Post(relativeUrl, s));
        }

        public virtual string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return string.Empty;
        }

        public virtual Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return Task.FromResult(PostStream(relativeUrl, contentType, content, parameters));
        }


        public string DumpParams(List<KeyValuePair<string, string>> parameters)
        {
            return string.Join("\n", parameters.ConvertAll(kv => string.Format("{0}={1}", kv.Key, kv.Value)).ToArray());
        }

        public string DumpParams(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            return DumpParams(new List<KeyValuePair<string, string>>(parameters));
        }

        public virtual string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var param = new List<KeyValuePair<string, string>>(parameters);
            foreach (var e in expectations)
            {
                if (e.Key == "handler") {
                    Assert.Equal(relativeUrl, e.Value);
                    param.Add(e);
                }

            }   
            Assert.Equal(expectations.Count, param.Count);
            foreach (var p in param)
                Assert.True(expectations.Contains(p));
            return response;
        }
        public virtual Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(Get(relativeUrl, parameters));
        }

    }
}
