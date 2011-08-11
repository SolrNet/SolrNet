#region license
// Copyright (c) 2007-2010 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Collections.Generic;

namespace SolrNet.Commands.Parameters {
    /// <summary>
    /// Query options
    /// </summary>
	public partial class QueryOptions {

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

		/// <summary>
		/// Sort order.
		/// By default, it's "score desc"
		/// </summary>
		public ICollection<SortOrder> OrderBy { get; set; }

		/// <summary>
		/// Highlighting parameters
		/// </summary>
		public HighlightingParameters Highlight { get; set; }

        /// <summary>
        /// Facet parameters
        /// </summary>
        public FacetParameters Facet { get; set; }

        /// <summary>
        /// Spell-checking parameters
        /// </summary>
        public SpellCheckingParameters SpellCheck { get; set; }

        /// <summary>
        /// Terms parameters
        /// </summary>
        public TermsParameters Terms { get; set; }

        /// <summary>
        /// More-like-this parameters
        /// </summary>
        public MoreLikeThisParameters MoreLikeThis { get; set; }

        /// <summary>
        /// This parameter can be used to specify a query that can be used to restrict the super set of documents that can be returned, without influencing score. 
        /// It can be very useful for speeding up complex queries since the queries specified with fq are cached independently from the main query. 
        /// This assumes the same Filter is used again for a latter query (i.e. there's a cache hit)
        /// </summary>
        public ICollection<ISolrQuery> FilterQueries { get; set; }

        /// <summary>
        /// This parameter can be used to return the stats for a specific query on top of the results that are normally returned.  Included in the stats are
        /// min, max, sum, count, missing, sumOfSquares, mean, and stddev values.  
        /// </summary>
        public StatsParameters Stats { get; set; }

        /// <summary>
        /// This parameter can be used to collapse - or group - documents by the unique values of a specified field. Included in the results are the number of
        /// records by document key and by field value
        /// </summary>
        public CollapseParameters Collapse { get; set; }

		/// <summary>
		/// (only SOLR 4.0)
		/// This parameter can be used to collapse - or group - documents by the unique values of a specified field. Included in the results are the number of
		/// records by document key and by field value
		/// </summary>
		public GroupingParameters Grouping { get; set; }


        /// <summary>
        /// This parmeter can be used to group query results into clusters based on document similarity 
        /// </summary>
        public ClusteringParameters Clustering { get; set; }

        /// <summary>
        /// Extra arbitrary parameters to be passed in the request querystring
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtraParams { get; set; }

	    public QueryOptions() {
			OrderBy = new List<SortOrder>();
	        Fields = new List<string>();
	        FilterQueries = new List<ISolrQuery>();
            Facet = new FacetParameters();
	        ExtraParams = new Dictionary<string, string>();
		}
	}
}