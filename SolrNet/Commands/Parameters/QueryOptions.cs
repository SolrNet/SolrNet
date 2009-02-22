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

        public SpellCheckingParameters SpellCheck { get; set; }

        /// <summary>
        /// This parameter can be used to specify a query that can be used to restrict the super set of documents that can be returned, without influencing score. 
        /// It can be very useful for speeding up complex queries since the queries specified with fq are cached independently from the main query. 
        /// This assumes the same Filter is used again for a latter query (i.e. there's a cache hit)
        /// </summary>
        public ICollection<ISolrQuery> FilterQueries { get; set; }

	    public QueryOptions() {
			OrderBy = new List<SortOrder>();
		}
	}
}