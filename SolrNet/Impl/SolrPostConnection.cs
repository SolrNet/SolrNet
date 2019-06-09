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
        public string ServerUrl
        {
            get { return serverUrl; }
        }

        public SolrQueryResponse Post(string relativeUrl, string s)
        {
            return conn.Post(relativeUrl, s);
        }

        public Task<SolrQueryResponse> PostAsync(string relativeUrl, string s)
        {
            return conn.PostAsync(relativeUrl, s);
        }

        public (HttpWebRequest request, string queryString) PrepareGet(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var u = new UriBuilder(serverUrl);
            u.Path += relativeUrl;
            var request = (HttpWebRequest)WebRequest.Create(u.Uri);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var qs = QueryBuilder.GetQuery(parameters);
            request.ContentLength = Encoding.UTF8.GetByteCount(qs);
            request.ProtocolVersion = HttpVersion.Version11;
            request.KeepAlive = true;

            return (request, qs);
        }

        public SolrQueryResponse Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var queryParameters = parameters?.ToList();
            var g = PrepareGet(relativeUrl, queryParameters);
            try
            {
                using (var postParams = g.request.GetRequestStream())
                using (var sw = new StreamWriter(postParams))
                    sw.Write(g.queryString);
                using (var response = g.request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var sr = new StreamReader(responseStream, Encoding.UTF8, true))
                {
                    var solrResponse = new SolrQueryResponse(sr.ReadToEnd());
                    solrResponse.MetaData.OriginalQuery = g.queryString;
                    return solrResponse;
                }
            }
            catch (WebException e)
            {
                throw new SolrConnectionException(e);
            }
        }

        public async Task<SolrQueryResponse> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            var queryParameters = parameters?.ToList();
            var g = PrepareGet(relativeUrl, queryParameters);
            try
            {
                using (var postParams = await g.request.GetRequestStreamAsync())
                using (var sw = new StreamWriter(postParams))
                    await sw.WriteAsync(g.queryString);
                using (var response = await g.request.GetResponseAsync())
                using (var responseStream = response.GetResponseStream())
                using (var sr = new StreamReader(responseStream, Encoding.UTF8, true))
                {
                    var solrResponse = new SolrQueryResponse(await sr.ReadToEndAsync());
                    solrResponse.MetaData.OriginalQuery = g.queryString;
                    return solrResponse;
                }
            }
            catch (WebException e)
            {
                throw new SolrConnectionException(e);
            }
        }

     public SolrQueryResponse PostStream(string relativeUrl, string contentType, System.IO.Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
     {
			return conn.PostStream(relativeUrl, contentType, content, getParameters);
		}
        public Task<SolrQueryResponse> PostStreamAsync(string relativeUrl, string contentType, System.IO.Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            return conn.PostStreamAsync(relativeUrl, contentType, content, getParameters);
        }

    }
}
