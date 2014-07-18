using System;
using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Common, shared query options
    /// </summary>
    public class CommonQueryOptions {
        /// <summary>
        /// Common, shared query options
        /// </summary>
        public CommonQueryOptions() {
            Fields = new List<string>();
            FilterQueries = new List<ISolrQuery>();
            Facet = new FacetParameters();
            ExtraParams = new Dictionary<string, string>();
        }

        /// <summary>
        /// Fields to retrieve.
        /// By default, all stored fields are returned
        /// </summary>
        public ICollection<string> Fields { get; set; }

        /// <summary>
        /// Offset in the complete result set for the queries where the set of returned documents should begin
        /// Default is 0
        /// </summary>
        [Obsolete("Use StartOrCursor instead, setting StartOrCursor.Start")]
        public int? Start { get; set; }

        /// <summary>
        /// Starting row or cursor mark for pagination.
        /// Default is start=0
        /// </summary>
        public StartOrCursor StartOrCursor { get; set; }

        /// <summary>
        /// Maximum number of documents from the complete result set to return to the client for every request.
        /// Default is 100000000.
        /// NOTE: do not rely on this default value. In a future release the default value will be reset to the Solr default. 
        /// Always define the number of rows you want. The high value is meant to mimic a SQL query without a TOP/LIMIT clause.
        /// </summary>
        public int? Rows { get; set; }

        /// <summary>
        /// Facet parameters
        /// </summary>
        public FacetParameters Facet { get; set; }

        /// <summary>
        /// This parameter can be used to specify a query that can be used to restrict the super set of documents that can be returned, without influencing score. 
        /// It can be very useful for speeding up complex queries since the queries specified with fq are cached independently from the main query. 
        /// This assumes the same Filter is used again for a latter query (i.e. there's a cache hit)
        /// </summary>
        public ICollection<ISolrQuery> FilterQueries { get; set; }

        /// <summary>
        /// Extra arbitrary parameters to be passed in the request querystring
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtraParams { get; set; }

    }
}