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
using System.Threading;
using System.Threading.Tasks;

namespace SolrNet.Tests.Utils {
    public class LoggingConnection : ISolrConnection {
        private readonly ISolrConnection conn;

        public LoggingConnection(ISolrConnection conn) {
            this.conn = conn;
        }

        public string Post(string relativeUrl, string s) {
            Console.WriteLine("Posting {0}", s);
            return conn.Post(relativeUrl, s);
        }

        public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> parameters) {
            Console.WriteLine("Posting Binary");
            return conn.PostStream(relativeUrl, contentType, content, parameters);
        }

        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            Console.WriteLine("Getting");
            var r = conn.Get(relativeUrl, parameters);
            Console.WriteLine("Result is:\n" + r);
            return r;
        }

        public virtual Task<string> PostAsync(string relativeUrl, string s)
        {
            Console.WriteLine("Posting Async {0}", s);
            return conn.PostAsync(relativeUrl, s);
        }

        public virtual Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            Console.WriteLine("Posting Binary Async");
            return conn.PostStreamAsync(relativeUrl, contentType, content, parameters);
        }

        public virtual  async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine("Getting Async");
            var r =  await conn.GetAsync(relativeUrl, parameters, cancellationToken);
            Console.WriteLine("Result is:\n" + r);
            return r;
        }
    }
}
