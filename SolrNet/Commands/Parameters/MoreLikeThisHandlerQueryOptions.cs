using System.Collections.Generic;
using SolrNet.Impl;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Query options for MoreLikeThis handler
    /// See http://wiki.apache.org/solr/MoreLikeThisHandler
    /// </summary>
    public class MoreLikeThisHandlerQueryOptions: CommonQueryOptions {
        /// <summary>
        /// Query options for MoreLikeThis handler
        /// See http://wiki.apache.org/solr/MoreLikeThisHandler
        /// </summary>
        public MoreLikeThisHandlerQueryOptions(MoreLikeThisHandlerParameters parameters) {
            Parameters = parameters;
        }

        /// <summary>
        /// Parameters for MoreLikeThis handler
        /// </summary>
        public MoreLikeThisHandlerParameters Parameters { get; set; }

        /// <summary>
        /// Request body content - eg. JSON query. Ignored by SolrQueryExecuter;
        /// use SolrPostQueryExecuter (or similar) to POST a query body.
        /// </summary>
        public ISolrQueryBody QueryBody { get; set; }

    }
}
