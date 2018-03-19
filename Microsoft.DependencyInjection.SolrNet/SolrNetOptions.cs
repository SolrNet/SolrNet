using System.Net.Http;

namespace SolrNet
{
    public class SolrNetOptions
    {
        public SolrNetOptions(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        /// <summary>
        /// Gets the HttpClient with which SolrNet connects to the Solr server. This is the place to add Default Headers for Basic Authentication for example..
        /// </summary>
        public HttpClient HttpClient { get; }
    }
}
