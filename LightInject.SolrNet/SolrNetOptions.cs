using System.Net;
using System.Net.Http;
using HttpWebAdapters;

namespace LightInject.SolrNet
{
    public class SolrNetOptions
    {
        public SolrNetOptions() : this(new HttpClient())
        {
        }

        public SolrNetOptions(HttpClient httpClient)
        {
            this.HttpClient = httpClient;
        }

        /// <summary>
        /// Gets the HttpClient with which SolrNet connects to the Solr server. This is the place to add Default Headers for Basic Authentication for example..
        /// </summary>
        public HttpClient HttpClient { get; }
           
    }
}
