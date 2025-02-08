using HttpWebAdapters;

namespace SolrNet.Cloud
{
    /// <summary>
    /// For using client certificates with Solr.Cloud
    /// </summary>
    public class ClientCertificateSolrOperationsProvider : ISolrOperationsProvider
    {
        private readonly IHttpWebRequestFactory _requestFactory;
        private readonly int _timeOutInMilliseconds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestFactory">The request factory</param>
        /// <param name="timeOutInMilliseconds">The timeout in milliseconds</param>
        public ClientCertificateSolrOperationsProvider(IHttpWebRequestFactory requestFactory, int timeOutInMilliseconds)
        {
            _requestFactory = requestFactory;
            _timeOutInMilliseconds = timeOutInMilliseconds;
        }


        public ISolrBasicOperations<T> GetBasicOperations<T>(string url, bool isPostConnection = false)
        {
            var connection = GetConnection(url, isPostConnection);
            return SolrNet.GetBasicServer<T>(connection);
        }


        public ISolrOperations<T> GetOperations<T>(string url, bool isPostConnection = false)
        {
            var connection = GetConnection(url, isPostConnection);
            return SolrNet.GetServer<T>(connection);
        }


        private ISolrConnection GetConnection(string url, bool isPostConnection)
        {
            var connection = new Impl.SolrConnection(url);
            connection.HttpWebRequestFactory = _requestFactory;
            connection.Timeout = _timeOutInMilliseconds;

            if (isPostConnection)
            {
                var postConnection = new Impl.PostSolrConnection(connection, url);
                postConnection.HttpWebRequestFactory = _requestFactory;
                postConnection.Timeout = _timeOutInMilliseconds;
                return postConnection;
            }

            return connection;
        }

    }
}
