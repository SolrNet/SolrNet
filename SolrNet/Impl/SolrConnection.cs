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
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HttpWebAdapters;
using HttpWebAdapters.Adapters;
using SolrNet.Exceptions;

namespace SolrNet.Impl {
    /// <summary>
    /// Manages HTTP connection with Solr
    /// </summary>
    public class SolrConnection : ISolrConnection {
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

        public SolrConnection(string serverURL) {
            ServerURL = serverURL;
            Timeout = -1;
            Cache = new NullCache();
            HttpWebRequestFactory = new HttpWebRequestFactory();
        }

        public string ServerURL {
            get { return serverURL; }
            set {
                try {
                    var u = new Uri(value);
                    if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
                        throw new InvalidURLException("Only HTTP or HTTPS protocols are supported");
                } catch (ArgumentException e) {
                    throw new InvalidURLException(e);
                } catch (UriFormatException e) {
                    throw new InvalidURLException(e);
                }
                serverURL = value;
            }
        }

        /// <summary>
        /// Solr XML response syntax version
        /// </summary>
        public string Version {
            get { return version; }
            set { version = value; }
        }

        /// <summary>
        /// HTTP connection timeout
        /// </summary>
        public int Timeout { get; set; }

        public string Post(string relativeUrl, string s) {
            var u = new UriBuilder(serverURL);
            u.Path += relativeUrl;
            var request = HttpWebRequestFactory.Create(u.Uri);
            request.Method = HttpWebRequestMethod.POST;
            request.KeepAlive = true;
            if (Timeout > 0) {
                request.ReadWriteTimeout = Timeout;
                request.Timeout = Timeout;                
            }
            request.ContentType = "text/xml; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(s);
            request.ContentLength = bytes.Length;
            request.ProtocolVersion = HttpVersion.Version11;
            try {
                using (var postParams = request.GetRequestStream())
                using (var sw = new StreamWriter(postParams)) {
                    sw.Write(s);
                }
                return GetResponse(request).Data;
            }
            catch (WebException e) {
                throw new SolrConnectionException(e);
            }
        }

        public KeyValuePair<T1, T2> KVP<T1, T2>(T1 a, T2 b) {
            return new KeyValuePair<T1, T2>(a, b);
        }

        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) {
            var u = new UriBuilder(serverURL);
            u.Path += relativeUrl;
            var param = new List<KeyValuePair<string, string>>();
            if (parameters != null)
                param.AddRange(parameters);
            param.Add(KVP("version", version));
            u.Query = string.Join("&", param
                .Select(kv => KVP(HttpUtility.UrlEncode(kv.Key), HttpUtility.UrlEncode(kv.Value)))
                .Select(kv => string.Format("{0}={1}", kv.Key, kv.Value))
                .ToArray());
            var request = HttpWebRequestFactory.Create(u.Uri);
            request.Method = HttpWebRequestMethod.GET;
            request.KeepAlive = true;

            //Issue request headers to say we can accept a compressed response
            request.Headers.Add(HttpRequestHeader.AcceptEncoding, "gzip,deflate");

            var cached = Cache[u.Uri.ToString()];
            if (cached != null) {
                request.Headers.Add(HttpRequestHeader.IfNoneMatch, cached.ETag);
            }
            if (Timeout > 0) {
                request.ReadWriteTimeout = Timeout;
                request.Timeout = Timeout;                
            }
            try {
                var response = GetResponse(request);
                if (response.ETag != null)
                    Cache.Add(new SolrCacheEntity(u.Uri.ToString(), response.ETag, response.Data));
                return response.Data;
            } catch (WebException e) {
                if (e.Response != null) {
                    var r = new HttpWebResponseAdapter(e.Response);
                    if (r.StatusCode == HttpStatusCode.NotModified) {
                        return cached.Data;
                    }
                    if (r.StatusCode == HttpStatusCode.BadRequest) {
                        throw new InvalidFieldException(r.StatusDescription, e);
                    }
                }
                throw new SolrConnectionException(e);
            }
        }

        /// <summary>
        /// Gets http response, returns (etag, data)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private SolrResponse GetResponse(IHttpWebRequest request) {
            using (var response = request.GetResponse()) {
                var etag = response.Headers[HttpResponseHeader.ETag];
                var cacheControl = response.Headers[HttpResponseHeader.CacheControl];
                if (cacheControl != null && cacheControl.Contains("no-cache"))
                    etag = null; // avoid caching things marked as no-cache

                return new SolrResponse(etag, DeflateResponse(response));
            }
        }
        /// <summary>
        /// Attempts to deflate response stream if compressed in any way.
        /// </summary>
        /// <see cref="http://west-wind.com/weblog/posts/102969.aspx"/>
        /// <param name="response">Web response from request to Solr</param>
        /// <returns></returns>
        private string DeflateResponse(IHttpWebResponse response)
        {
            Stream responseStream = response.GetResponseStream();

            if (response.ContentEncoding != null)
            {
                if (response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
                }
                else if (response.ContentEncoding.ToLower().Contains("deflate"))
                {
                    responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);
                }
            }

            if (responseStream == null)
            {
                return string.Empty;
            }

            var reader = new StreamReader(responseStream, TryGetEncoding(response));

            string rawResponse = reader.ReadToEnd();

            response.Close();
            responseStream.Close();

            return rawResponse;         
        }

        private struct SolrResponse {
            public string ETag { get; private set; }
            public string Data { get; private set; }
            public SolrResponse(string eTag, string data) : this() {
                ETag = eTag;
                Data = data;
            }
        }

        private Encoding TryGetEncoding(IHttpWebResponse response) {
            try {
                return Encoding.GetEncoding(response.CharacterSet);
            } catch {
                return Encoding.UTF8;
            }
        }
    }
}