using SolrNet.Commands.Parameters;

namespace SolrNet {
    public interface ISolrBasicReadOnlyOperations<T> {
        ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options);

        /// <summary>
        /// Pings the Solr server.
        /// It can be used by a load balancer in front of a set of Solr servers to check response time of all the Solr servers in order to do response time based load balancing.
        /// See <see cref="http://wiki.apache.org/solr/SolrConfigXml"/> for more information.
        /// </summary>
        void Ping();
    }
}