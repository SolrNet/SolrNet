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
using HttpWebAdapters.Adapters;
using SolrNet.Exceptions;
using SolrNet.Utils;
using System.Threading.Tasks;
using System.Threading;

namespace SolrNet.Impl
{
    /// <summary>
    /// Manages HTTP connection with Solr
    /// </summary>
    public class SolrConnection : ISolrConnection
    {
        private string serverURL;
        private string version = "2.2";

        /// <summary>
        /// HTTP cache implementation
        /// </summary>
        public ISolrCache Cache { get; set; }

        /// <summary>
        /// HTTP request factory
        /// </summary>
        public IHttpWebRequestFactory HttpWebRequestFactory { get; set; }

        /// <summary>
        /// Manages HTTP connection with Solr
        /// </summary>
        /// <param name="serverURL">URL to Solr</param>
        public SolrConnection(string serverURL) : this(serverURL, new HttpWebRequestFactory())
        {
        }

        /// <summary>
        /// Manages HTTP connection with Solr
        /// </summary>
        /// <param name="serverURL">URL to Solr</param>
        /// <param name="httpWebRequestFactory">Request factory to be used in synchronous fallback connections</param>
        public SolrConnection(string serverURL, IHttpWebRequestFactory httpWebRequestFactory)
        {
            ServerURL = serverURL;
            Timeout = -1;
            Cache = new NullCache();
            HttpWebRequestFactory = httpWebRequestFactory;
        }

        /// <summary>
        /// URL to Solr
        /// </summary>
        public string ServerURL
        {
            get { return serverURL; }
            set
            {
                serverURL = UriValidator.ValidateHTTP(value);
            }
        }

        /// <summary>
        /// Solr XML response syntax version
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// HTTP connection timeout
        /// </summary>
        public int Timeout { get; set; }

        /// <inheritdoc />
        public string Post(string relativeUrl, string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var content = new MemoryStream(bytes))
                return PostStream(relativeUrl, "text/xml; charset=utf-8", content, null);
        }

        /// <inheritdoc />
        public async Task<string> PostAsync(string relativeUrl, string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var content = new MemoryStream(bytes))
                return await PostStreamAsync(relativeUrl, "text/xml; charset=utf-8", content, null);
        }
        private IHttpWebRequest PreparePostStreamWebRequest(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var u = new UriBuilder(serverURL);
            u.Path += relativeUrl;
            u.Query = GetQuery(parameters);

            var request = HttpWebRequestFactory.Create(u.Uri);
            request.Method = HttpWebRequestMethod.POST;
            request.KeepAlive = true;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            if (Timeout > 0)
            {
                request.ReadWriteTimeout = Timeout;
                request.Timeout = Timeout;
            }
            if (contentType != null)
                request.ContentType = contentType;

            request.ContentLength = content.Length;
            request.ProtocolVersion = HttpVersion.Version11;
            return request;
        }

        /// <inheritdoc />
        public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> parameters)
        {

            var request = PreparePostStreamWebRequest(relativeUrl, contentType, content, parameters);

            try
            {
                using (var postStream = request.GetRequestStream())
                {
                    CopyTo(content, postStream);
                }
                return GetResponse(request).Data;
            }
            catch (WebException e)
            {
                var msg = e.Message;
                if (e.Response != null)
                {
                    using (var s = e.Response.GetResponseStream())
                    using (var sr = new StreamReader(s))
                        msg = sr.ReadToEnd();
                }
                throw new SolrConnectionException(msg, e, request.RequestUri.ToString());
            }
        }

        /// <inheritdoc />
        public async Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var request = PreparePostStreamWebRequest(relativeUrl, contentType, content, parameters);

            try
            {
                using (var postStream = await Task.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null))
                {
                    await content.CopyToAsync(postStream);
                }
                return (await GetResponseAsync(request)).Data;
            }
            catch (WebException e)
            {
                var msg = e.Message;
                if (e.Response != null)
                {
                    using (var s = e.Response.GetResponseStream())
                    using (var sr = new StreamReader(s))
                        msg = await sr.ReadToEndAsync();
                }
                throw new SolrConnectionException(msg, e, request.RequestUri.ToString());
            }
        }

        private static void CopyTo(Stream input, Stream output)
        {
            byte[] buffer = new byte[0x1000];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                output.Write(buffer, 0, read);
        }

        private (IHttpWebRequest request, UriBuilder uri, SolrCacheEntity cache) PrepareGetWebRequest(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var u = new UriBuilder(serverURL);
            u.Path += relativeUrl;
            u.Query = GetQuery(parameters);

            var request = HttpWebRequestFactory.Create(u.Uri);
            request.Method = HttpWebRequestMethod.GET;
            request.KeepAlive = true;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var cached = Cache[u.Uri.ToString()];
            if (cached != null)
            {
                request.Headers.Add(HttpRequestHeader.IfNoneMatch, cached.ETag);
            }
            if (Timeout > 0)
            {
                request.ReadWriteTimeout = Timeout;
                request.Timeout = Timeout;
            }
            return (request, u,cached);
        }
        
        /// <inheritdoc />
        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var wr  = PrepareGetWebRequest(relativeUrl, parameters);

            try
            {
                var response = GetResponse(wr.request);
                if (response.ETag != null)
                    Cache.Add(new SolrCacheEntity(wr.uri.Uri.ToString(), response.ETag, response.Data));
                return response.Data;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (e.Response)
                    {
                        var r = new HttpWebResponseAdapter(e.Response);
                        if (r.StatusCode == HttpStatusCode.NotModified)
                        {
                            return wr.cache.Data;
                        }
                        using (var s = e.Response.GetResponseStream())
                        using (var sr = new StreamReader(s))
                        {
                            throw new SolrConnectionException(sr.ReadToEnd(), e, wr.uri.Uri.ToString());
                        }
                    }
                }
                throw new SolrConnectionException(e, wr.uri.Uri.ToString());
            }
        }

        /// <inheritdoc />
        public async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            var wr = PrepareGetWebRequest(relativeUrl, parameters);

            try
            {
                var response = await GetResponseAsync(wr.request);
                if (response.ETag != null)
                    Cache.Add(new SolrCacheEntity(wr.uri.Uri.ToString(), response.ETag, response.Data));
                return response.Data;
            }
            catch (WebException e)
            {
                if (e.Response != null)
                {
                    using (e.Response)
                    {
                        var r = new HttpWebResponseAdapter(e.Response);
                        if (r.StatusCode == HttpStatusCode.NotModified)
                        {
                            return wr.cache.Data;
                        }
                        using (var s = e.Response.GetResponseStream())
                        using (var sr = new StreamReader(s))
                        {
                            throw new SolrConnectionException(await sr.ReadToEndAsync(), e, wr.uri.Uri.ToString());
                        }
                    }
                }
                throw new SolrConnectionException(e, wr.uri.Uri.ToString());
            }
        }

        /// <summary>
        /// Gets the Query 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GetQuery(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var param = new List<KeyValuePair<string, string>>();
            if (parameters != null)
                param.AddRange(parameters);

            param.Add(KV.Create("version", version));

            if (param.All(x => x.Key != "wt"))
            {
                // only set wt=xml if wt wasn't already set by the caller
                param.Add(KV.Create("wt", "xml"));
            }

            return string.Join("&", param
                .Select(kv => KV.Create(WebUtility.UrlEncode(kv.Key), WebUtility.UrlEncode(kv.Value)))
                .Select(kv => string.Format("{0}={1}", kv.Key, kv.Value))
                .ToArray());
        }

        /// <summary>
        /// Gets http response, returns (etag, data)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private SolrResponse GetResponse(IHttpWebRequest request)
        {
            using (var response = request.GetResponse())
            {
                var etag = response.Headers[HttpResponseHeader.ETag];
                var cacheControl = response.Headers[HttpResponseHeader.CacheControl];
                if (cacheControl != null && cacheControl.Contains("no-cache"))
                    etag = null; // avoid caching things marked as no-cache

                return new SolrResponse(etag, ReadResponseToString(response));
            }
        }

        private async Task<SolrResponse> GetResponseAsync(IHttpWebRequest request)
        {
            using (var response = await Task.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null))
            {
                var etag = response.Headers[HttpResponseHeader.ETag];
                var cacheControl = response.Headers[HttpResponseHeader.CacheControl];
                if (cacheControl != null && cacheControl.Contains("no-cache"))
                    etag = null; // avoid caching things marked as no-cache

                return new SolrResponse(etag, await ReadResponseToStringAsync(response));
            }
        }

        /// <summary>
        /// Reads the full stream from the response and returns the content as stream,
        /// using the correct encoding.
        /// </summary>
        /// <param name="response">Web response from request to Solr</param>
        private string ReadResponseToString(IHttpWebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream, TryGetEncoding(response)))
            {
                return reader.ReadToEnd();
            }
        }

        private async Task<string> ReadResponseToStringAsync(IHttpWebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream, TryGetEncoding(response)))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private struct SolrResponse
        {
            public string ETag { get; private set; }
            public string Data { get; private set; }
            public SolrResponse(string eTag, string data) : this()
            {
                ETag = eTag;
                Data = data;
            }
        }

        private Encoding TryGetEncoding(IHttpWebResponse response)
        {
            try
            {
                return Encoding.GetEncoding(response.CharacterSet);
            }
            catch
            {
                return Encoding.UTF8;
            }
        }
    }
}
