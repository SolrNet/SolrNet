using System.Collections.Generic;
using SolrNet.Commands.Parameters;

namespace SolrNet {
    public interface ISolrReadOnlyOperations<T> : ISolrBasicReadOnlyOperations<T> {
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
    }
}