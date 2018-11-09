using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SolrNet.Exceptions;
using SolrNet.Utils;

namespace SolrNet.Impl
{
    public class AutoSolrConnection : IStreamSolrConnection, IDisposable
    {
        private const string version = "2.2";

        public AutoSolrConnection(string serverUrl) : this(serverUrl, credentials: null)
        {


        }

        public AutoSolrConnection(string serverUrl, ICredentials credentials) : this(serverUrl, httpClient: null)
        {
            var httpClientHandler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                Credentials = credentials
            };

            this.HttpClient = new HttpClient(httpClientHandler);

        }


        public AutoSolrConnection(string serverUrl, HttpClient httpClient)
        {
            this.SyncFallbackConnection = new PostSolrConnection(new SolrConnection(serverUrl), serverUrl);
            this.ServerURL = Utils.UriValidator.ValidateHTTP(serverUrl);
            this.HttpClient = httpClient;


        }

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

        public SolrQueryResponse Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters) => SyncFallbackConnection.Get(relativeUrl, parameters);

        public async Task<SolrQueryResponse> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryParameters = parameters?.ToList();
            using (var responseStream = await GetAsStreamAsync(relativeUrl, queryParameters, cancellationToken))
            using (var sr = new StreamReader(responseStream))
            {
                var response = new SolrQueryResponse(await sr.ReadToEndAsync());
                response.MetaData.OriginalQuery = QueryBuilder.GetQuery(queryParameters);
                return response;
            }
        }


        public async Task<Stream> GetAsStreamAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken)
        {
            var u = new UriBuilder(ServerURL);
            u.Path += relativeUrl;
            var queryParameters = parameters?.ToList();
            u.Query = QueryBuilder.GetQuery(queryParameters, version);

            HttpResponseMessage response;
            if (u.Uri.ToString().Length > MaxUriLength)
            {
                u.Query = null;
                response = await HttpClient.PostAsync(u.Uri, new FormUrlEncodedContent(queryParameters), cancellationToken);
            }
            else
                response = await HttpClient.GetAsync(u.Uri, cancellationToken);

            if (!response.IsSuccessStatusCode)
                throw new SolrConnectionException($"{response.StatusCode}: {response.ReasonPhrase}", null, u.Uri.ToString());

            return await response.Content.ReadAsStreamAsync();

        }
        public SolrQueryResponse Post(string relativeUrl, string s) => SyncFallbackConnection.Post(relativeUrl, s);

        public Task<SolrQueryResponse> PostAsync(string relativeUrl, string s) => PostAsync(relativeUrl, s, CancellationToken.None);

        public async Task<SolrQueryResponse> PostAsync(string relativeUrl, string s, CancellationToken cancellationToken)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            using (var content = new MemoryStream(bytes))
            {
                using (var responseStream = await PostStreamAsStreamAsync(relativeUrl, "text/xml; charset=utf-8", content, null, cancellationToken))
                using (var sr = new StreamReader(responseStream))
                {
                    var response = new SolrQueryResponse(await sr.ReadToEndAsync());
                    response.MetaData.OriginalQuery = s;
                    return response;
                }
            }
        }

        public SolrQueryResponse PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters) => SyncFallbackConnection.PostStream(relativeUrl, contentType, content, getParameters);

        public async Task<SolrQueryResponse> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            var queryParameters = getParameters?.ToList();
            using (var responseStream = await PostStreamAsStreamAsync(relativeUrl, contentType, content, queryParameters, CancellationToken.None))
            using (var sr = new StreamReader(responseStream))
            {
                var response = new SolrQueryResponse(await sr.ReadToEndAsync());
                response.MetaData.OriginalQuery = QueryBuilder.GetQuery(queryParameters, version);
                return response;
            }
        }


        public async Task<Stream> PostStreamAsStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters, CancellationToken cancellationToken)
        {
            var u = new UriBuilder(ServerURL);
            u.Path += relativeUrl;
            u.Query = QueryBuilder.GetQuery(getParameters, version);

            var sc = new StreamContent(content);
            sc.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(contentType);

            var response = await HttpClient.PostAsync(u.Uri, sc);

            if (!response.IsSuccessStatusCode)
                throw new SolrConnectionException($"{response.StatusCode}: {response.ReasonPhrase}", null, u.Uri.ToString());

            return await response.Content.ReadAsStreamAsync();
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


        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        #endregion
    }
}
