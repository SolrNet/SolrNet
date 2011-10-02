using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SolrNet.Commands.Parameters
{
    public class MoreLikeThisHandlerQueryOptions
    {
        public MoreLikeThisHandlerQueryOptions()
        {
            this.Fields = new List<string>();
            this.FilterQueries = new List<ISolrQuery>();
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
        public int? Start { get; set; }

        /// <summary>
        /// Maximum number of documents from the complete result set to return to the client for every request.
        /// Default is 10
        /// </summary>
        public int? Rows { get; set; }

        public MoreLikeThisHandlerParameters Parameters { get; set; }

        /// <summary>
        /// This parameter can be used to specify a query that can be used to restrict the super set of documents that can be returned, without influencing score. 
        /// It can be very useful for speeding up complex queries since the queries specified with fq are cached independently from the main query. 
        /// This assumes the same Filter is used again for a latter query (i.e. there's a cache hit)
        /// </summary>
        public ICollection<ISolrQuery> FilterQueries { get; set; }

        /// <summary>
        /// Facet parameters
        /// </summary>
        public FacetParameters Facet { get; set; }

        
    }
}
