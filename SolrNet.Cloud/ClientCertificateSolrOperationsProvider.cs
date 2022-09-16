using System;
using HttpWebAdapters;

namespace SolrNet.Cloud
{
    /// <summary>
    /// For using client certificates with Solr.Cloud
    /// </summary>
    public class ClientCertificateSolrOperationsProvider : ISolrOperationsProvider
    {
        private readonly IHttpWebRequestFactory _requestFactory;
        private readonly int _timeOutSeconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestFactory">The request factory</param>
        /// <param name="timeOutSeconds">the timeout in seconds</param>
        public ClientCertificateSolrOperationsProvider(IHttpWebRequestFactory requestFactory, int timeOutSeconds)
        {
            _requestFactory = requestFactory;
            _timeOutSeconds = timeOutSeconds;
        }


        public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false)
        {
            var connection = new Impl.SolrConnection(url);
            connection.HttpWebRequestFactory = _requestFactory;
            connection.Timeout = (int)TimeSpan.FromSeconds(_timeOutSeconds).TotalMilliseconds;

            if (!isPostConnection)
            {
                return SolrNet.GetBasicServer<T>(connection);
            }
            else
            {
                var postConnection = new Impl.PostSolrConnection(connection, url);
                postConnection.HttpWebRequestFactory = _requestFactory;
                return SolrNet.GetBasicServer<T>(postConnection);
            }
        }



        public ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false)
        {
            var connection = new Impl.SolrConnection(url);
            connection.HttpWebRequestFactory = _requestFactory;
            connection.Timeout = (int)TimeSpan.FromSeconds(_timeOutSeconds).TotalMilliseconds;

            if (!isPostConnection)
            {
                return SolrNet.GetServer<T>(connection);
            }
            else
            {
                var postConnection = new Impl.PostSolrConnection(connection, url);
                postConnection.HttpWebRequestFactory = _requestFactory;
                return SolrNet.GetServer<T>(postConnection);
            }
        }
    }
}
