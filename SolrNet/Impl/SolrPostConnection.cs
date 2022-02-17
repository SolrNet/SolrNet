using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SolrNet.Exceptions;
using SolrNet.Utils;
using System.Threading.Tasks;
using System.Threading;
using HttpWebAdapters;

namespace SolrNet.Impl
{
    /// <summary>
    /// Manages HTTP connection with Solr, uses POST request instead of GET in order to handle large requests
    /// </summary>
    public class PostSolrConnection : ISolrConnection
    {
        private readonly ISolrConnection conn;
        private readonly string serverUrl;

        /// <summary>
        /// HTTP request factory
        /// </summary>
        public IHttpWebRequestFactory HttpWebRequestFactory { get; set; }

        /// <summary>
        /// Manages HTTP connection with Solr using HTTP POST.
        /// 
        /// Note that this constructor uses <see cref="HttpWebAdapters.HttpWebRequestFactory"/> and thus doesn't support basic authentication and such
        /// Please use <see cref="PostSolrConnection(ISolrConnection, string, IHttpWebRequestFactory)"/>
        /// </summary>
        /// <param name="conn">SolrConnection instance to handle HTTP requests</param>
        /// <param name="serverUrl">URL to Solr</param>
        public PostSolrConnection(ISolrConnection conn, string serverUrl) : this(conn, serverUrl, new HttpWebRequestFactory())
        {
        }

        /// <summary>
        /// Manages HTTP connection with Solr using HTTP POST.
        /// </summary>
        /// <param name="conn">SolrConnection instance to handle HTTP requests</param>
        /// <param name="serverUrl">URL to Solr</param>
        /// <param name="httpWebRequestFactory">Request factory to be used in HTTP requests</param>
        public PostSolrConnection(ISolrConnection conn, string serverUrl, IHttpWebRequestFactory httpWebRequestFactory)
        {
            this.conn = conn;
            this.serverUrl = serverUrl;
            this.HttpWebRequestFactory = httpWebRequestFactory;
        }

        /// <summary>
        /// URL to Solr
        /// </summary>
        public string ServerUrl
        {
            get { return serverUrl; }
        }

        /// <inheritdoc />
        public string Post(string relativeUrl, string s)
        {
            return conn.Post(relativeUrl, s);
        }

        /// <inheritdoc />
        public Task<string> PostAsync(string relativeUrl, string s)
        {
            return conn.PostAsync(relativeUrl, s);
        }

        public (IHttpWebRequest request, string queryString) PrepareGet(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var u = new UriBuilder(serverUrl);
            u.Path += relativeUrl;
            var request = HttpWebRequestFactory.Create(u.Uri);
            request.Method = HttpWebRequestMethod.POST;
            request.ContentType = "application/x-www-form-urlencoded";

            var param = new List<KeyValuePair<string, string>>();
            if (parameters != null)
                param.AddRange(parameters);

            param.Add(KV.Create("wt", "xml"));
            var qs = string.Join("&", param
                  .Select(kv => string.Format("{0}={1}", WebUtility.UrlEncode(kv.Key), WebUtility.UrlEncode(kv.Value)))
                  .ToArray());

            request.ContentLength = Encoding.UTF8.GetByteCount(qs);
            request.ProtocolVersion = HttpVersion.Version11;
            request.KeepAlive = true;

            return (request, qs);
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
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

        /// <inheritdoc />
        public string PostStream(string relativeUrl, string contentType, System.IO.Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            return conn.PostStream(relativeUrl, contentType, content, getParameters);
        }

        /// <inheritdoc />
        public Task<string> PostStreamAsync(string relativeUrl, string contentType, System.IO.Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            return conn.PostStreamAsync(relativeUrl, contentType, content, getParameters);
        }

    }
}
