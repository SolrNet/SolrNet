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
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SolrNet {
    /// <summary>
    /// Manages HTTP connection with Solr
    /// </summary>
    public interface ISolrConnection {
        /// <summary>
        /// POSTs to Solr
        /// </summary>
        /// <param name="relativeUrl">Path to post to</param>
        /// <param name="s">POST content</param>
        /// <returns></returns>
        string Post(string relativeUrl, string s);

        /// <summary>
        /// POSTs to Solr asynchronously
        /// </summary>
        /// <param name="relativeUrl">Path to post to</param>
        /// <param name="s">POST content</param>
        /// <returns></returns>
        Task<string> PostAsync(string relativeUrl, string s);

        /// <summary>
        /// POSTs binary to Solr
        /// </summary>
        /// <param name="relativeUrl">Path to post to</param>
        /// <param name="contentType">Request content type (optional)</param>
        /// <param name="content">Binary content</param>
        /// <param name="getParameters">extra parameters to pass in query string</param>
        /// <returns></returns>
        string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters);

        /// <summary>
        /// POSTs binary to Solr asynchronously
        /// </summary>
        /// <param name="relativeUrl">Path to post to</param>
        /// <param name="contentType">Request content type (optional)</param>
        /// <param name="content">Binary content</param>
        /// <param name="getParameters">extra parameters to pass in query string</param>
        /// <returns></returns>
        Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters);

        /// <summary>
        /// GETs from Solr
        /// </summary>
        /// <param name="relativeUrl">Path to get from</param>
        /// <param name="parameters">Query string parameters</param>
        /// <returns></returns>
        string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters);

        /// <summary>
        /// GETs from Solr asynchronously
        /// </summary>
        /// <param name="relativeUrl">Path to get from</param>
        /// <param name="parameters">Query string parameters</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken= default(CancellationToken));
    }

    public interface IStreamSolrConnection : ISolrConnection
    {
        Task<Stream> PostStreamAsStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters, CancellationToken cancellationToken);
        Task<Stream> GetAsStreamAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken);

    }
}
