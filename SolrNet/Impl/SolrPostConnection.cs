﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SolrNet.Exceptions;
using SolrNet.Utils;
using System.Threading.Tasks;

namespace SolrNet.Impl
{
    /// <summary>
    /// Manages HTTP connection with Solr, uses POST request instead of GET in order to handle large requests
    /// </summary>
    public class PostSolrConnection : ISolrConnection
    {
        private readonly ISolrConnection conn;
        private readonly string serverUrl;

        public PostSolrConnection(ISolrConnection conn, string serverUrl)
        {
            this.conn = conn;
            this.serverUrl = serverUrl;
        }

        /// <summary>
        /// URL to Solr
        /// </summary>
        public string ServerUrl => serverUrl;

        public string Post(string relativeUrl, string s)
        {
            return conn.Post(relativeUrl, s);
        }

        public Task<string> PostAsync(string relativeUrl, string s)
        {
            return conn.PostAsync(relativeUrl, s);
        }

        public (HttpWebRequest request, string queryString) PrepareGet(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, SolrRequestOptions options = null)
        {
            var u = new UriBuilder(serverUrl);
            u.Path += relativeUrl;
            var request = (HttpWebRequest)WebRequest.Create(u.Uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";            
            request = ApplyRequestOptions(request, options);

            var param = new List<KeyValuePair<string, string>>();
            if (parameters != null)
                param.AddRange(parameters);

            param.Add(KV.Create("wt", "xml"));
            var qs = string.Join("&", param
                  .Select(kv => string.Format("{0}={1}", HttpUtility.UrlEncode(kv.Key), HttpUtility.UrlEncode(kv.Value)))
                  .ToArray());

            request.ContentLength = Encoding.UTF8.GetByteCount(qs);
            request.ProtocolVersion = HttpVersion.Version11;
            request.KeepAlive = true;

            return (request, qs);
        }

        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var g = PrepareGet(relativeUrl, parameters);
            try
            {
                using (var postParams = g.request.GetRequestStream())
                using (var sw = new StreamWriter(postParams))
                    sw.Write(g.queryString);
                using (var response = g.request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var sr = new StreamReader(responseStream, Encoding.UTF8, true))
                    return sr.ReadToEnd();
            }
            catch (WebException e)
            {
                throw new SolrConnectionException(e);
            }
        }

        public async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var g = PrepareGet(relativeUrl, parameters);
            try
            {
                using (var postParams = await g.request.GetRequestStreamAsync())
                using (var sw = new StreamWriter(postParams))
                    await sw.WriteAsync(g.queryString);
                using (var response = await g.request.GetResponseAsync())
                using (var responseStream = response.GetResponseStream())
                using (var sr = new StreamReader(responseStream, Encoding.UTF8, true))
                    return await sr.ReadToEndAsync();
            }
            catch (WebException e)
            {
                throw new SolrConnectionException(e);
            }
        }


        public string PostStream(string relativeUrl, string contentType, System.IO.Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            return conn.PostStream(relativeUrl, contentType, content, getParameters);
        }

        public Task<string> PostStreamAsync(string relativeUrl, string contentType, System.IO.Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            return conn.PostStreamAsync(relativeUrl, contentType, content, getParameters);
        }


        #region private

        private HttpWebRequest ApplyRequestOptions(HttpWebRequest request, SolrRequestOptions options = null)
        {
            if (options == null) return request;
            if (options.SolrConnectionTimeout != null) request.Timeout = options.SolrConnectionTimeout();

            return request;
        }

        #endregion
    }
    
}

