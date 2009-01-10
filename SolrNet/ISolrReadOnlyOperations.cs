using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet {
    public interface ISolrReadOnlyOperations<T> {
        /// <summary>
        /// Queries documents
        /// </summary>
        /// <param name="q">query to execute</param>
        /// <returns>query results</returns>
        ISolrQueryResults<T> Query(string q);

        ISolrQueryResults<T> Query(string q, ICollection<SortOrder> orders);
        ISolrQueryResults<T> Query(string q, QueryOptions options);
        ISolrQueryResults<T> Query(ISolrQuery q);
        ISolrQueryResults<T> Query(ISolrQuery query, ICollection<SortOrder> orders);
        ISolrQueryResults<T> Query(ISolrQuery query, QueryOptions options);

        /// <summary>
        /// Pings the Solr server.
        /// It can be used by a load balancer in front of a set of Solr servers to check response time of all the Solr servers in order to do response time based load balancing.
        /// See <see cref="http://wiki.apache.org/solr/SolrConfigXml"/> for more information.
        /// </summary>
        string Ping();
    }
}