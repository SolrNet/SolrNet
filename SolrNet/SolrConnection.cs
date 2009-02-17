#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
using System.Net;
using System.Text;
using System.Web;
using HttpWebAdapters;
using HttpWebAdapters.Adapters;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet {
    /// <summary>
    /// Manages HTTP connection with Solr
    /// </summary>
    public class SolrConnection : ISolrConnection {
        private readonly IHttpWebRequestFactory httpWebRequestFactory = new HttpWebRequestFactory();
        private string serverURL;
        private string version = "2.2";

        public SolrConnection(string serverURL) {
            ServerURL = serverURL;
            Timeout = -1;
        }

        public SolrConnection(string serverURL, IHttpWebRequestFactory httpWebRequestFactory) : this(serverURL) {
            this.httpWebRequestFactory = httpWebRequestFactory;
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
            var request = httpWebRequestFactory.Create(u.Uri);
            request.Method = HttpWebRequestMethod.POST;
            request.KeepAlive = false;
            if (Timeout > 0)
                request.Timeout = Timeout;
            request.ContentType = "text/xml; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(s);
            request.ContentLength = bytes.Length;
            request.ProtocolVersion = HttpVersion.Version10;
            try {
                using (var postParams = request.GetRequestStream())
                using (var sw = new StreamWriter(postParams)) {
                    sw.Write(s);
                }
                return GetResponse(request);
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
            // TODO clean up, too messy
            u.Query = Func.Reduce(
                Func.Map(parameters, input => string.Format("{0}={1}", HttpUtility.UrlEncode(input.Key),
                                                            HttpUtility.UrlEncode(input.Value))), "?",
                (x, y) => string.Format("{0}&{1}", x, y));
            var request = httpWebRequestFactory.Create(u.Uri);
            request.Method = HttpWebRequestMethod.GET;
            request.KeepAlive = false;
            if (Timeout > 0)
                request.Timeout = Timeout;
            request.ProtocolVersion = HttpVersion.Version10; // for some reason Version11 throws
            try {
                return GetResponse(request);
            } catch (WebException e) {
                if (e.Response != null) {
                    var r = new HttpWebResponseAdapter(e.Response);
                    if (r.StatusCode == HttpStatusCode.BadRequest) {
                        throw new InvalidFieldException(r.StatusDescription, e);
                    }
                }
                throw new SolrConnectionException(e);
            }
        }

        private string GetResponse(IHttpWebRequest request) {
            using (var response = request.GetResponse())
            using (var rStream = response.GetResponseStream())
            using (var sr = new StreamReader(rStream, TryGetEncoding(response))) {
                return sr.ReadToEnd();
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