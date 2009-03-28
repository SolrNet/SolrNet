#region license
// Copyright (c) 2007-2009 Mauricio Scheffer
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
	public class QueryOptions {

	    /// <summary>
		/// Fields to retrieve.
		/// By default, all stored fields are returned
		/// </summary>
		public ICollection<string> Fields { get; set; }

		/// <summary>
		/// Collection of facet queries
		/// </summary>
		public ICollection<ISolrFacetQuery> FacetQueries { get; set; }

        /// <summary>
        /// Limits the terms on which to facet to those starting with the given string prefix.
        /// </summary>
        public string FacetPrefix { get; set; }

        /// <summary>
        /// Set to true, this parameter indicates that constraints should be sorted by their count. 
        /// If false, facets will be in their natural index order (unicode). 
        /// For terms in the ascii range, this will be alphabetically sorted. 
        /// The default is true if Limit is greater than 0, false otherwise.
        /// </summary>
        public bool? FacetSort { get; set; }

        /// <summary>
        /// This param indicates the maximum number of constraint counts that should be returned for the facet fields. 
        /// A negative value means unlimited. 
        /// The default value is 100. 
        /// </summary>
        public int? FacetLimit { get; set; }

        /// <summary>
        /// This param indicates an offset into the list of constraints to allow paging. 
        /// The default value is 0. 
        /// </summary>
        public int? FacetOffset { get; set; }

        /// <summary>
        /// This param indicates the minimum counts for facet fields should be included in the response.
        /// The default value is 0.
        /// </summary>
        public int? FacetMinCount { get; set; }

        /// <summary>
        /// Set to true this param indicates that in addition to the Term based constraints of a facet field, a count of all matching results which have no value for the field should be computed
        /// Default is false
        /// </summary>
        public bool? FacetMissing { get; set; }

        /// <summary>
        /// This param indicates the minimum document frequency (number of documents matching a term) for which the filterCache should be used when determining the constraint count for that term. 
        /// This is only used during the term enumeration method of faceting (field type faceting on multi-valued or full-text fields).
        /// A value greater than zero will decrease memory usage of the filterCache, but increase the query time. 
        /// When faceting on a field with a very large number of terms, and you wish to decrease memory usage, try a low value of 25 to 50 first.
        /// The default value is 0, causing the filterCache to be used for all terms in the field.
        /// </summary>
        public int? FacetEnumCacheMinDf { get; set; }

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
        /// Spell-checking parameters
        /// </summary>
        public SpellCheckingParameters SpellCheck { get; set; }

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
        /// Extra arbitrary parameters to be passed in the request querystring
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtraParams { get; set; }

	    public QueryOptions() {
			OrderBy = new List<SortOrder>();
	        ExtraParams = new Dictionary<string, string>();
		}
	}
}