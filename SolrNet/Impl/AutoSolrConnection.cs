using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpWebAdapters;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet.Impl
{
    /// <summary>
    /// Manages asynchronous and synchronous connections with Solr.
    /// </summary>
    public class AutoSolrConnection : IStreamSolrConnection, IDisposable
    {
        private const string version = "2.2";

        /// <summary>
        /// Manages HTTP connection with Solr
        /// </summary>
        /// <param name="serverUrl">URL to Solr</param>
        public AutoSolrConnection(string serverUrl) : this(serverUrl, credentials: null)
        {
        }

        /// <summary>
        /// Manages HTTP connection with Solr
        /// 
        /// Note that this constructor uses <see cref="HttpWebAdapters.HttpWebRequestFactory"/> and thus doesn't support basic authentication and such
        /// Please use <see cref="AutoSolrConnection(string, ICredentials, IHttpWebRequestFactory)"/>
        /// </summary>
        /// <param name="serverUrl">URL to Solr</param>
        /// <param name="credentials">Credentials to be used in asynchronous calls</param>
        public AutoSolrConnection(string serverUrl, ICredentials credentials) : this(serverUrl, credentials, new HttpWebRequestFactory())
        {
        }

        /// <summary>
        /// Manages HTTP connection with Solr
        /// </summary>
        /// <param name="serverUrl">URL to Solr</param>
        /// <param name="credentials">Credentials to be used in asynchronous calls</param>
        /// <param name="httpWebRequestFactory">Request factory to be used in synchronous fallback connections</param>
        public AutoSolrConnection(string serverUrl, ICredentials credentials, IHttpWebRequestFactory httpWebRequestFactory)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Credentials = credentials
            };

            this.HttpClient = new HttpClient(httpClientHandler);

            this.SyncFallbackConnection = new PostSolrConnection(new SolrConnection(serverUrl, httpWebRequestFactory), serverUrl, httpWebRequestFactory);
            this.ServerURL = Utils.UriValidator.ValidateHTTP(serverUrl);
        }

        /// <summary>
        /// Manages HTTP connection with Solr
        /// 
        /// Note that this constructor uses <see cref="HttpWebAdapters.HttpWebRequestFactory"/> and thus doesn't support basic authentication and such
        /// Please use <see cref="AutoSolrConnection(string, HttpClient, IHttpWebRequestFactory)"/>
        /// </summary>
        /// <param name="serverUrl">URL to Solr</param>
        /// <param name="httpClient">HttpClient used in asynchronous connections</param>
        public AutoSolrConnection(string serverUrl, HttpClient httpClient) : this(serverUrl, httpClient, new HttpWebRequestFactory())
        {
        }

        /// <summary>
        /// Manages HTTP connection with Solr
        /// </summary>
        /// <param name="serverUrl">URL to Solr</param>
        /// <param name="httpClient">HttpClient used in asynchronous connections</param>
        /// <param name="httpWebRequestFactory">Request factory to be used in synchronous fallback connections</param>
        public AutoSolrConnection(string serverUrl, HttpClient httpClient, IHttpWebRequestFactory httpWebRequestFactory)
        {
            this.SyncFallbackConnection = new PostSolrConnection(new SolrConnection(serverUrl, httpWebRequestFactory), serverUrl, httpWebRequestFactory);
            this.ServerURL = Utils.UriValidator.ValidateHTTP(serverUrl);
            this.HttpClient = httpClient;
        }

        /// <summary>
        /// Connection used in synchrounous calls.
        /// </summary>
        private PostSolrConnection SyncFallbackConnection { get; }

        /// <summary>
        /// URL to Solr
        /// </summary>
        public string ServerURL { get; }

        /// <summary>
        /// Gets the HttpClient used communicate with the Solr server.
        /// </summary>
        public HttpClient HttpClient { get; }

        /// <summary>
        /// The maximum length of the URL before switching to a POST request.
        /// </summary>
        public int MaxUriLength { get; set; } = 7600;

        /// <inheritdoc />
        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) => SyncFallbackConnection.Get(relativeUrl, parameters);

        /// <inheritdoc />
        public async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            using (var responseStream = await GetAsStreamAsync(relativeUrl, parameters, cancellationToken))
            using (var sr = new StreamReader(responseStream))
            {
                return await sr.ReadToEndAsync();
            }
        }
        
        /// <inheritdoc />
        public async Task<Stream> GetAsStreamAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken)
        {
            var u = new UriBuilder(ServerURL);
            u.Path += relativeUrl;
            u.Query = GetQuery(parameters);

            HttpResponseMessage response;
            if (UriValidator.UriLength(u) > MaxUriLength)
            {
                u.Query = null;
                response = await HttpClient.PostAsync(u.Uri, new FormUrlEncodedContent(parameters), cancellationToken);
            }
            else
                response = await HttpClient.GetAsync(u.Uri, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new SolrConnectionException(await response.Content.ReadAsStringAsync(), null, u.Uri.ToString());

            return await response.Content.ReadAsStreamAsync();

        }
        
        /// <inheritdoc />
        public string Post(string relativeUrl, string s) => SyncFallbackConnection.Post(relativeUrl, s);

        /// <inheritdoc />
        public Task<string> PostAsync(string relativeUrl, string s) => PostAsync(relativeUrl, s, CancellationToken.None);

        /// <inheritdoc />
        public async Task<string> PostAsync(string relativeUrl, string s, CancellationToken cancellationToken)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var content = new MemoryStream(bytes))
            {
                using (var responseStream = await PostStreamAsStreamAsync(relativeUrl, "text/xml; charset=utf-8", content, null, cancellationToken))
                using (var sr = new StreamReader(responseStream))
                {
                    return await sr.ReadToEndAsync();
                }
            }
        }

        /// <inheritdoc />
        public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters) => SyncFallbackConnection.PostStream(relativeUrl, contentType, content, getParameters);

        /// <inheritdoc />
        public async Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            using (var responseStream = await PostStreamAsStreamAsync(relativeUrl, contentType, content, getParameters, CancellationToken.None))
            using (var sr = new StreamReader(responseStream))
            {
                return await sr.ReadToEndAsync();
            }
        }


        /// <inheritdoc />
        public async Task<Stream> PostStreamAsStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters, CancellationToken cancellationToken)
        {
            var u = new UriBuilder(ServerURL);
            u.Path += relativeUrl;
            u.Query = GetQuery(getParameters);

            var sc = new StreamContent(content);
            if(contentType != null)
                sc.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);

            var response = await HttpClient.PostAsync(u.Uri, sc);

            if (!response.IsSuccessStatusCode)
                throw new SolrConnectionException(await response.Content.ReadAsStringAsync(), null, u.Uri.ToString());

            return await response.Content.ReadAsStreamAsync();
        }


        private string GetQuery(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var param = new List<KeyValuePair<string, string>>();
            if (parameters != null)
                param.AddRange(parameters);

            param.Add(new KeyValuePair<string, string>("version", version));
            param.Add(new KeyValuePair<string, string>("wt", "xml"));

            return string.Join("&", param.Select(kv => $"{WebUtility.UrlEncode(kv.Key)}={WebUtility.UrlEncode(kv.Value)}"));

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    HttpClient.Dispose();
                }
                disposedValue = true;
            }
        }


        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
